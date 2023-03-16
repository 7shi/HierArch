// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.Collections;

namespace Girl.HierArch
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class HAUploaderManager
	{
		public Hashtable Servers;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAUploaderManager()
		{
			this.Servers = new Hashtable();
		}

		public HAUploaderInfo GetInfo(string server)
		{
			if (this.Servers.Contains(server))
			{
				return this.Servers[server] as HAUploaderInfo;
			}
			HAUploaderInfo ret = new HAUploaderInfo(server);
			this.Servers[server] = ret;
			return ret;
		}
	}
}
