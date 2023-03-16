// このファイルは ..\..\..\Girl.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.Drawing;
using System.IO;
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

		public override string Text
		{
			get
			{
				return base.Text;
			}

			set
			{
				if (this.Parser == null || value == null)
				{
					base.Text = value;
					return;
				}
				
				RtfDocument rd = new RtfDocument();
				rd.CurrentFont = this.Font;
				StringReader sr = new StringReader(value);
				this.Parser.Reader = sr;
				this.Parser.Color_Default = this.ForeColor;
				while (this.Parser.Read())
				{
					rd.AppendText(this.Parser.Spacing);
					rd.AppendText(this.Parser.Text, this.Parser.TextColor);
				}
				this.Parser.Close();
				sr.Close();
				if (this.Parser.Spacing != "")
				{
					rd.AppendText(this.Parser.Spacing);
					if (this.Parser.Spacing.EndsWith("\n")) rd.AppendLine();
				}
				this.Rtf = rd.ToRtf();
			}
		}
	}
}
