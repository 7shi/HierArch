using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Girl.Windows.Forms;

namespace Girl.HierArch
{
	/// <summary>
	/// Document に対応する View です。
	/// </summary>
	public class View : System.Windows.Forms.UserControl
	{
		public event EventHandler Changed;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter1;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter2;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter3;
		public Girl.Windows.Forms.OpaqueSplitter opaqueSplitter4;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter5;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter6;
		public System.Windows.Forms.TabControl tabClass;
		public System.Windows.Forms.TabControl tabFunc;
		public System.Windows.Forms.TabControl tabMember;
		public System.Windows.Forms.TabControl tabArg;
		public System.Windows.Forms.TabControl tabObject;
		private System.Windows.Forms.TabPage tpClass;
		private System.Windows.Forms.TabPage tpFunc;
		private System.Windows.Forms.TabPage tpMember;
		private System.Windows.Forms.TabPage tpArg;
		private System.Windows.Forms.TabPage tpObject;
		public HAClass tvClass;
		private HAFunc tvFunc;
		private HAMember tvMember;
		private HAObject tvArg;
		private HAObject tvObject;
		private Girl.Windows.Forms.RichTextToolBar richTextToolBar1;
		public Girl.Windows.Forms.ExRichTextBox txtComment;
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
			this.tvClass.MemberTreeView = this.tvMember;
			this.tvClass.FuncTreeView   = this.tvFunc;
			this.tvFunc .ArgTreeView    = this.tvArg;
			this.tvFunc .ObjectTreeView = this.tvObject;
			this.tvFunc .CommentTextBox = this.txtComment;
			this.tvFunc .SourceTextBox  = this.txtSource;

			this.tvMember.MoveTarget.Add(this.tvArg);
			this.tvMember.MoveTarget.Add(this.tvObject);
			this.tvArg   .MoveTarget.Add(this.tvMember);
			this.tvArg   .MoveTarget.Add(this.tvObject);
			this.tvObject.MoveTarget.Add(this.tvMember);
			this.tvObject.MoveTarget.Add(this.tvArg);

			this.tvClass.SetView();

			this.tvClass .Changed += new EventHandler(this.OnChanged);
			this.tvFunc  .Changed += new EventHandler(this.OnChanged);
			this.tvMember.Changed += new EventHandler(this.OnChanged);
			this.tvArg   .Changed += new EventHandler(this.OnChanged);
			this.tvObject.Changed += new EventHandler(this.OnChanged);

			this.txtComment.TextChanged += new EventHandler(this.tvFunc.OnChanged);
			this.txtSource .TextChanged += new EventHandler(this.tvFunc.OnChanged);

