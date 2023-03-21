using System;
using System.Drawing;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
    /// <summary>
    /// タブ操作を拡張した TabControl です。
    /// </summary>
    public class TabControlEx : TabControl
    {
        public event EventHandler Changed;
        protected ContextMenu tabContextMenu;
        protected int dragTarget;
        protected Point clickPoint;
        protected bool noMove;

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public TabControlEx()
        {
            tabContextMenu = null;
            dragTarget = -1;
            noMove = true;
            clickPoint = Point.Empty;
        }

        public ContextMenu TabContextMenu
        {
            get => tabContextMenu;

            set => tabContextMenu = value;
        }

        public TabPage GetTabPage(int x, int y)
        {
            int index = GetTabIndex(x, y);
            return index < 0 ? null : TabPages[index];
        }

        public int GetTabIndex(int x, int y)
        {
            int len = TabCount;
            for (int i = 0; i < len; i++)
            {
                if (GetTabRect(i).Contains(x, y))
                {
                    return i;
                }
            }
            return -1;
        }

        public void AddTabPage(int prev, TabPage page)
        {
            InsertTabPage(prev + 1, page);
        }

        public void InsertTabPage(int next, TabPage page)
        {
            Control[] ctrls_p = new Control[page.Controls.Count];
            string text_p = page.Text;
            page.Controls.CopyTo(ctrls_p, 0);
            page.Controls.Clear();
            TabPages.Add(page);
            for (int i = TabCount - 1; i >= next; i--)
            {
                Control[] ctrls;
                string text;
                if (i == next)
                {
                    ctrls = ctrls_p;
                    text = text_p;
                }
                else
                {
                    TabPage tp_s = TabPages[i - 1];
                    ctrls = new Control[tp_s.Controls.Count];
                    tp_s.Controls.CopyTo(ctrls, 0);
                    tp_s.Controls.Clear();
                    text = tp_s.Text;
                }
                TabPage tp = TabPages[i];
                tp.Controls.AddRange(ctrls);
                tp.Text = text;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            dragTarget = GetTabIndex(e.X, e.Y);
            noMove = true;
            clickPoint = new Point(e.X, e.Y);
            if (e.Button != MouseButtons.Right || dragTarget == SelectedIndex)
            {
                return;
            }

            SelectedTab = TabPages[dragTarget];
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button != MouseButtons.Left || dragTarget == -1 || (noMove && Math.Abs(e.X - clickPoint.X) < 3 && Math.Abs(e.Y - clickPoint.Y) < 3))
            {
                return;
            }

            noMove = false;
            int len = TabCount;
            Rectangle r_s = GetTabRect(0);
            Rectangle r_e = GetTabRect(len - 1);
            int index;
            if (e.X < r_s.Left + (r_s.Width / 2))
            {
                index = 0;
            }
            else if (e.X > r_e.Right - (r_e.Width / 2))
            {
                index = len;
            }
            else
            {
                index = GetTabIndex(e.X, r_s.Top);
                if (index < 0 || index == dragTarget)
                {
                    return;
                }

                Rectangle r = GetTabRect(index);
                if (e.X > r.X + (r.Width / 2))
                {
                    index++;
                }
            }
            if (index == dragTarget || index - 1 == dragTarget)
            {
                return;
            }

            bool f = Focused, v = Visible;
            Visible = false;
            TabPage tp = TabPages[dragTarget];
            TabPages.Remove(tp);
            if (index > dragTarget)
            {
                index--;
            }

            InsertTabPage(index, tp);
            SelectedIndex = dragTarget = index;
            Visible = v;
            if (f)
            {
                _ = Focus();
            }

            Changed?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (TabContextMenu != null && e.Button == MouseButtons.Right && dragTarget != -1)
            {
                TabContextMenu.Show(this, new Point(e.X, e.Y));
            }
            dragTarget = -1;
            noMove = true;
        }
    }
}
