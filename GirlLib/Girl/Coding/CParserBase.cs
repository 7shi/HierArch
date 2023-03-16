// このファイルは Coding.hacls から生成されています。

using System;
using System.IO;

namespace Girl.Coding
{
	/// <summary>
	/// プルモデルによる C 型言語向けパーサのベースクラスです。
	/// </summary>
	public class CParserBase : ParserBase
	{
		private enum State
		{
			Normal,
			Comment1,
			Comment2,
			Directive,
			String,
			StringAt,
			Char,
			Number
		}
		protected string separator;
		private string preText;
		private int preChar;
		private int curChar;
		private int nextChar;
		private int curLineNum;
		private int prePos;
		private int curPos;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public CParserBase()
		{
		}

		protected override void Init()
		{
			base.Init();
			
			this.separator = ";,.(){}[]*";
			
			this.preText    = "";
			this.preChar    = -1;
			this.curChar    = -1;
			this.nextChar   = -1;
			this.curLineNum =  1;
			this.prePos     =  0;
			this.curPos     =  0;
			this.pos        =  0;
		}

		public override bool Read()
		{
			int ch;
			State st;

			this.preText = this.text;
			this.text = this.spacing = "";
			ch = -1;
			st = State.Normal;
			
			for (;;)
			{
				if (this.text.Length < 1)
				{
					this.lineNumber = this.curLineNum;
					this.prePos = this.pos;
					this.pos = this.curPos;
				}
				ch = ReadChar();
				if (ch == -1) break;
			
				if (st == State.String || st == State.StringAt
					|| st == State.Char || st == State.Comment1)
				{
				}
				else if (st == State.Comment2 || st == State.Directive)
				{
					if (ch == '\r' || ch == '\n')
					{
						this.nextChar = ch;
						break;
					}
				}
				else if (" \t\r\n".IndexOf((char)ch) >= 0) 
				{
					if (this.text.Length > 0)
					{
						this.nextChar = ch;
						break;
					}
					this.spacing += (char)ch;
					continue;
				}
				else if (ch == '"')
				{
					if (this.text.Length < 1)
					{
						st = State.String;
					}
					else if (this.text == "@")
					{
						st = State.StringAt;
					}
					else if (st != State.String)
					{
						this.nextChar = ch;
						break;
					}
				}
				else if (ch == '\'')
				{
					if (this.text.Length < 1)
					{
						st = State.Char;
					}
					else if (st != State.Char)
					{
						this.nextChar = ch;
						break;
					}
				}
				else if (ch == '.')
				{
					if (this.text.Length > 0 && st != State.Number)
					{
						this.nextChar = ch;
						break;
					}
				}
				else if (this.text == "/" && ch == '*')
				{
				}
				else if (this.text.Length > 0
					&& this.separator.IndexOf((char)ch) >= 0)
				{
					this.nextChar = ch;
					break;
				}
			
				this.text += (char)ch;
				if (this.text.Length == 1) 
				{
					this.lineNumber = this.curLineNum;
					if (this.text == "#") 
					{
						st = State.Directive;
					}
					else if (this.separator.IndexOf(this.text) >= 0)
					{
						this.preChar = ch;
						break;
					}
				}
				else if (this.text == "/*") 
				{
					st = State.Comment1;
				}
				else if (this.text == "//")
				{
					st = State.Comment2;
				}
				else if (this.text == "#") 
				{
					st = State.Directive;
				}
				else if (this.text.Length == 1 && '0' <= ch && ch <= '9')
				{
					st = State.Number;
				}
			
				if (st == State.Comment1)
				{
					if (this.preChar == '*' && ch == '/') break;
				} 
				else if (st == State.String)
				{
					if (this.text.Length > 1 && this.preChar != '\\'
						&& ch == '"')
					{
						break;
					}
				}
				else if (st == State.StringAt)
				{
					if (this.text.Length > 2 && ch == '"') break;
				}
				else if (st == State.Char)
				{
					if (this.text.Length > 1 && this.preChar != '\\'
						&& ch == '\'')
					{
						break;
					}
				}
			}
			
			return base.Read();
		}

		private int ReadChar()
		{
			if (this.nextChar != -1) 
			{
				this.curChar = this.nextChar;
				this.nextChar = -1;
			} 
			else 
			{
				this.preChar = this.curChar;
				this.curChar = this.reader.Read();
				if (this.curChar != -1)
				{
					this.curPos++;
					this.source.Append((char)this.curChar);
				}
				if (this.curChar == '\r'
					|| (this.preChar != '\r' && this.curChar == '\n'))
				{
					this.curLineNum++;
				}
			}
			return this.curChar;
		}

		#region Properties

		public override bool IsComment
		{
			get
			{
				return this.text.StartsWith("/*") || this.text.StartsWith("//");
			}
		}

		public override bool IsString
		{
			get
			{
				return this.text.StartsWith("\"") || this.text.StartsWith("'");
			}
		}

		public override bool IsDirective
		{
			get
			{
				return this.text.StartsWith("#");
			}
		}

		#endregion
	}
}