			this.richTextToolBar1.AddTarget(this.txtComment);
			this.richTextToolBar1.AddTarget(this.txtSource );
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
			this.tvMember = new Girl.HierArch.HAMember();
			this.tvArg = new Girl.HierArch.HAObject();
			this.tvObject = new Girl.HierArch.HAObject();
			this.panel2 = new System.Windows.Forms.Panel();
			this.txtSource = new Girl.Windows.Forms.CodeEditor();
			this.opaqueSplitter4 = new Girl.Windows.Forms.OpaqueSplitter();
			this.txtComment = new Girl.Windows.Forms.ExRichTextBox();
			this.richTextToolBar1 = new Girl.Windows.Forms.RichTextToolBar();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tabFunc = new System.Windows.Forms.TabControl();
			this.tpFunc = new System.Windows.Forms.TabPage();
			this.opaqueSplitter3 = new Girl.Windows.Forms.OpaqueSplitter();
			this.tabClass = new System.Windows.Forms.TabControl();
			this.tpClass = new System.Windows.Forms.TabPage();
			this.panel3 = new System.Windows.Forms.Panel();
			this.tabObject = new System.Windows.Forms.TabControl();
			this.tpObject = new System.Windows.Forms.TabPage();
			this.opaqueSplitter6 = new Girl.Windows.Forms.OpaqueSplitter();
			this.tabArg = new System.Windows.Forms.TabControl();
			this.tpArg = new System.Windows.Forms.TabPage();
			this.opaqueSplitter5 = new Girl.Windows.Forms.OpaqueSplitter();
			this.tabMember = new System.Windows.Forms.TabControl();
			this.tpMember = new System.Windows.Forms.TabPage();
			this.opaqueSplitter1 = new Girl.Windows.Forms.OpaqueSplitter();
			this.opaqueSplitter2 = new Girl.Windows.Forms.OpaqueSplitter();
			this.panel2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tabFunc.SuspendLayout();
			this.tpFunc.SuspendLayout();
			this.tabClass.SuspendLayout();
			this.tpClass.SuspendLayout();
			this.panel3.SuspendLayout();
			this.tabObject.SuspendLayout();
			this.tpObject.SuspendLayout();
			this.tabArg.SuspendLayout();
			this.tpArg.SuspendLayout();
			this.tabMember.SuspendLayout();
			this.tpMember.SuspendLayout();
			this.SuspendLayout();
			// 
			// tvClass
			// 
			this.tvClass.AllowDrop = true;
			this.tvClass.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvClass.HideSelection = false;
			this.tvClass.ItemHeight = 16;
			this.tvClass.LabelEdit = true;
			this.tvClass.Name = "tvClass";
			this.tvClass.Size = new System.Drawing.Size(168, 159);
			this.tvClass.TabIndex = 0;
			// 
			// tvFunc
			// 
			this.tvFunc.AllowDrop = true;
			this.tvFunc.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvFunc.HideSelection = false;
			this.tvFunc.ItemHeight = 16;
			this.tvFunc.LabelEdit = true;
			this.tvFunc.Name = "tvFunc";
			this.tvFunc.Size = new System.Drawing.Size(168, 276);
			this.tvFunc.TabIndex = 0;
			// 
			// tvMember
			// 
			this.tvMember.AllowDrop = true;
			this.tvMember.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvMember.HideSelection = false;
			this.tvMember.ItemHeight = 16;
			this.tvMember.LabelEdit = true;
			this.tvMember.Name = "tvMember";
			this.tvMember.Size = new System.Drawing.Size(168, 135);
			this.tvMember.TabIndex = 0;
			// 
			// tvArg
			// 
			this.tvArg.AllowDrop = true;
			this.tvArg.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvArg.HideSelection = false;
			this.tvArg.ItemHeight = 16;
			this.tvArg.LabelEdit = true;
			this.tvArg.Name = "tvArg";
			this.tvArg.Size = new System.Drawing.Size(168, 84);
			this.tvArg.TabIndex = 0;
			// 
			// tvObject
			// 
			this.tvObject.AllowDrop = true;
			this.tvObject.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvObject.HideSelection = false;
			this.tvObject.ItemHeight = 16;
			this.tvObject.LabelEdit = true;
			this.tvObject.Name = "tvObject";
			this.tvObject.Size = new System.Drawing.Size(168, 188);
			this.tvObject.TabIndex = 0;
			// 
			// panel2
			// 
			this.panel2.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.txtSource,
																				 this.opaqueSplitter4,
																				 this.txtComment,
																				 this.richTextToolBar1});
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(179, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(402, 488);
			this.panel2.TabIndex = 0;
			// 
			// txtSource
			// 
			this.txtSource.AcceptsTab = true;
			this.txtSource.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtSource.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F);
			this.txtSource.HideSelection = false;
			this.txtSource.Location = new System.Drawing.Point(0, 139);
			this.txtSource.Name = "txtSource";
			this.txtSource.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
			this.txtSource.Size = new System.Drawing.Size(402, 349);
			this.txtSource.TabIndex = 6;
			this.txtSource.Text = "";
			this.txtSource.WordWrap = false;
			// 
			// opaqueSplitter4
			// 
			this.opaqueSplitter4.Dock = System.Windows.Forms.DockStyle.Top;
			this.opaqueSplitter4.Location = new System.Drawing.Point(0, 136);
			this.opaqueSplitter4.Name = "opaqueSplitter4";
			this.opaqueSplitter4.Opaque = true;
			this.opaqueSplitter4.Size = new System.Drawing.Size(402, 3);
			this.opaqueSplitter4.TabIndex = 3;
			this.opaqueSplitter4.TabStop = false;
			// 
			// txtComment
			// 
			this.txtComment.AcceptsTab = true;
			this.txtComment.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtComment.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.txtComment.HideSelection = false;
			this.txtComment.Location = new System.Drawing.Point(0, 25);
			this.txtComment.Name = "txtComment";
			this.txtComment.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
			this.txtComment.Size = new System.Drawing.Size(402, 111);
			this.txtComment.TabIndex = 5;
			this.txtComment.Text = "";
			this.txtComment.WordWrap = false;
			// 
			// richTextToolBar1
			// 
			this.richTextToolBar1.Divider = true;
			this.richTextToolBar1.Dock = System.Windows.Forms.DockStyle.Top;
			this.richTextToolBar1.Name = "richTextToolBar1";
			this.richTextToolBar1.Size = new System.Drawing.Size(402, 25);
			this.richTextToolBar1.TabIndex = 4;
			this.richTextToolBar1.TabStop = false;
			this.richTextToolBar1.Target = null;
			// 
			// panel1
			// 
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.tabFunc,
																				 this.opaqueSplitter3,
																				 this.tabClass});
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(176, 488);
			this.panel1.TabIndex = 1;
			// 
			// tabFunc
			// 
			this.tabFunc.Controls.AddRange(new System.Windows.Forms.Control[] {
																				  this.tpFunc});
			this.tabFunc.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabFunc.Location = new System.Drawing.Point(0, 187);
			this.tabFunc.Name = "tabFunc";
			this.tabFunc.SelectedIndex = 0;
			this.tabFunc.Size = new System.Drawing.Size(176, 301);
			this.tabFunc.TabIndex = 2;
			// 
			// tpFunc
			// 
			this.tpFunc.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.tvFunc});
			this.tpFunc.Location = new System.Drawing.Point(4, 21);
			this.tpFunc.Name = "tpFunc";
			this.tpFunc.Size = new System.Drawing.Size(168, 276);
			this.tpFunc.TabIndex = 0;
			this.tpFunc.Text = "関数";
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
			// tabClass
			// 
			this.tabClass.Controls.AddRange(new System.Windows.Forms.Control[] {
																				   this.tpClass});
			this.tabClass.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabClass.Name = "tabClass";
			this.tabClass.SelectedIndex = 0;
			this.tabClass.Size = new System.Drawing.Size(176, 184);
			this.tabClass.TabIndex = 0;
			// 
			// tpClass
			// 
			this.tpClass.Controls.AddRange(new System.Windows.Forms.Control[] {
																				  this.tvClass});
			this.tpClass.Location = new System.Drawing.Point(4, 21);
			this.tpClass.Name = "tpClass";
			this.tpClass.Size = new System.Drawing.Size(168, 159);
			this.tpClass.TabIndex = 0;
			this.tpClass.Text = "クラス";
			// 
			// panel3
			// 
			this.panel3.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.tabObject,
																				 this.opaqueSplitter6,
																				 this.tabArg,
																				 this.opaqueSplitter5,
																				 this.tabMember});
			this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel3.Location = new System.Drawing.Point(584, 0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(176, 488);
			this.panel3.TabIndex = 2;
			// 
			// tabObject
			// 
			this.tabObject.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.tpObject});
			this.tabObject.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabObject.Location = new System.Drawing.Point(0, 275);
			this.tabObject.Name = "tabObject";
			this.tabObject.SelectedIndex = 0;
			this.tabObject.Size = new System.Drawing.Size(176, 213);
			this.tabObject.TabIndex = 4;
			// 
			// tpObject
			// 
			this.tpObject.Controls.AddRange(new System.Windows.Forms.Control[] {
																				   this.tvObject});
			this.tpObject.Location = new System.Drawing.Point(4, 21);
			this.tpObject.Name = "tpObject";
			this.tpObject.Size = new System.Drawing.Size(168, 188);
			this.tpObject.TabIndex = 0;
			this.tpObject.Text = "変数";
			// 
			// opaqueSplitter6
			// 
			this.opaqueSplitter6.Dock = System.Windows.Forms.DockStyle.Top;
			this.opaqueSplitter6.Location = new System.Drawing.Point(0, 272);
			this.opaqueSplitter6.Name = "opaqueSplitter6";
			this.opaqueSplitter6.Opaque = true;
			this.opaqueSplitter6.Size = new System.Drawing.Size(176, 3);
			this.opaqueSplitter6.TabIndex = 3;
			this.opaqueSplitter6.TabStop = false;
			// 
			// tabArg
			// 
			this.tabArg.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.tpArg});
			this.tabArg.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabArg.Location = new System.Drawing.Point(0, 163);
			this.tabArg.Name = "tabArg";
			this.tabArg.SelectedIndex = 0;
			this.tabArg.Size = new System.Drawing.Size(176, 109);
			this.tabArg.TabIndex = 2;
			// 
			// tpArg
			// 
			this.tpArg.Controls.AddRange(new System.Windows.Forms.Control[] {
																				this.tvArg});
			this.tpArg.Location = new System.Drawing.Point(4, 21);
			this.tpArg.Name = "tpArg";
			this.tpArg.Size = new System.Drawing.Size(168, 84);
			this.tpArg.TabIndex = 0;
			this.tpArg.Text = "引数";
			// 
			// opaqueSplitter5
			// 
			this.opaqueSplitter5.Dock = System.Windows.Forms.DockStyle.Top;
			this.opaqueSplitter5.Location = new System.Drawing.Point(0, 160);
			this.opaqueSplitter5.Name = "opaqueSplitter5";
			this.opaqueSplitter5.Opaque = true;
			this.opaqueSplitter5.Size = new System.Drawing.Size(176, 3);
			this.opaqueSplitter5.TabIndex = 1;
			this.opaqueSplitter5.TabStop = false;
			// 
			// tabMember
			// 
			this.tabMember.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.tpMember});
			this.tabMember.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabMember.Name = "tabMember";
			this.tabMember.SelectedIndex = 0;
			this.tabMember.Size = new System.Drawing.Size(176, 160);
			this.tabMember.TabIndex = 0;
			// 
			// tpMember
			// 
			this.tpMember.Controls.AddRange(new System.Windows.Forms.Control[] {
																				   this.tvMember});
			this.tpMember.Location = new System.Drawing.Point(4, 21);
			this.tpMember.Name = "tpMember";
			this.tpMember.Size = new System.Drawing.Size(168, 135);
			this.tpMember.TabIndex = 0;
			this.tpMember.Text = "メンバ";
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
			// opaqueSplitter2
			// 
			this.opaqueSplitter2.Dock = System.Windows.Forms.DockStyle.Right;
			this.opaqueSplitter2.Location = new System.Drawing.Point(581, 0);
			this.opaqueSplitter2.Name = "opaqueSplitter2";
			this.opaqueSplitter2.Opaque = true;
			this.opaqueSplitter2.Size = new System.Drawing.Size(3, 488);
			this.opaqueSplitter2.TabIndex = 4;
			this.opaqueSplitter2.TabStop = false;
			// 
			// View
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.panel2,
																		  this.opaqueSplitter2,
																		  this.opaqueSplitter1,
																		  this.panel1,
																		  this.panel3});
			this.Name = "View";
			this.Size = new System.Drawing.Size(760, 488);
			this.panel2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.tabFunc.ResumeLayout(false);
			this.tpFunc.ResumeLayout(false);
			this.tabClass.ResumeLayout(false);
			this.tpClass.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.tabObject.ResumeLayout(false);
			this.tpObject.ResumeLayout(false);
			this.tabArg.ResumeLayout(false);
			this.tpArg.ResumeLayout(false);
			this.tabMember.ResumeLayout(false);
			this.tpMember.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Visibility

		public void SetPanel1(bool vc, bool vf)
		{
			this.panel1.Visible = this.opaqueSplitter1.Visible = (vc || vf);
			if (!vc && !vf) return;

			this.tabClass.Dock = (vc && vf) ? DockStyle.Top : DockStyle.Fill;
			this.tabClass.Visible = vc;
			this.opaqueSplitter3.Visible = (vc && vf);
			this.tabFunc.Visible = vf;
		}

		public void SetPanel3(bool vm, bool va, bool vo)
		{
			this.panel3.Visible = this.opaqueSplitter2.Visible = (vm || va || vo);
			if (!vm && !va && !vo) return;

			this.tabMember.Dock = (vm && (va || vo)) ? DockStyle.Top : DockStyle.Fill;
			this.tabArg   .Dock =        (va && vo)  ? DockStyle.Top : DockStyle.Fill;

			this.tabMember.Visible = vm;
			this.opaqueSplitter5.Visible = (vm && (va || vo));
			this.tabArg.Visible = va;
			this.opaqueSplitter6.Visible = (va && vo);
			this.tabObject.Visible = vo;
		}

		#endregion

		private bool IgnoreChanged = false;

		protected virtual void OnChanged(object sender, System.EventArgs e)
		{
			if (!this.IgnoreChanged && this.Changed != null) Changed(sender, e);
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

		#region Generation

		public void GenerateAll(string path)
		{
			this.tvClass.Generate(path);
		}

		#endregion
	}
}
