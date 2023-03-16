// ここにソースコードの注釈を書きます。

using System;
using System.Drawing;

namespace Girl.Windows.Forms.Drawing2D
{
	public class CanvasGrip : CanvasObject
	{
		public CanvasGrip() : base() {}
		public CanvasGrip(float x, float y, float width, float height) : base(x, y, width, height) {}
		public CanvasGrip(RectangleF rect) : base(rect) {}

		protected override void Init()
		{
			base.Init();
			this.pen = SystemPens.ControlDark;
		}

		public override void Draw(Graphics g)
		{
			if (!this.CheckDraw(g)) return;
			float x1 = this.rect.Left, y1 = this.rect.Top;
			float x2 = this.rect.Right, y2 = this.rect.Bottom;
			if (this.Width < this.Height)
			{
				for (float y = y1; y < y2; y += 2F)
				{
					g.DrawLine(pen, x1, y, x2 - 1F, y);
				}
			}
			else
			{
				for (float x = x1; x < x2; x += 2F)
				{
					g.DrawLine(pen, x, y1, x, y2 - 1F);
				}
			}
		}
	}
}
