using System;
using System.Drawing;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// アプリケーションの設定を保持します。
	/// </summary>
	public class WindowSizeData
	{
		public FormWindowState WindowState = FormWindowState.Normal;
		public Size WindowSize = new Size();

		public static WindowSizeData Load(ApplicationDataManager adm)
		{
			WindowSizeData ret = (WindowSizeData)adm.Load("WindowSize.xml", typeof(WindowSizeData));
			if (ret == null) ret = new WindowSizeData();
			return ret;
		}

		public void Save(ApplicationDataManager adm)
		{
			adm.Save("WindowSize.xml", this);
		}
	}

	/// <summary>
	/// フォームのサイズを管理します。
	/// </summary>
	public class WindowSizeManager
	{
		private Form form;
		private WindowSizeData data;
		private FormWindowState state;
		private Size size;

		public WindowSizeManager(Form form, WindowSizeData data, bool hasMenu)
		{
			this.form = form;
			this.form.Resize += new EventHandler(Form_Resize);
			this.form.Closed += new EventHandler(Form_Closed);

			this.data  = data;
			this.state = data.WindowState;
			this.size  = data.WindowSize;

			this.form.WindowState = this.state;
			if (!this.size.IsEmpty)
			{
				if (hasMenu)
				{
					Object obj = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics").GetValue("MenuHeight", "-270");
					this.size.Height -= (Convert.ToInt32((String)obj) / -15 + 1);
				}
				this.form.Size = this.size;
			}
			else
			{
				this.size = this.form.Size;
			}
		}

		private void Form_Resize(object sender, EventArgs e)
		{
			if (this.form.WindowState == FormWindowState.Normal) this.size = this.form.Size;
			this.state = this.form.WindowState;
		}

		private void Form_Closed(object sender, System.EventArgs e)
		{
			this.data.WindowState = this.state;
			this.data.WindowSize  = this.size;
		}
	}

	/// <summary>
	/// フォームのサイズを監視します。
	/// </summary>
	public class WindowSizeMonitor
	{
		private Form form;
		private Rectangle rect;

		public WindowSizeMonitor(Form form)
		{
			this.form  = form;
			this.rect  = form.DesktopBounds;

			form.Move   += new EventHandler(Form_Move  );
			form.Resize += new EventHandler(Form_Resize);
		}

		private void Form_Move(object sender, EventArgs e)
		{
			if (this.form.WindowState != FormWindowState.Normal) return;

			this.rect.X = this.form.DesktopLocation.X;
			this.rect.Y = this.form.DesktopLocation.Y;
		}

		private void Form_Resize(object sender, EventArgs e)
		{
			if (this.form.WindowState != FormWindowState.Normal) return;

			this.rect.Width  = this.form.Width ;
			this.rect.Height = this.form.Height;
		}

		public Rectangle Rect
		{
			get
			{
				return this.rect;
			}
		}
	}
}
