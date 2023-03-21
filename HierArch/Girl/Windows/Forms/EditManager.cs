using System;
using System.Collections;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
    public enum EditAction
    {
        Undo,
        Redo,
        Cut,
        Copy,
        Paste,
        Delete,
        SelectAll
    }

    /// <summary>
    /// テキストボックスの状態を管理します。
    /// このクラスは[STAThread]でのみ正常動作します。
    /// [MTAThread]では使用しないでください。
    /// </summary>
    public class EditManager : ContextManager
    {
        private readonly ArrayList forms;

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public EditManager()
        {
            Controls = new ArrayList();
            Target = null;
            handlers = new EventHandler[]
            {
                new EventHandler(cmd_Undo), new EventHandler(cmd_Redo), new EventHandler(cmd_Cut), new EventHandler(cmd_Copy), new EventHandler(cmd_Paste), new EventHandler(cmd_Delete), new EventHandler(cmd_SelectAll)
            }
            ;
            forms = new ArrayList();
        }

        public override int MaxActions => Enum.GetNames(typeof(EditAction)).Length;

        #region Command

        public void SetCommand(EditAction action, object target)
        {
            SetCommand((int)action, target);
        }

        public void SetCommand(EditAction action, params object[] targets)
        {
            foreach (object obj in targets)
            {
                SetCommand((int)action, obj);
            }
        }

        #endregion

        #region Control Management

        public ArrayList Controls { get; }

        public Control Target { get; private set; }

        public void AddControl(Control control)
        {
            Form f;

            if (control == null || Controls.Contains(control))
            {
                return;
            }

            control.Disposed += new EventHandler(control_Disposed);
            control.Enter += new EventHandler(control_Enter);
            EventHandler eh = new EventHandler(target_Event);
            control.VisibleChanged += eh;
            control.EnabledChanged += eh;
            if (control is TextBox)
            {
                TextBox tb = control as TextBox;
                tb.TextChanged += eh;
                MouseEventHandler meh = new MouseEventHandler(textBox_MouseMove);
                tb.MouseDown += meh;
                tb.MouseUp += meh;
                tb.MouseMove += meh;
                KeyEventHandler keh = new KeyEventHandler(textBox_KeyUpDown);
                tb.KeyDown += keh;
                tb.KeyUp += keh;
            }
            else if (control is RichTextBox)
            {
                RichTextBox rtb = control as RichTextBox;
                rtb.SelectionChanged += eh;
            }
            f = control.TopLevelControl as Form;
            if (f == null || forms.Contains(f))
            {
                return;
            }

            _ = forms.Add(f);
            f.Activated += new EventHandler(form_Activated);
        }

        public void RemoveControl(Control control)
        {
            if (!Controls.Contains(control))
            {
                return;
            }

            Controls.Remove(control);
        }

        #endregion

        #region Edit

        public virtual void Undo()
        {
            if (Target is TextBoxBase)
            {
                (Target as TextBoxBase).Undo();
            }
            CheckStatus();
        }

        public virtual void Redo()
        {
            if (Target is RichTextBox)
            {
                (Target as RichTextBox).Redo();
            }
        }

        public virtual void Cut()
        {
            if (Target is TextBoxBase)
            {
                (Target as TextBoxBase).Cut();
            }
        }

        public virtual void Copy()
        {
            if (Target is RichTextBox)
            {
                RichTextBox rtb = Target as RichTextBox;
                if (rtb.SelectionLength > 0)
                {
                    DataObject obj = new DataObject();
                    obj.SetData(DataFormats.Rtf, rtb.SelectedRtf);
                    obj.SetData(rtb.SelectedText);
                    Clipboard.SetDataObject(obj);
                }
            }
            else if (Target is TextBoxBase)
            {
                (Target as TextBoxBase).Copy();
            }
        }

        public virtual void Paste()
        {
            if (Target is TextBoxBase)
            {
                (Target as TextBoxBase).Paste();
            }
        }

        public virtual void Delete()
        {
            if (Target is TextBoxBase)
            {
                (Target as TextBoxBase).SelectedText = "";
            }
        }

        public virtual void SelectAll()
        {
            if (Target is TextBoxBase)
            {
                (Target as TextBoxBase).SelectAll();
            }
        }

        #region Handlers

        private void cmd_Undo(object sender, EventArgs e)
        {
            Undo();
        }

        private void cmd_Redo(object sender, EventArgs e)
        {
            Redo();
        }

        private void cmd_Cut(object sender, EventArgs e)
        {
            Cut();
        }

        private void cmd_Copy(object sender, EventArgs e)
        {
            Copy();
        }

        private void cmd_Paste(object sender, EventArgs e)
        {
            Paste();
        }

        private void cmd_Delete(object sender, EventArgs e)
        {
            Delete();
        }

        private void cmd_SelectAll(object sender, EventArgs e)
        {
            SelectAll();
        }

        #endregion

        #endregion

        #region Check Status

        public void CheckStatus()
        {
            if (!Target.Visible || !Target.Enabled)
            {
                SetStatus(false);
            }
            else if (Target is TextBoxBase)
            {
                CheckTextBoxBase();
            }
        }

        private void CheckTextBoxBase()
        {
            TextBoxBase tbb;
            bool status;

            tbb = Target as TextBoxBase;
            SetStatus((int)EditAction.Undo, tbb.CanUndo);
            status = tbb.SelectionLength > 0;
            SetStatus((int)EditAction.Cut, status);
            SetStatus((int)EditAction.Copy, status);
            SetStatus((int)EditAction.Delete, status);
            SetStatus((int)EditAction.SelectAll, true);
            if (tbb is TextBox)
            {
                CheckTextBox();
            }
            else if (tbb is RichTextBox)
            {
                CheckRichTextBox();
            }
        }

        private void CheckTextBox()
        {
            bool status;

            _ = Target as TextBox;
            SetStatus((int)EditAction.Redo, false);
            status = false;
            IDataObject data = Clipboard.GetDataObject();
            if (data != null)
            {
                string[] dfs = data.GetFormats();
                foreach (string df in dfs)
                {
                    if (df == "UnicodeText")
                    {
                        status = true;
                        break;
                    }
                }
            }
            SetStatus((int)EditAction.Paste, status);
        }

        private void CheckRichTextBox()
        {
            RichTextBox rtb;
            bool status;

            rtb = Target as RichTextBox;
            SetStatus((int)EditAction.Redo, rtb.CanRedo);
            status = false;
            IDataObject data = Clipboard.GetDataObject();
            if (data != null)
            {
                string[] dfs = data.GetFormats();
                foreach (string df in dfs)
                {
                    if (rtb.CanPaste(DataFormats.GetFormat(df)))
                    {
                        status = true;
                        break;
                    }
                }
            }
            SetStatus((int)EditAction.Paste, status);
        }

        #region Handlers

        private void control_Disposed(object sender, EventArgs e)
        {
            if (Target == sender)
            {
                SetStatus(false);
            }

            RemoveControl(sender as Control);
        }

        private void control_Enter(object sender, EventArgs e)
        {
            Target = sender as Control;
            CheckStatus();
        }

        private void form_Activated(object sender, EventArgs e)
        {
            if (Target == null)
            {
                return;
            }

            CheckStatus();
        }

        private void target_Event(object sender, EventArgs e)
        {
            if (sender != Target)
            {
                return;
            }

            CheckStatus();
        }

        private void textBox_MouseMove(object sender, MouseEventArgs e)
        {
            target_Event(sender, EventArgs.Empty);
        }

        private void textBox_KeyUpDown(object sender, KeyEventArgs e)
        {
            target_Event(sender, EventArgs.Empty);
        }

        #endregion

        #endregion
    }
}
