using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Girl.HierarchyArchitect
{
	/// <summary>
	/// About の概要の説明です。
	/// </summary>
	public class About : System.Windows.Forms.Form
	{
		public System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

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

		#region Windows Form Designer generated code
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(384, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "label1";
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(165, 232);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(80, 24);
			this.button1.TabIndex = 0;
			this.button1.Text = "OK";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(384, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "このソフトウェアはパブリックドメインです。";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 80);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(384, 16);
			this.label4.TabIndex = 5;
			this.label4.Text = "開発には以下のソフトウェアを使用しています。";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(16, 112);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(384, 16);
			this.label5.TabIndex = 6;
			this.label5.Text = "Microsoft Development Environment 2002 Version 7.0.9486";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(16, 136);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(384, 16);
			this.label6.TabIndex = 7;
			this.label6.Text = "Copyright (c) 1987-2001 Microsoft Corporation. All rights reserved";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(16, 168);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(384, 16);
			this.label7.TabIndex = 8;
			this.label7.Text = "Microsoft .NET Framework 1.0 Version 1.0.3705";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(16, 192);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(384, 16);
			this.label8.TabIndex = 9;
			this.label8.Text = "Copyright (c) 1998-2001 Microsoft Corporation. All rights reserved";
			// 
			// About
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(410, 271);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.label8,
																		  this.label7,
																		  this.label6,
																		  this.label5,
																		  this.label4,
																		  this.label2,
																		  this.button1,
																		  this.label1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "About";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About...";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
