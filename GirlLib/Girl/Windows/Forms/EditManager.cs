// このクラスは[STAThread]でのみ正常動作します。
// [MTAThread]では使用しないでください。

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
}

namespace Girl.Windows.Forms
{
	/// <summary>
	/// テキストボックスの状態を管理します。
	/// </summary>
	public class EditManager : ContextManager
	{
		private ArrayList controls;
		private Control target;
		private ArrayList forms;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public EditManager()
		{
			this.controls = new ArrayList();
			this.target   = null;
			
			this.handlers = new EventHandler[]
				{
					new EventHandler(this.cmd_Undo     ),
					new EventHandler(this.cmd_Redo     ),
					new EventHandler(this.cmd_Cut      ),
					new EventHandler(this.cmd_Copy     ),
					new EventHandler(this.cmd_Paste    ),
					new EventHandler(this.cmd_Delete   ),
					new EventHandler(this.cmd_SelectAll)
				};
			
			this.forms = new ArrayList();
		}

		public override int MaxActions
		{
			get
			{
				return (int)Enum.GetNames(typeof(EditAction)).Length;
			}
		}

		#region Command

		public void SetCommand(EditAction action, object target)
		{
			this.SetCommand((int)action, target);
		}

		public void SetCommand(EditAction action, params object[] targets)
		{
			foreach (object obj in targets)
			{
				this.SetCommand((int)action, obj);
			}
		}

		#endregion

		#region Control Management

		public ArrayList Controls
		{
			get
			{
				return this.controls;
			}
		}

		public Control Target
		{
			get
			{
				return this.target;
			}
		}

		public void AddControl(Control control)
		{
			Form f;

			if (control == null || this.controls.Contains(control)) return;
			
			control.Disposed += new EventHandler(this.control_Disposed);
			control.Enter    += new EventHandler(this.control_Enter);
			
			EventHandler eh = new EventHandler(this.target_Event);
			control.VisibleChanged += eh;
			control.EnabledChanged += eh;
			
			if (control is TextBox)
			{
				TextBox tb = control as TextBox;
				tb.TextChanged += eh;
				MouseEventHandler meh = new MouseEventHandler(this.textBox_MouseMove);
				tb.MouseDown += meh;
				tb.MouseUp   += meh;
				tb.MouseMove += meh;
				KeyEventHandler keh = new KeyEventHandler(this.textBox_KeyUpDown);
				tb.KeyDown += keh;
				tb.KeyUp   += keh;
			}
			else if (control is RichTextBox)
			{
				RichTextBox rtb = control as RichTextBox;
				rtb.SelectionChanged += eh;
			}
			
			f = control.TopLevelControl as Form;
			if (f == null || this.forms.Contains(f)) return;
			
			this.forms.Add(f);
			f.Activated += new EventHandler(this.form_Activated);
		}

		public void RemoveControl(Control control)
		{
			if (!this.controls.Contains(control)) return;
			
			this.controls.Remove(control);
		}

		#endregion

		#region Edit

		public virtual void Undo()
		{
			if (this.target is TextBoxBase)
			{
				(this.target as TextBoxBase).Undo();
			}
			this.CheckStatus();
		}

		public virtual void Redo()
		{
			if (this.target is RichTextBox)
			{
				(this.target as RichTextBox).Redo();
			}
		}

		public virtual void Cut()
		{
			if (this.target is TextBoxBase)
			{
				(this.target as TextBoxBase).Cut();
			}
		}

		public virtual void Copy()
		{
			if (this.target is RichTextBox)
			{
				RichTextBox rtb = this.target as RichTextBox;
				if (rtb.SelectionLength > 0)
				{
					DataObject obj = new DataObject();
					obj.SetData(DataFormats.Rtf, rtb.SelectedRtf);
					obj.SetData(rtb.SelectedText);
					Clipboard.SetDataObject(obj);
				}
			}
			else if (this.target is TextBoxBase)
			{
				(this.target as TextBoxBase).Copy();
			}
		}

		public virtual void Paste()
		{
			if (this.target is TextBoxBase)
			{
				(this.target as TextBoxBase).Paste();
			}
		}

		public virtual void Delete()
		{
			if (this.target is TextBoxBase)
			{
				(this.target as TextBoxBase).SelectedText = "";
			}
		}

		public virtual void SelectAll()
		{
			if (this.target is TextBoxBase)
			{
				(this.target as TextBoxBase).SelectAll();
			}
		}

		#region Handlers

		private void cmd_Undo(object sender, EventArgs e)
		{
			this.Undo();
		}

		private void cmd_Redo(object sender, EventArgs e)
		{
			this.Redo();
		}

		private void cmd_Cut(object sender, EventArgs e)
		{
			this.Cut();
		}

		private void cmd_Copy(object sender, EventArgs e)
		{
			this.Copy();
		}

		private void cmd_Paste(object sender, EventArgs e)
		{
			this.Paste();
		}

		private void cmd_Delete(object sender, EventArgs e)
		{
			this.Delete();
		}

		private void cmd_SelectAll(object sender, EventArgs e)
		{
			this.SelectAll();
		}

		#endregion

		#endregion

		#region Check Status

		public void CheckStatus()
		{
			if (!this.target.Visible || !this.target.Enabled)
			{
				this.SetStatus(false);
			}
			else if (this.target is TextBoxBase)
			{
				this.CheckTextBoxBase();
			}
		}

		private void CheckTextBoxBase()
		{
			TextBoxBase tbb;
			bool status;

			tbb = this.target as TextBoxBase;
			
			this.SetStatus((int)EditAction.Undo, tbb.CanUndo);
			
			status = tbb.SelectionLength > 0;
			this.SetStatus((int)EditAction.Cut   , status);
			this.SetStatus((int)EditAction.Copy  , status);
			this.SetStatus((int)EditAction.Delete, status);
			
			this.SetStatus((int)EditAction.SelectAll, true);
			
			if (tbb is TextBox)
			{
				this.CheckTextBox();
			}
			else if (tbb is RichTextBox)
			{
				this.CheckRichTextBox();
			}
		}

		private void CheckTextBox()
		{
			TextBox tb;
			bool status;

			tb = this.target as TextBox;
			
			this.SetStatus((int)EditAction.Redo, false);
			
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
			this.SetStatus((int)EditAction.Paste, status);
		}

		private void CheckRichTextBox()
		{
			RichTextBox rtb;
			bool status;

			rtb = this.target as RichTextBox;
			
			this.SetStatus((int)EditAction.Redo, rtb.CanRedo);
			
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
			this.SetStatus((int)EditAction.Paste, status);
		}

		#region Handlers

		private void control_Disposed(object sender, EventArgs e)
		{
			if (this.target == sender) this.SetStatus(false);
			this.RemoveControl(sender as Control);
		}

		private void control_Enter(object sender, EventArgs e)
		{
			this.target = sender as Control;
			this.CheckStatus();
		}

		private void form_Activated(object sender, EventArgs e)
		{
			if (this.target == null) return;
			
			this.CheckStatus();
		}

		private void target_Event(object sender, EventArgs e)
		{
			if (sender != target) return;
			
			this.CheckStatus();
		}

		private void textBox_MouseMove(object sender, MouseEventArgs e)
		{
			this.target_Event(sender, EventArgs.Empty);
		}

		private void textBox_KeyUpDown(object sender, KeyEventArgs e)
		{
			this.target_Event(sender, EventArgs.Empty);
		}

		#endregion

		#endregion
	}
}
