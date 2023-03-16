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
		private Form m_Form;
		private WindowSizeData m_Data;
		private FormWindowState m_WindowState;
		private Size m_Size;

		public WindowSizeManager(Form form, WindowSizeData data, bool hasMenu)
		{
			m_Form = form;
			m_Form.Resize += new EventHandler(Form_Resize);
			m_Form.Closed += new EventHandler(Form_Closed);

			m_Data = data;
			m_WindowState = m_Data.WindowState;
			m_Size        = m_Data.WindowSize;

			m_Form.WindowState = m_WindowState;
			if (!m_Size.IsEmpty)
			{
				if (hasMenu)
				{
					Object obj = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics").GetValue("MenuHeight", "-270");
					m_Size.Height -= (Convert.ToInt32((String)obj) / -15 + 1);
				}
				m_Form.Size = m_Size;
			}
			else
			{
				m_Size = m_Form.Size;
			}
		}

		private void Form_Resize(object sender, EventArgs e)
		{
			if (m_Form.WindowState == FormWindowState.Normal) m_Size = m_Form.Size;
			m_WindowState = m_Form.WindowState;
		}

		private void Form_Closed(object sender, System.EventArgs e)
		{
			m_Data.WindowState = m_WindowState;
			m_Data.WindowSize  = m_Size;
		}
	}
}
