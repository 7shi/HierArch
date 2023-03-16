using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
	public enum CodeEditorOption
	{
		SmartEnter,
		SmartTab,
		SmartHome,
		SmartParenthesis
	}
}

namespace Girl.Windows.Forms
{
	/// <summary>
	/// TextBox にコード入力支援機能を付加します。
	/// </summary>
	public class CodeEditorManager : ContextManager
	{
		public string IndentString;
		public Hashtable menuOptions;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public CodeEditorManager()
		{
			this.IndentString = "\t";
			this.menuOptions = new Hashtable();
			this.handlers = new EventHandler[]
			{
				new EventHandler(this.menuItem_Click)
			}
			;
			this.SetStatus(true);
		}

		public override int MaxActions
		{
			get
			{
				return (int) Enum.GetNames(typeof (CodeEditorOption)).Length;
			}
		}

		public static string GetIndent(string text)
		{
			int len = text.Length;
			if (len < 1) return "";
			int pos;
			char ch = text[0];
			if (ch == '>' || ch == '|')
			{
				for (pos = 1; pos < len; pos++)
				{
					ch = text[pos];
					if (" \t>|".IndexOf(ch) < 0) break;
				}
			}
			else
			{
				for (pos = 0; pos < len; pos++)
				{
					ch = text[pos];
					if (ch != ' ' && ch != '\t') break;
				}
			}
			return text.Substring(0, pos);
		}

		public void SetTarget(TextBoxBase textBox)
		{
			textBox.KeyDown += new KeyEventHandler(this.textBox_KeyDown);
			textBox.KeyPress += new KeyPressEventHandler(this.textBox_KeyPress);
		}

		#region Command

		public void SetCommand(CodeEditorOption option, object target)
		{
			this.SetCommand((int) option, target);
		}

		public void SetCommand(CodeEditorOption option, params object[] targets)
		{
			foreach (object obj in targets)
			{
				this.SetCommand((int) option, obj);
			}
		}

		protected override void SetHandler(int action, object target)
		{
			if (target is MenuItem)
			{
				this.menuOptions[target] = action;
				(target as MenuItem).Click += this.handlers[0];
			}
		}

		#endregion

		#region Properties

		public bool SmartEnter
		{
			get
			{
				return this.flags[(int) CodeEditorOption.SmartEnter];
			}

			set
			{
				this.SetStatus((int) CodeEditorOption.SmartEnter, value);
			}
		}

		public bool SmartTab
		{
			get
			{
				return this.flags[(int) CodeEditorOption.SmartTab];
			}

			set
			{
				this.SetStatus((int) CodeEditorOption.SmartTab, value);
			}
		}

		public bool SmartHome
		{
			get
			{
				return this.flags[(int) CodeEditorOption.SmartHome];
			}

			set
			{
				this.SetStatus((int) CodeEditorOption.SmartHome, value);
			}
		}

		public bool SmartParenthesis
		{
			get
			{
				return this.flags[(int) CodeEditorOption.SmartParenthesis];
			}

			set
			{
				this.SetStatus((int) CodeEditorOption.SmartParenthesis, value);
			}
		}

		protected override void SetProperty(object target, bool status)
		{
			if (target is MenuItem)
			{
				MenuItem mi = target as MenuItem;
				if (mi.Checked != status) mi.Checked = status;
			}
		}

		#endregion

		#region Process

