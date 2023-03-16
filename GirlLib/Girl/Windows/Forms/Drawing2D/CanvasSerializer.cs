// ここにソースコードの注釈を書きます。

using System;
using System.Xml;
using Girl.Xml;

namespace Girl.Windows.Forms.Drawing2D
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class CanvasSerializer : XmlObjectSerializer
	{
		public override void Write(XmlWriter xw, string name, object obj)
		{
			if (obj == null) return;
			string objname = obj.GetType().Name;
			if (name == null) name = objname;
			if (obj is CanvasObject)
			{
				xw.WriteStartElement(name);
				(obj as CanvasObject).WriteXml(this, xw);
				xw.WriteEndElement();
				return;
			}
			base.Write(xw, name, obj);
		}

		public override object Read(XmlReader xr)
		{
			if (xr.NodeType != XmlNodeType.Element) return null;
			CanvasObject co = null;
			switch (xr.Name)
			{
				case "CanvasLine":
				co = new CanvasLine();
				break;
				case "CanvasLines":
				co = new CanvasLines();
				break;
				case "CanvasPolygon":
				co = new CanvasPolygon();
				break;
				case "CanvasRectangle":
				co = new CanvasRectangle();
				break;
				case "CanvasEllipse":
				co = new CanvasEllipse();
				break;
				case "CanvasString":
				co = new CanvasString();
				break;
				case "CanvasLineEx":
				co = new CanvasLineEx();
				break;
			}
			if (co != null)
			{
				co.ReadXml(this, xr);
				return co;
			}
			return base.Read(xr);
		}
	}
}
