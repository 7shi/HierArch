// ここにソースコードの注釈を書きます。

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Girl.Drawing;

namespace Girl.Windows.Forms.Drawing2D
{
	public class CanvasPolygon : CanvasObject
	{
		public CanvasPolygon() : base() {}
		public CanvasPolygon(PointF[] points) : this(0F, 0F, points) {}
		public CanvasPolygon(PointF pt, PointF[] points) : this(pt.X, pt.Y, points) {}
		
		public CanvasPolygon(float x, float y, PointF[] points) : base(x, y, 0F, 0F)
		{
			this.Points = points;
		}
		
		public CanvasPolygon(PointF pt1, PointF pt2) : base()
		{
			this.Points = new PointF[] {pt1, pt2};
		}
		protected PointF[] points;
		protected SizeF pointsSize;
		protected PointF[] apoints;
		protected PointF[] prevPoints;
		protected CanvasCorner addedCorner1;
		protected CanvasCorner addedCorner2;

		protected override void Init()
		{
			base.Init();
			this.editPath = true;
			this.points = null;
			this.pointsSize = SizeF.Empty;
			this.apoints = null;
			this.prevPoints = null;
			this.InitResize();
		}

		public override void Draw(Graphics g)
		{
			if (!this.CheckDraw(g)) return;
			if (this.brush != null)
			{
				g.FillPolygon(this.brush, this.apoints);
			}
			if (this.pen != null)
			{
				g.DrawPolygon(this.pen, this.apoints);
			}
		}

		public PointF[] Points
		{
			get
			{
				return this.points;
			}

			set
			{
				if (value == null || value.Length < 1)
				{
					this.points = null;
					this.Size = SizeF.Empty;
					return;
				}
				RectangleF rect = Geometry.ConvertToRectangle(value);
				int len = value.Length;
				this.points = new PointF[len];
				for (int i = 0; i < len; i++)
				{
					this.points[i] = new PointF(value[i].X - rect.X, value[i].Y - rect.Y);
				}
				rect.Offset(this.rect.X, this.rect.Y);
				this.pointsSize = rect.Size;
				this.Bounds = rect;
			}
		}

		public PointF[] AbsolutePoints
		{
			get
			{
				return this.apoints;
			}
		}

		protected override void SetClientRectangles()
		{
			this.SetClientRectangle();
			this.SetAbsolutePoints();
			if (this.points == null || this.brush != null)
			{
				this.SetDefaultClientRectangles();
				return;
			}
			int len = this.points.Length - 1;
			this.crects = new RectangleF[len];
			PointF[] pts = this.AbsolutePoints;
			for (int i = 0; i < len; i++)
			{
				this.crects[i] = RectangleF.FromLTRB(pts[i].X, pts[i].Y, pts[i + 1].X, pts[i + 1].Y);
			}
		}

		protected void SetAbsolutePoints()
		{
			if (this.points == null)
			{
				this.apoints = null;
				return;
			}
			int len = points.Length;
			this.apoints = new PointF[len];
			float zw =(this.pointsSize.Width != 0) ? this.rect.Width / this.pointsSize.Width:
			1;
			float zh =(this.pointsSize.Height != 0) ? this.rect.Height / this.pointsSize.Height:
			1;
			float dx = this.rect.X, dy = this.rect.Y;
			for (int i = 0; i < len; i++)
			{
				this.apoints[i] = new PointF(this.points[i].X * zw + dx, this.points[i].Y * zh + dy);
			}
		}

		public override void MemorizeStatus()
		{
			base.MemorizeStatus();
			this.prevPoints = this.points.Clone() as PointF[];
		}

		public override void RestoreStatus()
		{
			base.RestoreStatus();
			RectangleF r = this.rect;
			this.Points = this.prevPoints as PointF[];
			this.Bounds = r;
		}

		public override object TagBounds
		{
			get
			{
				return new object []
				{
					this.prevRect, this.prevPoints.Clone()
				}
				;
			}

			set
			{
				object [] tag = value as object [];
				RectangleF r =(RectangleF) tag[0];
				this.Points = tag[1] as PointF[];
				this.Bounds = r;
			}
		}

		#region 交差

		public override bool Contains(Matrix matrix, float angle, PointF pt)
		{
			Matrix m = matrix.Clone();
			this.Rotate(m, angle);
			float sc = Geometry.GetScale(m);
			PointF[] pts = this.apoints.Clone() as PointF[];
			m.TransformPoints(pts);
			m.Dispose();
			int len = this.points.Length;
			if (this.brush == null)
			{
				for (int i = 0; i < len; i++)
				{
					int j = i + 1;
					if (j == len) j = 0;
					if (CanvasLine.Contains(this, sc, pts[i], pts[j], pt)) return true;
				}
				return false;
			}
			return Geometry.Contains(pts, pt);
		}

