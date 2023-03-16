using System;
using System.Text;

namespace Girl.Coding
{
	/// <summary>
	/// 抽象構文木での名前空間を表します。
	/// </summary>
	public class ASTNamespace : ASTObject
	{
		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public ASTNamespace()
		{
		}

		public void ParseDeclaration(ParserBase parser)
		{
			this.DeclarationPos = parser.Pos;
			string text;
			StringBuilder sb = new StringBuilder();
			while (parser.Read())
			{
				text = parser.Text;
				if (text == "{")
				{
					this.Name = sb.ToString();
					this.Parse(parser);
					break;
				}
				else
				{
					sb.Append(text);
				}
			}
		}

		public void Parse(ParserBase parser)
		{
			string text;
			int level = 0;
			bool first = true;
			while (parser.Read())
			{
				if (first)
				{
					this.Pos = parser.Pos;
					first = false;
				}
				text = parser.Text;
				if (text == "{")
				{
					level++;
				}
				else if (text == "}")
				{
					level--;
					if (level < 0) break;
				}
				else if (text == "class")
				{
					ASTClass ac = new ASTClass();
					ac.ParseDeclaration(parser);
					this.objects.Add(ac);
				}
			}
		}
	}
}
