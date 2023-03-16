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
		}

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
			this.SelectedImageIndex = (int)m_Type;
			this.ImageIndex         = (int)m_Type;
		}

		public override object Clone()
		{
			HATreeNode ret = (HATreeNode)base.Clone();
			ret.m_IsExpanded = this.m_IsExpanded;
			ret.m_IsSelected = this.m_IsSelected;
			ret.Type         = this.Type;
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
