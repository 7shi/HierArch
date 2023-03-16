// ここにソースコードの注釈を書きます。

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Girl.Windows.Forms.Drawing2D;

namespace Girl.Windows.Forms
{
	public class FloatingToolBar : Control
	{
		private const int minimumSize = 26;
		
		#region Win32
		
		public const int WM_MOUSEACTIVATE = 0x0021;
		public const int MA_NOACTIVATE = 3;
		public const int SW_SHOWNOACTIVATE = 4;
		
		[DllImport("User32.dll")]
		public static extern bool ShowWindow(
			IntPtr hWnd,  // ウィンドウのハンドル
			int nCmdShow  // 表示状態
		);
		
		#endregion
		private Control target;
		private CanvasGrip grip;
		private CanvasRectangle back;
		private CanvasLine[] lines;
		private Canvas border;
		private Canvas canvas;
		private int prevCount;
		private bool prevIsHorizontal;
		private bool isFloating;
		private bool ignoreVisibleChanged;
		private SizeF arrangedSize;
		private bool okDragging;
		private bool isDragging;
		private Point ptDown;
		private Point ptOrig;
		private Form floating;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public FloatingToolBar()
		{
			this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			this.target = null;
			this.prevCount = -1;
			this.prevIsHorizontal = false;
			this.isFloating = false;
			this.ignoreVisibleChanged = false;
			this.arrangedSize = Size.Empty;
			this.okDragging = false;
			this.isDragging = false;
			this.ptDown = Point.Empty;
			this.ptOrig = Point.Empty;
			this.floating = null;
			this.border = new Canvas(this);
			this.grip = new CanvasGrip();
			this.border.Items.Add(this.grip);
			this.back = new CanvasRectangle();
			this.back.Pen = null;
			this.back.Brush = SystemBrushes.ControlLight;
			this.border.Items.Add(this.back);
			this.lines = new CanvasLine[2];
			for (int i = 0; i < 2; i++)
			{
				this.lines[i] = new CanvasLine();
				this.lines[i].Pen = SystemPens.ControlLight;
				this.border.Items.Add(this.lines[i]);
			}
			this.canvas = new Canvas(this);
		}

		public Canvas Canvas
		{
			get
			{
				return this.canvas;
			}
		}

		#region Event Handlers

		protected override void OnParentChanged(EventArgs e)
		{
			if (this.target == null) this.target = this.Parent;
			base.OnParentChanged(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.prevCount != this.canvas.Items.Count)
			{
				this.Arrange();
				this.prevCount = this.canvas.Items.Count;
			}
			base.OnPaint(e);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			bool horz = this.IsHorizontal;
			if (this.ignoreVisibleChanged || this.prevIsHorizontal == horz) return;
			this.Arrange();
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if (this.ignoreVisibleChanged) return;
			if (!this.Visible)
			{
				if (this.Parent == this.floating)
				{
					this.ignoreVisibleChanged = true;
					this.Dock = DockStyle.None;
					this.Parent = this.target;
					this.floating.Visible = false;
					this.isFloating = true;
					this.ignoreVisibleChanged = false;
				}
				else
				{
					this.isFloating = false;
				}
			}
			else
			{
				if (this.isFloating)
				{
					this.ignoreVisibleChanged = true;
					this.Parent = this.floating;
					this.Dock = DockStyle.Fill;
					ShowWindow(this.floating.Handle, SW_SHOWNOACTIVATE);
					this.ignoreVisibleChanged = false;
				}
				this.isFloating = false;
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button != MouseButtons.Left) return;
			if (!(this.canvas.ItemUnderMouse is CanvasButton) &&(e.X < this.arrangedSize.Width && e.Y < this.arrangedSize.Height))
			{
				this.okDragging = true;
				this.ptDown = Cursor.Position;
				this.ptOrig = this.GetFloatingLocation(e.X, e.Y);
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Button == MouseButtons.None)
			{
				Cursor cur = Cursors.Default;
				if ((this.prevIsHorizontal && e.X < 8) ||(!this.prevIsHorizontal && e.Y < 8))
				{
					cur = Cursors.SizeAll;
				}
				if (this.Cursor != cur) this.Cursor = cur;
			}
			if (e.Button != MouseButtons.Left || !this.okDragging) return;
			Point curpos = Cursor.Position;
			int dx = curpos.X - this.ptDown.X, dy = curpos.Y - this.ptDown.Y;
			if (!this.isDragging && Math.Abs(dx) < 3 && Math.Abs(dy) < 3) return;
			Point curpos2 = this.target.PointToClient(curpos);
			Rectangle rect = this.target.ClientRectangle;
			Rectangle rect2 = Rectangle.Inflate(rect, 32, 32);
			if (rect2.Contains(curpos2))
			{
				DockStyle ds;
				if (curpos2.Y < 32)
				{
					ds = DockStyle.Top;
				}
				else if (curpos2.X < 32)
				{
					ds = DockStyle.Left;
				}
				else if (curpos2.X > rect.Width - 32)
				{
					ds = DockStyle.Right;
				}
				else if (curpos2.Y > rect.Height - 32)
				{
					ds = DockStyle.Bottom;
				}
				else
				{
					ds = DockStyle.None;
				}
				if (ds != DockStyle.None)
				{
					if (this.floating != null && this.floating.Visible)
					{
						this.ignoreVisibleChanged = true;
						this.Dock = DockStyle.None;
						this.Parent = this.target;
						this.floating.Visible = false;
						this.ignoreVisibleChanged = false;
					}
					if (this.Dock != ds)
					{
						this.Size = new Size(minimumSize, minimumSize);
						this.Dock = ds;
						this.Invalidate();
					}
					return;
				}
			}
			// フローティング
			Point pt = this.ptOrig;
			pt.Offset(dx, dy);
			if (this.floating == null)
			{
				this.floating = new Form();
				this.floating.Bounds = new Rectangle(pt, Size.Empty);
				this.floating.Owner = this.target.TopLevelControl as Form;
				this.floating.FormBorderStyle = FormBorderStyle.None;
				this.floating.ShowInTaskbar = false;
				this.floating.CreateControl();
			}
			else if (!this.floating.Visible)
			{
				this.ignoreVisibleChanged = true;
				this.Dock = DockStyle.None;
				this.Arrange();
				this.Parent = this.floating;
				this.Dock = DockStyle.Fill;
				ShowWindow(this.floating.Handle, SW_SHOWNOACTIVATE);
				this.floating.ClientSize = Size.Truncate(this.arrangedSize);
				this.ignoreVisibleChanged = false;
				this.SetTopMost();
			}
			else
			{
				this.floating.Location = pt;
				this.isDragging = true;
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			this.okDragging = this.isDragging = false;
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_MOUSEACTIVATE)
			{
				m.Result = new IntPtr(MA_NOACTIVATE);
				return;
			}
			base.WndProc(ref m);
		}

