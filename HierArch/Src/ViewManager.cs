using Girl.Windows.Forms;
using System;

namespace Girl.HierArch
{
    /// <summary>
    /// View メニューの設定を保持します。
    /// </summary>
    public class ViewData
    {
        public bool ToolBar_Visible = true;
        public bool StatusBar_Visible = true;

        public static ViewData Load(ApplicationDataManager adm)
        {
            ViewData ret = adm.Load("View.xml", typeof(ViewData)) as ViewData;
            return ret ?? new ViewData();
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
        private readonly Form1 m_Form;
        private readonly ViewData m_Data;

        public ViewManager(Form1 form, ViewData data)
        {
            m_Form = form;
            m_Form.Closed += new EventHandler(Form_Closed);

            m_Data = data;
        }

        private void Form_Closed(object sender, System.EventArgs e)
        {
            m_Data.ToolBar_Visible = m_Form.toolBar1.Visible;
            m_Data.StatusBar_Visible = m_Form.statusBar1.Visible;
        }
    }
}
