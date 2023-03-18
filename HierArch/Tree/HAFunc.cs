// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Girl.Windows.Forms;

namespace Girl.HierArch
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class HAFunc : HATree
	{
		private ContextMenu contextMenu1;
		private MenuItem mnuType;
		public CodeEditor SourceTextBox;
		public HAClassNode OwnerClass;
		public HAFuncNode Body;
		public HAFuncNode TargetNode;
		private Font textFont;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAFunc()
		{
			this.dataFormat = "HierArch Function Data";
			this.SourceTextBox = null;
			this.OwnerClass = null;
			this.TargetNode = null;
			this.AllowDrop = true;
			this.ContextMenu = this.contextMenu1 = new ContextMenu();
			this.HideSelection = false;
			this.LabelEdit = true;
			this.ImageList = this.imageList1;
			this.textFont = new Font("ＭＳ ゴシック", 9);
//			this.mnuAccess.Text = "関数(&U)";
//			this.mnuFolderGray.Text = "仮想フォルダ(&V)";
			this.contextMenu1.MenuItems.AddRange(
				new MenuItem[] {
					mnuType = new MenuItem("種類変更(&T)", new MenuItem[] { this.mnuText, this.mnuFolder, this.mnuAccess, this.mnuEtc }),
					new MenuItem("-"), this.mnuChild, this.mnuAppend, this.mnuInsert,
					new MenuItem("-"), this.mnuDelete, this.mnuRename
				});
		}

		public override void OnChanged(object sender, EventArgs e)
		{
			if (this.IgnoreChanged) return;
			base.OnChanged(sender, e);
			if (this.TargetNode != null) this.TargetNode.LastModified = DateTime.Now;
		}

		public override void OnRefreshNode(object sender, EventArgs e)
		{
			base.OnRefreshNode(sender, e);
			if (sender != this.TargetNode) return;
			this.SetView();
		}

		protected override void MenuNodeChild_Click(object sender, EventArgs e)
		{
			HATreeNode p =(HATreeNode) this.SelectedNode;
			HATreeNode n = this.NewNode;
			if (p != null)
			{
				p.Nodes.Add(n);
				p.SetIcon();
			}
			else
			{
				this.Nodes.Add(n);
			}
			n.EnsureVisible();
			this.SelectedNode = n;
			n.BeginEdit();
			this.OnChanged(this, EventArgs.Empty);
		}

		protected override void StartDrag()
		{
			this.Focus();
			this.StoreData();
			base.StartDrag();
		}

		protected override void SetState()
		{
			if (this.Nodes.Count < 1 && this.TargetNode != null)
			{
				this.TargetNode = null;
				this.SetView();
			}
			HAFuncNode n = this.SelectedNode as HAFuncNode;
			mnuType.Enabled = mnuAppend.Enabled = mnuInsert.Enabled = mnuDelete.Enabled = mnuRename.Enabled =(n != null && n.AllowDrag);
		}

		protected override HATreeNode NewNode
		{
			get
			{
				return new HAFuncNode("新しい関数");
			}
		}

		public void StoreData()
		{
			if (this.TargetNode == null) return;
			this.StoreState();
			if (this.SourceTextBox != null)
			{
				this.TargetNode.Source = this.SourceTextBox.Code;
				this.TargetNode.SourceSelectionStart = this.SourceTextBox.SelectionStart;
				this.TargetNode.SourceSelectionLength = this.SourceTextBox.SelectionLength;
			}
		}

		public void SetView()
		{
			bool flag = this.IgnoreChanged;
			this.IgnoreChanged = true;
			if (this.TargetNode != null)
			{
				if (this.SourceTextBox != null)
				{
					this.SourceTextBox.Enabled = true;
					this.SourceTextBox.Clear();
					this.SourceTextBox.Code = this.TargetNode.Source;
					this.SourceTextBox.SelectionStart = this.TargetNode.SourceSelectionStart;
					this.SourceTextBox.SelectionLength = this.TargetNode.SourceSelectionLength;
				}
			}
			else
			{
				if (this.SourceTextBox != null)
				{
					this.SourceTextBox.Enabled = false;
					this.SourceTextBox.Clear();
					this.SourceTextBox.SelectionStart = 0;
					this.SourceTextBox.SelectionLength = 0;
				}
			}
			this.IgnoreChanged = flag;
		}

		public void SetView(HAClassNode cls)
		{
			bool flag = this.IgnoreChanged;
			this.IgnoreChanged = true;
			this.SelectedNode = null;
			this.TargetNode = null;
			this.SetView();
			this.Nodes.Clear();
			if (cls != null)
			{
				this.Enabled = true;
				this.BackColor = System.Drawing.SystemColors.Window;
				this.Body = cls.Body.Clone() as HAFuncNode;
				if (this.OwnerClass.IsObject)
				{
					this.BeginUpdate();
					this.Nodes.Add(this.Body);
					this.ApplyState();
					this.EndUpdate();
				}
				else if (this.Body.Nodes.Count > 0)
				{
					this.BeginUpdate();
					foreach (TreeNode n in this.Body.Nodes)
					{
						this.Nodes.Add(n.Clone() as HAFuncNode);
					}
					this.Body.Nodes.Clear();
					this.ApplyState();
					this.EndUpdate();
				}
				if (this.SelectedNode == null && this.Nodes.Count > 0)
				{
					this.SelectedNode = this.Nodes[0];
				}
				if (this.SelectedNode != null)
				{
					this.SelectedNode.EnsureVisible();
					this.TargetNode = this.SelectedNode as HAFuncNode;
					this.SetView();
				}
			}
			else
			{
				this.Enabled = false;
				this.BackColor = System.Drawing.SystemColors.ControlLight;
			}
			this.SetState();
			this.IgnoreChanged = flag;
		}

		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			base.OnAfterSelect(e);
			if (this.IgnoreChanged) return;
			this.StoreData();
			if (this.TargetNode == e.Node) return;
			this.TargetNode =(HAFuncNode) e.Node;
			this.IgnoreChanged = true;
			this.SetView();
			this.IgnoreChanged = false;
		}

		#region XML

		public override void FromXml(XmlTextReader xr, TreeNodeCollection nc, int index)
		{
			DnDTreeNode dn;

			bool first = true;
			while (xr.Read())
			{
				if (xr.Name == "HAFunc" && xr.NodeType == XmlNodeType.Element)
				{
					dn = new HAFuncNode();
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
