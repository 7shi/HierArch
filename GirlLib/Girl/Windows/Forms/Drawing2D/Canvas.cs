// ここにソースコードの注釈を書きます。

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using Girl.Drawing;

namespace Girl.Windows.Forms.Drawing2D
{
	/// <summary>
	/// 図形を描画するためコントロールに被せます。
	/// </summary>
	public class Canvas
	{
		#region Win32
		
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int x, y;
		}
		
		[DllImport("User32.dll")]
		public static extern IntPtr WindowFromPoint(
			POINT p  // 座標
		);
		
		#endregion
		
		public enum Messages
		{
			Click, DoubleClick,
			MouseUp, MouseDown, MouseMove, MouseEnter, MouseLeave
		}
		
		private static Bitmap defaultBitmap = null;
		public ArrayList Items;
		protected MouseButtons button;
		protected CanvasObject itemUnderMouse;
		protected ToolTip toolTip;
		protected bool visible;
		protected Matrix transform;
		protected Matrix invertedTransform;
		protected CanvasObject[] selectedItems;
		protected CanvasObject selectedItem;
		protected CanvasObjectSelection[] selections;
		protected CanvasPaintManager paintManager;
		public event EventHandler SelectionChanged;

		protected virtual void Init()
		{
			this.Items = new ArrayList();
			this.button = MouseButtons.None;
			this.itemUnderMouse = null;
			this.paintManager.Target.Click += new EventHandler(this.control_Click);
			this.paintManager.Target.DoubleClick += new EventHandler(this.control_DoubleClick);
			this.paintManager.Target.MouseDown += new MouseEventHandler(this.control_MouseDown);
			this.paintManager.Target.MouseUp += new MouseEventHandler(this.control_MouseUp);
			this.paintManager.Target.MouseMove += new MouseEventHandler(this.control_MouseMove);
			this.paintManager.Target.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.toolTip = new ToolTip();
			this.toolTip.AutoPopDelay = 20000;
			this.visible = true;
			this.transform = new Matrix();
			this.invertedTransform = null;
			this.selectedItems = null;
			this.selectedItem = null;
			this.selections = null;
		}

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public Canvas(Control control)
		{
			this.paintManager = new CanvasPaintManager(control);
			this.paintManager.CanvasCollection.Add(this);
			this.Init();
		}

		public Canvas(CanvasPaintManager paintManager)
		{
			this.paintManager = paintManager;
			this.Init();
		}

		#region Events

		public void Draw(PaintEventArgs e)
		{
			if (!this.visible) return;
			Graphics g = e.Graphics;
			CanvasObject[] targets = this.GetItemsAtInternal(e.ClipRectangle);
			float an = this.Angle;
			if (targets != null)
			{
				Array.Reverse(targets);
				this.Draw(targets, g, an, false, true);
				this.Draw(targets, g, an, false, false);
				this.Draw(targets, g, an, true, false);
			}
			if (this.selections == null) return;
			foreach (CanvasObjectSelection cosel in this.selections)
			{
				if (cosel == null) continue;
				cosel.Target.SetSelection(cosel, this.transform, an);
				cosel.Paint(e);
			}
		}

		protected void Draw(CanvasObject[] targets, Graphics g, float angle, bool frontMost, bool backMost)
		{
			Matrix old_m = g.Transform;
			foreach (CanvasObject co in targets)
			{
				if (!co.Visible || co.FrontMost != frontMost || co.BackMost != backMost) continue;
				float co_an = co.Angle;
				if (co.FixAngle) co_an -= angle;
				if (co_an == 0F)
				{
					g.Transform = this.transform;
					co.Draw(g);
				}
				else
				{
					Matrix m = this.transform.Clone();
					co.Rotate(m, angle);
					g.Transform = m;
					co.Draw(g);
					m.Dispose();
				}
			}
			g.Transform = old_m;
		}

		private void control_Click(object sender, EventArgs e)
		{
			CanvasControl c = this.itemUnderMouse as CanvasControl;
			if (c != null && c.IsUnderMouse && c.Enabled)
			{
				c.SendMessage(Messages.Click, e);
			}
		}

		private void control_DoubleClick(object sender, EventArgs e)
		{
			CanvasControl c = this.itemUnderMouse as CanvasControl;
			if (c != null && c.IsUnderMouse && c.Enabled)
			{
				c.SendMessage(Messages.DoubleClick, e);
			}
		}

