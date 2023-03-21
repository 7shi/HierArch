using System.Windows.Forms;

namespace Girl.HierArch
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            Form1.forms.Remove(this);
            bool ok = true;
            foreach (object obj in Form1.forms)
            {
                if ((obj as Form).Visible)
                {
                    ok = false;
                    break;
                }
            }
            if (ok)
            {
                Application.Exit();
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.tbFileNew = new System.Windows.Forms.ToolBarButton();
            this.tbFileOpen = new System.Windows.Forms.ToolBarButton();
            this.tbFileSave = new System.Windows.Forms.ToolBarButton();
            this.tbSeparator1 = new System.Windows.Forms.ToolBarButton();
            this.tbEditCut = new System.Windows.Forms.ToolBarButton();
            this.tbEditCopy = new System.Windows.Forms.ToolBarButton();
            this.tbEditPaste = new System.Windows.Forms.ToolBarButton();
            this.tbSeparator2 = new System.Windows.Forms.ToolBarButton();
            this.tbEditUndo = new System.Windows.Forms.ToolBarButton();
            this.tbEditRedo = new System.Windows.Forms.ToolBarButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.view1 = new Girl.HierArch.View();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.mnuFile = new System.Windows.Forms.MenuItem();
            this.mnuFileNew = new System.Windows.Forms.MenuItem();
            this.mnuFileOpen = new System.Windows.Forms.MenuItem();
            this.mnuFileSave = new System.Windows.Forms.MenuItem();
            this.mnuFileSaveAs = new System.Windows.Forms.MenuItem();
            this.mnuFileSeparator1 = new System.Windows.Forms.MenuItem();
            this.mnuFileClose = new System.Windows.Forms.MenuItem();
            this.mnuFileExit = new System.Windows.Forms.MenuItem();
            this.mnuEdit = new System.Windows.Forms.MenuItem();
            this.mnuEditUndo = new System.Windows.Forms.MenuItem();
            this.mnuEditRedo = new System.Windows.Forms.MenuItem();
            this.mnuEditSeparator1 = new System.Windows.Forms.MenuItem();
            this.mnuEditCut = new System.Windows.Forms.MenuItem();
            this.mnuEditCopy = new System.Windows.Forms.MenuItem();
            this.mnuEditPaste = new System.Windows.Forms.MenuItem();
            this.mnuEditDelete = new System.Windows.Forms.MenuItem();
            this.mnuEditSeparator2 = new System.Windows.Forms.MenuItem();
            this.mnuEditSelectAll = new System.Windows.Forms.MenuItem();
            this.mnuOption = new System.Windows.Forms.MenuItem();
            this.mnuOptionSmartEnter = new System.Windows.Forms.MenuItem();
            this.mnuOptionSmartTab = new System.Windows.Forms.MenuItem();
            this.mnuOptionSmartHome = new System.Windows.Forms.MenuItem();
            this.mnuOptionSmartParenthesis = new System.Windows.Forms.MenuItem();
            this.mnuOptionFont = new System.Windows.Forms.MenuItem();
            this.mnuOptionWordWrap = new System.Windows.Forms.MenuItem();
            this.mnuOptionSeparator1 = new System.Windows.Forms.MenuItem();
            this.mnuHelp = new System.Windows.Forms.MenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.MenuItem();
            this.cmEdit = new System.Windows.Forms.ContextMenu();
            this.cmEditUndo = new System.Windows.Forms.MenuItem();
            this.cmEditRedo = new System.Windows.Forms.MenuItem();
            this.cmEditSeparator1 = new System.Windows.Forms.MenuItem();
            this.cmEditCut = new System.Windows.Forms.MenuItem();
            this.cmEditCopy = new System.Windows.Forms.MenuItem();
            this.cmEditPaste = new System.Windows.Forms.MenuItem();
            this.cmEditDelete = new System.Windows.Forms.MenuItem();
            this.cmEditSeparator2 = new System.Windows.Forms.MenuItem();
            this.cmEditSelectAll = new System.Windows.Forms.MenuItem();
            this.opaqueSplitter1 = new Girl.Windows.Forms.OpaqueSplitter();
            this.SuspendLayout();
            // 
            // toolBar1
            // 
            this.toolBar1.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.tbFileNew,
            this.tbFileOpen,
            this.tbFileSave,
            this.tbSeparator1,
            this.tbEditCut,
            this.tbEditCopy,
            this.tbEditPaste,
            this.tbSeparator2,
            this.tbEditUndo,
            this.tbEditRedo});
            this.toolBar1.DropDownArrows = true;
            this.toolBar1.ImageList = this.imageList1;
            this.toolBar1.Location = new System.Drawing.Point(0, 0);
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ShowToolTips = true;
            this.toolBar1.Size = new System.Drawing.Size(712, 28);
            this.toolBar1.TabIndex = 0;
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // tbFileNew
            // 
            this.tbFileNew.ImageIndex = 0;
            this.tbFileNew.Name = "tbFileNew";
            this.tbFileNew.ToolTipText = "新規作成 (Ctrl+N)";
            // 
            // tbFileOpen
            // 
            this.tbFileOpen.ImageIndex = 1;
            this.tbFileOpen.Name = "tbFileOpen";
            this.tbFileOpen.ToolTipText = "開く (Ctrl+O)";
            // 
            // tbFileSave
            // 
            this.tbFileSave.ImageIndex = 2;
            this.tbFileSave.Name = "tbFileSave";
            this.tbFileSave.ToolTipText = "上書き保存 (Ctrl+S)";
            // 
            // tbSeparator1
            // 
            this.tbSeparator1.Name = "tbSeparator1";
            this.tbSeparator1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // tbEditCut
            // 
            this.tbEditCut.ImageIndex = 3;
            this.tbEditCut.Name = "tbEditCut";
            this.tbEditCut.ToolTipText = "切り取り (Ctrl+X)";
            // 
            // tbEditCopy
            // 
            this.tbEditCopy.ImageIndex = 4;
            this.tbEditCopy.Name = "tbEditCopy";
            this.tbEditCopy.ToolTipText = "コピー (Ctrl+C)";
            // 
            // tbEditPaste
            // 
            this.tbEditPaste.ImageIndex = 5;
            this.tbEditPaste.Name = "tbEditPaste";
            this.tbEditPaste.ToolTipText = "貼り付け (Ctrl+V)";
            // 
            // tbSeparator2
            // 
            this.tbSeparator2.Name = "tbSeparator2";
            this.tbSeparator2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // tbEditUndo
            // 
            this.tbEditUndo.ImageIndex = 6;
            this.tbEditUndo.Name = "tbEditUndo";
            this.tbEditUndo.ToolTipText = "元に戻す (Ctrl+Z)";
            // 
            // tbEditRedo
            // 
            this.tbEditRedo.ImageIndex = 7;
            this.tbEditRedo.Name = "tbEditRedo";
            this.tbEditRedo.ToolTipText = "やり直し (Ctrl+Y)";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            this.imageList1.Images.SetKeyName(8, "");
            this.imageList1.Images.SetKeyName(9, "");
            this.imageList1.Images.SetKeyName(10, "");
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 467);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Size = new System.Drawing.Size(712, 22);
            this.statusBar1.TabIndex = 1;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "hadoc";
            this.openFileDialog1.Filter = "HierArch 文書 (*.hadoc)|*.hadoc|HDS 文書 (*.hds)|*.hds|すべてのファイル (*.*)|*.*";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "hadoc";
            this.saveFileDialog1.FileName = "doc1";
            this.saveFileDialog1.Filter = "HierArch 文書 (*.hadoc)|*.hadoc|HDS 文書 (*.hds)|*.hds|すべてのファイル (*.*)|*.*";
            // 
            // view1
            // 
            this.view1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.view1.Location = new System.Drawing.Point(0, 28);
            this.view1.Name = "view1";
            this.view1.Size = new System.Drawing.Size(712, 436);
            this.view1.TabIndex = 2;
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuOption,
            this.mnuHelp});
            // 
            // mnuFile
            // 
            this.mnuFile.Index = 0;
            this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFileNew,
            this.mnuFileOpen,
            this.mnuFileSave,
            this.mnuFileSaveAs,
            this.mnuFileSeparator1,
            this.mnuFileClose,
            this.mnuFileExit});
            this.mnuFile.Text = "ファイル(&F)";
            // 
            // mnuFileNew
            // 
            this.mnuFileNew.Index = 0;
            this.mnuFileNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
            this.mnuFileNew.Text = "新規作成(&N)";
            this.mnuFileNew.Click += new System.EventHandler(this.mnuFileNew_Click);
            // 
            // mnuFileOpen
            // 
            this.mnuFileOpen.Index = 1;
            this.mnuFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.mnuFileOpen.Text = "開く(&O)";
            this.mnuFileOpen.Click += new System.EventHandler(this.mnuFileOpen_Click);
            // 
            // mnuFileSave
            // 
            this.mnuFileSave.Index = 2;
            this.mnuFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.mnuFileSave.Text = "上書き保存(&S)";
            this.mnuFileSave.Click += new System.EventHandler(this.mnuFileSave_Click);
            // 
            // mnuFileSaveAs
            // 
            this.mnuFileSaveAs.Index = 3;
            this.mnuFileSaveAs.Text = "名前を付けて保存(&A)";
            this.mnuFileSaveAs.Click += new System.EventHandler(this.mnuFileSaveAs_Click);
            // 
            // mnuFileSeparator1
            // 
            this.mnuFileSeparator1.Index = 4;
            this.mnuFileSeparator1.Text = "-";
            // 
            // mnuFileClose
            // 
            this.mnuFileClose.Index = 5;
            this.mnuFileClose.Shortcut = System.Windows.Forms.Shortcut.CtrlW;
            this.mnuFileClose.Text = "閉じる(&C)";
            this.mnuFileClose.Click += new System.EventHandler(this.mnuFileClose_Click);
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Index = 6;
            this.mnuFileExit.Text = "終了(&X)";
            this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
            // 
            // mnuEdit
            // 
            this.mnuEdit.Index = 1;
            this.mnuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuEditUndo,
            this.mnuEditRedo,
            this.mnuEditSeparator1,
            this.mnuEditCut,
            this.mnuEditCopy,
            this.mnuEditPaste,
            this.mnuEditDelete,
            this.mnuEditSeparator2,
            this.mnuEditSelectAll});
            this.mnuEdit.Text = "編集(&E)";
            // 
            // mnuEditUndo
            // 
            this.mnuEditUndo.Index = 0;
            this.mnuEditUndo.Text = "元に戻す(&U)";
            // 
            // mnuEditRedo
            // 
            this.mnuEditRedo.Index = 1;
            this.mnuEditRedo.Text = "やり直し(&R)";
            // 
            // mnuEditSeparator1
            // 
            this.mnuEditSeparator1.Index = 2;
            this.mnuEditSeparator1.Text = "-";
            // 
            // mnuEditCut
            // 
            this.mnuEditCut.Index = 3;
            this.mnuEditCut.Text = "切り取り(&T)";
            // 
            // mnuEditCopy
            // 
            this.mnuEditCopy.Index = 4;
            this.mnuEditCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            this.mnuEditCopy.Text = "コピー(&C)";
            // 
            // mnuEditPaste
            // 
            this.mnuEditPaste.Index = 5;
            this.mnuEditPaste.Text = "貼り付け(&P)";
            // 
            // mnuEditDelete
            // 
            this.mnuEditDelete.Index = 6;
            this.mnuEditDelete.Text = "削除(&D)";
            // 
            // mnuEditSeparator2
            // 
            this.mnuEditSeparator2.Index = 7;
            this.mnuEditSeparator2.Text = "-";
            // 
            // mnuEditSelectAll
            // 
            this.mnuEditSelectAll.Index = 8;
            this.mnuEditSelectAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
            this.mnuEditSelectAll.Text = "すべて選択(&A)";
            // 
            // mnuOption
            // 
            this.mnuOption.Index = 2;
            this.mnuOption.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuOptionSmartEnter,
            this.mnuOptionSmartTab,
            this.mnuOptionSmartHome,
            this.mnuOptionSmartParenthesis,
            this.mnuOptionSeparator1,
            this.mnuOptionWordWrap,
            this.mnuOptionFont});
            this.mnuOption.Text = "オプション(&O)";
            // 
            // mnuOptionSmartEnter
            // 
            this.mnuOptionSmartEnter.Index = 0;
            this.mnuOptionSmartEnter.Text = "スマート &Enter";
            // 
            // mnuOptionSmartTab
            // 
            this.mnuOptionSmartTab.Index = 1;
            this.mnuOptionSmartTab.Text = "スマート &Tab";
            // 
            // mnuOptionSmartHome
            // 
            this.mnuOptionSmartHome.Index = 2;
            this.mnuOptionSmartHome.Text = "スマート &Home";
            // 
            // mnuOptionSmartParenthesis
            // 
            this.mnuOptionSmartParenthesis.Index = 3;
            this.mnuOptionSmartParenthesis.Text = "自動括弧挿入(&P)";
            // 
            // mnuOptionFont
            // 
            this.mnuOptionFont.Index = 6;
            this.mnuOptionFont.Text = "フォント(&F)";
            this.mnuOptionFont.Click += new System.EventHandler(this.mnuOptionFont_Click);
            // 
            // mnuOptionWordWrap
            // 
            this.mnuOptionWordWrap.Index = 5;
            this.mnuOptionWordWrap.Text = "折り返し(&W)";
            this.mnuOptionWordWrap.Click += new System.EventHandler(this.mnuOptionWordWrap_Click);
            // 
            // mnuOptionSeparator1
            // 
            this.mnuOptionSeparator1.Index = 4;
            this.mnuOptionSeparator1.Text = "-";
            // 
            // mnuHelp
            // 
            this.mnuHelp.Index = 3;
            this.mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuHelpAbout});
            this.mnuHelp.Text = "ヘルプ(&H)";
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Index = 0;
            this.mnuHelpAbout.Text = "バージョン情報(&A)";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // cmEdit
            // 
            this.cmEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.cmEditUndo,
            this.cmEditRedo,
            this.cmEditSeparator1,
            this.cmEditCut,
            this.cmEditCopy,
            this.cmEditPaste,
            this.cmEditDelete,
            this.cmEditSeparator2,
            this.cmEditSelectAll});
            // 
            // cmEditUndo
            // 
            this.cmEditUndo.Index = 0;
            this.cmEditUndo.Text = "元に戻す(&U)";
            // 
            // cmEditRedo
            // 
            this.cmEditRedo.Index = 1;
            this.cmEditRedo.Text = "やり直し(&R)";
            // 
            // cmEditSeparator1
            // 
            this.cmEditSeparator1.Index = 2;
            this.cmEditSeparator1.Text = "-";
            // 
            // cmEditCut
            // 
            this.cmEditCut.Index = 3;
            this.cmEditCut.Text = "切り取り(&T)";
            // 
            // cmEditCopy
            // 
            this.cmEditCopy.Index = 4;
            this.cmEditCopy.Text = "コピー(&C)";
            // 
            // cmEditPaste
            // 
            this.cmEditPaste.Index = 5;
            this.cmEditPaste.Text = "貼り付け(&P)";
            // 
            // cmEditDelete
            // 
            this.cmEditDelete.Index = 6;
            this.cmEditDelete.Text = "削除(&D)";
            // 
            // cmEditSeparator2
            // 
            this.cmEditSeparator2.Index = 7;
            this.cmEditSeparator2.Text = "-";
            // 
            // cmEditSelectAll
            // 
            this.cmEditSelectAll.Index = 8;
            this.cmEditSelectAll.Text = "すべて選択(&A)";
            // 
            // opaqueSplitter1
            // 
            this.opaqueSplitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.opaqueSplitter1.Location = new System.Drawing.Point(0, 464);
            this.opaqueSplitter1.Name = "opaqueSplitter1";
            this.opaqueSplitter1.Opaque = true;
            this.opaqueSplitter1.Size = new System.Drawing.Size(712, 3);
            this.opaqueSplitter1.TabIndex = 4;
            this.opaqueSplitter1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
            this.ClientSize = new System.Drawing.Size(712, 489);
            this.Controls.Add(this.view1);
            this.Controls.Add(this.opaqueSplitter1);
            this.Controls.Add(this.toolBar1);
            this.Controls.Add(this.statusBar1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "HierArch";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ToolBar toolBar1;
        public System.Windows.Forms.StatusBar statusBar1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolBarButton tbFileNew;
        private System.Windows.Forms.ToolBarButton tbFileOpen;
        private System.Windows.Forms.ToolBarButton tbFileSave;
        private System.Windows.Forms.ToolBarButton tbSeparator1;
        private System.Windows.Forms.ToolBarButton tbEditCut;
        private System.Windows.Forms.ToolBarButton tbEditCopy;
        private System.Windows.Forms.ToolBarButton tbEditPaste;
        private System.Windows.Forms.ToolBarButton tbSeparator2;
        private System.Windows.Forms.ToolBarButton tbEditUndo;
        private System.Windows.Forms.ToolBarButton tbEditRedo;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        public Girl.HierArch.View view1;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem mnuFile;
        private System.Windows.Forms.MenuItem mnuFileNew;
        private System.Windows.Forms.MenuItem mnuFileOpen;
        private System.Windows.Forms.MenuItem mnuFileSave;
        private System.Windows.Forms.MenuItem mnuFileSaveAs;
        private System.Windows.Forms.MenuItem mnuFileSeparator1;
        private System.Windows.Forms.MenuItem mnuFileClose;
        private System.Windows.Forms.MenuItem mnuFileExit;
        private System.Windows.Forms.MenuItem mnuEdit;
        private System.Windows.Forms.MenuItem mnuEditCut;
        private System.Windows.Forms.MenuItem mnuEditCopy;
        private System.Windows.Forms.MenuItem mnuEditPaste;
        private System.Windows.Forms.MenuItem mnuHelp;
        private System.Windows.Forms.MenuItem mnuHelpAbout;
        private System.Windows.Forms.MenuItem mnuEditUndo;
        private System.Windows.Forms.MenuItem mnuEditRedo;
        private System.Windows.Forms.MenuItem mnuEditSeparator1;
        private System.Windows.Forms.MenuItem mnuEditDelete;
        private System.Windows.Forms.MenuItem mnuEditSeparator2;
        private System.Windows.Forms.MenuItem mnuEditSelectAll;
        private System.Windows.Forms.MenuItem mnuOption;
        private System.Windows.Forms.MenuItem mnuOptionSmartEnter;
        private System.Windows.Forms.MenuItem mnuOptionSmartTab;
        private System.Windows.Forms.MenuItem mnuOptionSmartHome;
        private System.Windows.Forms.MenuItem mnuOptionSmartParenthesis;
        private System.Windows.Forms.MenuItem mnuOptionSeparator1;
        public System.Windows.Forms.MenuItem mnuOptionWordWrap;
        private System.Windows.Forms.MenuItem mnuOptionFont;
        private System.Windows.Forms.ContextMenu cmEdit;
        private System.Windows.Forms.MenuItem cmEditUndo;
        private System.Windows.Forms.MenuItem cmEditRedo;
        private System.Windows.Forms.MenuItem cmEditSeparator1;
        private System.Windows.Forms.MenuItem cmEditCut;
        private System.Windows.Forms.MenuItem cmEditCopy;
        private System.Windows.Forms.MenuItem cmEditPaste;
        private System.Windows.Forms.MenuItem cmEditDelete;
        private System.Windows.Forms.MenuItem cmEditSeparator2;
        private System.Windows.Forms.MenuItem cmEditSelectAll;
        private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter1;
    }
}