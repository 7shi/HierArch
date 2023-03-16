using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Girl.Windows.Forms;

namespace Girl.HierarchyArchitect
{
	/// <summary>
	/// HATree の概要の説明です。
	/// </summary>
	public class HATree : DnDTreeView
	{
		private System.ComponentModel.IContainer components;
		public System.Windows.Forms.ImageList imageList1;
		public bool IgnoreChange = false;

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(HATree));
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// HATree
			// 
			this.ItemHeight = 14;

		}

		public HATree()
		{
			InitializeComponent();
			MakeMenu();
		}

		protected virtual HATreeNode NewNode
		{
			get
			{
				return new HATreeNode("新しいノード");
			}
		}

		#region Menu

		public MenuItem
			mnuChild, mnuAppend, mnuInsert, mnuDelete,
			mnuAccess, mnuAccessPublic, mnuAccessProtected, mnuAccessPrivate,
			mnuFolder, mnuFolderNormal, mnuFolderBlue, mnuFolderBrown,
			mnuFolderGray, mnuFolderGreen, mnuFolderRed,
			mnuText, mnuTextNormal, mnuTextBlue, mnuTextBrown,
			mnuTextGray, mnuTextGreen, mnuTextRed,
			mnuEtc, mnuEtcComment, mnuEtcSmile;

		public System.Collections.Hashtable menuType = new System.Collections.Hashtable();

		private void MakeMenu()
		{
			mnuChild  = new MenuItem("下に追加(&C)", new EventHandler(this.MenuNodeChild_Click));
			mnuAppend = new MenuItem("後に追加(&A)", new EventHandler(this.MenuNodeAppend_Click));
			mnuInsert = new MenuItem("前に追加(&I)", new EventHandler(this.MenuNodeInsert_Click));
			mnuDelete = new MenuItem("削除(&D)"    , new EventHandler(this.MenuNodeDelete_Click));

			EventHandler eh = new EventHandler(this.MenuNodeType_Click);

			this.mnuAccess = new MenuItem("アクセス制御(&A)", new MenuItem[]
				{
					this.mnuAccessPublic    = new MenuItem("&Public", eh),
					this.mnuAccessProtected = new MenuItem("P&rotected", eh),
					this.mnuAccessPrivate   = new MenuItem("Pr&ivate", eh)
                });

			this.mnuFolder = new MenuItem("フォルダ(&F)", new MenuItem[]
				{
					this.mnuFolderNormal = new MenuItem("標準(&N)", eh),
					this.mnuFolderRed    = new MenuItem("赤色(&R)", eh),
					this.mnuFolderBlue   = new MenuItem("青色(&B)", eh),
					this.mnuFolderGreen  = new MenuItem("緑色(&G)", eh),
					this.mnuFolderGray   = new MenuItem("灰色(&Y)", eh),
					this.mnuFolderBrown  = new MenuItem("茶色(&W)", eh)
				});

			this.mnuText = new MenuItem("テキスト(&T)", new MenuItem[]
				{
					this.mnuTextNormal = new MenuItem("標準(&N)", eh),
					this.mnuTextRed    = new MenuItem("赤色(&R)", eh),
					this.mnuTextBlue   = new MenuItem("青色(&B)", eh),
					this.mnuTextGreen  = new MenuItem("緑色(&G)", eh),
					this.mnuTextGray   = new MenuItem("灰色(&Y)", eh),
					this.mnuTextBrown  = new MenuItem("茶色(&W)", eh)
				});

			this.mnuEtc = new MenuItem("その他(&E)", new MenuItem[]
				{
					this.mnuEtcComment = new MenuItem("注釈(&C)", eh),
					this.mnuEtcSmile   = new MenuItem("人物(&H)", eh)
				});

			menuType.Add(this.mnuAccessPublic   , HAType.Public);
			menuType.Add(this.mnuAccessProtected, HAType.Protected);
			menuType.Add(this.mnuAccessPrivate  , HAType.Private);
			menuType.Add(this.mnuFolderNormal   , HAType.Folder);
			menuType.Add(this.mnuFolderRed      , HAType.FolderRed);
			menuType.Add(this.mnuFolderBlue     , HAType.FolderBlue);
			menuType.Add(this.mnuFolderGreen    , HAType.FolderGreen);
			menuType.Add(this.mnuFolderGray     , HAType.FolderGray);
			menuType.Add(this.mnuFolderBrown    , HAType.FolderBrown);
			menuType.Add(this.mnuTextNormal     , HAType.Text);
			menuType.Add(this.mnuTextRed        , HAType.TextRed);
			menuType.Add(this.mnuTextBlue       , HAType.TextBlue);
			menuType.Add(this.mnuTextGreen      , HAType.TextGreen);
			menuType.Add(this.mnuTextGray       , HAType.TextGray);
			menuType.Add(this.mnuTextBrown      , HAType.TextBrown);
			menuType.Add(this.mnuEtcComment     , HAType.Comment);
			menuType.Add(this.mnuEtcSmile       , HAType.Smile);
		}

		protected virtual void MenuNodeType_Click(object sender, System.EventArgs e)
		{
			HATreeNode n = (HATreeNode)this.SelectedNode;
			if (n != null && n.AllowDrag) n.Type = (HAType)menuType[sender];
		}

		protected virtual void MenuNodeChild_Click(object sender, System.EventArgs e)
		{
			HATreeNode n = this.NewNode;
			HATreeNode p = (HATreeNode)this.SelectedNode;
			if (p == null)
			{
				this.Nodes.Add(n);
			}
			else
			{
				p.Nodes.Add(n);
				p.SetIcon();
			}
			n.EnsureVisible();
			this.SelectedNode = n;
			n.BeginEdit();
		}

		protected virtual void MenuNodeAppend_Click(object sender, System.EventArgs e)
		{
			HATreeNode n = this.NewNode;
			HATreeNode p = (HATreeNode)this.SelectedNode;
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

		protected virtual void MenuNodeInsert_Click(object sender, System.EventArgs e)
		{
			HATreeNode n = this.NewNode;
			HATreeNode p = (HATreeNode)this.SelectedNode;
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

		protected virtual void MenuNodeDelete_Click(object sender, System.EventArgs e)
		{
			HATreeNode n = (HATreeNode)this.SelectedNode;
			if (n == null) return;

			HATreeNode p = (HATreeNode)n.Parent;
			TreeNodeCollection tc = (p != null) ? p.Nodes : this.Nodes;
			tc.Remove(n);
			if (p != null && tc.Count < 1) p.SetIcon();
			if (this.Nodes.Count < 1) this.SetState();
		}

		#endregion

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			if (this.ImageList != null)
			{
				int h = this.ImageList.ImageSize.Height;
				if (this.ItemHeight < h) this.ItemHeight = h;
			}
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

		protected override void OnAfterCollapse(System.Windows.Forms.TreeViewEventArgs e)
		{
			base.OnAfterCollapse(e);
			((HATreeNode)e.Node).SetIcon();
		}

		protected override void OnAfterExpand(System.Windows.Forms.TreeViewEventArgs e)
		{
			base.OnAfterExpand(e);
			((HATreeNode)e.Node).SetIcon();
		}

		protected virtual void SetState()
		{
		}

		public void StoreState()
		{
			foreach (TreeNode n in this.Nodes)
			{
				if (n is HATreeNode) ((HATreeNode)n).StoreState();
			}
		}

		public void ApplyState()
		{
			foreach (TreeNode n in this.Nodes)
			{
				if (n is HATreeNode) ((HATreeNode)n).ApplyState();
			}
		}
	}

	/// <summary>
	/// HATreeNode の概要の説明です。
	/// </summary>
	public class HATreeNode : DnDTreeNode
	{
		public HAType m_Type;
		public bool m_IsExpanded = false;
		public bool m_IsSelected = false;

		public HATreeNode()
		{
			Type = HAType.Public;
		}

		public HATreeNode(string text) : base(text)
		{
			Type = HAType.Public;
		}

		public virtual string XmlName
		{
			get
			{
				return "HATree";
			}
		}

		public virtual HATreeNode NewNode
		{
			get
			{
				return new HATreeNode();
			}
		}

		public HAType Type
		{
			get
			{
				return m_Type;
			}

			set
			{
				m_Type = value;
				SetIcon();
			}
		}

		public virtual void SetIcon()
		{
			int t = (int)m_Type;
			if (m_Type.ToString().StartsWith("Folder"))
			{
				if (m_Type.ToString().EndsWith("_Open")) t--;
				if (this.Nodes.Count > 0 && this.IsExpanded) t++;
			}
			this.SelectedImageIndex = this.ImageIndex = t;
		}

		public override object Clone()
		{
			HATreeNode ret = (HATreeNode)base.Clone();
			ret.Type         = this.Type;
			ret.m_IsExpanded = this.m_IsExpanded;
			ret.m_IsSelected = this.m_IsSelected;
			return ret;
		}

		public void StoreState()
		{
			this.m_IsExpanded = this.IsExpanded;
			this.m_IsSelected = this.IsSelected;

			foreach (TreeNode n in this.Nodes)
			{
				if (n is HATreeNode) ((HATreeNode)n).StoreState();
			}
		}

		public void ApplyState()
		{
			if (this.m_IsExpanded) this.Expand();
			if (this.m_IsSelected) this.TreeView.SelectedNode = this;

			foreach (TreeNode n in this.Nodes)
			{
				if (n is HATreeNode) ((HATreeNode)n).ApplyState();
			}
		}

		#region XML

		public override void ToXml(XmlTextWriter xw)
		{
			xw.WriteStartElement(this.XmlName);
			this.WriteXml(xw);

			DnDTreeNode dn;
			foreach(TreeNode n in Nodes)
			{
				dn = (DnDTreeNode)n;
				if (dn != null) dn.ToXml(xw);
			}

			xw.WriteEndElement();
		}

		public virtual void WriteXml(XmlTextWriter xw)
		{
			xw.WriteAttributeString("Type", Type.ToString());
			xw.WriteAttributeString("Text", this.Text);
			xw.WriteAttributeString("IsExpanded", XmlConvert.ToString(this.m_IsExpanded));
			xw.WriteAttributeString("IsSelected", XmlConvert.ToString(this.m_IsSelected));
			xw.WriteAttributeString("AllowDrag" , XmlConvert.ToString(this.AllowDrag));
		}

		public override void FromXml(XmlTextReader xr)
		{
			if (xr.Name != this.XmlName || xr.NodeType != XmlNodeType.Element) return;

			this.ReadXml(xr);
			if (xr.IsEmptyElement) return;

			HATreeNode n;
			while (xr.Read())
			{
				if (xr.Name == this.XmlName && xr.NodeType == XmlNodeType.Element)
				{
					n = this.NewNode;
					Nodes.Add(n);
					n.FromXml(xr);
				}
				else if (xr.Name == this.XmlName && xr.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				else
				{
					ReadXmlNode(xr);
				}
			}

			if (m_IsExpanded) Expand();
		} 

		public virtual void ReadXml(XmlTextReader xr)
		{
			this.Type = (HAType)HAType.Parse(typeof(HAType), xr.GetAttribute("Type"));
			this.Text = xr.GetAttribute("Text");
			this.m_IsExpanded = XmlConvert.ToBoolean(xr.GetAttribute("IsExpanded"));
			this.m_IsSelected = XmlConvert.ToBoolean(xr.GetAttribute("IsSelected"));
			this.AllowDrag    = XmlConvert.ToBoolean(xr.GetAttribute("AllowDrag"));
		}

		public virtual void ReadXmlNode(XmlTextReader xr)
		{
		}

		#endregion
	}

	public enum HAType
	{
		Public,
		Protected,
		Private,
		Class,
		Module,
		Comment,
		Smile,
		Folder,
		Folder_Open,
		FolderBlue,
		FolderBule_Open,
		FolderBrown,
		FolderBrown_Open,
		FolderGray,
		FolderGray_Open,
		FolderGreen,
		FolderGreen_Open,
		FolderRed,
		FolderRed_Open,
		Text,
		TextBlue,
		TextBrown,
		TextGray,
		TextGreen,
		TextRed,
		Point,
		PointRed
	}
}
