// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Girl.Coding;
using Girl.Windows.Forms;

namespace Girl.HierArch
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class HAFunc : HATree
	{
		private ContextMenu contextMenu1;
		private MenuItem mnuType;
		public HAObject ArgTreeView;
		public HAObject ObjectTreeView;
		public ExRichTextBox CommentTextBox;
		public CodeEditor SourceTextBox;
		public HAClassNode OwnerClass;
		public HAFuncNode Header;
		public HAFuncNode Body;
		public HAFuncNode Footer;
		public HAFuncNode TargetNode;
		private Font textFont;
		private ParserBase parser;
		public PropertyGrid Property;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAFunc()
		{
			this.dataFormat = "HierArch Function Data";
			this.ArgTreeView = null;
			this.ObjectTreeView = null;
			this.CommentTextBox = null;
			this.SourceTextBox = null;
			this.OwnerClass = null;
			this.TargetNode = null;
			this.Property = null;
			this.AllowDrop = true;
			this.ContextMenu = this.contextMenu1 = new ContextMenu();
			this.HideSelection = false;
			this.LabelEdit = true;
			this.ImageList = this.imageList1;
			this.textFont = new Font("ＭＳ ゴシック", 9);
			this.parser = new CSharpParser();
			this.mnuAccess.Text = "関数(&U)";
			this.mnuFolderGray.Text = "仮想フォルダ(&V)";
			this.contextMenu1.MenuItems.AddRange(new MenuItem[] { mnuType = new MenuItem("種類変更(&T)", new MenuItem[] { this.mnuAccess, this.mnuFolder, this.mnuText, this.mnuEtc }), new MenuItem("-"), this.mnuChild, this.mnuAppend, this.mnuInsert, new MenuItem("-"), this.mnuDelete, this.mnuRename });
		}

		public override void OnChanged(object sender, EventArgs e)
		{
			if (this.IgnoreChanged) return;
			base.OnChanged(sender, e);
			if (this.TargetNode != null) this.TargetNode.LastModified = DateTime.Now;
			if (this.Property != null) this.Property.Refresh();
		}

		public override void OnRefreshNode(object sender, EventArgs e)
		{
			base.OnRefreshNode(sender, e);
			if (sender != this.TargetNode) return;
			this.SetView();
		}

		protected override void MenuNodeChild_Click(object sender, EventArgs e)
		{
			HATreeNode p =(HATreeNode) this.SelectedNode;
			HATreeNode n = this.NewNode;
			if (p == this.Header || p == this.Footer)
			{
				n.Text = "新しい項目";
				n.Type = HAType.Text;
			}
			if (p != null)
			{
				p.Nodes.Add(n);
				p.SetIcon();
			}
			else
			{
				this.Nodes.Add(n);
			}
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
			if (this.Nodes.Count < 1 && this.TargetNode != null)
			{
				this.TargetNode = null;
				this.SetView();
			}
			HAFuncNode n = this.SelectedNode as HAFuncNode;
			mnuType.Enabled = mnuAppend.Enabled = mnuInsert.Enabled = mnuDelete.Enabled = mnuRename.Enabled =(n != null && n.AllowDrag);
		}

		protected override HATreeNode NewNode
		{
			get
			{
				return new HAFuncNode("新しい関数");
			}
		}

		public void StoreData()
		{
			if (this.TargetNode == null) return;
			this.StoreState();
			this.ArgTreeView.StoreState();
			this.ObjectTreeView.StoreState();
			this.TargetNode.Args.Clear();
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
				this.TargetNode.Comment = this.CommentTextBox.Text;
				this.TargetNode.CommentSelectionStart = this.CommentTextBox.SelectionStart;
				this.TargetNode.CommentSelectionLength = this.CommentTextBox.SelectionLength;
			}
			if (this.SourceTextBox != null)
			{
				this.TargetNode.Source = this.SourceTextBox.Code;
				this.TargetNode.SourceSelectionStart = this.SourceTextBox.SelectionStart;
				this.TargetNode.SourceSelectionLength = this.SourceTextBox.SelectionLength;
				if (this.TargetNode.EnableRtf)
				{
					this.TargetNode.Rtf = this.SourceTextBox.Rtf;
				}
				else
				{
					this.TargetNode.Rtf = "";
				}
			}
		}

		public void SetView()
		{
			bool flag = this.IgnoreChanged;
			this.IgnoreChanged = true;
			if (this.TargetNode != null)
			{
				if (this.ArgTreeView != null) this.ArgTreeView.SetView(this.TargetNode.Args);
				if (this.ObjectTreeView != null) this.ObjectTreeView.SetView(this.TargetNode.Objects);
				if (this.CommentTextBox != null)
				{
					this.CommentTextBox.Enabled = true;
					this.CommentTextBox.Clear();
					this.CommentTextBox.Text = this.TargetNode.Comment;
					this.CommentTextBox.SelectionStart = this.TargetNode.CommentSelectionStart;
					this.CommentTextBox.SelectionLength = this.TargetNode.CommentSelectionLength;
				}
				if (this.SourceTextBox != null)
				{
					this.SourceTextBox.Enabled = true;
					this.SourceTextBox.Clear();
					if (this.TargetNode.IsObject || this.TargetNode.Type == HAType.Class || this.TargetNode == this.Header || this.TargetNode == this.Footer)
					{
						this.SourceTextBox.Parser = this.parser;
						this.SourceTextBox.DetectUrls = false;
						this.SourceTextBox.Code = this.TargetNode.Source;
					}
					else
					{
						this.SourceTextBox.Parser = null;
						this.SourceTextBox.DetectUrls = true;
						if (this.TargetNode.EnableRtf && this.TargetNode.Rtf != "")
						{
							this.SourceTextBox.Rtf = this.TargetNode.Rtf;
						}
						else
						{
							this.SourceTextBox.Code = this.TargetNode.Source;
						}
					}
					this.SourceTextBox.SelectionStart = this.TargetNode.SourceSelectionStart;
					this.SourceTextBox.SelectionLength = this.TargetNode.SourceSelectionLength;
				}
				if (this.Property != null)
				{
					this.Property.SelectedObject = this.TargetNode.Property;
				}
			}
			else
			{
				if (this.ArgTreeView != null) this.ArgTreeView.SetView(null);
				if (this.ObjectTreeView != null) this.ObjectTreeView.SetView(null);
				if (this.CommentTextBox != null)
				{
					this.CommentTextBox.Enabled = false;
					this.CommentTextBox.Clear();
					this.CommentTextBox.SelectionStart = 0;
					this.CommentTextBox.SelectionLength = 0;
				}
				if (this.SourceTextBox != null)
				{
					this.SourceTextBox.Enabled = false;
					this.SourceTextBox.Clear();
					this.SourceTextBox.SelectionStart = 0;
					this.SourceTextBox.SelectionLength = 0;
				}
				if (this.Property != null)
				{
					this.Property.SelectedObject = null;
				}
			}
			this.IgnoreChanged = flag;
		}

		public void SetView(HAClassNode cls)
		{
			bool flag = this.IgnoreChanged;
			this.IgnoreChanged = true;
			this.SelectedNode = null;
			this.TargetNode = null;
			this.SetView();
			this.Nodes.Clear();
			if (cls != null)
			{
				this.Enabled = true;
				this.BackColor = System.Drawing.SystemColors.Window;
				this.Header = cls.Header.Clone() as HAFuncNode;
				this.Body = cls.Body.Clone() as HAFuncNode;
				this.Footer = cls.Footer.Clone() as HAFuncNode;
				if (this.OwnerClass.IsObject)
				{
					this.BeginUpdate();
					this.Nodes.Add(this.Header);
					this.Nodes.Add(this.Body);
					this.Nodes.Add(this.Footer);
					this.ApplyState();
					this.EndUpdate();
				}
				else if (this.Body.Nodes.Count > 0)
				{
					this.BeginUpdate();
					foreach (TreeNode n in this.Body.Nodes)
					{
						this.Nodes.Add(n.Clone() as HAFuncNode);
					}
					this.Body.Nodes.Clear();
					this.ApplyState();
					this.EndUpdate();
				}
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
			}
			else
			{
				this.Enabled = false;
				this.BackColor = System.Drawing.SystemColors.ControlLight;
				this.Header = this.Body = this.Footer = null;
			}
			this.SetState();
			this.IgnoreChanged = flag;
		}

		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			base.OnAfterSelect(e);
			if (this.IgnoreChanged) return;
			this.StoreData();
			if (this.TargetNode == e.Node) return;
			this.TargetNode =(HAFuncNode) e.Node;
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
}
