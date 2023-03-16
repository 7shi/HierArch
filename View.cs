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
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Panel panel1;
		private Girl.Windows.Forms.DnDTreeView tvNameSpace;
		private Girl.Windows.Forms.DnDTreeView tvFunc;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter2;
		private Girl.Windows.Forms.DnDTreeView tvClass;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter3;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem cm1Child;
		private System.Windows.Forms.MenuItem cm1Append;
		private System.Windows.Forms.MenuItem cm1Insert;
		private System.Windows.Forms.MenuItem cm1Separator1;
		private System.Windows.Forms.MenuItem cm1Delete;
		private Girl.Windows.Forms.OpaqueSplitter opaqueSplitter1;

		public View()
		{
			// この呼び出しは、Windows.Forms フォーム デザイナで必要です。
			InitializeComponent();

			// TODO: InitForm を呼び出しの後に初期化処理を追加します。

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

		#region Component Designer generated code
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.tvNameSpace = new Girl.Windows.Forms.DnDTreeView();
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.cm1Child = new System.Windows.Forms.MenuItem();
			this.cm1Append = new System.Windows.Forms.MenuItem();
			this.cm1Insert = new System.Windows.Forms.MenuItem();
			this.cm1Separator1 = new System.Windows.Forms.MenuItem();
			this.cm1Delete = new System.Windows.Forms.MenuItem();
			this.opaqueSplitter3 = new Girl.Windows.Forms.OpaqueSplitter();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tvFunc = new Girl.Windows.Forms.DnDTreeView();
			this.opaqueSplitter2 = new Girl.Windows.Forms.OpaqueSplitter();
			this.tvClass = new Girl.Windows.Forms.DnDTreeView();
			this.opaqueSplitter1 = new Girl.Windows.Forms.OpaqueSplitter();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tvNameSpace
			// 
			this.tvNameSpace.AllowDrop = true;
			this.tvNameSpace.ContextMenu = this.contextMenu1;
			this.tvNameSpace.Dock = System.Windows.Forms.DockStyle.Top;
			this.tvNameSpace.ImageIndex = -1;
			this.tvNameSpace.LabelEdit = true;
			this.tvNameSpace.Name = "tvNameSpace";
			this.tvNameSpace.SelectedImageIndex = -1;
			this.tvNameSpace.Size = new System.Drawing.Size(192, 72);
			this.tvNameSpace.TabIndex = 1;
			// 
			// contextMenu1
			// 
			this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.cm1Child,
																						 this.cm1Append,
																						 this.cm1Insert,
																						 this.cm1Separator1,
																						 this.cm1Delete});
			// 
			// cm1Child
			// 
			this.cm1Child.Index = 0;
			this.cm1Child.Text = "下に追加";
			// 
			// cm1Append
			// 
			this.cm1Append.Index = 1;
			this.cm1Append.Text = "後に追加";
			// 
			// cm1Insert
			// 
			this.cm1Insert.Index = 2;
			this.cm1Insert.Text = "前に追加";
			// 
			// cm1Separator1
			// 
			this.cm1Separator1.Index = 3;
			this.cm1Separator1.Text = "-";
			// 
			// cm1Delete
			// 
			this.cm1Delete.Index = 4;
			this.cm1Delete.Text = "削除";
			// 
			// opaqueSplitter3
			// 
			this.opaqueSplitter3.Location = new System.Drawing.Point(192, 0);
			this.opaqueSplitter3.Name = "opaqueSplitter3";
			this.opaqueSplitter3.Opaque = true;
			this.opaqueSplitter3.Size = new System.Drawing.Size(3, 280);
			this.opaqueSplitter3.TabIndex = 1;
			this.opaqueSplitter3.TabStop = false;
			// 
			// textBox1
			// 
			this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox1.Location = new System.Drawing.Point(195, 0);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox1.Size = new System.Drawing.Size(205, 280);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			this.textBox1.WordWrap = false;
			// 
			// panel1
			// 
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.tvFunc,
																				 this.opaqueSplitter2,
																				 this.tvClass,
																				 this.opaqueSplitter1,
																				 this.tvNameSpace});
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(192, 280);
			this.panel1.TabIndex = 3;
			// 
			// tvFunc
			// 
			this.tvFunc.AllowDrop = true;
			this.tvFunc.ContextMenu = this.contextMenu1;
			this.tvFunc.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvFunc.ImageIndex = -1;
			this.tvFunc.LabelEdit = true;
			this.tvFunc.Location = new System.Drawing.Point(0, 179);
			this.tvFunc.Name = "tvFunc";
			this.tvFunc.SelectedImageIndex = -1;
			this.tvFunc.Size = new System.Drawing.Size(192, 101);
			this.tvFunc.TabIndex = 3;
			// 
			// opaqueSplitter2
			// 
			this.opaqueSplitter2.Dock = System.Windows.Forms.DockStyle.Top;
			this.opaqueSplitter2.Location = new System.Drawing.Point(0, 176);
			this.opaqueSplitter2.Name = "opaqueSplitter2";
			this.opaqueSplitter2.Opaque = true;
			this.opaqueSplitter2.Size = new System.Drawing.Size(192, 3);
			this.opaqueSplitter2.TabIndex = 5;
			this.opaqueSplitter2.TabStop = false;
			// 
			// tvClass
			// 
			this.tvClass.AllowDrop = true;
			this.tvClass.ContextMenu = this.contextMenu1;
			this.tvClass.Dock = System.Windows.Forms.DockStyle.Top;
			this.tvClass.ImageIndex = -1;
			this.tvClass.LabelEdit = true;
			this.tvClass.Location = new System.Drawing.Point(0, 75);
			this.tvClass.Name = "tvClass";
			this.tvClass.SelectedImageIndex = -1;
			this.tvClass.Size = new System.Drawing.Size(192, 101);
			this.tvClass.TabIndex = 2;
			// 
			// opaqueSplitter1
			// 
			this.opaqueSplitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.opaqueSplitter1.Location = new System.Drawing.Point(0, 72);
			this.opaqueSplitter1.Name = "opaqueSplitter1";
			this.opaqueSplitter1.Opaque = true;
			this.opaqueSplitter1.Size = new System.Drawing.Size(192, 3);
			this.opaqueSplitter1.TabIndex = 2;
			this.opaqueSplitter1.TabStop = false;
			// 
			// View
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.textBox1,
																		  this.opaqueSplitter3,
																		  this.panel1});
			this.Name = "View";
			this.Size = new System.Drawing.Size(400, 280);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private bool m_bIgnoreChanged = false;
		protected virtual void OnChanged(object sender, System.EventArgs e)
		{
			if(!m_bIgnoreChanged && Changed != null) Changed(sender, e);
		}

		public void SetDocument(Document doc)
		{
			m_bIgnoreChanged = true;

			// TODO: Document を View に反映するための処理を追加します。

			m_bIgnoreChanged = false;
		}
	}
}
