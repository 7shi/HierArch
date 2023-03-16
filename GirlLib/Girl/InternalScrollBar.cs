using System;
using System.Windows.Forms;
using Girl.Windows.API;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// ウィンドウ内部のスクロールバーをラップします。
	/// </summary>
	public class InternalScrollBar
	{
		private Control m_Control;
		protected Win32API.SB m_fnBar;
		protected Win32API.WS m_Style;
		protected Win32API.WM m_Msg;

		public InternalScrollBar(Control ctrl)
		{
			m_Control = ctrl;
		}

		public Win32API.ScrollInfo GetScrollInfo(Win32API.SIF fMask)
		{
			Win32API.ScrollInfo ret = new Win32API.ScrollInfo();
			ret.cbSize = 28;
			ret.fMask = (uint)fMask;
			if (Visible) Win32API.GetScrollInfo(m_Control.Handle, (int)m_fnBar, ref ret);
			return ret;
		}

		public bool Visible
		{
			get
			{
				long style = Win32API.GetWindowLong(m_Control.Handle, (int)Win32API.GWL.Style);
				return (style & (long)m_Style) != 0;
			}
		}

		public int Pos
		{
			get
			{
				return GetScrollInfo(Win32API.SIF.Pos).nPos;
			}

			set
			{
				if (!Visible) return;

				Win32API.ScrollInfo si1 = GetScrollInfo(Win32API.SIF.All);
				Win32API.ScrollInfo si2 = new Win32API.ScrollInfo();
				si2.cbSize = 28;
				si2.fMask = (uint)Win32API.SIF.Pos;
				si2.nPos = Math.Min(Math.Max(value, si1.nMin), si1.nMax - Math.Max((int)si1.nPage - 1, 0));
				if (si1.nPos == si2.nPos) return;
				Win32API.SetScrollInfo(m_Control.Handle, (int)m_fnBar, ref si2, true);
				Win32API.SendMessage(m_Control.Handle, (int)m_Msg, (IntPtr)(((int)Win32API.SB.ThumbPosition) + ((si2.nPos & 0xffff) << 16)), IntPtr.Zero);
			}
		}

		public int Min
		{
			get
			{
				return GetScrollInfo(Win32API.SIF.Range).nMin;
			}
		}

		public int Max
		{
			get
			{
				return GetScrollInfo(Win32API.SIF.Range).nMax;
			}
		}

		public void Scroll(int d)
		{
			Pos = Pos + d;
		}

		public void Increase(int num)
		{
			for (int i = 0; i < num; i++) 
			{
				if (CanIncrease)
				{
					Win32API.SendMessage(m_Control.Handle, (int)m_Msg, (IntPtr)Win32API.SB.LineDown, IntPtr.Zero);
				}
			}
		}

		public void Decrease(int num)
		{
			for (int i = 0; i < num; i++)
			{
				if (CanDecrease)
				{
					Win32API.SendMessage(m_Control.Handle, (int)m_Msg, (IntPtr)Win32API.SB.LineUp, IntPtr.Zero);
				}
			}
		}

		public bool CanIncrease
		{
			get
			{
				if (!Visible) return false;

				Win32API.ScrollInfo si = GetScrollInfo(Win32API.SIF.All);
				return si.nPos < si.nMax - Math.Max(si.nPage - 1, 0);
			}
		}

		public bool CanDecrease
		{
			get
			{
				if (!Visible) return false;

				Win32API.ScrollInfo si = GetScrollInfo(Win32API.SIF.All);
				return si.nPos > si.nMin;
			}
		}
	}

	/// <summary>
	/// ウィンドウ内部の横スクロールバーをラップします。
	/// </summary>
	public class InternalHScrollBar : InternalScrollBar
	{
		public InternalHScrollBar(Control ctrl) : base(ctrl)
		{
			m_fnBar = Win32API.SB.Horz;
			m_Style = Win32API.WS.HScroll;
			m_Msg   = Win32API.WM.HScroll;
		}
	}

	/// <summary>
	/// ウィンドウ内部の縦スクロールバーをラップします。
	/// </summary>
	public class InternalVScrollBar : InternalScrollBar
	{
		public InternalVScrollBar(Control ctrl) : base(ctrl)
		{
			m_fnBar = Win32API.SB.Vert;
			m_Style = Win32API.WS.VScroll;
			m_Msg   = Win32API.WM.VScroll;
		}
	}
}
