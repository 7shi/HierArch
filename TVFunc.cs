using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Girl.Windows.Forms;

namespace Girl.HierarchyArchitect
{
	/// <summary>
	/// CustomControl1 の概要の説明です。
	/// </summary>
	public class TVFunc : DnDTreeView
	{
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem cm1Child;
		private System.Windows.Forms.MenuItem cm1Append;
		private System.Windows.Forms.MenuItem cm1Insert;
		private System.Windows.Forms.MenuItem cm1Separator1;
		private System.Windows.Forms.MenuItem cm1Delete;

		private void InitializeComponent()
		{
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.cm1Child = new System.Windows.Forms.MenuItem();
			this.cm1Append = new System.Windows.Forms.MenuItem();
			this.cm1Insert = new System.Windows.Forms.MenuItem();
			this.cm1Separator1 = new System.Windows.Forms.MenuItem();
			this.cm1Delete = new System.Windows.Forms.MenuItem();
			// 
			// cm1Child
			// 
			this.cm1Child.Text = "下に追加";
			this.cm1Child.Click += new System.EventHandler(this.cmChild_Click);
			// 
			// cm1Append
			// 
			this.cm1Append.Text = "後に追加";
			this.cm1Append.Click += new System.EventHandler(this.cmAppend_Click);
			// 
			// cm1Insert
			// 
			this.cm1Insert.Text = "前に追加";
			this.cm1Insert.Click += new System.EventHandler(this.cmInsert_Click);
			// 
			// cm1Separator1
			// 
			this.cm1Separator1.Text = "-";
			// 
			// cm1Delete
			// 
			this.cm1Delete.Text = "削除";
			this.cm1Delete.Click += new System.EventHandler(this.cmDelete_Click);
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
			// TVFunc
			// 
			this.ContextMenu = this.contextMenu1;
			this.ItemHeight = 14;

		}
	
		public TVFunc()
		{
			InitializeComponent();
		}

		public void SetButtons()
		{
			cm1Delete.Enabled = (this.SelectedNode != null);
		}

		private TreeNode MakeNewNode()
		{
			TreeNode ret = new DnDTreeNode("新しい関数");
			return ret;
		}

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
			SetButtons();
		}
	}
}
