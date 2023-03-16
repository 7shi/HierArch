// このファイルは ..\..\..\Girl.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.IO;
using System.Text;
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

		public string LoadString(string fileName)
		{
			string path = this.DataPath;
			if (!path.EndsWith(@"\")) path += @"\";
			FileStream fs;
			try
			{
				fs = new FileStream(path + fileName, FileMode.Open);
			}
			catch
			{
				return null;
			}
			StreamReader sr = new StreamReader(fs, Encoding.UTF8);
			string ret = sr.ReadToEnd();
			sr.Close();
			fs.Close();
			return ret;
		}

		public void Save(string fileName, object data)
		{
			string path = this.DataPath;
			if (!path.EndsWith(@"\")) path += @"\";
			
			XmlSerializer xs = new XmlSerializer(data.GetType());
			StreamWriter sw = new StreamWriter(path + fileName, false, Encoding.UTF8);
			xs.Serialize(sw, data);
			sw.Close();
		}

		public void SaveString(string fileName, string text)
		{
			string path = this.DataPath;
			if (!path.EndsWith(@"\")) path += @"\";
			
			StreamWriter sw = new StreamWriter(path + fileName, false, Encoding.UTF8);
			sw.Write(text);
			sw.Close();
		}

		public static string SearchFolder(string folder)
		{
			DirectoryInfo di = new FileInfo(Application.ExecutablePath).Directory;
			
			for (; di != null && di.Exists; di = di.Parent)
			{
				string path = di.FullName;
				if (!path.EndsWith(@"\")) path += @"\";
				path += folder;
				if (Directory.Exists(path)) return path;
			}
			
			return null;
		}
	}
}
