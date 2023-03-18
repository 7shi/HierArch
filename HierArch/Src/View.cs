using System;
using System.Windows.Forms;
using Girl.Windows.Forms;

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
		private System.ComponentModel.Container components = null;

		public View()
		{
			// この呼び出しは、Windows.Forms フォーム デザイナで必要です。
			InitializeComponent();

			// TODO: InitializeComponent を呼び出しの後に初期化処理を追加します。
			this.tvClass.FuncTreeView   = this.tvFunc;
			this.tvFunc .SourceTextBox  = this.txtSource;

			this.tvClass.SetView();

			this.tvClass .Changed += new EventHandler(this.OnChanged);
			this.tvFunc  .Changed += new EventHandler(this.OnChanged);

			this.txtSource.TextChanged += new EventHandler(this.tvFunc.OnChanged);
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
		}

		#region Component Designer generated code
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
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
			this.tvClass.ItemHeight = 16;
			this.tvClass.LabelEdit = true;
			this.tvClass.Name = "tvClass";
			this.tvClass.Size = new System.Drawing.Size(176, 184);
			this.tvClass.TabIndex = 0;
			// 
			// tvFunc
			// 
			this.tvFunc.AllowDrop = true;
			this.tvFunc.DataFormat = "HierArch Function Data";
			this.tvFunc.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvFunc.HideSelection = false;
			this.tvFunc.ItemHeight = 16;
			this.tvFunc.LabelEdit = true;
			this.tvFunc.Name = "tvFunc";
			this.tvFunc.Size = new System.Drawing.Size(176, 301);
			this.tvFunc.TabIndex = 0;
			// 
			// txtSource
			// 
			this.txtSource.AcceptsTab = true;
			this.txtSource.Code = "";
			this.txtSource.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtSource.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F);
			this.txtSource.HideSelection = false;
			this.txtSource.Location = new System.Drawing.Point(179, 0);
			this.txtSource.Name = "txtSource";
			this.txtSource.ScrollBars = ScrollBars.Both;
			this.txtSource.Size = new System.Drawing.Size(402, 488);
			this.txtSource.TabIndex = 6;
			this.txtSource.Text = "";
			this.txtSource.WordWrap = false;
			// 
			// panel1
			// 
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.tvFunc,
																				 this.opaqueSplitter3,
																				 this.tvClass});
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
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
			this.opaqueSplitter3.Size = new System.Drawing.Size(176, 3);
			this.opaqueSplitter3.TabIndex = 1;
			this.opaqueSplitter3.TabStop = false;
			// 
			// opaqueSplitter1
			// 
			this.opaqueSplitter1.Location = new System.Drawing.Point(176, 0);
			this.opaqueSplitter1.Name = "opaqueSplitter1";
			this.opaqueSplitter1.Opaque = true;
			this.opaqueSplitter1.Size = new System.Drawing.Size(3, 488);
			this.opaqueSplitter1.TabIndex = 3;
			this.opaqueSplitter1.TabStop = false;
			// 
			// View
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.txtSource,
																		  this.opaqueSplitter1,
																		  this.panel1});
			this.Name = "View";
			this.Size = new System.Drawing.Size(760, 488);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private bool IgnoreChanged = false;

		protected virtual void OnChanged(object sender, System.EventArgs e)
		{
			if (this.IgnoreChanged) return;

			if (this.Changed != null) Changed(sender, e);
			if (sender != this.tvClass)
			{
				this.IgnoreChanged = true;
				this.tvClass.OnChanged(sender, e);
				this.IgnoreChanged = false;
			}
		}

		public void SetDocument(Document doc)
		{
			this.IgnoreChanged = true;

			// TODO: Document を View に反映するための処理を追加します。

			this.IgnoreChanged = false;
		}

		private DnDTreeView m_tvTarget = null;

		private void TreeView_Enter(object sender, System.EventArgs e)
		{
			m_tvTarget = (DnDTreeView)sender;
		}

		private void propertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem.Label == "Server")
			{
				(s as PropertyGrid).Refresh();
			}
		}

		private void txtComment_LinkClicked(object sender, System.Windows.Forms.LinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(e.LinkText);
		}

		private void txtSource_LinkClicked(object sender, System.Windows.Forms.LinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(e.LinkText);
		}
	}
}
