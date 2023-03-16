// このファイルは ..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using Girl.Windows.Forms;

namespace Girl.HierArch
{
	public class HAAccount
	{
		public string Server = "", Id = "", Password = "";
	}
}

namespace Girl.HierArch
{
	/// <summary>
	/// サーバのアカウントを管理します。
	/// </summary>
	public class HAAccountManager
	{
		private HAAccount[] accounts;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAAccountManager()
		{
			this.accounts = new HAAccount[0];
		}

		public HAAccount Get(string server)
		{
			int num = this.Find(server);
			return (num >= 0) ? this.accounts[num] : null;
		}

		public void Set(string server, string id, string password)
		{
			int num = this.Find(server);
			if (num < 0)
			{
				int len = this.accounts.Length;
				HAAccount[] acs = new HAAccount[len + 1];
				for (int i = 0; i < len; i++)
				{
					acs[i] = this.accounts[i];
				}
				acs[len] = new HAAccount();
				this.accounts = acs;
				num = len;
			}
			this.accounts[num].Server   = server;
			this.accounts[num].Id       = id;
			this.accounts[num].Password = password;
		}

		public int Find(string server)
		{
			int i = 0;
			foreach (HAAccount ac in this.accounts)
			{
				if (ac.Server == server) return i;
				i++;
			}
			return -1;
		}

		public void Load(ApplicationDataManager adm)
		{
			HAAccount[] acs = adm.Load("Accounts.xml", typeof(HAAccount[])) as HAAccount[];
			this.accounts = (acs != null) ? acs : new HAAccount[0];
		}

		public void Save(ApplicationDataManager adm)
		{
			adm.Save("Accounts.xml", this.accounts);
		}
	}
}
