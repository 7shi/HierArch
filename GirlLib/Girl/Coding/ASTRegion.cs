using System;

namespace Girl.Coding
{
	/// <summary>
	/// 抽象構文木でのリージョンを表します。
	/// </summary>
	public class ASTRegion : ASTObject
	{
		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public ASTRegion(string region, int pos)
		{
			int p = 8, len = region.Length;
			for (;p < len; p++)
			{
				char ch = region[p];
				if (ch != ' ' && ch != '\t') break;
			}
			this.Name = region.Substring(p);
			this.Pos = this.DeclarationPos = pos;
		}
	}
}
