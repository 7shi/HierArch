// ここにソースコードの注釈を書きます。

using System;
using System.Drawing;

namespace Girl.Windows.Forms.Drawing2D
{
	public class CanvasSeparator : CanvasObject
	{
		public CanvasSeparator() : base() {}
		public CanvasSeparator(float x, float y, float width, float height) : base(x, y, width, height) {}
		public CanvasSeparator(RectangleF rect) : base(rect) {}

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
				g.DrawLine(pen, x1 + 2F, y1, x1 + 2F, y2 - 1F);
			}
			else
			{
				g.DrawLine(pen, x1, y1 + 2F, x2 - 1F, y1 + 2F);
			}
		}
	}
}
