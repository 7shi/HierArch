// ここにソースコードの注釈を書きます。

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Girl.Drawing;

namespace Girl.Windows.Forms.Drawing2D
{
	public class CanvasImage : CanvasObject
	{
		public CanvasImage() : base() {}
		public CanvasImage(Image image) : this(0, 0, image) {}
		public CanvasImage(PointF pt, Image image) : this(pt.X, pt.Y, image) {}
		
		public CanvasImage(float x, float y, Image image) : base(x, y, 0F, 0F)
		{
			this.Image = image;
		}
		protected Image image;

		protected override void Init()
		{
			base.Init();
			this.image = null;
		}

		public override void Draw(Graphics g)
		{
			if (!this.CheckDraw(g) || this.image == null) return;
			RectangleF r = this.rect;
			try
			{
				Matrix old = g.Transform;
				Matrix m = old.Clone();
				if (r.Width < 0)
				{
					r.X = -r.X;
					r.Width = -r.Width;
					m.Scale(-1F, 1F);
				}
				if (r.Height < 0)
				{
					r.Y = -r.Y;
					r.Height = -r.Height;
					m.Scale(1F, -1F);
				}
				g.Transform = m;
				g.DrawImage(this.image, r);
				g.Transform = old;
				m.Dispose();
			}
			catch
			{
				CanvasRectangle.DrawRectangle(this, g);
			}
		}

		public Image Image
		{
			get
			{
				return this.image;
			}

			set
			{
				this.image = value;
				this.Size = value.Size;
			}
		}
	}
}
