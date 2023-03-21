using Girl.Windows.Forms;
using System;
using System.Windows.Forms;

namespace Girl.HierArch
{
    /// <summary>
    /// HATree の概要の説明です。
    /// </summary>
    public class HATree : DnDTreeView
    {
        public event EventHandler Changed;
        private System.ComponentModel.IContainer components;
        public System.Windows.Forms.ImageList imageList1;
        public bool IgnoreChanged = false;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(HATree));
            imageList1 = new System.Windows.Forms.ImageList(components)
            {
                // 
                // imageList1
                // 
                ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit,
                ImageSize = new System.Drawing.Size(16, 16),
                ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList1.ImageStream"),
                TransparentColor = System.Drawing.Color.Transparent
            };
            // 
            // HATree
            // 
            ItemHeight = 14;

        }

        public HATree()
        {
            InitializeComponent();
            MakeMenu();
        }

        public virtual HATreeNode NewNode => new HATreeNode("新しいノード");

        #region Menu

        public MenuItem
            mnuChild, mnuAppend, mnuInsert, mnuDelete, mnuRename,
            mnuAccess, mnuAccessPublic, mnuAccessProtected, mnuAccessPrivate,
            mnuFolder, mnuFolderNormal, mnuFolderBlue, mnuFolderBrown,
            mnuFolderGray, mnuFolderGreen, mnuFolderRed,
            mnuText, mnuTextNormal, mnuTextBlue, mnuTextBrown,
            mnuTextGray, mnuTextGreen, mnuTextRed,
            mnuEtc, mnuEtcComment, mnuEtcSmile;

        public System.Collections.Hashtable menuType = new System.Collections.Hashtable();
        public EventHandler MenuNodeTypeHandler = null;

        private void MakeMenu()
        {
            mnuChild = new MenuItem("子を追加(&C)", new EventHandler(MenuNodeChild_Click));
            mnuAppend = new MenuItem("後に追加(&A)", new EventHandler(MenuNodeAppend_Click));
            mnuInsert = new MenuItem("前に追加(&I)", new EventHandler(MenuNodeInsert_Click));
            mnuDelete = new MenuItem("削除(&D)", new EventHandler(MenuNodeDelete_Click));
            mnuRename = new MenuItem("名前を変更(&M)", new EventHandler(MenuNodeRename_Click));

            MenuNodeTypeHandler = new EventHandler(MenuNodeType_Click);

            mnuAccess = new MenuItem("アクセス制御(&A)", new MenuItem[]
                {
                    mnuAccessPublic    = new MenuItem("&Public", MenuNodeTypeHandler),
                    mnuAccessProtected = new MenuItem("P&rotected", MenuNodeTypeHandler),
                    mnuAccessPrivate   = new MenuItem("Pr&ivate", MenuNodeTypeHandler)
                });

            mnuFolder = new MenuItem("フォルダ(&F)", new MenuItem[]
                {
                    mnuFolderNormal = new MenuItem("標準(&N)", MenuNodeTypeHandler),
                    mnuFolderRed    = new MenuItem("赤色(&R)", MenuNodeTypeHandler),
                    mnuFolderBlue   = new MenuItem("青色(&B)", MenuNodeTypeHandler),
                    mnuFolderGreen  = new MenuItem("緑色(&G)", MenuNodeTypeHandler),
                    mnuFolderBrown  = new MenuItem("茶色(&W)", MenuNodeTypeHandler),
                    mnuFolderGray   = new MenuItem("灰色(&Y)", MenuNodeTypeHandler)
                });

            mnuText = new MenuItem("テキスト(&T)", new MenuItem[]
                {
                    mnuTextNormal = new MenuItem("標準(&N)", MenuNodeTypeHandler),
                    mnuTextRed    = new MenuItem("赤色(&R)", MenuNodeTypeHandler),
                    mnuTextBlue   = new MenuItem("青色(&B)", MenuNodeTypeHandler),
                    mnuTextGreen  = new MenuItem("緑色(&G)", MenuNodeTypeHandler),
                    mnuTextBrown  = new MenuItem("茶色(&W)", MenuNodeTypeHandler),
                    mnuTextGray   = new MenuItem("灰色(&Y)", MenuNodeTypeHandler)
                });

            mnuEtc = new MenuItem("その他(&E)", new MenuItem[]
                {
                    mnuEtcComment = new MenuItem("注釈(&C)", MenuNodeTypeHandler),
                    mnuEtcSmile   = new MenuItem("人物(&H)", MenuNodeTypeHandler)
                });

            menuType.Add(mnuAccessPublic, HAType.Public);
            menuType.Add(mnuAccessProtected, HAType.Protected);
            menuType.Add(mnuAccessPrivate, HAType.Private);
            menuType.Add(mnuFolderNormal, HAType.Folder);
            menuType.Add(mnuFolderRed, HAType.FolderRed);
            menuType.Add(mnuFolderBlue, HAType.FolderBlue);
            menuType.Add(mnuFolderGreen, HAType.FolderGreen);
            menuType.Add(mnuFolderGray, HAType.FolderGray);
            menuType.Add(mnuFolderBrown, HAType.FolderBrown);
            menuType.Add(mnuTextNormal, HAType.Text);
            menuType.Add(mnuTextRed, HAType.TextRed);
            menuType.Add(mnuTextBlue, HAType.TextBlue);
            menuType.Add(mnuTextGreen, HAType.TextGreen);
            menuType.Add(mnuTextGray, HAType.TextGray);
            menuType.Add(mnuTextBrown, HAType.TextBrown);
            menuType.Add(mnuEtcComment, HAType.Comment);
            menuType.Add(mnuEtcSmile, HAType.Smile);
        }

        protected virtual void MenuNodeType_Click(object sender, System.EventArgs e)
        {
            HATreeNode n = (HATreeNode)SelectedNode;
            HAType type = (HAType)menuType[sender];
            if (n != null && n.AllowDrag && n.Type != type)
            {
                n.Type = type;
                OnChanged(this, EventArgs.Empty);
                OnNodeTypeChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnNodeTypeChanged(object sender, EventArgs e)
        {
        }

        protected virtual void MenuNodeChild_Click(object sender, System.EventArgs e)
        {
            HATreeNode n = NewNode;
            HATreeNode p = (HATreeNode)SelectedNode;
            if (p == null)
            {
                _ = Nodes.Add(n);
            }
            else
            {
                _ = p.Nodes.Add(n);
                p.SetIcon();
            }
            n.EnsureVisible();
            SelectedNode = n;
            n.BeginEdit();
            OnChanged(this, EventArgs.Empty);
        }

        protected virtual void MenuNodeAppend_Click(object sender, System.EventArgs e)
        {
            HATreeNode n = NewNode;
            HATreeNode p = (HATreeNode)SelectedNode;
            if (p == null)
            {
                _ = Nodes.Add(n);
            }
            else
            {
                TreeNodeCollection ns = (p.Parent != null) ? p.Parent.Nodes : Nodes;
                ns.Insert(p.Index + 1, n);
            }
            n.EnsureVisible();
            SelectedNode = n;
            n.BeginEdit();
            OnChanged(this, EventArgs.Empty);
        }

        protected virtual void MenuNodeInsert_Click(object sender, System.EventArgs e)
        {
            HATreeNode n = NewNode;
            HATreeNode p = (HATreeNode)SelectedNode;
            if (p == null)
            {
                Nodes.Insert(0, n);
            }
            else
            {
                TreeNodeCollection ns = (p.Parent != null) ? p.Parent.Nodes : Nodes;
                ns.Insert(p.Index, n);
                p.EnsureVisible();
            }
            n.EnsureVisible();
            SelectedNode = n;
            n.BeginEdit();
            OnChanged(this, EventArgs.Empty);
        }

        protected virtual void MenuNodeDelete_Click(object sender, System.EventArgs e)
        {
            HATreeNode n = (HATreeNode)SelectedNode;
            if (n == null)
            {
                return;
            }

            HATreeNode p = (HATreeNode)n.Parent;
            TreeNodeCollection tc = (p != null) ? p.Nodes : Nodes;
            tc.Remove(n);
            if (p != null && tc.Count < 1)
            {
                p.SetIcon();
            }

            SetState();
            OnChanged(this, EventArgs.Empty);
        }

        protected virtual void MenuNodeRename_Click(object sender, System.EventArgs e)
        {
            HATreeNode n = (HATreeNode)SelectedNode;
            if (n == null)
            {
                return;
            }

            n.BeginEdit();
        }

        #endregion

        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            bool flag = true;
            char k = e.KeyChar;
            if ('A' <= k && k <= 'Z')
            {
            }

            if (e.KeyChar == 'c' && mnuChild.Enabled)
            {
                MenuNodeChild_Click(this, EventArgs.Empty);
            }
            else if (e.KeyChar == 'a' && mnuAppend.Enabled)
            {
                MenuNodeAppend_Click(this, EventArgs.Empty);
            }
            else if (e.KeyChar == 'i' && mnuInsert.Enabled)
            {
                MenuNodeInsert_Click(this, EventArgs.Empty);
            }
            else if (e.KeyChar == 'e')
            {
                HATreeNode n = SelectedNode as HATreeNode;
                n.BeginEdit();
            }
            else
            {
                flag = false;
            }
            e.Handled = flag;
            base.OnKeyPress(e);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (ImageList != null)
            {
                int h = ImageList.ImageSize.Height;
                if (ItemHeight < h)
                {
                    ItemHeight = h;
                }
            }
        }

        protected override void OnAfterSelect(System.Windows.Forms.TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
            SetState();
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            TreeNode n = GetNodeAt(e.X, e.Y);
            if (n != null)
            {
                SelectedNode = n;
                SetState();
            }
        }

        protected override void OnAfterCollapse(System.Windows.Forms.TreeViewEventArgs e)
        {
            base.OnAfterCollapse(e);
            ((HATreeNode)e.Node).SetIcon();
        }

        protected override void OnAfterExpand(System.Windows.Forms.TreeViewEventArgs e)
        {
            base.OnAfterExpand(e);
            ((HATreeNode)e.Node).SetIcon();
        }

        protected virtual void SetState()
        {
        }

        public void StoreState()
        {
            foreach (TreeNode n in Nodes)
            {
                if (n is HATreeNode)
                {
                    ((HATreeNode)n).StoreState();
                }
            }
        }

        public void ApplyState()
        {
            foreach (TreeNode n in Nodes)
            {
                if (n is HATreeNode)
                {
                    ((HATreeNode)n).ApplyState();
                }
            }
        }

        public virtual void OnChanged(object sender, EventArgs e)
        {
            if (!IgnoreChanged && Changed != null)
            {
                IgnoreChanged = true;
                Changed(sender, e);
                IgnoreChanged = false;
            }
        }

        public virtual void OnRefreshNode(object sender, EventArgs e)
        {
        }

        protected override void DragMove()
        {
            base.DragMove();
            SetState();
            OnChanged(this, EventArgs.Empty);
        }
    }
}
