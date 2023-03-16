// このファイルは Document.hacls から生成されています。

using System;
using System.IO;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// 文書を管理するクラスです。
	/// </summary>
	public class Document
	{
		public string FullName;
		public bool Changed;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public Document()
		{
			this.FullName = "";
			this.Changed  = false;
		}

		public string Name
		{
			get
			{
				if(this.FullName == "") return "無題";
				return new FileInfo(this.FullName).Name;
			}
		}

		public virtual bool Open()
		{
			this.Changed = false;
			return true;
		}

		public virtual bool Save()
		{
			this.Changed = false;
			return true;
		}
	}
}
