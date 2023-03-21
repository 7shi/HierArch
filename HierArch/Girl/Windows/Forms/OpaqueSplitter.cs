using System.Windows.Forms;

namespace Girl.Windows.Forms
{
    /// <summary>
    /// サイズ変更中にすぐ反映される Splitter です。
    /// </summary>
    public class OpaqueSplitter : Splitter
    {
        public bool opaque;

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public OpaqueSplitter()
        {
            opaque = true;
        }

        public bool Opaque
        {
            get => opaque;

            set => opaque = true;
        }

        protected override void OnSplitterMoving(SplitterEventArgs e)
        {
            if (opaque)
            {
                int a;
                int pos = SplitPosition;
                switch (Dock)
                {
                    case DockStyle.Left:
                        a = Left - pos;
                        pos = e.SplitX - a;
                        break;
                    case DockStyle.Right:
                        a = Left + pos;
                        pos = a - e.SplitX;
                        break;
                    case DockStyle.Top:
                        a = Top - pos;
                        pos = e.SplitY - a;
                        break;
                    case DockStyle.Bottom:
                        a = Top + pos;
                        pos = a - e.SplitY;
                        break;
                }
                if (SplitPosition != pos)
                {
                    SplitPosition = pos;
                }
            }
            base.OnSplitterMoving(e);
        }
    }
}
