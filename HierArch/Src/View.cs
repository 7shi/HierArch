using Girl.Windows.Forms;
using System;
using System.Windows.Forms;

namespace Girl.HierArch
{
    /// <summary>
    /// Document に対応する View です。
    /// </summary>
    public class View : System.Windows.Forms.UserControl
    {
        public System.Windows.Forms.Panel panel1;
        public event EventHandler Changed;
        private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter1;
        private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter3;
        public HAClass tvClass;
        private HAFunc tvFunc;
        public Girl.Windows.Forms.CodeEditor txtSource;

        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private readonly System.ComponentModel.Container components = null;

        public View()
        {
            // この呼び出しは、Windows.Forms フォーム デザイナで必要です。
            InitializeComponent();

            // TODO: InitializeComponent を呼び出しの後に初期化処理を追加します。
            tvClass.MoveTarget.Add(tvFunc);
            tvClass.FuncTreeView = tvFunc;
            tvFunc.SourceTextBox = txtSource;

            tvClass.SetView();

            tvClass.Changed += new EventHandler(OnChanged);
            tvFunc.Changed += new EventHandler(OnChanged);

            txtSource.TextChanged += new EventHandler(tvFunc.OnChanged);
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
        }

        #region Component Designer generated code
        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            tvClass = new Girl.HierArch.HAClass();
            tvFunc = new Girl.HierArch.HAFunc();
            txtSource = new Girl.Windows.Forms.CodeEditor();
            panel1 = new System.Windows.Forms.Panel();
            opaqueSplitter3 = new Girl.Windows.Forms.OpaqueSplitter();
            opaqueSplitter1 = new Girl.Windows.Forms.OpaqueSplitter();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // tvClass
            // 
            tvClass.AllowDrop = true;
            tvClass.DataFormat = "HierArch Class Data";
            tvClass.Dock = System.Windows.Forms.DockStyle.Top;
            tvClass.HideSelection = false;
            tvClass.ItemHeight = 16;
            tvClass.LabelEdit = true;
            tvClass.Name = "tvClass";
            tvClass.Size = new System.Drawing.Size(176, 184);
            tvClass.TabIndex = 0;
            // 
            // tvFunc
            // 
            tvFunc.AllowDrop = true;
            tvFunc.DataFormat = "HierArch Function Data";
            tvFunc.Dock = System.Windows.Forms.DockStyle.Fill;
            tvFunc.HideSelection = false;
            tvFunc.ItemHeight = 16;
            tvFunc.LabelEdit = true;
            tvFunc.Name = "tvFunc";
            tvFunc.Size = new System.Drawing.Size(176, 301);
            tvFunc.TabIndex = 0;
            // 
            // txtSource
            // 
            txtSource.AcceptsTab = true;
            txtSource.Code = "";
            txtSource.Dock = System.Windows.Forms.DockStyle.Fill;
            txtSource.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F);
            txtSource.HideSelection = false;
            txtSource.Location = new System.Drawing.Point(179, 0);
            txtSource.Name = "txtSource";
            txtSource.ScrollBars = ScrollBars.Both;
            txtSource.Size = new System.Drawing.Size(402, 488);
            txtSource.TabIndex = 6;
            txtSource.Text = "";
            txtSource.WordWrap = false;
            // 
            // panel1
            // 
            panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                 tvFunc,
                                                                                 opaqueSplitter3,
                                                                                 tvClass});
            panel1.Dock = System.Windows.Forms.DockStyle.Left;
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(200, 488);
            panel1.TabIndex = 1;
            // 
            // opaqueSplitter3
            // 
            opaqueSplitter3.Dock = System.Windows.Forms.DockStyle.Top;
            opaqueSplitter3.Location = new System.Drawing.Point(0, 184);
            opaqueSplitter3.Name = "opaqueSplitter3";
            opaqueSplitter3.Opaque = true;
            opaqueSplitter3.Size = new System.Drawing.Size(176, 3);
            opaqueSplitter3.TabIndex = 1;
            opaqueSplitter3.TabStop = false;
            // 
            // opaqueSplitter1
            // 
            opaqueSplitter1.Location = new System.Drawing.Point(176, 0);
            opaqueSplitter1.Name = "opaqueSplitter1";
            opaqueSplitter1.Opaque = true;
            opaqueSplitter1.Size = new System.Drawing.Size(3, 488);
            opaqueSplitter1.TabIndex = 3;
            opaqueSplitter1.TabStop = false;
            // 
            // View
            // 
            Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          txtSource,
                                                                          opaqueSplitter1,
                                                                          panel1});
            Name = "View";
            Size = new System.Drawing.Size(760, 488);
            panel1.ResumeLayout(false);
            ResumeLayout(false);

        }
        #endregion

        private bool IgnoreChanged = false;

        protected virtual void OnChanged(object sender, System.EventArgs e)
        {
            if (IgnoreChanged)
            {
                return;
            }

            Changed?.Invoke(sender, e);
            if (sender != tvClass)
            {
                IgnoreChanged = true;
                tvClass.OnChanged(sender, e);
                IgnoreChanged = false;
            }
        }

        public void SetDocument(Document doc)
        {
            IgnoreChanged = true;

            // TODO: Document を View に反映するための処理を追加します。

            IgnoreChanged = false;
        }

        private DnDTreeView m_tvTarget = null;

        private void TreeView_Enter(object sender, System.EventArgs e)
        {
            m_tvTarget = (DnDTreeView)sender;
        }
    }
}
