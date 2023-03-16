using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Girl.Windows.Forms;

namespace Girl.HierarchyArchitect
{
	/// <summary>
	/// HAFunc の概要の説明です。
	/// </summary>
	public class HAFunc : HATree
	{
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem cm1Child;
		private System.Windows.Forms.MenuItem cm1Append;
		private System.Windows.Forms.MenuItem cm1Insert;
		private System.Windows.Forms.MenuItem cm1Separator1;
		private System.Windows.Forms.MenuItem cm1Delete;
		public HAObject ObjectTreeView = null;
		public System.Windows.Forms.TextBox CommentTextBox = null;
		public System.Windows.Forms.TextBox SourceTextBox  = null;

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(HAFunc));
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.cm1Child = new System.Windows.Forms.MenuItem();
			this.cm1Append = new System.Windows.Forms.MenuItem();
			this.cm1Insert = new System.Windows.Forms.MenuItem();
			this.cm1Separator1 = new System.Windows.Forms.MenuItem();
			this.cm1Delete = new System.Windows.Forms.MenuItem();
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			// 
			// contextMenu1
			// 
			this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.cm1Child,
																						 this.cm1Append,
																						 this.cm1Insert,
																						 this.cm1Separator1,
																						 this.cm1Delete});
			// 
			// cm1Child
			// 
			this.cm1Child.Index = 0;
			this.cm1Child.Text = "下に追加";
			this.cm1Child.Click += new System.EventHandler(this.cmChild_Click);
			// 
			// cm1Append
			// 
			this.cm1Append.Index = 1;
			this.cm1Append.Text = "後に追加";
			this.cm1Append.Click += new System.EventHandler(this.cmAppend_Click);
			// 
			// cm1Insert
			// 
			this.cm1Insert.Index = 2;
			this.cm1Insert.Text = "前に追加";
			this.cm1Insert.Click += new System.EventHandler(this.cmInsert_Click);
			// 
			// cm1Separator1
			// 
			this.cm1Separator1.Index = 3;
			this.cm1Separator1.Text = "-";
			// 
			// cm1Delete
			// 
			this.cm1Delete.Index = 4;
			this.cm1Delete.Text = "削除";
			this.cm1Delete.Click += new System.EventHandler(this.cmDelete_Click);
			// 
			// HAFunc
			// 
			this.AllowDrop = true;
			this.ContextMenu = this.contextMenu1;
			this.HideSelection = false;
			this.LabelEdit = true;

		}
	
		public HAFunc()
		{
			InitializeComponent();
			this.ImageList = this.imageList1;
		}

		protected override void StartDrag()
		{
			this.Focus();
			this.StoreData();
			base.StartDrag();
		}

		protected override void SetState()
		{
			cm1Delete.Enabled = (this.SelectedNode != null);
		}

		private TreeNode MakeNewNode()
		{
			TreeNode ret = new HAFuncNode("新しい関数");
			return ret;
		}

		public HAFuncNode TargetNode = null;

		public void StoreData()
		{
			if (this.TargetNode == null) return;

			this.StoreState();
			if (this.CommentTextBox != null) this.TargetNode.Comment = this.CommentTextBox.Text;
			if (this.SourceTextBox  != null) this.TargetNode.Source  = this.SourceTextBox .Text;
		}

		public void SetView()
		{
			if (this.TargetNode != null)
			{
				if (this.CommentTextBox != null)
				{
					this.CommentTextBox.Enabled = true;
					this.CommentTextBox.Text = this.TargetNode.Comment;
				}
				if (this.SourceTextBox != null)
				{
					this.SourceTextBox.Enabled = true;
					this.SourceTextBox.Text = this.TargetNode.Source;
				}
			}
			else
			{
				if (this.CommentTextBox != null)
				{
					this.CommentTextBox.Enabled = false;
					this.CommentTextBox.Text = "";
				}
				if (this.SourceTextBox != null)
				{
					this.SourceTextBox.Enabled = false;
					this.SourceTextBox.Text = "";
				}
			}
		}

		protected override void OnAfterSelect(System.Windows.Forms.TreeViewEventArgs e)
		{
			base.OnAfterSelect(e);
			if (this.IgnoreChange) return;

			this.StoreData();
			if (this.TargetNode == e.Node) return;

			this.TargetNode = (HAFuncNode)e.Node;
			this.SetView();
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
					}
				}
			}
		}

		#endregion

		private void cmChild_Click(object sender, System.EventArgs e)
		{
			TreeNode n = MakeNewNode();
			TreeNode p = this.SelectedNode;
			if (p == null)
			{
				this.Nodes.Add(n);
			}
			else
			{
				p.Nodes.Add(n);
			}
			n.EnsureVisible();
			this.SelectedNode = n;
			n.BeginEdit();
		}

		private void cmAppend_Click(object sender, System.EventArgs e)
		{
			TreeNode n = MakeNewNode();
			TreeNode p = this.SelectedNode;
			if (p == null)
			{
				this.Nodes.Add(n);
			}
			else
			{
				TreeNodeCollection ns = (p.Parent != null) ? p.Parent.Nodes : this.Nodes;
				ns.Insert(p.Index + 1, n);
			}
			n.EnsureVisible();
			this.SelectedNode = n;
			n.BeginEdit();
		}

		private void cmInsert_Click(object sender, System.EventArgs e)
		{
			TreeNode n = MakeNewNode();
			TreeNode p = this.SelectedNode;
			if (p == null)
			{
				this.Nodes.Insert(0, n);
			}
			else
			{
				TreeNodeCollection ns = (p.Parent != null) ? p.Parent.Nodes : this.Nodes;
				ns.Insert(p.Index, n);
				p.EnsureVisible();
			}
			n.EnsureVisible();
			this.SelectedNode = n;
			n.BeginEdit();
		}

		private void cmDelete_Click(object sender, System.EventArgs e)
		{
			TreeNode n = this.SelectedNode;
			if (n == null) return;
			this.Nodes.Remove(n);
			SetState();
		}
	}

	/// <summary>
	/// HAFuncNode の概要の説明です。
	/// </summary>
	public class HAFuncNode : HATreeNode
	{
		public string Comment = "";
		public string Source  = "";

		public HAFuncNode()
		{
		}

		public HAFuncNode(string text) : base(text)
		{
		}

		public override string XmlName
		{
			get
			{
				return "HAFunc";
			}
		}

		public override HATreeNode NewNode
		{
			get
			{
				return new HAFuncNode();
			}
		}

		public override object Clone()
		{
			HAFuncNode ret = (HAFuncNode)base.Clone();
			ret.Comment = this.Comment;
			ret.Source  = this.Source;
			return ret;
		}

		#region XML

		public override void WriteXml(XmlTextWriter xw)
		{
			base.WriteXml(xw);

			xw.WriteStartElement("Comment");
			xw.WriteString(this.Comment);
			xw.WriteEndElement();

			xw.WriteStartElement("Source");
			xw.WriteString(this.Source);
			xw.WriteEndElement();
		}

		public override void ReadXmlNode(XmlTextReader xr)
		{
			if (xr.Name == "Comment" && xr.NodeType == XmlNodeType.Element)
			{
				if (!xr.IsEmptyElement && xr.Read()) this.Comment = xr.ReadString();
			}
			else if (xr.Name == "Source" && xr.NodeType == XmlNodeType.Element)
			{
				if (!xr.IsEmptyElement && xr.Read()) this.Source = xr.ReadString();
			}
		} 

		#endregion
	}
}
