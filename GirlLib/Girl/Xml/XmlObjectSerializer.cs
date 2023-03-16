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
using Girl.Drawing;

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
				return;
				case "RectangleF":
				this.WriteRectangleF(xw, name,(RectangleF) obj);
				return;
				case "Pen":
				this.WritePen(xw, name, obj as Pen);
				return;
				case "SolidBrush":
				this.WriteSolidBrush(xw, name, obj as SolidBrush);
				return;
				case "Font":
				this.WriteFont(xw, name, obj as Font);
				return;
				case "AdjustableArrowCap":
				this.WriteAdjustableArrowCap(xw, name, obj as AdjustableArrowCap);
				return;
				case "Matrix":
				this.WriteMatrix(xw, name, obj as Matrix);
				return;
				case "PaperSize":
				this.WritePaperSize(xw, name, obj as PaperSize);
				return;
				case "Margins":
				this.WriteMargins(xw, name, obj as Margins);
				return;
			}
		}

		public void WriteBoolean(XmlWriter xw, string name, Boolean b)
		{
			xw.WriteStartElement(name);
			xw.WriteAttributeString("Value", XmlConvert.ToString(b));
			xw.WriteEndElement();
		}

		public void WriteRectangleF(XmlWriter xw, string name, RectangleF rect)
		{
			xw.WriteStartElement(name);
			xw.WriteAttributeString("X", XmlConvert.ToString(rect.X));
			xw.WriteAttributeString("Y", XmlConvert.ToString(rect.Y));
			xw.WriteAttributeString("Width", XmlConvert.ToString(rect.Width));
			xw.WriteAttributeString("Height", XmlConvert.ToString(rect.Height));
			xw.WriteEndElement();
		}

		public void WritePen(XmlWriter xw, string name, Pen pen)
		{
			xw.WriteStartElement(name);
			xw.WriteAttributeString("Color", XmlConvert.ToString(pen.Color.ToArgb()));
			xw.WriteAttributeString("Width", XmlConvert.ToString(pen.Width));
			if (pen.StartCap == LineCap.Custom)
			{
				xw.WriteStartElement("CustomStartCap");
				this.Write(xw, null, pen.CustomStartCap);
				xw.WriteEndElement();
			}
			if (pen.EndCap == LineCap.Custom)
			{
				xw.WriteStartElement("CustomEndCap");
				this.Write(xw, null, pen.CustomEndCap);
				xw.WriteEndElement();
			}
			if (pen.CompoundArray != null && pen.CompoundArray.Length > 0)
			{
				xw.WriteStartElement("CompoundArray");
				new XmlSerializer(typeof (float [])).Serialize(xw, pen.CompoundArray);
				xw.WriteEndElement();
			}
			xw.WriteEndElement();
		}

		public void WriteSolidBrush(XmlWriter xw, string name, SolidBrush brush)
		{
			xw.WriteStartElement(name);
			xw.WriteAttributeString("Color", XmlConvert.ToString(brush.Color.ToArgb()));
			xw.WriteEndElement();
		}

		public void WriteFont(XmlWriter xw, string name, Font font)
		{
			xw.WriteStartElement(name);
			xw.WriteAttributeString("Name", font.Name);
			xw.WriteAttributeString("Size", XmlConvert.ToString(font.Size));
			xw.WriteAttributeString("Style", font.Style.ToString());
			xw.WriteEndElement();
		}

		public void WriteAdjustableArrowCap(XmlWriter xw, string name, AdjustableArrowCap arrow)
		{
			xw.WriteStartElement(name);
			xw.WriteAttributeString("Width", XmlConvert.ToString(arrow.Width));
			xw.WriteAttributeString("Height", XmlConvert.ToString(arrow.Height));
			xw.WriteAttributeString("Filled", XmlConvert.ToString(arrow.Filled));
			xw.WriteEndElement();
		}

		public void WriteMatrix(XmlWriter xw, string name, Matrix matrix)
		{
			xw.WriteStartElement(name);
			new XmlSerializer(typeof (float [])).Serialize(xw, matrix.Elements);
			xw.WriteEndElement();
		}

		public void WritePaperSize(XmlWriter xw, string name, PaperSize size)
		{
			xw.WriteStartElement(name);
			xw.WriteAttributeString("PaperName", size.PaperName);
			xw.WriteAttributeString("Kind", size.Kind.ToString());
			xw.WriteAttributeString("Width", XmlConvert.ToString(size.Width));
			xw.WriteAttributeString("Height", XmlConvert.ToString(size.Height));
			xw.WriteEndElement();
		}

		public void WriteMargins(XmlWriter xw, string name, Margins margins)
		{
			xw.WriteStartElement(name);
			xw.WriteAttributeString("Left", XmlConvert.ToString(margins.Left));
			xw.WriteAttributeString("Right", XmlConvert.ToString(margins.Right));
			xw.WriteAttributeString("Top", XmlConvert.ToString(margins.Top));
			xw.WriteAttributeString("Bottom", XmlConvert.ToString(margins.Bottom));
			xw.WriteEndElement();
		}

		public virtual object Read(XmlReader xr)
		{
			if (xr.NodeType != XmlNodeType.Element) return null;
			switch (xr.Name)
			{
				case "Boolean":
				return this.ReadBoolean(xr);
				case "RectangleF":
				return this.ReadRectangleF(xr);
				case "Pen":
				return this.ReadPen(xr);
				case "SolidBrush":
				return this.ReadSolidBrush(xr);
				case "Font":
				return this.ReadFont(xr);
				case "AdjustableArrowCap":
				return this.ReadAdjustableArrowCap(xr);
				case "Matrix":
				return this.ReadMatrix(xr);
				case "PaperSize":
				return this.ReadPaperSize(xr);
				case "Margins":
				return this.ReadMargins(xr);
			}
			return null;
		}

		public Boolean ReadBoolean(XmlReader xr)
		{
			return XmlConvert.ToBoolean(xr.GetAttribute("Value"));
		}

		public RectangleF ReadRectangleF(XmlReader xr)
		{
			return new RectangleF((float) XmlConvert.ToDouble(xr.GetAttribute("X")),(float) XmlConvert.ToDouble(xr.GetAttribute("Y")),(float) XmlConvert.ToDouble(xr.GetAttribute("Width")),(float) XmlConvert.ToDouble(xr.GetAttribute("Height")));
		}

		public Pen ReadPen(XmlReader xr)
		{
			string name = xr.Name;
			Pen ret = new Pen(Color.FromArgb(XmlConvert.ToInt32(xr.GetAttribute("Color"))),(float) XmlConvert.ToDouble(xr.GetAttribute("Width")));
			if (!xr.IsEmptyElement)
			{
				while (xr.Read())
				{
					if (xr.Name == name && xr.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					else if (xr.Name == "CustomStartCap" && xr.NodeType == XmlNodeType.Element && this.ReadNext(xr))
					{
						CustomLineCap cap = this.Read(xr) as CustomLineCap;
						if (cap != null) ret.CustomStartCap = cap;
					}
					else if (xr.Name == "CustomEndCap" && xr.NodeType == XmlNodeType.Element && this.ReadNext(xr))
					{
						CustomLineCap cap = this.Read(xr) as CustomLineCap;
						if (cap != null) ret.CustomEndCap = cap;
					}
					else if (xr.Name == "CompoundArray" && xr.NodeType == XmlNodeType.Element && this.ReadNext(xr) && xr.Name == "ArrayOfFloat" && xr.NodeType == XmlNodeType.Element)
					{
						ret.CompoundArray = new XmlSerializer(typeof (float [])).Deserialize(xr) as float [];
					}
				}
			}
			string xml = this.GetXml(ret);
			if (resources.Contains(xml))
			{
				ret.Dispose();
				ret = resources[xml] as Pen;
			}
			else
			{
				resources[xml] = ret;
			}
			return ret;
		}

		public SolidBrush ReadSolidBrush(XmlReader xr)
		{
			SolidBrush ret = new SolidBrush(Color.FromArgb(XmlConvert.ToInt32(xr.GetAttribute("Color"))));
			string xml = this.GetXml(ret);
			if (resources.Contains(xml))
			{
				ret.Dispose();
				ret = resources[xml] as SolidBrush;
			}
			else
			{
				resources[xml] = ret;
			}
			return ret;
		}

		public Font ReadFont(XmlReader xr)
		{
			Font ret;
			try
			{
				ret = new Font(xr.GetAttribute("Name"),(float) XmlConvert.ToDouble(xr.GetAttribute("Size")),(FontStyle) Enum.Parse(typeof (FontStyle), xr.GetAttribute("Style")));
			}
			catch
			{
				return Control.DefaultFont;
			}
			string xml = this.GetXml(ret);
			if (resources.Contains(xml))
			{
				ret.Dispose();
				ret = resources[xml] as Font;
			}
			else
			{
				resources[xml] = ret;
			}
			return ret;
		}

		public AdjustableArrowCap ReadAdjustableArrowCap(XmlReader xr)
		{
			return new AdjustableArrowCap((float) XmlConvert.ToDouble(xr.GetAttribute("Width")),(float) XmlConvert.ToDouble(xr.GetAttribute("Height")), XmlConvert.ToBoolean(xr.GetAttribute("Filled")));
		}

		public Matrix ReadMatrix(XmlReader xr)
		{
			if (!this.ReadNext(xr) || xr.Name != "ArrayOfFloat" || xr.NodeType != XmlNodeType.Element)
			{
				return null;
			}
			float [] elems = new XmlSerializer(typeof (float [])).Deserialize(xr) as float [];
			return new Matrix(elems[0], elems[1], elems[2], elems[3], elems[4], elems[5]);
		}

		public PaperSize ReadPaperSize(XmlReader xr)
		{
			return new PaperSize(xr.GetAttribute("PaperName"), XmlConvert.ToInt32(xr.GetAttribute("Width")), XmlConvert.ToInt32(xr.GetAttribute("Height")));
		}

		public PaperSize ReadPaperSize(XmlReader xr, PrinterSettings settings)
		{
			PaperKind kind =(PaperKind) Enum.Parse(typeof (PaperKind), xr.GetAttribute("Kind"));
			if (kind == PaperKind.Custom) return this.ReadPaperSize(xr);
			foreach (PaperSize sz in settings.PaperSizes)
			{
				if (sz.Kind == kind) return sz;
			}
			return this.ReadPaperSize(xr);
		}

		public Margins ReadMargins(XmlReader xr)
		{
			return new Margins(XmlConvert.ToInt32(xr.GetAttribute("Left")), XmlConvert.ToInt32(xr.GetAttribute("Right")), XmlConvert.ToInt32(xr.GetAttribute("Top")), XmlConvert.ToInt32(xr.GetAttribute("Bottom")));
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
