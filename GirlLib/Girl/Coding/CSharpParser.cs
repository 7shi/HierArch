// このファイルは Coding.hacls から生成されています。

using System;
using System.Collections;

namespace Girl.Coding
{
	/// <summary>
	/// プルモデルによる C# のパーサです。
	/// </summary>
	public class CSharpParser : CParserBase
	{
		protected ArrayList objects;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public CSharpParser()
		{
			this.keyWords = new string[]
				{
					"abstract", "as", "base", "bool",
					"break", "byte", "case", "catch",
					"char", "checked", "class", "const",
					"continue", "decimal", "default", "delegate",
					"do", "double", "else", "enum",
					"event", "explicit", "extern", "false",
					"finally", "fixed", "float", "for",
					"foreach", "goto", "if", "implicit",
					"in", "int", "interface", "internal",
					"is", "lock", "long", "namespace",
					"new", "null", "object", "operator",
					"out", "override", "params", "private",
					"protected", "public", "readonly", "ref",
					"return", "sbyte", "sealed", "short",
					"sizeof", "stackalloc", "static", "string",
					"struct", "switch", "this", "throw",
					"true", "try", "typeof", "uint",
					"ulong", "unchecked", "unsafe", "ushort",
					"using", "virtual", "volatile", "void",
					"while"
				};
		}

		protected override void Init()
		{
			base.Init();
			
			this.objects = new ArrayList();
			this.access  = "private";
		}

		public override bool Read()
		{
			bool ret = base.Read();
			if (this.text == "public"
				|| this.text == "protected"
				|| this.text == "private")
			{
				this.access = this.text;
			}
			else if (this.text == "}")
			{
				this.access = "private";
			}
			return ret;
		}

		public override void Parse()
		{
			string text;
			
			while (this.Read())
			{
				text = this.Text;
				
				if (text == "namespace")
				{
					ASTNamespace an = new ASTNamespace();
					an.ParseDeclaration(this);
					this.objects.Add(an);
				}
				else if (text == "class")
				{
					ASTClass ac = new ASTClass();
					ac.ParseDeclaration(this);
					this.objects.Add(ac);
				}
			}
			
			this.Close();
		}

		public ArrayList Objects
		{
			get
			{
				return this.objects;
			}
		}
	}
}
