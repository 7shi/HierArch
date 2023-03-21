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
        public bool WordWrap;
        public string FontName;
        public int FontSize;

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public HAViewInfo()
        {
            Init();
        }

        public void Init()
        {
            X = Y = Width = Height = 0;
            State = FormWindowState.Normal;
            LeftPanelWidth = ClassHeight = 0;
            WordWrap = false;
            Font font = Control.DefaultFont;
            FontName = font.Name;
            FontSize = (int)Math.Round(font.Size);
        }

        public void InitHds()
        {
            Init();
        }

        public void Apply(Form1 form1)
        {
            if (Width > 0 && Height > 0)
            {
                Rectangle r1 = new Rectangle(X, Y, Width, Height);
                Rectangle r2 = Screen.GetWorkingArea(r1);
                if (r1.IntersectsWith(r2))
                {
                    form1.SetDesktopBounds(r1.X, r1.Y, r1.Width, r1.Height);
                }
                else
                {
                    form1.Size = new Size(Width, Height);
                }
                form1.WindowState = State;
            }
            if (LeftPanelWidth > 0)
            {
                form1.view1.panel1.Width = LeftPanelWidth;
            }

            if (ClassHeight > 0)
            {
                form1.view1.tvClass.Height = ClassHeight;
            }

            Windows.Forms.CodeEditor src = form1.view1.txtSource;
            src.WordWrap = form1.mnuOptionWordWrap.Checked = WordWrap;
            if (!string.IsNullOrEmpty(FontName) && FontSize > 0)
            {
                src.Font = new Font(FontName, FontSize);
            }
        }

        public void Store(Form1 form1)
        {
            Rectangle rect = form1.sizeMonitor.Rect;
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
            State = form1.LastState;
            LeftPanelWidth = form1.view1.panel1.Width;
            ClassHeight = form1.view1.tvClass.Height;
            Windows.Forms.CodeEditor src = form1.view1.txtSource;
            WordWrap = src.WordWrap;
            FontName = src.Font.Name;
            FontSize = (int)Math.Round(src.Font.Size);
        }
    }
}
