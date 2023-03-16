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
	public class HAClass : HATree
	{
		private ContextMenu contextMenu1;
		private MenuItem mnuType;
		public HAMember MemberTreeView;
		public HAFunc FuncTreeView;
		private HAClassNode TargetNode;
		public Font LinkFont;
		public PropertyGrid Property;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAClass()
		{
			this.dataFormat = "HierArch Class Data";
			this.MemberTreeView = null;
			this.FuncTreeView = null;
			this.TargetNode = null;
			this.Property = null;
			this.AllowDrop = true;
			this.ContextMenu = this.contextMenu1 = new ContextMenu();
			this.HideSelection = false;
			this.LabelEdit = true;
			this.ImageList = this.imageList1;
			this.mnuAccess.Text = "クラス(&C)";
			this.mnuFolderRed.Text = "GUI 実行ファイル(&W)";
			this.mnuFolderBlue.Text = "CUI 実行ファイル(&E)";
			this.mnuFolderGreen.Text = "ライブラリ(&L)";
			this.mnuFolderBrown.Text = "モジュール(&M)";
			this.mnuFolderGray.Text = "仮想フォルダ(&V)";
			this.contextMenu1.MenuItems.AddRange(new MenuItem[] { mnuType = new MenuItem("種類変更(&T)", new MenuItem[] { this.mnuAccess, this.mnuFolder, this.mnuText, this.mnuEtc }), new MenuItem("-"), this.mnuChild, this.mnuAppend, this.mnuInsert, new MenuItem("-"), this.mnuDelete, this.mnuRename, });
			this.LinkFont = new Font(this.Font, FontStyle.Italic);
		}

		public override void OnChanged(object sender, EventArgs e)
		{
			if (this.IgnoreChanged) return;
			base.OnChanged(sender, e);
			if (this.TargetNode != null) this.TargetNode.LastModified = DateTime.Now;
			if (this.Property != null) this.Property.Refresh();
		}

		protected override void OnNodeTypeChanged(object sender, EventArgs e)
		{
			this.StoreData();
			this.SetView();
		}

		public override void OnRefreshNode(object sender, EventArgs e)
		{
			base.OnRefreshNode(sender, e);
			if (sender != this.TargetNode) return;
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
			mnuType.Enabled = mnuDelete.Enabled = mnuRename.Enabled =(n != null && n.AllowDrag);
		}

		public void InitNode(HAClassNode n)
		{
			n.Header = new HAFuncNode("ヘッダ");
			n.Header.Type = HAType.Text;
			n.Header.m_IsSelected = true;
			n.Header.AllowDrag = false;
			n.Header.Comment = "ここにソースコードの注釈を書きます。\r\n";
			n.Header.Source = "using System;\r\n";
			n.Body = new HAFuncNode("本体");
			n.Body.Type = HAType.Class;
			n.Body.m_IsExpanded = true;
			n.Body.AllowDrag = false;
			n.Body.Comment = "<summary>\r\nここにクラスの説明を書きます。\r\n</summary>\r\n";
			HAFuncNode cst = new HAFuncNode("__" + "CLASS");
			cst.Comment = "<summary>\r\nコンストラクタです。\r\n</summary>\r\n";
			n.Body.Nodes.Add(cst);
			HAFuncNode dst = new HAFuncNode("~__" + "CLASS");
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

		public void StoreData()
		{
			if (this.TargetNode == null || this.FuncTreeView == null) return;
			this.StoreState();
			this.FuncTreeView.StoreData();
			this.MemberTreeView.StoreState();
			this.TargetNode.Members.Clear();
			foreach (TreeNode n in this.MemberTreeView.Nodes)
			{
				if (n is HAMemberNode) this.TargetNode.Members.Add(n.Clone());
			}
			this.TargetNode.Header = this.FuncTreeView.Header.Clone() as HAFuncNode;
			this.TargetNode.Body = this.FuncTreeView.Body.Clone() as HAFuncNode;
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
			bool flag = this.IgnoreChanged;
			this.IgnoreChanged = true;
			if (this.TargetNode != null)
			{
				if (this.MemberTreeView != null) this.MemberTreeView.SetView(this.TargetNode.Members);
				if (this.FuncTreeView != null)
				{
					this.FuncTreeView.OwnerClass = this.TargetNode;
					this.FuncTreeView.SetView(this.TargetNode);
				}
				if (this.Property != null)
				{
					this.Property.SelectedObject = this.TargetNode.Property;
				}
			}
			else
			{
				if (this.MemberTreeView != null) this.MemberTreeView.SetView(null);
				if (this.FuncTreeView != null)
				{
					this.FuncTreeView.OwnerClass = null;
					this.FuncTreeView.SetView(null);
				}
				if (this.Property != null)
				{
					this.Property.SelectedObject = null;
				}
			}
			this.SetState();
			this.IgnoreChanged = flag;
			Cursor.Current = curOrig;
		}

		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			base.OnAfterSelect(e);
			if (this.IgnoreChanged) return;
			this.StoreData();
			if (this.TargetNode == e.Node) return;
			this.TargetNode =(HAClassNode) e.Node;
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
}
