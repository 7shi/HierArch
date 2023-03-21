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
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CanvasStringDialog";
            Opacity = 0.9;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        }

        public Point GetCenterPoint()
        {
            Point ret = Location;
            ret.Offset(Width / 2, Height / 2);
            return ret;
        }

        public void SetCenterPoint(Point pos)
        {
            int w = Width, h = Height;
            Bounds = AdjustToScreen(new Rectangle(pos.X - (w / 2), pos.Y - (h / 2), w, h));
        }

        public void SetCenterPoint(Control parent, Point pos)
        {
            int w = Width, h = Height;
            Bounds = AdjustToScreen(AdjustToRectangle(new Rectangle(pos.X - (w / 2), pos.Y - (h / 2), w, h), parent.RectangleToScreen(parent.Bounds)));
        }

        public static Rectangle AdjustToScreen(Rectangle bounds)
        {
            return AdjustToRectangle(bounds, Screen.GetWorkingArea(bounds));
        }

        public static Rectangle AdjustToRectangle(Rectangle bounds, Rectangle area)
        {
            if (bounds.Right > area.Right)
            {
                bounds.X -= bounds.Right - area.Right;
            }

            if (bounds.Bottom > area.Bottom)
            {
                bounds.Y -= bounds.Bottom - area.Bottom;
            }

            if (bounds.Left < area.Left)
            {
                bounds.X += area.Left - bounds.Left;
            }

            if (bounds.Top < area.Top)
            {
                bounds.Y += area.Top - bounds.Top;
            }

            return bounds;
        }
    }
}
