using Girl.Windows.API;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
    /// <summary>
    /// スクロール領域の処理を加えた TreeView です。
    /// </summary>
    public class ExTreeView : TreeView
    {
        public ExTreeView()
        {
            HScrollBar = new InternalHScrollBar(this);
            VScrollBar = new InternalVScrollBar(this);
        }

        #region Scroll Bar

        public InternalHScrollBar HScrollBar { get; }

        public InternalVScrollBar VScrollBar { get; }

        #endregion

        #region Virtual

        public Rectangle VirtualRectangle
        {
            get
            {
                Rectangle ret = ClientRectangle;
                ret.Location = VirtualLocation;
                return ret;
            }
        }

        public Point VirtualLocation => new Point(VirtualLeft, VirtualTop);

        public int VirtualLeft => !HScrollBar.Visible ? 0 : HScrollBar.Pos;

        public int VirtualTop
        {
            get
            {
                ExTreeNode n = FirstVisibleNode;
                if (!VScrollBar.Visible || n == null)
                {
                    return 0;
                }

                int ret = 0;
                int pos = VScrollBar.Pos;
                for (int i = 0; i < pos && n != null; i++, n = (ExTreeNode)n.NextVisibleNode)
                {
                    ret += n.Height;
                }
                return ret;
            }
        }

        public Size VirtualSize => new Size(VirtualWidth, VirtualHeight);

        public int VirtualWidth
        {
            get
            {
                int ret = ClientSize.Width;
                if (HScrollBar.Visible)
                {
                    ret = HScrollBar.Max + 1;
                }

                return ret;
            }
        }

        public int VirtualHeight
        {
            get
            {
                int ret = ClientSize.Height;
                ExTreeNode n = FirstVisibleNode;
                if (VScrollBar.Visible && n != null)
                {
                    int h = 0;
                    for (; n != null; n = (ExTreeNode)n.NextVisibleNode)
                    {
                        h += n.Height;
                    }

                    ret = h;
                }
                return ret;
            }
        }

        #endregion

        #region Node

        public ExTreeNode FirstVisibleNode => Nodes.Count < 1 ? null : (ExTreeNode)Nodes[0];

        public ExTreeNode LastVisibleNode
        {
            get
            {
                TreeNode ret = null;
                for (TreeNode n = FirstVisibleNode; n != null; n = n.NextVisibleNode)
                {
                    ret = n;
                }

                return (ExTreeNode)ret;
            }
        }

        #endregion

        #region Debug

        public string _DebugFieldInfo()
        {
            Rectangle r = VirtualRectangle;
            Size s = VirtualSize;
            return string.Format("領域: ({0}, {1}) + ({2}, {3}) / ({4}, {5})\r\n", r.X, r.Y, r.Width, r.Height, s.Width, s.Height);
        }

        public string _DebugSelectedNodeTextWidth()
        {
            ExTreeNode n = (ExTreeNode)SelectedNode;
            if (n == null)
            {
                return "";
            }

            Rectangle r = n.DisplayRectangle;
            return string.Format(
                "文字列: {0}\r\n文字数: {1}\r\n位置: [{2}, {3}] ({4}, {5}) + ({6}, {7})\r\n",
                n.Text, n.Text.Length, n.Depth, n.VisibleIndex,
                r.Left, r.Top, r.Width, r.Height
                );
        }

        #endregion
    }

    /// <summary>
    /// 領域の処理を加えた TreeNode です。
    /// </summary>
    public class ExTreeNode : TreeNode
    {
        public ExTreeNode()
        {
        }

        public ExTreeNode(string text) : base(text)
        {
        }

        public Font GetNodeFont()
        {
            Font ret = NodeFont;
            return ret ?? TreeView.Font;
        }

        #region Location

        public int TextWidth
        {
            get
            {
                Graphics g = TreeView.CreateGraphics();
                IntPtr hdc = g.GetHdc();
                IntPtr f = Win32API.SelectObject(hdc, GetNodeFont().ToHfont());
                Win32API.Size sz = new Win32API.Size();
                _ = Win32API.GetTextExtentPoint32(hdc, Text, Text.Length, ref sz);
                _ = Win32API.SelectObject(hdc, f);
                g.ReleaseHdc(hdc);
                g.Dispose();

                return sz.cx;
            }
        }

        public int Depth
        {
            get
            {
                int ret = -1;
                for (TreeNode n = this; n != null; n = n.Parent)
                {
                    ret++;
                }

                return ret;
            }
        }

        public int VisibleIndex
        {
            get
            {
                int ret = -1;
                for (TreeNode n = this; n != null; n = n.PrevVisibleNode)
                {
                    ret++;
                }

                return ret;
            }
        }

        public Rectangle DisplayRectangle => new Rectangle(Left, Top, Width, Height);

        public Point Location => new Point(Left, Top);

        public int Left
        {
            get
            {
                int d = Depth;
                if (TreeView.ShowRootLines)
                {
                    d++;
                }

                return d * TreeView.Indent;
            }
        }

        public int Top
        {
            get
            {
                int ret = 0;
                ExTreeNode n = (ExTreeNode)PrevVisibleNode;
                for (; n != null; n = (ExTreeNode)n.PrevVisibleNode)
                {
                    ret += n.Height;
                }

                return ret;
            }
        }

        public int Width
        {
            get
            {
                int ret = TextWidth + 4;
                if (TreeView.ImageList != null)
                {
                    ret += TreeView.ImageList.ImageSize.Width + 3;
                }

                return ret;
            }
        }

        public int Height => TreeView.ItemHeight;

        #endregion

        public bool IsAncestorOf(TreeNode n)
        {
            if (n == null)
            {
                return false;
            }

            for (; n != null; n = n.Parent)
            {
                if (n == this)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