		private void ProcessEnter(TextBoxBase textBox)
		{
			int ln = TextBoxPlus.GetCurrentLine(textBox);
			int cl = TextBoxPlus.GetCurrentColumn(textBox);
			string crtext = TextBoxPlus.GetLineText(textBox, ln);
			string crind = CodeEditorManager.GetIndent(crtext);
			int pos = -1;
			if (cl == crind.Length && cl < crtext.Length)
			{
				pos = textBox.SelectionStart;
				if (crtext.EndsWith("}"))
				{
					this.InsertText(textBox, this.IndentString);
					pos++;
				}
				this.InsertText(textBox, "\r\n" + crind);
			}
			else if (cl < crind.Length)
			{
				pos =(textBox.SelectionStart += crind.Length - cl);
				if (crtext.EndsWith("}"))
				{
					this.InsertText(textBox, this.IndentString);
					pos++;
				}
				this.InsertText(textBox, "\r\n" + crind);
			}
			else if (cl > 0 && crtext.Substring(cl - 1, 1) == "{")
			{
				string nxtext = TextBoxPlus.GetLineText(textBox, ln + 1);
				string nxind = CodeEditorManager.GetIndent(nxtext);
				this.InsertText(textBox, "\r\n" + crind + this.IndentString);
				bool needsClose = !(nxtext.EndsWith("}") && crind == nxind) && crind.Length >= nxind.Length;
				if (cl < crtext.Length)
				{
					pos = textBox.SelectionStart;
					if (crtext.Substring(cl, 1) == "}")
					{
						this.InsertText(textBox, "\r\n" + crind);
						textBox.SelectionStart++;
						if (textBox.SelectionStart == textBox.TextLength)
						{
							this.InsertText(textBox, "\r\n");
						}
						needsClose = false;
					}
					else if (needsClose)
					{
						textBox.SelectionStart += crtext.Length - cl;
					}
				}
				if (needsClose)
				{
					if (pos < 0) pos = textBox.SelectionStart;
					this.InsertText(textBox, "\r\n" + crind + "}");
					if (textBox.SelectionStart == textBox.TextLength)
					{
						this.InsertText(textBox, "\r\n");
					}
				}
			}
			else if (crtext.EndsWith(":"))
			{
				this.InsertText(textBox, "\r\n" + crind + this.IndentString);
			}
			else
			{
				this.InsertText(textBox, "\r\n" + crind);
			}
			if (pos >= 0) textBox.SelectionStart = pos;
		}

		private bool ProcessHome(TextBoxBase textBox)
		{
			int clm = TextBoxPlus.GetCurrentColumn(textBox);
			int ind = CodeEditorManager.GetIndent(TextBoxPlus.GetLineText(textBox, TextBoxPlus.GetCurrentLine(textBox))).Length;
			if (clm == 0 && ind > 0)
			{
				textBox.SelectionStart += ind;
				return true;
			}
			else if (ind < clm)
			{
				textBox.SelectionStart -= clm - ind;
				return true;
			}
			return false;
		}

		private bool ProcessParenthesis(TextBoxBase textBox, char ch)
		{
			int pos = textBox.SelectionStart;
			char prv1 = pos > 0 ? textBox.Text[pos - 1]:
			'\0';
			char prv2 = pos > 1 ? textBox.Text[pos - 2]:
			'\0';
			char curr = pos < textBox.TextLength ? textBox.Text[pos]:
			'\0';
			switch (ch)
			{
				case '(':
				this.InsertText(textBox, ")");
				textBox.SelectionStart--;
				break;
				case '[':
				this.InsertText(textBox, "]");
				textBox.SelectionStart--;
				break;
				case '{':
				this.InsertText(textBox, "}");
				textBox.SelectionStart--;
				break;
				case '<':
				this.InsertText(textBox, ">");
				textBox.SelectionStart--;
				break;
				case '*':
				if (prv1 == '/')
				{
					this.InsertText(textBox, "*/");
					textBox.SelectionStart -= 2;
				}
				break;
				case '"':
				case '\'':
				if (prv1 != '\\' ||(prv1 == '\\' && prv2 == '\\'))
				{
					if (ch == curr)
					{
						textBox.SelectionStart++;
						return true;
					}
					this.InsertText(textBox, ch.ToString());
					textBox.SelectionStart--;
				}
				break;
				case ')':
				case ']':
				case '}':
				case '>':
				if (ch != curr) break;
				textBox.SelectionStart++;
				return true;
			}
			return false;
		}

