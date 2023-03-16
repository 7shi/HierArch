using System;
using System.Drawing.Printing;
using System.IO;
using System.Xml;
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

		public void ReadPageSettings(XmlReader xr)
		{
			PageSettings pset = this.PrintDocument.DefaultPageSettings;
			string name = xr.Name;
			while (xr.Read())
			{
				if (xr.Name == name && xr.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				else if (xr.Name == "PaperSize" && xr.NodeType == XmlNodeType.Element)
				{
					try
					{
						pset.PaperSize = this.Serializer.ReadPaperSize(xr, this.PrintDocument.PrinterSettings);
					}
					catch
					{
					}
				}
				else if (xr.Name == "Landscape" && xr.NodeType == XmlNodeType.Element)
				{
					pset.Landscape = this.Serializer.ReadBoolean(xr);
				}
				else if (xr.Name == "Margins" && xr.NodeType == XmlNodeType.Element)
				{
					pset.Margins = this.Serializer.ReadMargins(xr);
				}
			}
		}

		public virtual bool Save()
		{
			this.Changed = false;
			return true;
		}

		public void WritePageSettings(XmlWriter xw)
		{
			PageSettings pset = this.PrintDocument.DefaultPageSettings;
			PaperSize psz;
			try
			{
				psz = pset.PaperSize;
			}
			catch
			{
				return;
			}
			xw.WriteStartElement("PageSettings");
			this.Serializer.WritePaperSize(xw, "PaperSize", psz);
			this.Serializer.WriteBoolean(xw, "Landscape", pset.Landscape);
			///this.Serializer.WriteMargins(xw, "Margins", pset.Margins);
			xw.WriteEndElement();
		}
	}
}