		public override bool IntersectsWith(Matrix matrix, float angle, RectangleF rect)
		{
			Matrix m = matrix.Clone();
			this.Rotate(m, angle);
			m.Invert();
			PointF[] pts = Geometry.ConvertToPoints(rect);
			m.TransformPoints(pts);
			m.Dispose();
			return Geometry.IntersectsWith(this.apoints, pts);
		}

		#endregion

		#region 編集

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public override void InitSelection(CanvasObjectSelection sel)
		{
			base.InitSelection(sel);
			this.InitResize();
			this.InitBorders(sel);
			int len = this.apoints.Length;
			for (int i = 0; i < len; i++)
			{
				sel.Add(new CanvasCorner(this, 'E', Cursors.SizeAll));
			}
			for (int i = 0; i < len; i++)
			{
				sel.Add(new CanvasCorner(this, 'L'));
			}
		}

		protected virtual void InitBorders(CanvasObjectSelection sel)
		{
			CanvasObject[] cos = new CanvasObject[]
			{
				sel.Borders[0], new CanvasPolygon()
			}
			;
			cos[1].Pen = cos[0].Pen;
			sel.Borders = cos;
		}

		public override void SetSelection(CanvasObjectSelection sel, Matrix transform, float angle)
		{
			base.SetSelection(sel, transform, angle);
			PointF[] pts = this.apoints.Clone() as PointF[];
			Matrix m = transform.Clone();
			this.Rotate(m, angle);
			m.TransformPoints(pts);
			m.Dispose();
			sel.Borders[1].Location = PointF.Empty;
			(sel.Borders[1] as CanvasPolygon).Points = pts;
			int len = pts.Length;
			for (int i = 0; i < len; i++)
			{
				int j = i + 1;
				if (j == pts.Length) j = 0;
				sel.Corners[i + 8].CenterPoint = pts[i];
				sel.Corners[i + 8 + len].CenterPoint = Geometry.GetCenter(pts[i], pts[j]);
			}
		}

		protected void InitResize()
		{
			this.addedCorner1 = this.addedCorner2 = null;
		}

		public override void Resize(CanvasCorner corner, float x, float y, Matrix transform, float angle, Keys modifier)
		{
			int index = corner.Index;
			if (index < 8)
			{
				base.Resize(corner, x, y, transform, angle, modifier);
				return;
			}
			index -= 8;
			int len = this.apoints.Length;
			if (index >= len) return;
			Matrix m1 = transform.Clone();
			this.Rotate(m1, angle);
			Matrix m2 = m1.Clone();
			m1.Invert();
			PointF pt = new PointF(x, y);
			PointF cpt = this.CenterPoint;
			bool ctrl =(modifier & Keys.Control) != 0;
			if (ctrl && this.addedCorner1 == null)
			{
				this.RestoreStatus();
				cpt = this.CenterPoint;
				this.AddPoint(corner, Geometry.TransformPoint(m1, pt));
			}
			else if (!ctrl && this.addedCorner1 != null)
			{
				this.RemovePoint();
				this.RestoreStatus();
				cpt = this.CenterPoint;
			}
			index = corner.Index - 8;
			if ((modifier & Keys.Shift) != 0)
			{
				PointF[] pts = this.GetRemovedPoints(index);
				m2.TransformPoints(pts);
				pt = Geometry.GetRightAngled(pt, pts, 5);
			}
			this.apoints[index] = Geometry.TransformPoint(m1, pt);
			m2.Dispose();
			m1.Dispose();
			this.rect = RectangleF.Empty;
			this.Points = this.apoints;
			this.AdjustPosition(cpt, angle);
		}

		protected virtual void AddPoint(CanvasCorner corner, PointF pt)
		{
			int index1 = corner.Index - 8;
			int index2 = this.CheckPoint(index1, pt);
			this.apoints = this.GetAddedPoints(index2, pt);
			this.addedCorner1 = new CanvasCorner(this, 'E', Cursors.SizeAll);
			this.addedCorner2 = new CanvasCorner(this, 'L');
			int pos = 8;
			if (index1 == index2) pos--;
			CanvasObjectSelection sel = corner.Selection;
			sel.Add(this.addedCorner1, pos + index1);
			sel.Add(this.addedCorner2, pos + this.apoints.Length + index1);
		}

