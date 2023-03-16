using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Girl.Coding
{
	/// <summary>
	/// ソースコードを出力します。
	/// </summary>
	public class CodeWriter : StreamWriter
	{
		private int m_Indent = 0;
		public string IndentString = "\t";
		private string m_IndentString = "";
		private int m_CurLine = 1;
		private int m_StartLine = 1;
		public string ClassName = "";

		public CodeWriter(Stream stream) : base(stream)
		{
		}

		public CodeWriter(Stream stream, Encoding encoding) : base(stream, encoding)
		{
		}

		public int Indent
		{
			get
			{
				return this.m_Indent;
			}
		}

		public void SetStart()
		{
			this.m_StartLine = this.m_CurLine;
		}

		public int CurLine
		{
			get
			{
				return this.m_CurLine;
			}
		}

		private void MakeIndent()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < this.m_Indent; i++) sb.Append(this.IndentString);
			this.m_IndentString = sb.ToString();
		}

		public void WriteStartBlock(string code)
		{
			this.WriteCode(code);
			this.WriteCode("{");
			this.m_Indent++;
			this.MakeIndent();
			this.SetStart();
		}

		public void WriteEndBlock()
		{
			if (this.m_Indent < 1) return;

			this.m_Indent--;
			this.MakeIndent();
			this.WriteCode("}");
		}

		public void WriteCode(string code)
		{
			this.WriteLine(this.m_IndentString + code);
			this.m_CurLine++;
		}

		public void WriteCodes(string codes)
		{
			this.WriteCodes("", codes);
		}

		public void WriteCodes(string prefix, string codes)
		{
			codes = codes.Replace("\r\n", "\n");
			int ps = 0, pe = codes.Length;
			while (ps < codes.Length && codes[ps] == '\n') ps++;
			while (pe > 0 && codes[pe - 1] == '\n') pe--;
			if (ps == pe) return;

			string[] lines = codes.Substring(ps, pe - ps).Split('\n');
			foreach (string line in lines)
			{
				this.WriteLine(this.m_IndentString + prefix + line);
				this.m_CurLine++;
			}
		}

		public void WriteBlankLine()
		{
			if (this.m_CurLine == this.m_StartLine) return;

			this.WriteLine();
			this.m_CurLine++;
		}

		public string ReplaceKeywords(string str)
		{
			string ret = new Regex(@"\b__CLASS\b").Replace(str, this.ClassName);
			return new Regex(@"\b__DATETIME_NOW\b").Replace(ret, DateTime.Now.ToString());
		}
	}
}
