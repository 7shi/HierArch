using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// MouseWheel イベントをフィルタします。
	/// </summary>
	public class MouseWheelMessageFilter : IMessageFilter
	{
		#region Win32
		
		public const int WM_MOUSEWHEEL = 0x020A;
		
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int x, y;
		}
		
		[DllImport("User32.dll")]
		public static extern IntPtr WindowFromPoint(
			POINT p  // 座標
		);
		
		[DllImport("User32.dll")]
		public static extern IntPtr SendMessage(
			IntPtr hWnd,    // 送信先ウィンドウのハンドル
			int Msg,        // メッセージ
			IntPtr wParam,  // メッセージの最初のパラメータ
			IntPtr lParam   // メッセージの 2 番目のパラメータ
		);
		
		#endregion

		public bool PreFilterMessage(ref Message m)
		{
			if (m.Msg != WM_MOUSEWHEEL) return false;
			Point p = Cursor.Position;
			POINT pp = new POINT();
			pp.x = p.X;
			pp.y = p.Y;
			IntPtr hWnd = WindowFromPoint(pp);
			if (hWnd == m.HWnd || hWnd == IntPtr.Zero) return false;
			SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
			return true;
		}
	}
}
