using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Girl.Coding;
using Girl.Windows.Forms;

namespace Girl.HierArch.PluginEditor
{
	/// <summary>
	/// Document に対応する View です。
	/// </summary>
	public class View : System.Windows.Forms.UserControl
	{
		public Girl.Windows.Forms.CodeEditor codeEditor;
		public Girl.Windows.Forms.LinkRichTextBox logViewer;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter1;

		public event EventHandler Changed;

		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public View()
		{
			// この呼び出しは、Windows.Forms フォーム デザイナで必要です。
			InitializeComponent();

			// TODO: InitializeComponent を呼び出しの後に初期化処理を追加します。
			this.codeEditor.Parser = new CSharpParser();
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		private bool ignoreChanged = false;
		protected virtual void OnChanged(object sender, System.EventArgs e)
		{
			if(!ignoreChanged && Changed != null) Changed(sender, e);
		}

		#region Component Designer generated code
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.codeEditor = new Girl.Windows.Forms.CodeEditor();
			this.logViewer = new Girl.Windows.Forms.LinkRichTextBox();
			this.opaqueSplitter1 = new Girl.Windows.Forms.OpaqueSplitter();
			this.SuspendLayout();
			// 
			// codeEditor
			// 
			this.codeEditor.AcceptsTab = true;
			this.codeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.codeEditor.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F);
			this.codeEditor.HideSelection = false;
			this.codeEditor.Name = "codeEditor";
			this.codeEditor.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
			this.codeEditor.Size = new System.Drawing.Size(360, 205);
			this.codeEditor.TabIndex = 0;
			this.codeEditor.Text = "";
			this.codeEditor.WordWrap = false;
			this.codeEditor.TextChanged += new System.EventHandler(this.codeEditor_TextChanged);
			// 
			// logViewer
			// 
			this.logViewer.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.logViewer.HideSelection = false;
			this.logViewer.LinkColor = System.Drawing.Color.Blue;
			this.logViewer.Location = new System.Drawing.Point(0, 208);
			this.logViewer.Name = "logViewer";
			this.logViewer.ReadOnly = true;
			this.logViewer.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
			this.logViewer.Size = new System.Drawing.Size(360, 112);
			this.logViewer.TabIndex = 1;
			this.logViewer.Text = "";
			this.logViewer.WordWrap = false;
			this.logViewer.LinkTargetClicked += new Girl.Windows.Forms.LinkRichTextBox.LinkTargetEventHandler(this.logViewer_LinkTargetClicked);
			// 
			// opaqueSplitter1
			// 
			this.opaqueSplitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.opaqueSplitter1.Location = new System.Drawing.Point(0, 205);
			this.opaqueSplitter1.Name = "opaqueSplitter1";
			this.opaqueSplitter1.Opaque = true;
			this.opaqueSplitter1.Size = new System.Drawing.Size(360, 3);
			this.opaqueSplitter1.TabIndex = 2;
			this.opaqueSplitter1.TabStop = false;
			// 
			// View
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.codeEditor,
																		  this.opaqueSplitter1,
																		  this.logViewer});
			this.Name = "View";
			this.Size = new System.Drawing.Size(360, 320);
			this.ResumeLayout(false);

		}
		#endregion

		public void SetDocument(PluginEditorDoc doc)
		{
			Cursor cur = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			this.ignoreChanged = true;

			// TODO: Document を View に反映するための処理を追加します。
			this.codeEditor.Code = doc.Text;

			this.ignoreChanged = false;
			Cursor.Current = cur;
		}

		private void codeEditor_TextChanged(object sender, System.EventArgs e)
		{
			this.OnChanged(this, e);
		}

		private void logViewer_LinkTargetClicked(object sender, Girl.Windows.Forms.LinkRichTextBox.LinkTargetEventArgs e)
		{
			this.codeEditor.JumpToError((Point)e.Target);
		}

		public void Recolor()
		{
			this.ignoreChanged = true;
			Cursor cur = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			int pos = this.codeEditor.SelectionStart;
			this.codeEditor.Code = this.codeEditor.Code;
			this.codeEditor.SelectionStart = pos;
			Cursor.Current = cur;
			this.codeEditor.Focus();
			this.ignoreChanged = false;
		}
	}
}
