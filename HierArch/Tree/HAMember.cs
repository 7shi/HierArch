using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Girl.Code;
using Girl.Windows.Forms;

namespace Girl.HierArch
{
	/// <summary>
	/// HAMember の概要の説明です。
	/// </summary>
	public class HAMember : HATree
	{
		private System.Windows.Forms.ContextMenu contextMenu1;

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(HAMember));
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			// 
			// HAMember
			// 
			this.AllowDrop = true;
			this.ContextMenu = this.contextMenu1;
			this.HideSelection = false;
			this.LabelEdit = true;
		}
	
		private MenuItem mnuType;

		public HAMember()
		{
			InitializeComponent();

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
					this.mnuDelete
				});

			this.ImageList = this.imageList1;
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
			mnuType  .Enabled = mnuDelete.Enabled = (n != null);
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

	/// <summary>
	/// HAMemberNode の概要の説明です。
	/// </summary>
	public class HAMemberNode : HATreeNode
	{
		public HAMemberNode()
		{
		}

		public HAMemberNode(string text) : base(text)
		{
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
				cw.WriteCode(t.ToString().ToLower() + " "
					+ cw.ReplaceKeywords(new ObjectParser(this.Text).ObjectDeclaration) + ";");
			}

			foreach (TreeNode n in this.Nodes)
			{
				(n as HAMemberNode).Generate(cw);
			}
		}

		#endregion
	}
}
