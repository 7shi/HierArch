// ここにソースコードの注釈を書きます。

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Girl.Drawing;

namespace Girl.Windows.Forms.Drawing2D
{
	public class CanvasObject
	{
		public static Cursor[] CursorSizes = new Cursor[]
			{
				Cursors.SizeWE, Cursors.SizeNWSE, Cursors.SizeNS, Cursors.SizeNESW
			};
		protected Guid guid;
		protected string name;
		protected RectangleF rect;
		protected RectangleF prevRect;
		protected RectangleF crect;
		protected RectangleF[] crects;
		protected Pen pen;
		protected Brush brush;
		protected object tag;
		protected string toolTipText;
		protected float angle;
		protected float prevAngle;
		protected bool visible;
		protected bool enabled;
		protected bool fixAngle;
		protected bool resizable;
		protected SizeF minSize;
		protected SizeF maxSize;
		protected bool editPath;
		protected int order;
		protected bool frontMost;
		protected bool backMost;
		protected CanvasGroup group;

		protected virtual void Init()
		{
			this.guid = Guid.NewGuid();
			this.prevRect = RectangleF.Empty;
			this.pen = SystemPens.WindowText;
			this.brush = null;
			this.name = null;
			this.tag = null;
			this.toolTipText = null;
			this.angle = this.prevAngle = 0;
			this.visible = true;
			this.enabled = true;
			this.fixAngle = false;
			this.resizable = true;
			this.minSize = SizeF.Empty;
			this.maxSize = SizeF.Empty;
			this.crects = null;
			this.editPath = false;
			this.order = -1;
			this.frontMost = this.backMost = false;
			this.group = null;
		}

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public CanvasObject()
		{
			this.Init();
		}

		public CanvasObject(float x, float y, float width, float height)
		{
			this.Init();
			this.Bounds = new RectangleF(x, y, width, height);
		}

		public CanvasObject(RectangleF rect)
		{
			this.Init();
			this.Bounds = rect;
		}

		public void CheckGuid(Hashtable guidTable)
		{
			if (guidTable == null || !guidTable.Contains(this.guid)) return;
			this.guid = Guid.NewGuid();
		}

		#region Properties

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}

