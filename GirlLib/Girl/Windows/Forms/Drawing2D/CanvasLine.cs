// ここにソースコードの注釈を書きます。

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Girl.Drawing;

namespace Girl.Windows.Forms.Drawing2D
{
	public class CanvasLine : CanvasObject
	{
		public CanvasLine() : base() {}
		public CanvasLine(float x, float y, float width, float height) : base(x, y, width, height) {}
		public CanvasLine(RectangleF rect) : base(rect) {}
		
		public CanvasLine(PointF pt1, PointF pt2) : base()
		{
			this.SetPoints(pt1, pt2);
		}

		protected override void Init()
		{
			base.Init();
			this.editPath = true;
		}

		public override void Draw(Graphics g)
		{
			if (!this.CheckDraw(g) || this.pen == null) return;
			g.DrawLine(this.pen, this.rect.Left, this.rect.Top, this.rect.Right, this.rect.Bottom);
		}

		public void SetPoints(PointF pt1, PointF pt2)
		{
			this.Bounds = new RectangleF(pt1.X, pt1.Y, pt2.X - pt1.X, pt2.Y - pt1.Y);
		}

		#region 交差

		public override bool Contains(Matrix matrix, float angle, PointF pt)
		{
			Matrix m = matrix.Clone();
			this.Rotate(m, angle);
			float sc = Geometry.GetScale(m);
			PointF[] pts = new PointF[]
			{
				this.rect.Location, new PointF(this.rect.Right, this.rect.Bottom)
			}
			;
			m.TransformPoints(pts);
			m.Dispose();
			return CanvasLine.Contains(this, sc, pts[0], pts[1], pt);
		}

		public static bool Contains(CanvasObject co, float sc, PointF pt1, PointF pt2, PointF pt)
		{
			const float min = 3F;
			float w =(co.Pen != null) ? co.Pen.Width * sc / 2F + min:
			min;
			Geometry.LineEquation le1 = new Geometry.LineEquation(pt1, pt2);
			Geometry.LineEquation le2 = le1.GetCrossLine(pt);
			if (!le1.Contains(le2.pt2) && Geometry.GetDistance(pt, pt1) > w && Geometry.GetDistance(pt, pt2) > w)
			{
				return false;
			}
			return le1.GetDistance(pt) <= w;
		}

		public override bool IntersectsWith(Matrix matrix, float angle, RectangleF rect)
		{
			Matrix m = matrix.Clone();
			this.Rotate(m, angle);
			m.Invert();
			PointF[] pts = Geometry.ConvertToPoints(rect);
			m.TransformPoints(pts);
			m.Dispose();
			Geometry.LineEquation le = new Geometry.LineEquation(this.rect);
			return le.IntersectsWith(Geometry.ConvertToEquations(pts)) || Geometry.Contains(pts, le.pt1) || Geometry.Contains(pts, le.pt2);
		}

		#endregion

		#region 編集

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public override void InitSelection(CanvasObjectSelection sel)
		{
			base.InitSelection(sel);
			CanvasObject[] cos = new CanvasObject[]
			{
				sel.Borders[0], new CanvasLine()
			}
			;
			cos[1].Pen = cos[0].Pen;
			sel.Borders = cos;
			sel.Add(new CanvasCorner(this, 'E', Cursors.SizeAll));
			sel.Add(new CanvasCorner(this, 'E', Cursors.SizeAll));
		}

		public override void SetSelection(CanvasObjectSelection sel, Matrix transform, float angle)
		{
			base.SetSelection(sel, transform, angle);
			PointF[] pts = Geometry.ConvertToPoints(this.rect);
			Matrix m = transform.Clone();
			this.Rotate(m, angle);
			m.TransformPoints(pts);
			m.Dispose();
			(sel.Borders[1] as CanvasLine).SetPoints(pts[0], pts[2]);
			sel.Corners[8].CenterPoint = pts[0];
			sel.Corners[9].CenterPoint = pts[2];
		}

		public override void Resize(CanvasCorner corner, float x, float y, Matrix transform, float angle, Keys modifier)
		{
			int index = corner.Index;
			if (index < 8)
			{
				base.Resize(corner, x, y, transform, angle, modifier);
				return;
			}
			Matrix m = transform.Clone();
			this.Rotate(m, angle);
			m.Invert();
			PointF pt = Geometry.TransformPoint(m, x, y);
			m.Dispose();
			PointF cpt = this.CenterPoint;
			bool shift =(modifier & Keys.Shift) != 0;
			float x1 = this.rect.Left, y1 = this.rect.Top, x2 = this.rect.Right, y2 = this.rect.Bottom;
			switch (index)
			{
				case 8: x1 = pt.X;
				y1 = pt.Y;
				if (shift)
				{
					float dx = Math.Abs(x2 - x1), dy = Math.Abs(y2 - y1);
					if (dx > dy) y1 = y2;
					else x1 = x2;
				}
				break;
				case 9: x2 = pt.X;
				y2 = pt.Y;
				if (shift)
				{
					float dx = Math.Abs(x2 - x1), dy = Math.Abs(y2 - y1);
					if (dx > dy) y2 = y1;
					else x2 = x1;
				}
				break;
			}
			this.Bounds = RectangleF.FromLTRB(x1, y1, x2, y2);
			this.AdjustPosition(cpt, angle);
		}

		#endregion
	}
}
