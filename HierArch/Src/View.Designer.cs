using System;

namespace Girl.HierArch
{
    partial class View
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tvClass = new Girl.HierArch.HAClass();
            this.tvFunc = new Girl.HierArch.HAFunc();
            this.txtSource = new Girl.Windows.Forms.CodeEditor();
            this.panel1 = new System.Windows.Forms.Panel();
            this.opaqueSplitter3 = new Girl.Windows.Forms.OpaqueSplitter();
            this.opaqueSplitter1 = new Girl.Windows.Forms.OpaqueSplitter();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvClass
            // 
            this.tvClass.AllowDrop = true;
            this.tvClass.DataFormat = "HierArch Class Data";
            this.tvClass.Dock = System.Windows.Forms.DockStyle.Top;
            this.tvClass.HideSelection = false;
            this.tvClass.ImageIndex = 0;
            this.tvClass.ItemHeight = 16;
            this.tvClass.LabelEdit = true;
            this.tvClass.Location = new System.Drawing.Point(0, 0);
            this.tvClass.Name = "tvClass";
            this.tvClass.SelectedImageIndex = 0;
            this.tvClass.Size = new System.Drawing.Size(200, 184);
            this.tvClass.TabIndex = 0;
            // 
            // tvFunc
            // 
            this.tvFunc.AllowDrop = true;
            this.tvFunc.DataFormat = "HierArch Function Data";
            this.tvFunc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFunc.HideSelection = false;
            this.tvFunc.ImageIndex = 0;
            this.tvFunc.ItemHeight = 16;
            this.tvFunc.LabelEdit = true;
            this.tvFunc.Location = new System.Drawing.Point(0, 187);
            this.tvFunc.Name = "tvFunc";
            this.tvFunc.SelectedImageIndex = 0;
            this.tvFunc.Size = new System.Drawing.Size(200, 301);
            this.tvFunc.TabIndex = 0;
            // 
            // txtSource
            // 
            this.txtSource.AcceptsTab = true;
            this.txtSource.Code = "";
            this.txtSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSource.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F);
            this.txtSource.HideSelection = false;
            this.txtSource.Location = new System.Drawing.Point(203, 0);
            this.txtSource.Multiline = true;
            this.txtSource.Name = "txtSource";
            this.txtSource.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSource.Size = new System.Drawing.Size(557, 488);
            this.txtSource.TabIndex = 6;
            this.txtSource.WordWrap = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tvFunc);
            this.panel1.Controls.Add(this.opaqueSplitter3);
            this.panel1.Controls.Add(this.tvClass);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 488);
            this.panel1.TabIndex = 1;
            // 
            // opaqueSplitter3
            // 
            this.opaqueSplitter3.Dock = System.Windows.Forms.DockStyle.Top;
            this.opaqueSplitter3.Location = new System.Drawing.Point(0, 184);
            this.opaqueSplitter3.Name = "opaqueSplitter3";
            this.opaqueSplitter3.Opaque = true;
            this.opaqueSplitter3.Size = new System.Drawing.Size(200, 3);
            this.opaqueSplitter3.TabIndex = 1;
            this.opaqueSplitter3.TabStop = false;
            // 
            // opaqueSplitter1
            // 
            this.opaqueSplitter1.Location = new System.Drawing.Point(200, 0);
            this.opaqueSplitter1.Name = "opaqueSplitter1";
            this.opaqueSplitter1.Opaque = true;
            this.opaqueSplitter1.Size = new System.Drawing.Size(3, 488);
            this.opaqueSplitter1.TabIndex = 3;
            this.opaqueSplitter1.TabStop = false;
            // 
            // View
            // 
            this.Controls.Add(this.txtSource);
            this.Controls.Add(this.opaqueSplitter1);
            this.Controls.Add(this.panel1);
            this.Name = "View";
            this.Size = new System.Drawing.Size(760, 488);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Panel panel1;
        public event EventHandler Changed;
        private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter1;
        private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter3;
        public HAClass tvClass;
        private HAFunc tvFunc;
        public Girl.Windows.Forms.CodeEditor txtSource;
    }
}
