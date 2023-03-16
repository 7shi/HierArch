using System;
using System.Drawing;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// スタイル付き文字列の追加が簡単な RichTextBox です。
	/// </summary>
	public class ExRichTextBox : RichTextBox
	{
		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public ExRichTextBox()
		{
		}

		#region Text Manipulation

		public void AppendText(string text, Color color)
		{
			if (text.Length < 1) return;
			Color c = this.SelectionColor;
			this.SelectionColor = color;
			this.AppendText(text);
			this.SelectionColor = c;
		}

		public void AppendText(string text, FontStyle fontStyle)
		{
			if (text.Length < 1) return;
			Font f1 = this.SelectionFont;
			Font f2 = new Font(f1.FontFamily, f1.Size, fontStyle);
			this.SelectionFont = f2;
			this.AppendText(text);
			this.SelectionFont = f1;
			f2.Dispose();
		}

		public void AppendText(string text, Color color, FontStyle fontStyle)
		{
			if (text.Length < 1) return;
			Color c = this.SelectionColor;
			this.SelectionColor = color;
			this.AppendText(text, fontStyle);
			this.SelectionColor = c;
		}

		public void AppendLine()
		{
			this.AppendText("\r\n");
		}

		public void AppendLine(string text)
		{
			this.AppendText(text);
			this.AppendLine();
		}

		public void AppendLine(string text, Color color)
		{
			this.AppendText(text, color);
			this.AppendLine();
		}

		public void AppendLine(string text, FontStyle fontStyle)
		{
			this.AppendText(text, fontStyle);
			this.AppendLine();
		}

		public void AppendLine(string text, Color color, FontStyle fontStyle)
		{
			this.AppendText(text, color, fontStyle);
			this.AppendLine();
		}

		public void InsertText(string text, Color color)
		{
			if (text.Length < 1) return;
			Color c = this.SelectionColor;
			this.SelectionColor = color;
			this.SelectedText = text;
			this.SelectionColor = c;
		}

		public void InsertText(string text, FontStyle fontStyle)
		{
			if (text.Length < 1) return;
			Font f1 = this.SelectionFont;
			Font f2 = new Font(f1.FontFamily, f1.Size, fontStyle);
			this.SelectionFont = f2;
			this.SelectedText = text;
			this.SelectionFont = f1;
			f2.Dispose();
		}

		public void InsertText(string text, Color color, FontStyle fontStyle)
		{
			if (text.Length < 1) return;
			Color c = this.SelectionColor;
			this.SelectionColor = color;
			this.InsertText(text, fontStyle);
			this.SelectionColor = c;
		}

		#endregion

		public void ShowLast()
		{
			this.SelectionStart = this.TextLength;
			this.Focus();
			this.Refresh();
		}

		#region Wrappers

		public int CurrentLine
		{
			get
			{
				return TextBoxPlus.GetCurrentLine(this);
			}
		}

		public int CurrentColumn
		{
			get
			{
				return TextBoxPlus.GetCurrentColumn(this);
			}
		}

		public int GetLinePosition(int line)
		{
			return TextBoxPlus.GetLinePosition(this, line);
		}

		public int GetLine(int pos)
		{
			return TextBoxPlus.GetLine(this, pos);
		}

		public string GetLineText(int line)
		{
			return TextBoxPlus.GetLineText(this, line);
		}

		#endregion
	}
}
