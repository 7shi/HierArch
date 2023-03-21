namespace Girl.HierArch
{
    /// <summary>
    /// About の概要の説明です。
    /// </summary>
    public class About : System.Windows.Forms.Form
    {
        public System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private readonly System.ComponentModel.Container components = null;

        public About()
        {
            //
            // Windows フォーム デザイナ サポートに必要です。
            //
            InitializeComponent();

            //
            // TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
            //
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

        #region Windows Form Designer generated code
        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            button1 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(16, 16);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(384, 16);
            label1.TabIndex = 1;
            label1.Text = "label1";
            // 
            // label2
            // 
            label2.Location = new System.Drawing.Point(16, 40);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(384, 16);
            label2.TabIndex = 3;
            label2.Text = "このソフトウェアは CC0 で提供されます。";
            // 
            // button1
            // 
            button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            button1.Location = new System.Drawing.Point(165, 64);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(80, 24);
            button1.TabIndex = 0;
            button1.Text = "OK";
            // 
            // About
            // 
            AcceptButton = button1;
            AutoScaleBaseSize = new System.Drawing.Size(5, 12);
            ClientSize = new System.Drawing.Size(410, 103);
            Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          button1,
                                                                          label2,
                                                                          label1});
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "About";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "About...";
            ResumeLayout(false);

        }
        #endregion
    }
}
