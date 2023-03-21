using Girl.Windows.Forms;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Girl.HierArch
{
    /// <summary>
    /// ここにクラスの説明を書きます。
    /// </summary>
    public class HAClass : HATree
    {
        private readonly ContextMenu contextMenu1;
        private readonly MenuItem mnuType;
        public HAFunc FuncTreeView;
        private HAClassNode TargetNode;
        public Font LinkFont;

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public HAClass()
        {
            dataFormat = "HierArch Class Data";
            FuncTreeView = null;
            TargetNode = null;
            AllowDrop = true;
            ContextMenu = contextMenu1 = new ContextMenu();
            HideSelection = false;
            LabelEdit = true;
            ImageList = imageList1;
            //			this.mnuAccess.Text = "クラス(&C)";
            //			this.mnuFolderRed.Text = "GUI 実行ファイル(&W)";
            //			this.mnuFolderBlue.Text = "CUI 実行ファイル(&E)";
            //			this.mnuFolderGreen.Text = "ライブラリ(&L)";
            //			this.mnuFolderBrown.Text = "モジュール(&M)";
            //			this.mnuFolderGray.Text = "仮想フォルダ(&V)";
            contextMenu1.MenuItems.AddRange(
                new MenuItem[] {
                    mnuType = new MenuItem("種類変更(&T)", new MenuItem[] { mnuText, mnuFolder, mnuAccess, mnuEtc }),
                    new MenuItem("-"), mnuChild, mnuAppend, mnuInsert,
                    new MenuItem("-"), mnuDelete, mnuRename
                });
            LinkFont = new Font(Font, FontStyle.Italic);
        }

        public override void OnChanged(object sender, EventArgs e)
        {
            if (IgnoreChanged)
            {
                return;
            }

            base.OnChanged(sender, e);
            if (TargetNode != null)
            {
                TargetNode.LastModified = DateTime.Now;
            }
        }

        protected override void OnNodeTypeChanged(object sender, EventArgs e)
        {
            StoreData();
            //			this.SetView();
        }

        public override void OnRefreshNode(object sender, EventArgs e)
        {
            base.OnRefreshNode(sender, e);
            if (sender != TargetNode)
            {
                return;
            }

            SetView();
        }

        protected override void StartDrag()
        {
            _ = Focus();
            StoreData();
            base.StartDrag();
        }

        protected override void SetState()
        {
            if (Nodes.Count < 1 && TargetNode != null)
            {
                TargetNode = null;
                SetView();
            }
            mnuType.Enabled = mnuDelete.Enabled = mnuRename.Enabled = SelectedNode is HAClassNode n && n.AllowDrag;
        }

        public void InitNode(HAClassNode n)
        {
            n.Body = new HAFuncNode("Body")
            {
                Type = HAType.Class,
                m_IsExpanded = true,
                AllowDrag = false
            };
            _ = n.Body.Nodes.Add(FuncTreeView.NewNode);
        }

        public override HATreeNode NewNode
        {
            get
            {
                HAClassNode ret = new HAClassNode("新しい項目");
                InitNode(ret);
                return ret;
            }
        }

        public void StoreData()
        {
            if (TargetNode == null || FuncTreeView == null)
            {
                return;
            }

            StoreState();
            FuncTreeView.StoreData();
            TargetNode.Body = FuncTreeView.Body.Clone() as HAFuncNode;
            if (FuncTreeView.Body.TreeView == null)
            {
                foreach (TreeNode n in FuncTreeView.Nodes)
                {
                    _ = TargetNode.Body.Nodes.Add(n.Clone() as HAFuncNode);
                }
            }
        }

        public void SetView()
        {
            Cursor curOrig = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            bool flag = IgnoreChanged;
            IgnoreChanged = true;
            FuncTreeView.OwnerClass = TargetNode;
            FuncTreeView.SetView(TargetNode);
            SetState();
            IgnoreChanged = flag;
            Cursor.Current = curOrig;
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
            if (IgnoreChanged)
            {
                return;
            }

            StoreData();
            if (TargetNode == e.Node)
            {
                return;
            }

            TargetNode = (HAClassNode)e.Node;
            SetView();
        }

        #region Drag

        public override bool IsDragValid(IDataObject data)
        {
            bool ftv = Nodes.Count > 0 && data.GetDataPresent(FuncTreeView.DataFormat);
            enableSibling = !ftv;
            return ftv || base.IsDragValid(data);
        }

        #endregion

        #region Drop

        protected override void OnDragAccept(DragEventArgs e, DnDTreeNode n, TreeNodeCollection nc, int index)
        {
            if (e.Data.GetDataPresent(FuncTreeView.DataFormat))
            {
                // GetData を呼ぶと DoDragDrop の戻り値に反映する
                using (StringReader sr = new StringReader((string)e.Data.GetData(FuncTreeView.DataFormat)))
                using (XmlTextReader xr = new XmlTextReader(sr))
                {
                    if (n.IsSelected)
                    {
                        TreeNodeCollection nc2 = FuncTreeView.Nodes;
                        FuncTreeView.FromXml(xr, nc2, nc2.Count);
                    }
                    else
                    {
                        HAFuncNode target = (n as HAClassNode).Body;
                        bool first = true;
                        while (xr.Read())
                        {
                            if (xr.Name == "HAFunc" && xr.NodeType == XmlNodeType.Element)
                            {
                                HAFuncNode dn = new HAFuncNode();
                                _ = target.Nodes.Add(dn);
                                dn.FromXml(xr);
                                if (first)
                                {
                                    first = false;
                                    OnChanged(this, EventArgs.Empty);
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
                        OnChanged(this, new EventArgs());
                    }
                }
            }
        }

        #endregion
    }
}
