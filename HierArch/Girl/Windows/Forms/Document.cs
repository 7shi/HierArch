using System;
using System.Drawing.Printing;
using System.IO;
using Girl.Xml;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// 文書を管理するクラスです。
	/// </summary>
	public class Document
	{
		public string FullName;
		public bool Changed;
		public PrintDocument PrintDocument;
		public XmlObjectSerializer Serializer;

		protected virtual void Init()
		{
			this.Changed = false;
			this.PrintDocument = new PrintDocument();
			this.Serializer = this.NewSerializer;
		}

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public Document()
		{
			this.FullName = "";
			this.Init();
		}

		public string Name
		{
			get
			{
				if (this.FullName == "") return "無題";
				return Path.GetFileNameWithoutExtension(this.FullName);
			}
		}

		protected virtual XmlObjectSerializer NewSerializer
		{
			get
			{
				return new XmlObjectSerializer();
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
