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
        private readonly string indentString;
        private readonly int indentSpace = 0;
        public Hashtable menuOptions;

        public CodeEditorManager() : this("\t")
        {
        }

        public CodeEditorManager(int indentSpace) : this(new string(' ', indentSpace))
        {
            this.indentSpace = indentSpace;
        }

        public CodeEditorManager(string indentString)
        {
            this.indentString = indentString;
            menuOptions = new Hashtable();
            handlers = new EventHandler[]
            {
                new EventHandler(menuItem_Click)
            }
            ;
            SetStatus(true);
        }

        public override int MaxActions => Enum.GetNames(typeof(CodeEditorOption)).Length;

        public static string GetIndent(string text)
        {
            int len = text.Length;
            if (len < 1)
            {
                return "";
            }

            int pos;
            char ch = text[0];
            if (ch == '>' || ch == '|')
            {
                for (pos = 1; pos < len; pos++)
                {
                    ch = text[pos];
                    if (" \t>|".IndexOf(ch) < 0)
                    {
                        break;
                    }
                }
            }
            else
            {
                for (pos = 0; pos < len; pos++)
                {
                    ch = text[pos];
                    if (ch != ' ' && ch != '\t')
                    {
                        break;
                    }
                }
            }
            return text.Substring(0, pos);
        }

        public void SetTarget(TextBoxBase textBox)
        {
            textBox.KeyDown += new KeyEventHandler(textBox_KeyDown);
            textBox.KeyPress += new KeyPressEventHandler(textBox_KeyPress);
        }

        #region Command

        public void SetCommand(CodeEditorOption option, object target)
        {
            SetCommand((int)option, target);
        }

        public void SetCommand(CodeEditorOption option, params object[] targets)
        {
            foreach (object obj in targets)
            {
                SetCommand((int)option, obj);
            }
        }

        protected override void SetHandler(int action, object target)
        {
            if (target is MenuItem)
            {
                menuOptions[target] = action;
                (target as MenuItem).Click += handlers[0];
            }
        }

        #endregion

        #region Properties

        public bool SmartEnter
        {
            get => flags[(int)CodeEditorOption.SmartEnter];

            set => SetStatus((int)CodeEditorOption.SmartEnter, value);
        }

        public bool SmartTab
        {
            get => flags[(int)CodeEditorOption.SmartTab];

            set => SetStatus((int)CodeEditorOption.SmartTab, value);
        }

        public bool SmartHome
        {
            get => flags[(int)CodeEditorOption.SmartHome];

            set => SetStatus((int)CodeEditorOption.SmartHome, value);
        }

        public bool SmartParenthesis
        {
            get => flags[(int)CodeEditorOption.SmartParenthesis];

            set => SetStatus((int)CodeEditorOption.SmartParenthesis, value);
        }

        protected override void SetProperty(object target, bool status)
        {
            if (target is MenuItem)
            {
                MenuItem mi = target as MenuItem;
                if (mi.Checked != status)
                {
                    mi.Checked = status;
                }
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
                    InsertText(textBox, indentString);
                    pos++;
                }
                InsertText(textBox, "\r\n" + crind);
            }
            else if (cl < crind.Length)
            {
                pos = textBox.SelectionStart += crind.Length - cl;
                if (crtext.EndsWith("}"))
                {
                    InsertText(textBox, indentString);
                    pos++;
                }
                InsertText(textBox, "\r\n" + crind);
            }
            else if (cl > 0 && crtext.Substring(cl - 1, 1) == "{")
            {
                string nxtext = TextBoxPlus.GetLineText(textBox, ln + 1);
                string nxind = CodeEditorManager.GetIndent(nxtext);
                InsertText(textBox, "\r\n" + crind + indentString);
                bool needsClose = !(nxtext.EndsWith("}") && crind == nxind) && crind.Length >= nxind.Length;
                if (cl < crtext.Length)
                {
                    pos = textBox.SelectionStart;
                    if (crtext.Substring(cl, 1) == "}")
                    {
                        InsertText(textBox, "\r\n" + crind);
                        textBox.SelectionStart++;
                        if (textBox.SelectionStart == textBox.TextLength)
                        {
                            InsertText(textBox, "\r\n");
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
                    if (pos < 0)
                    {
                        pos = textBox.SelectionStart;
                    }

                    InsertText(textBox, "\r\n" + crind + "}");
                    if (textBox.SelectionStart == textBox.TextLength)
                    {
                        InsertText(textBox, "\r\n");
                    }
                }
            }
            else if (crtext.EndsWith(":"))
            {
                InsertText(textBox, "\r\n" + crind + indentString);
            }
            else
            {
                InsertText(textBox, "\r\n" + crind);
            }
            if (pos >= 0)
            {
                textBox.SelectionStart = pos;
            }
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
            char prv1 = pos > 0 ? textBox.Text[pos - 1] :
            '\0';
            char prv2 = pos > 1 ? textBox.Text[pos - 2] :
            '\0';
            char curr = pos < textBox.TextLength ? textBox.Text[pos] :
            '\0';
            switch (ch)
            {
                case '(':
                    InsertText(textBox, ")");
                    textBox.SelectionStart--;
                    break;
                case '[':
                    InsertText(textBox, "]");
                    textBox.SelectionStart--;
                    break;
                case '{':
                    InsertText(textBox, "}");
                    textBox.SelectionStart--;
                    break;
                case '<':
                    InsertText(textBox, ">");
                    textBox.SelectionStart--;
                    break;
                case '*':
                    if (prv1 == '/')
                    {
                        InsertText(textBox, "*/");
                        textBox.SelectionStart -= 2;
                    }
                    break;
                case '"':
                case '\'':
                    if (prv1 != '\\' || (prv1 == '\\' && prv2 == '\\'))
                    {
                        if (ch == curr)
                        {
                            textBox.SelectionStart++;
                            return true;
                        }
                        InsertText(textBox, ch.ToString());
                        textBox.SelectionStart--;
                    }
                    break;
                case ')':
                case ']':
                case '}':
                case '>':
                    if (ch != curr)
                    {
                        break;
                    }

                    textBox.SelectionStart++;
                    return true;
            }
            return false;
        }

        public static void Paste(TextBoxBase textBox, string text)
        {
            if (textBox is TextBox tb)
            {
                tb.Paste(text);
            }
            else
            {
                textBox.SelectedText = text;
            }
        }

        private bool ProcessTab(TextBoxBase textBox, bool shift)
        {
            int len = textBox.SelectionLength;
            if (len == 0 && !shift)
            {
                if (indentString == "\t")
                {
                    return false;
                }

                if (indentSpace == 0)
                {
                    InsertText(textBox, indentString);
                    return true;
                }
                int clm = TextBoxPlus.GetCurrentColumn(textBox);
                InsertText(textBox, new string(' ', indentSpace - (clm % indentSpace)));
                return true;
            }
            int pos = textBox.SelectionStart;
            int sl = TextBoxPlus.GetLineFromCharIndex(textBox, pos);
            int el = TextBoxPlus.GetLineFromCharIndex(textBox, pos + len);
            if (textBox.SelectedText.EndsWith("\n"))
            {
                el--;
            }

            int sp = TextBoxPlus.GetFirstCharIndexFromLine(textBox, sl);
            int ep = TextBoxPlus.GetFirstCharIndexFromLine(textBox, el + 1);
            textBox.SelectionStart = sp;
            textBox.SelectionLength = ep - sp;
            StringReader sr = new StringReader(textBox.SelectedText);
            StringWriter sw = new StringWriter();
            string ind = indentString, line;
            while ((line = sr.ReadLine()) != null)
            {
                if (!shift)
                {
                    sw.WriteLine(ind + line);
                }
                else
                {
                    char ch = (line.Length > 0) ? line[0] :
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
            Paste(textBox, sw.ToString());
            int nlen = textBox.SelectionStart - sp;
            textBox.SelectionStart = sp;
            textBox.SelectionLength = nlen;
            return true;
        }

        private void InsertText(TextBoxBase textBox, string text)
        {
            if (textBox is RichTextBox rtb)
            {
                rtb.SelectionColor = rtb.ForeColor;
                rtb.SelectionFont = rtb.Font;
            }
            Paste(textBox, text);
        }

        #endregion

        #region Event Handler

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is TextBoxBase textBox))
            {
                return;
            }

            if (e.KeyCode == Keys.Enter && SmartEnter)
            {
                if (e.Modifiers == Keys.None)
                {
                    ProcessEnter(textBox);
                }
                else if (e.Modifiers == Keys.Shift)
                {
                    // 次の行の行頭に移動
                    int line = TextBoxPlus.GetCurrentLine(textBox) + 1;
                    if (line < textBox.Lines.Length)
                    {
                        string ind = CodeEditorManager.GetIndent(TextBoxPlus.GetLineText(textBox, line));
                        textBox.SelectionStart = TextBoxPlus.GetFirstCharIndexFromLine(textBox, line) + ind.Length;
                    }
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Home && SmartHome && e.Modifiers == Keys.None)
            {
                if (ProcessHome(textBox))
                {
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Tab && SmartTab)
            {
                if (ProcessTab(textBox, e.Shift))
                {
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Back && SmartTab && textBox.SelectionLength == 0 && indentSpace > 1)
            {
                int line = TextBoxPlus.GetCurrentLine(textBox);
                int clm = TextBoxPlus.GetCurrentColumn(textBox);
                string ind = CodeEditorManager.GetIndent(TextBoxPlus.GetLineText(textBox, line));
                if (clm > 0 && clm <= ind.Length)
                {
                    int w = clm % indentSpace;
                    if (w != 1)
                    {
                        if (w == 0)
                        {
                            w = indentSpace;
                        }

                        string spc = new string(' ', w);
                        if (ind.EndsWith(spc))
                        {
                            textBox.SelectionStart -= w;
                            textBox.SelectionLength = w;
                        }
                    }
                }
            }
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(sender is TextBoxBase textBox))
            {
                return;
            }

            if (e.KeyChar == (char)13 && SmartEnter && textBox is TextBox)
            {
                e.Handled = true;
            }
            else if (e.KeyChar == '\t' && SmartTab && indentString != "\t")
            {
                e.Handled = true;
            }
            else if (SmartParenthesis && ProcessParenthesis(textBox, e.KeyChar))
            {
                e.Handled = true;
            }
            else if (e.KeyChar >= ' ' && textBox is RichTextBox)
            {
                InsertText(textBox, e.KeyChar.ToString());
                e.Handled = true;
            }
        }

        private void menuItem_Click(object sender, EventArgs e)
        {
            MenuItem mi;

            mi = sender as MenuItem;
            if (mi == null || !menuOptions.Contains(mi))
            {
                return;
            }

            SetStatus((int)menuOptions[sender], !mi.Checked);
        }

        #endregion
    }
}
