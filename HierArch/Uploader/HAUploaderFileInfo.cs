// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.IO;
using System.Net;
using System.Text;

namespace Girl.HierArch
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class HAUploaderFileInfo
	{
		public HAUploaderInfo UploaderInfo;
		public string Name;
		public long Length;
		public DateTime LastWriteTime;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAUploaderFileInfo(HAUploaderInfo uploaderInfo, string[] data)
		{
			this.UploaderInfo = uploaderInfo;
			this.Name = data[0];
			this.Length = Convert.ToInt64(data[2]);
			this.SetTime(data[1]);
			uploaderInfo.Files.Add(this.Name, this);
		}

		public HAUploaderFileInfo(HAUploaderInfo uploaderInfo, string fileName)
		{
			this.UploaderInfo = uploaderInfo;
			this.Name = Path.GetFileName(fileName);
			this.Length = new FileInfo(fileName).Length;
			this.LastWriteTime = DateTime.Now;
			uploaderInfo.Files.Add(this.Name, this);
		}

		public void SetTime(string time)
		{
			this.LastWriteTime = new DateTime(Convert.ToInt32(time.Substring(0, 4)), Convert.ToInt32(time.Substring(4, 2)), Convert.ToInt32(time.Substring(6, 2)), Convert.ToInt32(time.Substring(8, 2)), Convert.ToInt32(time.Substring(10, 2)), Convert.ToInt32(time.Substring(12, 2)));
		}

		public string Upload(string fileName)
		{
			HAAccount haa = Form1.AccountManager.Get(this.UploaderInfo.Server);
			string id =(haa != null) ? haa.Id:
			"";
			string password =(haa != null) ? haa.Password:
			"";
			WebClient wc = new WebClient();
			byte [] res;
			try
			{
				string url = this.UploaderInfo.MakeUrl(string.Format("session=text&id={0}&password={1}&filename={2}", id, password, Path.GetFileName(fileName)));
				res = wc.UploadFile(url, fileName);
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
			string msg = Encoding.UTF8.GetString(res);
			return (msg.StartsWith("ERROR: ")) ? msg.Substring(7):
			null;
		}

		public byte[] DownloadData()
		{
			WebClient wc = new WebClient();
			byte [] data = null;
			try
			{
				data = wc.DownloadData(this.UploaderInfo.MakeUrl(string.Format("session=download&filename={0}", this.Name)));
			}
			catch
			{
				data = null;
			}
			return data;
		}
	}
}
