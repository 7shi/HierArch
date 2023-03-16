using System;
using System.Drawing;
using System.Windows.Forms;
using Girl.Windows.Forms;

namespace Girl.HierArch.PluginEditor
{
	/// <summary>
	/// View メニューの設定を保持します。
	/// </summary>
	public class ViewData
	{
		public bool ToolBar_Visible    = true;
		public bool StatusBar_Visible  = true;
		public bool SmartEnter         = true;
		public bool SmartTab           = true;
		public bool SmartHome          = true;
		public bool SmartParenthesis   = true;
		public int  LogViewerHeight    = 0;

		public static ViewData Load(ApplicationDataManager adm)
		{
			ViewData ret = (ViewData)adm.Load("View.xml", typeof(ViewData));
			if(ret == null) ret = new ViewData();
			return ret;
		}

		public void Save(ApplicationDataManager adm)
		{
			adm.Save("View.xml", this);
		}
	}

	/// <summary>
	/// View メニューの設定を管理します。
	/// </summary>
	public class ViewManager
	{
		private Form1 m_Form;
		private ViewData m_Data;

		public ViewManager(Form1 form, ViewData data)
		{
			m_Form = form;
			m_Form.Closed += new EventHandler(Form_Closed);

			m_Data = data;
			form.toolBar1  .Visible = form.mnuViewToolBar  .Checked = data.ToolBar_Visible;
			form.statusBar1.Visible = form.mnuViewStatusBar.Checked = data.StatusBar_Visible;
			if (!data.SmartEnter         ) m_Form.mnuOptionSmartEnter      .PerformClick();
			if (!data.SmartTab           ) m_Form.mnuOptionSmartTab        .PerformClick();
			if (!data.SmartHome          ) m_Form.mnuOptionSmartHome       .PerformClick();
			if (!data.SmartParenthesis   ) m_Form.mnuOptionSmartParenthesis.PerformClick();
			if ( data.LogViewerHeight > 0) m_Form.view1.logViewer.Height = data.LogViewerHeight;
		}

		private void Form_Closed(object sender, System.EventArgs e)
		{
			this.m_Data.ToolBar_Visible    = this.m_Form.toolBar1  .Visible;
			this.m_Data.StatusBar_Visible  = this.m_Form.statusBar1.Visible;
			this.m_Data.SmartEnter         = this.m_Form.mnuOptionSmartEnter      .Checked;
			this.m_Data.SmartTab           = this.m_Form.mnuOptionSmartTab        .Checked;
			this.m_Data.SmartHome          = this.m_Form.mnuOptionSmartHome       .Checked;
			this.m_Data.SmartParenthesis   = this.m_Form.mnuOptionSmartParenthesis.Checked;
			this.m_Data.LogViewerHeight    = this.m_Form.view1.logViewer.Height;
		}
	}
}
