using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
    /// <summary>
    /// MenuItem の表示を拡張します。
    /// </summary>
    public class MenuItemEx
    {
        public static string IconPath = null;
        public static string Extension = null;

        #region Win32

        [DllImport("Gdi32.dll")]
        public static extern bool BitBlt(
            IntPtr hdcDest, // コピー先デバイスコンテキストのハンドル
            int nXDest,     // コピー先長方形の左上隅の x 座標
            int nYDest,     // コピー先長方形の左上隅の y 座標
            int nWidth,     // コピー先長方形の幅
            int nHeight,    // コピー先長方形の高さ
            IntPtr hdcSrc,  // コピー元デバイスコンテキストのハンドル
            int nXSrc,      // コピー元長方形の左上隅の x 座標
            int nYSrc,      // コピー元長方形の左上隅の y 座標
            int dwRop       // ラスタオペレーションコード
        );

        public const int SRCCOPY = 0x00CC0020;

        #endregion
        public string Name;
        public object Tag;
        protected MenuItem menuItem;
        protected Image image;
        protected Image light;
        protected Image shadow;
        protected Font font;
        protected Bitmap backImage;

        public static Hashtable Apply(Menu menu, Hashtable images, bool mne)
        {
            Hashtable ret = new Hashtable();
            Apply(menu, images, ret, mne);
            return ret;
        }

        public static Hashtable Apply(Menu menu, bool mne)
        {
            Hashtable ret = new Hashtable();
            Apply(menu, null, ret, mne);
            return ret;
        }

        public static Hashtable Apply(Menu menu, Hashtable images)
        {
            Hashtable ret = new Hashtable();
            Apply(menu, images, ret, false);
            return ret;
        }

        public static Hashtable Apply(Menu menu)
        {
            Hashtable ret = new Hashtable();
            Apply(menu, null, ret, false);
            return ret;
        }

        public static void Apply(Menu menu, Hashtable images, Hashtable map, bool mne)
        {
            if (menu is MenuItem)
            {
                MenuItem mni = menu as MenuItem;
                if (!(mni.Parent is MainMenu))  // && Environment.OSVersion.Platform != PlatformID.Win32NT
                {
                    if (mne) mni.Text = RemoveMnemonic(mni.Text);
                    if (images != null && images.Contains(menu))
                    {
                        object obj = images[menu];
                        if (obj is Image)
                        {
                            map[mni] = new MenuItemEx(mni, obj as Image);
                        }
                        else
                        {
                            map[mni] = new MenuItemEx(mni, obj as string);
                        }
                    }
                    else
                    {
                        map[mni] = new MenuItemEx(mni);
                    }
                }
            }
            foreach (MenuItem mi in menu.MenuItems)
            {
                Apply(mi, images, map, mne);
            }
        }

        protected virtual void Init()
        {
            this.Name = null;
            this.Tag = null;
            this.menuItem = null;
            this.image = null;
            this.light = null;
            this.shadow = null;
            this.font = Control.DefaultFont;
            this.backImage = null;
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public MenuItemEx()
        {
            this.Init();
        }

        public MenuItemEx(MenuItem menuItem)
        {
            this.Init();
            this.MenuItem = menuItem;
        }

        public MenuItemEx(MenuItem menuItem, Image image)
        {
            this.Init();
            this.MenuItem = menuItem;
            this.Image = image;
        }

        public MenuItemEx(MenuItem menuItem, string fileName)
        {
            this.Init();
            this.MenuItem = menuItem;
            this.Image = GetImage(fileName);
            this.Name = fileName;
        }

        #region Properties

        public Font Font
        {
            get
            {
                return this.font;
            }

            set
            {
                this.font = value;
            }
        }

        public Image Image
        {
            get
            {
                return this.image;
            }

            set
            {
                if (this.light != null) light.Dispose();
                if (this.shadow != null) shadow.Dispose();
                this.image = value;
                this.light = MakeLight(value);
                this.shadow = MakeShadow(value);
            }
        }

        public MenuItem MenuItem
        {
            get
            {
                return this.menuItem;
            }

            set
            {
                this.menuItem = value;
                value.OwnerDraw = true;
                value.MeasureItem += new MeasureItemEventHandler(this.menuItem_MeasureItem);
                value.DrawItem += new DrawItemEventHandler(this.menuItem_DrawItem);
            }
        }

        #endregion

        #region Event Handlers

        private void menuItem_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.HotkeyPrefix = HotkeyPrefix.Show;
            Size sz = Size.Truncate(e.Graphics.MeasureString(this.menuItem.Text, this.font, PointF.Empty, sf));
            int w, h;
            if (this.menuItem.Parent is MainMenu)
            {
                w = sz.Width + 2;
                h = sz.Height;
            }
            else if (this.menuItem.Text == "-")
            {
                w = 31;
                h = 3;
            }
            else
            {
                w = sz.Width + 31;
                h = sz.Height + 2;
                if (h < 22) h = 22;
                if (this.menuItem.Shortcut != Shortcut.None && this.menuItem.ShowShortcut)
                {
                    string st = GetShortcutText(this.menuItem.Shortcut);
                    Size sz2 = Size.Truncate(e.Graphics.MeasureString(st, this.font));
                    w += sz2.Width + 16;
                }
            }
            e.ItemWidth = w;
            e.ItemHeight = h;
        }

        private void menuItem_DrawItem(object sender, DrawItemEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.HotkeyPrefix = HotkeyPrefix.Show;
            Size sz = Size.Truncate(e.Graphics.MeasureString(this.menuItem.Text, this.font, PointF.Empty, sf));
            Rectangle r = e.Bounds;
            int x = r.X + (r.Width - sz.Width) / 2;
            int y = r.Y + (r.Height - sz.Height) / 2;
            Brush brush = SystemBrushes.WindowText;
            if (!this.menuItem.Enabled || (e.State & DrawItemState.Inactive) != 0) brush = SystemBrushes.ControlDark;
            bool sel = (e.State & DrawItemState.Selected) != 0 && this.menuItem.Enabled;
            if (this.menuItem.Parent is MainMenu)
            {
                if (this.backImage == null)
                {
                    IntPtr hdc1 = e.Graphics.GetHdc();
                    this.backImage = new Bitmap(r.Width, r.Height);
                    Graphics g = Graphics.FromImage(this.backImage);
                    IntPtr hdc2 = g.GetHdc();
                    bool ok = BitBlt(hdc2, 0, 0, r.Width, r.Height, hdc1, r.X, r.Y, SRCCOPY);
                    g.ReleaseHdc(hdc2);
                    g.Dispose();
                    e.Graphics.ReleaseHdc(hdc1);
                }
                Point pt = Cursor.Position;
                Form f = (this.MenuItem.Parent as MainMenu).GetForm();
                pt.Offset(-f.Left, -f.Top);
                if ((e.State & DrawItemState.HotLight) != 0 || (sel && !e.Bounds.Contains(pt)))
                {
                    DrawSelection(e.Graphics, r, false);
                }
                else if (sel)
                {
                    e.Graphics.FillRectangle(SystemBrushes.ControlLight, r);
                    e.Graphics.DrawLine(SystemPens.ControlDark, r.Left, r.Top, r.Left, r.Bottom);
                    e.Graphics.DrawLine(SystemPens.ControlDark, r.Left, r.Top, r.Right - 1, r.Top);
                    e.Graphics.DrawLine(SystemPens.ControlDark, r.Right - 1, r.Top, r.Right - 1, r.Bottom);
                }
                else
                {
                    e.Graphics.DrawImage(this.backImage, r);
                }
                e.Graphics.DrawString(this.menuItem.Text, this.font, brush, x, y, sf);
                return;
            }
            if (!sel)
            {
                Rectangle r1 = new Rectangle(r.X, r.Y, 22, r.Height);
                Rectangle r2 = new Rectangle(r.X + 22, r.Y, r.Width - 22, r.Height);
                e.Graphics.FillRectangle(SystemBrushes.Control, r1);
                e.Graphics.FillRectangle(SystemBrushes.Window, r2);
                if (!this.menuItem.Checked && this.image != null)
                {
                    Image img = this.light;
                    if (!this.menuItem.Enabled) img = this.shadow;
                    e.Graphics.DrawImage(img, r.X + 3, r.Y + 3);
                }
            }
            else
            {
                DrawSelection(e.Graphics, r, false);
                if (!this.menuItem.Checked && this.image != null)
                {
                    e.Graphics.DrawImage(this.shadow, r.X + 4, r.Y + 4);
                    e.Graphics.DrawImage(this.image, r.X + 2, r.Y + 2);
                }
            }
            int str_x = r.X + 28;
            if (this.menuItem.Text == "-")
            {
                e.Graphics.DrawLine(SystemPens.ControlDark, str_x, r.Y + 1, r.Right, r.Y + 1);
                return;
            }
            if (this.menuItem.Checked)
            {
                string chk = (this.menuItem.RadioCheck) ? "h" :
                "a";
                Font f = new Font("Marlett", this.font.Size);
                Size sz_c = Size.Truncate(e.Graphics.MeasureString(chk, f));
                int sz_x = r.Left + (22 - sz_c.Width) / 2;
                int sz_y = r.Top + (r.Height - sz.Height) / 2;
                DrawSelection(e.Graphics, new Rectangle(r.Left, r.Top, 21, r.Height - 1), sel);
                e.Graphics.DrawString(chk, f, SystemBrushes.WindowText, sz_x, sz_y);
                f.Dispose();
            }
            e.Graphics.DrawString(this.menuItem.Text, this.font, brush, str_x, y, sf);
            if (this.menuItem.Shortcut != Shortcut.None && this.menuItem.ShowShortcut)
            {
                string st = GetShortcutText(this.menuItem.Shortcut);
                Size sz2 = Size.Truncate(e.Graphics.MeasureString(st, this.font));
                e.Graphics.DrawString(st, this.font, brush, r.Right - sz2.Width - 16, y);
            }
        }

        #endregion

        #region Static Methods

        public static implicit operator MenuItem(MenuItemEx mix)
        {
            return mix.MenuItem;
        }

        public static string GetFileName(string name)
        {
            string ret;
            if (MenuItemEx.IconPath != null)
            {
                ret = Path.Combine(MenuItemEx.IconPath, name);
            }
            else
            {
                ret = name;
            }
            if (MenuItemEx.Extension != null)
            {
                ret += MenuItemEx.Extension;
            }
            return ret;
        }

        public static Bitmap GetImage(string fileName)
        {
            string fn = MenuItemEx.GetFileName(fileName);
            if (!File.Exists(fn)) return null;
            Bitmap icon = new Bitmap(fn);
            Bitmap ret = new Bitmap(icon, 16, 16);
            icon.Dispose();
            return ret;
        }

        public static string GetShortcutText(Shortcut sc)
        {
            return sc.ToString().Replace("Shift", "Shift+").Replace("Ctrl", "Ctrl+").Replace("Alt", "Alt+");
        }

        public static void DrawSelection(Graphics g, Rectangle rect, bool dark)
        {
            Brush b = new SolidBrush(GetSelectionColor(dark));
            g.FillRectangle(b, rect);
            b.Dispose();
            g.DrawRectangle(SystemPens.Highlight, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        }

        public static Color GetSelectionColor(bool dark)
        {
            if (dark) return Mix(SystemColors.Highlight, SystemColors.Window);
            return Mix(SystemColors.Highlight, 2, SystemColors.Window, 5);
        }

        public static Bitmap MakeLight(Image img)
        {
            if (img == null) return null;
            Bitmap ret = new Bitmap(img);
            Size sz = ret.Size;
            for (int y = 0; y < sz.Height; y++)
            {
                for (int x = 0; x < sz.Width; x++)
                {
                    Color c = ret.GetPixel(x, y);
                    if (c.A < 1) continue;
                    ret.SetPixel(x, y, MakeLight(c));
                }
            }
            return ret;
        }

        public static Color MakeLight(Color c)
        {
            return Color.FromArgb(c.A, (int)((double)c.R * ((256.0 - 74.0) / 256.0) + 74.0), (int)((double)c.G * ((256.0 - 77.0) / 256.0) + 77.0), (int)((double)c.B * ((256.0 - 74.0) / 256.0) + 74.0));
        }

        public static Bitmap MakeShadow(Image img)
        {
            if (img == null) return null;
            Bitmap ret = new Bitmap(img);
            Size sz = ret.Size;
            for (int y = 0; y < sz.Height; y++)
            {
                for (int x = 0; x < sz.Width; x++)
                {
                    Color c = ret.GetPixel(x, y);
                    if (c.A < 1) continue;
                    ret.SetPixel(x, y, MakeShadow(c));
                }
            }
            return ret;
        }

        public static Color MakeShadow(Color c)
        {
            if (c.R + c.G + c.B < 500) return SystemColors.ControlDark;
            return Color.Transparent;
        }

        public static string RemoveMnemonic(string text)
        {
            int len = text.Length;
            if (len > 4 && text.EndsWith(")") && text.Substring(len - 4, 2) == "(&")
            {
                return text.Substring(0, len - 4);
            }
            return text;
        }

        #region From Girl.Drawing.ImageManipulator

        protected static Color Mix(Color c1, Color c2)
        {
            return Color.FromArgb((c1.A + c2.A) / 2, (c1.R + c2.R) / 2, (c1.G + c2.G) / 2, (c1.B + c2.B) / 2);
        }

        protected static Color Mix(Color c1, int r1, Color c2, int r2)
        {
            return Color.FromArgb((c1.A * r1 + c2.A * r2) / (r1 + r2), (c1.R * r1 + c2.R * r2) / (r1 + r2), (c1.G * r1 + c2.G * r2) / (r1 + r2), (c1.B * r1 + c2.B * r2) / (r1 + r2));
        }

        #endregion

        #endregion
    }
}