		private void ProcessTab(TextBoxBase textBox, bool shift)
		{
			int pos = textBox.SelectionStart;
			int len = textBox.SelectionLength;
			int sl = TextBoxPlus.GetLine(textBox, pos);
			int el = TextBoxPlus.GetLine(textBox, pos + len);
			if (textBox.SelectedText.EndsWith("\n")) el--;
			int sp = TextBoxPlus.GetLinePosition(textBox, sl);
			int ep = TextBoxPlus.GetLinePosition(textBox, el + 1);
			textBox.SelectionStart = sp;
			textBox.SelectionLength = ep - sp;
			StringReader sr = new StringReader(textBox.SelectedText);
			StringWriter sw = new StringWriter();
			string ind = this.IndentString, line;
			while ((line = sr.ReadLine()) != null)
			{
				if (!shift)
				{
					sw.WriteLine(ind + line);
				}
				else
				{
					char ch =(line.Length > 0) ? line[0]:
					'\0';
					if (line.StartsWith(ind))
					{
						sw.WriteLine(line.Substring(ind.Length));
					}
					else if (ch == ' ' || ch == '\t' || ch == '>' || ch == '|')
					{
						sw.WriteLine(line.Substring(1));
					}
					else
					{
						sw.WriteLine(line);
					}
				}
			}
			sw.Close();
			sr.Close();
			textBox.SelectedText = sw.ToString();
			int nlen = textBox.SelectionStart - sp;
			textBox.SelectionStart = sp;
			textBox.SelectionLength = nlen;
		}

		private void InsertText(TextBoxBase textBox, string text)
		{
			RichTextBox rtb = textBox as RichTextBox;
			if (rtb != null)
			{
				rtb.SelectionColor = rtb.ForeColor;
				rtb.SelectionFont = rtb.Font;
			}
			textBox.SelectedText = text;
		}

		#endregion

		#region Event Handler

		private void textBox_KeyDown(object sender, KeyEventArgs e)
		{
			TextBoxBase textBox = sender as TextBoxBase;
			if (textBox == null) return;
			if (e.KeyCode == Keys.Enter && this.SmartEnter)
			{
				if (e.Modifiers == Keys.None)
				{
					this.ProcessEnter(textBox);
				}
				else if (e.Modifiers == Keys.Shift)
				{
					int line = TextBoxPlus.GetCurrentLine(textBox);
					if (line < textBox.Lines.Length)
					{
						int clm = TextBoxPlus.GetCurrentColumn(textBox);
						int ind = CodeEditorManager.GetIndent(TextBoxPlus.GetLineText(textBox, line + 1)).Length;
						textBox.SelectionStart +=(TextBoxPlus.GetLineText(textBox, line).Length - clm) + TextBoxPlus.GetEndLineWidth(textBox) + ind;
					}
				}
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Home && this.SmartHome && e.Modifiers == Keys.None)
			{
				if (this.ProcessHome(textBox)) e.Handled = true;
			}
			else if (e.KeyCode == Keys.Tab && this.SmartTab &&(e.Shift || textBox.SelectionLength > 0))
			{
				this.ProcessTab(textBox, e.Shift);
				e.Handled = true;
			}
		}

		private void textBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			TextBoxBase textBox = sender as TextBoxBase;
			if (textBox == null) return;
			if (e.KeyChar ==(char) 13 && this.SmartEnter && textBox is TextBox)
			{
				e.Handled = true;
			}
			else if (e.KeyChar == '\t' && textBox.SelectionLength > 0 && this.SmartTab)
			{
				e.Handled = true;
			}
			else if (this.SmartParenthesis && this.ProcessParenthesis(textBox, e.KeyChar))
			{
				e.Handled = true;
			}
			else if (e.KeyChar >= ' ' && textBox is RichTextBox)
			{
				this.InsertText(textBox, e.KeyChar.ToString());
				e.Handled = true;
			}
		}

		private void menuItem_Click(object sender, EventArgs e)
		{
			MenuItem mi;

			mi = sender as MenuItem;
			if (mi == null || !this.menuOptions.Contains(mi)) return;
			this.SetStatus((int) this.menuOptions[sender], !mi.Checked);
		}

		#endregion
	}
}