		protected virtual int CheckPoint(int index, PointF pt)
		{
			int len = this.apoints.Length;
			int index2 = index - 1;
			if (index2 == -1) index2 = len - 1;
			int index3 = index + 1;
			if (index3 == len) index3 = 0;
			PointF pt1 = this.apoints[index];
			PointF pt2 = this.apoints[index2];
			PointF pt3 = this.apoints[index3];
			float d12 = -1, d13 = -1;
			if (pt1 != pt2)
			{
				Geometry.LineEquation le1 = new Geometry.LineEquation(pt1, pt2);
				Geometry.LineEquation le2 = le1.GetCrossLine(pt);
				if (le1.Contains(le2.pt2)) d12 = le1.GetDistance(pt);
			}
			if (pt1 != pt3)
			{
				Geometry.LineEquation le1 = new Geometry.LineEquation(pt1, pt3);
				Geometry.LineEquation le2 = le1.GetCrossLine(pt);
				if (le1.Contains(le2.pt2)) d13 = le1.GetDistance(pt);
			}
			if (d12 >= 0 && d13 >= 0)
			{
				if (d12 < d13) return index - 1;
				if (d13 < d12) return index;
			}
			if (d12 >= 0 && d13 < 0) return index - 1;
			if (d13 >= 0 && d12 < 0) return index;
			float d2 = Geometry.GetDistance(pt2, pt);
			float d3 = Geometry.GetDistance(pt3, pt);
			if (d2 < d3) return index - 1;
			return index;
		}

		protected virtual void RemovePoint()
		{
			CanvasObjectSelection sel = this.addedCorner1.Selection;
			sel.Remove(this.addedCorner1.Index);
			sel.Remove(this.addedCorner2.Index);
			this.InitResize();
		}

		public override void CancelResize()
		{
			if (this.addedCorner1 != null) this.RemovePoint();
			base.CancelResize();
		}

		public override void EndResize()
		{
			base.EndResize();
			this.InitResize();
		}

		public void AddVertex(int index)
		{
			int len = this.apoints.Length;
			index -= len + 8;
			if (index < 0 || index >= len) return;
			int index2 = index + 1;
			if (index2 == len) index2 = 0;
			PointF pt = Geometry.GetCenter(this.apoints[index], this.apoints[index2]);
			this.rect = RectangleF.Empty;
			this.Points = this.GetAddedPoints(index, pt);
		}

		public void RemoveVertex(int index, float angle)
		{
			index -= 8;
			int len = this.apoints.Length;
			if (len <= 2 || index < 0 || index >= len) return;
			PointF cpt = this.CenterPoint;
			this.rect = RectangleF.Empty;
			this.Points = this.GetRemovedPoints(index);
			this.AdjustPosition(cpt, angle);
		}

		protected PointF[] GetAddedPoints(int index, PointF pt)
		{
			int len = this.apoints.Length;
			PointF[] ret = new PointF[len + 1];
			if (index + 1 > 0) Array.Copy(this.apoints, ret, index + 1);
			ret[index + 1] = pt;
			int len2 = len - index - 1;
			if (len2 > 0) Array.Copy(this.apoints, index + 1, ret, index + 2, len2);
			return ret;
		}

		protected PointF[] GetRemovedPoints(int index)
		{
			int len = this.apoints.Length;
			PointF[] ret = new PointF[len - 1];
			if (index > 0) Array.Copy(this.apoints, ret, index);
			int len2 = len - index - 1;
			if (len2 > 0) Array.Copy(this.apoints, index + 1, ret, index, len2);
			return ret;
		}

		#endregion

		#region XML

		protected override void WriteXmlData(CanvasSerializer serializer, XmlWriter xw)
		{
			base.WriteXmlData(serializer, xw);
			if (this.points == null) return;
			xw.WriteStartElement("Points");
			new XmlSerializer(typeof (PointF[])).Serialize(xw, this.points);
			xw.WriteEndElement();
		}

		protected override void ReadXmlData(CanvasSerializer serializer, XmlReader xr)
		{
			if (xr.Name == "Points" && serializer.ReadNext(xr) && xr.Name == "ArrayOfPointF" && xr.NodeType == XmlNodeType.Element)
			{
				RectangleF r = this.rect;
				this.Points =(PointF[]) new XmlSerializer(typeof (PointF[])).Deserialize(xr);
				this.Bounds = r;
				return;
			}
			base.ReadXmlData(serializer, xr);
		}

		protected override void ReadXmlAdjust()
		{
			if (this.points != null) return;
			RectangleF r = this.rect;
			this.rect = RectangleF.Empty;
			this.Points = new PointF[]
			{
				r.Location, new PointF(r.X + r.Width, r.Y + r.Height)
			}
			;
		}

		#endregion
	}
}
