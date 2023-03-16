using System;
using System.Runtime.InteropServices;

namespace Girl.Windows.API
{
	/// <summary>
	/// Win32API を呼び出すためのラッパークラスです。
	/// </summary>
	public class Win32API
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct Point
		{
			public int x, y;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Size
		{
			public int cx, cy;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Rect
		{
			public int left, top, right, bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ScrollInfo
		{
			public uint cbSize, fMask;
			public int nMin, nMax;
			public uint nPage;
			public int nPos, nTrackPos;
		}

		public enum WM
		{
			Paint      = 0x000F,
			SetFont    = 0x0030,
			HScroll    = 0x0114,
			VScroll    = 0x0115,
			MouseWheel = 0x020A
		};

		public enum GWL
		{
			Style   = -16,
			ExStyle = -20,
			ID      = -12
		};

		public enum WS
		{
			VScroll = 0x00200000,
			HScroll = 0x00100000
		};

		public enum SB
		{
			Horz          = 0,
			Vert          = 1,
			Ctl           = 2,
			Both          = 3,

			LineUp        = 0,
			LineDown      = 1,
			LineLeft      = 0,
			LineRight     = 1,
			PageUp        = 2,
			PageDown      = 3,
			PageLeft      = 2,
			PageRight     = 3,
			ThumbPosition = 4,
			ThumbTrack    = 5,
			EndScroll     = 8,
			Left          = 6,
			Right         = 7,
			Bottom        = 7,
			Top           = 6
		};

		public enum SIF
		{
			Range           = 0x0001,
			Page            = 0x0002,
			Pos             = 0x0004,
			DisableNoScroll = 0x0008,
			TrackPos        = 0x0010,
			All             = Range | Page | Pos | TrackPos
		};

		[DllImport("User32.dll")]
		public static extern IntPtr SendMessage(
			IntPtr hWnd,    // 送信先ウィンドウのハンドル
			int Msg,        // メッセージ
			IntPtr wParam,  // メッセージの最初のパラメータ
			IntPtr lParam   // メッセージの 2 番目のパラメータ
		);

		[DllImport("User32.dll")]
		public static extern long GetWindowLong(
			IntPtr hWnd,  // ウィンドウのハンドル
			int nIndex    // 取得する値のオフセット
		);

		[DllImport("User32.dll")]
		public static extern int ScrollWindowEx(
			IntPtr hWnd,         // ウィンドウのハンドル
			int dx,              // 水平方向のスクロール量
			int dy,              // 垂直方向のスクロール量
			Rect[] prcScroll,    // クライアント領域
			Rect[] prcClip,      // クリッピング長方形
			IntPtr hrgnUpdate,   // 更新リージョンのハンドル
			Rect[] prcUpdate,    // 無効にするべきリージョン
			uint flags           // スクロールオプション
		);

		[DllImport("User32.dll")]
		public static extern bool GetScrollInfo(
			IntPtr hwnd,         // ウィンドウのハンドル
			int fnBar,           // スクロールバーのタイプ
			ref ScrollInfo lpsi  // スクロールバーのパラメータ
		);

		[DllImport("User32.dll")]
		public static extern int SetScrollInfo(
			IntPtr hwnd,          // ウィンドウのハンドル
			int fnBar,            // スクロールバーのタイプ
			ref ScrollInfo lpsi,  // スクロール操作の詳細
			bool fRedraw          // 再描画フラグ
		);

		[DllImport("User32.dll")]
		public static extern IntPtr WindowFromPoint(
			Point p  // 座標
		);

		[DllImport("GDI32.dll")]
		public static extern IntPtr SelectObject(
			IntPtr hdc,     // デバイスコンテキストのハンドル
			IntPtr hgdiobj  // オブジェクトのハンドル
		);

		[DllImport("GDI32.dll", CharSet=CharSet.Unicode)]
		public static extern bool GetTextExtentPoint32(
			IntPtr hdc,       // デバイスコンテキストのハンドル
			string lpString,  // 文字列
			int cbString,     // 文字列内の文字数
			ref Size lpSize   // 文字列のサイズ
		);

		[DllImport("GDI32.dll", CharSet=CharSet.Unicode)]
		public static extern bool TextOut(
			IntPtr hdc,       // デバイスコンテキストのハンドル
			int nXStart,      // 開始位置（基準点）の x 座標
			int nYStart,      // 開始位置（基準点）の y 座標
			string lpString,  // 文字列
			int cbString      // 文字数
		);
	}
}
