using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Girl.Windows.Forms;

namespace Girl.HierarchyArchitect
{
	/// <summary>
	/// HAClass の概要の説明です。
	/// </summary>
	public class HAClass : HATree
	{
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem cm1Child;
		private System.Windows.Forms.MenuItem cm1Append;
		private System.Windows.Forms.MenuItem cm1Insert;
		private System.Windows.Forms.MenuItem cm1Separator1;
		private System.Windows.Forms.MenuItem cm1Delete;
		public HAMember MemberTreeView = null;
		public HAFunc FuncTreeView = null;

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(HAClass));
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
			// HAClass
			// 
			this.AllowDrop = true;
			this.ContextMenu = this.contextMenu1;
			this.HideSelection = false;
			this.LabelEdit = true;

		}
	
		public HAClass()
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
			HAClassNode ret = new HAClassNode("新しいクラス");
			ret.Type = HAType.Class;

			HAFuncNode hdr = new HAFuncNode("ヘッダ");
			hdr.Type = HAType.Text;
			hdr.m_IsSelected = true;
			ret.Functions.Add(hdr);

			HAFuncNode cls = new HAFuncNode("クラス");
			cls.Type = HAType.Class;
			cls.m_IsExpanded = true;

			HAFuncNode cst = new HAFuncNode("__CLASS");
			cst.Comment = "コンストラクタ\r\n※ __CLASS はクラス名に置換されます。\r\n";
			cls.Nodes.Add(cst);

			HAFuncNode dst = new HAFuncNode("~__CLASS");
			dst.Comment = "デストラクタ\r\n※ __CLASS はクラス名に置換されます。\r\n";
			cls.Nodes.Add(dst);

			ret.Functions.Add(cls);

			HAFuncNode ftr = new HAFuncNode("フッタ");
			ftr.Type = HAType.Text;
			ret.Functions.Add(ftr);
			return ret;
		}

		private HAClassNode TargetNode = null;

		public void StoreData()
		{
			if (this.TargetNode == null || this.FuncTreeView == null) return;

			this.StoreState();
			this.FuncTreeView.StoreData();
			this.TargetNode.Functions.Clear();
			foreach (TreeNode n in this.FuncTreeView.Nodes)
			{
				if (n is HAFuncNode) this.TargetNode.Functions.Add(n.Clone());
			}
		}

		public void SetView()
		{
			if (this.FuncTreeView == null) return;

			Cursor.Current = Cursors.WaitCursor;
			this.FuncTreeView.IgnoreChange = true;
			this.FuncTreeView.SelectedNode = null;
			this.FuncTreeView.TargetNode = null;
			this.FuncTreeView.SetView();
			this.FuncTreeView.Nodes.Clear();
			if (this.TargetNode != null)
			{
				this.FuncTreeView.Enabled = true;
				this.FuncTreeView.BackColor = System.Drawing.SystemColors.Window;
				this.FuncTreeView.BeginUpdate();
				foreach (Object obj in this.TargetNode.Functions)
				{
					if (obj is HAFuncNode) this.FuncTreeView.Nodes.Add((HAFuncNode)((HAFuncNode)obj).Clone());
				}
				this.FuncTreeView.ApplyState();
				this.FuncTreeView.EndUpdate();
				if (this.FuncTreeView.SelectedNode != null)
				{
					this.FuncTreeView.SelectedNode.EnsureVisible();
					this.FuncTreeView.TargetNode = (HAFuncNode)this.FuncTreeView.SelectedNode;
					this.FuncTreeView.SetView();
				}
			}
			else
			{
				this.FuncTreeView.Enabled = false;
				this.FuncTreeView.BackColor = System.Drawing.SystemColors.ControlLight;
			}
			this.FuncTreeView.IgnoreChange = false;
			Cursor.Current = Cursors.Default;
		}

		protected override void OnAfterSelect(System.Windows.Forms.TreeViewEventArgs e)
		{
			base.OnAfterSelect(e);
			if (this.IgnoreChange) return;

			this.StoreData();
			if (this.TargetNode == e.Node) return;

			this.TargetNode = (HAClassNode)e.Node;
			this.SetView();
		}

		#region XML

		public override void FromXml(XmlTextReader xr, TreeNodeCollection nc, int index)
		{
			DnDTreeNode dn;
			bool first = true;
			while (xr.Read())
			{
				if (xr.Name == "HAClass" && xr.NodeType == XmlNodeType.Element)
				{
					dn = new HAClassNode();
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
	/// HAClassNode の概要の説明です。
	/// </summary>
	public class HAClassNode : HATreeNode
	{
		public ArrayList Functions = new ArrayList();
		public ArrayList Members   = new ArrayList();

		public HAClassNode()
		{
		}

		public HAClassNode(string text) : base(text)
		{
		}

		public override string XmlName
		{
			get
			{
				return "HAClass";
			}
		}

		public override HATreeNode NewNode
		{
			get
			{
				return new HAClassNode();
			}
		}

		#region XML

		public override void WriteXml(XmlTextWriter xw)
		{
			base.WriteXml(xw);

			foreach (Object obj in Functions)
			{
				if (obj is HAFuncNode) ((HAFuncNode)obj).ToXml(xw);
			}
		}

		public override void ReadXmlNode(XmlTextReader xr)
		{
			if (xr.Name == "HAFunc" && xr.NodeType == XmlNodeType.Element)
			{
				HAFuncNode n = new HAFuncNode();
				Functions.Add(n);
				n.FromXml(xr);
			}
		} 

		#endregion
	}
}
