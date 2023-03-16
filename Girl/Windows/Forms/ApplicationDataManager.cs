// このファイルは ..\..\..\Girl.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

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
		private string dataPath;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public ApplicationDataManager()
		{
			this.dataPath = null;
		}

		public string DataPath
		{
			get
			{
				if(this.dataPath != null) return this.dataPath;
				
				string path = Application.LocalUserAppDataPath;
				string ret = Directory.GetParent(path).FullName;
				try
				{
					Directory.Delete(path);
				}
				catch
				{
				}
				this.dataPath = ret;
				return ret;
			}

			set
			{
				this.dataPath = value;
			}
		}

		public object Load(string fileName, Type dataType)
		{
			object ret = null;
			XmlSerializer xs = new XmlSerializer(dataType);
			string path = this.DataPath;
			if (!path.EndsWith(@"\")) path += @"\";
			FileStream fs;
			try
			{
				fs = new FileStream(path + fileName, FileMode.Open);
			}
			catch
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
			StreamWriter sw = new StreamWriter(DataPath + @"\" + fileName);
			xs.Serialize(sw, data);
			sw.Close();
		}
	}
}