		private void control_MouseDown(object sender, MouseEventArgs e)
		{
			this.button |= e.Button;
			CanvasControl c = this.itemUnderMouse as CanvasControl;
			if (c != null)
			{
				c.SendMessage(Messages.MouseDown, e);
				this.Invalidate(c);
			}
		}

		private void control_MouseUp(object sender, MouseEventArgs e)
		{
			this.button &= ~(e.Button);
			CanvasControl c = this.itemUnderMouse as CanvasControl;
			if (c != null)
			{
				c.SendMessage(Messages.MouseUp, e);
				this.Invalidate(c);
			}
			if (this.button == MouseButtons.None)
			{
				this.ItemUnderMouse = this.GetItemAt(this.paintManager.Target.PointToClient(Cursor.Position));
			}
		}

		private void control_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.button == MouseButtons.None)
			{
				this.ItemUnderMouse = this.GetItemAt(e.X, e.Y);
			}
			CanvasControl c = this.itemUnderMouse as CanvasControl;
			if (c != null)
			{
				c.SendMessage(Messages.MouseMove, e);
				bool um = c.Contains(this.transform, this.Angle, e.X, e.Y);
				if (um != c.IsUnderMouse)
				{
					if (um)
					{
						c.SendMessage(Messages.MouseEnter, EventArgs.Empty);
					}
					else
					{
						c.SendMessage(Messages.MouseLeave, EventArgs.Empty);
					}
					this.Invalidate(c);
				}
			}
		}

		private void control_MouseLeave(object sender, EventArgs e)
		{
			if (this.button == MouseButtons.None)
			{
				Point p1 = Cursor.Position;
				POINT p2 = new POINT();
				p2.x = p1.X;
				p2.y = p1.Y;
				IntPtr hWnd = WindowFromPoint(p2);
				if (hWnd == this.paintManager.Target.Handle)
				{
					this.ItemUnderMouse = this.GetItemAt(this.paintManager.Target.PointToClient(p1));
				}
				else
				{
					this.ItemUnderMouse = null;
				}
			}
		}

		#endregion

		#region Properties

		public CanvasPaintManager PaintManager
		{
			get
			{
				return this.paintManager;
			}
		}

		public bool Visible
		{
			get
			{
				return this.visible;
			}

			set
			{
				if (this.visible == value) return;
				this.visible = value;
				this.paintManager.Target.Invalidate();
			}
		}

		#endregion

		#region Item

		public void Invalidate(CanvasObject co)
		{
			if (co.ClientRectangles == null) return;
			Matrix m = this.transform.Clone();
			co.Rotate(m, this.Angle);
			float pw =(co.PenWidth + 2) * 3F;
			foreach (RectangleF r in co.ClientRectangles)
			{
				RectangleF r2 = r;
				r2.Inflate(pw, pw);
				Rectangle rect = Rectangle.Truncate(Geometry.TransformRectangle(m, r2));
				rect.Inflate(4, 4);
				this.paintManager.Target.Invalidate(rect);
			}
			if (true /*this.IsItemSelected(co)*/)
			{
				RectangleF r2 = co.ClientRectangle;
				r2.Inflate(pw, pw);
				Rectangle rect = Rectangle.Truncate(Geometry.TransformRectangle(m, r2));
				rect.Inflate(14, 14);
				this.paintManager.Target.Invalidate(rect);
			}
			m.Dispose();
		}

		public void Invalidate(CanvasObject[] cos)
		{
			foreach (CanvasObject co in cos) this.Invalidate(co);
		}

		public void Invalidate(CanvasGroup gr)
		{
			this.Invalidate(this.GetGroupItems(gr));
		}

		protected CanvasObject[] GetItemsAtInternal(RectangleF rect)
		{
			rect = Geometry.TransformRectangle(this.InvertedTransform, rect);
			ArrayList list = new ArrayList();
			// Macro: object配列(this.Items)を型(CanvasObject)だけ反復子(co)で評価
			{
				foreach (object __0_0 in this.Items)
				{
					CanvasObject co = __0_0 as CanvasObject;
					if (co == null) continue;
					// begin __YIELD
					if (co.Visible)
					{
						RectangleF rect2 = co.ClientRectangle;
						float w = Math.Max(rect2.Width, rect2.Height) * 1.5F + 2F;
						rect2 = new RectangleF(rect2.Left +(rect2.Width - w) / 2F, rect2.Top +(rect2.Height - w) / 2F, w, w);
						if (rect2.IntersectsWith(rect)) list.Add(co);
					}
					// end __YIELD
				}
			}
			if (list.Count < 1) return null;
			return list.ToArray(typeof (CanvasObject)) as CanvasObject[];
		}

		public CanvasObject GetItemAt(PointF pt)
		{
			CanvasObject[] targets = this.GetItemsAtInternal(new RectangleF(pt, new SizeF(1F, 1F)));
			if (targets == null) return null;
			float an = this.Angle;
			foreach (CanvasObject co in targets)
			{
				if (co.Contains(this.transform, an, pt)) return co;
			}
			return null;
		}

		public CanvasObject GetItemAt(float x, float y)
		{
			return this.GetItemAt(new PointF(x, y));
		}

		public CanvasObject[] GetItemsAt(PointF pt)
		{
			CanvasObject[] targets = this.GetItemsAtInternal(new RectangleF(pt, new SizeF(1F, 1F)));
			if (targets == null) return null;
			ArrayList list = new ArrayList();
			float an = this.Angle;
			foreach (CanvasObject co in targets)
			{
				if (co.Contains(this.transform, an, pt)) list.Add(co);
			}
			if (list.Count < 1) return null;
			return list.ToArray(typeof (CanvasObject)) as CanvasObject[];
		}

		public CanvasObject[] GetItemsAt(float x, float y)
		{
			return this.GetItemsAt(new PointF(x, y));
		}

		public CanvasObject[] GetItemsAt(RectangleF rect)
		{
			bool contains = true;
			if (rect.Width < 0)
			{
				rect.X += rect.Width;
				rect.Width = -rect.Width;
				contains = false;
			}
			if (rect.Height < 0)
			{
				rect.Y += rect.Height;
				rect.Height = -rect.Height;
			}
			CanvasObject[] targets = this.GetItemsAtInternal(rect);
			if (targets == null) return null;
			ArrayList list = new ArrayList();
			float an = this.Angle;
			foreach (CanvasObject co in targets)
			{
				if ((contains && co.IsContainedWith(this.transform, an, rect)) ||(!contains && co.IntersectsWith(this.transform, an, rect))) list.Add(co);
			}
			if (list.Count < 1) return null;
			return list.ToArray(typeof (CanvasObject)) as CanvasObject[];
		}

		public CanvasObject[] GetItemsAt(float x, float y, float width, float height)
		{
			return this.GetItemsAt(new RectangleF(x, y, width, height));
		}

		public CanvasObject ItemUnderMouse
		{
			get
			{
				return this.itemUnderMouse;
			}

			set
			{
				if (this.itemUnderMouse == value) return;
				if (this.itemUnderMouse != null)
				{
					CanvasControl c = this.itemUnderMouse as CanvasControl;
					if (c != null) c.SendMessage(Messages.MouseLeave, EventArgs.Empty);
					this.Invalidate(this.itemUnderMouse);
				}
				this.itemUnderMouse = value;
				if (value != null)
				{
					CanvasControl c = value as CanvasControl;
					if (c != null) c.SendMessage(Messages.MouseEnter, EventArgs.Empty);
					this.Invalidate(value);
				}
				if (value != null && value.ToolTipText != null)
				{
					this.toolTip.SetToolTip(this.paintManager.Target, value.ToolTipText);
				}
				else
				{
					this.toolTip.RemoveAll();
				}
			}
		}

		public static PointF GetItemsCenter(CanvasObject[] cos)
		{
			float cx = 0, cy = 0;
			foreach (CanvasObject co in cos)
			{
				PointF pt = co.CenterPoint;
				cx += pt.X;
				cy += pt.Y;
			}
			float len = cos.Length;
			return new PointF(cx / len, cy / len);
		}

		public CanvasCorner GetCornerAt(int x, int y)
		{
			if (this.selections == null) return null;
			foreach (CanvasObjectSelection cosel in this.selections)
			{
				if (cosel == null) continue;
				foreach (CanvasCorner corner in cosel.Corners)
				{
					if (!corner.Visible) continue;
					RectangleF r = corner.ClientRectangle;
					r.Inflate(2, 2);
					if (r.Contains(x, y)) return corner;
				}
			}
			return null;
		}

		public CanvasObjectSelection GetSelection(CanvasObject target)
		{
			if (this.selections == null) return null;
			foreach (CanvasObjectSelection cosel in this.selections)
			{
				if (cosel == null) continue;
				if (cosel.Target == target) return cosel;
			}
			return null;
		}

		public void ResizeItem(CanvasCorner corner, float x, float y, Keys modifier)
		{
			this.Invalidate(corner.Target);
			float an = this.Angle;
			corner.Target.Resize(corner, x, y, this.transform, an, modifier);
			corner.Target.SetSelection(corner.Selection, this.transform, an);
			this.Invalidate(corner.Target);
		}

		public static void RotateItems(CanvasObject[] cos, float angle)
		{
			PointF cpt = GetItemsCenter(cos);
			Matrix m = new Matrix();
			m.RotateAt(-angle, cpt);
			foreach (CanvasObject co in cos)
			{
				co.CenterPoint = Geometry.TransformPoint(m, co.CenterPoint);
				if (!co.FixAngle) co.Angle -= angle;
			}
			m.Dispose();
		}

		public static string SerializeItems(CanvasObject[] cos, string element, CanvasSerializer serializer, Matrix matrix)
		{
			StringWriter sw = new StringWriter();
			XmlTextWriter xw = new XmlTextWriter(sw);
			xw.Formatting = Formatting.Indented;
			xw.WriteStartDocument();
			xw.WriteStartElement(element);
			xw.WriteAttributeString("version", Application.ProductVersion);
			serializer.WriteMatrix(xw, "Transform", matrix);
			xw.WriteStartElement("Items");
			foreach (CanvasObject co in cos)
			{
				serializer.Write(xw, null, co);
			}
			xw.WriteEndElement();
			xw.WriteEndElement();
			xw.WriteEndDocument();
			xw.Close();
			sw.Close();
			return sw.ToString();
		}

		#region グループ

		public void Group(CanvasObject[] cos)
		{
			if (!CanvasGroup.CanGroup(cos)) return;
			CanvasGroup gr = new CanvasGroup();
			foreach (CanvasObject co in cos)
			{
				if (co.Group == null)
				{
					co.Group = gr;
				}
				else
				{
					CanvasGroup r = co.RootGroup;
					if (co.RootGroup != gr) co.RootGroup.Group = gr;
				}
			}
		}

		public void Ungroup(CanvasObject[] cos)
		{
			if (!CanvasGroup.CanUngroup(cos)) return;
			ArrayList list = new ArrayList();
			foreach (CanvasObject co in cos)
			{
				CanvasGroup r = co.RootGroup;
				if (r == null) continue;
				if (co.Group == r)
				{
					co.Group = null;
				}
				else if (!list.Contains(r))
				{
					list.Add(r);
				}
			}
			foreach (object obj in list)
			{
				CanvasGroup gr = obj as CanvasGroup;
				foreach (CanvasObject co in this.GetGroupItems(gr))
				{
					if (co.Group == gr)
					{
						co.Group = null;
					}
					else
					{
						co.Group.Ungroup(gr);
					}
				}
			}
		}

		protected CanvasObject[] AppendGroup(CanvasObject[] cos)
		{
			if (cos == null || cos.Length < 1) return null;
			ArrayList list1 = new ArrayList();
			ArrayList list2 = new ArrayList();
			foreach (CanvasObject co in cos)
			{
				CanvasGroup gr = co.RootGroup;
				if (gr != null && !list2.Contains(gr))
				{
					foreach (object obj in this.Items)
					{
						if (!list1.Contains(obj) &&(obj as CanvasObject).RootGroup == gr) list1.Add(obj);
					}
					list2.Add(gr);
				}
				else if (!list1.Contains(co))
				{
					list1.Add(co);
				}
			}
			return list1.ToArray(typeof (CanvasObject)) as CanvasObject[];
		}

		public CanvasObject[] GetGroupItems(CanvasGroup gr)
		{
			ArrayList list = new ArrayList();
			foreach (CanvasObject co in this.Items)
			{
				if (co.Group != null && co.Group.IsAncestorOf(gr)) list.Add(co);
			}
			return list.ToArray(typeof (CanvasObject)) as CanvasObject[];
		}

		#endregion

		public void SortByOrder(CanvasObject[] cos)
		{
			ArrayList list = new ArrayList();
			foreach (CanvasObject co in cos)
			{
				co.Order = this.Items.IndexOf(co);
				int i = 0;
				foreach (object obj in list)
				{
					if ((obj as CanvasObject).Order > co.Order)
					{
						list.Insert(i, co);
						i = -1;
						break;
					}
					i++;
				}
				if (i != -1) list.Add(co);
			}
			list.CopyTo(cos);
		}

		public Hashtable GetGuidTable()
		{
			if (this.Items.Count < 1) return null;
			Hashtable ret = new Hashtable();
			int i = 0;
			foreach (object obj in this.Items)
			{
				CanvasObject co = obj as CanvasObject;
				co.Order = i++;
				co.CheckGuid(ret);
				ret[co.Guid] = co;
			}
			return ret;
		}

		#endregion

		#region 選択

		protected void OnSelectionChanged(EventArgs e)
		{
			if (this.SelectionChanged != null) this.SelectionChanged(this, e);
		}

		public void SetSelection(CanvasObject[] sels, CanvasObject sel)
		{
			this.SetSelectedItems(sels, false);
			this.selectedItem = sel;
			this.OnSelectionChanged(EventArgs.Empty);
		}

		public void SetSelectedItems(CanvasObject[] sels, bool broadcast)
		{
			if (this.selectedItems != null)
			{
				foreach (CanvasObject co in this.selectedItems)
				{
					if (sels == null || Array.IndexOf(sels, co) < 0) this.Invalidate(co);
				}
			}
			this.selectedItems = this.AppendGroup(sels);
			if (this.selectedItems != null)
			{
				float an = this.Angle;
				int len = this.selectedItems.Length;
				CanvasObjectSelection[] cosels = new CanvasObjectSelection[len];
				Hashtable table = new Hashtable();
				if (this.selections != null)
				{
					foreach (CanvasObjectSelection cosel in this.selections)
					{
						table[cosel.Target] = cosel;
					}
				}
				int i = 0;
				foreach (CanvasObject co in this.selectedItems)
				{
					CanvasObjectSelection cosel = null;
					if (co.Enabled)
					{
						if (table.Contains(co))
						{
							cosel = table[co] as CanvasObjectSelection;
						}
						else
						{
							cosel = new CanvasObjectSelection(co);
							co.SetSelection(cosel, this.transform, an);
						}
					}
					cosels[i++] = cosel;
				}
				this.selections = cosels;
				this.Invalidate(this.selectedItems);
			}
			else
			{
				this.selections = null;
			}
			if (broadcast) this.OnSelectionChanged(EventArgs.Empty);
		}

		public CanvasObject SelectedItem
		{
			get
			{
				return this.selectedItem;
			}

			set
			{
				CanvasObject co = value;
				if (co != null && !co.Enabled) co = null;
				if (this.selectedItem == co) return;
				this.selectedItem = co;
				this.OnSelectionChanged(EventArgs.Empty);
			}
		}

		public CanvasObject[] SelectedItems
		{
			get
			{
				return this.selectedItems;
			}

			set
			{
				if (value != null && value.Length == 1)
				{
					this.SetSelection(value, value[0]);
				}
				else
				{
					this.SetSelection(value, null);
				}
			}
		}

		public bool IsItemSelected(CanvasObject co)
		{
			if (this.selectedItems == null || co == null) return false;
			return Array.IndexOf(this.selectedItems, co) >= 0;
		}

		public CanvasObject CheckSelectedItem(PointF pt)
		{
			if (this.selectedItems != null)
			{
				float an = this.Angle;
				foreach (CanvasObject sel in this.selectedItems)
				{
					if (sel.Contains(this.transform, an, pt)) return sel;
				}
			}
			return this.GetItemAt(pt);
		}

		public void SelectItem(CanvasObject item)
		{
			if (this.IsItemSelected(item))
			{
				this.SelectedItem = item;
				return;
			}
			if (item != null)
			{
				this.SetSelection(new CanvasObject[] { item }, item);
			}
			else
			{
				this.SetSelection(null, null);
			}
		}

		public void SelectItem(PointF pt)
		{
			this.SelectItem(this.CheckSelectedItem(pt));
		}

		public void SelectItem(float x, float y)
		{
			this.SelectItem(new PointF(x, y));
		}

		public void ToggledSelectItem(CanvasObject item)
		{
			if (item == null)
			{
				this.SelectedItem = null;
				return;
			}
			ArrayList list = new ArrayList();
			CanvasObject sel = null;
			bool exists = false;
			if (this.selectedItems != null)
			{
				foreach (CanvasObject co in this.selectedItems)
				{
					if (co != item)
					{
						list.Add(co);
					}
					else
					{
						exists = true;
					}
				}
			}
			if (!exists)
			{
				sel = item;
				list.Add(item);
			}
			if (list.Count > 0)
			{
				this.SetSelection(list.ToArray(typeof (CanvasObject)) as CanvasObject[], sel);
			}
			else
			{
				this.SetSelection(null, sel);
			}
		}

		public void ToggledSelectItem(PointF pt)
		{
			this.ToggledSelectItem(this.CheckSelectedItem(pt));
		}

		public void ToggledSelectItem(float x, float y)
		{
			this.ToggledSelectItem(new PointF(x, y));
		}

		public void SelectItem(Keys modifier, CanvasObject item)
		{
			if ((modifier & Keys.Shift) == Keys.Shift)
			{
				this.ToggledSelectItem(item);
			}
			else
			{
				this.SelectItem(item);
			}
		}

		public void SelectItem(Keys modifier, PointF pt)
		{
			if ((modifier & Keys.Shift) == Keys.Shift)
			{
				this.ToggledSelectItem(pt);
			}
			else
			{
				this.SelectItem(pt);
			}
		}

		public void SelectItem(Keys modifier, float x, float y)
		{
			this.SelectItem(modifier, new PointF(x, y));
		}

		public void SelectItems(CanvasObject[] items)
		{
			this.SelectedItems = items;
			this.SelectedItem = null;
		}

		public void SelectItems(RectangleF rect)
		{
			this.SelectItems(this.GetItemsAt(rect));
		}

		public void SelectItems(float x, float y, float width, float height)
		{
			this.SelectItems(this.GetItemsAt(x, y, width, height));
		}

		public void ToggledSelectItems(CanvasObject[] sels, CanvasObject[] items)
		{
			ArrayList list = new ArrayList();
			if (sels != null)
			{
				foreach (CanvasObject co in sels)
				{
					if (items == null || Array.IndexOf(items, co) < 0) list.Add(co);
				}
			}
			if (items != null)
			{
				foreach (CanvasObject co in items)
				{
					if (sels == null || Array.IndexOf(sels, co) < 0) list.Add(co);
				}
			}
			if (list.Count > 0)
			{
				this.SelectedItems = list.ToArray(typeof (CanvasObject)) as CanvasObject[];
			}
			else
			{
				this.SelectedItems = null;
			}
			this.SelectedItem = null;
		}

		public void ToggledSelectItems(CanvasObject[] sels, RectangleF rect)
		{
			this.ToggledSelectItems(sels, this.GetItemsAt(rect));
		}

		public void ToggledSelectItems(CanvasObject[] sels, float x, float y, float width, float height)
		{
			this.ToggledSelectItems(sels, this.GetItemsAt(x, y, width, height));
		}

		public void SelectItems(Keys modifier, CanvasObject[] sels, CanvasObject[] items)
		{
			if ((modifier & Keys.Shift) == Keys.Shift)
			{
				this.ToggledSelectItems(sels, items);
			}
			else
			{
				this.SelectItems(items);
			}
		}

		public void SelectItems(Keys modifier, CanvasObject[] sels, RectangleF rect)
		{
			this.SelectItems(modifier, sels, this.GetItemsAt(rect));
		}

		public void SelectItems(Keys modifier, CanvasObject[] sels, float x, float y, float width, float height)
		{
			this.SelectItems(modifier, sels, this.GetItemsAt(x, y, width, height));
		}

		#endregion

		#region アフィン変換

		public Matrix Transform
		{
			get
			{
				return this.transform;
			}

			set
			{
				this.transform = value;
			}
		}

		public Matrix InvertedTransform
		{
			get
			{
				if (this.invertedTransform != null) this.invertedTransform.Dispose();
				this.invertedTransform = this.transform.Clone() as Matrix;
				this.invertedTransform.Invert();
				return this.invertedTransform;
			}
		}

		public float Scale
		{
			get
			{
				return Geometry.GetScale(this.transform);
			}
		}

		public float Angle
		{
			get
			{
				return Geometry.GetAngle(this.transform);
			}
		}

		public void RotateAt(float angle, PointF pt)
		{
			this.transform.RotateAt(angle, pt);
			this.paintManager.Target.Invalidate();
		}

		public void ScaleAt(float scale, PointF pt)
		{
			Geometry.ScaleAt(this.transform, scale, scale, pt);
		}

		#endregion

		public static Graphics DefaultGraphics
		{
			get
			{
				if (defaultBitmap == null) defaultBitmap = new Bitmap(16, 16);
				return Graphics.FromImage(defaultBitmap);
			}
		}

		public static Bitmap CreateBitmap(Cursor cursor)
		{
			Size sz = cursor.Size;
			Bitmap ret = new Bitmap(sz.Width, sz.Height);
			Graphics g = Graphics.FromImage(ret);
			cursor.Draw(g, Rectangle.Empty);
			g.Dispose();
			return ret;
		}
	}
}
