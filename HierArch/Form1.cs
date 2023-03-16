using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Data;
using Girl.Windows.Forms;

namespace Girl.HierArch
{
	/// <summary>
	/// SDI アプリケーションの雛型です。
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private static WindowSizeData m_Data1;
		private static ViewData m_Data2;
		private static ArrayList m_Forms = new ArrayList();

		private HADoc m_Doc = new HADoc();
		private Hashtable m_tblView = new Hashtable();
		private string m_sCaption;
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
		private System.Windows.Forms.MenuItem mnuView;
		public System.Windows.Forms.MenuItem mnuViewToolBar;
		public System.Windows.Forms.MenuItem mnuViewStatusBar;
		private System.Windows.Forms.MenuItem mnuHelp;
		private System.Windows.Forms.MenuItem mnuHelpAbout;
		private System.Windows.Forms.MenuItem mnuViewSeparator1;
		private System.Windows.Forms.MenuItem mnuViewClass;
		private System.Windows.Forms.MenuItem mnuViewFunc;
		private System.Windows.Forms.MenuItem mnuViewMember;
		private System.Windows.Forms.MenuItem mnuViewArg;
		private System.Windows.Forms.MenuItem mnuViewObject;
		private System.Windows.Forms.MenuItem mnuViewSeparator2;
		private System.Windows.Forms.MenuItem mnuViewComment;
		private System.Windows.Forms.MenuItem mnuCode;
		private System.Windows.Forms.MenuItem mnuCodeGenerateAll;
		private System.Windows.Forms.MenuItem mnuHelpHomePage;
		private System.ComponentModel.IContainer components;

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
			this.tbFileNew  .Tag = mnuFileNew;
			this.tbFileOpen .Tag = mnuFileOpen;
			this.tbFileSave .Tag = mnuFileSave;
			this.tbEditCut  .Tag = mnuEditCut;
			this.tbEditCopy .Tag = mnuEditCopy;
			this.tbEditPaste.Tag = mnuEditPaste;

			// View メニューの項目とコントロールを対応させます。
			// ここで定義した情報は mnuViewItem_Click() で使用されます。
			this.m_tblView.Add(mnuViewToolBar  , toolBar1  );
			this.m_tblView.Add(mnuViewStatusBar, statusBar1);

