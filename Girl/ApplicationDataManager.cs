using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// アプリケーションの設定を管理します。
	/// </summary>
	public class ApplicationDataManager
	{
		public ApplicationDataManager()
		{
		}

		private string m_sDataPath = null;
		public string DataPath
		{
			get
			{
				if(m_sDataPath != null) return m_sDataPath;

				string path = Application.LocalUserAppDataPath;
				string ret = Directory.GetParent(path).FullName;
				try
				{
					Directory.Delete(path);
				}
				catch
				{
				}
				m_sDataPath = ret;
				return ret;
			}

			set
			{
				m_sDataPath = value;
			}
		}

		public object Load(string fileName, Type dataType)
		{
			object ret = null;
			XmlSerializer xs = new XmlSerializer(dataType);
			FileStream fs;
			try
			{
				fs = new FileStream(DataPath + "\\" + fileName, FileMode.Open);
			}
			catch(FileNotFoundException)
			{
				return ret;
			}
			try
			{
				ret = xs.Deserialize(fs);
			}
			catch
			{
			}
			fs.Close();
			return ret;
		}

		public void Save(string fileName, object data)
		{
			XmlSerializer xs = new XmlSerializer(data.GetType());
			StreamWriter sw = new StreamWriter(DataPath + "\\" + fileName);
			xs.Serialize(sw, data);
			sw.Close();
		}
	}
}
