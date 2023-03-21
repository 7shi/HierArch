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
        private Control control;
        protected Win32API.SB fnBar;
        protected Win32API.WS style;
        protected Win32API.WM message;

        public InternalScrollBar(Control ctrl)
        {
            this.control = ctrl;
        }

        public Win32API.ScrollInfo GetScrollInfo(Win32API.SIF fMask)
        {
            Win32API.ScrollInfo ret = new Win32API.ScrollInfo();
            ret.cbSize = 28;
            ret.fMask = (uint)fMask;
            if (Visible) Win32API.GetScrollInfo(this.control.Handle, (int)this.fnBar, ref ret);
            return ret;
        }

        public bool Visible
        {
            get
            {
                int style = Win32API.GetWindowLong(this.control.Handle, (int)Win32API.GWL.Style);
                return (style & (int)this.style) != 0;
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
                Win32API.SetScrollInfo(this.control.Handle, (int)this.fnBar, ref si2, true);
                Win32API.SendMessage(this.control.Handle, (int)this.message, (IntPtr)(((int)Win32API.SB.ThumbPosition) + ((si2.nPos & 0xffff) << 16)), IntPtr.Zero);
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
                    Win32API.SendMessage(this.control.Handle, (int)this.message, (IntPtr)Win32API.SB.LineDown, IntPtr.Zero);
                }
            }
        }

        public void Decrease(int num)
        {
            for (int i = 0; i < num; i++)
            {
                if (CanDecrease)
                {
                    Win32API.SendMessage(this.control.Handle, (int)this.message, (IntPtr)Win32API.SB.LineUp, IntPtr.Zero);
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
            this.fnBar = Win32API.SB.Horz;
            this.style = Win32API.WS.HScroll;
            this.message = Win32API.WM.HScroll;
        }
    }

    /// <summary>
    /// ウィンドウ内部の縦スクロールバーをラップします。
    /// </summary>
    public class InternalVScrollBar : InternalScrollBar
    {
        public InternalVScrollBar(Control ctrl) : base(ctrl)
        {
            this.fnBar = Win32API.SB.Vert;
            this.style = Win32API.WS.VScroll;
            this.message = Win32API.WM.VScroll;
        }
    }
}
