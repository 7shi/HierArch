using System;
using System.Drawing;
using System.Windows.Forms;
using Girl.Windows.API;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// MouseWheel イベントをフィルタします。
	/// </summary>
	public class MouseWheelMessageFilter : IMessageFilter
	{
		public bool PreFilterMessage(ref Message m)
		{
			if (m.Msg != (int)Win32API.WM.MouseWheel) return false;

			Point p = Cursor.Position;
			Win32API.Point pp = new Win32API.Point();
			pp.x = p.X;
			pp.y = p.Y;
			IntPtr hWnd = Win32API.WindowFromPoint(pp);
			if (hWnd == m.HWnd || hWnd == IntPtr.Zero) return false;

			Win32API.SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
			return true;
		}
	}
}
