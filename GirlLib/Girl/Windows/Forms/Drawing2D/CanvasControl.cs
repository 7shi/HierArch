// ここにソースコードの注釈を書きます。

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Girl.Windows.Forms.Drawing2D
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class CanvasControl : CanvasObject
	{
		public CanvasControl() : base() {}
		public CanvasControl(float x, float y, float width, float height) : base(x, y, width, height) {}
		public CanvasControl(RectangleF rect) : base(rect) {}
		public event EventHandler Click;
		public event EventHandler DoubleClick;
		public event MouseEventHandler MouseDown;
		public event MouseEventHandler MouseUp;
		public event MouseEventHandler MouseMove;
		public event EventHandler MouseEnter;
		public event EventHandler MouseLeave;
		protected bool isUnderMouse;
		protected MouseButtons button;

		protected override void Init()
		{
			base.Init();
			this.isUnderMouse = false;
			this.button = MouseButtons.None;
			this.enabled = true;
		}

		public void SendMessage(Canvas.Messages msg, EventArgs e)
		{
			switch (msg)
			{
				case Canvas.Messages.Click: this.OnClick(e);
				break;
				case Canvas.Messages.DoubleClick: this.OnDoubleClick(e);
				break;
				case Canvas.Messages.MouseDown: this.OnMouseDown(e as MouseEventArgs);
				break;
				case Canvas.Messages.MouseUp: this.OnMouseUp(e as MouseEventArgs);
				break;
				case Canvas.Messages.MouseMove: this.OnMouseMove(e as MouseEventArgs);
				break;
				case Canvas.Messages.MouseEnter: this.OnMouseEnter(e);
				break;
				case Canvas.Messages.MouseLeave: this.OnMouseLeave(e);
				break;
			}
		}

		protected virtual void OnClick(EventArgs e)
		{
			if (this.Click != null) this.Click(this, e);
		}

		protected virtual void OnDoubleClick(EventArgs e)
		{
			if (this.DoubleClick != null) this.DoubleClick(this, e);
		}

		protected virtual void OnMouseDown(MouseEventArgs e)
		{
			this.button |= e.Button;
			if (this.MouseDown != null) this.MouseDown(this, e);
		}

		protected virtual void OnMouseUp(MouseEventArgs e)
		{
			this.button &= ~(e.Button);
			if (this.MouseUp != null) this.MouseUp(this, e);
		}

		protected virtual void OnMouseMove(MouseEventArgs e)
		{
			if (this.MouseMove != null) this.MouseMove(this, e);
		}

		protected virtual void OnMouseEnter(EventArgs e)
		{
			if (this.MouseEnter != null) this.MouseEnter(this, e);
			this.isUnderMouse = true;
		}

		protected virtual void OnMouseLeave(EventArgs e)
		{
			if (this.MouseLeave != null) this.MouseLeave(this, e);
			this.isUnderMouse = false;
		}

		public bool IsUnderMouse
		{
			get
			{
				return this.isUnderMouse;
			}
		}
	}
}
