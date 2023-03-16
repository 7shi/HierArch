// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Girl.Coding;
using Girl.Windows.Forms;

namespace Girl.HierArch
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class HAObjectNode : HATreeNode
	{
		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAObjectNode()
		{
		}

		public HAObjectNode(string text)
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
				return new HAObjectNode();
			}
		}

		public override void SetIcon()
		{
			if (this.IsObject)
			{
				this.SelectedImageIndex =(int) HAType.PointRed;
				this.ImageIndex =(int) HAType.Point;
				return;
			}
			base.SetIcon();
		}

		#region Generation

		public void Generate(CodeWriter cw, StringBuilder sb)
		{
			HAType t = this.Type;
			if (t == HAType.Comment)
			{
				return;
			}
			else if (this.IsObject)
			{
				if (sb.Length > 0) sb.Append(", ");
				sb.Append(cw.ReplaceKeywords(new ObjectParser(this.Text).ObjectDeclaration));
			}
			foreach (TreeNode n in this.Nodes)
			{
				(n as HAObjectNode).Generate(cw, sb);
			}
		}

		public void Generate(CodeWriter cw)
		{
			HAType t = this.Type;
			if (t == HAType.Comment)
			{
				return;
			}
			else if (this.IsObject)
			{
				cw.WriteCode(cw.ReplaceKeywords(new ObjectParser(this.Text).ObjectDeclaration) + ";");
			}
			foreach (TreeNode n in this.Nodes)
			{
				(n as HAObjectNode).Generate(cw);
			}
		}

		#endregion
	}
}
