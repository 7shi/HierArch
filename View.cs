using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Girl.Windows.Forms;

namespace Girl.HierarchyArchitect
{
	/// <summary>
	/// Document に対応する View です。
	/// </summary>
	public class View : System.Windows.Forms.UserControl
	{
		public event EventHandler Changed;

		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panel1;
		private HAFunc tvFunc;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter2;
		public HAClass tvClass;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter3;
		private System.Windows.Forms.Panel panel2;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter4;
		private System.Windows.Forms.TextBox txtSource;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.TextBox txtComment;
		private HAMember tvMember;
		private HAObject tvObject;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter1;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter6;

		public View()
		{
			// この呼び出しは、Windows.Forms フォーム デザイナで必要です。
			InitializeComponent();

			// TODO: InitForm を呼び出しの後に初期化処理を追加します。
			this.tvClass.MemberTreeView = this.tvMember;
			this.tvClass.FuncTreeView   = this.tvFunc;
			this.tvFunc.ObjectTreeView  = this.tvObject;
			this.tvFunc.CommentTextBox  = this.txtComment;
            this.tvFunc.SourceTextBox   = this.txtSource;
			tvClass.SetView();
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
			this.opaqueSplitter3 = new Girl.Windows.Forms.OpaqueSplitter();
			this.txtSource = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tvFunc = new Girl.HierarchyArchitect.HAFunc();
			this.opaqueSplitter2 = new Girl.Windows.Forms.OpaqueSplitter();
			this.tvClass = new Girl.HierarchyArchitect.HAClass();
			this.panel2 = new System.Windows.Forms.Panel();
			this.tvObject = new Girl.HierarchyArchitect.HAObject();
			this.opaqueSplitter1 = new Girl.Windows.Forms.OpaqueSplitter();
			this.tvMember = new Girl.HierarchyArchitect.HAMember();
			this.opaqueSplitter4 = new Girl.Windows.Forms.OpaqueSplitter();
			this.panel3 = new System.Windows.Forms.Panel();
			this.opaqueSplitter6 = new Girl.Windows.Forms.OpaqueSplitter();
			this.txtComment = new System.Windows.Forms.TextBox();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// opaqueSplitter3
			// 
			this.opaqueSplitter3.Location = new System.Drawing.Point(152, 0);
			this.opaqueSplitter3.Name = "opaqueSplitter3";
			this.opaqueSplitter3.Opaque = true;
			this.opaqueSplitter3.Size = new System.Drawing.Size(3, 312);
			this.opaqueSplitter3.TabIndex = 1;
			this.opaqueSplitter3.TabStop = false;
			// 
			// txtSource
			// 
			this.txtSource.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtSource.Location = new System.Drawing.Point(0, 123);
			this.txtSource.Multiline = true;
			this.txtSource.Name = "txtSource";
			this.txtSource.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtSource.Size = new System.Drawing.Size(234, 189);
			this.txtSource.TabIndex = 0;
			this.txtSource.Text = "";
			this.txtSource.WordWrap = false;
			// 
			// panel1
			// 
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.tvFunc,
																				 this.opaqueSplitter2,
																				 this.tvClass});
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(152, 312);
			this.panel1.TabIndex = 3;
			// 
			// tvFunc
			// 
			this.tvFunc.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvFunc.ImageIndex = -1;
			this.tvFunc.ItemHeight = 14;
			this.tvFunc.Location = new System.Drawing.Point(0, 155);
			this.tvFunc.Name = "tvFunc";
			this.tvFunc.SelectedImageIndex = -1;
			this.tvFunc.Size = new System.Drawing.Size(152, 157);
			this.tvFunc.TabIndex = 4;
			this.tvFunc.Enter += new System.EventHandler(this.TreeView_Enter);
			// 
			// opaqueSplitter2
			// 
			this.opaqueSplitter2.Dock = System.Windows.Forms.DockStyle.Top;
			this.opaqueSplitter2.Location = new System.Drawing.Point(0, 152);
			this.opaqueSplitter2.Name = "opaqueSplitter2";
			this.opaqueSplitter2.Opaque = true;
			this.opaqueSplitter2.Size = new System.Drawing.Size(152, 3);
			this.opaqueSplitter2.TabIndex = 5;
			this.opaqueSplitter2.TabStop = false;
			// 
			// tvClass
			// 
			this.tvClass.Dock = System.Windows.Forms.DockStyle.Top;
			this.tvClass.ImageIndex = -1;
			this.tvClass.ItemHeight = 14;
			this.tvClass.Name = "tvClass";
			this.tvClass.SelectedImageIndex = -1;
			this.tvClass.Size = new System.Drawing.Size(152, 152);
			this.tvClass.TabIndex = 3;
			this.tvClass.Enter += new System.EventHandler(this.TreeView_Enter);
			// 
			// panel2
			// 
			this.panel2.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.tvObject,
																				 this.opaqueSplitter1,
																				 this.tvMember});
			this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel2.Location = new System.Drawing.Point(392, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(152, 312);
			this.panel2.TabIndex = 4;
			// 
			// tvObject
			// 
			this.tvObject.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvObject.ImageIndex = -1;
			this.tvObject.ItemHeight = 14;
			this.tvObject.Location = new System.Drawing.Point(0, 211);
			this.tvObject.Name = "tvObject";
			this.tvObject.SelectedImageIndex = -1;
			this.tvObject.Size = new System.Drawing.Size(152, 101);
			this.tvObject.TabIndex = 1;
			// 
			// opaqueSplitter1
			// 
			this.opaqueSplitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.opaqueSplitter1.Location = new System.Drawing.Point(0, 208);
			this.opaqueSplitter1.Name = "opaqueSplitter1";
			this.opaqueSplitter1.Opaque = true;
			this.opaqueSplitter1.Size = new System.Drawing.Size(152, 3);
			this.opaqueSplitter1.TabIndex = 3;
			this.opaqueSplitter1.TabStop = false;
			// 
			// tvMember
			// 
			this.tvMember.Dock = System.Windows.Forms.DockStyle.Top;
			this.tvMember.ImageIndex = -1;
			this.tvMember.ItemHeight = 14;
			this.tvMember.Name = "tvMember";
			this.tvMember.SelectedImageIndex = -1;
			this.tvMember.Size = new System.Drawing.Size(152, 208);
			this.tvMember.TabIndex = 0;
			// 
			// opaqueSplitter4
			// 
			this.opaqueSplitter4.Dock = System.Windows.Forms.DockStyle.Right;
			this.opaqueSplitter4.Location = new System.Drawing.Point(389, 0);
			this.opaqueSplitter4.Name = "opaqueSplitter4";
			this.opaqueSplitter4.Opaque = true;
			this.opaqueSplitter4.Size = new System.Drawing.Size(3, 312);
			this.opaqueSplitter4.TabIndex = 5;
			this.opaqueSplitter4.TabStop = false;
			// 
			// panel3
			// 
			this.panel3.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.txtSource,
																				 this.opaqueSplitter6,
																				 this.txtComment});
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(155, 0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(234, 312);
			this.panel3.TabIndex = 6;
			// 
			// opaqueSplitter6
			// 
			this.opaqueSplitter6.Dock = System.Windows.Forms.DockStyle.Top;
			this.opaqueSplitter6.Location = new System.Drawing.Point(0, 120);
			this.opaqueSplitter6.Name = "opaqueSplitter6";
			this.opaqueSplitter6.Opaque = true;
			this.opaqueSplitter6.Size = new System.Drawing.Size(234, 3);
			this.opaqueSplitter6.TabIndex = 2;
			this.opaqueSplitter6.TabStop = false;
			// 
			// txtComment
			// 
			this.txtComment.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtComment.Multiline = true;
			this.txtComment.Name = "txtComment";
			this.txtComment.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtComment.Size = new System.Drawing.Size(234, 120);
			this.txtComment.TabIndex = 1;
			this.txtComment.Text = "";
			this.txtComment.WordWrap = false;
			// 
			// View
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.panel3,
																		  this.opaqueSplitter4,
																		  this.panel2,
																		  this.opaqueSplitter3,
																		  this.panel1});
			this.Name = "View";
			this.Size = new System.Drawing.Size(544, 312);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private bool m_bIgnoreChanged = false;

		protected virtual void OnChanged(object sender, System.EventArgs e)
		{
			if (!m_bIgnoreChanged && Changed != null) Changed(sender, e);
		}

		public void SetDocument(Document doc)
		{
			m_bIgnoreChanged = true;

			// TODO: Document を View に反映するための処理を追加します。

			m_bIgnoreChanged = false;
		}

		private DnDTreeView m_tvTarget = null;

		private void TreeView_Enter(object sender, System.EventArgs e)
		{
			m_tvTarget = (DnDTreeView)sender;
		}
	}
}
