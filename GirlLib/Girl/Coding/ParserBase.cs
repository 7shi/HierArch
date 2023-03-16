// このファイルは Coding.hacls から生成されています。

using System;
using System.Drawing;
using System.IO;
using System.Text;
using Girl.Rtf;

namespace Girl.Coding
{
	/// <summary>
	/// プルモデルによる C 型言語向けパーサのベースクラスです。
	/// </summary>
	public class ParserBase
	{
		protected TextReader reader;
		protected StringBuilder source;
		protected int lineNumber;
		protected int pos;
		protected string text;
		protected string spacing;
		protected string[] keyWords;
		protected RtfDocument rtf;
		protected string access;
		public Color Color_Default;
		public Color Color_KeyWord;
		public Color Color_Comment;
		public Color Color_String;
		public Color Color_Directive;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public ParserBase()
		{
			this.reader   = null;
			this.keyWords = null;
			
			this.Color_Default   = Color.Black;
			this.Color_KeyWord   = Color.Blue;
			this.Color_Comment   = Color.DarkGreen;
			this.Color_String    = Color.Magenta;
			this.Color_Directive = Color.DarkMagenta;
		}

		protected virtual void Init()
		{
			this.source     = new StringBuilder();
			this.lineNumber = 1;
			this.text       = "";
			this.spacing    = "";
			this.rtf        = new RtfDocument();
		}

		public virtual bool Read()
		{
			if (this.text.Length < 1) return false;
			
			this.rtf.AppendText(this.spacing);
			this.rtf.AppendText(this.text, this.TextColor);
			return true;
		}

		public void Close()
		{
			if (this.spacing != "")
			{
				this.rtf.AppendText(this.spacing);
				if (this.spacing.EndsWith("\n")) this.rtf.AppendLine();
			}
			
			this.reader = null;
		}

		#region Properties

		public TextReader Reader
		{
			get
			{
				return this.reader;
			}

			set
			{
				this.reader = value;
				this.Init();
			}
		}

		public string Source
		{
			get
			{
				return this.source.ToString();
			}
		}

		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		public int Pos
		{
			get
			{
				return this.pos;
			}
		}

		public string Text
		{
			get
			{
				return this.text;
			}
		}

		public virtual bool IsKeyWord
		{
			get
			{
				if (this.keyWords == null) return false;
				
				foreach (string kw in this.KeyWords)
				{
					if (this.text == kw) return true;
				}
				return false;
			}
		}

		public virtual bool IsComment
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsString
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsDirective
		{
			get
			{
				return false;
			}
		}

		public virtual Color TextColor
		{
			get
			{
				if (this.IsKeyWord)
				{
					return this.Color_KeyWord;
				}
				else if (this.IsComment)
				{
					return this.Color_Comment;
				}
				else if (this.IsString)
				{
					return this.Color_String;
				}
				else if (this.IsDirective)
				{
					return this.Color_Directive;
				}
				return this.Color_Default;
			}
		}

		public string Spacing
		{
			get
			{
				return this.spacing;
			}
		}

		public string[] KeyWords
		{
			get
			{
				return this.keyWords;
			}
		}

		public string Rtf
		{
			get
			{
				return this.rtf.ToRtf();
			}
		}

		public string Access
		{
			get
			{
				return this.access;
			}
		}

		#endregion

		public virtual void Parse()
		{
			while (this.Read());
			this.Close();
		}
	}
}
