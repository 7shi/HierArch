using System;
using System.Drawing;
using System.Windows.Forms;
using Girl.Drawing;
using Girl.Windows.Forms;

namespace Girl.Windows.Forms.Drawing2D
{
	public class CanvasButton : CanvasControl
	{
		public CanvasButton() : base(0F, 0F, 23F, 22F) {}
		
		public CanvasButton(Image image) : this()
		{
			this.Image = image;
		}
		
		public CanvasButton(string name, Image image) : this(image)
		{
			this.name = name;
		}
		
		public CanvasButton(string name, Image image, EventHandler click) : this(name, image)
		{
			this.Click += click;
		}
		
		public CanvasButton(string name, EventHandler click, string toolTip) : this()
		{
			this.name = name;
			this.Image = Girl.Windows.Forms.MenuItemEx.GetImage(name);
			this.Click += click;
			this.toolTipText = toolTip;
		}
		
		public CanvasButton(string name, EventHandler click, string toolTip, Styles style)
			: this(name, click, toolTip)
		{
			this.Style = style;
		}
		
		public CanvasButton(string name, Image image, MenuItem menuItem) : this(name, image)
		{
			this.MenuItem = menuItem;
		}
		
		public CanvasButton(MenuItem menuItem) : this()
		{
			this.MenuItem = menuItem;
		}
		
		public CanvasButton(Girl.Windows.Forms.MenuItemEx menuItemEx) : this()
		{
			this.MenuItemEx = menuItemEx;
		}
		
		public CanvasButton(object obj) : this()
		{
			if (obj is MenuItemEx)
			{
				this.MenuItemEx = obj as MenuItemEx;
			}
			else if (obj is MenuItem)
			{
				this.MenuItem = obj as MenuItem;
			}
		}
		
		public CanvasButton(Girl.Windows.Forms.MenuItemEx menuItemEx, EventHandler click)
			: this(menuItemEx)
		{
			this.Click += click;
		}
		
		public enum Styles
		{
			PushButton, ToggleButton
		}
		protected Image image;
		protected Bitmap light;
		protected Bitmap shadow;
		public bool Pushed;
		public Styles Style;
		protected MenuItem menuItem;

		protected override void Init()
		{
			base.Init();
			this.Pushed = false;
			this.Style = Styles.PushButton;
			this.menuItem = null;
			this.image = null;
			this.light = null;
			this.shadow = null;
		}

		public Image Image
		{
			get
			{
				return this.image;
			}

			set
			{
				if (this.light != null) this.light.Dispose();
				if (this.shadow != null) this.shadow.Dispose();
				this.image = value;
				if (value != null)
				{
					this.light = Girl.Windows.Forms.MenuItemEx.MakeLight(value);
					this.shadow = Girl.Windows.Forms.MenuItemEx.MakeShadow(value);
				}
				else
				{
					this.light = null;
					this.shadow = null;
				}
			}
		}

		public override void Draw(Graphics g)
		{
			if (!this.CheckDraw(g)) return;
			RectangleF r = this.rect;
			bool pushing =(this.button & MouseButtons.Left) != MouseButtons.None;
			int t1 = 0, t2 = 0;
			if (this.enabled)
			{
				if (!this.Pushed)
				{
					if (!pushing)
					{
						t1 = t2 =(!this.isUnderMouse) ? 0:
						1;
					}
					else
					{
						t1 = t2 =(this.isUnderMouse) ? 2:
						1;
					}
				}
				else
				{
					t1 =(!pushing && !this.isUnderMouse) ? 1:
					2;
					t2 = 2;
				}
			}
			else
			{
				t2 = 3;
			}
			if (t1 != 0)
			{
				Brush brush = new SolidBrush(Girl.Windows.Forms.MenuItemEx.GetSelectionColor(t1 != 1));
				g.FillRectangle(brush, r.X, r.Y, r.Width - 1F, r.Height - 1F);
				brush.Dispose();
				g.DrawRectangle(pen, r.X, r.Y, r.Width - 1F, r.Height - 1F);
			}
			if (this.Image == null) return;
			switch (t2)
			{
				case 0: g.DrawImage(this.light, r.X + 3F, r.Y + 3F);
				break;
				case 1: g.DrawImage(this.shadow, r.X + 4F, r.Y + 4F);
				g.DrawImage(this.image, r.X + 2F, r.Y + 2F);
				break;
				case 2: g.DrawImage(this.image, r.X + 3F, r.Y + 3F);
				break;
				case 3: g.DrawImage(this.shadow, r.X + 3F, r.Y + 3F);
				break;
			}
		}

		public virtual void PerformClick()
		{
			this.OnClick(EventArgs.Empty);
		}

		protected override void OnClick(EventArgs e)
		{
			if (this.Style == Styles.ToggleButton)
			{
				this.Pushed = !this.Pushed;
			}
			base.OnClick(e);
			if (this.menuItem != null) this.menuItem.PerformClick();
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			if (this.Style == Styles.ToggleButton)
			{
				this.Pushed = !this.Pushed;
			}
			base.OnDoubleClick(e);
			if (this.menuItem != null) this.menuItem.PerformClick();
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
				this.Enabled = value.Enabled;
				string tip = Girl.Windows.Forms.MenuItemEx.RemoveMnemonic(value.Text);
				if (value.Shortcut != Shortcut.None)
				{
					tip += " (" + Girl.Windows.Forms.MenuItemEx.GetShortcutText(value.Shortcut) + ")";
				}
				this.ToolTipText = tip;
			}
		}

		public MenuItemEx MenuItemEx
		{
			set
			{
				this.name = value.Name;
				this.MenuItem = value.MenuItem;
				this.Image = value.Image;
			}
		}
	}
}
