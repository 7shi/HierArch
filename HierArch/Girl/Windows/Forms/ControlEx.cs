using System;
using System.Drawing;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
    /// <summary>
    /// ここにクラスの説明を書きます。
    /// </summary>
    public class ControlEx : Control
    {
        protected string eventStatus;
        protected MouseButtons mouseButtons;
        protected MouseButtons prevButton;
        protected Keys modifierKeys;
        protected bool modifierShift;
        protected bool modifierControl;
        protected bool modifierAlt;
        protected Point clickPoint;
        protected bool noMove;
        protected bool acceptsArrow;
        public event MouseEventHandler PrepareEvent;
        public event EventHandler StartEvent;
        public event MouseEventHandler DispatchEvent;
        public event EventHandler CancelEvent;
        public event EventHandler EndEvent;

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public ControlEx()
        {
            this.eventStatus = null;
            this.mouseButtons = this.prevButton = MouseButtons.None;
            this.clickPoint = Point.Empty;
            this.noMove = false;
            this.modifierKeys = Keys.None;
            this.modifierShift = this.modifierControl = false;
            this.acceptsArrow = false;
        }

        protected override void Dispose(bool disposing)
        {
            this.Cancel();
            base.Dispose(disposing);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (this.eventStatus != null && e.Button == MouseButtons.Right)
            {
                this.Cancel();
                return;
            }
            if (this.mouseButtons != MouseButtons.None) return;
            this.clickPoint = new Point(e.X, e.Y);
            this.mouseButtons = this.prevButton = e.Button;
            this.modifierKeys = Control.ModifierKeys;
            this.modifierShift = (this.modifierKeys & Keys.Shift) != 0;
            this.modifierControl = (this.modifierKeys & Keys.Control) != 0;
            this.modifierAlt = (this.modifierKeys & Keys.Alt) != 0;
            this.noMove = true;
            this.OnPrepareEvent(e);
        }

        protected virtual void OnPrepareEvent(MouseEventArgs e)
        {
            if (this.PrepareEvent != null) this.PrepareEvent(this, e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.eventStatus != null && this.OnDispatchEvent(e)) return;
            int dx = e.X - this.clickPoint.X, dy = e.Y - this.clickPoint.Y;
            if (this.noMove && Math.Abs(dx) < 3 && Math.Abs(dy) < 3) return;
            if (this.noMove) this.OnStartEvent(EventArgs.Empty);
            this.OnDispatchEvent(e);
            this.noMove = false;
        }

        protected virtual void OnStartEvent(EventArgs e)
        {
            if (this.StartEvent != null) this.StartEvent(this, e);
        }

        protected virtual bool OnDispatchEvent(MouseEventArgs e)
        {
            if (this.DispatchEvent != null) this.DispatchEvent(this, e);
            return false;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this.prevButton = e.Button;
            if (this.eventStatus != null)
            {
                this.OnEndEvent(EventArgs.Empty);
                return;
            }
            if (e.Button != this.mouseButtons) return;
            SetMouseButtons(e);
        }

        protected virtual void OnEndEvent(EventArgs e)
        {
            if (this.eventStatus == null) return;
            this.eventStatus = null;
            this.mouseButtons = MouseButtons.None;
            if (this.EndEvent != null) this.EndEvent(this, e);
        }

        private void SetMouseButtons(MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    this.OnLeftClick(e);
                    break;
                case MouseButtons.Right:
                    this.OnRightClick(e);
                    break;
                case MouseButtons.Middle:
                    this.OnMiddleClick(e);
                    break;
            }
            this.mouseButtons = MouseButtons.None;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            SetMouseButtons(e);
            base.OnMouseClick(e);
        }

        protected virtual void OnLeftClick(MouseEventArgs e)
        {
        }

        protected virtual void OnRightClick(MouseEventArgs e)
        {
        }

        protected virtual void OnMiddleClick(MouseEventArgs e)
        {
        }

        public void Cancel()
        {
            if (this.eventStatus == null) return;
            this.OnCancelEvent(EventArgs.Empty);
        }

        protected virtual void OnCancelEvent(EventArgs e)
        {
            if (this.eventStatus == null) return;
            this.eventStatus = null;
            this.mouseButtons = MouseButtons.None;
            if (this.CancelEvent != null) this.CancelEvent(this, e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey)
            {
                this.OnDispatchEvent(this.DefaultMouseEventArgs);
                return;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.OnCancelEvent(EventArgs.Empty);
                return;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey)
            {
                this.OnDispatchEvent(this.DefaultMouseEventArgs);
                return;
            }
        }

        public override bool PreProcessMessage(ref Message msg)
        {
            const int WM_KEYDOWN = 0x0100, WM_KEYUP = 0x0101;
            if (this.acceptsArrow && (msg.Msg == WM_KEYDOWN || msg.Msg == WM_KEYUP))
            {
                Keys k = (Keys)msg.WParam.ToInt32();
                if (k == Keys.Left || k == Keys.Up || k == Keys.Right || k == Keys.Down)
                {
                    if (msg.Msg == WM_KEYDOWN)
                    {
                        this.OnKeyDown(new KeyEventArgs(k | Control.ModifierKeys));
                    }
                    else
                    {
                        this.OnKeyUp(new KeyEventArgs(k | Control.ModifierKeys));
                    }
                    return true;
                }
            }
            return base.PreProcessMessage(ref msg);
        }

        public bool AcceptsArrow
        {
            get
            {
                return this.acceptsArrow;
            }

            set
            {
                this.acceptsArrow = value;
            }
        }

        public MouseEventArgs DefaultMouseEventArgs
        {
            get
            {
                Point pt = this.PointToClient(Cursor.Position);
                return new MouseEventArgs(MouseButtons.None, 0, pt.X, pt.Y, 0);
            }
        }
    }
}
