using Girl.Windows.Forms;
using System;
using System.Windows.Forms;

namespace Girl.HierArch
{
    /// <summary>
    /// Document に対応する View です。
    /// </summary>
    public partial class View : UserControl
    {
        public View()
        {
            // この呼び出しは、Windows.Forms フォーム デザイナで必要です。
            InitializeComponent();

            // TODO: InitializeComponent を呼び出しの後に初期化処理を追加します。
            tvClass.MoveTarget.Add(tvFunc);
            tvClass.FuncTreeView = tvFunc;
            tvFunc.SourceTextBox = txtSource;

            tvClass.SetView();

            tvClass.Changed += new EventHandler(OnChanged);
            tvFunc.Changed += new EventHandler(OnChanged);

            txtSource.TextChanged += new EventHandler(tvFunc.OnChanged);
        }

        private bool IgnoreChanged = false;

        protected virtual void OnChanged(object sender, System.EventArgs e)
        {
            if (IgnoreChanged)
            {
                return;
            }

            Changed?.Invoke(sender, e);
            if (sender != tvClass)
            {
                IgnoreChanged = true;
                tvClass.OnChanged(sender, e);
                IgnoreChanged = false;
            }
        }

        public void SetDocument(Document doc)
        {
            IgnoreChanged = true;

            // TODO: Document を View に反映するための処理を追加します。

            IgnoreChanged = false;
        }

        private DnDTreeView m_tvTarget = null;

        private void TreeView_Enter(object sender, System.EventArgs e)
        {
            m_tvTarget = (DnDTreeView)sender;
        }
    }
}
