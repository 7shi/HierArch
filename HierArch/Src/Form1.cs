using Girl.Windows.Forms;
using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace Girl.HierArch
{
    public partial class Form1 : Form
    {
        public WindowSizeMonitor sizeMonitor;
        private static ViewData viewData;
        private static readonly ArrayList forms = new ArrayList();

        private readonly HADoc document = new HADoc();
        private readonly Hashtable m_tblView = new Hashtable();
        private readonly string m_sCaption;
        private readonly EditManager editManager = new EditManager();
        private static readonly CodeEditorManager codeEditorManager = new CodeEditorManager(2);

        public Form1()
        {
            InitializeComponent();

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
