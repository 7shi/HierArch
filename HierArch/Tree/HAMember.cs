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
	public class HAMember : HATree
	{
		private ContextMenu contextMenu1;
		private MenuItem mnuType;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAMember()
		{
			this.AllowDrop = true;
			this.ContextMenu = this.contextMenu1 = new ContextMenu();
			this.HideSelection = false;
			this.LabelEdit = true;
			this.ImageList = this.imageList1;
			
			this.mnuAccess.Text = "変数(&O)";
			
			this.contextMenu1.MenuItems.AddRange(new MenuItem[]
				{
					mnuType = new MenuItem("種類変更(&T)", new MenuItem[]
						{
							this.mnuAccess,
							this.mnuFolder,
							this.mnuEtc
						}),
					new MenuItem("-"),
					this.mnuChild,
					this.mnuAppend,
					this.mnuInsert,
					new MenuItem("-"),
					this.mnuDelete,
					this.mnuRename
				});
		}

		protected override void StartDrag()
		{
			this.Focus();
			this.StoreState();
			base.StartDrag();
		}

		protected override void SetState()
		{
			HAMemberNode n = this.SelectedNode as HAMemberNode;
			mnuType.Enabled = mnuDelete.Enabled = mnuRename.Enabled = (n != null);
		}

		protected override HATreeNode NewNode
		{
			get
			{
				return new HAMemberNode("新しいメンバ");
			}
		}

		public void SetView(ArrayList list)
		{
			this.IgnoreChanged = true;
			this.SelectedNode = null;
			this.Nodes.Clear();
			if (list != null)
			{
				this.Enabled = true;
				this.BackColor = System.Drawing.SystemColors.Window;
				if (list.Count > 0)
				{
					this.BeginUpdate();
					foreach (Object obj in list)
					{
						if (obj is HAMemberNode) Nodes.Add((HAMemberNode)((HAMemberNode)obj).Clone());
					}
					this.ApplyState();
					if (this.SelectedNode != null)
					{
						this.SelectedNode.EnsureVisible();
					}
					this.EndUpdate();
				}
			}
			else
			{
				this.Enabled = false;
				this.BackColor = System.Drawing.SystemColors.ControlLight;
			}
			this.SetState();
			this.IgnoreChanged = false;
		}

		#region XML

		public override void FromXml(XmlTextReader xr, TreeNodeCollection nc, int index)
		{
			DnDTreeNode dn;

			bool first = true;
			while (xr.Read())
			{
				if (xr.Name == "HAObject" && xr.NodeType == XmlNodeType.Element)
				{
					dn = new HAMemberNode();
					nc.Insert(index, dn);
					dn.FromXml(xr);
					index++;
					if (first)
					{
						dn.EnsureVisible();
						SelectedNode = dn;
						first = false;
						this.OnChanged(this, new EventArgs());
					}
				}
			}
		}

		#endregion
	}
}
