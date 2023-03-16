using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Girl.Windows.Forms;

namespace Girl.HierArch.PluginEditor
{
	/// <summary>
	/// SDI アプリケーションの雛型です。
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private static WindowSizeData s_Data1;
		private static ViewData s_Data2;
		private static ArrayList s_Forms = new ArrayList();

		private PluginEditorDoc document = new PluginEditorDoc();
		private Hashtable m_tblView = new Hashtable();
		private EditManager editManager = new EditManager();
		private string m_sCaption;
		private static CodeEditorManager codeEditorManager = new CodeEditorManager();

		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem mnuFile;
		private System.Windows.Forms.MenuItem mnuEdit;
		private System.Windows.Forms.MenuItem mnuView;
		private System.Windows.Forms.MenuItem mnuHelp;
		private System.Windows.Forms.MenuItem mnuHelpAbout;
		public System.Windows.Forms.ToolBar toolBar1;
		public System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ToolBarButton tbFileSave;
		private System.Windows.Forms.ToolBarButton tbSeparator1;
		private System.Windows.Forms.ToolBarButton tbEditCut;
		private System.Windows.Forms.ToolBarButton tbEditCopy;
		private System.Windows.Forms.ToolBarButton tbEditPaste;
		private System.Windows.Forms.MenuItem mnuFileSeparator1;
		private System.Windows.Forms.MenuItem mnuEditCut;
		private System.Windows.Forms.MenuItem mnuEditCopy;
		private System.Windows.Forms.MenuItem mnuEditPaste;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		public System.Windows.Forms.MenuItem mnuViewToolBar;
		public System.Windows.Forms.MenuItem mnuViewStatusBar;
		private System.Windows.Forms.MenuItem mnuFileClose;
		public Girl.HierArch.PluginEditor.View view1;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MenuItem mnuEditDelete;
		private System.Windows.Forms.MenuItem mnuEditSeparator1;
		private System.Windows.Forms.MenuItem mnuEditSelectAll;
		private System.Windows.Forms.MenuItem mnuHelpHomePage;
		private System.Windows.Forms.MenuItem mnuEditUndo;
		private System.Windows.Forms.MenuItem mnuEditSeparator2;
		private System.Windows.Forms.MenuItem mnuEditRedo;
		private System.Windows.Forms.MenuItem mnuOption;
		public System.Windows.Forms.MenuItem mnuOptionSmartEnter;
		public System.Windows.Forms.MenuItem mnuOptionSmartHome;
		public System.Windows.Forms.MenuItem mnuOptionSmartParenthesis;
		private System.Windows.Forms.ToolBarButton tbSeparator2;
		private System.Windows.Forms.ToolBarButton tbEditRedo;
		private System.Windows.Forms.ToolBarButton tbEditUndo;
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
		private System.Windows.Forms.MenuItem mnuBuild;
		private System.Windows.Forms.MenuItem mnuBuildBuild;
		private System.Windows.Forms.ToolBarButton tbSeparator3;
		private System.Windows.Forms.ToolBarButton tbBuildBuild;
		private System.Windows.Forms.MenuItem mnuEditSeparator3;
		private System.Windows.Forms.MenuItem mnuEditRecolor;
		private System.Windows.Forms.MenuItem mnuFileSave;
		public System.Windows.Forms.MenuItem mnuOptionSmartTab;

