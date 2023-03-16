using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Girl.Coding;
using Girl.Rtf;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// ソースコードを編集するための RichTextBox です。
	/// </summary>
	public class CodeEditor : ExRichTextBox
	{
		public ParserBase Parser;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public CodeEditor()
		{
			this.AcceptsTab = true;
			this.DetectUrls = false;
			this.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
			this.WordWrap = false;
			this.Font = new Font("ＭＳ ゴシック", 9);
			
			this.Parser = null;
		}

		public void JumpToError(Point p)
		{
			if (p.X < 0 || p.Y < 0) return;
			
			this.SelectionStart = this.GetLinePosition(p.Y) + p.X;
			this.SelectionLength = 0;
			if (this.Parser != null)
			{
				StringReader sr = new StringReader(this.Lines[p.Y].Substring(p.X));
				this.Parser.Reader = sr;
				if (this.Parser.Read() && this.Parser.Spacing == "")
				{
					this.SelectionLength = this.Parser.Text.Length;
				}
				this.Parser.Close();
				sr.Close();
			}
			this.Focus();
		}

		public string Code
		{
			get
			{
				StringBuilder sb;
				char prev;

				sb = new StringBuilder();
				prev = '\0';
				foreach (char ch in this.Text)
				{
					if (ch == '\r')
					{
						sb.Append("\r\n");
					}
					else if (ch == '\n')
					{
						if (prev != '\r') sb.Append("\r\n");
					}
					else
					{
						sb.Append(ch);
					}
				}
				return sb.ToString();
			}

			set
			{
				if (this.Parser == null || value == null)
				{
					this.Text = value;
					return;
				}
				
				StringReader sr = new StringReader(value);
				this.Parser.Reader = sr;
				this.Parser.Color_Default = this.ForeColor;
				this.Parser.Parse();
				sr.Close();
				
				this.Rtf = this.Parser.Rtf;
			}
		}

		public void Reanalyze()
		{
			int pos = this.SelectionStart;
			int len = this.SelectionLength;
			
			InternalHScrollBar hbar = new InternalHScrollBar(this);
			InternalVScrollBar vbar = new InternalVScrollBar(this);
			int hpos = hbar.Pos;
			int vpos = vbar.Pos;
			
			this.Code = this.Code;
			this.SelectionStart  = pos;
			this.SelectionLength = len;
			hbar.Pos = hpos;
			vbar.Pos = vpos;
		}
	}
}
