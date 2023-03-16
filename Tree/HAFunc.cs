using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Girl.Code;
using Girl.Windows.Forms;

namespace Girl.HierarchyArchitect
{
	/// <summary>
	/// HAFunc の概要の説明です。
	/// </summary>
	public class HAFunc : HATree
	{
		private System.Windows.Forms.ContextMenu contextMenu1;
		public HAObject ArgTreeView = null;
		public HAObject ObjectTreeView = null;
		public TextBox CommentTextBox = null;
		public TextBox SourceTextBox  = null;
		public HAFuncNode Header, Body, Footer;
		public HAClassNode OwnerClass = null;

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

		private MenuItem mnuType;
	
		public HAFunc()
		{
			InitializeComponent();

			this.contextMenu1.MenuItems.AddRange(new MenuItem[]
				{
					mnuType = new MenuItem("種類変更(&T)", new MenuItem[]
						{
							this.mnuAccess,
							this.mnuFolder,
							this.mnuText,
							this.mnuEtc
						}),
					new MenuItem("-"),
					this.mnuChild,
					this.mnuAppend,
					this.mnuInsert,
					new MenuItem("-"),
					this.mnuDelete
				});

			this.ImageList = this.imageList1;
		}

		protected override void MenuNodeChild_Click(object sender, System.EventArgs e)
		{
			HATreeNode p = (HATreeNode)this.SelectedNode;
			if (p == null) return;

			HATreeNode n = this.NewNode;
			if (p == this.Header || p == this.Footer)
			{
				n.Text = "新しい項目";
				n.Type = HAType.Text;
			}
			p.Nodes.Add(n);
			p.SetIcon();
			n.EnsureVisible();
			this.SelectedNode = n;
			n.BeginEdit();
			this.OnChanged(this, EventArgs.Empty);
		}

		protected override void StartDrag()
		{
			this.Focus();
			this.StoreData();
			base.StartDrag();
		}

		protected override void SetState()
		{
			HAFuncNode n = this.SelectedNode as HAFuncNode;
			mnuType.Enabled = mnuAppend.Enabled = mnuInsert.Enabled = mnuDelete.Enabled = (n != null && n.AllowDrag);
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

			this.ArgTreeView   .StoreState();
			this.ObjectTreeView.StoreState();
			this.TargetNode.Args   .Clear();
			this.TargetNode.Objects.Clear();
			foreach (TreeNode n in this.ArgTreeView.Nodes)
			{
				if (n is HAObjectNode) this.TargetNode.Args.Add(n.Clone());
			}

			foreach (TreeNode n in this.ObjectTreeView.Nodes)
			{
				if (n is HAObjectNode) this.TargetNode.Objects.Add(n.Clone());
			}

			if (this.CommentTextBox != null)
			{
				this.TargetNode.Comment                = this.CommentTextBox.Text;
				this.TargetNode.CommentSelectionStart  = this.CommentTextBox.SelectionStart;
				this.TargetNode.CommentSelectionLength = this.CommentTextBox.SelectionLength;
			}
			if (this.SourceTextBox  != null)
			{
				this.TargetNode.Source                = this.SourceTextBox .Text;
				this.TargetNode.SourceSelectionStart  = this.SourceTextBox.SelectionStart;
				this.TargetNode.SourceSelectionLength = this.SourceTextBox.SelectionLength;
			}
		}

