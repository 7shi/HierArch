using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Girl.Rtf;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// RichTextBox を簡単に操作するためのツールバーです。
	/// </summary>
	public class RichTextToolBar : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ToolBarButton tbBold;
		private System.Windows.Forms.ToolBarButton tbItalic;
		private System.Windows.Forms.ToolBarButton tbUnderline;
		private System.Windows.Forms.ToolBarButton tbColor;
		private System.Windows.Forms.ToolBarButton tbSeparator1;
		private System.Windows.Forms.ToolBarButton tbLeft;
		private System.Windows.Forms.ToolBarButton tbCenter;
		private System.Windows.Forms.ToolBarButton tbRight;
		private System.Windows.Forms.ToolBarButton tbSeparator2;
		private System.Windows.Forms.ToolBarButton tbBullet;
		public Girl.Windows.Forms.ExComboBox cmbFontSize;
		public Girl.Windows.Forms.FontComboBox cmbFontName;
		public System.Windows.Forms.ToolBar toolBar;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.ComponentModel.IContainer components;

		public RichTextToolBar()
		{
			// この呼び出しは、Windows.Forms フォーム デザイナで必要です。
			InitializeComponent();

			// TODO: InitForm を呼び出しの後に初期化処理を追加します。
			this.Dock = DockStyle.Top;
			this.TabStop = false;

			this.cmbFontName.InitializeItems();
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(RichTextToolBar));
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.toolBar = new System.Windows.Forms.ToolBar();
			this.tbBold = new System.Windows.Forms.ToolBarButton();
			this.tbItalic = new System.Windows.Forms.ToolBarButton();
			this.tbUnderline = new System.Windows.Forms.ToolBarButton();
			this.tbColor = new System.Windows.Forms.ToolBarButton();
			this.tbSeparator1 = new System.Windows.Forms.ToolBarButton();
			this.tbLeft = new System.Windows.Forms.ToolBarButton();
			this.tbCenter = new System.Windows.Forms.ToolBarButton();
			this.tbRight = new System.Windows.Forms.ToolBarButton();
			this.tbSeparator2 = new System.Windows.Forms.ToolBarButton();
			this.tbBullet = new System.Windows.Forms.ToolBarButton();
			this.cmbFontSize = new Girl.Windows.Forms.ExComboBox();
			this.cmbFontName = new Girl.Windows.Forms.FontComboBox();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.SuspendLayout();
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// toolBar
			// 
			this.toolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																					   this.tbBold,
																					   this.tbItalic,
																					   this.tbUnderline,
																					   this.tbColor,
																					   this.tbSeparator1,
																					   this.tbLeft,
																					   this.tbCenter,
																					   this.tbRight,
																					   this.tbSeparator2,
																					   this.tbBullet});
			this.toolBar.Dock = System.Windows.Forms.DockStyle.None;
			this.toolBar.DropDownArrows = true;
			this.toolBar.ImageList = this.imageList1;
			this.toolBar.Location = new System.Drawing.Point(203, 0);
			this.toolBar.Name = "toolBar";
			this.toolBar.ShowToolTips = true;
			this.toolBar.Size = new System.Drawing.Size(208, 25);
			this.toolBar.TabIndex = 2;
			this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
			// 
			// tbBold
			// 
			this.tbBold.ImageIndex = 0;
			this.tbBold.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbBold.ToolTipText = "太字";
			// 
			// tbItalic
			// 
			this.tbItalic.ImageIndex = 1;
			this.tbItalic.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbItalic.ToolTipText = "斜体";
			// 
			// tbUnderline
			// 
			this.tbUnderline.ImageIndex = 2;
			this.tbUnderline.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbUnderline.ToolTipText = "下線";
			// 
			// tbColor
			// 
			this.tbColor.ImageIndex = 3;
			this.tbColor.ToolTipText = "色";
			// 
			// tbSeparator1
			// 
			this.tbSeparator1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbLeft
			// 
			this.tbLeft.ImageIndex = 4;
			this.tbLeft.Pushed = true;
			this.tbLeft.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbLeft.ToolTipText = "左";
			// 
			// tbCenter
			// 
			this.tbCenter.ImageIndex = 5;
			this.tbCenter.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbCenter.ToolTipText = "中央";
			// 
			// tbRight
			// 
			this.tbRight.ImageIndex = 6;
			this.tbRight.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbRight.ToolTipText = "右";
			// 
			// tbSeparator2
			// 
			this.tbSeparator2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbBullet
			// 
			this.tbBullet.ImageIndex = 7;
			this.tbBullet.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbBullet.ToolTipText = "箇条書き";
			// 
			// cmbFontSize
			// 
			this.cmbFontSize.ItemHeight = 12;
			this.cmbFontSize.Items.AddRange(new object[] {
															 "8",
															 "9",
															 "10",
															 "11",
															 "12",
															 "14",
															 "16",
															 "18",
															 "20",
															 "22",
															 "24",
															 "26",
															 "28",
															 "36",
															 "48",
															 "72"});
			this.cmbFontSize.Location = new System.Drawing.Point(141, 3);
			this.cmbFontSize.MaxDropDownItems = 16;
			this.cmbFontSize.MaxLength = 4;
			this.cmbFontSize.Name = "cmbFontSize";
			this.cmbFontSize.Size = new System.Drawing.Size(55, 20);
			this.cmbFontSize.TabIndex = 1;
			this.cmbFontSize.EscapeKeyPress += new System.EventHandler(this.cmbFont_EscapeKeyPress);
			this.cmbFontSize.SelectionChangeCommitted += new System.EventHandler(this.cmbFont_SelectionChangeCommitted);
			this.cmbFontSize.EnterKeyPress += new System.EventHandler(this.cmbFont_SelectionChangeCommitted);
			this.cmbFontSize.Validated += new System.EventHandler(this.cmbFontSize_Validated);
			// 
			// cmbFontName
			// 
			this.cmbFontName.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.cmbFontName.Location = new System.Drawing.Point(5, 3);
			this.cmbFontName.MaxDropDownItems = 16;
			this.cmbFontName.Name = "cmbFontName";
			this.cmbFontName.Size = new System.Drawing.Size(128, 20);
			this.cmbFontName.TabIndex = 0;
			this.cmbFontName.EscapeKeyPress += new System.EventHandler(this.cmbFont_EscapeKeyPress);
			this.cmbFontName.EnterKeyPress += new System.EventHandler(this.cmbFont_SelectionChangeCommitted);
			this.cmbFontName.Validated += new System.EventHandler(this.cmbFontName_Validated);
			this.cmbFontName.SelectionChangeCommitted += new System.EventHandler(this.cmbFont_SelectionChangeCommitted);
			// 
			// RichTextToolBar
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.toolBar,
																		  this.cmbFontSize,
																		  this.cmbFontName});
			this.Name = "RichTextToolBar";
			this.Size = new System.Drawing.Size(416, 25);
			this.ResumeLayout(false);

		}
		#endregion

		private bool divider = true;

		public bool Divider
		{
			get
			{
				return this.divider;
			}

			set
			{
				this.divider = this.toolBar.Divider = value;
				this.Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.DrawLine(SystemPens.ControlDark      , 0, 0, this.Width, 0);
			e.Graphics.DrawLine(SystemPens.ControlLightLight, 0, 1, this.Width, 1);
		}

		/// <summary>
		/// フォーカスを避ける。
		/// </summary>
		protected override void OnMouseDown(MouseEventArgs e)
		{
		}

		private RichTextBox target = null;
		private ArrayList richTextBoxes = new ArrayList();

		public RichTextBox Target
		{
			get
			{
				return this.target;
			}

			set
			{
				this.target = value;
				this.AddTarget(value);
			}
		}

		public void AddTarget(RichTextBox richTextBox)
		{
			if (richTextBox == null || this.richTextBoxes.Contains(richTextBox)) return;

			this.richTextBoxes.Add(richTextBox);
			richTextBox.Enter            += new EventHandler(this.target_Enter);
			richTextBox.Disposed         += new EventHandler(this.target_Disposed);
			richTextBox.SelectionChanged += new EventHandler(this.target_SelectionChanged);
		}

		public void RemoveTarget(RichTextBox richTextBox)
		{
			if (this.target == richTextBox) this.target = null;
			if (!this.richTextBoxes.Contains(richTextBox)) return;

			this.richTextBoxes.Remove(richTextBox);
		}

		private Font TargetFont
		{
			get
			{
				Font f = this.target.SelectionFont;
				if (f != null) return f;

				return this.target.Font;
			}
		}

		private string TargetFontName
		{
			get
			{
				if (this.target == null) return "";

				if (this.target.SelectionLength < 1)
				{
					return this.TargetFont.Name;
				}

				RtfDocument rd = RtfDocument.Parse(this.target.SelectedRtf);
				return rd.FontName;
			}
		}

		private string TargetFontSize
		{
			get
			{
				if (this.target == null) return "";

				if (this.target.SelectionLength < 1)
				{
					return this.TargetFont.Size.ToString();
				}

				RtfDocument rd = RtfDocument.Parse(this.target.SelectedRtf);
				float fs = rd.FontSize;
				if (fs < 1) return "";
				return fs.ToString();
			}
		}

		private void target_Enter(object sender, EventArgs e)
		{
			if (!this.richTextBoxes.Contains(sender)) return;

			this.target = sender as RichTextBox;
			this.target_SelectionChanged(sender, EventArgs.Empty);
		}

		private void target_Disposed(object sender, EventArgs e)
		{
			if (!this.richTextBoxes.Contains(sender)) return;

			this.richTextBoxes.Remove(sender);
		}

		private void target_SelectionChanged(object sender, EventArgs e)
		{
			if (sender != this.target) return;

			this.cmbFontName.Text = this.TargetFontName;
			this.cmbFontSize.Text = this.TargetFontSize;

			if (this.target.SelectionLength > 0)
			{
				RtfDocument rd = RtfDocument.Parse(this.target.SelectedRtf);
				this.tbBold     .Pushed = rd.IsBold;
				this.tbItalic   .Pushed = rd.IsItalic;
				this.tbUnderline.Pushed = rd.IsUnderline;
			}
			else
			{
				Font f = this.target.SelectionFont;
				if (f != null)
				{
					this.tbBold     .Pushed = (f.Style & FontStyle.Bold     ) == FontStyle.Bold;
					this.tbItalic   .Pushed = (f.Style & FontStyle.Italic   ) == FontStyle.Italic;
					this.tbUnderline.Pushed = (f.Style & FontStyle.Underline) == FontStyle.Underline;
				}
				else
				{
					this.tbBold     .Pushed = false;
					this.tbItalic   .Pushed = false;
					this.tbUnderline.Pushed = false;
				}
			}

			this.tbLeft  .Pushed = this.target.SelectionAlignment == HorizontalAlignment.Left;
			this.tbCenter.Pushed = this.target.SelectionAlignment == HorizontalAlignment.Center;
			this.tbRight .Pushed = this.target.SelectionAlignment == HorizontalAlignment.Right;
			this.tbBullet.Pushed = this.target.SelectionBullet;
		}

		private void cmbFont_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			if (target == null) return;

			ComboBox cb = sender as ComboBox;
			int sel = cb.SelectedIndex;
			if (sel >= 0) cb.Text = cb.Items[cb.SelectedIndex].ToString();
			this.target.Focus();
		}

		private void cmbFont_EscapeKeyPress(object sender, System.EventArgs e)
		{
			this.cmbFontName.Text = "";
			this.cmbFontSize.Text = "";
			if (target == null) return;

			this.target.Focus();
		}

		private void cmbFontName_Validated(object sender, System.EventArgs e)
		{
			if (this.target == null) return;

			string fn = this.cmbFontName.Text;
			if (fn == "")
			{
				this.cmbFontName.Text = this.TargetFontName;
				return;
			}

			if (this.target.SelectionLength < 1)
			{
				Font f = this.TargetFont;
				this.target.SelectionFont = new Font(fn, f.Size, f.Style);
				return;
			}

			RtfDocument rd = RtfDocument.Parse(this.target.SelectedRtf);
			rd.FontName = fn;
			this.SetSelectedRtf(rd.ToRtf());
		}

		private void cmbFontSize_Validated(object sender, System.EventArgs e)
		{
			if (this.target == null) return;

			float size = this.GetFontSize();
			if (size < 1)
			{
				this.cmbFontSize.Text = this.TargetFontSize;
				return;
			}

			if (this.target.SelectionLength < 1)
			{
				Font f = this.TargetFont;
				this.target.SelectionFont = new Font(f.Name, size, f.Style);
				return;
			}

			RtfDocument rd = RtfDocument.Parse(this.target.SelectedRtf);
			rd.FontSize = size;
			this.SetSelectedRtf(rd.ToRtf());
		}

		private float GetFontSize()
		{
			string text = this.cmbFontSize.Text;
			if (text == "") return 0;

			float ret;
			try
			{
				ret = (float)Convert.ToDouble(text);
			}
			catch
			{
				MessageBox.Show(this.ParentForm, "無効な数字です。",
					Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return 0;
			}

			if (ret < 1 || 1638 < ret)
			{
				MessageBox.Show(this.ParentForm, "1 から 1638 の間の値を入力してください。",
					Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return 0;
			}

			return ret;
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == this.tbLeft)
			{
				this.tbCenter.Pushed = this.tbRight.Pushed = false;
			}
			else if (e.Button == this.tbCenter)
			{
				this.tbLeft.Pushed = this.tbRight.Pushed = false;
			}
			else if (e.Button == this.tbRight)
			{
				this.tbLeft.Pushed = this.tbCenter.Pushed = false;
			}
			if (this.target == null) return;

			if (e.Button == this.tbBold || e.Button == this.tbItalic || e.Button == this.tbUnderline)
			{
				if (this.target.SelectionLength > 0)
				{
					RtfDocument rd = RtfDocument.Parse(this.target.SelectedRtf);
					if (e.Button == this.tbBold)
					{
						rd.IsBold = e.Button.Pushed;
					}
					else if (e.Button == this.tbItalic)
					{
						rd.IsItalic = e.Button.Pushed;
					}
					else if (e.Button == this.tbUnderline)
					{
						rd.IsUnderline = e.Button.Pushed;
					}
					this.SetSelectedRtf(rd.ToRtf());
				}
				else
				{
					Font f = this.TargetFont;
					FontStyle fs1 = FontStyle.Bold;
					if (e.Button == this.tbItalic)
					{
						fs1 = FontStyle.Italic;
					}
					else if (e.Button == this.tbUnderline)
					{
						fs1 = FontStyle.Underline;
					}
					FontStyle fs2 = f.Style & ~fs1;
					if (e.Button.Pushed) fs2 |= fs1;
					this.target.SelectionFont = new Font(f.Name, f.Size, fs2);
				}
			}
			else if (e.Button == this.tbColor)
			{
				if (this.colorDialog1.ShowDialog(this.ParentForm) == DialogResult.OK)
				{
					this.target.SelectionColor = this.colorDialog1.Color;
				}
			}
			else if (e.Button == this.tbLeft)
			{
				this.target.SelectionAlignment = HorizontalAlignment.Left;
			}
			else if (e.Button == this.tbCenter)
			{
				this.target.SelectionAlignment = HorizontalAlignment.Center;
			}
			else if (e.Button == this.tbRight)
			{
				this.target.SelectionAlignment = HorizontalAlignment.Right;
			}
			else if (e.Button == this.tbBullet)
			{
				this.target.SelectionBullet = this.tbBullet.Pushed;
			}
		}

		public void SetSelectedRtf(string rtf)
		{
			int p = this.target.SelectionStart;
			this.target.SelectedRtf = rtf;
			int len = this.target.SelectionStart - p;
			this.target.SelectionStart  = p;
			this.target.SelectionLength = len;
		}
	}
}
