// ここにソースコードの注釈を書きます。

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Girl.Windows.Forms.Drawing2D
{
	public class CanvasCorner : CanvasObject
	{
		protected static Brush selBrush = null;
		
		public CanvasCorner(CanvasObject target)
			: base(0F, 0F, 6F, 6F)
		{
			this.Selection = null;
			this.Index = 0;
			this.target = target;
			this.cursor = Cursors.Default;
			this.type = 'R';
			this.pen = SystemPens.Highlight;
			if (CanvasCorner.selBrush == null)
			{
				CanvasCorner.selBrush = new SolidBrush(MenuItemEx.GetSelectionColor(true));
			}
			this.brush = target.Resizable ? CanvasCorner.selBrush : Brushes.White;
		}
		
		public CanvasCorner(CanvasObject target, char type)
			: this(target)
		{
			this.type = type;
			if (type == 'L') this.brush = Brushes.White;
		}
		
		public CanvasCorner(CanvasObject target, char type, Cursor cursor)
			: this(target, type)
		{
			this.cursor = cursor;
		}
		public CanvasObjectSelection Selection;
		public int Index;
		protected CanvasObject target;
		protected Cursor cursor;
		protected char type;

		public override void Draw(Graphics g)
		{
			if (!this.CheckDraw(g)) return;
			if (type == 'E')
			{
				CanvasEllipse.DrawEllipse(this, g);
			}
			else
			{
				CanvasRectangle.DrawRectangle(this, g);
			}
		}

		public CanvasObject Target
		{
			get
			{
				return this.target;
			}
		}

		public Cursor Cursor
		{
			get
			{
				return this.cursor;
			}

			set
			{
				this.cursor = value;
			}
		}

		public char Type
		{
			get
			{
				return this.type;
			}
		}
	}
}
