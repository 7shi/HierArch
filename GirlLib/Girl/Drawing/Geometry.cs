using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Girl.Drawing
{
	/// <summary>
	/// 幾何計算を行います。
	/// </summary>
	public class Geometry
	{
		public struct LineEquation
		{
			public PointF pt1, pt2;
			public float a, b, c;
			
			public LineEquation(PointF pt1, PointF pt2)
			{
				this.pt1 = pt1;
				this.pt2 = pt2;
				this.a = pt2.Y - pt1.Y;
				this.b = pt1.X - pt2.X;
				this.c = pt2.X * pt1.Y - pt1.X * pt2.Y;
			}
			
			public LineEquation(float x1, float y1, float x2, float y2)
				: this(new PointF(x1, y1), new PointF(x2, y2))
			{
			}
			
			public LineEquation(RectangleF rect)
				: this(rect.Location, new PointF(rect.Right, rect.Bottom))
			{
			}
			
			public bool IntersectsWith(LineEquation line)
			{
				float x, y;
				if (this.b != 0)
				{
					float d = this.a * line.b - line.a * this.b;
					if (d == 0) return false;
					x = (this.b * line.c - line.b * this.c) / d;
					y = -(this.c + this.a * x) / this.b;
				}
				else if (this.a != 0)
				{
					float d = line.a * this.b - this.a * line.b;
					if (d == 0) return false;
					y = (this.a * line.c - line.a * this.c) / d;
					x = -(this.c + this.b * y) / this.a;
				}
				else
				{
					return false;
				}
				return this.Contains(x, y) && line.Contains(x, y);
			}
			
			public bool IntersectsWith(LineEquation[] lines)
			{
				foreach (LineEquation line in lines)
				{
					if (this.IntersectsWith(line)) return true;
				}
				return false;
			}
			
			public PointF GetIntersectionPoint(LineEquation line)
			{
				float x, y;
				if (this.b != 0)
				{
					float d = this.a * line.b - line.a * this.b;
					if (d == 0) return PointF.Empty;
					x = (this.b * line.c - line.b * this.c) / d;
					y = -(this.c + this.a * x) / this.b;
				}
				else if (this.a != 0)
				{
					float d = line.a * this.b - this.a * line.b;
					if (d == 0) return PointF.Empty;
					y = (this.a * line.c - line.a * this.c) / d;
					x = -(this.c + this.b * y) / this.a;
				}
				else
				{
					return PointF.Empty;
				}
				return new PointF(x, y);
			}
			
			public bool Contains(PointF pt)
			{
				return this.Contains(pt.X, pt.Y);
			}
			
			public bool Contains(float x, float y)
			{
				return (pt1.X == pt2.X)
					? Math.Min(pt1.Y, pt2.Y) <= y && y <= Math.Max(pt1.Y, pt2.Y)
					: Math.Min(pt1.X, pt2.X) <= x && x <= Math.Max(pt1.X, pt2.X);
			}
			
			public float GetDistance(PointF pt)
			{
				if (pt1 == pt2)
				{
					return Geometry.GetDistance(pt1, pt);
				}
				return (float)(Math.Abs(this.a * pt.X + this.b * pt.Y + this.c)
					/ Math.Sqrt(a * a + b * b));
			}
			
			public LineEquation GetCrossLine(PointF pt)
			{
				LineEquation ret;
				ret.a = this.b;
				ret.b = -this.a;
				ret.c = this.a * pt.Y - this.b * pt.X;
				ret.pt1 = pt;
				ret.pt2 = PointF.Empty;
				ret.pt2 = this.GetIntersectionPoint(ret);
				return ret;
			}
		
			public float Length
			{
				get
				{
					return Geometry.GetDistance(this.pt1, this.pt2);
				}
			}
		
			public PointF[] SplitPoints(float space)
			{
				int len = (int)(this.Length / space) + 1;
				PointF[] ret = new PointF[len];
				ret[0] = this.pt1;
				float dx = pt2.X - pt1.X, dy = pt2.Y - pt1.Y;
				for (int i = 1; i < len; i++)
				{
					float d = (float)i / (float)len;
					ret[i] = new PointF(pt1.X + dx * d, pt1.Y + dy * d);
				}
				return ret;
			}
		
			public PointF GetRoutePoint(float distance)
			{
				float len = this.Length;
				float dx = pt2.X - pt1.X, dy = pt2.Y - pt1.Y;
				return new PointF(pt1.X + dx * distance / len, pt1.Y + dy * distance / len);
			}
		}

		#region アフィン変換

		/// <summary>
		/// Matrix のスケールを求めます。
		/// </summary>
		public static float GetScale(Matrix matrix)
		{
			PointF[] ptfs = new PointF[]
			{
				new PointF(1F, 0F)
			}
			;
			matrix.TransformVectors(ptfs);
			double x =(double) ptfs[0].X, y =(double) ptfs[0].Y;
			return (float) Math.Sqrt(x * x + y * y);
		}

		/// <summary>
		/// Matrix の回転角度を求めます。
		/// </summary>
		public static float GetAngle(Matrix matrix)
		{
			PointF[] ptfs = new PointF[]
			{
				new PointF(1F, 0F)
			}
			;
			matrix.TransformVectors(ptfs);
			return (float) (Math.Atan2((double) ptfs[0].Y,(double) ptfs[0].X) * 180D / Math.PI);
		}

		public static PointF TransformPoint(Matrix matrix, PointF pt)
		{
			PointF[] pts = new PointF[]
			{
				pt
			}
			;
			matrix.TransformPoints(pts);
			return pts[0];
		}

		public static PointF TransformPoint(Matrix matrix, float x, float y)
		{
			return TransformPoint(matrix, new PointF(x, y));
		}

		/// <summary>
		/// アフィン変換された長方形に外接する長方形を求めます。
		/// </summary>
		public static RectangleF TransformRectangle(Matrix matrix, RectangleF rect)
		{
			PointF[] pts = ConvertToPoints(rect);
			matrix.TransformPoints(pts);
			return ConvertToRectangle(pts);
		}

		public static void ScaleAt(Matrix matrix, float scaleX, float scaleY, PointF pt)
		{
			PointF pt1 = TransformPoint(matrix, pt);
			matrix.Scale(scaleX, scaleY);
			Matrix m = matrix.Clone();
			m.Invert();
			PointF pt2 = TransformPoint(m, pt1);
			m.Dispose();
			matrix.Translate(pt2.X - pt.X, pt2.Y - pt.Y);
		}

		#endregion

		#region 座標計算

		/// <summary>
		/// 中点を求めます。
		/// </summary>
		public static PointF GetCenter(PointF pt1, PointF pt2)
		{
			return new PointF((pt1.X + pt2.X) / 2,(pt1.Y + pt2.Y) / 2);
		}

		/// <summary>
		/// 重心を求めます。
		/// </summary>
		public static PointF GetCenter(RectangleF rect)
		{
			return new PointF(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
		}

		/// <summary>
		/// 重心を求めます。
		/// </summary>
		public static PointF GetCenter(PointF[] poly)
		{
			float len =(float) poly.Length, x = 0F, y = 0F;
			foreach (PointF pt in poly)
			{
				x += pt.X;
				y += pt.Y;
			}
			return new PointF(x / len, y / len);
		}

		/// <summary>
		/// 2点間の距離を求めます。
		/// </summary>
		public static float GetDistance(PointF pt1, PointF pt2)
		{
			double dx =(double) (pt1.X - pt2.X), dy =(double) (pt1.Y - pt2.Y);
			return (float) Math.Sqrt(dx * dx + dy * dy);
		}

		/// <summary>
		/// 四角形に点が含まれるよう拡大します。
		/// </summary>
		public static Rectangle ExpandRectangle(Rectangle rect, Point pt)
		{
			if (pt.X < rect.Left)
			{
				int dx = rect.Left - pt.X;
				rect.X = pt.X;
				rect.Width += dx;
			}
			else if (pt.X > rect.Right)
			{
				rect.Width += pt.X - rect.Right;
			}
			if (pt.Y < rect.Top)
			{
				int dy = rect.Top - pt.Y;
				rect.Y = pt.Y;
				rect.Height += dy;
			}
			else if (pt.Y > rect.Bottom)
			{
				rect.Height += pt.Y - rect.Bottom;
			}
			return rect;
		}

		/// <summary>
		/// 四角形に点が含まれるよう拡大します。
		/// </summary>
		public static Rectangle ExpandRectangle(Rectangle rect, int x, int y)
		{
			return ExpandRectangle(rect, new Point(x, y));
		}

		public static Rectangle ExpandRectangle(Rectangle rect, Point[] pts)
		{
			foreach (Point pt in pts) rect = ExpandRectangle(rect, pt);
			return rect;
		}

		/// <summary>
		/// 四角形に点が含まれるよう拡大します。
		/// </summary>
		public static RectangleF ExpandRectangle(RectangleF rect, PointF pt)
		{
			if (pt.X < rect.Left)
			{
				float dx = rect.Left - pt.X;
				rect.X = pt.X;
				rect.Width += dx;
			}
			else if (pt.X > rect.Right)
			{
				rect.Width += pt.X - rect.Right;
			}
			if (pt.Y < rect.Top)
			{
				float dy = rect.Top - pt.Y;
				rect.Y = pt.Y;
				rect.Height += dy;
			}
			else if (pt.Y > rect.Bottom)
			{
				rect.Height += pt.Y - rect.Bottom;
			}
			return rect;
		}

		/// <summary>
		/// 四角形に点が含まれるよう拡大します。
		/// </summary>
		public static RectangleF ExpandRectangle(RectangleF rect, float x, float y)
		{
			return ExpandRectangle(rect, new PointF(x, y));
		}

		public static RectangleF ExpandRectangle(RectangleF rect, PointF[] pts)
		{
			foreach (PointF pt in pts) rect = ExpandRectangle(rect, pt);
			return rect;
		}

		/// <summary>
		/// 多角形に外接する長方形を求めます。
		/// </summary>
		public static RectangleF ConvertToRectangle(PointF[] poly)
		{
			if (poly == null || poly.Length < 0) return RectangleF.Empty;
			RectangleF ret = new RectangleF(poly[0], SizeF.Empty);
			int len = poly.Length;
			for (int i = 1; i < len; i++) ret = ExpandRectangle(ret, poly[i]);
			return ret;
		}

		public static PointF[] ConvertToPoints(RectangleF rect)
		{
			return new PointF[]
			{
				new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Top), new PointF(rect.Right, rect.Bottom), new PointF(rect.Left, rect.Bottom)
			}
			;
		}

		public static LineEquation[] ConvertToEquations(PointF[] poly)
		{
			int len = poly.Length;
			LineEquation[] ret = new LineEquation[len];
			for (int i = 0; i < len; i++)
			{
				int j = i + 1;
				if (j == len) j = 0;
				ret[i] = new LineEquation(poly[i], poly[j]);
			}
			return ret;
		}

		public static float GetAngle(PointF pt1, PointF pt2)
		{
			double dx =(double) (pt2.X - pt1.X), dy =(double) (pt2.Y - pt1.Y);
			return (float) (Math.Atan2(dy, dx) * 180D / Math.PI);
		}

		/// <summary>
		/// 三角形の一辺の角度を求めます。
		/// </summary>
		/// <returns>度数</returns>
		public static float GetAngle(PointF pt1, PointF pt2, PointF pt3)
		{
			float ret = Math.Abs(GetAngle(pt2, pt1) - GetAngle(pt2, pt3));
			return (ret < 180F) ? ret:
			360F - ret;
		}

		/// <summary>
		/// 三角形に点が含まれるかを判断します。
		/// </summary>
		public static bool Contains(PointF pt1, PointF pt2, PointF pt3, PointF pt)
		{
			float a123 = GetAngle(pt1, pt2, pt3);
			if (a123 < GetAngle(pt1, pt2, pt) || a123 < GetAngle(pt, pt2, pt3)) return false;
			float a231 = GetAngle(pt2, pt3, pt1);
			if (a231 < GetAngle(pt2, pt3, pt) || a231 < GetAngle(pt, pt3, pt1)) return false;
			float a312 = GetAngle(pt3, pt1, pt2);
			if (a312 < GetAngle(pt3, pt1, pt) || a312 < GetAngle(pt, pt1, pt2)) return false;
			return true;
		}

		/// <summary>
		/// 長方形の中に四角形が含まれるかを判断します。
		/// </summary>
		public static bool Contains(RectangleF rect, PointF[] poly)
		{
			foreach (PointF pt in poly) if (!rect.Contains(pt)) return false;
			return true;
		}

		/// <summary>
		/// 多角形の中に多角形が含まれるかを判断します。
		/// </summary>
		public static bool Contains(PointF[] poly, PointF pt)
		{
			int len = poly.Length;
			for (int i = 1; i < len - 1; i++)
			{
				if (Contains(poly[0], poly[i], poly[i + 1], pt)) return true;
			}
			return false;
		}

		/// <summary>
		/// 多角形の中に多角形が含まれるかを判断します。
		/// </summary>
		public static bool Contains(PointF[] poly1, PointF[] poly2)
		{
			int len = poly1.Length;
			foreach (PointF pt in poly2)
			{
				if (!Contains(poly1, pt)) return false;
			}
			return true;
		}

		/// <summary>
		/// 長方形と多角形が交差するかを判断します。
		/// </summary>
		public static bool IntersectsWith(RectangleF rect, PointF[] poly)
		{
			return IntersectsWith(ConvertToPoints(rect), poly);
		}

		/// <summary>
		/// 多角形と多角形が交差するかを判断します。
		/// </summary>
		public static bool IntersectsWith(LineEquation[] lines1, LineEquation[] lines2)
		{
			foreach (LineEquation line in lines1)
			{
				if (line.IntersectsWith(lines2)) return true;
			}
			return false;
		}

		/// <summary>
		/// 多角形と多角形が交差するかを判断します。
		/// </summary>
		public static bool IntersectsWith(PointF[] poly1, PointF[] poly2)
		{
			if (IntersectsWith(ConvertToEquations(poly1), ConvertToEquations(poly2))) return true;
			return Contains(poly1, poly2) || Contains(poly2, poly1);
		}

		public static void Offset(ref PointF pt, float dx, float dy)
		{
			pt.X += dx;
			pt.Y += dy;
		}

		public static void Offset(PointF[] pts, float dx, float dy)
		{
			if (pts == null) return;
			int len = pts.Length;
			if (len < 1) return;
			for (int i = 0; i < len; i++)
			{
				pts[i].X += dx;
				pts[i].Y += dy;
			}
		}

		/// <summary>
		/// 点を中心から指定した距離だけずらします。
		/// </summary>
		public static PointF Inflate(float x, float y, float cx, float cy, float dx, float dy, float signX, float signY)
		{
			float cdx = x - cx, cdy = y - cy;
			float sx = Math.Sign(cdx), sy = Math.Sign(cdy);
			if (sx == 0) sx = signX;
			if (sy == 0) sy = signY;
			x = cx + Math.Max(0, Math.Abs(cdx) + dx) * sx;
			y = cy + Math.Max(0, Math.Abs(cdy) + dy) * sy;
			return new PointF(x, y);
		}

		/// <summary>
		/// 最も近くで直角に交わる点を求めます。
		/// </summary>
		public static PointF GetRightAngled(PointF pt, PointF[] pts, float min)
		{
			PointF ret = pt;
			int len = pts.Length;
			float [] dxs = new float [len], dys = new float [len];
			for (int i = 0; i < len; i++)
			{
				dxs[i] = pts[i].X - pt.X;
				dys[i] = pts[i].Y - pt.Y;
			}
			int ix = GetMinimumIndex(dxs);
			int iy = GetMinimumIndex(dys);
			float dx = Math.Abs(dxs[ix]);
			float dy = Math.Abs(dys[iy]);
			if (dx <= min || dx <= dy)
			{
				ret.X = pts[ix].X;
			}
			if (dy <= min || dy <= dx)
			{
				ret.Y = pts[iy].Y;
			}
			return ret;
		}

		/// <summary>
		/// 最も近くで直角に交わる点を求めます。
		/// </summary>
		public static PointF GetRightAngled(PointF pt, PointF[] pts)
		{
			return GetRightAngled(pt, pts, 0);
		}

		/// <summary>
		/// 直角に交わる点を求めます。
		/// </summary>
		public static PointF GetRightAngled(PointF pt1, PointF pt2)
		{
			PointF ret = pt1;
			float dx = pt2.X - pt1.X, dy = pt2.Y - pt1.Y;
			if (Math.Abs(dx) < Math.Abs(dy))
			{
				ret.X = pt2.X;
			}
			else
			{
				ret.Y = pt2.Y;
			}
			return ret;
		}

		/// <summary>
		/// 最も絶対値の小さい値のインデックスを返します。
		/// </summary>
		public static int GetMinimumIndex(float[] nums)
		{
			int ret = 0;
			float min = Math.Abs(nums[0]);
			for (int i = 1; i < nums.Length; i++)
			{
				float n = Math.Abs(nums[i]);
				if (min <= n) continue;
				min = n;
				ret = i;
			}
			return ret;
		}

		#endregion
	}
}
