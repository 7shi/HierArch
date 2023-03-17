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
			this.tabContextMenu = null;
			this.dragTarget = -1;
			this.noMove = true;
			this.clickPoint = Point.Empty;
		}

		public ContextMenu TabContextMenu
		{
			get
			{
				return this.tabContextMenu;
			}

			set
			{
				this.tabContextMenu = value;
			}
		}

		public TabPage GetTabPage(int x, int y)
		{
			int index = this.GetTabIndex(x, y);
			if (index < 0) return null;
			return this.TabPages[index];
		}

		public int GetTabIndex(int x, int y)
		{
			int len = this.TabCount;
			for (int i = 0; i < len; i++)
			{
				if (this.GetTabRect(i).Contains(x, y))
				{
					return i;
				}
			}
			return -1;
		}

		public void AddTabPage(int prev, TabPage page)
		{
			this.InsertTabPage(prev + 1, page);
		}

		public void InsertTabPage(int next, TabPage page)
		{
			Control[] ctrls_p = new Control[page.Controls.Count];
			string text_p = page.Text;
			page.Controls.CopyTo(ctrls_p, 0);
			page.Controls.Clear();
			this.TabPages.Add(page);
			for (int i = this.TabCount - 1; i >= next; i--)
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
					TabPage tp_s = this.TabPages[i - 1];
					ctrls = new Control[tp_s.Controls.Count];
					tp_s.Controls.CopyTo(ctrls, 0);
					tp_s.Controls.Clear();
					text = tp_s.Text;
				}
				TabPage tp = this.TabPages[i];
				tp.Controls.AddRange(ctrls);
				tp.Text = text;
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			this.dragTarget = this.GetTabIndex(e.X, e.Y);
			this.noMove = true;
			this.clickPoint = new Point(e.X, e.Y);
			if (e.Button != MouseButtons.Right || this.dragTarget == this.SelectedIndex) return;
			this.SelectedTab = this.TabPages[this.dragTarget];
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Button != MouseButtons.Left || this.dragTarget == -1 ||(this.noMove && Math.Abs(e.X - this.clickPoint.X) < 3 && Math.Abs(e.Y - this.clickPoint.Y) < 3)) return;
			this.noMove = false;
			int len = this.TabCount;
			Rectangle r_s = this.GetTabRect(0);
			Rectangle r_e = this.GetTabRect(len - 1);
			int index;
			if (e.X < r_s.Left + r_s.Width / 2)
			{
				index = 0;
			}
			else if (e.X > r_e.Right - r_e.Width / 2)
			{
				index = len;
			}
			else
			{
				index = this.GetTabIndex(e.X, r_s.Top);
				if (index < 0 || index == this.dragTarget) return;
				Rectangle r = this.GetTabRect(index);
				if (e.X > r.X + r.Width / 2) index++;
			}
			if (index == this.dragTarget || index - 1 == this.dragTarget) return;
			bool f = this.Focused, v = this.Visible;
			this.Visible = false;
			TabPage tp = this.TabPages[this.dragTarget];
			this.TabPages.Remove(tp);
			if (index > this.dragTarget) index--;
			this.InsertTabPage(index, tp);
			this.SelectedIndex = this.dragTarget = index;
			this.Visible = v;
			if (f) this.Focus();
			if (this.Changed != null) this.Changed(this, EventArgs.Empty);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if (this.TabContextMenu != null && e.Button == MouseButtons.Right && this.dragTarget != -1)
			{
				this.TabContextMenu.Show(this, new Point(e.X, e.Y));
			}
			this.dragTarget = -1;
			this.noMove = true;
		}
	}
}
