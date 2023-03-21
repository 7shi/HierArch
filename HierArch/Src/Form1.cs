using Girl.Windows.Forms;
using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace Girl.HierArch
{
    /// <summary>
    /// SDI アプリケーションの雛型です。
    /// </summary>
    public class Form1 : System.Windows.Forms.Form
    {
        public WindowSizeMonitor sizeMonitor;
        private static ViewData viewData;
        private static readonly ArrayList forms = new ArrayList();

        private readonly HADoc document = new HADoc();
        private readonly Hashtable m_tblView = new Hashtable();
        private readonly string m_sCaption;
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
        private System.ComponentModel.IContainer components;
        private readonly EditManager editManager = new EditManager();
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
        private static readonly CodeEditorManager codeEditorManager = new CodeEditorManager(2);

        public Form1()
        {
            //
            // Windows フォーム デザイナ サポートに必要です。
            //
            InitializeComponent();

            //
            // TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
            //
            _ = Form1.forms.Add(this);
            _ = new ViewManager(this, Form1.viewData);

            m_sCaption = Text;
            SetCaption();
            SetDocument();
            view1.Changed += new System.EventHandler(view1_Changed);

            // ツールバーのボタンをメニューの項目に対応させます。
            // ここで定義した情報は toolBar1_ButtonClick() で使用されます。
            tbFileNew.Tag = mnuFileNew;
            tbFileOpen.Tag = mnuFileOpen;
            tbFileSave.Tag = mnuFileSave;

            // テキストボックスの状態をメニューと連動させます。
            editManager.AddControl(view1.txtSource);
            editManager.SetCommand(EditAction.Undo, mnuEditUndo, cmEditUndo, tbEditUndo);
            editManager.SetCommand(EditAction.Redo, mnuEditRedo, cmEditRedo, tbEditRedo);
            editManager.SetCommand(EditAction.Cut, mnuEditCut, cmEditCut, tbEditCut);
            editManager.SetCommand(EditAction.Copy, mnuEditCopy, cmEditCopy, tbEditCopy);
            editManager.SetCommand(EditAction.Paste, mnuEditPaste, cmEditPaste, tbEditPaste);
            editManager.SetCommand(EditAction.Delete, mnuEditDelete, cmEditDelete);
            editManager.SetCommand(EditAction.SelectAll, mnuEditSelectAll, cmEditSelectAll);

            // エディタオプションを設定します。
            Form1.codeEditorManager.SetTarget(view1.txtSource);
            Form1.codeEditorManager.SetCommand(CodeEditorOption.SmartEnter, mnuOptionSmartEnter);
            Form1.codeEditorManager.SetCommand(CodeEditorOption.SmartTab, mnuOptionSmartTab);
            Form1.codeEditorManager.SetCommand(CodeEditorOption.SmartHome, mnuOptionSmartHome);
            Form1.codeEditorManager.SetCommand(CodeEditorOption.SmartParenthesis, mnuOptionSmartParenthesis);

            document.ClassTreeView = view1.tvClass;
            view1.txtSource.ContextMenu = cmEdit;

            sizeMonitor = new WindowSizeMonitor(this);
        }

        /// <summary>
        /// 使用されているリソースに後処理を実行します。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
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
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
            toolBar1 = new System.Windows.Forms.ToolBar();
            tbFileNew = new System.Windows.Forms.ToolBarButton();
            tbFileOpen = new System.Windows.Forms.ToolBarButton();
            tbFileSave = new System.Windows.Forms.ToolBarButton();
            tbSeparator1 = new System.Windows.Forms.ToolBarButton();
            tbEditCut = new System.Windows.Forms.ToolBarButton();
            tbEditCopy = new System.Windows.Forms.ToolBarButton();
            tbEditPaste = new System.Windows.Forms.ToolBarButton();
            tbSeparator2 = new System.Windows.Forms.ToolBarButton();
            tbEditUndo = new System.Windows.Forms.ToolBarButton();
            tbEditRedo = new System.Windows.Forms.ToolBarButton();
            imageList1 = new System.Windows.Forms.ImageList(components);
            statusBar1 = new System.Windows.Forms.StatusBar();
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            view1 = new Girl.HierArch.View();
            mainMenu1 = new System.Windows.Forms.MainMenu();
            mnuFile = new System.Windows.Forms.MenuItem();
            mnuFileNew = new System.Windows.Forms.MenuItem();
            mnuFileOpen = new System.Windows.Forms.MenuItem();
            mnuFileSave = new System.Windows.Forms.MenuItem();
            mnuFileSaveAs = new System.Windows.Forms.MenuItem();
            mnuFileSeparator1 = new System.Windows.Forms.MenuItem();
            mnuFileClose = new System.Windows.Forms.MenuItem();
            mnuFileExit = new System.Windows.Forms.MenuItem();
            mnuEdit = new System.Windows.Forms.MenuItem();
            mnuEditUndo = new System.Windows.Forms.MenuItem();
            mnuEditRedo = new System.Windows.Forms.MenuItem();
            mnuEditSeparator1 = new System.Windows.Forms.MenuItem();
            mnuEditCut = new System.Windows.Forms.MenuItem();
            mnuEditCopy = new System.Windows.Forms.MenuItem();
            mnuEditPaste = new System.Windows.Forms.MenuItem();
            mnuEditDelete = new System.Windows.Forms.MenuItem();
            mnuEditSeparator2 = new System.Windows.Forms.MenuItem();
            mnuEditSelectAll = new System.Windows.Forms.MenuItem();
            mnuOption = new System.Windows.Forms.MenuItem();
            mnuOptionSmartEnter = new System.Windows.Forms.MenuItem();
            mnuOptionSmartTab = new System.Windows.Forms.MenuItem();
            mnuOptionSmartHome = new System.Windows.Forms.MenuItem();
            mnuOptionSmartParenthesis = new System.Windows.Forms.MenuItem();
            mnuOptionSeparator1 = new System.Windows.Forms.MenuItem();
            mnuOptionWordWrap = new System.Windows.Forms.MenuItem();
            mnuOptionFont = new System.Windows.Forms.MenuItem();
            mnuHelp = new System.Windows.Forms.MenuItem();
            mnuHelpAbout = new System.Windows.Forms.MenuItem();
            cmEdit = new System.Windows.Forms.ContextMenu();
            cmEditUndo = new System.Windows.Forms.MenuItem();
            cmEditRedo = new System.Windows.Forms.MenuItem();
            cmEditSeparator1 = new System.Windows.Forms.MenuItem();
            cmEditCut = new System.Windows.Forms.MenuItem();
            cmEditCopy = new System.Windows.Forms.MenuItem();
            cmEditPaste = new System.Windows.Forms.MenuItem();
            cmEditDelete = new System.Windows.Forms.MenuItem();
            cmEditSeparator2 = new System.Windows.Forms.MenuItem();
            cmEditSelectAll = new System.Windows.Forms.MenuItem();
            opaqueSplitter1 = new Girl.Windows.Forms.OpaqueSplitter();
            SuspendLayout();
            // 
            // toolBar1
            // 
            toolBar1.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
                                                                                        tbFileNew,
                                                                                        tbFileOpen,
                                                                                        tbFileSave,
                                                                                        tbSeparator1,
                                                                                        tbEditCut,
                                                                                        tbEditCopy,
                                                                                        tbEditPaste,
                                                                                        tbSeparator2,
                                                                                        tbEditUndo,
                                                                                        tbEditRedo});
            toolBar1.DropDownArrows = true;
            toolBar1.ImageList = imageList1;
            toolBar1.Name = "toolBar1";
            toolBar1.ShowToolTips = true;
            toolBar1.Size = new System.Drawing.Size(712, 25);
            toolBar1.TabIndex = 0;
            toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(toolBar1_ButtonClick);
            // 
            // tbFileNew
            // 
            tbFileNew.ImageIndex = 0;
            tbFileNew.ToolTipText = "新規作成 (Ctrl+N)";
            // 
            // tbFileOpen
            // 
            tbFileOpen.ImageIndex = 1;
            tbFileOpen.ToolTipText = "開く (Ctrl+O)";
            // 
            // tbFileSave
            // 
            tbFileSave.ImageIndex = 2;
            tbFileSave.ToolTipText = "上書き保存 (Ctrl+S)";
            // 
            // tbSeparator1
            // 
            tbSeparator1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // tbEditCut
            // 
            tbEditCut.ImageIndex = 3;
            tbEditCut.ToolTipText = "切り取り (Ctrl+X)";
            // 
            // tbEditCopy
            // 
            tbEditCopy.ImageIndex = 4;
            tbEditCopy.ToolTipText = "コピー (Ctrl+C)";
            // 
            // tbEditPaste
            // 
            tbEditPaste.ImageIndex = 5;
            tbEditPaste.ToolTipText = "貼り付け (Ctrl+V)";
            // 
            // tbSeparator2
            // 
            tbSeparator2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // tbEditUndo
            // 
            tbEditUndo.ImageIndex = 6;
            tbEditUndo.ToolTipText = "元に戻す (Ctrl+Z)";
            // 
            // tbEditRedo
            // 
            tbEditRedo.ImageIndex = 7;
            tbEditRedo.ToolTipText = "やり直し (Ctrl+Y)";
            // 
            // imageList1
            // 
            imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            imageList1.ImageSize = new System.Drawing.Size(16, 16);
            imageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // statusBar1
            // 
            statusBar1.Location = new System.Drawing.Point(0, 467);
            statusBar1.Name = "statusBar1";
            statusBar1.Size = new System.Drawing.Size(712, 22);
            statusBar1.TabIndex = 1;
            // 
            // openFileDialog1
            // 
            openFileDialog1.DefaultExt = "hadoc";
            openFileDialog1.Filter = "HierArch 文書 (*.hadoc)|*.hadoc|HDS 文書 (*.hds)|*.hds|すべてのファイル (*.*)|*.*";
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.DefaultExt = "hadoc";
            saveFileDialog1.FileName = "doc1";
            saveFileDialog1.Filter = "HierArch 文書 (*.hadoc)|*.hadoc|HDS 文書 (*.hds)|*.hds|すべてのファイル (*.*)|*.*";
            // 
            // view1
            // 
            view1.Dock = System.Windows.Forms.DockStyle.Fill;
            view1.Location = new System.Drawing.Point(0, 25);
            view1.Name = "view1";
            view1.Size = new System.Drawing.Size(712, 311);
            view1.TabIndex = 2;
            // 
            // mainMenu1
            // 
            mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      mnuFile,
                                                                                      mnuEdit,
                                                                                      mnuOption,
                                                                                      mnuHelp});
            // 
            // mnuFile
            // 
            mnuFile.Index = 0;
            mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                    mnuFileNew,
                                                                                    mnuFileOpen,
                                                                                    mnuFileSave,
                                                                                    mnuFileSaveAs,
                                                                                    mnuFileSeparator1,
                                                                                    mnuFileClose,
                                                                                    mnuFileExit});
            mnuFile.Text = "ファイル(&F)";
            // 
            // mnuFileNew
            // 
            mnuFileNew.Index = 0;
            mnuFileNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
            mnuFileNew.Text = "新規作成(&N)";
            mnuFileNew.Click += new System.EventHandler(mnuFileNew_Click);
            // 
            // mnuFileOpen
            // 
            mnuFileOpen.Index = 1;
            mnuFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            mnuFileOpen.Text = "開く(&O)";
            mnuFileOpen.Click += new System.EventHandler(mnuFileOpen_Click);
            // 
            // mnuFileSave
            // 
            mnuFileSave.Index = 2;
            mnuFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            mnuFileSave.Text = "上書き保存(&S)";
            mnuFileSave.Click += new System.EventHandler(mnuFileSave_Click);
            // 
            // mnuFileSaveAs
            // 
            mnuFileSaveAs.Index = 3;
            mnuFileSaveAs.Text = "名前を付けて保存(&A)";
            mnuFileSaveAs.Click += new System.EventHandler(mnuFileSaveAs_Click);
            // 
            // mnuFileSeparator1
            // 
            mnuFileSeparator1.Index = 4;
            mnuFileSeparator1.Text = "-";
            // 
            // mnuFileClose
            // 
            mnuFileClose.Index = 5;
            mnuFileClose.Shortcut = System.Windows.Forms.Shortcut.CtrlW;
            mnuFileClose.Text = "閉じる(&C)";
            mnuFileClose.Click += new System.EventHandler(mnuFileClose_Click);
            // 
            // mnuFileExit
            // 
            mnuFileExit.Index = 6;
            mnuFileExit.Text = "終了(&X)";
            mnuFileExit.Click += new System.EventHandler(mnuFileExit_Click);
            // 
            // mnuEdit
            // 
            mnuEdit.Index = 1;
            mnuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                    mnuEditUndo,
                                                                                    mnuEditRedo,
                                                                                    mnuEditSeparator1,
                                                                                    mnuEditCut,
                                                                                    mnuEditCopy,
                                                                                    mnuEditPaste,
                                                                                    mnuEditDelete,
                                                                                    mnuEditSeparator2,
                                                                                    mnuEditSelectAll});
            mnuEdit.Text = "編集(&E)";
            // 
            // mnuEditUndo
            // 
            mnuEditUndo.Index = 0;
            mnuEditUndo.Text = "元に戻す(&U)";
            // 
            // mnuEditRedo
            // 
            mnuEditRedo.Index = 1;
            mnuEditRedo.Text = "やり直し(&R)";
            // 
            // mnuEditSeparator1
            // 
            mnuEditSeparator1.Index = 2;
            mnuEditSeparator1.Text = "-";
            // 
            // mnuEditCut
            // 
            mnuEditCut.Index = 3;
            mnuEditCut.Text = "切り取り(&T)";
            // 
            // mnuEditCopy
            // 
            mnuEditCopy.Index = 4;
            mnuEditCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            mnuEditCopy.Text = "コピー(&C)";
            // 
            // mnuEditPaste
            // 
            mnuEditPaste.Index = 5;
            mnuEditPaste.Text = "貼り付け(&P)";
            // 
            // mnuEditDelete
            // 
            mnuEditDelete.Index = 6;
            mnuEditDelete.Text = "削除(&D)";
            // 
            // mnuEditSeparator2
            // 
            mnuEditSeparator2.Index = 7;
            mnuEditSeparator2.Text = "-";
            // 
            // mnuEditSelectAll
            // 
            mnuEditSelectAll.Index = 8;
            mnuEditSelectAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
            mnuEditSelectAll.Text = "すべて選択(&A)";
            // 
            // menuItem1
            // 
            mnuOption.Index = 2;
            mnuOption.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      mnuOptionSmartEnter,
                                                                                      mnuOptionSmartTab,
                                                                                      mnuOptionSmartHome,
                                                                                      mnuOptionSmartParenthesis,
                                                                                      mnuOptionFont,
                                                                                      mnuOptionWordWrap,
                                                                                      mnuOptionSeparator1});
            mnuOption.Text = "オプション(&O)";
            // 
            // mnuOptionSmartEnter
            // 
            mnuOptionSmartEnter.Index = 0;
            mnuOptionSmartEnter.Text = "スマート &Enter";
            // 
            // mnuOptionSmartTab
            // 
            mnuOptionSmartTab.Index = 1;
            mnuOptionSmartTab.Text = "スマート &Tab";
            // 
            // mnuOptionSmartHome
            // 
            mnuOptionSmartHome.Index = 2;
            mnuOptionSmartHome.Text = "スマート &Home";
            // 
            // mnuOptionSmartParenthesis
            // 
            mnuOptionSmartParenthesis.Index = 3;
            mnuOptionSmartParenthesis.Text = "自動括弧挿入(&P)";
            // 
            // mnuOptionSeparator1
            // 
            mnuOptionSeparator1.Index = 4;
            mnuOptionSeparator1.Text = "-";
            // 
            // mnuOptionWordWrap
            // 
            mnuOptionWordWrap.Index = 5;
            mnuOptionWordWrap.Text = "折り返し(&W)";
            mnuOptionWordWrap.Click += new System.EventHandler(mnuOptionWordWrap_Click);
            // 
            // mnuOptionFont
            // 
            mnuOptionFont.Index = 6;
            mnuOptionFont.Text = "フォント(&F)";
            mnuOptionFont.Click += new System.EventHandler(mnuOptionFont_Click);
            // 
            // mnuHelp
            // 
            mnuHelp.Index = 3;
            mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                    mnuHelpAbout});
            mnuHelp.Text = "ヘルプ(&H)";
            // 
            // mnuHelpAbout
            // 
            mnuHelpAbout.Index = 0;
            mnuHelpAbout.Text = "バージョン情報(&A)";
            mnuHelpAbout.Click += new System.EventHandler(mnuHelpAbout_Click);
            // 
            // cmEdit
            // 
            cmEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                   cmEditUndo,
                                                                                   cmEditRedo,
                                                                                   cmEditSeparator1,
                                                                                   cmEditCut,
                                                                                   cmEditCopy,
                                                                                   cmEditPaste,
                                                                                   cmEditDelete,
                                                                                   cmEditSeparator2,
                                                                                   cmEditSelectAll});
            // 
            // cmEditUndo
            // 
            cmEditUndo.Index = 0;
            cmEditUndo.Text = "元に戻す(&U)";
            // 
            // cmEditRedo
            // 
            cmEditRedo.Index = 1;
            cmEditRedo.Text = "やり直し(&R)";
            // 
            // cmEditSeparator1
            // 
            cmEditSeparator1.Index = 2;
            cmEditSeparator1.Text = "-";
            // 
            // cmEditCut
            // 
            cmEditCut.Index = 3;
            cmEditCut.Text = "切り取り(&T)";
            // 
            // cmEditCopy
            // 
            cmEditCopy.Index = 4;
            cmEditCopy.Text = "コピー(&C)";
            // 
            // cmEditPaste
            // 
            cmEditPaste.Index = 5;
            cmEditPaste.Text = "貼り付け(&P)";
            // 
            // cmEditDelete
            // 
            cmEditDelete.Index = 6;
            cmEditDelete.Text = "削除(&D)";
            // 
            // cmEditSeparator2
            // 
            cmEditSeparator2.Index = 7;
            cmEditSeparator2.Text = "-";
            // 
            // cmEditSelectAll
            // 
            cmEditSelectAll.Index = 8;
            cmEditSelectAll.Text = "すべて選択(&A)";
            // 
            // opaqueSplitter1
            // 
            opaqueSplitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            opaqueSplitter1.Location = new System.Drawing.Point(0, 336);
            opaqueSplitter1.Name = "opaqueSplitter1";
            opaqueSplitter1.Opaque = true;
            opaqueSplitter1.Size = new System.Drawing.Size(712, 3);
            opaqueSplitter1.TabIndex = 4;
            opaqueSplitter1.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleBaseSize = new System.Drawing.Size(5, 12);
            ClientSize = new System.Drawing.Size(712, 489);
            Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          view1,
                                                                          opaqueSplitter1,
                                                                          toolBar1,
                                                                          statusBar1});
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Menu = mainMenu1;
            Name = "Form1";
            StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultLocation;
            Text = "HierArch";
            Closing += new System.ComponentModel.CancelEventHandler(Form1_Closing);
            ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            ApplicationDataManager adm = new ApplicationDataManager();
            Form1.viewData = ViewData.Load(adm);

            Form1 f;
            if (args.GetLength(0) < 1)
            {
                f = new Form1();
                f.Show();
            }
            else
            {
                foreach (string fn in args)
                {
                    f = new Form1();
                    f.CreateHandle();
                    _ = f.Open(fn);
                    f.Show();
                }
            }
            Application.Run();

            Form1.viewData.Save(adm);
        }

        private static void Exit()
        {
            ArrayList list = (ArrayList)Form1.forms.Clone();
            foreach (Form1 f in list)
            {
                f.Close();
                if (f.Visible)
                {
                    break;
                }
            }
        }

        public FormWindowState LastState = FormWindowState.Normal;

        protected override void OnResize(EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                LastState = WindowState;
            }

            base.OnResize(e);
        }

        #region Menu

        private void mnuFileNew_Click(object sender, System.EventArgs e)
        {
            new Form1().Show();
        }

        private void mnuFileOpen_Click(object sender, System.EventArgs e)
        {
            _ = Open();
        }

        private void mnuFileSave_Click(object sender, System.EventArgs e)
        {
            _ = Save();
        }

        private void mnuFileSaveAs_Click(object sender, System.EventArgs e)
        {
            _ = SaveAs();
        }

        private void mnuFileClose_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void mnuFileExit_Click(object sender, System.EventArgs e)
        {
            Form1.Exit();
        }

        private void mnuOptionWordWrap_Click(object sender, System.EventArgs e)
        {
            view1.txtSource.WordWrap
                = mnuOptionWordWrap.Checked
                = !mnuOptionWordWrap.Checked;
        }

        private void mnuOptionFont_Click(object sender, System.EventArgs e)
        {
            using (FontDialog fd = new FontDialog())
            {
                fd.Font = view1.txtSource.Font;
                if (fd.ShowDialog(this) == DialogResult.OK)
                {
                    view1.txtSource.Font = fd.Font;
                }
            }
        }

        private void mnuHelpAbout_Click(object sender, System.EventArgs e)
        {
            About a = new About();
            a.label1.Text = m_sCaption/*ProductName*/ + " Version " + ProductVersion;
            _ = a.ShowDialog(this);
            a.Dispose();
        }

        private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
        {
            object target = e.Button.Tag;
            if (target == null || !(target is MenuItem))
            {
                return;
            } (target as MenuItem).PerformClick();
        }

        #endregion

        #region Document

        public bool Open()
        {
            openFileDialog1.FileName = document.FullName;
            if (openFileDialog1.ShowDialog(this) == DialogResult.Cancel)
            {
                _ = view1.Focus();
                return false;
            }

            Form1 target = this;
            if (document.Changed || document.FullName != "")
            {
                target = new Form1();
                target.CreateHandle();
            }
            bool ret = target.Open(openFileDialog1.FileName);
            target.Show();
            return ret;
        }

        public bool Open(string fn)
        {
            document.FullName = fn;

            Cursor cur = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            bool ret = document.Open();
            SetCaption();
            SetDocument();
            _ = view1.Focus();
            Cursor.Current = cur;

            return ret;
        }

        public bool Save()
        {
            if (document.FullName == "")
            {
                return SaveAs();
            }

            Cursor cur = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            document.ViewInfo.Store(this);
            bool ret = document.Save();
            SetCaption();
            Cursor.Current = cur;

            return ret;
        }

        public bool SaveAs()
        {
            string fn = document.FullName;
            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(fn);
            saveFileDialog1.FileName = Path.GetFileName(fn);
            DialogResult res = saveFileDialog1.ShowDialog(this);
            _ = view1.Focus();
            if (res == DialogResult.Cancel)
            {
                return false;
            }

            document.FullName = saveFileDialog1.FileName;
            return Save();
        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (document.Changed)
            {
                string msg = string.Format("ファイル {0} の内容は変更されています。\r\n\r\n変更を保存しますか?", document.Name);
                DialogResult res = MessageBox.Show(this, msg, m_sCaption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                if (res == DialogResult.Yes)
                {
                    if (!Save())
                    {
                        e.Cancel = true;
                    }
                }
                else if (res == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        public void SetDocument()
        {
            document.ViewInfo.Apply(this);
            view1.SetDocument(document);
        }

        public void SetCaption()
        {
            statusBar1.Text = document.FullName;
            Text = document.Name + " - " + m_sCaption + (document.Changed ? "*" : "");
        }

        private void view1_Changed(object sender, System.EventArgs e)
        {
            if (document.Changed)
            {
                return;
            }

            document.Changed = true;
            SetCaption();
        }

        #endregion
    }
}
