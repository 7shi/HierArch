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
            eventStatus = null;
            mouseButtons = prevButton = MouseButtons.None;
            clickPoint = Point.Empty;
            noMove = false;
            modifierKeys = Keys.None;
            modifierShift = modifierControl = false;
            acceptsArrow = false;
        }

        protected override void Dispose(bool disposing)
        {
            Cancel();
            base.Dispose(disposing);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (eventStatus != null && e.Button == MouseButtons.Right)
            {
                Cancel();
                return;
            }
            if (mouseButtons != MouseButtons.None)
            {
                return;
            }

            clickPoint = new Point(e.X, e.Y);
            mouseButtons = prevButton = e.Button;
            modifierKeys = Control.ModifierKeys;
            modifierShift = (modifierKeys & Keys.Shift) != 0;
            modifierControl = (modifierKeys & Keys.Control) != 0;
            modifierAlt = (modifierKeys & Keys.Alt) != 0;
            noMove = true;
            OnPrepareEvent(e);
        }

        protected virtual void OnPrepareEvent(MouseEventArgs e)
        {
            PrepareEvent?.Invoke(this, e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (eventStatus != null && OnDispatchEvent(e))
            {
                return;
            }

            int dx = e.X - clickPoint.X, dy = e.Y - clickPoint.Y;
            if (noMove && Math.Abs(dx) < 3 && Math.Abs(dy) < 3)
            {
                return;
            }

            if (noMove)
            {
                OnStartEvent(EventArgs.Empty);
            }

            _ = OnDispatchEvent(e);
            noMove = false;
        }

        protected virtual void OnStartEvent(EventArgs e)
        {
            StartEvent?.Invoke(this, e);
        }

        protected virtual bool OnDispatchEvent(MouseEventArgs e)
        {
            DispatchEvent?.Invoke(this, e);
            return false;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            prevButton = e.Button;
            if (eventStatus != null)
            {
                OnEndEvent(EventArgs.Empty);
                return;
            }
            if (e.Button != mouseButtons)
            {
                return;
            }

            SetMouseButtons(e);
        }

        protected virtual void OnEndEvent(EventArgs e)
        {
            if (eventStatus == null)
            {
                return;
            }

            eventStatus = null;
            mouseButtons = MouseButtons.None;
            EndEvent?.Invoke(this, e);
        }

        private void SetMouseButtons(MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    OnLeftClick(e);
                    break;
                case MouseButtons.Right:
                    OnRightClick(e);
                    break;
                case MouseButtons.Middle:
                    OnMiddleClick(e);
                    break;
            }
            mouseButtons = MouseButtons.None;
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
            if (eventStatus == null)
            {
                return;
            }

            OnCancelEvent(EventArgs.Empty);
        }

        protected virtual void OnCancelEvent(EventArgs e)
        {
            if (eventStatus == null)
            {
                return;
            }

            eventStatus = null;
            mouseButtons = MouseButtons.None;
            CancelEvent?.Invoke(this, e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey)
            {
                _ = OnDispatchEvent(DefaultMouseEventArgs);
                return;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                OnCancelEvent(EventArgs.Empty);
                return;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey)
            {
                _ = OnDispatchEvent(DefaultMouseEventArgs);
                return;
            }
        }

        public override bool PreProcessMessage(ref Message msg)
        {
            const int WM_KEYDOWN = 0x0100, WM_KEYUP = 0x0101;
            if (acceptsArrow && (msg.Msg == WM_KEYDOWN || msg.Msg == WM_KEYUP))
            {
                Keys k = (Keys)msg.WParam.ToInt32();
                if (k == Keys.Left || k == Keys.Up || k == Keys.Right || k == Keys.Down)
                {
                    if (msg.Msg == WM_KEYDOWN)
                    {
                        OnKeyDown(new KeyEventArgs(k | Control.ModifierKeys));
                    }
                    else
                    {
                        OnKeyUp(new KeyEventArgs(k | Control.ModifierKeys));
                    }
                    return true;
                }
            }
            return base.PreProcessMessage(ref msg);
        }

        public bool AcceptsArrow
        {
            get => acceptsArrow;

            set => acceptsArrow = value;
        }

        public MouseEventArgs DefaultMouseEventArgs
        {
            get
            {
                Point pt = PointToClient(Cursor.Position);
                return new MouseEventArgs(MouseButtons.None, 0, pt.X, pt.Y, 0);
            }
        }
    }
}
