// ここにソースコードの注釈を書きます。

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Girl.Drawing;

namespace Girl.Windows.Forms.Drawing2D
{
	public class CanvasRectangle : CanvasObject
	{
		public CanvasRectangle() : base() {}
		public CanvasRectangle(float x, float y, float width, float height) : base(x, y, width, height) {}
		public CanvasRectangle(RectangleF rect) : base(rect) {}

		public override void Draw(Graphics g)
		{
			CanvasRectangle.DrawRectangle(this, g);
		}

		public static void DrawRectangle(CanvasObject co, Graphics g)
		{
			RectangleF rect;

			// Macro: Rectangle(rect)=Rectangle(co.Bounds)の絶対値
			{
				rect = co.Bounds;
				if (co.Bounds.Width < 0)
				{
					rect.X += co.Bounds.Width;
					rect.Width = Math.Abs(co.Bounds.Width);
				}
				if (co.Bounds.Height < 0)
				{
					rect.Y += co.Bounds.Height;
					rect.Height = Math.Abs(co.Bounds.Height);
				}
			}
			if (co.Brush != null)
			{
				g.FillRectangle(co.Brush, rect);
			}
			if (co.Pen != null)
			{
				g.DrawRectangle(co.Pen, rect.Left, rect.Top, rect.Width, rect.Height);
			}
		}

		#region 交差

		public override bool Contains(Matrix matrix, float angle, PointF pt)
		{
			if (this.brush != null) return base.Contains(matrix, angle, pt);
			Matrix m = matrix.Clone();
			this.Rotate(m, angle);
			float sc = Geometry.GetScale(m);
			PointF[] pts = Geometry.ConvertToPoints(this.rect);
			m.TransformPoints(pts);
			m.Dispose();
			int len = pts.Length;
			for (int i = 0; i < len; i++)
			{
				int j = i + 1;
				if (j == len) j = 0;
				if (CanvasLine.Contains(this, sc, pts[i], pts[j], pt)) return true;
			}
			return false;
		}

		public override bool IntersectsWith(Matrix matrix, float angle, RectangleF rect)
		{
			if (this.brush != null) return base.IntersectsWith(matrix, angle, rect);
			Matrix m = matrix.Clone();
			this.Rotate(m, angle);
			m.Invert();
			PointF[] pts1 = Geometry.ConvertToPoints(rect);
			m.TransformPoints(pts1);
			m.Dispose();
			PointF[] pts2 = Geometry.ConvertToPoints(this.rect);
			if (Geometry.IntersectsWith(Geometry.ConvertToEquations(pts1), Geometry.ConvertToEquations(pts2))) return true;
			return Geometry.Contains(pts1, pts2);
		}

		#endregion

		protected override void SetClientRectangles()
		{
			this.SetClientRectangle();
			if (this.brush != null)
			{
				this.SetDefaultClientRectangles();
				return;
			}
			PointF[] pts = Geometry.ConvertToPoints(this.crect);
			this.crects = new RectangleF[]
			{
				new RectangleF(pts[0].X, pts[0].Y, pts[1].X - pts[0].X, 0), new RectangleF(pts[1].X, pts[1].Y, 0, pts[2].Y - pts[1].Y), new RectangleF(pts[2].X, pts[2].Y, pts[3].X - pts[2].X, 0), new RectangleF(pts[3].X, pts[3].Y, 0, pts[0].Y - pts[3].Y)
			}
			;
		}
	}
}
