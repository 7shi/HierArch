// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Girl.Coding;
using Girl.Windows.Forms;

namespace Girl.HierArch
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class HAMemberNode : HATreeNode
	{
		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAMemberNode()
		{
		}

		public HAMemberNode(string text)
		{
			this.Text = text;
		}

		public override string XmlName
		{
			get
			{
				return "HAObject";
			}
		}

		public override HATreeNode NewNode
		{
			get
			{
				return new HAMemberNode();
			}
		}

		#region Generation

		public void Generate(CodeWriter cw)
		{
			HAType t = this.Type;
			if (t == HAType.Comment)
			{
				return;
			}
			else if (this.IsObject)
			{
				cw.WriteCode(t.ToString().ToLower() + " " + cw.ReplaceKeywords(new ObjectParser(this.Text).ObjectDeclaration) + ";");
			}
			foreach (TreeNode n in this.Nodes)
			{
				(n as HAMemberNode).Generate(cw);
			}
		}

		#endregion
	}
}
