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
            if (ret == null)
            {
                ret = new WindowSizeData();
            }

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
        private readonly Form form;
        private readonly WindowSizeData data;
        private FormWindowState state;
        private Size size;

        public WindowSizeManager(Form form, WindowSizeData data, bool hasMenu)
        {
            this.form = form;
            this.form.Resize += new EventHandler(Form_Resize);
            this.form.Closed += new EventHandler(Form_Closed);

            this.data = data;
            state = data.WindowState;
            size = data.WindowSize;

            this.form.WindowState = state;
            if (!size.IsEmpty)
            {
                if (hasMenu)
                {
                    object obj = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics").GetValue("MenuHeight", "-270");
                    if (data.WindowState == FormWindowState.Normal)
                    {
                        size.Height -= (Convert.ToInt32((string)obj) / -15) + 1;
                    }
                }
                this.form.Size = size;
            }
            else
            {
                size = this.form.Size;
            }
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            if (form.WindowState == FormWindowState.Normal)
            {
                size = form.Size;
            }

            state = form.WindowState;
        }

        private void Form_Closed(object sender, System.EventArgs e)
        {
            data.WindowState = state;
            data.WindowSize = size;
        }
    }

    /// <summary>
    /// フォームのサイズを監視します。
    /// </summary>
    public class WindowSizeMonitor
    {
        private readonly Form form;
        private Rectangle rect;

        public WindowSizeMonitor(Form form)
        {
            this.form = form;
            rect = form.DesktopBounds;

            form.Move += new EventHandler(Form_Move);
            form.Resize += new EventHandler(Form_Resize);
        }

        private void Form_Move(object sender, EventArgs e)
        {
            if (form.WindowState != FormWindowState.Normal)
            {
                return;
            }

            rect.X = form.DesktopLocation.X;
            rect.Y = form.DesktopLocation.Y;
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            if (form.WindowState != FormWindowState.Normal)
            {
                return;
            }

            rect.Width = form.Width;
            rect.Height = form.Height;
        }

        public Rectangle Rect => rect;
    }
}
