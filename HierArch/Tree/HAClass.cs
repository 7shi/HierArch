using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Girl.Code;
using Girl.Windows.Forms;

namespace Girl.HierArch
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

		private MenuItem mnuType;
	
		public HAClass()
		{
			InitializeComponent();

			this.mnuAccess     .Text = "クラス(&C)";
			this.mnuFolderRed  .Text = "GUI 実行ファイル(&W)";
            this.mnuFolderBlue .Text = "CUI 実行ファイル(&E)";
			this.mnuFolderGreen.Text = "ライブラリ(&L)";
			this.mnuFolderBrown.Text = "モジュール(&M)";
            this.mnuFolderGray .Text = "仮想フォルダ(&V)";

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

		protected override void OnNodeTypeChanged(object sender, EventArgs e)
		{
			this.StoreData();
			this.SetView();
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

			HAClassNode n = this.SelectedNode as HAClassNode;
			mnuType  .Enabled = mnuDelete.Enabled = (n != null && n.AllowDrag);
		}

		public void InitNode(HAClassNode n)
		{
			n.Header = new HAFuncNode("ヘッダ");
			n.Header.Type = HAType.Text;
			n.Header.m_IsSelected = true;
			n.Header.AllowDrag = false;
			n.Header.Comment = "ここにソースコードの注釈を書きます。\r\n";
			n.Header.Source  = "using System;\r\n";

			n.Body = new HAFuncNode("本体");
			n.Body.Type = HAType.Class;
			n.Body.m_IsExpanded = true;
			n.Body.AllowDrag = false;
			n.Body.Comment = "<summary>\r\nここにクラスの説明を書きます。\r\n</summary>\r\n";

			HAFuncNode cst = new HAFuncNode("__CLASS");
			cst.Comment = "<summary>\r\nコンストラクタです。\r\n</summary>\r\n";
			n.Body.Nodes.Add(cst);

			HAFuncNode dst = new HAFuncNode("~__CLASS");
			dst.Comment = "<summary>\r\nデストラクタです。\r\n</summary>\r\n";
			n.Body.Nodes.Add(dst);

			n.Footer = new HAFuncNode("フッタ");
			n.Footer.Type = HAType.Text;
			n.Footer.AllowDrag = false;
		}

		protected override HATreeNode NewNode
		{
			get
			{
				HAClassNode ret = new HAClassNode("新しいクラス");
				this.InitNode(ret);
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
			this.TargetNode.Members  .Clear();
			foreach (TreeNode n in this.MemberTreeView.Nodes)
			{
				if (n is HAMemberNode) this.TargetNode.Members.Add(n.Clone());
			}

			this.TargetNode.Header = this.FuncTreeView.Header.Clone() as HAFuncNode;
			this.TargetNode.Body   = this.FuncTreeView.Body  .Clone() as HAFuncNode;
			if (this.FuncTreeView.Body.TreeView == null)
			{
				foreach (TreeNode n in this.FuncTreeView.Nodes)
				{
					this.TargetNode.Body.Nodes.Add(n.Clone() as HAFuncNode);
				}
			}
			this.TargetNode.Footer = this.FuncTreeView.Footer.Clone() as HAFuncNode;
		}

		public void SetView()
		{
			Cursor curOrig = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			if (this.TargetNode != null)
			{
				if (this.MemberTreeView != null) this.MemberTreeView.SetView(this.TargetNode.Members);
				if (this.FuncTreeView   != null)
				{
					this.FuncTreeView.OwnerClass = this.TargetNode;
					this.FuncTreeView.SetView(this.TargetNode);
				}
			}
			else
			{
				if (this.MemberTreeView != null) this.MemberTreeView.SetView(null);
				if (this.FuncTreeView   != null)
				{
					this.FuncTreeView.OwnerClass = null;
					this.FuncTreeView.SetView(null);
				}
			}
			this.SetState();

			Cursor.Current = curOrig;
		}

		protected override void OnAfterSelect(System.Windows.Forms.TreeViewEventArgs e)
		{
			base.OnAfterSelect(e);
			if (this.IgnoreChanged) return;

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
						this.OnChanged(this, new EventArgs());
					}
				}
			}
		}

		#endregion

		#region Generation

		public void Generate(string path)
		{
			this.StoreData();
			foreach (TreeNode n in this.Nodes)
			{
				(n as HAClassNode).Generate(path);
			}
		}

		#endregion
	}

	/// <summary>
	/// HAClassNode の概要の説明です。
	/// </summary>
	public class HAClassNode : HATreeNode
	{
		public ArrayList Members  = new ArrayList();
		public HAFuncNode Header, Body, Footer;

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

		public string Namespace
		{
			get
			{
				ObjectParser op = new ObjectParser(this.Text);
				string ns = (this.IsFolder) ? op.Type : "";
				if (ns.IndexOf('.') >= 0 || this.Parent == null) return ns;

				string pns = (this.Parent as HAClassNode).Namespace;
				if (pns != "") return (ns != "") ? pns + "." + ns : pns;
				return ns;
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

			xw.WriteStartElement("Header");
			this.Header.ToXml(xw);
			xw.WriteEndElement();

			xw.WriteStartElement("Body");
			this.Body.ToXml(xw);
			xw.WriteEndElement();

			xw.WriteStartElement("Footer");
			this.Footer.ToXml(xw);
			xw.WriteEndElement();
		}

		public override void ReadXmlNode(XmlTextReader xr)
		{
			if (xr.Name == "HAObject" && xr.NodeType == XmlNodeType.Element)
			{
				HAMemberNode n = new HAMemberNode();
				Members.Add(n);
				n.FromXml(xr);
			}
			else if (xr.Name == "Header" && xr.NodeType == XmlNodeType.Element && !xr.IsEmptyElement)
			{
				this.Header = new HAFuncNode();
				while (xr.Read() && xr.NodeType == XmlNodeType.Whitespace);
				if (xr.Name == "HAFunc" && xr.NodeType == XmlNodeType.Element) this.Header.FromXml(xr);
			}
			else if (xr.Name == "Body" && xr.NodeType == XmlNodeType.Element && !xr.IsEmptyElement)
			{
				this.Body = new HAFuncNode();
				while (xr.Read() && xr.NodeType == XmlNodeType.Whitespace);
				if (xr.Name == "HAFunc" && xr.NodeType == XmlNodeType.Element) this.Body.FromXml(xr);
			}
			else if (xr.Name == "Footer" && xr.NodeType == XmlNodeType.Element && !xr.IsEmptyElement)
			{
				this.Footer = new HAFuncNode();
				while (xr.Read() && xr.NodeType == XmlNodeType.Whitespace);
				if (xr.Name == "HAFunc" && xr.NodeType == XmlNodeType.Element) this.Footer.FromXml(xr);
			}
		}

		public void FromHds(XmlTextReader xr)
		{
			this.Type = HAType.Text;
			if (xr.Name != "hds" || xr.NodeType != XmlNodeType.Element || xr.IsEmptyElement) return;

			HAFuncNode n;
			while (xr.Read())
			{
				if (xr.Name == "node" && xr.NodeType == XmlNodeType.Element)
				{
					n = new HAFuncNode();
					this.Body.Nodes.Add(n);
					n.FromHds(xr);
				}
				else if (xr.Name == "hds" && xr.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		#endregion

		#region Generation

		public void Generate(string path)
		{
			HAType t = this.Type;
			string target = path;
			if (!target.EndsWith("\\")) target += "\\";
			target += new ObjectParser(this.Text).Name;

			if (t == HAType.Comment)
			{
				return;
			}
			else if (this.IsRealFolder)
			{
				if (!new DirectoryInfo(target).Exists)
				{
					try
					{
						Directory.CreateDirectory(target);
					}
					catch
					{
						return;
					}
				}
				path = target;
				foreach (TreeNode n in this.Body.Nodes)
				{
					(n as HAFuncNode).GenerateFolder(path);
				}
			}
			else if (this.IsObject)
			{
				this.GenerateClass(target + ".cs");
			}
			else if (this.IsText)
			{
				this.GenerateText(target + ".txt");
			}

			foreach (TreeNode n in this.Nodes)
			{
				(n as HAClassNode).Generate(path);
			}
		}

		private void GenerateClass(string target)
		{
			FileStream fs;
			try
			{
				fs = new FileStream(target, FileMode.Create);
			}
			catch
			{
				return;
			}

			CodeWriter cw = new CodeWriter(fs);
			ObjectParser op = new ObjectParser(this.Text);
			cw.ClassName = op.Name;

			// Header
			if (this.Header.Comment != "")
			{
				cw.WriteCodes("// ", this.Header.Comment);
			}
			if (this.Header.Source != "")
			{
				cw.WriteBlankLine();
				cw.WriteCodes(this.Header.Source);
			}

			// Namespace
			string ns = this.Namespace;
			if (ns != "")
			{
				cw.WriteBlankLine();
				cw.WriteStartBlock("namespace " + ns);
			}

			// Body
			cw.WriteBlankLine();
			if (this.Body.Comment != "") cw.WriteCodes("/// ", this.Body.Comment);
			string classdecl = this.Type.ToString().ToLower() + " class " + op.Name;
			if (op.Type != "") classdecl += " : " + op.Type;
			cw.WriteStartBlock(classdecl);
			if (this.Body.Source != "") cw.WriteCodes(this.Body.Source);
			foreach (Object obj in this.Members)
			{
				(obj as HAMemberNode).Generate(cw);
			}
			foreach (Object obj in this.Body.Nodes)
			{
				(obj as HAFuncNode).GenerateClass(cw);
			}
			cw.WriteEndBlock();

			if (ns != "") cw.WriteEndBlock();

			// Footer
			if (this.Footer.Comment != "")
			{
				cw.WriteBlankLine();
				cw.WriteCodes("/// ", this.Footer.Comment);
			}
			if (this.Footer.Source != "")
			{
				cw.WriteBlankLine();
				cw.WriteCodes(this.Footer.Source);
			}

			cw.Close();
			fs.Close();
		}

		private void GenerateText(string target)
		{
			FileStream fs;
			try
			{
				fs = new FileStream(target, FileMode.Create);
			}
			catch
			{
				return;
			}

			StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
			sw.WriteLine(string.Format("  **** {0} ****", this.Text));
			int i = 1;
			foreach (Object obj in this.Body.Nodes)
			{
				(obj as HAFuncNode).GenerateText(sw, i.ToString(), this.Type == HAType.TextBlue);
				i++;
			}
			sw.Close();
			fs.Close();
		}

		#endregion
	}
}
