// ここにソースコードの注釈を書きます。

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Girl.Drawing;

namespace Girl.Windows.Forms.Drawing2D
{
	public class CanvasString : CanvasObject
	{
		public CanvasString() : base() {}
		public CanvasString(string text) : this(0, 0, text) {}
		public CanvasString(PointF pt, string text) : this(pt.X, pt.Y, text) {}
		
		public CanvasString(float x, float y, string text) : base(x, y, 0F, 0F)
		{
			this.Text = text;
		}
		protected string text;
		protected Font font;

		protected override void Init()
		{
			base.Init();
			this.text = null;
			this.font = Control.DefaultFont;
			this.brush = Brushes.Black;
			this.resizable = false;
		}

		public override void Draw(Graphics g)
		{
			RectangleF r;

			if (!this.CheckDraw(g) || this.brush == null || this.text == null || this.font == null) return;
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			// Macro: Rectangle(r)=Rectangle(this.rect)の絶対値
			{
				r = this.rect;
				if (this.rect.Width < 0)
				{
					r.X += this.rect.Width;
					r.Width = Math.Abs(this.rect.Width);
				}
				if (this.rect.Height < 0)
				{
					r.Y += this.rect.Height;
					r.Height = Math.Abs(this.rect.Height);
				}
			}
			r.Size = g.MeasureString(this.text, this.font);
			g.DrawString(this.text, this.font, this.brush, r, format);
		}

		public string Text
		{
			get
			{
				return this.text;
			}

			set
			{
				this.text = value;
				this.SetSize();
			}
		}

		public Font Font
		{
			get
			{
				return this.font;
			}

			set
			{
				this.font = value;
				this.SetSize();
			}
		}

		protected void SetSize()
		{
			Graphics g = Canvas.DefaultGraphics;
			string text = this.text;
			if (text == null || text.Length < 1) text = "WW";
			this.Size = g.MeasureString(text, this.font);
			g.Dispose();
		}

		protected override void WriteXmlData(CanvasSerializer serializer, XmlWriter xw)
		{
			base.WriteXmlData(serializer, xw);
			serializer.Write(xw, "Font", this.font);
			xw.WriteStartElement("Text");
			xw.WriteString(this.text);
			xw.WriteEndElement();
		}

		protected override void ReadXmlData(CanvasSerializer serializer, XmlReader xr)
		{
			if (xr.Name == "Font")
			{
				this.font = serializer.ReadFont(xr);
				return;
			}
			else if (xr.Name == "Text")
			{
				this.text = xr.ReadString();
				return;
			}
			base.ReadXmlData(serializer, xr);
		}
	}
}
