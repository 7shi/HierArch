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
		public HAObject ObjectTreeView = null;
		public System.Windows.Forms.TextBox CommentTextBox = null;
		public System.Windows.Forms.TextBox SourceTextBox  = null;

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(HAFunc));
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
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
			mnuDelete.Enabled = (this.TargetNode != null && this.TargetNode.AllowDrag);
		}

		protected override HATreeNode NewNode
		{
			get
			{
				return new HAFuncNode("新しい関数");
			}
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

		public void SetView(ArrayList list)
		{
			this.IgnoreChange = true;
			this.SelectedNode = null;
			this.TargetNode = null;
			this.SetView();
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
						if (obj is HAFuncNode) this.Nodes.Add((HAFuncNode)((HAFuncNode)obj).Clone());
					}
					this.ApplyState();
					this.EndUpdate();
					if (this.SelectedNode != null)
					{
						this.SelectedNode.EnsureVisible();
						this.TargetNode = (HAFuncNode)this.SelectedNode;
						this.SetView();
					}
				}
			}
			else
			{
				this.Enabled = false;
				this.BackColor = System.Drawing.SystemColors.ControlLight;
			}
			this.IgnoreChange = false;
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
	}

	/// <summary>
	/// HAFuncNode の概要の説明です。
	/// </summary>
	public class HAFuncNode : HATreeNode
	{
		public ArrayList Args    = new ArrayList();
		public ArrayList Objects = new ArrayList();
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
			ret.Args    = (ArrayList)this.Args.Clone();
			ret.Objects = (ArrayList)this.Objects.Clone();
			ret.Comment = this.Comment;
			ret.Source  = this.Source;
			return ret;
		}

		#region XML

		public override void WriteXml(XmlTextWriter xw)
		{
			base.WriteXml(xw);

			xw.WriteStartElement("Arguments");
			foreach (Object obj in this.Args)
			{
				if (obj is HAObjectNode) ((HAObjectNode)obj).ToXml(xw);
			}
			xw.WriteEndElement();

			foreach (Object obj in this.Objects)
			{
				if (obj is HAObjectNode) ((HAObjectNode)obj).ToXml(xw);
			}

			xw.WriteStartElement("Comment");
			xw.WriteString(this.Comment);
			xw.WriteEndElement();

			xw.WriteStartElement("Source");
			xw.WriteString(this.Source);
			xw.WriteEndElement();
		}

		public override void ReadXmlNode(XmlTextReader xr)
		{
			if (xr.Name == "Arguments" && xr.NodeType == XmlNodeType.Element && !xr.IsEmptyElement)
			{
				while (xr.Read())
				{
					if (xr.Name == "Arguments" && xr.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					else if (xr.Name == "HAObject" && xr.NodeType == XmlNodeType.Element)
					{
						HAObjectNode n = new HAObjectNode();
						this.Args.Add(n);
						n.FromXml(xr);
					}
				}
			}
			else if (xr.Name == "HAObject" && xr.NodeType == XmlNodeType.Element)
			{
				HAObjectNode n = new HAObjectNode();
				this.Objects.Add(n);
				n.FromXml(xr);
			}
			else if (xr.Name == "Comment" && xr.NodeType == XmlNodeType.Element)
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