		#endregion

		public void Arrange()
		{
			PointF pt;
			RectangleF r = this.ClientRectangle;
			this.prevIsHorizontal = this.IsHorizontal;
			if (this.prevIsHorizontal)
			{
				this.grip.Bounds = new Rectangle(3, 5, 3, 16);
				pt = new Point(8, 2);
				foreach (object obj in this.canvas.Items)
				{
					CanvasObject dobj = obj as CanvasObject;
					if (dobj is CanvasSeparator) dobj.Size = new Size(6, this.Height - 4);
					dobj.Location = pt;
					pt.X += dobj.Width;
				}
				this.back.Bounds = new RectangleF(2, 1, pt.X - 2, r.Height - 2);
				this.lines[0].Bounds = new RectangleF(1, 2, 0, r.Height - 5);
				this.lines[1].Bounds = new RectangleF(pt.X, 2, 0, r.Height - 5);
				this.arrangedSize = new SizeF(pt.X + 2, r.Height);
			}
			else
			{
				this.grip.Bounds = new Rectangle(5, 3, 16, 3);
				pt = new Point(2, 8);
				foreach (object obj in this.canvas.Items)
				{
					CanvasObject dobj = obj as CanvasObject;
					if (dobj is CanvasSeparator) dobj.Size = new Size(this.Width - 4, 6);
					dobj.Location = pt;
					pt.Y += dobj.Height;
				}
				this.back.Bounds = new RectangleF(1, 2, r.Width - 2, pt.Y - 2);
				this.lines[0].Bounds = new RectangleF(2, 1, r.Width - 5, 0);
				this.lines[1].Bounds = new RectangleF(2, pt.Y, r.Width - 5, 0);
				this.arrangedSize = new SizeF(r.Width, pt.Y + 2);
			}
		}

		public bool IsHorizontal
		{
			get
			{
				return this.Dock != DockStyle.Left && this.Dock != DockStyle.Right;
			}
		}

		public Point GetFloatingLocation(int x, int y)
		{
			Point pt = new Point(x, y), ret = this.PointToScreen(pt);
			switch (this.Dock)
			{
				case DockStyle.Left: case DockStyle.Right: ret.Offset(-y, -x);
				break;
				default: ret.Offset(-x, -y);
				break;
			}
			return ret;
		}

		public void SetTopMost()
		{
			if (this.floating == null) return;
			Form f = this.target.TopLevelControl as Form;
			if (this.floating.TopMost == f.TopMost) return;
			this.floating.TopMost = f.TopMost;
			f.Activate();
		}

		public CanvasButton GetButton(string name)
		{
			foreach (object obj in canvas.Items)
			{
				CanvasButton cb = obj as CanvasButton;
				if (cb != null && cb.Name == name) return cb;
			}
			return null;
		}

		public void SetButtonEnabled()
		{
			foreach (object obj in canvas.Items)
			{
				CanvasButton cb = obj as CanvasButton;
				if (cb != null) this.SetButtonEnabled(cb);
			}
		}

		public void SetButtonEnabled(CanvasButton button)
		{
			MenuItem mni = button.MenuItem;
			if (mni == null) return;
			bool enabled = mni.Enabled;
			if (button.Enabled == enabled) return;
			button.Enabled = enabled;
			this.canvas.Invalidate(button);
		}

		public void SetButtonEnabled(CanvasButton button, bool enabled)
		{
			if (button.Enabled == enabled) return;
			button.Enabled = enabled;
			this.canvas.Invalidate(button);
			MenuItem mni = button.MenuItem;
			if (mni != null) mni.Enabled = enabled;
		}

		public void SetButtonEnabled(string name, bool enabled)
		{
			this.SetButtonEnabled(this.GetButton(name), enabled);
		}
	}
}
