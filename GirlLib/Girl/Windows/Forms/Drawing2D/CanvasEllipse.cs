// ここにソースコードの注釈を書きます。

using System;
using System.Drawing;

namespace Girl.Windows.Forms.Drawing2D
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class CanvasEllipse : CanvasObject
	{
		public CanvasEllipse() : base() {}
		public CanvasEllipse(float x, float y, float width, float height) : base(x, y, width, height) {}
		public CanvasEllipse(RectangleF rect) : base(rect) {}

		public override void Draw(Graphics g)
		{
			if (!this.CheckDraw(g)) return;
			CanvasEllipse.DrawEllipse(this, g);
		}

		public static void DrawEllipse(CanvasObject co, Graphics g)
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
				g.FillEllipse(co.Brush, rect);
			}
			if (co.Pen != null)
			{
				g.DrawEllipse(co.Pen, rect);
			}
		}
	}
}
