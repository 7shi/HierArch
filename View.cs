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
	/// Document に対応する View です。
	/// </summary>
	public class View : System.Windows.Forms.UserControl
	{
		public event EventHandler Changed;

		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panel1;
		private Girl.Windows.Forms.DnDTreeView tvNameSpace;
		private TVFunc tvFunc;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter2;
		private Girl.Windows.Forms.DnDTreeView tvClass;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter3;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem cm1Child;
		private System.Windows.Forms.MenuItem cm1Append;
		private System.Windows.Forms.MenuItem cm1Insert;
		private System.Windows.Forms.MenuItem cm1Separator1;
		private System.Windows.Forms.MenuItem cm1Delete;
		private System.Windows.Forms.Panel panel2;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter4;
		private System.Windows.Forms.TextBox txtSource;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.TextBox txtComment;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter6;
		private Girl.Windows.Forms.DnDTreeView trVariable;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter1;

		public View()
		{
			// この呼び出しは、Windows.Forms フォーム デザイナで必要です。
			InitializeComponent();

			// TODO: InitForm を呼び出しの後に初期化処理を追加します。

		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.tvNameSpace = new Girl.Windows.Forms.DnDTreeView();
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.cm1Child = new System.Windows.Forms.MenuItem();
			this.cm1Append = new System.Windows.Forms.MenuItem();
			this.cm1Insert = new System.Windows.Forms.MenuItem();
			this.cm1Separator1 = new System.Windows.Forms.MenuItem();
			this.cm1Delete = new System.Windows.Forms.MenuItem();
			this.opaqueSplitter3 = new Girl.Windows.Forms.OpaqueSplitter();
			this.txtSource = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tvFunc = new Girl.HierarchyArchitect.TVFunc();
			this.opaqueSplitter2 = new Girl.Windows.Forms.OpaqueSplitter();
			this.tvClass = new Girl.Windows.Forms.DnDTreeView();
			this.opaqueSplitter1 = new Girl.Windows.Forms.OpaqueSplitter();
			this.panel2 = new System.Windows.Forms.Panel();
			this.trVariable = new Girl.Windows.Forms.DnDTreeView();
			this.opaqueSplitter4 = new Girl.Windows.Forms.OpaqueSplitter();
			this.panel3 = new System.Windows.Forms.Panel();
			this.opaqueSplitter6 = new Girl.Windows.Forms.OpaqueSplitter();
			this.txtComment = new System.Windows.Forms.TextBox();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// tvNameSpace
			// 
			this.tvNameSpace.AllowDrop = true;
			this.tvNameSpace.ContextMenu = this.contextMenu1;
			this.tvNameSpace.Dock = System.Windows.Forms.DockStyle.Top;
			this.tvNameSpace.HideSelection = false;
			this.tvNameSpace.ImageIndex = -1;
			this.tvNameSpace.LabelEdit = true;
			this.tvNameSpace.Name = "tvNameSpace";
			this.tvNameSpace.SelectedImageIndex = -1;
			this.tvNameSpace.Size = new System.Drawing.Size(152, 72);
			this.tvNameSpace.TabIndex = 2;
			this.tvNameSpace.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeView_MouseDown);
			this.tvNameSpace.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_AfterSelect);
			this.tvNameSpace.Enter += new System.EventHandler(this.TreeView_Enter);
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
			this.cm1Child.Click += new System.EventHandler(this.btnChild_Click);
			// 
			// cm1Append
			// 
			this.cm1Append.Index = 1;
			this.cm1Append.Text = "後に追加";
			this.cm1Append.Click += new System.EventHandler(this.btnAppend_Click);
			// 
			// cm1Insert
			// 
			this.cm1Insert.Index = 2;
			this.cm1Insert.Text = "前に追加";
			this.cm1Insert.Click += new System.EventHandler(this.btnInsert_Click);
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
			this.cm1Delete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// opaqueSplitter3
			// 
			this.opaqueSplitter3.Location = new System.Drawing.Point(152, 0);
			this.opaqueSplitter3.Name = "opaqueSplitter3";
			this.opaqueSplitter3.Opaque = true;
			this.opaqueSplitter3.Size = new System.Drawing.Size(3, 312);
			this.opaqueSplitter3.TabIndex = 1;
			this.opaqueSplitter3.TabStop = false;
			// 
			// txtSource
			// 
			this.txtSource.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtSource.Location = new System.Drawing.Point(0, 123);
			this.txtSource.Multiline = true;
			this.txtSource.Name = "txtSource";
			this.txtSource.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtSource.Size = new System.Drawing.Size(234, 189);
			this.txtSource.TabIndex = 0;
			this.txtSource.Text = "";
			this.txtSource.WordWrap = false;
			// 
			// panel1
			// 
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.tvFunc,
																				 this.opaqueSplitter2,
																				 this.tvClass,
																				 this.opaqueSplitter1,
																				 this.tvNameSpace});
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(152, 312);
			this.panel1.TabIndex = 3;
			// 
			// tvFunc
			// 
			this.tvFunc.AllowDrop = true;
			this.tvFunc.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvFunc.HideSelection = false;
			this.tvFunc.ImageIndex = -1;
			this.tvFunc.ItemHeight = 14;
			this.tvFunc.LabelEdit = true;
			this.tvFunc.Location = new System.Drawing.Point(0, 179);
			this.tvFunc.Name = "tvFunc";
			this.tvFunc.SelectedImageIndex = -1;
			this.tvFunc.Size = new System.Drawing.Size(152, 133);
			this.tvFunc.TabIndex = 4;
			this.tvFunc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeView_MouseDown);
			this.tvFunc.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_AfterSelect);
			this.tvFunc.Enter += new System.EventHandler(this.TreeView_Enter);
			// 
			// opaqueSplitter2
			// 
			this.opaqueSplitter2.Dock = System.Windows.Forms.DockStyle.Top;
			this.opaqueSplitter2.Location = new System.Drawing.Point(0, 176);
			this.opaqueSplitter2.Name = "opaqueSplitter2";
			this.opaqueSplitter2.Opaque = true;
			this.opaqueSplitter2.Size = new System.Drawing.Size(152, 3);
			this.opaqueSplitter2.TabIndex = 5;
			this.opaqueSplitter2.TabStop = false;
			// 
			// tvClass
			// 
			this.tvClass.AllowDrop = true;
			this.tvClass.ContextMenu = this.contextMenu1;
			this.tvClass.Dock = System.Windows.Forms.DockStyle.Top;
			this.tvClass.HideSelection = false;
			this.tvClass.ImageIndex = -1;
			this.tvClass.LabelEdit = true;
			this.tvClass.Location = new System.Drawing.Point(0, 75);
			this.tvClass.Name = "tvClass";
			this.tvClass.SelectedImageIndex = -1;
			this.tvClass.Size = new System.Drawing.Size(152, 101);
			this.tvClass.TabIndex = 3;
			this.tvClass.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeView_MouseDown);
			this.tvClass.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_AfterSelect);
			this.tvClass.Enter += new System.EventHandler(this.TreeView_Enter);
			// 
			// opaqueSplitter1
			// 
			this.opaqueSplitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.opaqueSplitter1.Location = new System.Drawing.Point(0, 72);
			this.opaqueSplitter1.Name = "opaqueSplitter1";
			this.opaqueSplitter1.Opaque = true;
			this.opaqueSplitter1.Size = new System.Drawing.Size(152, 3);
			this.opaqueSplitter1.TabIndex = 2;
			this.opaqueSplitter1.TabStop = false;
			// 
			// panel2
			// 
			this.panel2.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.trVariable});
			this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel2.Location = new System.Drawing.Point(392, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(152, 312);
			this.panel2.TabIndex = 4;
			// 
			// trVariable
			// 
			this.trVariable.AllowDrop = true;
			this.trVariable.ContextMenu = this.contextMenu1;
			this.trVariable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.trVariable.HideSelection = false;
			this.trVariable.ImageIndex = -1;
			this.trVariable.LabelEdit = true;
			this.trVariable.Name = "trVariable";
			this.trVariable.SelectedImageIndex = -1;
			this.trVariable.Size = new System.Drawing.Size(152, 312);
			this.trVariable.TabIndex = 6;
			this.trVariable.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeView_MouseDown);
			this.trVariable.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_AfterSelect);
			this.trVariable.Enter += new System.EventHandler(this.TreeView_Enter);
			// 
			// opaqueSplitter4
			// 
			this.opaqueSplitter4.Dock = System.Windows.Forms.DockStyle.Right;
			this.opaqueSplitter4.Location = new System.Drawing.Point(389, 0);
			this.opaqueSplitter4.Name = "opaqueSplitter4";
			this.opaqueSplitter4.Opaque = true;
			this.opaqueSplitter4.Size = new System.Drawing.Size(3, 312);
			this.opaqueSplitter4.TabIndex = 5;
			this.opaqueSplitter4.TabStop = false;
			// 
			// panel3
			// 
			this.panel3.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.txtSource,
																				 this.opaqueSplitter6,
																				 this.txtComment});
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(155, 0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(234, 312);
			this.panel3.TabIndex = 6;
			// 
			// opaqueSplitter6
			// 
			this.opaqueSplitter6.Dock = System.Windows.Forms.DockStyle.Top;
			this.opaqueSplitter6.Location = new System.Drawing.Point(0, 120);
			this.opaqueSplitter6.Name = "opaqueSplitter6";
			this.opaqueSplitter6.Opaque = true;
			this.opaqueSplitter6.Size = new System.Drawing.Size(234, 3);
			this.opaqueSplitter6.TabIndex = 2;
			this.opaqueSplitter6.TabStop = false;
			// 
			// txtComment
			// 
			this.txtComment.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtComment.Multiline = true;
			this.txtComment.Name = "txtComment";
			this.txtComment.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtComment.Size = new System.Drawing.Size(234, 120);
			this.txtComment.TabIndex = 1;
			this.txtComment.Text = "";
			this.txtComment.WordWrap = false;
			// 
			// View
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.panel3,
																		  this.opaqueSplitter4,
																		  this.panel2,
																		  this.opaqueSplitter3,
																		  this.panel1});
			this.Name = "View";
			this.Size = new System.Drawing.Size(544, 312);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private bool m_bIgnoreChanged = false;

		protected virtual void OnChanged(object sender, System.EventArgs e)
		{
			if (!m_bIgnoreChanged && Changed != null) Changed(sender, e);
		}

		public void SetDocument(Document doc)
		{
			m_bIgnoreChanged = true;

			// TODO: Document を View に反映するための処理を追加します。

			m_bIgnoreChanged = false;
		}

		private DnDTreeView m_tvTarget = null;

		private void TreeView_Enter(object sender, System.EventArgs e)
		{
			m_tvTarget = (DnDTreeView)sender;
			SetButtons();
		}

		private void TreeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			SetButtons();
		}

		private void TreeView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right) return;

			DnDTreeView tv = (DnDTreeView)sender;
			if (!tv.Focused) tv.Focus();
			if (e.Button != MouseButtons.Right) return;

			TreeNode n = tv.GetNodeAt(e.X, e.Y);
			if (n != null) 
			{
				tv.SelectedNode = n;
				SetButtons();
			}
		}

		private int m_nNodeCount = 0;

		private TreeNode MakeNewNode()
		{
			TreeNode ret = new DnDTreeNode(string.Format("項目{0}", m_nNodeCount));
			m_nNodeCount++;
			return ret;
		}

		#region Buttons

		public void SetButtons()
		{
			TreeNode n = (m_tvTarget != null) ? m_tvTarget.SelectedNode : null;
			cm1Delete.Enabled = (n != null);
		}

		private void btnChild_Click(object sender, System.EventArgs e)
		{
			if (m_tvTarget == null) return;

			TreeNode n = MakeNewNode();
			TreeNode p = m_tvTarget.SelectedNode;
			if (p == null)
			{
				m_tvTarget.Nodes.Add(n);
			}
			else
			{
				p.Nodes.Add(n);
			}
			n.EnsureVisible();
			m_tvTarget.SelectedNode = n;
			n.BeginEdit();
		}

		private void btnAppend_Click(object sender, System.EventArgs e)
		{
			if (m_tvTarget == null) return;

			TreeNode n = MakeNewNode();
			TreeNode p = m_tvTarget.SelectedNode;
			if (p == null)
			{
				m_tvTarget.Nodes.Add(n);
			}
			else
			{
				TreeNodeCollection ns = (p.Parent != null) ? p.Parent.Nodes : m_tvTarget.Nodes;
				ns.Insert(p.Index + 1, n);
			}
			n.EnsureVisible();
			m_tvTarget.SelectedNode = n;
			n.BeginEdit();
		}

		private void btnInsert_Click(object sender, System.EventArgs e)
		{
			if (m_tvTarget == null) return;

			TreeNode n = MakeNewNode();
			TreeNode p = m_tvTarget.SelectedNode;
			if (p == null)
			{
				m_tvTarget.Nodes.Insert(0, n);
			}
			else
			{
				TreeNodeCollection ns = (p.Parent != null) ? p.Parent.Nodes : m_tvTarget.Nodes;
				ns.Insert(p.Index, n);
				p.EnsureVisible();
			}
			n.EnsureVisible();
			m_tvTarget.SelectedNode = n;
			n.BeginEdit();
		}

		private void btnDelete_Click(object sender, System.EventArgs e)
		{
			if (m_tvTarget == null) return;

			TreeNode n = this.m_tvTarget.SelectedNode;
			if (n == null) return;
			this.m_tvTarget.Nodes.Remove(n);
			SetButtons();
		}

		#endregion
	}
}
