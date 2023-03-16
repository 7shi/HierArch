using System;

namespace Girl.Coding
{
	/// <summary>
	/// 抽象構文木でのメソッドを表します。
	/// </summary>
	public class ASTMethod : ASTObject
	{
		public string Access;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public ASTMethod()
		{
		}

		public void ParseDeclaration(ParserBase parser)
		{
			this.Access = parser.Access;
			
			while (parser.Read())
			{
				if (parser.Text == "{")
				{
					this.Parse(parser);
					break;
				}
			}
		}

		public void Parse(ParserBase parser)
		{
			int level = 0;
			string text;
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
			}
		}
	}
}