		public Form1()
		{
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//
			this.m_sCaption = Text;
			this.SetCaption();
			this.SetDocument();
			this.view1.Changed += new System.EventHandler(view1_Changed);

			// ツールバーのボタンをメニューの項目に対応させます。
			// ここで定義した情報は toolBar1_ButtonClick() で使用されます。
			this.tbFileSave  .Tag = this.mnuFileSave;
			this.tbBuildBuild.Tag = this.mnuBuildBuild;

			// テキストボックスの状態をメニューと連動させます。
			this.editManager.AddControl(this.view1.codeEditor);
			this.editManager.AddControl(this.view1.logViewer);
			this.editManager.SetCommand(EditAction.Undo     , this.mnuEditUndo     , this.cmEditUndo , this.tbEditUndo );
			this.editManager.SetCommand(EditAction.Redo     , this.mnuEditRedo     , this.cmEditRedo , this.tbEditRedo );
			this.editManager.SetCommand(EditAction.Cut      , this.mnuEditCut      , this.cmEditCut  , this.tbEditCut  );
			this.editManager.SetCommand(EditAction.Copy     , this.mnuEditCopy     , this.cmEditCopy , this.tbEditCopy );
			this.editManager.SetCommand(EditAction.Paste    , this.mnuEditPaste    , this.cmEditPaste, this.tbEditPaste);
			this.editManager.SetCommand(EditAction.Delete   , this.mnuEditDelete   , this.cmEditDelete   );
			this.editManager.SetCommand(EditAction.SelectAll, this.mnuEditSelectAll, this.cmEditSelectAll);

			// エディタオプションを設定します。
			Form1.codeEditorManager.SetTarget(this.view1.codeEditor);
			Form1.codeEditorManager.SetCommand(CodeEditorOption.SmartEnter      , this.mnuOptionSmartEnter      );
			Form1.codeEditorManager.SetCommand(CodeEditorOption.SmartTab        , this.mnuOptionSmartTab        );
			Form1.codeEditorManager.SetCommand(CodeEditorOption.SmartHome       , this.mnuOptionSmartHome       );
			Form1.codeEditorManager.SetCommand(CodeEditorOption.SmartParenthesis, this.mnuOptionSmartParenthesis);

			// View メニューの項目とコントロールを対応させます。
			// ここで定義した情報は mnuViewItem_Click() で使用されます。
			this.m_tblView.Add(mnuViewToolBar  , toolBar1  );
			this.m_tblView.Add(mnuViewStatusBar, statusBar1);

			this.view1.codeEditor.ContextMenu = this.cmEdit;
			this.view1.logViewer .ContextMenu = this.cmEdit;
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );

