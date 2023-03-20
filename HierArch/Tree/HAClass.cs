using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
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
		public HAFunc FuncTreeView;
		private HAClassNode TargetNode;
		public Font LinkFont;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAClass()
		{
			this.dataFormat = "HierArch Class Data";
			this.FuncTreeView = null;
			this.TargetNode = null;
			this.AllowDrop = true;
			this.ContextMenu = this.contextMenu1 = new ContextMenu();
			this.HideSelection = false;
			this.LabelEdit = true;
			this.ImageList = this.imageList1;
//			this.mnuAccess.Text = "クラス(&C)";
//			this.mnuFolderRed.Text = "GUI 実行ファイル(&W)";
//			this.mnuFolderBlue.Text = "CUI 実行ファイル(&E)";
//			this.mnuFolderGreen.Text = "ライブラリ(&L)";
//			this.mnuFolderBrown.Text = "モジュール(&M)";
//			this.mnuFolderGray.Text = "仮想フォルダ(&V)";
			this.contextMenu1.MenuItems.AddRange(
				new MenuItem[] {
					mnuType = new MenuItem("種類変更(&T)", new MenuItem[] { this.mnuText, this.mnuFolder, this.mnuAccess, this.mnuEtc }),
					new MenuItem("-"), this.mnuChild, this.mnuAppend, this.mnuInsert,
					new MenuItem("-"), this.mnuDelete, this.mnuRename
				});
			this.LinkFont = new Font(this.Font, FontStyle.Italic);
		}

		public override void OnChanged(object sender, EventArgs e)
		{
			if (this.IgnoreChanged) return;
			base.OnChanged(sender, e);
			if (this.TargetNode != null) this.TargetNode.LastModified = DateTime.Now;
		}

		protected override void OnNodeTypeChanged(object sender, EventArgs e)
		{
			this.StoreData();
//			this.SetView();
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
			n.Body = new HAFuncNode("Body");
			n.Body.Type = HAType.Class;
			n.Body.m_IsExpanded = true;
			n.Body.AllowDrag = false;
			n.Body.Nodes.Add(this.FuncTreeView.NewNode);
		}

		public override HATreeNode NewNode
		{
			get
			{
				HAClassNode ret = new HAClassNode("新しい項目");
				this.InitNode(ret);
				return ret;
			}
		}

		public void StoreData()
		{
			if (this.TargetNode == null || this.FuncTreeView == null) return;
			this.StoreState();
			this.FuncTreeView.StoreData();
			this.TargetNode.Body = this.FuncTreeView.Body.Clone() as HAFuncNode;
			if (this.FuncTreeView.Body.TreeView == null)
			{
				foreach (TreeNode n in this.FuncTreeView.Nodes)
				{
					this.TargetNode.Body.Nodes.Add(n.Clone() as HAFuncNode);
				}
			}
		}

		public void SetView()
		{
			Cursor curOrig = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			bool flag = this.IgnoreChanged;
			this.IgnoreChanged = true;
			this.FuncTreeView.OwnerClass = this.TargetNode;
			this.FuncTreeView.SetView(this.TargetNode);
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

		#region Drag

		public override bool IsDragValid(IDataObject data)
		{
			var ftv = this.Nodes.Count > 0 && data.GetDataPresent(this.FuncTreeView.DataFormat);
			this.enableSibling = !ftv;
			return ftv || base.IsDragValid(data);
		}

		#endregion

		#region Drop

		protected override void OnDragAccept(DragEventArgs e, DnDTreeNode n, TreeNodeCollection nc, int index)
		{
			if (e.Data.GetDataPresent(this.FuncTreeView.DataFormat))
			{
				// GetData を呼ぶと DoDragDrop の戻り値に反映する
				using (var sr = new StringReader((string)e.Data.GetData(this.FuncTreeView.DataFormat)))
				using (var xr = new XmlTextReader(sr))
				{
					if (n.IsSelected)
					{
						var nc2 = this.FuncTreeView.Nodes;
						this.FuncTreeView.FromXml(xr, nc2, nc2.Count);
					}
					else
					{
						var target = (n as HAClassNode).Body;
						bool first = true;
						while (xr.Read())
						{
							if (xr.Name == "HAFunc" && xr.NodeType == XmlNodeType.Element)
							{
								var dn = new HAFuncNode();
								target.Nodes.Add(dn);
								dn.FromXml(xr);
								if (first)
								{
									first = false;
									this.OnChanged(this, EventArgs.Empty);
								}
							}
						}
					}
				}
				return;
			}
			base.OnDragAccept(e, n, nc, index);
		}

		#endregion

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
	}
}
