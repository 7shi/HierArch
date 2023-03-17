// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Girl.HierArch
{
	/// <summary>
	/// ウィンドウの状態を保持します。
	/// </summary>
	public class HAViewInfo
	{
		public int X;
		public int Y;
		public int Width;
		public int Height;
		public FormWindowState State;
		public int LeftPanelWidth;
		public int ClassHeight;
		public string FontName;
		public int FontSize;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAViewInfo()
		{
			this.Init();
		}

		public void Init()
		{
			this.X = this.Y = this.Width = this.Height = 0;
			this.State = FormWindowState.Normal;
			this.LeftPanelWidth = this.ClassHeight = 0;
			var font = Control.DefaultFont;
			this.FontName = "";
			this.FontSize = 0;
		}

		public void InitHds()
		{
			this.Init();
		}

		public void Apply(Form1 form1)
		{
			if (this.Width > 0 && this.Height > 0)
			{
				Rectangle r1 = new Rectangle(this.X, this.Y, this.Width, this.Height);
				Rectangle r2 = Screen.GetWorkingArea(r1);
				if (r1.IntersectsWith(r2))
				{
					form1.SetDesktopBounds(r1.X, r1.Y, r1.Width, r1.Height);
				}
				else
				{
					form1.Size = new Size(this.Width, this.Height);
				}
				form1.WindowState = this.State;
			}
			if (this.LeftPanelWidth > 0) form1.view1.panel1.Width = this.LeftPanelWidth;
			if (this.ClassHeight > 0) form1.view1.tvClass.Height = this.ClassHeight;
			if (!String.IsNullOrEmpty(this.FontName) && this.FontSize > 0)
			{
				form1.view1.txtSource.Font = new Font(this.FontName, this.FontSize);
			}
		}

		public void Store(Form1 form1)
		{
			Rectangle rect = form1.sizeMonitor.Rect;
			this.X = rect.X;
			this.Y = rect.Y;
			this.Width = rect.Width;
			this.Height = rect.Height;
			this.State = form1.WindowState;
			this.LeftPanelWidth = form1.view1.panel1.Width;
			this.ClassHeight = form1.view1.tvClass.Height;
			var font = form1.view1.txtSource.Font;
			this.FontName = font.Name;
			this.FontSize = (int)font.Size;
		}
	}
}