			this.m_Doc.ClassTreeView = view1.tvClass;
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if (disposing)
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);

			m_Forms.Remove(this);
			if (m_Forms.Count == 0) Application.Exit();
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
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.tbFileNew = new System.Windows.Forms.ToolBarButton();
			this.tbFileOpen = new System.Windows.Forms.ToolBarButton();
			this.tbFileSave = new System.Windows.Forms.ToolBarButton();
			this.tbSeparator1 = new System.Windows.Forms.ToolBarButton();
			this.tbEditCut = new System.Windows.Forms.ToolBarButton();
			this.tbEditCopy = new System.Windows.Forms.ToolBarButton();
			this.tbEditPaste = new System.Windows.Forms.ToolBarButton();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.view1 = new Girl.HierArch.View();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.mnuFile = new System.Windows.Forms.MenuItem();
			this.mnuFileNew = new System.Windows.Forms.MenuItem();
			this.mnuFileOpen = new System.Windows.Forms.MenuItem();
			this.mnuFileSave = new System.Windows.Forms.MenuItem();
			this.mnuFileSaveAs = new System.Windows.Forms.MenuItem();
			this.mnuFileSeparator1 = new System.Windows.Forms.MenuItem();
			this.mnuFileClose = new System.Windows.Forms.MenuItem();
			this.mnuFileExit = new System.Windows.Forms.MenuItem();
			this.mnuEdit = new System.Windows.Forms.MenuItem();
			this.mnuEditCut = new System.Windows.Forms.MenuItem();
			this.mnuEditCopy = new System.Windows.Forms.MenuItem();
			this.mnuEditPaste = new System.Windows.Forms.MenuItem();
			this.mnuView = new System.Windows.Forms.MenuItem();
			this.mnuViewToolBar = new System.Windows.Forms.MenuItem();
			this.mnuViewStatusBar = new System.Windows.Forms.MenuItem();
			this.mnuViewSeparator1 = new System.Windows.Forms.MenuItem();
			this.mnuViewClass = new System.Windows.Forms.MenuItem();
			this.mnuViewFunc = new System.Windows.Forms.MenuItem();
			this.mnuViewMember = new System.Windows.Forms.MenuItem();
			this.mnuViewArg = new System.Windows.Forms.MenuItem();
			this.mnuViewObject = new System.Windows.Forms.MenuItem();
			this.mnuViewSeparator2 = new System.Windows.Forms.MenuItem();
			this.mnuViewComment = new System.Windows.Forms.MenuItem();
			this.mnuCode = new System.Windows.Forms.MenuItem();
			this.mnuCodeGenerateAll = new System.Windows.Forms.MenuItem();
			this.mnuHelp = new System.Windows.Forms.MenuItem();
			this.mnuHelpHomePage = new System.Windows.Forms.MenuItem();
			this.mnuHelpAbout = new System.Windows.Forms.MenuItem();
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
																						this.tbEditPaste});
			this.toolBar1.DropDownArrows = true;
			this.toolBar1.ImageList = this.imageList1;
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.ShowToolTips = true;
			this.toolBar1.Size = new System.Drawing.Size(712, 25);
			this.toolBar1.TabIndex = 0;
			this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
			// 
			// tbFileNew
			// 
			this.tbFileNew.ImageIndex = 0;
			this.tbFileNew.ToolTipText = "新規作成 (Ctrl+N)";
			// 
			// tbFileOpen
			// 
			this.tbFileOpen.ImageIndex = 1;
			this.tbFileOpen.ToolTipText = "開く (Ctrl+O)";
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
			this.tbEditCut.Enabled = false;
			this.tbEditCut.ImageIndex = 3;
			this.tbEditCut.ToolTipText = "切り取り (Ctrl+X)";
			// 
			// tbEditCopy
			// 
			this.tbEditCopy.Enabled = false;
			this.tbEditCopy.ImageIndex = 4;
			this.tbEditCopy.ToolTipText = "コピー (Ctrl+C)";
			// 
			// tbEditPaste
			// 
			this.tbEditPaste.Enabled = false;
			this.tbEditPaste.ImageIndex = 5;
			this.tbEditPaste.ToolTipText = "貼り付け (Ctrl+V)";
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
			this.statusBar1.Location = new System.Drawing.Point(0, 467);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Size = new System.Drawing.Size(712, 22);
			this.statusBar1.TabIndex = 1;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.DefaultExt = "haprj";
			this.openFileDialog1.Filter = "HierArch プロジェクト (*.haprj)|*.haprj|HDS 文書 (*.hds)|*.hds|すべてのファイル (*.*)|*.*";
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.DefaultExt = "haprj";
			this.saveFileDialog1.FileName = "doc1";
			this.saveFileDialog1.Filter = "HierArch プロジェクト (*.haprj)|*.haprj|HDS 文書 (*.hds)|*.hds|すべてのファイル (*.*)|*.*";
			// 
			// view1
			// 
			this.view1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.view1.Location = new System.Drawing.Point(0, 25);
			this.view1.Name = "view1";
			this.view1.Size = new System.Drawing.Size(712, 442);
			this.view1.TabIndex = 2;
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuFile,
																					  this.mnuEdit,
																					  this.mnuView,
																					  this.mnuCode,
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
																					this.mnuEditCut,
																					this.mnuEditCopy,
																					this.mnuEditPaste});
			this.mnuEdit.Text = "編集(&E)";
			// 
			// mnuEditCut
			// 
			this.mnuEditCut.Enabled = false;
			this.mnuEditCut.Index = 0;
			this.mnuEditCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
			this.mnuEditCut.Text = "切り取り(&T)";
			// 
			// mnuEditCopy
			// 
			this.mnuEditCopy.Enabled = false;
			this.mnuEditCopy.Index = 1;
			this.mnuEditCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
			this.mnuEditCopy.Text = "コピー(&C)";
			// 
			// mnuEditPaste
			// 
			this.mnuEditPaste.Enabled = false;
			this.mnuEditPaste.Index = 2;
			this.mnuEditPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
			this.mnuEditPaste.Text = "貼り付け(&P)";
			// 
			// mnuView
			// 
			this.mnuView.Index = 2;
			this.mnuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuViewToolBar,
																					this.mnuViewStatusBar,
																					this.mnuViewSeparator1,
																					this.mnuViewClass,
																					this.mnuViewFunc,
																					this.mnuViewMember,
																					this.mnuViewArg,
																					this.mnuViewObject,
																					this.mnuViewSeparator2,
																					this.mnuViewComment});
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
			// mnuViewSeparator1
			// 
			this.mnuViewSeparator1.Index = 2;
			this.mnuViewSeparator1.Text = "-";
			// 
			// mnuViewClass
			// 
			this.mnuViewClass.Checked = true;
			this.mnuViewClass.Index = 3;
			this.mnuViewClass.Text = "クラス(&C)";
			this.mnuViewClass.Click += new System.EventHandler(this.mnuViewClass_Click);
			// 
			// mnuViewFunc
			// 
			this.mnuViewFunc.Checked = true;
			this.mnuViewFunc.Index = 4;
			this.mnuViewFunc.Text = "関数(&F)";
			this.mnuViewFunc.Click += new System.EventHandler(this.mnuViewFunc_Click);
			// 
			// mnuViewMember
			// 
			this.mnuViewMember.Checked = true;
			this.mnuViewMember.Index = 5;
			this.mnuViewMember.Text = "メンバ(&M)";
			this.mnuViewMember.Click += new System.EventHandler(this.mnuViewMember_Click);
			// 
			// mnuViewArg
			// 
			this.mnuViewArg.Checked = true;
			this.mnuViewArg.Index = 6;
			this.mnuViewArg.Text = "引数(&A)";
			this.mnuViewArg.Click += new System.EventHandler(this.mnuViewArg_Click);
			// 
			// mnuViewObject
			// 
			this.mnuViewObject.Checked = true;
			this.mnuViewObject.Index = 7;
			this.mnuViewObject.Text = "変数(&O)";
			this.mnuViewObject.Click += new System.EventHandler(this.mnuViewObject_Click);
			// 
			// mnuViewSeparator2
			// 
			this.mnuViewSeparator2.Index = 8;
			this.mnuViewSeparator2.Text = "-";
			// 
			// mnuViewComment
			// 
			this.mnuViewComment.Checked = true;
			this.mnuViewComment.Index = 9;
			this.mnuViewComment.Text = "注釈(&E)";
			this.mnuViewComment.Click += new System.EventHandler(this.mnuViewComment_Click);
			// 
			// mnuCode
			// 
			this.mnuCode.Index = 3;
			this.mnuCode.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuCodeGenerateAll});
			this.mnuCode.Text = "コード(&C)";
			// 
			// mnuCodeGenerateAll
			// 
			this.mnuCodeGenerateAll.Index = 0;
			this.mnuCodeGenerateAll.Text = "全生成(&A)";
			this.mnuCodeGenerateAll.Click += new System.EventHandler(this.mnuCodeGenerateAll_Click);
			// 
			// mnuHelp
			// 
			this.mnuHelp.Index = 4;
			this.mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuHelpHomePage,
																					this.mnuHelpAbout});
			this.mnuHelp.Text = "ヘルプ(&H)";
			// 
			// mnuHelpHomePage
			// 
			this.mnuHelpHomePage.Index = 0;
			this.mnuHelpHomePage.Text = "ホームページ(&H)";
			this.mnuHelpHomePage.Click += new System.EventHandler(this.menuHelpHomePage_Click);
			// 
			// mnuHelpAbout
			// 
			this.mnuHelpAbout.Index = 1;
			this.mnuHelpAbout.Text = "バージョン情報(&A)";
			this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(712, 489);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.view1,
																		  this.statusBar1,
																		  this.toolBar1});
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.Text = "HierArch";
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
			m_Data1 = WindowSizeData.Load(adm);
			m_Data2 = ViewData.Load(adm);

			Form1 f;
			if (args.GetLength(0) < 1)
			{
				f = CreateForm();
				f.Show();
			}
			else
			{
				foreach (string fn in args)
				{
					f = CreateForm();
					f.Show();
					f.Open(fn);
				}
			}
			Application.Run();

			m_Data1.Save(adm);
			m_Data2.Save(adm);
		}

		private static Form1 CreateForm()
		{
			Form1 ret = new Form1();
			m_Forms.Add(ret);
			new WindowSizeManager(ret, m_Data1, true);
			new ViewManager(ret, m_Data2);
			return ret;
		}

		private static void Exit()
		{
			ArrayList list = (ArrayList)m_Forms.Clone();
			foreach(Form1 f in list)
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

		private void mnuFileOpen_Click(object sender, System.EventArgs e)
		{
			this.Open();
		}

		private void mnuFileSave_Click(object sender, System.EventArgs e)
		{
			this.Save();
		}

		private void mnuFileSaveAs_Click(object sender, System.EventArgs e)
		{
			this.SaveAs();
		}

		private void mnuFileClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void mnuFileExit_Click(object sender, System.EventArgs e)
		{
			Form1.Exit();
		}

		private void mnuViewItem_Click(object sender, System.EventArgs e)
		{
			object target = m_tblView[sender];
			if(target == null) return;

			bool st = !((Control)target).Visible;
			((Control)target).Visible = st;
			((MenuItem)sender).Checked = st;
		}

		#region Visibility

		private void mnuViewClass_Click(object sender, System.EventArgs e)
		{
			bool v = this.mnuViewClass.Checked = !this.view1.tabClass.Visible;
			this.view1.SetPanel1(v, this.view1.tabFunc.Visible);
		}

		private void mnuViewFunc_Click(object sender, System.EventArgs e)
		{
			bool v = this.mnuViewFunc.Checked = !this.view1.tabFunc.Visible;
			this.view1.SetPanel1(this.view1.tabClass.Visible, v);
		}

		private void mnuViewMember_Click(object sender, System.EventArgs e)
		{
			bool v = this.mnuViewMember.Checked = !this.view1.tabMember.Visible;
			this.view1.SetPanel3(v, this.view1.tabArg.Visible, this.view1.tabObject.Visible);
		}

		private void mnuViewArg_Click(object sender, System.EventArgs e)
		{
			bool v = this.mnuViewArg.Checked = !this.view1.tabArg.Visible;
			this.view1.SetPanel3(this.view1.tabMember.Visible, v, this.view1.tabObject.Visible);
		}

		private void mnuViewObject_Click(object sender, System.EventArgs e)
		{
			bool v = this.mnuViewObject.Checked = !this.view1.tabObject.Visible;
			this.view1.SetPanel3(this.view1.tabMember.Visible, this.view1.tabArg.Visible, v);
		}

		private void mnuViewComment_Click(object sender, System.EventArgs e)
		{
			this.view1.txtComment.Visible
				= this.view1.opaqueSplitter4.Visible
				= this.mnuViewComment.Checked
				= !this.view1.txtComment.Visible;
		}

		#endregion

		private void mnuCodeGenerateAll_Click(object sender, System.EventArgs e)
		{
			string path;
			try
			{
				path = Path.GetDirectoryName(this.m_Doc.FullName);
			}
			catch
			{
				MessageBox.Show(this, "プロジェクトを保存してください。", this.m_sCaption,
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			Cursor cur = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			this.view1.GenerateAll(path);
			Cursor.Current = cur;
		}

		private void menuHelpHomePage_Click(object sender, System.EventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.egroups.co.jp/files/miscprj-dev/HierArch/");
		}

		private void mnuHelpAbout_Click(object sender, System.EventArgs e)
		{
			About a = new About();
			a.label1.Text = m_sCaption/*ProductName*/ + " Version " + ProductVersion;
			a.ShowDialog(this);
			a.Dispose();
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			object target = e.Button.Tag;
			if(target.GetType() == typeof(MenuItem))
			{
				((MenuItem)target).PerformClick();
			}
		}

		#endregion

		#region Document

		public bool Open()
		{
			openFileDialog1.FileName = m_Doc.FullName;
			if(openFileDialog1.ShowDialog(this) == DialogResult.Cancel)
			{
				view1.Focus();
				return false;
			}

			Form1 target = this;
			if(m_Doc.Changed || m_Doc.FullName != "")
			{
				target = CreateForm();
				target.Show();
			}
			return target.Open(openFileDialog1.FileName);
		}

		public bool Open(string fn)
		{
			m_Doc.FullName = fn;
			bool ret = m_Doc.Open();
			SetCaption();
			SetDocument();
			view1.Focus();
			return ret;
		}

		public bool Save()
		{
			if(m_Doc.FullName == "") return SaveAs();

			bool ret = m_Doc.Save();
			SetCaption();
			return ret;
		}

		public bool SaveAs()
		{
			saveFileDialog1.FileName = m_Doc.FullName;
			DialogResult res = saveFileDialog1.ShowDialog(this);
			view1.Focus();
			if(res == DialogResult.Cancel) return false;

			m_Doc.FullName = saveFileDialog1.FileName;
			bool ret = m_Doc.Save();
			SetCaption();
			return ret;
		}

		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(m_Doc.Changed)
			{
				string msg = string.Format("ファイル {0} の内容は変更されています。\r\n\r\n変更を保存しますか?", m_Doc.Name);
				DialogResult res = MessageBox.Show(this, msg, m_sCaption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
				if(res == DialogResult.Yes)
				{
					if(!Save()) e.Cancel = true;
				}
				else if(res == DialogResult.Cancel)
				{
					e.Cancel = true;
				}
			}
		}

		public void SetDocument()
		{
			view1.SetDocument(m_Doc);
		}

		public void SetCaption()
		{
			statusBar1.Text = m_Doc.FullName;
			Text = m_Doc.Name + " - " + m_sCaption + (m_Doc.Changed ? "*" : "");
		}

		private void view1_Changed(object sender, System.EventArgs e)
		{
			if(m_Doc.Changed) return;

			m_Doc.Changed = true;
			SetCaption();
		}

		#endregion
	}
}
