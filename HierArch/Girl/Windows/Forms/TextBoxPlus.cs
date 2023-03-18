using System;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// TextBoxBase を操作するクラスです。
	/// </summary>
	public static class TextBoxPlus
	{
		public static int GetEndLineWidth(TextBoxBase textBox)
		{
			return (textBox is TextBox) ? 2:
			1;
		}

		public static int GetLinePosition(TextBoxBase textBox, int line)
		{
			if (line > textBox.Lines.Length) return textBox.TextLength;
			int endLine = TextBoxPlus.GetEndLineWidth(textBox);
			int ret = 0;
			for (int i = 0; i < line; i++)
			{
				ret += textBox.Lines[i].Length + endLine;
			}
			return ret;
		}

		public static int GetLine(TextBoxBase textBox, int pos)
		{
			int endLine = TextBoxPlus.GetEndLineWidth(textBox);
			int ret = 0, lpos = 0, llen = 0;
			foreach (string line in textBox.Lines)
			{
				llen = line.Length;
				if (pos < lpos + llen + endLine)
				{
					break;
				}
				ret++;
				lpos += llen + endLine;
			}
			return ret;
		}

		public static int GetCurrentLine(TextBoxBase textBox)
		{
			return TextBoxPlus.GetLine(textBox, textBox.SelectionStart);
		}

		public static int GetCurrentColumn(TextBoxBase textBox)
		{
			int endLine = TextBoxPlus.GetEndLineWidth(textBox);
			int lpos = 0, llen = 0;
			int pos = textBox.SelectionStart;
			foreach (string line in textBox.Lines)
			{
				llen = line.Length;
				if (pos < lpos + llen + endLine)
				{
					break;
				}
				lpos += llen + endLine;
			}
			return textBox.SelectionStart - lpos;
		}

		public static string GetLineText(TextBoxBase textBox, int line)
		{
			if (line < textBox.Lines.Length) return textBox.Lines[line];
			return "";
		}
	}
}
