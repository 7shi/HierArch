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
		private int indent = 0;
		public string IndentString = "\t";
		private string indentString = "";
		private int curLine = 1;
		private int startLine = 1;
		public string ClassName = "";
		
		public CodeWriter(Stream stream) : base(stream)
		{
		}
		
		public CodeWriter(Stream stream, Encoding encoding) : base(stream, encoding)
		{
		}

		public void SetStart()
		{
			this.startLine = this.curLine;
		}

		public int Indent
		{
			get
			{
				return this.indent;
			}
		}

		public int CurLine
		{
			get
			{
				return this.curLine;
			}
		}

		public string ReplaceKeywords(string str)
		{
			string ret = new Regex(@"\b__CLASS\b").Replace(str, this.ClassName);
			return new Regex(@"\b__DATETIME_NOW\b").Replace(ret, DateTime.Now.ToString());
		}

		#region Write

		public void WriteCode(string code)
		{
			this.WriteLine(this.indentString + code);
			this.curLine++;
		}

		public void WriteCodes(string prefix, string codes)
		{
			codes = codes.Replace("\r\n", "\n");
			int ps = 0, pe = codes.Length;
			while (ps < codes.Length && codes[ps] == '\n') ps++;
			while (pe > 0 && codes[pe - 1] == '\n') pe--;
			if (ps == pe) return;
			string [] lines = codes.Substring(ps, pe - ps).Split('\n');
			foreach (string line in lines)
			{
				this.WriteLine(this.indentString + prefix + line);
				this.curLine++;
			}
		}

		public void WriteCodes(string codes)
		{
			this.WriteCodes("", codes);
		}

		public void WriteBlankLine()
		{
			if (this.curLine == this.startLine) return;
			this.WriteLine();
			this.curLine++;
		}

		public void WriteStartBlock()
		{
			this.WriteCode("{");
			this.indent++;
			this.MakeIndent();
			this.SetStart();
		}

		public void WriteStartBlock(string code)
		{
			this.WriteCode(code);
			this.WriteStartBlock();
		}

		public void WriteEndBlock()
		{
			if (this.indent < 1) return;
			this.indent--;
			this.MakeIndent();
			this.WriteCode("}");
		}

		private void MakeIndent()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < this.indent; i++) sb.Append(this.IndentString);
			this.indentString = sb.ToString();
		}

		#endregion
	}
}