		public void SetView()
		{
			if (this.TargetNode != null)
			{
				if (this.ArgTreeView    != null) this.ArgTreeView   .SetView(this.TargetNode.Args);
				if (this.ObjectTreeView != null) this.ObjectTreeView.SetView(this.TargetNode.Objects);
				if (this.CommentTextBox != null)
				{
					this.CommentTextBox.Enabled = true;
					this.CommentTextBox.Text = this.TargetNode.Comment;
					this.CommentTextBox.SelectionStart  = this.TargetNode.CommentSelectionStart;
					this.CommentTextBox.SelectionLength = this.TargetNode.CommentSelectionLength;
				}
				if (this.SourceTextBox != null)
				{
					this.SourceTextBox.Enabled = true;
					this.SourceTextBox.Text = this.TargetNode.Source;
					this.SourceTextBox.SelectionStart  = this.TargetNode.SourceSelectionStart;
					this.SourceTextBox.SelectionLength = this.TargetNode.SourceSelectionLength;
				}
			}
			else
			{
				if (this.ArgTreeView    != null) this.ArgTreeView   .SetView(null);
				if (this.ObjectTreeView != null) this.ObjectTreeView.SetView(null);
				if (this.CommentTextBox != null)
				{
					this.CommentTextBox.Enabled = false;
					this.CommentTextBox.Text = "";
					this.CommentTextBox.SelectionStart  = 0;
					this.CommentTextBox.SelectionLength = 0;
				}
				if (this.SourceTextBox != null)
				{
					this.SourceTextBox.Enabled = false;
					this.SourceTextBox.Text = "";
					this.SourceTextBox.SelectionStart  = 0;
					this.SourceTextBox.SelectionLength = 0;
				}
			}
		}

		public void SetView(HAClassNode cls)
		{
			this.IgnoreChanged = true;
			this.SelectedNode = null;
			this.TargetNode = null;
			this.SetView();
			this.Nodes.Clear();
			if (cls != null)
			{
				this.Enabled = true;
				this.BackColor = System.Drawing.SystemColors.Window;
				this.BeginUpdate();
				this.Header = cls.Header.Clone() as HAFuncNode;
				this.Body   = cls.Body  .Clone() as HAFuncNode;
				this.Footer = cls.Footer.Clone() as HAFuncNode;
				if (this.OwnerClass.Type == HAType.Public
					|| this.OwnerClass.Type == HAType.Protected
					|| this.OwnerClass.Type == HAType.Private)
				{
					this.Nodes.Add(this.Header);
					this.Nodes.Add(this.Body);
					this.Nodes.Add(this.Footer);
				}
				else
				{
					this.Nodes.Add(this.Body);
				}
				this.ApplyState();
				if (this.SelectedNode == null && this.Nodes.Count > 0)
				{
					this.SelectedNode = this.Nodes[0];
				}
				if (this.SelectedNode != null)
				{
					this.SelectedNode.EnsureVisible();
					this.TargetNode = this.SelectedNode as HAFuncNode;
					this.SetView();
				}
				this.EndUpdate();
			}
			else
			{
				this.Enabled = false;
				this.BackColor = System.Drawing.SystemColors.ControlLight;
				this.Header = this.Body = this.Footer = null;
			}
			this.SetState();
			this.IgnoreChanged = false;
		}

		protected override void OnAfterSelect(System.Windows.Forms.TreeViewEventArgs e)
		{
			base.OnAfterSelect(e);
			if (this.IgnoreChanged) return;

			this.StoreData();
			if (this.TargetNode == e.Node) return;

			this.TargetNode = (HAFuncNode)e.Node;
			this.IgnoreChanged = true;
			this.SetView();
			this.IgnoreChanged = false;
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
						this.OnChanged(this, new EventArgs());
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
		public int CommentSelectionStart  = 0;
		public int CommentSelectionLength = 0;
		public int SourceSelectionStart   = 0;
		public int SourceSelectionLength  = 0;

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
			HAFuncNode ret = base.Clone() as HAFuncNode;
			ret.Args    = this.Args.Clone() as ArrayList;
			ret.Objects = this.Objects.Clone() as ArrayList;
			ret.Comment = this.Comment;
			ret.Source  = this.Source;
			ret.CommentSelectionStart  = this.CommentSelectionStart;
			ret.CommentSelectionLength = this.CommentSelectionLength;
			ret.SourceSelectionStart   = this.SourceSelectionStart;
			ret.SourceSelectionLength  = this.SourceSelectionLength;
			return ret;
		}

