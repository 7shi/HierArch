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
		public HAMember MemberTreeView = null;
		public HAFunc FuncTreeView = null;

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(HAClass));
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
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

			this.contextMenu1.MenuItems.AddRange(new MenuItem[]
				{
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
			this.StoreData();
			base.StartDrag();
		}

		protected override void SetState()
		{
			if (this.Nodes.Count < 1 && this.TargetNode != null)
			{
				this.TargetNode = null;
				SetView();
			}
			mnuDelete.Enabled = (this.SelectedNode != null);
		}

		protected override HATreeNode NewNode
		{
			get
			{
				HAClassNode ret = new HAClassNode("新しいクラス");
				ret.Type = HAType.Class;

				HAFuncNode hdr = new HAFuncNode("ヘッダ");
				hdr.Type = HAType.Text;
				hdr.m_IsSelected = true;
				hdr.AllowDrag = false;
				ret.Functions.Add(hdr);

				HAFuncNode cls = new HAFuncNode("クラス");
				cls.Type = HAType.Class;
				cls.m_IsExpanded = true;
				cls.AllowDrag = false;

				HAFuncNode cst = new HAFuncNode("__CLASS");
				cst.Comment = "コンストラクタ\r\n※ __CLASS はクラス名に置換されます。\r\n";
				cls.Nodes.Add(cst);

				HAFuncNode dst = new HAFuncNode("~__CLASS");
				dst.Comment = "デストラクタ\r\n※ __CLASS はクラス名に置換されます。\r\n";
				cls.Nodes.Add(dst);

				ret.Functions.Add(cls);

				HAFuncNode ftr = new HAFuncNode("フッタ");
				ftr.Type = HAType.Text;
				ftr.AllowDrag = false;
				ret.Functions.Add(ftr);

				return ret;
			}
		}

		private HAClassNode TargetNode = null;

		public void StoreData()
		{
			if (this.TargetNode == null || this.FuncTreeView == null) return;

			this.StoreState();

			this.FuncTreeView  .StoreData();
			this.MemberTreeView.StoreState();
			this.TargetNode.Functions.Clear();
			this.TargetNode.Members  .Clear();
			foreach (TreeNode n in this.MemberTreeView.Nodes)
			{
				if (n is HAMemberNode) this.TargetNode.Members.Add(n.Clone());
			}

			foreach (TreeNode n in this.FuncTreeView.Nodes)
			{
				if (n is HAFuncNode) this.TargetNode.Functions.Add(n.Clone());
			}
		}

		public void SetView()
		{
			Cursor curOrig = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			if (this.TargetNode != null)
			{
				if (this.MemberTreeView != null) this.MemberTreeView.SetView(this.TargetNode.Members);
				if (this.FuncTreeView   != null) this.FuncTreeView  .SetView(this.TargetNode.Functions);
			}
			else
			{
				if (this.MemberTreeView != null) this.MemberTreeView.SetView(null);
				if (this.FuncTreeView   != null) this.FuncTreeView  .SetView(null);
			}
			this.SetState();

			Cursor.Current = curOrig;
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

			foreach (Object obj in this.Members)
			{
				if (obj is HAMemberNode) ((HAMemberNode)obj).ToXml(xw);
			}

			foreach (Object obj in this.Functions)
			{
				if (obj is HAFuncNode) ((HAFuncNode)obj).ToXml(xw);
			}
		}

		public override void ReadXmlNode(XmlTextReader xr)
		{
			if (xr.Name == "HAObject" && xr.NodeType == XmlNodeType.Element)
			{
				HAMemberNode n = new HAMemberNode();
				Members.Add(n);
				n.FromXml(xr);
			}
			else if (xr.Name == "HAFunc" && xr.NodeType == XmlNodeType.Element)
			{
				HAFuncNode n = new HAFuncNode();
				Functions.Add(n);
				n.FromXml(xr);
			}
		} 

		#endregion
	}
}
