// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Girl.HierArch
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class HANodeProperty
	{
		protected HATreeNode node;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HANodeProperty(HATreeNode node)
		{
			this.node = node;
		}

		public string Server
		{
			get
			{
				return this.node.Server;
			}

			set
			{
				this.node.Server = value;
			}
		}

		public string Id
		{
			get
			{
				HAAccount haa = Form1.AccountManager.Get(this.Server);
				return (haa != null) ? haa.Id : "";
			}

			set
			{
				Form1.AccountManager.Set(this.Server, value, this.Id);
			}
		}

		public string Password
		{
			get
			{
				HAAccount haa = Form1.AccountManager.Get(this.Server);
				return (haa != null) ? haa.Password : "";
			}

			set
			{
				Form1.AccountManager.Set(this.Server, this.Id, value);
			}
		}

		public DateTime LastModified
		{
			get
			{
				return this.node.LastModified;
			}

			set
			{
				this.node.LastModified = value;
			}
		}

		public string TargetFileName
		{
			get
			{
				return this.node.TargetFileName;
			}
		}
	}
}
