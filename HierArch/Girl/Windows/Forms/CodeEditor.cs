using System;
using System.Drawing;
using System.IO;
using System.Text;
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
			this.AcceptsTab = true;
			this.ScrollBars = ScrollBars.Both;
			this.WordWrap = false;
			this.Font = new Font("ＭＳ ゴシック", 9);
			this.Multiline = true;
			//this.ShowSelectionMargin = true;
		}

		public void JumpToError(Point p)
		{
			if (p.X < 0 || p.Y < 0) return;
			this.SelectionStart = this.GetFirstCharIndexFromLine(p.Y) + p.X;
			this.SelectionLength = 0;
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
				this.Text = value;
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
			this.SelectionStart = pos;
			this.SelectionLength = len;
			hbar.Pos = hpos;
			vbar.Pos = vpos;
		}
	}
}