			set
			{
				this.name = value;
			}
		}

		public object Tag
		{
			get
			{
				return this.tag;
			}

			set
			{
				this.tag = value;
			}
		}

		public CanvasGroup Group
		{
			get
			{
				return this.group;
			}

			set
			{
				this.group = value;
			}
		}

		public CanvasGroup RootGroup
		{
			get
			{
				if (this.group == null) return null;
				return this.group.RootGroup;
			}
		}

		public bool Visible
		{
			get
			{
				return this.visible &&(this.group == null || this.group.Visible);
			}

			set
			{
				this.visible = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}

			set
			{
				this.enabled = value;
			}
		}

		public bool FixAngle
		{
			get
			{
				return this.fixAngle;
			}

			set
			{
				this.fixAngle = value;
			}
		}

		public string ToolTipText
		{
			get
			{
				return this.toolTipText;
			}

			set
			{
				this.toolTipText = value;
			}
		}

		public bool Resizable
		{
			get
			{
				return this.resizable;
			}

			set
			{
				this.resizable = value;
			}
		}

		public int Order
		{
			get
			{
				return this.order;
			}

			set
			{
				this.order = value;
			}
		}

		public bool FrontMost
		{
			get
			{
				return this.frontMost;
			}

			set
			{
				this.frontMost = value;
				this.backMost = false;
			}
		}

		public bool BackMost
		{
			get
			{
				return this.backMost;
			}

			set
			{
				this.frontMost = false;
				this.backMost = value;
			}
		}

		#endregion

		#region 描画

		public virtual void Draw(Graphics g)
		{
			if (!this.CheckDraw(g)) return;
		}

		protected bool CheckDraw(Graphics g)
		{
			float sc = Geometry.GetScale(g.Transform);
			if (Math.Abs(this.rect.Width * sc) < 2 && Math.Abs(this.rect.Height * sc) < 2)
			{
				CanvasRectangle.DrawRectangle(this, g);
				return false;
			}
			return true;
		}

		public Pen Pen
		{
			get
			{
				return this.pen;
			}

			set
			{
				this.pen = value;
			}
		}

		public Brush Brush
		{
			get
			{
				return this.brush;
			}

			set
			{
				this.brush = value;
			}
		}

		public Image GetThumbnailImage(int width, int height)
		{
			RectangleF r = this.crect;
			Bitmap ret = new Bitmap(width, height);
			Matrix m = new Matrix();
			float zx =(r.Width >= 10) ?(float) (width - 1) / r.Width:
			1;
			float zy =(r.Height >= 10) ?(float) (height - 1) / r.Height:
			1;
			m.Scale(zx, zy);
			if (r.Width < 10) m.Translate((width - r.Width) / 2, 0);
			if (r.Height < 10) m.Translate(0,(height - r.Height) / 2);
			Graphics g = Graphics.FromImage(ret);
			g.Transform = m;
			this.Draw(g);
			g.Dispose();
			m.Dispose();
			return ret;
		}

		#endregion

		#region 座標

		public RectangleF Bounds
		{
			get
			{
				return this.rect;
			}

			set
			{
				RectangleF r = this.rect;
				this.rect = value;
				this.SetClientRectangles();
				if (this.rect.Location != r.Location)
				{
					this.OnMove(EventArgs.Empty);
				}
				else if (this.rect.Size != r.Size)
				{
					this.OnResize(EventArgs.Empty);
				}
			}
		}

		public PointF Location
		{
			get
			{
				return this.rect.Location;
			}

			set
			{
				this.Bounds = new RectangleF(value, this.rect.Size);
			}
		}

		public SizeF Size
		{
			get
			{
				return this.rect.Size;
			}

			set
			{
				this.Bounds = new RectangleF(this.rect.Location, value);
			}
		}

		public float Left
		{
			get
			{
				return this.rect.Left;
			}

			set
			{
				RectangleF r = this.rect;
				r.X = value;
				this.Bounds = r;
			}
		}

		public float Top
		{
			get
			{
				return this.rect.Top;
			}

			set
			{
				RectangleF r = this.rect;
				r.Y = value;
				this.Bounds = r;
			}
		}

		public float Right
		{
			get
			{
				return this.rect.Right;
			}

			set
			{
				RectangleF r = this.rect;
				r.X = value - r.Width;
				this.Bounds = r;
			}
		}

		public float Bottom
		{
			get
			{
				return this.rect.Bottom;
			}

			set
			{
				RectangleF r = this.rect;
				r.Y = value - r.Height;
				this.Bounds = r;
			}
		}

		public float Width
		{
			get
			{
				return this.rect.Width;
			}

			set
			{
				RectangleF r = this.rect;
				r.Width = value;
				this.Bounds = r;
			}
		}

		public float Height
		{
			get
			{
				return this.rect.Height;
			}

			set
			{
				RectangleF r = this.rect;
				r.Height = value;
				this.Bounds = r;
			}
		}

		public PointF CenterPoint
		{
			get
			{
				return Geometry.GetCenter(this.rect);
			}

			set
			{
				this.Location = new PointF(value.X - this.rect.Width / 2F, value.Y - this.rect.Height / 2F);
			}
		}

		public SizeF MinSize
		{
			get
			{
				return this.minSize;
			}

			set
			{
				this.minSize = value;
			}
		}

		public float MinWidth
		{
			get
			{
				return this.minSize.Width;
			}

			set
			{
				this.minSize.Width = value;
			}
		}

		public float MinHeight
		{
			get
			{
				return this.minSize.Height;
			}

			set
			{
				this.minSize.Height = value;
			}
		}

		public SizeF MaxSize
		{
			get
			{
				return this.maxSize;
			}

			set
			{
				this.maxSize = value;
			}
		}

		public float MaxWidth
		{
			get
			{
				return this.maxSize.Width;
			}

			set
			{
				this.maxSize.Width = value;
			}
		}

		public float MaxHeight
		{
			get
			{
				return this.maxSize.Height;
			}

			set
			{
				this.maxSize.Height = value;
			}
		}

		public RectangleF ClientRectangle
		{
			get
			{
				return this.crect;
			}
		}

		public RectangleF[] ClientRectangles
		{
			get
			{
				return this.crects;
			}
		}

		public void Offset(float x, float y)
		{
			this.Location = new PointF(this.Left + x, this.Top + y);
		}

		public void Offset(PointF pt)
		{
			this.Offset(pt.X, pt.Y);
		}

		public float Angle
		{
			get
			{
				return this.angle;
			}

			set
			{
				while (value > 180F) value -= 360F;
				while (value < -180F) value += 360F;
				this.angle = value;
			}
		}

		protected virtual void SetClientRectangles()
		{
			this.SetClientRectangle();
			this.SetDefaultClientRectangles();
		}

		protected void SetClientRectangle()
		{
			// Macro: Rectangle(this.crect)=Rectangle(this.rect)の絶対値
			{
				this.crect = this.rect;
				if (this.rect.Width < 0)
				{
					this.crect.X += this.rect.Width;
					this.crect.Width = Math.Abs(this.rect.Width);
				}
				if (this.rect.Height < 0)
				{
					this.crect.Y += this.rect.Height;
					this.crect.Height = Math.Abs(this.rect.Height);
				}
			}
		}

		protected void SetDefaultClientRectangles()
		{
			if (this.crects == null || this.crects.Length != 1)
			{
				this.crects = new RectangleF[1];
			}
			this.crects[0] = this.crect;
		}

		public float PenWidth
		{
			get
			{
				return (this.pen != null) ? this.pen.Width:
				1F;
			}
		}

		protected virtual void OnMove(EventArgs e)
		{
		}

		protected virtual void OnResize(EventArgs e)
		{
		}

		public virtual void MemorizeStatus()
		{
			this.prevRect = this.rect;
			this.prevAngle = this.angle;
		}

		public virtual void RestoreStatus()
		{
			this.Bounds = this.prevRect;
			this.Angle = this.prevAngle;
		}

		public RectangleF MemorizedBounds
		{
			get
			{
				return this.prevRect;
			}
		}

		public float MemorizedAngle
		{
			get
			{
				return this.prevAngle;
			}
		}

		public virtual object TagBounds
		{
			get
			{
				return this.prevRect;
			}

			set
			{
				this.Bounds =(RectangleF) value;
			}
		}

		#endregion

		#region 交差

		public void Rotate(Matrix matrix, float angle)
		{
			float an = this.angle;
			if (this.fixAngle) an -= angle;
			if (an != 0F) matrix.RotateAt(an, this.CenterPoint);
		}

		public virtual bool Contains(Matrix matrix, float angle, PointF pt)
		{
			Matrix m = matrix.Clone();
			this.Rotate(m, angle);
			m.Invert();
			pt = Geometry.TransformPoint(m, pt);
			m.Dispose();
			foreach (RectangleF r in this.crects)
			{
				if (r.Contains(pt)) return true;
			}
			return false;
		}

		public bool Contains(Matrix matrix, float angle, float x, float y)
		{
			return this.Contains(matrix, angle, new PointF(x, y));
		}

		public virtual bool IntersectsWith(Matrix matrix, float angle, RectangleF rect)
		{
			Matrix m = matrix.Clone();
			this.Rotate(m, angle);
			bool ret = false;
			foreach (RectangleF r in this.crects)
			{
				PointF[] quad = Geometry.ConvertToPoints(r);
				m.TransformPoints(quad);
				if (Geometry.IntersectsWith(rect, quad))
				{
					ret = true;
					break;
				}
			}
			m.Dispose();
			return ret;
		}

		public bool IntersectsWith(Matrix matrix, float angle, float x, float y, float width, float height)
		{
			return this.IntersectsWith(matrix, angle, x, y, width, height);
		}

		public virtual bool IsContainedWith(Matrix matrix, float angle, RectangleF rect)
		{
			Matrix m = matrix.Clone();
			this.Rotate(m, angle);
			bool ret = true;
			foreach (RectangleF r in this.crects)
			{
				PointF[] quad = Geometry.ConvertToPoints(r);
				m.TransformPoints(quad);
				if (!Geometry.Contains(rect, quad))
				{
					ret = false;
					break;
				}
			}
			m.Dispose();
			return ret;
		}

		public bool IsContainedWith(Matrix matrix, float angle, float x, float y, float width, float height)
		{
			return this.IsContainedWith(matrix, angle, new RectangleF(x, y, width, height));
		}

		#endregion

		#region 編集

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public virtual void InitSelection(CanvasObjectSelection sel)
		{
			sel.Borders = new CanvasObject[]
			{
				new CanvasPolygon(), new CanvasPolygon()
			}
			;
			sel.Borders[0].Pen = SystemPens.Highlight;
			sel.Corners = new CanvasCorner[0];
			for (int i = 0; i < 8; i++) sel.Add(new CanvasCorner(this));
		}

		public virtual void SetSelection(CanvasObjectSelection sel, Matrix transform, float angle)
		{
			PointF[] pts = Geometry.ConvertToPoints(this.rect);
			PointF cpt = this.CenterPoint;
			float sc = 7 / Geometry.GetScale(transform);
			pts[0] = Geometry.Inflate(pts[0].X, pts[0].Y, cpt.X, cpt.Y, sc, sc, -1, -1);
			pts[1] = Geometry.Inflate(pts[1].X, pts[1].Y, cpt.X, cpt.Y, sc, sc, 1, -1);
			pts[2] = Geometry.Inflate(pts[2].X, pts[2].Y, cpt.X, cpt.Y, sc, sc, 1, 1);
			pts[3] = Geometry.Inflate(pts[3].X, pts[3].Y, cpt.X, cpt.Y, sc, sc, -1, 1);
			Matrix m = transform.Clone();
			this.Rotate(m, angle);
			m.TransformPoints(pts);
			m.Dispose();
			CanvasPolygon poly = sel.Borders[0] as CanvasPolygon;
			poly.Location = PointF.Empty;
			poly.Points = pts;
			sel.Corners[0].CenterPoint = pts[0];
			sel.Corners[1].CenterPoint = Geometry.GetCenter(pts[0], pts[1]);
			sel.Corners[2].CenterPoint = pts[1];
			sel.Corners[3].CenterPoint = Geometry.GetCenter(pts[0], pts[3]);
			sel.Corners[4].CenterPoint = Geometry.GetCenter(pts[1], pts[2]);
			sel.Corners[5].CenterPoint = pts[3];
			sel.Corners[6].CenterPoint = Geometry.GetCenter(pts[3], pts[2]);
			sel.Corners[7].CenterPoint = pts[2];
			if (this.resizable)
			{
				PointF cp = Geometry.GetCenter(pts);
				float an = Geometry.GetAngle(cp, sel.Corners[1].CenterPoint);
				if (an < 0F) an += 180F;
				int cur =(int) ((an + 22.5F) / 45F);
				sel.Corners[1].Cursor = sel.Corners[6].Cursor = CursorSizes[cur & 3];
				sel.Corners[3].Cursor = sel.Corners[4].Cursor = CursorSizes[(cur + 2) & 3];
				if (sel.Corners[0].Left > sel.Corners[1].Left) cur += 2;
				if (sel.Corners[0].Top > sel.Corners[3].Top) cur += 2;
				sel.Corners[0].Cursor = sel.Corners[7].Cursor = CursorSizes[(cur - 1) & 3];
				sel.Corners[2].Cursor = sel.Corners[5].Cursor = CursorSizes[(cur + 1) & 3];
			}
			else
			{
				foreach (CanvasCorner cc in sel.Corners)
				{
					cc.Cursor = Cursors.Default;
				}
			}
		}

		public virtual void Resize(CanvasCorner corner, float x, float y, Matrix transform, float angle, Keys modifier)
		{
			int index = corner.Index;
			Matrix m1 = transform.Clone();
			this.Rotate(m1, angle);
			PointF cpt1 = this.CenterPoint;
			PointF cpt2 = Geometry.TransformPoint(m1, cpt1);
			m1.Invert();
			PointF pt = Geometry.TransformPoint(m1, Geometry.Inflate(x, y, cpt2.X, cpt2.Y, -7, -7, 0, 0));
			m1.Dispose();
			float x1 = this.rect.Left, y1 = this.rect.Top;
			float x2 = this.rect.Right, y2 = this.rect.Bottom;
			float rx1 = this.prevRect.Left, ry1 = this.prevRect.Top;
			float rx2 = this.prevRect.Right, ry2 = this.prevRect.Bottom;
			float rw = this.prevRect.Width, rh = this.prevRect.Height;
			bool shift = false;
			///(modifier & Keys.Shift) != 0;
			if (index == 0 || index == 3 || index == 5)
			{
				float dx = x2 - pt.X;
				if (minSize.Width > 0 && Math.Abs(dx) < minSize.Width) if (dx > 0) pt.X = x2 - minSize.Width;
				else pt.X = x2 + minSize.Width;
				if (maxSize.Width > 0 && Math.Abs(dx) > maxSize.Width) if (dx > 0) pt.X = x2 - maxSize.Width;
				else pt.X = x2 + maxSize.Width;
				x1 = pt.X;
				if (shift)
				{
					float dh = rh * Math.Abs(rx2 - x1) / rw - rh;
					if (index == 0)
					{
						y1 = ry1 - dh;
					}
					else if (index == 5)
					{
						y2 = ry2 + dh;
					}
					else
					{
						y1 = ry1 - dh / 2;
						y2 = ry2 + dh / 2;
					}
				}
			}
			else if (index == 2 || index == 4 || index == 7)
			{
				float dx = pt.X - x1;
				if (minSize.Width > 0 && Math.Abs(dx) < minSize.Width) if (dx > 0) pt.X = x1 + minSize.Width;
				else pt.X = x1 - minSize.Width;
				if (maxSize.Width > 0 && Math.Abs(dx) > maxSize.Width) if (dx > 0) pt.X = x1 + maxSize.Width;
				else pt.X = x1 - maxSize.Width;
				x2 = pt.X;
				if (shift)
				{
					float dh = rh * Math.Abs(x2 - rx1) / rw - rh;
					if (index == 2)
					{
						y1 = ry1 - dh;
					}
					else if (index == 7)
					{
						y2 = ry2 + dh;
					}
					else
					{
						y1 = ry1 - dh / 2;
						y2 = ry2 + dh / 2;
					}
				}
			}
			if ((!shift && index == 0) || index == 1 ||(!shift && index == 2))
			{
				float dy = y2 - pt.Y;
				if (minSize.Height > 0 && Math.Abs(dy) < minSize.Height) if (dy > 0) pt.Y = y2 - minSize.Height;
				else pt.Y = y2 + minSize.Height;
				if (maxSize.Height > 0 && Math.Abs(dy) > maxSize.Height) if (dy > 0) pt.Y = y1 - maxSize.Height;
				else pt.Y = y1 + maxSize.Height;
				y1 = pt.Y;
			}
			else if ((!shift && index == 5) || index == 6 ||(!shift && index == 7))
			{
				float dy = pt.Y - y1;
				if (minSize.Height > 0 && Math.Abs(dy) < minSize.Height) if (dy > 0) pt.Y = y1 + minSize.Height;
				else pt.Y = y1 - minSize.Height;
				if (maxSize.Height > 0 && Math.Abs(dy) > maxSize.Height) if (dy > 0) pt.Y = y1 + maxSize.Height;
				else pt.Y = y1 - maxSize.Height;
				y2 = pt.Y;
			}
			this.Bounds = RectangleF.FromLTRB(x1, y1, x2, y2);
			this.AdjustPosition(cpt1, angle);
		}

		public virtual void CancelResize()
		{
			this.RestoreStatus();
		}

		public virtual void EndResize()
		{
		}

		protected void AdjustPosition(PointF cpt, float angle)
		{
			Matrix m = new Matrix();
			float an = this.angle;
			if (this.fixAngle) an -= angle;
			if (an != 0F) m.RotateAt(an, cpt);
			this.CenterPoint = Geometry.TransformPoint(m, this.CenterPoint);
			m.Dispose();
		}

		#endregion

		#region XML

		public void WriteXml(CanvasSerializer serializer, XmlWriter xw)
		{
			xw.WriteAttributeString("Guid", XmlConvert.ToString(this.guid));
			xw.WriteAttributeString("Name", this.name);
			xw.WriteAttributeString("ToolTipText", this.toolTipText);
			xw.WriteAttributeString("Angle", XmlConvert.ToString(this.angle));
			xw.WriteAttributeString("Visible", XmlConvert.ToString(this.visible));
			xw.WriteAttributeString("Enabled", XmlConvert.ToString(this.enabled));
			xw.WriteAttributeString("FixAngle", XmlConvert.ToString(this.fixAngle));
			xw.WriteAttributeString("Resizable", XmlConvert.ToString(this.resizable));
			xw.WriteAttributeString("FrontMost", XmlConvert.ToString(this.frontMost));
			xw.WriteAttributeString("BackMost", XmlConvert.ToString(this.backMost));
			if (this.order >= 0)
			{
				xw.WriteAttributeString("Order", XmlConvert.ToString(this.order));
			}
			//xw.WriteAttributeString("Group", XmlConvert.ToString(this.group.Guid));
			serializer.WriteRectangleF(xw, "Bounds", this.rect);
			if (this.pen != null) serializer.Write(xw, "Pen", this.pen);
			if (this.brush != null)
			{
				xw.WriteStartElement("Brush");
				serializer.Write(xw, null, this.brush);
				xw.WriteEndElement();
			}
			this.WriteXmlData(serializer, xw);
		}

		protected virtual void WriteXmlData(CanvasSerializer serializer, XmlWriter xw)
		{
		}

		public void ReadXml(CanvasSerializer serializer, XmlReader xr)
		{
			string v;

			if (xr.NodeType != XmlNodeType.Element) return;
			string name = xr.Name;
			this.guid = XmlConvert.ToGuid(xr.GetAttribute("Guid"));
			this.name = xr.GetAttribute("Name");
			this.ToolTipText = xr.GetAttribute("ToolTipText");
			this.angle =(float) XmlConvert.ToDouble(xr.GetAttribute("Angle"));
			this.visible = XmlConvert.ToBoolean(xr.GetAttribute("Visible"));
			this.enabled = XmlConvert.ToBoolean(xr.GetAttribute("Enabled"));
			this.fixAngle = XmlConvert.ToBoolean(xr.GetAttribute("FixAngle"));
			v = xr.GetAttribute("Resizable");
			if (v != null) this.resizable = XmlConvert.ToBoolean(v);
			v = xr.GetAttribute("Order");
			if (v != null) this.order = XmlConvert.ToInt32(v);
			v = xr.GetAttribute("FrontMost");
			if (v != null) this.frontMost = XmlConvert.ToBoolean(v);
			v = xr.GetAttribute("BackMost");
			if (v != null) this.backMost = XmlConvert.ToBoolean(v);
			this.pen = null;
			this.brush = null;
			//Guid group = XmlConvert.ToGuid(xr.GetAttribute("Group"));
			//this.group = ...;
			if (xr.IsEmptyElement) return;
			while (xr.Read())
			{
				if (xr.Name == name && xr.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				else if (xr.NodeType == XmlNodeType.Element)
				{
					this.ReadXmlData(serializer, xr);
				}
			}
			this.ReadXmlAdjust();
			this.SetClientRectangles();
			this.MemorizeStatus();
		}

		protected virtual void ReadXmlData(CanvasSerializer serializer, XmlReader xr)
		{
			switch (xr.Name)
			{
				case "Bounds":
				this.rect = serializer.ReadRectangleF(xr);
				return;
				case "Pen":
				this.pen = serializer.ReadPen(xr);
				return;
				case "Brush":
				if (serializer.ReadNext(xr))
				{
					this.brush = serializer.Read(xr) as Brush;
				}
				return;
			}
		}

		protected virtual void ReadXmlAdjust()
		{
		}

		#endregion
	}
}
