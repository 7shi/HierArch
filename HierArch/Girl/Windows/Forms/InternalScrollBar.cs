using Girl.Windows.API;
using System;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
    /// <summary>
    /// ウィンドウ内部のスクロールバーをラップします。
    /// </summary>
    public class InternalScrollBar
    {
        private readonly Control control;
        protected Win32API.SB fnBar;
        protected Win32API.WS style;
        protected Win32API.WM message;

        public InternalScrollBar(Control ctrl)
        {
            control = ctrl;
        }

        public Win32API.ScrollInfo GetScrollInfo(Win32API.SIF fMask)
        {
            Win32API.ScrollInfo ret = new Win32API.ScrollInfo
            {
                cbSize = 28,
                fMask = (uint)fMask
            };
            if (Visible)
            {
                _ = Win32API.GetScrollInfo(control.Handle, (int)fnBar, ref ret);
            }

            return ret;
        }

        public bool Visible
        {
            get
            {
                int style = Win32API.GetWindowLong(control.Handle, (int)Win32API.GWL.Style);
                return (style & (int)this.style) != 0;
            }
        }

        public int Pos
        {
            get => GetScrollInfo(Win32API.SIF.Pos).nPos;

            set
            {
                if (!Visible)
                {
                    return;
                }

                Win32API.ScrollInfo si1 = GetScrollInfo(Win32API.SIF.All);
                Win32API.ScrollInfo si2 = new Win32API.ScrollInfo
                {
                    cbSize = 28,
                    fMask = (uint)Win32API.SIF.Pos,
                    nPos = Math.Min(Math.Max(value, si1.nMin), si1.nMax - Math.Max((int)si1.nPage - 1, 0))
                };
                if (si1.nPos == si2.nPos)
                {
                    return;
                }

                _ = Win32API.SetScrollInfo(control.Handle, (int)fnBar, ref si2, true);
                _ = Win32API.SendMessage(control.Handle, (int)message, (IntPtr)(((int)Win32API.SB.ThumbPosition) + ((si2.nPos & 0xffff) << 16)), IntPtr.Zero);
            }
        }

        public int Min => GetScrollInfo(Win32API.SIF.Range).nMin;

        public int Max => GetScrollInfo(Win32API.SIF.Range).nMax;

        public void Scroll(int d)
        {
            Pos += d;
        }

        public void Increase(int num)
        {
            for (int i = 0; i < num; i++)
            {
                if (CanIncrease)
                {
                    _ = Win32API.SendMessage(control.Handle, (int)message, (IntPtr)Win32API.SB.LineDown, IntPtr.Zero);
                }
            }
        }

        public void Decrease(int num)
        {
            for (int i = 0; i < num; i++)
            {
                if (CanDecrease)
                {
                    _ = Win32API.SendMessage(control.Handle, (int)message, (IntPtr)Win32API.SB.LineUp, IntPtr.Zero);
                }
            }
        }

        public bool CanIncrease
        {
            get
            {
                if (!Visible)
                {
                    return false;
                }

                Win32API.ScrollInfo si = GetScrollInfo(Win32API.SIF.All);
                return si.nPos < si.nMax - Math.Max(si.nPage - 1, 0);
            }
        }

        public bool CanDecrease
        {
            get
            {
                if (!Visible)
                {
                    return false;
                }

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
            fnBar = Win32API.SB.Horz;
            style = Win32API.WS.HScroll;
            message = Win32API.WM.HScroll;
        }
    }

    /// <summary>
    /// ウィンドウ内部の縦スクロールバーをラップします。
    /// </summary>
    public class InternalVScrollBar : InternalScrollBar
    {
        public InternalVScrollBar(Control ctrl) : base(ctrl)
        {
            fnBar = Win32API.SB.Vert;
            style = Win32API.WS.VScroll;
            message = Win32API.WM.VScroll;
        }
    }
}
