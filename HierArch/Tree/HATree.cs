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
            this.components = new System.ComponentModel.Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(HATree));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // HATree
            // 
            this.ItemHeight = 14;

        }

        public HATree()
        {
            InitializeComponent();
            MakeMenu();
        }

        public virtual HATreeNode NewNode
        {
            get
            {
                return new HATreeNode("新しいノード");
            }
        }

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
            mnuChild = new MenuItem("子を追加(&C)", new EventHandler(this.MenuNodeChild_Click));
            mnuAppend = new MenuItem("後に追加(&A)", new EventHandler(this.MenuNodeAppend_Click));
            mnuInsert = new MenuItem("前に追加(&I)", new EventHandler(this.MenuNodeInsert_Click));
            mnuDelete = new MenuItem("削除(&D)", new EventHandler(this.MenuNodeDelete_Click));
            mnuRename = new MenuItem("名前を変更(&M)", new EventHandler(this.MenuNodeRename_Click));

            MenuNodeTypeHandler = new EventHandler(this.MenuNodeType_Click);

            this.mnuAccess = new MenuItem("アクセス制御(&A)", new MenuItem[]
                {
                    this.mnuAccessPublic    = new MenuItem("&Public", MenuNodeTypeHandler),
                    this.mnuAccessProtected = new MenuItem("P&rotected", MenuNodeTypeHandler),
                    this.mnuAccessPrivate   = new MenuItem("Pr&ivate", MenuNodeTypeHandler)
                });

            this.mnuFolder = new MenuItem("フォルダ(&F)", new MenuItem[]
                {
                    this.mnuFolderNormal = new MenuItem("標準(&N)", MenuNodeTypeHandler),
                    this.mnuFolderRed    = new MenuItem("赤色(&R)", MenuNodeTypeHandler),
                    this.mnuFolderBlue   = new MenuItem("青色(&B)", MenuNodeTypeHandler),
                    this.mnuFolderGreen  = new MenuItem("緑色(&G)", MenuNodeTypeHandler),
                    this.mnuFolderBrown  = new MenuItem("茶色(&W)", MenuNodeTypeHandler),
                    this.mnuFolderGray   = new MenuItem("灰色(&Y)", MenuNodeTypeHandler)
                });

            this.mnuText = new MenuItem("テキスト(&T)", new MenuItem[]
                {
                    this.mnuTextNormal = new MenuItem("標準(&N)", MenuNodeTypeHandler),
                    this.mnuTextRed    = new MenuItem("赤色(&R)", MenuNodeTypeHandler),
                    this.mnuTextBlue   = new MenuItem("青色(&B)", MenuNodeTypeHandler),
                    this.mnuTextGreen  = new MenuItem("緑色(&G)", MenuNodeTypeHandler),
                    this.mnuTextBrown  = new MenuItem("茶色(&W)", MenuNodeTypeHandler),
                    this.mnuTextGray   = new MenuItem("灰色(&Y)", MenuNodeTypeHandler)
                });

            this.mnuEtc = new MenuItem("その他(&E)", new MenuItem[]
                {
                    this.mnuEtcComment = new MenuItem("注釈(&C)", MenuNodeTypeHandler),
                    this.mnuEtcSmile   = new MenuItem("人物(&H)", MenuNodeTypeHandler)
                });

            menuType.Add(this.mnuAccessPublic, HAType.Public);
            menuType.Add(this.mnuAccessProtected, HAType.Protected);
            menuType.Add(this.mnuAccessPrivate, HAType.Private);
            menuType.Add(this.mnuFolderNormal, HAType.Folder);
            menuType.Add(this.mnuFolderRed, HAType.FolderRed);
            menuType.Add(this.mnuFolderBlue, HAType.FolderBlue);
            menuType.Add(this.mnuFolderGreen, HAType.FolderGreen);
            menuType.Add(this.mnuFolderGray, HAType.FolderGray);
            menuType.Add(this.mnuFolderBrown, HAType.FolderBrown);
            menuType.Add(this.mnuTextNormal, HAType.Text);
            menuType.Add(this.mnuTextRed, HAType.TextRed);
            menuType.Add(this.mnuTextBlue, HAType.TextBlue);
            menuType.Add(this.mnuTextGreen, HAType.TextGreen);
            menuType.Add(this.mnuTextGray, HAType.TextGray);
            menuType.Add(this.mnuTextBrown, HAType.TextBrown);
            menuType.Add(this.mnuEtcComment, HAType.Comment);
            menuType.Add(this.mnuEtcSmile, HAType.Smile);
        }

        protected virtual void MenuNodeType_Click(object sender, System.EventArgs e)
        {
            HATreeNode n = (HATreeNode)this.SelectedNode;
            HAType type = (HAType)menuType[sender];
            if (n != null && n.AllowDrag && n.Type != type)
            {
                n.Type = type;
                this.OnChanged(this, EventArgs.Empty);
                this.OnNodeTypeChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnNodeTypeChanged(object sender, EventArgs e)
        {
        }

        protected virtual void MenuNodeChild_Click(object sender, System.EventArgs e)
        {
            HATreeNode n = this.NewNode;
            HATreeNode p = (HATreeNode)this.SelectedNode;
            if (p == null)
            {
                this.Nodes.Add(n);
            }
            else
            {
                p.Nodes.Add(n);
                p.SetIcon();
            }
            n.EnsureVisible();
            this.SelectedNode = n;
            n.BeginEdit();
            this.OnChanged(this, EventArgs.Empty);
        }

        protected virtual void MenuNodeAppend_Click(object sender, System.EventArgs e)
        {
            HATreeNode n = this.NewNode;
            HATreeNode p = (HATreeNode)this.SelectedNode;
            if (p == null)
            {
                this.Nodes.Add(n);
            }
            else
            {
                TreeNodeCollection ns = (p.Parent != null) ? p.Parent.Nodes : this.Nodes;
                ns.Insert(p.Index + 1, n);
            }
            n.EnsureVisible();
            this.SelectedNode = n;
            n.BeginEdit();
            this.OnChanged(this, EventArgs.Empty);
        }

        protected virtual void MenuNodeInsert_Click(object sender, System.EventArgs e)
        {
            HATreeNode n = this.NewNode;
            HATreeNode p = (HATreeNode)this.SelectedNode;
            if (p == null)
            {
                this.Nodes.Insert(0, n);
            }
            else
            {
                TreeNodeCollection ns = (p.Parent != null) ? p.Parent.Nodes : this.Nodes;
                ns.Insert(p.Index, n);
                p.EnsureVisible();
            }
            n.EnsureVisible();
            this.SelectedNode = n;
            n.BeginEdit();
            this.OnChanged(this, EventArgs.Empty);
        }

        protected virtual void MenuNodeDelete_Click(object sender, System.EventArgs e)
        {
            HATreeNode n = (HATreeNode)this.SelectedNode;
            if (n == null) return;

            HATreeNode p = (HATreeNode)n.Parent;
            TreeNodeCollection tc = (p != null) ? p.Nodes : this.Nodes;
            tc.Remove(n);
            if (p != null && tc.Count < 1) p.SetIcon();
            this.SetState();
            this.OnChanged(this, EventArgs.Empty);
        }

        protected virtual void MenuNodeRename_Click(object sender, System.EventArgs e)
        {
            HATreeNode n = (HATreeNode)this.SelectedNode;
            if (n == null) return;

            n.BeginEdit();
        }

        #endregion

        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            bool flag = true;
            char k = e.KeyChar;
            if ('A' <= k && k <= 'Z') k += (char)32;
            if (e.KeyChar == 'c' && this.mnuChild.Enabled)
            {
                this.MenuNodeChild_Click(this, EventArgs.Empty);
            }
            else if (e.KeyChar == 'a' && this.mnuAppend.Enabled)
            {
                this.MenuNodeAppend_Click(this, EventArgs.Empty);
            }
            else if (e.KeyChar == 'i' && this.mnuInsert.Enabled)
            {
                this.MenuNodeInsert_Click(this, EventArgs.Empty);
            }
            else if (e.KeyChar == 'e')
            {
                HATreeNode n = this.SelectedNode as HATreeNode;
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
            if (this.ImageList != null)
            {
                int h = this.ImageList.ImageSize.Height;
                if (this.ItemHeight < h) this.ItemHeight = h;
            }
        }

        protected override void OnAfterSelect(System.Windows.Forms.TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
            this.SetState();
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Right) return;

            TreeNode n = this.GetNodeAt(e.X, e.Y);
            if (n != null)
            {
                this.SelectedNode = n;
                this.SetState();
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
            foreach (TreeNode n in this.Nodes)
            {
                if (n is HATreeNode) ((HATreeNode)n).StoreState();
            }
        }

        public void ApplyState()
        {
            foreach (TreeNode n in this.Nodes)
            {
                if (n is HATreeNode) ((HATreeNode)n).ApplyState();
            }
        }

        public virtual void OnChanged(object sender, EventArgs e)
        {
            if (!this.IgnoreChanged && this.Changed != null)
            {
                this.IgnoreChanged = true;
                this.Changed(sender, e);
                this.IgnoreChanged = false;
            }
        }

        public virtual void OnRefreshNode(object sender, EventArgs e)
        {
        }

        protected override void DragMove()
        {
            base.DragMove();
            this.SetState();
            this.OnChanged(this, EventArgs.Empty);
        }
    }
}
