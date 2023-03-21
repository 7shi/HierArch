using Girl.Windows.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace Girl.HierArch
{
    /// <summary>
    /// ここにクラスの説明を書きます。
    /// </summary>
    public class HAFunc : HATree
    {
        private readonly ContextMenu contextMenu1;
        private readonly MenuItem mnuType;
        public CodeEditor SourceTextBox;
        public HAClassNode OwnerClass;
        public HAFuncNode Body;
        public HAFuncNode TargetNode;
        private readonly Font textFont;

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public HAFunc()
        {
            dataFormat = "HierArch Function Data";
            SourceTextBox = null;
            OwnerClass = null;
            TargetNode = null;
            AllowDrop = true;
            ContextMenu = contextMenu1 = new ContextMenu();
            HideSelection = false;
            LabelEdit = true;
            ImageList = imageList1;
            textFont = new Font("ＭＳ ゴシック", 9);
            //			this.mnuAccess.Text = "関数(&U)";
            //			this.mnuFolderGray.Text = "仮想フォルダ(&V)";
            contextMenu1.MenuItems.AddRange(
                new MenuItem[] {
                    mnuType = new MenuItem("種類変更(&T)", new MenuItem[] { mnuText, mnuFolder, mnuAccess, mnuEtc }),
                    new MenuItem("-"), mnuChild, mnuAppend, mnuInsert,
                    new MenuItem("-"), mnuDelete, mnuRename
                });
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

        public override void OnRefreshNode(object sender, EventArgs e)
        {
            base.OnRefreshNode(sender, e);
            if (sender != TargetNode)
            {
                return;
            }

            SetView();
        }

        protected override void MenuNodeChild_Click(object sender, EventArgs e)
        {
            HATreeNode p = (HATreeNode)SelectedNode;
            HATreeNode n = NewNode;
            if (p != null)
            {
                _ = p.Nodes.Add(n);
                p.SetIcon();
            }
            else
            {
                _ = Nodes.Add(n);
            }
            n.EnsureVisible();
            SelectedNode = n;
            n.BeginEdit();
            OnChanged(this, EventArgs.Empty);
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
            mnuType.Enabled = mnuAppend.Enabled = mnuInsert.Enabled = mnuDelete.Enabled = mnuRename.Enabled = SelectedNode is HAFuncNode n && n.AllowDrag;
        }

        public override HATreeNode NewNode => new HAFuncNode("新しい項目");

        public void StoreData()
        {
            if (TargetNode == null)
            {
                return;
            }

            StoreState();
            if (SourceTextBox != null)
            {
                TargetNode.Source = SourceTextBox.Code;
                TargetNode.SourceSelectionStart = SourceTextBox.SelectionStart;
                TargetNode.SourceSelectionLength = SourceTextBox.SelectionLength;
            }
        }

        public void SetView()
        {
            bool flag = IgnoreChanged;
            IgnoreChanged = true;
            if (TargetNode != null)
            {
                if (SourceTextBox != null)
                {
                    SourceTextBox.Enabled = true;
                    SourceTextBox.Clear();
                    SourceTextBox.Code = TargetNode.Source;
                    SourceTextBox.SelectionStart = TargetNode.SourceSelectionStart;
                    SourceTextBox.SelectionLength = TargetNode.SourceSelectionLength;
                }
            }
            else
            {
                if (SourceTextBox != null)
                {
                    SourceTextBox.Enabled = false;
                    SourceTextBox.Clear();
                    SourceTextBox.SelectionStart = 0;
                    SourceTextBox.SelectionLength = 0;
                }
            }
            IgnoreChanged = flag;
        }

        public void SetView(HAClassNode cls)
        {
            bool flag = IgnoreChanged;
            IgnoreChanged = true;
            SelectedNode = null;
            TargetNode = null;
            SetView();
            Nodes.Clear();
            if (cls != null)
            {
                Enabled = true;
                BackColor = System.Drawing.SystemColors.Window;
                Body = cls.Body.Clone() as HAFuncNode;
                if (OwnerClass.IsObject)
                {
                    BeginUpdate();
                    _ = Nodes.Add(Body);
                    ApplyState();
                    EndUpdate();
                }
                else if (Body.Nodes.Count > 0)
                {
                    BeginUpdate();
                    foreach (TreeNode n in Body.Nodes)
                    {
                        _ = Nodes.Add(n.Clone() as HAFuncNode);
                    }
                    Body.Nodes.Clear();
                    ApplyState();
                    EndUpdate();
                }
                if (SelectedNode == null && Nodes.Count > 0)
                {
                    SelectedNode = Nodes[0];
                }
                if (SelectedNode != null)
                {
                    SelectedNode.EnsureVisible();
                    TargetNode = SelectedNode as HAFuncNode;
                    SetView();
                }
            }
            else
            {
                Enabled = false;
                BackColor = System.Drawing.SystemColors.ControlLight;
            }
            SetState();
            IgnoreChanged = flag;
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

            TargetNode = (HAFuncNode)e.Node;
            IgnoreChanged = true;
            SetView();
            IgnoreChanged = false;
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
                        OnChanged(this, new EventArgs());
                    }
                }
            }
        }

        #endregion
    }
}
