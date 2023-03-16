using System;
using System.Collections;

namespace Girl.Coding
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class ASTClass : ASTObject
	{
		public string Access;

		/// <summary>
		/// 抽象構文木でのクラスを表します。
		/// </summary>
		public ASTClass()
		{
		}

		public void ParseDeclaration(ParserBase parser)
		{
			this.DeclarationPos = parser.Pos;
			string text;
			
			while (parser.Read())
			{
				text = parser.Text;
				if (this.Name == "") this.Name = text;
				if (text == "{")
				{
					this.Access = parser.Access;
					this.Parse(parser);
					break;
				}
			}
		}

		public void Parse(ParserBase parser)
		{
			int level = 0;
			bool ignore = false;
			string text = parser.Text, preText;
			int pos = parser.Pos, prePos;
			bool first = true;
			Stack region = new Stack();
			
			while (parser.Read())
			{
				if (first)
				{
					this.Pos = parser.Pos;
					first = false;
				}
				preText = text;
				prePos  = pos;
				text = parser.Text;
				pos  = parser.Pos;
				
				if (text == "{")
				{
					if (level == 0 && !ignore)
					{
						this.ParseProperty(parser, region, preText, prePos);
					}
					else
					{
						level++;
					}
				}
				else if (text == "}")
				{
					level--;
					if (level < 0) break;
				}
				else if (text == "=" || text == "delegate")
				{
					ignore = true;
				}
				else if (text == ";")
				{
					ignore = false;
				}
				else if (text == "(" && level == 0 && !ignore)
				{
					ASTMethod am = new ASTMethod();
					am.Name = preText;
					am.DeclarationPos = prePos;
					am.ParseDeclaration(parser);
					this.AddObject(region, am);
					ignore = false;
				}
				else if (text == "class" || text == "struct" || text == "enum")
				{
					ASTClass ac = new ASTClass();
					ac.ParseDeclaration(parser);
					this.objects.Add(ac);
				}
				else if (text.StartsWith("#region"))
				{
					ASTRegion ar = new ASTRegion(text, pos);
					this.AddObject(region, ar);
					region.Push(ar);
				}
				else if (text == "#endregion")
				{
					if (region.Count > 0) region.Pop();
				}
			}
		}

		private void ParseProperty(ParserBase parser, Stack region, string name, int pos)
		{
			string text = parser.Text, preText, access = parser.Access;
			
			while (parser.Read())
			{
				preText = text;
				text = parser.Text;
				
				if (text == "{")
				{
					ASTMethod am = new ASTMethod();
					am.Name = preText + "_" + name;
					am.Access = access;
					am.DeclarationPos = pos;
					am.Parse(parser);
					this.AddObject(region, am);
				}
				else if (text == "}")
				{
					break;
				}
			}
		}

		private void AddObject(Stack region, ASTObject ao)
		{
			if (region.Count < 1)
			{
				this.objects.Add(ao);
			}
			else
			{
				(region.Peek() as ASTObject).Objects.Add(ao);
			}
		}
	}
}
