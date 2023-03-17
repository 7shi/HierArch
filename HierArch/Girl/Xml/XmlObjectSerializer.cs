// ここにソースコードの注釈を書きます。

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Girl.Xml
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class XmlObjectSerializer
	{
		private static Hashtable resources = new Hashtable();

		public virtual void Write(XmlWriter xw, string name, object obj)
		{
			if (obj == null) return;
			string objname = obj.GetType().Name;
			if (name == null) name = objname;
			switch (objname)
			{
				case "Boolean":
				this.WriteBoolean(xw, name,(Boolean) obj);
				break;
			}
		}

		public void WriteBoolean(XmlWriter xw, string name, Boolean b)
		{
			xw.WriteStartElement(name);
			xw.WriteAttributeString("Value", XmlConvert.ToString(b));
			xw.WriteEndElement();
		}

		public virtual object Read(XmlReader xr)
		{
			if (xr.NodeType != XmlNodeType.Element) return null;
			switch (xr.Name)
			{
				case "Boolean":
				return this.ReadBoolean(xr);
			}
			return null;
		}

		public Boolean ReadBoolean(XmlReader xr)
		{
			return XmlConvert.ToBoolean(xr.GetAttribute("Value"));
		}

		public bool ReadNext(XmlReader xr)
		{
			while (xr.Read())
			{
				if (xr.NodeType != XmlNodeType.Whitespace) return true;
			}
			return false;
		}

		public string GetXml(object obj)
		{
			StringWriter sw = new StringWriter();
			XmlTextWriter xw = new XmlTextWriter(sw);
			this.Write(xw, null, obj);
			xw.Close();
			sw.Close();
			return sw.ToString();
		}
	}
}
