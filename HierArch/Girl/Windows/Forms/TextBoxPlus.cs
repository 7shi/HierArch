using System;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// TextBoxBase を操作するクラスです。
	/// </summary>
	public static class TextBoxPlus
	{
		/// <summary>
		/// WordWrap に依存しない GetFirstCharIndexFromLine 相当
		/// </summary>
		public static int GetFirstCharIndexFromLine(TextBoxBase textBox, int line)
		{
			int ln = 0;
			var t = textBox.Text;
			for (int i = 0; i < t.Length; i++) {
				char ch = t[i];
				if (ch == '\r')
			    {
					if (i + 1 < t.Length && t[i + 1] == '\n') i++;
					ln++;
					if (line == ln) return i + 1;
			    }
				else if (ch == '\n')
				{
					ln++;
					if (line == ln) return i + 1;
				}
			}
			return t.Length;
		}

		/// <summary>
		/// WordWrap に依存しない GetLineFromCharIndex 相当
		/// </summary>
		public static int GetLineFromCharIndex(TextBoxBase textBox, int index)
		{
			int ln = 0;
			var t = textBox.Text;
			for (int i = 0; i < index; i++) {
				char ch = t[i];
				if (ch == '\r')
			    {
					if (i + 1 < t.Length && t[i + 1] == '\n') i++;
					ln++;
			    }
				else if (ch == '\n')
				{
					ln++;
				}
			}
			return ln;
		}

		/// <summary>
		/// WordWrap に依存しない GetFirstCharIndexOfCurrentLine 相当
		/// </summary>
		public static int GetFirstCharIndexOfCurrentLine(TextBoxBase textBox)
		{
			return GetFirstCharIndexOfLineFromCharIndex(textBox, textBox.SelectionStart);
		}

		public static int GetFirstCharIndexOfLineFromCharIndex(TextBoxBase textBox, int index)
		{
			int ln = 0, fi = 0;
			var t = textBox.Text;
			for (int i = 0; i < index; i++) {
				char ch = t[i];
				if (ch == '\r')
			    {
					if (i + 1 < t.Length && t[i + 1] == '\n') i++;
					ln++;
					fi = i + 1;
			    }
				else if (ch == '\n')
				{
					ln++;
					fi = i + 1;
				}
			}
			return fi;
		}

		public static int GetCurrentLine(TextBoxBase textBox)
		{
			return GetLineFromCharIndex(textBox, textBox.SelectionStart);
		}

		public static int GetCurrentColumn(TextBoxBase textBox)
		{
			return textBox.SelectionStart - GetFirstCharIndexOfCurrentLine(textBox);
		}

		public static string GetLineText(TextBoxBase textBox, int line)
		{
			if (line < textBox.Lines.Length) return textBox.Lines[line];
			return "";
		}
	}
}