			s_Forms.Remove(this);
			if (s_Forms.Count == 0) Application.Exit();
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.mnuFile = new System.Windows.Forms.MenuItem();
			this.mnuFileSeparator1 = new System.Windows.Forms.MenuItem();
			this.mnuFileClose = new System.Windows.Forms.MenuItem();
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
			this.mnuEditSeparator3 = new System.Windows.Forms.MenuItem();
			this.mnuEditRecolor = new System.Windows.Forms.MenuItem();
			this.mnuView = new System.Windows.Forms.MenuItem();
			this.mnuViewToolBar = new System.Windows.Forms.MenuItem();
			this.mnuViewStatusBar = new System.Windows.Forms.MenuItem();
			this.mnuBuild = new System.Windows.Forms.MenuItem();
			this.mnuBuildBuild = new System.Windows.Forms.MenuItem();
			this.mnuOption = new System.Windows.Forms.MenuItem();
			this.mnuOptionSmartEnter = new System.Windows.Forms.MenuItem();
			this.mnuOptionSmartTab = new System.Windows.Forms.MenuItem();
			this.mnuOptionSmartHome = new System.Windows.Forms.MenuItem();
			this.mnuOptionSmartParenthesis = new System.Windows.Forms.MenuItem();
			this.mnuHelp = new System.Windows.Forms.MenuItem();
			this.mnuHelpHomePage = new System.Windows.Forms.MenuItem();
			this.mnuHelpAbout = new System.Windows.Forms.MenuItem();
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.tbFileSave = new System.Windows.Forms.ToolBarButton();
			this.tbSeparator1 = new System.Windows.Forms.ToolBarButton();
			this.tbEditCut = new System.Windows.Forms.ToolBarButton();
			this.tbEditCopy = new System.Windows.Forms.ToolBarButton();
			this.tbEditPaste = new System.Windows.Forms.ToolBarButton();
			this.tbSeparator2 = new System.Windows.Forms.ToolBarButton();
			this.tbEditUndo = new System.Windows.Forms.ToolBarButton();
			this.tbEditRedo = new System.Windows.Forms.ToolBarButton();
			this.tbSeparator3 = new System.Windows.Forms.ToolBarButton();
			this.tbBuildBuild = new System.Windows.Forms.ToolBarButton();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.view1 = new Girl.HierArch.PluginEditor.View();
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
			this.mnuFileSave = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuFile,
																					  this.mnuEdit,
																					  this.mnuView,
																					  this.mnuBuild,
																					  this.mnuOption,
																					  this.mnuHelp});
			// 
			// mnuFile
			// 
			this.mnuFile.Index = 0;
			this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuFileSave,
																					this.mnuFileSeparator1,
																					this.mnuFileClose});
			this.mnuFile.Text = "ファイル(&F)";
			// 
			// mnuFileSeparator1
			// 
			this.mnuFileSeparator1.Index = 1;
			this.mnuFileSeparator1.Text = "-";
			// 
			// mnuFileClose
			// 
			this.mnuFileClose.Index = 2;
			this.mnuFileClose.Shortcut = System.Windows.Forms.Shortcut.CtrlW;
			this.mnuFileClose.Text = "閉じる(&C)";
			this.mnuFileClose.Click += new System.EventHandler(this.mnuFileClose_Click);
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
																					this.mnuEditSelectAll,
																					this.mnuEditSeparator3,
																					this.mnuEditRecolor});
			this.mnuEdit.Text = "編集(&E)";
			// 
			// mnuEditUndo
			// 
			this.mnuEditUndo.Index = 0;
			this.mnuEditUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
			this.mnuEditUndo.Text = "元に戻す(&U)";
			// 
			// mnuEditRedo
			// 
			this.mnuEditRedo.Index = 1;
			this.mnuEditRedo.Shortcut = System.Windows.Forms.Shortcut.CtrlY;
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
			this.mnuEditCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
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
			this.mnuEditPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
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
			// mnuEditSeparator3
			// 
			this.mnuEditSeparator3.Index = 9;
			this.mnuEditSeparator3.Text = "-";
			// 
			// mnuEditRecolor
			// 
			this.mnuEditRecolor.Index = 10;
			this.mnuEditRecolor.Text = "色の付け直し(&E)";
			this.mnuEditRecolor.Click += new System.EventHandler(this.mnuEditRecolor_Click);
			// 
			// mnuView
			// 
			this.mnuView.Index = 2;
			this.mnuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuViewToolBar,
																					this.mnuViewStatusBar});
			this.mnuView.Text = "表示(&V)";
			// 
			// mnuViewToolBar
			// 
			this.mnuViewToolBar.Index = 0;
			this.mnuViewToolBar.Text = "ツールバー(&T)";
			this.mnuViewToolBar.Click += new System.EventHandler(this.mnuViewItem_Click);
			// 
			// mnuViewStatusBar
			// 
			this.mnuViewStatusBar.Index = 1;
			this.mnuViewStatusBar.Text = "ステータスバー(&S)";
			this.mnuViewStatusBar.Click += new System.EventHandler(this.mnuViewItem_Click);
			// 
			// mnuBuild
			// 
			this.mnuBuild.Index = 3;
			this.mnuBuild.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.mnuBuildBuild});
			this.mnuBuild.Text = "ビルド(&B)";
			// 
			// mnuBuildBuild
			// 
			this.mnuBuildBuild.Enabled = false;
			this.mnuBuildBuild.Index = 0;
			this.mnuBuildBuild.Text = "ビルド(&B)";
			this.mnuBuildBuild.Click += new System.EventHandler(this.mnuBuildBuild_Click);
			// 
			// mnuOption
			// 
			this.mnuOption.Index = 4;
			this.mnuOption.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuOptionSmartEnter,
																					  this.mnuOptionSmartTab,
																					  this.mnuOptionSmartHome,
																					  this.mnuOptionSmartParenthesis});
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
			// mnuHelp
			// 
			this.mnuHelp.Index = 5;
			this.mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuHelpHomePage,
																					this.mnuHelpAbout});
			this.mnuHelp.Text = "ヘルプ(&H)";
			// 
			// mnuHelpHomePage
			// 
			this.mnuHelpHomePage.Index = 0;
			this.mnuHelpHomePage.Text = "ホームページ(&H)";
			this.mnuHelpHomePage.Click += new System.EventHandler(this.mnuHelpHomePage_Click);
			// 
			// mnuHelpAbout
			// 
			this.mnuHelpAbout.Index = 1;
			this.mnuHelpAbout.Text = "バージョン情報(&A)";
			this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
			// 
			// toolBar1
			// 
			this.toolBar1.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						this.tbFileSave,
																						this.tbSeparator1,
																						this.tbEditCut,
																						this.tbEditCopy,
																						this.tbEditPaste,
																						this.tbSeparator2,
																						this.tbEditUndo,
																						this.tbEditRedo,
																						this.tbSeparator3,
																						this.tbBuildBuild});
			this.toolBar1.DropDownArrows = true;
			this.toolBar1.ImageList = this.imageList1;
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.ShowToolTips = true;
			this.toolBar1.Size = new System.Drawing.Size(520, 25);
			this.toolBar1.TabIndex = 99;
			this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
			// 
			// tbFileSave
			// 
			this.tbFileSave.ImageIndex = 2;
			this.tbFileSave.ToolTipText = "上書き保存 (Ctrl+S)";
			// 
			// tbSeparator1
			// 
			this.tbSeparator1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbEditCut
			// 
			this.tbEditCut.ImageIndex = 3;
			this.tbEditCut.ToolTipText = "切り取り (Ctrl+X)";
			// 
			// tbEditCopy
			// 
			this.tbEditCopy.ImageIndex = 4;
			this.tbEditCopy.ToolTipText = "コピー (Ctrl+C)";
			// 
			// tbEditPaste
			// 
			this.tbEditPaste.ImageIndex = 5;
			this.tbEditPaste.ToolTipText = "貼り付け (Ctrl+V)";
			// 
			// tbSeparator2
			// 
			this.tbSeparator2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbEditUndo
			// 
			this.tbEditUndo.ImageIndex = 6;
			this.tbEditUndo.ToolTipText = "元に戻す (Ctrl+Z)";
			// 
			// tbEditRedo
			// 
			this.tbEditRedo.ImageIndex = 7;
			this.tbEditRedo.ToolTipText = "やり直し (Ctrl+Y)";
			// 
			// tbSeparator3
			// 
			this.tbSeparator3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbBuildBuild
			// 
			this.tbBuildBuild.Enabled = false;
			this.tbBuildBuild.ImageIndex = 8;
			this.tbBuildBuild.ToolTipText = "ビルド";
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 443);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Size = new System.Drawing.Size(520, 22);
			this.statusBar1.TabIndex = 1;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.Filter = "C# ファイル (*.cs)|*.cs|すべてのファイル (*.*)|*.*";
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.FileName = "無題";
			this.saveFileDialog1.Filter = "C# ファイル (*.cs)|*.cs|すべてのファイル (*.*)|*.*";
			// 
			// view1
			// 
			this.view1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.view1.Location = new System.Drawing.Point(0, 25);
			this.view1.Name = "view1";
			this.view1.Size = new System.Drawing.Size(520, 418);
			this.view1.TabIndex = 1;
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
			// mnuFileSave
			// 
			this.mnuFileSave.Index = 0;
			this.mnuFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.mnuFileSave.Text = "上書き保存(&S)";
			this.mnuFileSave.Click += new System.EventHandler(this.mnuFileSave_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(520, 465);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.view1,
																		  this.statusBar1,
																		  this.toolBar1});
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.Text = "HierArch Plugin Editor";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.AddMessageFilter(new MouseWheelMessageFilter());

			ApplicationDataManager adm = new ApplicationDataManager();
			s_Data1 = WindowSizeData.Load(adm);
			s_Data2 = ViewData.Load(adm);

			Form1 f;
			if (args.GetLength(0) < 1)
			{
				f = Form1.CreateForm();
				f.Show();
			}
			else
			{
				foreach (string fn in args)
				{
					f = Form1.CreateForm();
					f.Show();
					f.Open(fn);
				}
			}
			Application.Run();

			s_Data1.Save(adm);
			s_Data2.Save(adm);
		}

		private static Form1 CreateForm()
		{
			Form1 ret = new Form1();
			Form1.s_Forms.Add(ret);
			new WindowSizeManager(ret, s_Data1, true);
			new ViewManager(ret, s_Data2);
			return ret;
		}

		private static void Exit()
		{
			ArrayList list = Form1.s_Forms.Clone() as ArrayList;
			foreach (Form1 f in list)
			{
				f.Close();
				if (f.Visible) break;
			}
		}

		#region Menu

		private void mnuFileNew_Click(object sender, System.EventArgs e)
		{
			Form1.CreateForm().Show();
		}

		private void mnuFileSave_Click(object sender, System.EventArgs e)
		{
			this.Save();
		}

		private void mnuFileClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void mnuEditRecolor_Click(object sender, System.EventArgs e)
		{
			this.view1.Recolor();
		}

		private void mnuViewItem_Click(object sender, System.EventArgs e)
		{
			Control target = m_tblView[sender] as Control;
			if (target == null) return;

			(sender as MenuItem).Checked = target.Visible = !target.Visible;
		}

		private void mnuBuildBuild_Click(object sender, System.EventArgs e)
		{
			this.Build();
		}

		private void mnuHelpHomePage_Click(object sender, System.EventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.egroups.co.jp/files/miscprj-dev/HierArch/");
		}

		private void mnuHelpAbout_Click(object sender, System.EventArgs e)
		{
			MessageBox.Show(this,
				this.ProductName + " ver." + this.ProductVersion
				+ " (" + PluginEditorDoc.BuildDateTime + ")\r\n"
				+ "このソフトウェアはパブリックドメインです。",
				"About...\r\n");
			this.view1.codeEditor.Focus();
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			object target = e.Button.Tag;
			if (target is MenuItem)
			{
				(target as MenuItem).PerformClick();
			}
		}

		#endregion

		#region Document

		public bool Open()
		{
			this.openFileDialog1.FileName = this.document.FullName;
			if (this.openFileDialog1.ShowDialog(this) == DialogResult.Cancel)
			{
				this.view1.codeEditor.Focus();
				return false;
			}

			Form1 target = this;
			if (this.document.Changed || this.document.FullName != "")
			{
				target = Form1.CreateForm();
				target.Show();
			}
			return target.Open(openFileDialog1.FileName);
		}

		private void EnableBuild()
		{
			this.mnuBuildBuild.Enabled = this.tbBuildBuild.Enabled = true;
		}

		public bool Open(string fn)
		{
			this.document.FullName = fn;
			bool ret = this.document.Open();
			this.SetCaption();
			this.SetDocument();
			this.view1.codeEditor.Focus();
			this.EnableBuild();
			return ret;
		}

		public bool Save()
		{
			//if (this.document.FullName == "") return this.SaveAs();

			this.document.Text = this.view1.codeEditor.Code;
			bool ret = this.document.Save();
			this.SetCaption();
			this.EnableBuild();
			return ret;
		}

		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.document.Changed)
			{
				string msg = string.Format("ファイル {0} の内容は変更されています。\r\n\r\n変更を保存しますか?", document.Name);
				DialogResult res = MessageBox.Show(this, msg, m_sCaption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
				if (res == DialogResult.Yes)
				{
					if (!this.Save()) e.Cancel = true;
				}
				else if (res == DialogResult.Cancel)
				{
					e.Cancel = true;
				}
			}
		}

		public void SetDocument()
		{
			this.view1.SetDocument(document);
		}

		public void SetCaption()
		{
			this.statusBar1.Text = this.document.FullName;
			Text = this.document.Name + " - " + this.m_sCaption + (this.document.Changed ? "*" : "");
		}

		private void view1_Changed(object sender, System.EventArgs e)
		{
			if (this.document.Changed) return;

			this.document.Changed = true;
			this.SetCaption();
		}

		#endregion

		#region Build

		private string buildTarget = "";

		private bool Build()
		{
			Cursor cur = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			this.view1.logViewer.Clear();
			this.view1.logViewer.AppendLine("[コンパイル開始]", Color.DarkGreen);

			int p = this.document.FullName.LastIndexOf('.');
			if (p > 0)
			{
				this.buildTarget = this.document.FullName.Substring(0, p) + ".exe";
			}
			else
			{
				this.buildTarget = this.document.FullName + ".exe";
			}

			Microsoft.CSharp.CSharpCodeProvider codeProvider =
				new Microsoft.CSharp.CSharpCodeProvider();
			ICodeCompiler icc = codeProvider.CreateCompiler();
			CompilerParameters parameters = new CompilerParameters();
			parameters.GenerateExecutable = true;
			parameters.ReferencedAssemblies.AddRange(new string[]
				{
					"System.dll", "System.Data.dll", "System.Drawing.dll",
                    "System.Windows.Forms.dll", "System.XML.dll"
				});
			parameters.OutputAssembly = this.buildTarget;
			parameters.CompilerOptions = "/target:library";
			CompilerResults results = icc.CompileAssemblyFromSource(
				parameters, this.view1.codeEditor.Code);

			Cursor.Current = cur;
			if (results.Errors.Count > 0)
			{
				foreach (CompilerError ce in results.Errors)
				{
					this.view1.logViewer.AppendLink(string.Format("{0} 行 {1} 列", ce.Line, ce.Column),
						new Point(ce.Column - 1, ce.Line - 1));
					this.view1.logViewer.AppendLine(string.Format(" : {0} {1}: {2}",
						ce.IsWarning ? "警告" : "エラー", ce.ErrorNumber, ce.ErrorText));
				}
				this.view1.logViewer.AppendLine("[コンパイル終了] (エラー)", Color.Red);
				this.buildTarget = "";
				return false;
			}

			this.view1.logViewer.AppendLine("[コンパイル終了]", Color.DarkGreen);
			return true;
		}

		#endregion
	}
}
