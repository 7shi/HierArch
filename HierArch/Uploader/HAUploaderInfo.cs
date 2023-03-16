// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;

namespace Girl.HierArch
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class HAUploaderInfo
	{
		public string Server;
		public Hashtable Files;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAUploaderInfo(string server)
		{
			this.Server = server;
			this.Files  = null;
		}

		public string Dir()
		{
			WebClient wc = new WebClient();
			byte[] res;
			try
			{
				res = wc.DownloadData(this.MakeUrl("session=dir"));
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
			
			this.Files = new Hashtable();
			StringReader sr = new StringReader(Encoding.ASCII.GetString(res));
			string line;
			while ((line = sr.ReadLine()) != null)
			{
				if (line.Length < 1) continue;
			
				string[] data = line.Split(',');
				if (data.Length != 3) continue;
				
				new HAUploaderFileInfo(this, data);
			}
			return null;
		}

		public string MakeUrl(string query)
		{
			return this.Server + ((this.Server.IndexOf('?') < 0) ? "?" : "&") + query;
		}
	}
}
