using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Crownwood.Magic.Common;
using Crownwood.Magic.Controls;
using Crownwood.Magic.Docking;
using Girl.Windows.Forms;

namespace Girl.HierarchyArchitect
{
	/// <summary>
	/// Document に対応する View です。
	/// </summary>
	public class View : System.Windows.Forms.UserControl
	{
		public event EventHandler Changed;
		private Crownwood.Magic.Controls.TabbedGroups tabbedGroups1;

		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public View()
		{
			// この呼び出しは、Windows.Forms フォーム デザイナで必要です。
			InitializeComponent();

			// TODO: InitForm を呼び出しの後に初期化処理を追加します。
			this.CreateDockingWindows();
			this.CreateInitialPagesGroup();

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
			this.tabbedGroups1 = new Crownwood.Magic.Controls.TabbedGroups();
			((System.ComponentModel.ISupportInitialize)(this.tabbedGroups1)).BeginInit();
			this.SuspendLayout();
			// 
			// tabbedGroups1
			// 
			this.tabbedGroups1.ActiveLeaf = null;
			this.tabbedGroups1.AllowDrop = true;
			this.tabbedGroups1.AtLeastOneLeaf = true;
			this.tabbedGroups1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabbedGroups1.Name = "tabbedGroups1";
			this.tabbedGroups1.ProminentLeaf = null;
			this.tabbedGroups1.ResizeBarColor = System.Drawing.SystemColors.Control;
			this.tabbedGroups1.Size = new System.Drawing.Size(400, 312);
			this.tabbedGroups1.TabIndex = 0;
			// 
			// View
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.tabbedGroups1});
			this.Name = "View";
			this.Size = new System.Drawing.Size(400, 312);
			((System.ComponentModel.ISupportInitialize)(this.tabbedGroups1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		#region Docking

		private DockingManager _manager;
		public Content ctClass, ctFunc, ctMember, ctArg, ctObject;
		public HAClass tvClass;
		public HAFunc tvFunc;
		public HAMember tvMember;
		public HAObject tvArg;
		public HAObject tvObject;

		private void CreateDockingWindows()
		{
			// Create the docking manager instance
			_manager = new DockingManager(this, VisualStyle.IDE);

			// Define innner/outer controls for correct docking operation
			_manager.InnerControl = this.tabbedGroups1;
			//_manager.OuterControl = this.statusBar1;
		    
			this.tvClass = new HAClass();
			this.ctClass = _manager.Contents.Add(tvClass, "クラス");
			WindowContent wcClass = _manager.AddContentWithState(ctClass, State.DockLeft);

			this.tvFunc = new HAFunc();
			this.ctFunc = _manager.Contents.Add(tvFunc, "関数");
			_manager.AddContentToZone(ctFunc, wcClass.ParentZone, 1);

			this.tvMember = new HAMember();
			this.ctMember = _manager.Contents.Add(tvMember, "メンバ");
			WindowContent wcMember = _manager.AddContentWithState(ctMember, State.DockRight);

			this.tvArg = new HAObject();
			this.ctArg = _manager.Contents.Add(tvArg, "引数");
			_manager.AddContentToZone(ctArg, wcMember.ParentZone, 1);

			this.tvObject = new HAObject();
			this.ctObject = _manager.Contents.Add(tvObject, "変数");
			_manager.AddContentToZone(ctObject, wcMember.ParentZone, 2);
		}

		public TextBox txtComment, txtSource;

		private void CreateInitialPagesGroup()
		{
			// Change direction to opposite
			tabbedGroups1.RootSequence.Direction = Direction.Vertical;

			// Access the default leaf group
			txtComment = new TextBox();
			txtComment.Multiline = true;
			txtComment.WordWrap = false;
			txtComment.ScrollBars = ScrollBars.Both;
			txtComment.HideSelection = false;
			TabGroupLeaf tgl1 = tabbedGroups1.RootSequence[0] as TabGroupLeaf;
			tgl1.TabPages.Add(new Crownwood.Magic.Controls.TabPage("コメント", txtComment));

			// Add a new leaf group in the same sequence
			txtSource = new TextBox();
			txtSource.Multiline = true;
			txtSource.WordWrap = false;
			txtSource.ScrollBars = ScrollBars.Both;
			txtSource.HideSelection = false;
			TabGroupLeaf tgl2 = tabbedGroups1.RootSequence.AddNewLeaf();
			tgl2.TabPages.Add(new Crownwood.Magic.Controls.TabPage("ソース", txtSource));
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
