using System;
using System.Collections;

namespace Girl.Coding
{
	/// <summary>
	/// 抽象構文木の要素を表します。
	/// </summary>
	public class ASTObject
	{
		public string Name;
		public int Pos;
		public int DeclarationPos;
		protected ArrayList objects;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public ASTObject()
		{
			this.Name = "";
			this.Pos = this.DeclarationPos = 0;
			this.objects = new ArrayList();
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
