// ここにソースコードの注釈を書きます。

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class CenterPointDialog : Form
	{
		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public CenterPointDialog()
		{
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CanvasStringDialog";
			this.Opacity = 0.9;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
		}

		public Point GetCenterPoint()
		{
			Point ret = this.Location;
			ret.Offset(this.Width / 2, this.Height / 2);
			return ret;
		}

		public void SetCenterPoint(Point pos)
		{
			int w = this.Width, h = this.Height;
			this.Bounds = AdjustToScreen(new Rectangle(pos.X - w / 2, pos.Y - h / 2, w, h));
		}

		public void SetCenterPoint(Control parent, Point pos)
		{
			int w = this.Width, h = this.Height;
			this.Bounds = AdjustToScreen(AdjustToRectangle(new Rectangle(pos.X - w / 2, pos.Y - h / 2, w, h), parent.RectangleToScreen(parent.Bounds)));
		}

		public static Rectangle AdjustToScreen(Rectangle bounds)
		{
			return AdjustToRectangle(bounds, Screen.GetWorkingArea(bounds));
		}

		public static Rectangle AdjustToRectangle(Rectangle bounds, Rectangle area)
		{
			if (bounds.Right > area.Right) bounds.X -= bounds.Right - area.Right;
			if (bounds.Bottom > area.Bottom) bounds.Y -= bounds.Bottom - area.Bottom;
			if (bounds.Left < area.Left) bounds.X += area.Left - bounds.Left;
			if (bounds.Top < area.Top) bounds.Y += area.Top - bounds.Top;
			return bounds;
		}
	}
}
