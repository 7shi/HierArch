// ここにソースコードの注釈を書きます。

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Girl.Drawing;

namespace Girl.Windows.Forms.Drawing2D
{
	public class CanvasLines : CanvasPolygon
	{
		public CanvasLines() : base() {}
		public CanvasLines(PointF[] points) : base(points) {}
		public CanvasLines(PointF pt, PointF[] points) : base(pt, points) {}
		public CanvasLines(float x, float y, PointF[] points) : base(x, y, points) {}
		public CanvasLines(PointF pt1, PointF pt2) : base(pt1, pt2) {}

		public override void Draw(Graphics g)
		{
			if (!this.CheckDraw(g) || this.points == null) return;
			if (this.brush != null)
			{
				g.FillPolygon(this.brush, this.apoints);
			}
			if (this.pen != null)
			{
				g.DrawLines(this.pen, this.apoints);
			}
		}

		protected override void SetClientRectangles()
		{
			this.SetClientRectangle();
			if (this.points == null)
			{
				this.SetDefaultClientRectangles();
				return;
			}
			this.SetAbsolutePoints();
			int len = this.points.Length - 1;
			this.crects = new RectangleF[len];
			for (int i = 0; i < len; i++)
			{
				this.crects[i] = RectangleF.FromLTRB(this.apoints[i].X, this.apoints[i].Y, this.apoints[i + 1].X, this.apoints[i + 1].Y);
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
			for (int i = 0; i < len - 1; i++)
			{
				if (CanvasLine.Contains(this, sc, pts[i], pts[i + 1], pt)) return true;
			}
			if (this.brush != null) return base.Contains(matrix, angle, pt);
			return false;
		}

		public override bool IntersectsWith(Matrix matrix, float angle, RectangleF rect)
		{
			Matrix m = matrix.Clone();
			this.Rotate(m, angle);
			m.Invert();
			PointF[] pts = Geometry.ConvertToPoints(rect);
			m.TransformPoints(pts);
			m.Dispose();
			Geometry.LineEquation[] eqs = Geometry.ConvertToEquations(pts);
			float dx = this.rect.X, dy = this.rect.Y;
			for (int i = 0; i < this.points.Length - 1; i++)
			{
				Geometry.LineEquation le = new Geometry.LineEquation(this.apoints[i], this.apoints[i + 1]);
				if (le.IntersectsWith(eqs) || Geometry.Contains(pts, le.pt1) || Geometry.Contains(pts, le.pt2))
				{
					return true;
				}
			}
			if (this.brush != null) base.IntersectsWith(matrix, angle, rect);
			return false;
		}

		#endregion

		#region 編集

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public override void InitSelection(CanvasObjectSelection sel)
		{
			base.InitSelection(sel);
			this.SetCornerVisible(sel);
		}

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		protected override void InitBorders(CanvasObjectSelection sel)
		{
			CanvasObject[] cos = new CanvasObject[]
			{
				sel.Borders[0], new CanvasLines()
			}
			;
			cos[1].Pen = cos[0].Pen;
			sel.Borders = cos;
		}

		protected void SetCornerVisible(CanvasObjectSelection sel)
		{
			int len = sel.Corners.Length;
			for (int i = 0; i < len - 1; i++) sel.Corners[i].Visible = true;
			sel.Corners[len - 1].Visible = false;
		}

		protected override void AddPoint(CanvasCorner corner, PointF pt)
		{
			base.AddPoint(corner, pt);
			this.SetCornerVisible(corner.Selection);
		}

		protected override int CheckPoint(int index, PointF pt)
		{
			if (index == 0) return -1;
			if (index == this.apoints.Length - 1) return index;
			return base.CheckPoint(index, pt);
		}

		protected override void RemovePoint()
		{
			CanvasObjectSelection sel = this.addedCorner1.Selection;
			base.RemovePoint();
			this.SetCornerVisible(sel);
		}

		#endregion
	}
}
