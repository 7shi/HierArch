using System.Drawing;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
    /// <summary>
    /// ソースコードを編集するための TextBox です。
    /// </summary>
    public class CodeEditor : TextBox
    {
        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public CodeEditor()
        {
            AcceptsTab = true;
            ScrollBars = ScrollBars.Both;
            WordWrap = false;
            Font = new Font("ＭＳ ゴシック", 9);
            Multiline = true;
            //this.ShowSelectionMargin = true;
        }

        public void JumpToError(Point p)
        {
            if (p.X < 0 || p.Y < 0)
            {
                return;
            }

            SelectionStart = GetFirstCharIndexFromLine(p.Y) + p.X;
            SelectionLength = 0;
            _ = Focus();
        }

        public string Code
        {
            get => Text.Replace("\r\n", "\r").Replace("\n", "").Replace("\r", "\r\n");

            set => Text = value;
        }

        public void Reanalyze()
        {
            int pos = SelectionStart;
            int len = SelectionLength;
            InternalHScrollBar hbar = new InternalHScrollBar(this);
            InternalVScrollBar vbar = new InternalVScrollBar(this);
            int hpos = hbar.Pos;
            int vpos = vbar.Pos;
            Code = Code;
            SelectionStart = pos;
            SelectionLength = len;
            hbar.Pos = hpos;
            vbar.Pos = vpos;
        }
    }
}
