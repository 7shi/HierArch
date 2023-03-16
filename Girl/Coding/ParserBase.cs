// このファイルは ..\..\Girl.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.Drawing;
using System.IO;
using System.Text;

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
		protected string text;
		protected string spacing;
		protected string[] keyWords;
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
		}

		public virtual bool Read()
		{
			return false;
		}

		public void Close()
		{
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

		#endregion
	}
}
