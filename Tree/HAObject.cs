using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Girl.Windows.Forms;

namespace Girl.HierarchyArchitect
{
	/// <summary>
	/// HAObject の概要の説明です。
	/// </summary>
	public class HAObject : HATree
	{
		private System.Windows.Forms.ContextMenu contextMenu1;

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(HAObject));
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			// 
			// HAObject
			// 
			this.AllowDrop = true;
			this.ContextMenu = this.contextMenu1;
			this.HideSelection = false;
			this.LabelEdit = true;
		}
	
		public HAObject()
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
			this.StoreState();
			base.StartDrag();
		}

		protected override void SetState()
		{
			mnuDelete.Enabled = (this.SelectedNode != null);
		}

		protected override HATreeNode NewNode
		{
			get
			{
				HAObjectNode ret = new HAObjectNode("新しいオブジェクト");
				ret.Type = HAType.Private;
				return ret;
			}
		}

		public void SetView(ArrayList list)
		{
			this.IgnoreChange = true;
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
						if (obj is HAObjectNode) Nodes.Add((obj as HAObjectNode).Clone() as HAObjectNode);
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
			this.IgnoreChange = false;
			this.SetState();
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
					dn = new HAObjectNode();
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
	/// HAObjectNode の概要の説明です。
	/// </summary>
	public class HAObjectNode : HATreeNode
	{
		public HAObjectNode()
		{
		}

		public HAObjectNode(string text) : base(text)
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
				return new HAObjectNode();
			}
		}

		public override void SetIcon()
		{
			switch (this.m_Type)
			{
				case HAType.Public:
				case HAType.Protected:
				case HAType.Private:
					this.SelectedImageIndex = (int)HAType.PointRed;
					this.ImageIndex         = (int)HAType.Point;
					return;
			}
			base.SetIcon();
		}
	}
}