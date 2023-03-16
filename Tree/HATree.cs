using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Girl.Windows.Forms;

namespace Girl.HierarchyArchitect
{
	/// <summary>
	/// TVBase の概要の説明です。
	/// </summary>
	public class HATree : DnDTreeView
	{
		public bool IgnoreChange = false;

		public HATree()
		{
			// 
			// TODO: コンストラクタ ロジックをここに追加してください。
			//
		}

		protected override void OnAfterSelect(System.Windows.Forms.TreeViewEventArgs e)
		{
			base.OnAfterSelect(e);
			this.SetState();
		}

		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button != MouseButtons.Right) return;

			TreeNode n = this.GetNodeAt(e.X, e.Y);
			if (n != null) 
			{
				this.SelectedNode = n;
				this.SetState();
			}
		}

		protected virtual void SetState()
		{
		}

		public void StoreExpandedState()
		{
			foreach (TreeNode n in this.Nodes)
			{
				if (n is HATreeNode) ((HATreeNode)n).StoreExpandedState();
			}
		}

		public void ApplyExpandedState()
		{
			foreach (TreeNode n in this.Nodes)
			{
				if (n is HATreeNode) ((HATreeNode)n).ApplyExpandedState();
			}
		}
	}

	/// <summary>
	/// HAFuncNode の概要の説明です。
	/// </summary>
	public class HATreeNode : DnDTreeNode
	{
		public bool m_IsExpanded = false;
		public bool m_IsSelected = false;

		public HATreeNode()
		{
		}

		public HATreeNode(string text) : base(text)
		{
		}

		public override object Clone()
		{
			HATreeNode ret = (HATreeNode)base.Clone();
			ret.m_IsExpanded = this.m_IsExpanded;
			ret.m_IsSelected = this.m_IsSelected;
			return ret;
		}

		public void StoreExpandedState()
		{
			this.m_IsExpanded = this.IsExpanded;
			this.m_IsSelected = this.IsSelected;

			foreach (TreeNode n in this.Nodes)
			{
				if (n is HATreeNode) ((HATreeNode)n).StoreExpandedState();
			}
		}

		public void ApplyExpandedState()
		{
			if (this.m_IsExpanded) this.Expand();
			if (this.m_IsSelected) this.TreeView.SelectedNode = this;

			foreach (TreeNode n in this.Nodes)
			{
				if (n is HATreeNode) ((HATreeNode)n).ApplyExpandedState();
			}
		}
	}
}
