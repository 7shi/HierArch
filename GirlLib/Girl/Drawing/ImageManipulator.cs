// ここにソースコードの注釈を書きます。

using System;
using System.Collections;
using System.Drawing;

namespace Girl.Drawing
{
	public enum PrimaryColors
	{
		Red, Green, Blue
	}
}

namespace Girl.Drawing
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class ImageManipulator
	{
		private static Point[] dirs = new Point[]
			{
				new Point( 0,  0),
				new Point( 0, -1),
				new Point( 1, -1),
				new Point( 1,  0),
				new Point( 1,  1),
				new Point( 0,  1),
				new Point(-1,  1),
				new Point(-1,  0),
				new Point(-1, -1)
			};

		#region Color

		public static Color Swap(Color c, PrimaryColors r, PrimaryColors g, PrimaryColors b)
		{
			return Color.FromArgb(c.A, GetPrimary(c, r), GetPrimary(c, g), GetPrimary(c, b));
		}

		public static byte GetPrimary(Color c, PrimaryColors p)
		{
			if (p == PrimaryColors.Red)
			{
				return c.R;
			}
			else if (p == PrimaryColors.Green)
			{
				return c.G;
			}
			else
			{
				return c.B;
			}
		}

		public static Color Invert(Color c)
		{
			return Color.FromArgb(c.A, 255 - c.R, 255 - c.G, 255 - c.B);
		}

		public static Color Mix(Color c1, Color c2)
		{
			return Color.FromArgb((c1.A + c2.A) / 2,(c1.R + c2.R) / 2,(c1.G + c2.G) / 2,(c1.B + c2.B) / 2);
		}

		public static Color Mix(Color c1, int r1, Color c2, int r2)
		{
			return Color.FromArgb((c1.A * r1 + c2.A * r2) /(r1 + r2),(c1.R * r1 + c2.R * r2) /(r1 + r2),(c1.G * r1 + c2.G * r2) /(r1 + r2),(c1.B * r1 + c2.B * r2) /(r1 + r2));
		}

		#endregion

		#region Bitmap

		public static Rectangle[] SplitBitmap(Image img, Color back)
		{
			Bitmap bmp = new Bitmap(img);
			int w = bmp.Width, h = bmp.Height;
			ArrayList list = new ArrayList();
			for (int x = 0; x < w; x++)
			{
				for (int y = 0; y < h; y++)
				{
					if (bmp.GetPixel(x, y).ToArgb() == back.ToArgb()) continue;
					Rectangle r = ChasePixel(new Rectangle(x, y, 0, 0), bmp, back, x, y);
					r.Width++;
					r.Height++;
					list.Add(r);
				}
			}
			bmp.Dispose();
			if (list.Count < 1) return null;
			return list.ToArray(typeof (Rectangle)) as Rectangle[];
		}

		public static Rectangle ChasePixel(Rectangle rect, Bitmap bmp, Color back, int x, int y)
		{
			if (x < 0 || y < 0 || x >= bmp.Width || y >= bmp.Height || bmp.GetPixel(x, y).ToArgb() == back.ToArgb()) return rect;
			rect = Geometry.ExpandRectangle(rect, x, y);
			bmp.SetPixel(x, y, back);
			for (int i = 1; i <= 8; i++)
			{
				Point d = dirs[i];
				rect = ChasePixel(rect, bmp, back, x + d.X, y + d.Y);
			}
			return rect;
		}

		public static Bitmap[] GetBitmaps(Image img, Rectangle[] rects)
		{
			if (rects == null) return null;
			int len = rects.Length;
			if (len < 1) return null;
			Bitmap[] ret = new Bitmap[len];
			for (int i = 0; i < len; i++)
			{
				ret[i] = new Bitmap(rects[i].Width, rects[i].Height);
				Graphics g = Graphics.FromImage(ret[i]);
				g.DrawImage(img, 0, 0, rects[i], GraphicsUnit.Pixel);
				g.Dispose();
			}
			return ret;
		}

		public static bool CompareBitmaps(Bitmap bmp1, Bitmap bmp2)
		{
			int w1 = bmp1.Width, h1 = bmp1.Height;
			int w2 = bmp2.Width, h2 = bmp2.Height;
			if (w1 != w2 || h1 != h2) return false;
			for (int y = 0; y < h1; y++)
			{
				for (int x = 0; x < w1; x++)
				{
					if (bmp1.GetPixel(x, y).ToArgb() != bmp2.GetPixel(x, y).ToArgb())
					{
						return false;
					}
				}
			}
			return true;
		}

		#endregion
	}
}