		#region XML

		public override void WriteXml(XmlTextWriter xw)
		{
			base.WriteXml(xw);

			xw.WriteStartElement("Arguments");
			foreach (Object obj in this.Args)
			{
				if (obj is HAObjectNode) (obj as HAObjectNode).ToXml(xw);
			}
			xw.WriteEndElement();

			foreach (Object obj in this.Objects)
			{
				if (obj is HAObjectNode) (obj as HAObjectNode).ToXml(xw);
			}

			xw.WriteStartElement("Comment");
			xw.WriteAttributeString("SelectionStart" , XmlConvert.ToString(this.CommentSelectionStart));
			xw.WriteAttributeString("SelectionLength", XmlConvert.ToString(this.CommentSelectionLength));
			xw.WriteString(this.Comment);
			xw.WriteEndElement();

			xw.WriteStartElement("Source");
			xw.WriteAttributeString("SelectionStart" , XmlConvert.ToString(this.SourceSelectionStart));
			xw.WriteAttributeString("SelectionLength", XmlConvert.ToString(this.SourceSelectionLength));
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
				this.CommentSelectionStart  = XmlConvert.ToInt32(xr.GetAttribute("SelectionStart"));
				this.CommentSelectionLength = XmlConvert.ToInt32(xr.GetAttribute("SelectionLength"));
				if (!xr.IsEmptyElement && xr.Read()) this.Comment = xr.ReadString();
			}
			else if (xr.Name == "Source" && xr.NodeType == XmlNodeType.Element)
			{
				this.SourceSelectionStart  = XmlConvert.ToInt32(xr.GetAttribute("SelectionStart"));
				this.SourceSelectionLength = XmlConvert.ToInt32(xr.GetAttribute("SelectionLength"));
				if (!xr.IsEmptyElement && xr.Read()) this.Source = xr.ReadString();
			}
		} 

		#endregion

		#region Generation

		public void Generate(CodeWriter cw, HAType classType)
		{
			HAType t = this.Type;
			if (this.IsObject)
			{
				this.GenerateFunc(cw);
			}
			else if (t.ToString().StartsWith("Folder"))
			{
				cw.WriteBlankLine();
				cw.WriteCode("#region " + this.Text);
			}

			foreach (TreeNode n in this.Nodes)
			{
				(n as HAFuncNode).Generate(cw, classType);
			}

			if (t.ToString().StartsWith("Folder"))
			{
				cw.WriteBlankLine();
				cw.WriteCode("#endregion");
			}
		}

		private void GenerateFunc(CodeWriter cw)
		{
			cw.WriteBlankLine();
			if (this.Comment != "") cw.WriteCodes("/// ", this.Comment);

			string code = this.Type.ToString().ToLower()
				+ " " + new ObjectParser(this.Text).FunctionDeclaration + "(";
			bool first = true;
			foreach (Object obj in this.Args)
			{
				HAObjectNode n = obj as HAObjectNode;
				if (n == null || !n.IsObject) continue;

				if (!first)
				{
					code += ", ";
				}
				else
				{
					first = false;
				}
				code += new ObjectParser(n.Text).ObjectDeclaration;
			}
			code += ")";

			cw.WriteStartBlock(cw.ReplaceKeywords(code));
			cw.SetStart();
			foreach (Object obj in this.Objects)
			{
				HAObjectNode n = obj as HAObjectNode;
				if (n == null || !n.IsObject) continue;

				cw.WriteCode(new ObjectParser(n.Text).ObjectDeclaration + ";");
			}
			if (this.Source != "")
			{
				cw.WriteBlankLine();
				cw.WriteCodes(cw.ReplaceKeywords(this.Source));
			}
			cw.WriteEndBlock();
		}

		#endregion
	}
}
