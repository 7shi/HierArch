// ここにソースコードの注釈を書きます。

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Girl.Drawing;
using Girl.Windows.Forms;

namespace Girl.Windows.Forms.Drawing2D
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class DrawingBox : ControlEx
	{
		public const float WheelZoomUpScale = 6F / 5F;
		public const float WheelZoomDownScale = 1F / WheelZoomUpScale;
		
		public class Operation
		{
			public string Name;
			public Guid Guid;
			public object Tag;
			
			public Operation(string name) : this(name, Guid.Empty, null) {}
			public Operation(string name, object tag) : this(name, Guid.Empty, tag) {}
			public Operation(string name, Guid guid) : this(name, guid, null) {}
			
			public Operation(string name, Guid guid, object tag)
			{
				this.Name = name;
				this.Guid = guid;
				this.Tag = tag;
			}
		}
		protected CanvasPaintManager paintManager;
		protected Canvas canvas;
		protected Canvas frontCanvas;
		protected CanvasGroup group1;
		protected CanvasEllipse compass1;
		protected CanvasEllipse compass2;
		protected CanvasLine compass3;
		protected CanvasLine compass4;
		protected CanvasGroup group2;
		protected CanvasEllipse rotate1;
		protected CanvasImage rotate2;
		protected CanvasRectangle selBox;
		protected CanvasLine vline;
		protected CanvasLine hline;
		protected CanvasString zoomInfo;
		protected Canvas backCanvas;
		protected CanvasRectangle printBox;
		protected float standardAngle;
		protected string prevEvent;
		protected PointF prevPos;
		protected float origAngle;
		protected float prevAngle;
		protected CanvasObject[] preSels;
		protected CanvasCorner corner;
		protected PointF rotateCenter;
		protected Point cmnPos;
		public DrawingBoxContextMenu Menu;
		public ContextMenu cmnPath;
		public MenuItem mnuPathMakeVertex;
		public MenuItem mnuPathRemove;
		public ContextMenu cmnRightDrag;
		public MenuItem mnuRightDragMoveTo;
		public MenuItem mnuRightDragCopyTo;
		public ContextMenu cmnZoom;
		public CanvasSerializer Serializer;
		public event EventHandler Changed;
		protected Stack opsUndo;
		protected Stack opsRedo;
		protected CanvasObject addItem;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public DrawingBox()
		{
			this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			this.AcceptsArrow = true;
			this.BackColor = SystemColors.Window;
			this.prevEvent = null;
			this.standardAngle = 0F;
			this.prevPos = PointF.Empty;
			this.origAngle = this.prevAngle = 0F;
			this.preSels = null;
			this.corner = null;
			this.Serializer = new CanvasSerializer();
			this.rotateCenter = Point.Empty;
			this.cmnPos = Point.Empty;
			this.addItem = null;
			this.opsUndo = new Stack();
			this.opsRedo = new Stack();
			this.InitContextMenu();
			this.InitCanvas();
			this.Transform = new Matrix();
			this.canvas.SelectionChanged += new EventHandler(this.canvas_SelectionChanged);
		}

		protected virtual void InitContextMenu()
		{
			this.Menu = null;
			cmnPath = new ContextMenu(new MenuItem[] { mnuPathMakeVertex = new MenuItem("頂点化(&V)", new EventHandler(AddVertex)), mnuPathRemove = new MenuItem("削除(&D)", new EventHandler(RemoveVertex)) });
			cmnRightDrag = new ContextMenu(new MenuItem[] { mnuRightDragMoveTo = new MenuItem("ここへ移動(&M)", new EventHandler(MoveTo)), mnuRightDragCopyTo = new MenuItem("ここへコピー(&C)", new EventHandler(CopyTo)), new MenuItem("-"), new MenuItem("キャンセル(&A)") });
			cmnZoom = new ContextMenu();
			EventHandler ehz = new EventHandler(SetZoom);
			foreach (int z in new int [] { 1, 5, 10, 25, 50, 75, 100, 150, 200, 300, 400, 500, 600, 800, 1000, 1200, 1500, 2000, 2500, 3000 })
			{
				cmnZoom.MenuItems.Add(string.Format("{0}%", z), ehz);
			}
		}

		protected virtual void InitCanvas()
		{
			this.backCanvas = new Canvas(this);
			this.printBox = new CanvasRectangle();
			this.printBox.Pen = new Pen(Color.Indigo, 4);
			this.printBox.Visible = false;
			this.backCanvas.Items.Add(this.printBox);
			this.paintManager = new CanvasPaintManager(this);
			this.canvas = new Canvas(this.paintManager);
			this.paintManager.CanvasCollection.Add(this.canvas);
			this.InitFrontCanvas();
		}

		protected virtual void InitFrontCanvas()
		{
			this.frontCanvas = new Canvas(this);
			this.frontCanvas.PaintManager.SmoothingMode = SmoothingMode.AntiAlias;
			this.group1 = new CanvasGroup();
			this.compass1 = new CanvasEllipse(0, 0, 64, 64);
			this.compass1.Brush = new SolidBrush(Color.FromArgb(192, 192, 255, 255));
			this.compass1.Group = this.group1;
			this.compass2 = new CanvasEllipse(0, 0, 64, 64);
			this.compass2.Pen = new Pen(Color.Black, 2F);
			this.compass2.Group = this.group1;
			AdjustableArrowCap arrow = new AdjustableArrowCap(6, 6, false);
			this.compass3 = new CanvasLine(32, 0, 0, 64);
			this.compass3.Pen = new Pen(Color.FromArgb(192, 255, 128, 128), 2F);
			this.compass3.Pen.CustomStartCap = arrow;
			this.compass3.Group = this.group1;
			this.compass4 = new CanvasLine(0, 32, 64, 0);
			this.compass4.Pen = new Pen(Color.Black, 2F);
			this.compass4.Pen.CustomStartCap = arrow;
			this.compass4.Group = this.group1;
			this.selBox = new CanvasRectangle();
			this.selBox.Pen = new Pen(Color.Black);
			this.selBox.Brush = new SolidBrush(Color.Gray);
			this.SetSelBoxColor();
			this.selBox.Visible = false;
			this.group2 = new CanvasGroup();
			this.group2.Visible = false;
			Size sz = Cursors.NoMove2D.Size;
			this.rotate1 = new CanvasEllipse(0, 0, sz.Width, sz.Height);
			this.rotate1.Brush = new SolidBrush(Color.FromArgb(128, 192, 192, 192));
			this.rotate1.Group = this.group2;
			this.rotate2 = new CanvasImage(Canvas.CreateBitmap(Cursors.NoMove2D));
			this.rotate2.Group = this.group2;
			this.vline = new CanvasLine();
			this.hline = new CanvasLine();
			this.vline.Pen = this.hline.Pen = new Pen(Color.Red);
			this.vline.Visible = this.hline.Visible = false;
			this.zoomInfo = new CanvasString();
			this.frontCanvas.Items.AddRange(new object [] { this.selBox, this.vline, this.hline, this.rotate2, this.rotate1, this.zoomInfo, this.compass4, this.compass3, this.compass2, this.compass1 });
		}

		protected void SetSelBoxColor()
		{
			Color c1 = SystemColors.Highlight;
			Color c2 = ImageManipulator.Mix(c1, this.BackColor);
			this.selBox.Pen.Color = c1;
			(this.selBox.Brush as SolidBrush).Color = Color.FromArgb(128, c2.R, c2.G, c2.B);
		}

		#region Properties

		public Canvas Canvas
		{
			get
			{
				return this.canvas;
			}
		}

		public Canvas FrontCanvas
		{
			get
			{
				return this.frontCanvas;
			}
		}

		public Canvas BackCanvas
		{
			get
			{
				return this.backCanvas;
			}
		}

		public bool ShowCompass
		{
			get
			{
				return this.group1.Visible;
			}

			set
			{
				this.group1.Visible = value;
				this.frontCanvas.Invalidate(this.group1);
			}
		}

		public float StandardAngle
		{
			get
			{
				return this.standardAngle;
			}

			set
			{
				this.standardAngle = value;
				this.SetCompass();
			}
		}

		public CanvasPaintManager PaintManager
		{
			get
			{
				return this.paintManager;
			}
		}

		public bool CanCopy
		{
			get
			{
				CanvasObject[] sels = this.canvas.SelectedItems;
				return sels != null && sels.Length > 0;
			}
		}

		public bool CanPaste
		{
			get
			{
				return Clipboard.GetDataObject().GetDataPresent(this.DataFormat);
			}
		}

		public virtual Matrix Transform
		{
			get
			{
				return this.canvas.Transform;
			}

			set
			{
				this.backCanvas.Transform = this.canvas.Transform = value;
			}
		}

		public virtual string XmlElement
		{
			get
			{
				return "Canvas";
			}
		}

		public virtual string DataFormat
		{
			get
			{
				return this.XmlElement + "Items";
			}
		}

		public CanvasRectangle PrintBox
		{
			get
			{
				return this.printBox;
			}
		}

		public bool CanUndo
		{
			get
			{
				return this.opsUndo.Count > 0;
			}
		}

		public bool CanRedo
		{
			get
			{
				return this.opsRedo.Count > 0;
			}
		}

		#endregion

		#region Event Handlers

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			this.SetCompass();
			this.SetZoomInfo();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (!this.Focused) this.Focus();
			if (e.Button == MouseButtons.Middle)
			{
				this.prevEvent = this.eventStatus;
				this.OnCancelEvent(EventArgs.Empty);
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (this.vline.Visible) this.SetLines();
			this.SetCursor();
			if (e.Button == MouseButtons.None) return;
			base.OnMouseMove(e);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if (e.Delta == 0) return;
			PointF pt = this.PointToCanvas(this.rotateCenter);
			float sc = this.canvas.Scale, zoom;
			if (e.Delta < 0)
			{
				if (sc < 0.01F) return;
				zoom = DrawingBox.WheelZoomDownScale;
			}
			else
			{
				if (sc > 30F) return;
				zoom = DrawingBox.WheelZoomUpScale;
			}
			this.canvas.ScaleAt(zoom, this.PointToCanvas(e.X, e.Y));
			CanvasObject sel = this.canvas.SelectedItem;
			if (sel == null)
			{
				this.rotateCenter = this.PointFromCanvas(pt);
			}
			else
			{
				this.rotateCenter = this.PointFromCanvas(sel.CenterPoint);
			}
			this.rotate1.CenterPoint = this.rotate2.CenterPoint = this.rotateCenter;
			this.Invalidate();
			this.SetCursor();
			this.SetZoomInfo();
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);
			if (this.prevButton == MouseButtons.Middle)
			{
				this.OnCancelEvent(e);
				this.SetZoom(1, this.PointToCanvas(this.PointToClient(Cursor.Position)));
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			switch (e.KeyCode)
			{
				case Keys.Delete: this.Delete(this, EventArgs.Empty);
				break;
				case Keys.Home: this.MoveOrigin(this, EventArgs.Empty);
				break;
				case Keys.Left: this.ArrowMove(-1, 0, e.Modifiers);
				break;
				case Keys.Up: this.ArrowMove(0, -1, e.Modifiers);
				break;
				case Keys.Right: this.ArrowMove(1, 0, e.Modifiers);
				break;
				case Keys.Down: this.ArrowMove(0, 1, e.Modifiers);
				break;
			}
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if (e.KeyCode == Keys.Apps)
			{
				CanvasObject sel = this.canvas.SelectedItem;
				CanvasObject[] sels = this.canvas.SelectedItems;
				if (sel != null)
				{
					this.clickPoint = Point.Truncate(this.PointFromCanvas(sel.CenterPoint));
				}
				else
				{
					this.clickPoint = this.CenterPoint;
				}
				if (sels != null)
				{
					if (this.Menu != null) this.Menu.cmnItem.Show(this, this.clickPoint);
				}
				else
				{
					if (this.Menu != null) this.Menu.cmnCanvas.Show(this, this.clickPoint);
				}
			}
		}

		private void canvas_SelectionChanged(object sender, EventArgs e)
		{
			if (this.Menu != null) this.Menu.OnSetMenuEnabled(EventArgs.Empty);
		}

		#region 拡張イベント

		protected override void OnPrepareEvent(MouseEventArgs e)
		{
			this.corner = this.canvas.GetCornerAt(e.X, e.Y);
			if (this.eventStatus == "PrepareRotate") this.prevAngle = this.MouseAngle;
			if (this.eventStatus == null) base.OnPrepareEvent(e);
			this.preSels = this.canvas.SelectedItems;
		}

		protected override void OnStartEvent(EventArgs e)
		{
			CanvasObject co = this.canvas.GetItemAt(this.clickPoint);
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (this.mouseButtons == MouseButtons.Middle ||(this.mouseButtons == MouseButtons.Right && this.modifierShift && this.modifierControl))
			{
				this.prevPos = PointF.Empty;
				this.eventStatus = "CanvasMove";
			}
			else if (this.eventStatus == "PrepareSelectPrint")
			{
				this.UnsetLines();
				this.SetSelBoxColor();
				this.selBox.Pen.Color = ImageManipulator.Swap(this.selBox.Pen.Color, PrimaryColors.Red, PrimaryColors.Blue, PrimaryColors.Green);
				SolidBrush sb = this.selBox.Brush as SolidBrush;
				sb.Color = ImageManipulator.Swap(sb.Color, PrimaryColors.Red, PrimaryColors.Blue, PrimaryColors.Green);
				this.eventStatus = "SelectPrint";
			}
			else if (this.eventStatus == "PrepareRotate")
			{
				this.Cursor = Cursors.Hand;
				if (sels != null && sels.Length > 0)
				{
					foreach (CanvasObject sel in sels) sel.MemorizeStatus();
				}
				this.origAngle = this.canvas.Angle;
				this.eventStatus = "Rotate";
			}
			else if (this.eventStatus == "PrepareAddItem")
			{
				this.OnStartAddItem();
			}
			else if (this.mouseButtons == MouseButtons.Left)
			{
				if (this.modifierShift && this.modifierControl)
				{
					this.eventStatus = "SelectZoom";
				}
				else if (this.corner != null && this.corner.Target.Resizable)
				{
					this.corner.Target.MemorizeStatus();
					this.eventStatus = "ItemResize";
				}
				else if (co != null && !this.modifierShift)
				{
					this.OnStartItemMove();
				}
				else
				{
					this.eventStatus = "Select";
				}
			}
			else if (this.mouseButtons == MouseButtons.Right)
			{
				if (co != null) this.OnStartItemMove();
			}
			base.OnStartEvent(e);
		}

		protected virtual void OnStartItemMove()
		{
			this.canvas.SelectItem(this.clickPoint);
			foreach (CanvasObject sel in this.canvas.SelectedItems)
			{
				sel.MemorizeStatus();
			}
			this.eventStatus = "ItemMove";
		}

		protected virtual void OnStartAddItem()
		{
			this.eventStatus = "AddItem";
			this.UnsetLines();
			this.addItem.CenterPoint = this.PointToCanvas(this.clickPoint);
			this.addItem.Angle = -this.canvas.Angle;
			this.addItem.MemorizeStatus();
			this.canvas.Items.Insert(0, this.addItem);
			this.canvas.ToggledSelectItem(this.addItem);
			if (!this.addItem.Resizable) return;
			CanvasObjectSelection sel = this.canvas.GetSelection(this.addItem);
			this.canvas.ResizeItem(sel.Corners[0], this.clickPoint.X, this.clickPoint.Y, Keys.None);
			this.corner = sel.Corners[7];
		}

		protected override bool OnDispatchEvent(MouseEventArgs e)
		{
			switch (this.eventStatus)
			{
				case "ItemMove":
				this.OnItemMove(e);
				return true;
				case "ItemCopy":
				this.OnItemCopy(e);
				return true;
				case "ItemResize":
				this.OnItemResize(e);
				return true;
				case "CanvasMove":
				this.OnCanvasMove(e);
				return true;
				case "Rotate":
				this.OnRotate(e);
				return true;
				case "Select":
				this.OnSelect(e);
				return true;
				case "SelectPrint":
				this.OnSelectPrint(e);
				return true;
				case "AddItem":
				this.OnAddItem(e);
				return true;
				case "SelectZoom":
				this.OnSelectZoom(e);
				return true;
			}
			return base.OnDispatchEvent(e);
		}

		protected virtual void OnItemMove(MouseEventArgs e)
		{
			if (this.mouseButtons == MouseButtons.Left &&(Control.ModifierKeys & Keys.Control) != 0)
			{
				this.CancelMove();
				this.CopyItems();
				this.eventStatus = "ItemCopy";
			}
			this.MoveItems(e.X, e.Y);
		}

		protected virtual void OnItemCopy(MouseEventArgs e)
		{
			if ((Control.ModifierKeys & Keys.Control) == 0)
			{
				CanvasObject[] sels = this.canvas.SelectedItems;
				this.canvas.SelectedItems = this.preSels;
				foreach (CanvasObject sel in sels) this.canvas.Items.Remove(sel);
				this.eventStatus = "ItemMove";
			}
			this.MoveItems(e.X, e.Y);
		}

		protected virtual void OnItemResize(MouseEventArgs e)
		{
			this.canvas.ResizeItem(this.corner, e.X, e.Y, Control.ModifierKeys);
			this.Cursor = this.corner.Cursor;
		}

		protected virtual void OnCanvasMove(MouseEventArgs e)
		{
			if (this.noMove) this.Cursor = Cursors.Hand;
			PointF pt = this.GetMouseDiff(e.X, e.Y);
			this.Transform.Translate(pt.X - this.prevPos.X, pt.Y - this.prevPos.Y);
			this.prevPos = pt;
			this.Invalidate();
		}

		protected virtual void OnRotate(MouseEventArgs e)
		{
			float an = this.MouseAngle;
			this.Rotate(an - this.prevAngle);
			this.prevAngle = an;
		}

		protected virtual void OnSelect(MouseEventArgs e)
		{
			if (this.selBox.Visible)
			{
				this.frontCanvas.Invalidate(this.selBox);
			}
			else
			{
				this.selBox.Visible = true;
			}
			this.selBox.Bounds = new RectangleF(this.clickPoint, new SizeF(e.X - this.clickPoint.X, e.Y - this.clickPoint.Y));
			this.frontCanvas.Invalidate(this.selBox);
			this.canvas.SelectItems(this.modifierKeys, this.preSels, this.selBox.Bounds);
		}

		protected virtual void OnSelectPrint(MouseEventArgs e)
		{
			if (this.selBox.Visible)
			{
				this.frontCanvas.Invalidate(this.selBox);
			}
			else
			{
				this.selBox.Visible = true;
			}
			this.selBox.Bounds = new RectangleF(this.clickPoint, new SizeF(e.X - this.clickPoint.X, e.Y - this.clickPoint.Y));
			this.frontCanvas.Invalidate(this.selBox);
		}

		protected virtual void OnAddItem(MouseEventArgs e)
		{
			this.canvas.Invalidate(this.addItem);
			if (!this.addItem.Resizable) return;
			this.canvas.ResizeItem(this.corner, e.X, e.Y, Keys.None);
		}

		protected virtual void OnSelectZoom(MouseEventArgs e)
		{
			if (this.selBox.Visible)
			{
				this.frontCanvas.Invalidate(this.selBox);
			}
			else
			{
				this.selBox.Visible = true;
			}
			this.selBox.Bounds = new RectangleF(this.clickPoint, new SizeF(e.X - this.clickPoint.X, e.Y - this.clickPoint.Y));
			this.frontCanvas.Invalidate(this.selBox);
		}

		protected override void OnCancelEvent(EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			switch (this.eventStatus)
			{
				case "ItemMove":
				this.CancelMove();
				break;
				case "ItemCopy":
				this.canvas.SelectedItems = null;
				if (sels != null)
				{
					foreach (CanvasObject sel in sels)
					{
						this.canvas.Items.Remove(sel);
					}
				}
				this.canvas.SelectedItems = this.preSels;
				break;
				case "ItemResize":
				this.canvas.Invalidate(this.corner.Target);
				this.corner.Target.CancelResize();
				this.canvas.Invalidate(this.corner.Target);
				this.corner = null;
				break;
				case "PrepareSelectPrint":
				break;
				case "Select":
				this.selBox.Visible = false;
				this.canvas.SelectedItems = this.preSels;
				this.Invalidate();
				break;
				case "SelectPrint":
				this.selBox.Visible = false;
				this.Invalidate();
				this.SetSelBoxColor();
				break;
				case "PrepareRotate":
				this.frontCanvas.Invalidate(this.group2);
				this.group2.Visible = false;
				break;
				case "Rotate":
				this.frontCanvas.Invalidate(this.group2);
				this.group2.Visible = false;
				if (sels == null)
				{
					this.Rotate(this.origAngle - this.canvas.Angle);
				}
				else
				{
					foreach (CanvasObject sel in sels) sel.RestoreStatus();
				}
				this.Invalidate();
				break;
				case "CanvasMove":
				this.Transform.Translate(-this.prevPos.X, -this.prevPos.Y);
				this.Invalidate();
				break;
				case "PrepareAddItem":
				this.addItem = null;
				break;
				case "AddItem":
				this.addItem = null;
				break;
				case "SelectZoom":
				this.selBox.Visible = false;
				this.Invalidate();
				break;
			}
			base.OnCancelEvent(e);
			this.SetCursor();
			this.UnsetLines();
		}

		protected override void OnEndEvent(EventArgs e)
		{
			switch (this.eventStatus)
			{
				case "ItemMove":
				this.OnItemMoved();
				break;
				case "ItemCopy":
				this.OnItemCopied();
				break;
				case "ItemResize":
				this.OnItemResized();
				break;
				case "CanvasMove":
				this.OnCanvasMoved();
				break;
				case "PrepareRotate":
				this.frontCanvas.Invalidate(this.group2);
				this.group2.Visible = false;
				break;
				case "Rotate":
				this.OnRotated();
				break;
				case "Select":
				this.OnSelected();
				break;
				case "SelectPrint":
				this.OnSelectedPrint();
				break;
				case "PrepareAddItem":
				this.addItem.CenterPoint = this.PointToCanvas(this.clickPoint);
				this.addItem.Angle = -this.canvas.Angle;
				this.canvas.Items.Insert(0, this.addItem);
				this.Canvas.ToggledSelectItem(this.addItem);
				this.OnAddedItem();
				break;
				case "AddItem":
				this.OnAddedItem();
				break;
				case "SelectZoom":
				this.OnSelectedZoom();
				break;
			}
			string pe = this.prevEvent;
			this.prevEvent = null;
			base.OnEndEvent(e);
			this.SetCursor();
			this.UnsetLines();
			if (pe == "PrepareSelectPrint")
			{
				this.SelectPrint(this, EventArgs.Empty);
			}
		}

		protected virtual void OnItemMoved()
		{
			if (this.mouseButtons == MouseButtons.Right)
			{
				this.CancelMove();
				this.cmnPos = this.PointToClient(Cursor.Position);
				this.cmnRightDrag.Show(this, this.cmnPos);
				return;
			}
			this.SetUndoBounds(this.canvas.SelectedItems);
		}

		protected virtual void OnItemCopied()
		{
			this.SetUndoDelete(this.canvas.SelectedItems);
		}

		protected virtual void OnItemResized()
		{
			this.corner.Target.EndResize();
			this.SetUndoBounds(new CanvasObject[] { this.corner.Target });
		}

		protected virtual void OnCanvasMoved()
		{
			this.Cursor = Cursors.Default;
		}

		protected virtual void OnRotated()
		{
			this.frontCanvas.Invalidate(this.group2);
			this.group2.Visible = false;
			this.Cursor = Cursors.Default;
			this.SetUndoAngle(this.canvas.SelectedItems);
		}

		protected virtual void OnSelected()
		{
			this.selBox.Visible = false;
			this.Invalidate();
		}

		protected virtual void OnSelectedPrint()
		{
			this.Cursor = Cursors.Default;
			this.selBox.Visible = false;
			this.Invalidate();
			this.SetSelBoxColor();
			float sc = this.canvas.Scale;
			this.SetPrintArea(this.PointToCanvas(this.selBox.CenterPoint), this.selBox.Width / sc, this.selBox.Height / sc);
		}

		protected virtual void OnAddedItem()
		{
			this.SetUndoDelete(new CanvasObject[] { this.addItem });
			this.addItem = null;
		}

		protected virtual void OnSelectedZoom()
		{
			this.selBox.Visible = false;
			PointF cpt = this.PointToCanvas(this.selBox.CenterPoint);
			float sc = this.canvas.Scale;
			float w = this.selBox.Width / sc, h = this.selBox.Height / sc;
			RectangleF r = new RectangleF(cpt.X - w / 2, cpt.Y - h / 2, w, h);
			this.SetDisplayArea(r, -this.canvas.Angle);
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			switch (e.Button)
			{
				case MouseButtons.XButton1: this.canvas.SelectItem(e.X, e.Y);
				this.rotateCenter = new Point(e.X, e.Y);
				this.RotateLeft90(this, EventArgs.Empty);
				break;
				case MouseButtons.XButton2: this.canvas.SelectItem(e.X, e.Y);
				this.rotateCenter = new Point(e.X, e.Y);
				this.RotateRight90(this, EventArgs.Empty);
				break;
			}
			base.OnMouseClick(e);
		}

		protected override void OnLeftClick(MouseEventArgs e)
		{
			base.OnLeftClick(e);
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (this.modifierShift && this.modifierControl)
			{
				this.OnMouseWheel(new MouseEventArgs(MouseButtons.None, 0, e.X, e.Y, 120));
			}
			else
			{
				this.canvas.SelectItem(this.modifierKeys, this.clickPoint);
			}
		}

		protected override void OnRightClick(MouseEventArgs e)
		{
			base.OnRightClick(e);
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (this.modifierShift && this.modifierControl)
			{
				this.OnMouseWheel(new MouseEventArgs(MouseButtons.None, 0, e.X, e.Y, -120));
			}
			else if (this.zoomInfo.ClientRectangle.Contains(e.X, e.Y))
			{
				this.cmnZoom.Show(this, new Point(e.X, e.Y));
			}
			else if (this.corner != null)
			{
				if (this.corner.Index >= 8)
				{
					bool flg = this.corner.Type == 'L';
					this.mnuPathMakeVertex.Visible = flg;
					flg = !flg;
					this.mnuPathRemove.Visible = flg;
					if (flg)
					{
						CanvasPolygon poly = this.corner.Target as CanvasPolygon;
						this.mnuPathRemove.Enabled = poly.Points.Length > 2;
					}
					this.cmnPath.Show(this, new Point(e.X, e.Y));
				}
			}
			else if (this.Menu != null)
			{
				this.canvas.SelectItem(e.X, e.Y);
				CanvasObject sel = this.canvas.SelectedItem;
				if (sel != null)
				{
					this.Menu.cmnItem.Show(this, new Point(e.X, e.Y));
				}
				else
				{
					this.Menu.cmnCanvas.Show(this, new Point(e.X, e.Y));
				}
			}
		}

		protected override void OnMiddleClick(MouseEventArgs e)
		{
			base.OnMiddleClick(e);
			CanvasObject[] sels = this.canvas.SelectedItems;
			this.canvas.SelectItem(this.modifierKeys, this.clickPoint);
			this.RotateFree(this, EventArgs.Empty);
		}

		#endregion

		#endregion

		#region 座標

		public Point CenterPoint
		{
			get
			{
				Size sz = this.ClientSize;
				return new Point(sz.Width / 2, sz.Height / 2);
			}
		}

		public PointF CenterPointInCanvas
		{
			get
			{
				return this.PointToCanvas(this.CenterPoint);
			}
		}

		public PointF PointToCanvas(PointF pt)
		{
			return Geometry.TransformPoint(this.canvas.InvertedTransform, pt);
		}

		public PointF PointToCanvas(float x, float y)
		{
			return this.PointToCanvas(new PointF(x, y));
		}

		public PointF PointFromCanvas(PointF pt)
		{
			return Geometry.TransformPoint(this.canvas.Transform, pt);
		}

		public PointF PointFromCanvas(float x, float y)
		{
			return this.PointFromCanvas(new PointF(x, y));
		}

		protected PointF GetMouseDiff(int x, int y)
		{
			PointF pt = new PointF(x, y);
			if ((Control.ModifierKeys & Keys.Shift) != 0 && !(this.eventStatus == "CanvasMove" &&(Control.ModifierKeys & Keys.Control) != 0))
			{
				pt = Geometry.GetRightAngled(pt, this.clickPoint);
			}
			PointF[] pts = new PointF[]
			{
				this.clickPoint, pt
			}
			;
			this.canvas.InvertedTransform.TransformPoints(pts);
			return new PointF(pts[1].X - pts[0].X, pts[1].Y - pts[0].Y);
		}

		public void SetZoom(float zoom, PointF pt)
		{
			this.canvas.ScaleAt(zoom / this.canvas.Scale, pt);
			this.Invalidate();
			this.SetCursor();
			this.SetZoomInfo();
		}

		public void SetDisplayArea(RectangleF rect, float angle)
		{
			RectangleF r = this.ClientRectangle;
			float zx = Math.Abs(r.Width / rect.Width);
			float zy = Math.Abs(r.Height / rect.Height);
			float zoom =(zx < zy) ? zx:
			zy;
			PointF cpt1 = Geometry.GetCenter(rect);
			Matrix m = new Matrix();
			m.Translate(-cpt1.X, -cpt1.Y);
			if (zoom < 0.01F) zoom = 0.01F;
			if (zoom > 30F) zoom = 30F;
			Geometry.ScaleAt(m, zoom, zoom, cpt1);
			Matrix mm = m.Clone();
			mm.Invert();
			PointF cpt2 = Geometry.TransformPoint(mm, Geometry.GetCenter(r));
			mm.Dispose();
			m.Translate(cpt2.X - cpt1.X, cpt2.Y - cpt1.Y);
			m.RotateAt(-angle, cpt1);
			this.Transform = m;
			this.SetCompass();
			this.SetZoomInfo();
			this.Invalidate();
		}

		#region 移動

		protected void CancelMove()
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null) return;
			foreach (CanvasObject co in sels)
			{
				this.canvas.Invalidate(co);
				co.RestoreStatus();
				this.canvas.Invalidate(co);
			}
		}

		public void MoveItems(int x, int y)
		{
			PointF d = this.GetMouseDiff(x, y);
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null)
			{
				this.Cancel();
				return;
			}
			foreach (CanvasObject sel in sels)
			{
				this.canvas.Invalidate(sel);
				sel.RestoreStatus();
				sel.Offset(d);
				this.canvas.Invalidate(sel);
			}
		}

		public void CopyItems()
		{
			string xml = this.SerializeItems(this.canvas.SelectedItems);
			CanvasObject[] cos = this.DeserializeItems(xml);
			Hashtable guidTable = this.canvas.GetGuidTable();
			foreach (CanvasObject co in cos) co.CheckGuid(guidTable);
			this.canvas.Items.InsertRange(0, cos);
			this.canvas.SelectedItems = cos;
		}

		protected void ArrowMove(float dx, float dy, Keys modifiers)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if ((modifiers & Keys.Shift) == 0)
			{
				dx = dx * 16;
				dy = dy * 16;
			}
			Matrix i = this.canvas.InvertedTransform;
			if (sels == null)
			{
				PointF[] pts = new PointF[]
				{
					new PointF(dx, dy), PointF.Empty
				}
				;
				i.TransformPoints(pts);
				this.canvas.Transform.Translate(pts[1].X - pts[0].X, pts[1].Y - pts[0].Y);
				this.Invalidate();
				return;
			}
			foreach (CanvasObject sel in sels)
			{
				sel.MemorizeStatus();
				this.canvas.Invalidate(sel);
				PointF cpt = Geometry.TransformPoint(this.canvas.Transform, sel.CenterPoint);
				cpt.X += dx;
				cpt.Y += dy;
				sel.CenterPoint = Geometry.TransformPoint(i, cpt);
				this.canvas.Invalidate(sel);
			}
			this.SetUndoBounds(sels);
		}

		#endregion

		#region 回転

		public void SetCompass()
		{
			double an;
			double s;
			double c;

			if (!this.ShowCompass) return;
			this.frontCanvas.Invalidate(this.compass1);
			SizeF sz = this.ClientSize;
			this.compass1.Location = this.compass2.Location = new PointF(16, sz.Height - 80);
			an =(double) this.standardAngle / 180D * Math.PI - Math.PI / 2D;
			s = Math.Sin(an) * 30D;
			c = Math.Cos(an) * 30D;
			this.compass3.Bounds = new RectangleF((float) (48D + c),(float) ((double) sz.Height - 48D + s),(float) (c *(-2D)),(float) (s *(-2D)));
			an =(double) this.canvas.Angle / 180D * Math.PI - Math.PI / 2D;
			s = Math.Sin(an) * 30D;
			c = Math.Cos(an) * 30D;
			this.compass4.Bounds = new RectangleF((float) (48D + c),(float) ((double) sz.Height - 48D + s),(float) (c *(-2D)),(float) (s *(-2D)));
			this.frontCanvas.Invalidate(this.compass1);
		}

		public void RotateAt(float angle, PointF pt)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null)
			{
				this.canvas.RotateAt(angle, pt);
				this.SetCompass();
				return;
			}
			foreach (CanvasObject sel in sels)
			{
				this.canvas.Invalidate(sel);
				sel.Angle += angle;
				this.canvas.Invalidate(sel);
			}
		}

		public void Rotate(float angle)
		{
			this.RotateAt(angle, this.PointToCanvas(this.rotateCenter));
		}

		public float MouseAngle
		{
			get
			{
				CanvasObject sel = this.canvas.SelectedItem;
				Point pt = this.PointToClient(Cursor.Position);
				return Geometry.GetAngle(pt, this.rotateCenter);
			}
		}

		#endregion

		#region 情報

		protected void SetLines()
		{
			if (!this.vline.Visible)
			{
				this.vline.Visible = this.hline.Visible = true;
			}
			else
			{
				this.frontCanvas.Invalidate(this.vline);
				this.frontCanvas.Invalidate(this.hline);
			}
			PointF pt = this.PointToClient(Cursor.Position);
			SizeF sz = this.ClientSize;
			vline.Bounds = new RectangleF(pt.X, 0, 0, sz.Height);
			hline.Bounds = new RectangleF(0, pt.Y, sz.Width, 0);
			this.frontCanvas.Invalidate(this.vline);
			this.frontCanvas.Invalidate(this.hline);
		}

		public void UnsetLines()
		{
			if (!this.vline.Visible) return;
			this.frontCanvas.Invalidate(this.vline);
			this.frontCanvas.Invalidate(this.hline);
			this.vline.Visible = this.hline.Visible = false;
		}

		public void SetPrintArea(PointF cpt, float width, float height)
		{
			this.printBox.Size = new SizeF(width, height);
			this.printBox.CenterPoint = cpt;
			this.printBox.Visible = true;
			this.printBox.Angle = -this.canvas.Angle;
			this.Invalidate();
		}

		public void SetZoomInfo()
		{
			if (!this.zoomInfo.Visible) return;
			this.frontCanvas.Invalidate(this.zoomInfo);
			this.zoomInfo.Text = string.Format("{0:0.0}%", this.canvas.Scale * 100);
			SizeF sz1 = this.ClientSize;
			SizeF sz2 = this.zoomInfo.Size;
			this.zoomInfo.Location = new PointF(sz1.Width - sz2.Width - 16, sz1.Height - sz2.Height - 16);
			this.frontCanvas.Invalidate(this.zoomInfo);
		}

		public void SetCursor()
		{
			if (this.mouseButtons != MouseButtons.None || this.eventStatus != null)
			{
				return;
			}
			Cursor cur = Cursors.Default;
			Point pt = this.PointToClient(Cursor.Position);
			CanvasCorner corner = this.canvas.GetCornerAt(pt.X, pt.Y);
			if (corner != null) cur = corner.Cursor;
			if (this.Cursor != cur) this.Cursor = cur;
		}

		#endregion

		#endregion

		#region Context Menu

		public void ResetClickPos(object sender, EventArgs e)
		{
			this.clickPoint = this.CenterPoint;
		}

		public void SetZoom(object sender, EventArgs e)
		{
			MenuItem mni = sender as MenuItem;
			if (mni == null) return;
			string text = mni.Text;
			float sc = float.Parse(text.Substring(0, text.Length - 1)) / 100;
			this.SetZoom(sc, this.CenterPointInCanvas);
		}

		#region 回転・反転

		public void RotateFree(object sender, EventArgs e)
		{
			this.Cancel();
			this.rotateCenter = this.clickPoint;
			CanvasObject sel = this.canvas.SelectedItem;
			if (sel != null)
			{
				this.rotateCenter = this.PointFromCanvas(sel.CenterPoint);
			}
			this.group2.Visible = true;
			this.rotate1.CenterPoint = this.rotate2.CenterPoint = this.rotateCenter;
			this.frontCanvas.Invalidate(this.group2);
			this.eventStatus = "PrepareRotate";
		}

		public void RotateLeft90(object sender, EventArgs e)
		{
			this.rotateCenter = this.clickPoint;
			this.Rotate(-90F);
			this.SetUndoAngle(this.canvas.SelectedItems);
		}

		public void RotateRight90(object sender, EventArgs e)
		{
			this.rotateCenter = this.clickPoint;
			this.Rotate(90F);
			this.SetUndoAngle(this.canvas.SelectedItems);
		}

		public void Rotate180(object sender, EventArgs e)
		{
			this.rotateCenter = this.clickPoint;
			this.Rotate(180F);
			this.SetUndoAngle(this.canvas.SelectedItems);
		}

		public void RotateAxis(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null)
			{
				this.rotateCenter = this.clickPoint;
				this.Rotate(-this.canvas.Angle);
				return;
			}
			foreach (CanvasObject sel in sels)
			{
				this.canvas.Invalidate(sel);
				sel.MemorizeStatus();
				sel.Angle = 0F;
				this.canvas.Invalidate(sel);
			}
			this.SetUndoAngle(this.canvas.SelectedItems);
		}

		public void RotateCanvas(object sender, EventArgs e)
		{
			CanvasObject sel = this.canvas.SelectedItem;
			if (sel == null) return;
			this.canvas.RotateAt(-this.canvas.Angle - sel.Angle, sel.CenterPoint);
			this.SetCompass();
		}

		public void RotateToCanvas(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null) return;
			foreach (CanvasObject sel in sels)
			{
				this.canvas.Invalidate(sel);
				sel.MemorizeStatus();
				sel.Angle = -this.canvas.Angle;
				this.canvas.Invalidate(sel);
			}
			this.SetUndoAngle(this.canvas.SelectedItems);
		}

		public void RotateStandard(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null)
			{
				this.rotateCenter = this.clickPoint;
				this.Rotate(this.standardAngle - this.canvas.Angle);
				return;
			}
			foreach (CanvasObject sel in sels)
			{
				this.canvas.Invalidate(sel);
				sel.MemorizeStatus();
				sel.Angle = this.standardAngle;
				this.canvas.Invalidate(sel);
			}
			this.SetUndoAngle(this.canvas.SelectedItems);
		}

		public void SetStandard(object sender, EventArgs e)
		{
			this.StandardAngle = this.canvas.Angle;
		}

		public void FixAngle(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null) return;
			foreach (CanvasObject sel in sels)
			{
				if (sel.FixAngle)
				{
					sel.Angle -= this.canvas.Angle;
				}
				else
				{
					sel.Angle += this.canvas.Angle;
				}
				sel.FixAngle = !sel.FixAngle;
			}
			this.Change();
		}

		public void MirrorH(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null) return;
			foreach (CanvasObject sel in sels)
			{
				this.canvas.Invalidate(sel);
				sel.MemorizeStatus();
				RectangleF rect = sel.Bounds;
				rect.X += rect.Width;
				rect.Width = -rect.Width;
				sel.Bounds = rect;
				this.canvas.Invalidate(sel);
			}
			this.SetUndoBounds(sels);
		}

		public void MirrorV(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null) return;
			foreach (CanvasObject sel in sels)
			{
				this.canvas.Invalidate(sel);
				sel.MemorizeStatus();
				RectangleF rect = sel.Bounds;
				rect.Y += rect.Height;
				rect.Height = -rect.Height;
				sel.Bounds = rect;
				this.canvas.Invalidate(sel);
			}
			this.SetUndoBounds(sels);
		}

		public void MoveOrigin(object sender, EventArgs e)
		{
			this.canvas.Transform.Reset();
			this.SetCompass();
			this.SetZoomInfo();
			this.Invalidate();
		}

		#endregion

		#region 配置

		public void ArrangeLeft(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null || sels.Length < 1) return;
			RectangleF[] rects = new RectangleF[sels.Length];
			Matrix m = this.canvas.Transform;
			float x = 0F;
			for (int i = 0; i < sels.Length; i++)
			{
				rects[i] = Geometry.TransformRectangle(m, sels[i].Bounds);
				float xx = rects[i].Left;
				if (i == 0 || x > xx) x = xx;
			}
			m = this.canvas.InvertedTransform;
			for (int i = 0; i < sels.Length; i++)
			{
				this.canvas.Invalidate(sels[i]);
				sels[i].MemorizeStatus();
				sels[i].CenterPoint = Geometry.TransformPoint(m, x + rects[i].Width / 2F, rects[i].Top + rects[i].Height / 2F);
				this.canvas.Invalidate(sels[i]);
			}
			this.SetUndoBounds(sels);
		}

		public void ArrangeCenterH(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null || sels.Length < 1) return;
			RectangleF[] rects = new RectangleF[sels.Length];
			Matrix m = this.canvas.Transform;
			float x = 0F;
			for (int i = 0; i < sels.Length; i++)
			{
				rects[i] = Geometry.TransformRectangle(m, sels[i].Bounds);
				x += rects[i].Left + rects[i].Width / 2F;
			}
			x /=(float) sels.Length;
			m = this.canvas.InvertedTransform;
			for (int i = 0; i < sels.Length; i++)
			{
				this.canvas.Invalidate(sels[i]);
				sels[i].MemorizeStatus();
				sels[i].CenterPoint = Geometry.TransformPoint(m, x, rects[i].Top + rects[i].Height / 2F);
				this.canvas.Invalidate(sels[i]);
			}
			this.SetUndoBounds(sels);
		}

		public void ArrangeRight(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null || sels.Length < 1) return;
			RectangleF[] rects = new RectangleF[sels.Length];
			Matrix m = this.canvas.Transform;
			float x = 0F;
			for (int i = 0; i < sels.Length; i++)
			{
				rects[i] = Geometry.TransformRectangle(m, sels[i].Bounds);
				float xx = rects[i].Right;
				if (i == 0 || x < xx) x = xx;
			}
			m = this.canvas.InvertedTransform;
			for (int i = 0; i < sels.Length; i++)
			{
				this.canvas.Invalidate(sels[i]);
				sels[i].MemorizeStatus();
				sels[i].CenterPoint = Geometry.TransformPoint(m, x - rects[i].Width / 2F, rects[i].Top + rects[i].Height / 2F);
				this.canvas.Invalidate(sels[i]);
			}
			this.SetUndoBounds(sels);
		}

		public void ArrangeTop(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null || sels.Length < 1) return;
			RectangleF[] rects = new RectangleF[sels.Length];
			Matrix m = this.canvas.Transform;
			float y = 0F;
			for (int i = 0; i < sels.Length; i++)
			{
				rects[i] = Geometry.TransformRectangle(m, sels[i].Bounds);
				float yy = rects[i].Top;
				if (i == 0 || y > yy) y = yy;
			}
			m = this.canvas.InvertedTransform;
			for (int i = 0; i < sels.Length; i++)
			{
				this.canvas.Invalidate(sels[i]);
				sels[i].MemorizeStatus();
				sels[i].CenterPoint = Geometry.TransformPoint(m, rects[i].Left + rects[i].Width / 2F, y + rects[i].Height / 2F);
				this.canvas.Invalidate(sels[i]);
			}
			this.SetUndoBounds(sels);
		}

		public void ArrangeCenterV(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null || sels.Length < 1) return;
			RectangleF[] rects = new RectangleF[sels.Length];
			Matrix m = this.canvas.Transform;
			float y = 0F;
			for (int i = 0; i < sels.Length; i++)
			{
				rects[i] = Geometry.TransformRectangle(m, sels[i].Bounds);
				y += rects[i].Top + rects[i].Height / 2F;
			}
			y /=(float) sels.Length;
			m = this.canvas.InvertedTransform;
			for (int i = 0; i < sels.Length; i++)
			{
				this.canvas.Invalidate(sels[i]);
				sels[i].MemorizeStatus();
				sels[i].CenterPoint = Geometry.TransformPoint(m, rects[i].Left + rects[i].Width / 2F, y);
				this.canvas.Invalidate(sels[i]);
			}
			this.SetUndoBounds(sels);
		}

		public void ArrangeBottom(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null || sels.Length < 1) return;
			RectangleF[] rects = new RectangleF[sels.Length];
			Matrix m = this.canvas.Transform;
			float y = 0F;
			for (int i = 0; i < sels.Length; i++)
			{
				rects[i] = Geometry.TransformRectangle(m, sels[i].Bounds);
				float yy = rects[i].Bottom;
				if (i == 0 || y < yy) y = yy;
			}
			m = this.canvas.InvertedTransform;
			for (int i = 0; i < sels.Length; i++)
			{
				this.canvas.Invalidate(sels[i]);
				sels[i].MemorizeStatus();
				sels[i].CenterPoint = Geometry.TransformPoint(m, rects[i].Left + rects[i].Width / 2F, y - rects[i].Height / 2F);
				this.canvas.Invalidate(sels[i]);
			}
			this.SetUndoBounds(sels);
		}

		public void ArrangeH(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null || sels.Length < 3) return;
			ArrayList list1 = new ArrayList(), list2 = new ArrayList();
			Matrix m = this.canvas.Transform;
			foreach (CanvasObject sel in sels)
			{
				RectangleF r = Geometry.TransformRectangle(m, sel.Bounds);
				bool ok = false;
				for (int i = 0; i < list1.Count; i++)
				{
					if (r.Left <((RectangleF) list2[i]).Left)
					{
						list1.Insert(i, sel);
						list2.Insert(i, r);
						ok = true;
						break;
					}
				}
				if (!ok)
				{
					list1.Add(sel);
					list2.Add(r);
				}
			}
			m = this.canvas.InvertedTransform;
			int len = list1.Count;
			RectangleF rs =(RectangleF) list2[0], re =(RectangleF) list2[len - 1];
			float xs = rs.Left + rs.Width / 2F, xe = re.Left + re.Width / 2F;
			for (int i = 1; i < len - 1; i++)
			{
				CanvasObject co = list1[i] as CanvasObject;
				RectangleF r =(RectangleF) list2[i];
				this.canvas.Invalidate(co);
				co.MemorizeStatus();
				co.CenterPoint = Geometry.TransformPoint(m, xs +(xe - xs) *((float) i /(float) (len - 1F)), r.Top + r.Height / 2F);
				this.canvas.Invalidate(co);
			}
			list1.RemoveAt(len - 1);
			list1.RemoveAt(0);
			this.SetUndoBounds(list1.ToArray(typeof (CanvasObject)) as CanvasObject[]);
		}

		public void ArrangeV(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null || sels.Length < 3) return;
			ArrayList list1 = new ArrayList(), list2 = new ArrayList();
			Matrix m = this.canvas.Transform;
			foreach (CanvasObject sel in sels)
			{
				RectangleF r = Geometry.TransformRectangle(m, sel.Bounds);
				bool ok = false;
				for (int i = 0; i < list1.Count; i++)
				{
					if (r.Top <((RectangleF) list2[i]).Top)
					{
						list1.Insert(i, sel);
						list2.Insert(i, r);
						ok = true;
						break;
					}
				}
				if (!ok)
				{
					list1.Add(sel);
					list2.Add(r);
				}
			}
			m = this.canvas.InvertedTransform;
			int len = list1.Count;
			RectangleF rs =(RectangleF) list2[0], re =(RectangleF) list2[len - 1];
			float ys = rs.Top + rs.Height / 2F, ye = re.Top + re.Height / 2F;
			for (int i = 1; i < len - 1; i++)
			{
				CanvasObject co = list1[i] as CanvasObject;
				RectangleF r =(RectangleF) list2[i];
				this.canvas.Invalidate(co);
				co.MemorizeStatus();
				co.CenterPoint = Geometry.TransformPoint(m, r.Left + r.Width / 2F, ys +(ye - ys) *((float) i /(float) (len - 1F)));
				this.canvas.Invalidate(co);
			}
			list1.RemoveAt(len - 1);
			list1.RemoveAt(0);
			this.SetUndoBounds(list1.ToArray(typeof (CanvasObject)) as CanvasObject[]);
		}

		public void ArrangePlugL(object sender, EventArgs e)
		{
			Matrix m;

			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null || sels.Length < 2) return;
			ArrayList list1 = new ArrayList(), list2 = new ArrayList();
			float an = this.canvas.Angle;
			foreach (CanvasObject sel in sels)
			{
				m = this.canvas.Transform.Clone();
				sel.Rotate(m, an);
				RectangleF r = Geometry.TransformRectangle(m, sel.Bounds);
				m.Dispose();
				bool ok = false;
				for (int i = 0; i < list1.Count; i++)
				{
					if (r.Left <((RectangleF) list2[i]).Left)
					{
						list1.Insert(i, sel);
						list2.Insert(i, r);
						ok = true;
						break;
					}
				}
				if (!ok)
				{
					list1.Add(sel);
					list2.Add(r);
				}
			}
			m = this.canvas.InvertedTransform;
			int len = list1.Count;
			float x =((RectangleF) list2[0]).Left;
			for (int i = 0; i < len; i++)
			{
				CanvasObject co = list1[i] as CanvasObject;
				RectangleF r =(RectangleF) list2[i];
				this.canvas.Invalidate(co);
				co.MemorizeStatus();
				co.CenterPoint = Geometry.TransformPoint(m, x + r.Width / 2, r.Top + r.Height / 2F);
				this.canvas.Invalidate(co);
				x += r.Width;
			}
			this.SetUndoBounds(list1.ToArray(typeof (CanvasObject)) as CanvasObject[]);
		}

		public void ArrangePlugR(object sender, EventArgs e)
		{
			Matrix m;

			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null || sels.Length < 1) return;
			ArrayList list1 = new ArrayList(), list2 = new ArrayList();
			float an = this.canvas.Angle;
			foreach (CanvasObject sel in sels)
			{
				m = this.canvas.Transform.Clone();
				sel.Rotate(m, an);
				RectangleF r = Geometry.TransformRectangle(m, sel.Bounds);
				m.Dispose();
				bool ok = false;
				for (int i = 0; i < list1.Count; i++)
				{
					if (r.Left <((RectangleF) list2[i]).Left)
					{
						list1.Insert(i, sel);
						list2.Insert(i, r);
						ok = true;
						break;
					}
				}
				if (!ok)
				{
					list1.Add(sel);
					list2.Add(r);
				}
			}
			m = this.canvas.InvertedTransform;
			int len = list1.Count;
			float x =((RectangleF) list2[len - 1]).Right;
			for (int i = len - 1; i >= 0; i--)
			{
				CanvasObject co = list1[i] as CanvasObject;
				RectangleF r =(RectangleF) list2[i];
				this.canvas.Invalidate(co);
				co.MemorizeStatus();
				co.CenterPoint = Geometry.TransformPoint(m, x - r.Width / 2, r.Top + r.Height / 2F);
				this.canvas.Invalidate(co);
				x -= r.Width;
			}
			this.SetUndoBounds(list1.ToArray(typeof (CanvasObject)) as CanvasObject[]);
		}

		public void ArrangePlugT(object sender, EventArgs e)
		{
			Matrix m;

			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null || sels.Length < 1) return;
			ArrayList list1 = new ArrayList(), list2 = new ArrayList();
			float an = this.canvas.Angle;
			foreach (CanvasObject sel in sels)
			{
				m = this.canvas.Transform.Clone();
				sel.Rotate(m, an);
				RectangleF r = Geometry.TransformRectangle(m, sel.Bounds);
				m.Dispose();
				bool ok = false;
				for (int i = 0; i < list1.Count; i++)
				{
					if (r.Top <((RectangleF) list2[i]).Top)
					{
						list1.Insert(i, sel);
						list2.Insert(i, r);
						ok = true;
						break;
					}
				}
				if (!ok)
				{
					list1.Add(sel);
					list2.Add(r);
				}
			}
			m = this.canvas.InvertedTransform;
			int len = list1.Count;
			float y =((RectangleF) list2[0]).Top;
			for (int i = 0; i < len; i++)
			{
				CanvasObject co = list1[i] as CanvasObject;
				RectangleF r =(RectangleF) list2[i];
				this.canvas.Invalidate(co);
				co.MemorizeStatus();
				co.CenterPoint = Geometry.TransformPoint(m, r.Left + r.Width / 2F, y + r.Height / 2);
				this.canvas.Invalidate(co);
				y += r.Height;
			}
			this.SetUndoBounds(list1.ToArray(typeof (CanvasObject)) as CanvasObject[]);
		}

		public void ArrangePlugB(object sender, EventArgs e)
		{
			Matrix m;

			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null || sels.Length < 1) return;
			ArrayList list1 = new ArrayList(), list2 = new ArrayList();
			float an = this.canvas.Angle;
			foreach (CanvasObject sel in sels)
			{
				m = this.canvas.Transform.Clone();
				sel.Rotate(m, an);
				RectangleF r = Geometry.TransformRectangle(m, sel.Bounds);
				m.Dispose();
				bool ok = false;
				for (int i = 0; i < list1.Count; i++)
				{
					if (r.Top <((RectangleF) list2[i]).Top)
					{
						list1.Insert(i, sel);
						list2.Insert(i, r);
						ok = true;
						break;
					}
				}
				if (!ok)
				{
					list1.Add(sel);
					list2.Add(r);
				}
			}
			m = this.canvas.InvertedTransform;
			int len = list1.Count;
			float y =((RectangleF) list2[len - 1]).Bottom;
			for (int i = len - 1; i >= 0; i--)
			{
				CanvasObject co = list1[i] as CanvasObject;
				RectangleF r =(RectangleF) list2[i];
				this.canvas.Invalidate(co);
				co.MemorizeStatus();
				co.CenterPoint = Geometry.TransformPoint(m, r.Left + r.Width / 2F, y - r.Height / 2);
				this.canvas.Invalidate(co);
				y -= r.Height;
			}
			this.SetUndoBounds(list1.ToArray(typeof (CanvasObject)) as CanvasObject[]);
		}

		#endregion

		#region 編集

		public void Undo(object sender, EventArgs e)
		{
			if (!this.CanUndo) return;
			Hashtable guidTable = this.canvas.GetGuidTable();
			Operation[] ops = opsUndo.Pop() as Operation[];
			foreach (Operation op in ops) this.Operate(op, guidTable);
			this.opsRedo.Push(ops);
			this.OnChanged(EventArgs.Empty);
		}

		public void Redo(object sender, EventArgs e)
		{
			if (!this.CanRedo) return;
			Hashtable guidTable = this.canvas.GetGuidTable();
			Operation[] ops = opsRedo.Pop() as Operation[];
			foreach (Operation op in ops) this.Operate(op, guidTable);
			this.opsUndo.Push(ops);
			this.OnChanged(EventArgs.Empty);
		}

		public void Cut(object sender, EventArgs e)
		{
			this.Copy(this, EventArgs.Empty);
			this.Delete(this, EventArgs.Empty);
		}

		public void Copy(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null || sels.Length < 1) return;
			Clipboard.SetDataObject(new DataObject(this.DataFormat, this.SerializeItems(sels)), true);
			if (this.Menu == null || this.Menu.mnuCanvasPaste.Enabled) return;
			this.Menu.OnSetMenuEnabled(EventArgs.Empty);
		}

		public void Paste(object sender, EventArgs e)
		{
			string xml = Clipboard.GetDataObject().GetData(this.DataFormat) as string;
			if (xml == null || xml.Length < 1) return;
			this.CreateFromXml(xml, this.clickPoint);
		}

		public void Delete(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null) return;
			this.canvas.SortByOrder(sels);
			foreach (CanvasObject sel in sels)
			{
				this.canvas.Items.Remove(sel);
				this.canvas.Invalidate(sel);
			}
			this.canvas.SelectItem(null);
			this.SetUndoNew(sels);
		}

		public void Group(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			this.canvas.Group(sels);
			this.canvas.SelectedItems = sels;
			this.Invalidate();
			this.Change();
		}

		public void Ungroup(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			this.canvas.Ungroup(sels);
			this.canvas.SelectedItems = sels;
			this.Invalidate();
			this.Change();
		}

		public void FrontMost(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null) return;
			foreach (CanvasObject sel in sels)
			{
				this.canvas.Items.Remove(sel);
				this.canvas.Items.Insert(0, sel);
				this.canvas.Invalidate(sel);
			}
			this.Change();
		}

		public void BackMost(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null) return;
			foreach (CanvasObject sel in sels)
			{
				this.canvas.Items.Remove(sel);
				this.canvas.Items.Add(sel);
				this.canvas.Invalidate(sel);
			}
			this.Change();
		}

		public void AlwaysFrontMost(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null) return;
			foreach (CanvasObject sel in sels)
			{
				sel.FrontMost = !sel.FrontMost;
				this.canvas.Invalidate(sel);
			}
			this.Change();
		}

		public void AlwaysBackMost(object sender, EventArgs e)
		{
			CanvasObject[] sels = this.canvas.SelectedItems;
			if (sels == null) return;
			foreach (CanvasObject sel in sels)
			{
				sel.BackMost = !sel.BackMost;
				this.canvas.Invalidate(sel);
			}
			this.Change();
		}

		public void AddVertex(object sender, EventArgs e)
		{
			if (this.corner == null) return;
			CanvasPolygon poly = this.corner.Target as CanvasPolygon;
			if (poly == null) return;
			poly.MemorizeStatus();
			this.canvas.Invalidate(poly);
			poly.AddVertex(this.corner.Index);
			poly.InitSelection(this.corner.Selection);
			this.canvas.Invalidate(poly);
			this.SetUndoBounds(new CanvasObject[] { poly });
		}

		public void RemoveVertex(object sender, EventArgs e)
		{
			if (this.corner == null) return;
			CanvasPolygon poly = this.corner.Target as CanvasPolygon;
			if (poly == null || poly.Points.Length <= 2) return;
			poly.MemorizeStatus();
			this.canvas.Invalidate(poly);
			poly.RemoveVertex(this.corner.Index, this.canvas.Angle);
			poly.InitSelection(this.corner.Selection);
			this.canvas.Invalidate(poly);
			this.SetUndoBounds(new CanvasObject[] { poly });
		}

		public void MoveTo(object sender, EventArgs e)
		{
			this.MoveItems(this.cmnPos.X, this.cmnPos.Y);
			this.SetUndoBounds(this.canvas.SelectedItems);
		}

		public void CopyTo(object sender, EventArgs e)
		{
			this.CopyItems();
			this.MoveItems(this.cmnPos.X, this.cmnPos.Y);
			this.SetUndoDelete(this.canvas.SelectedItems);
		}

		#endregion

		#region 印刷

		public void SelectPrint(object sender, EventArgs e)
		{
			this.Cancel();
			this.eventStatus = "PrepareSelectPrint";
			this.Cursor = Cursors.Cross;
			this.SetSelBoxColor();
			this.vline.Pen.Color = ImageManipulator.Swap(this.selBox.Pen.Color, PrimaryColors.Red, PrimaryColors.Blue, PrimaryColors.Green);
			this.SetLines();
		}

		public void ResetPrint(object sender, EventArgs e)
		{
			if (!this.printBox.Visible) return;
			this.printBox.Visible = false;
			this.Invalidate();
		}

		public void DisplayPrint(object sender, EventArgs e)
		{
			if (!this.printBox.Visible) return;
			this.SetDisplayArea(this.printBox.Bounds, this.printBox.Angle);
		}

		#endregion

		#region 新規作成

		public void NewLine(object sender, EventArgs e)
		{
			this.canvas.SelectItem(null);
			PointF pt = this.PointToCanvas(this.clickPoint);
			PointF pt1 = new PointF(pt.X - 16, pt.Y - 16);
			PointF pt2 = new PointF(pt.X + 16, pt.Y + 16);
			this.BeginAddItem(new CanvasLines(pt1, pt2));
		}

		public void NewArrow(object sender, EventArgs e)
		{
			this.canvas.SelectItem(null);
			PointF pt = this.PointToCanvas(this.clickPoint);
			PointF pt1 = new PointF(pt.X - 16, pt.Y - 16);
			PointF pt2 = new PointF(pt.X + 16, pt.Y + 16);
			CanvasLines cl = new CanvasLines(pt1, pt2);
			cl.Pen = new Pen(Color.Black);
			cl.Pen.CustomEndCap = new AdjustableArrowCap(5, 5, false);
			this.BeginAddItem(cl);
		}

		public void NewRectangle(object sender, EventArgs e)
		{
			this.canvas.SelectItem(null);
			PointF pt = this.PointToCanvas(this.clickPoint);
			this.BeginAddItem(new CanvasRectangle(pt.X - 16F, pt.Y - 16F, 32F, 32F));
		}

		public void NewEllipse(object sender, EventArgs e)
		{
			this.canvas.SelectItem(null);
			PointF pt = this.PointToCanvas(this.clickPoint);
			this.BeginAddItem(new CanvasEllipse(pt.X - 16F, pt.Y - 16F, 32F, 32F));
		}

		public void NewString(object sender, EventArgs e)
		{
			this.canvas.SelectItem(null);
			PointF pt = this.PointToCanvas(this.clickPoint);
			this.BeginAddItem(new CanvasString(pt.X, pt.Y, "文字列"));
		}

		#endregion

		#endregion

		#region XML

		public void CreateFromXml(string xml, Point pt)
		{
			CanvasObject[] cos = this.DeserializeItems(xml);
			if (cos == null) return;
			PointF pt2 = this.PointToCanvas(pt);
			PointF pt3 = Canvas.GetItemsCenter(cos);
			float dx = pt2.X - pt3.X, dy = pt2.Y - pt3.Y;
			Hashtable guidTable = this.canvas.GetGuidTable();
			foreach (CanvasObject co in cos)
			{
				co.CheckGuid(guidTable);
				co.Offset(dx, dy);
			}
			this.canvas.Items.InsertRange(0, cos);
			this.canvas.SelectedItems = cos;
			this.SetUndoDelete(cos);
		}

		public string SerializeItem(CanvasObject co)
		{
			StringWriter sw = new StringWriter();
			XmlTextWriter xw = new XmlTextWriter(sw);
			this.Serializer.Write(xw, null, co);
			xw.Close();
			sw.Close();
			return sw.ToString();
		}

		public CanvasObject DeserializeItem(string xml)
		{
			ArrayList list = new ArrayList();
			StringReader sr = new StringReader(xml);
			XmlTextReader xr = new XmlTextReader(sr);
			CanvasObject ret = null;
			if (xr.Read()) ret = this.Serializer.Read(xr) as CanvasObject;
			xr.Close();
			sr.Close();
			return ret;
		}

		public string SerializeItems(CanvasObject[] cos)
		{
			return Canvas.SerializeItems(cos, this.XmlElement, this.Serializer, this.canvas.Transform);
		}

		public CanvasObject[] DeserializeItems(string xml)
		{
			Matrix m = null;
			ArrayList list = new ArrayList();
			StringReader sr = new StringReader(xml);
			XmlTextReader xr = new XmlTextReader(sr);
			while (xr.Read())
			{
				if (xr.Name == this.XmlElement && xr.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				else if (xr.Name == "Transform" && xr.NodeType == XmlNodeType.Element && !xr.IsEmptyElement)
				{
					if (m != null) m.Dispose();
					m = this.Serializer.ReadMatrix(xr);
				}
				else if (xr.Name == "Items" && xr.NodeType == XmlNodeType.Element && !xr.IsEmptyElement)
				{
					while (xr.Read())
					{
						if (xr.Name == "Items" && xr.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						else if (xr.NodeType == XmlNodeType.Element)
						{
							object obj = this.Serializer.Read(xr);
							if (obj != null && obj is CanvasObject) list.Add(obj);
						}
					}
				}
			}
			xr.Close();
			sr.Close();
			float an = 0F;
			if (m != null)
			{
				an = Geometry.GetAngle(m);
				m.Dispose();
			}
			if (list.Count < 1) return null;
			CanvasObject[] ret = list.ToArray(typeof (CanvasObject)) as CanvasObject[];
			Canvas.RotateItems(ret, this.canvas.Angle - an);
			return ret;
		}

		#endregion

		#region 操作

		public void Change()
		{
			if (this.CanUndo) this.opsUndo.Clear();
			if (this.CanRedo) this.opsRedo.Clear();
			this.OnChanged(EventArgs.Empty);
		}

		protected virtual void OnChanged(EventArgs e)
		{
			if (this.Changed != null) this.Changed(this, e);
		}

		protected void SetUndo(Operation[] ops)
		{
			this.opsUndo.Push(ops);
			if (this.CanRedo) this.opsRedo.Clear();
			this.OnChanged(EventArgs.Empty);
		}

		public void SetUndoBounds(CanvasObject[] cos)
		{
			if (cos == null || cos.Length < 1) return;
			Operation[] ops = new Operation[cos.Length + 1];
			ops[0] = new Operation("Deselect");
			int i = 1;
			foreach (CanvasObject co in cos)
			{
				ops[i++] = new Operation("Bounds", co.Guid, co.TagBounds);
			}
			this.SetUndo(ops);
		}

		public void SetUndoAngle(CanvasObject[] cos)
		{
			if (cos == null || cos.Length < 1) return;
			Operation[] ops = new Operation[cos.Length + 1];
			ops[0] = new Operation("Deselect");
			int i = 1;
			foreach (CanvasObject co in cos)
			{
				ops[i++] = new Operation("Angle", co.Guid, co.MemorizedAngle);
			}
			this.SetUndo(ops);
		}

		public void SetUndoNew(CanvasObject[] cos)
		{
			if (cos == null || cos.Length < 1) return;
			Operation[] ops = new Operation[cos.Length + 1];
			ops[0] = new Operation("Deselect");
			int i = 1;
			foreach (CanvasObject co in cos)
			{
				ops[i++] = new Operation("New", co.Guid, this.SerializeItem(co));
			}
			this.SetUndo(ops);
		}

		public void SetUndoDelete(CanvasObject[] cos)
		{
			if (cos == null || cos.Length < 1) return;
			Operation[] ops = new Operation[cos.Length + 1];
			ops[0] = new Operation("Deselect");
			int i = 1;
			foreach (CanvasObject co in cos)
			{
				ops[i++] = new Operation("Delete", co.Guid);
			}
			this.SetUndo(ops);
		}

		public virtual void Operate(Operation op, Hashtable guidTable)
		{
			this.Cancel();
			switch (op.Name)
			{
				case "Bounds":
				this.OperateBounds(op, guidTable);
				break;
				case "Angle":
				this.OperateAngle(op, guidTable);
				break;
				case "Deselect":
				this.canvas.SelectItem(null);
				break;
				case "New":
				this.OperateNew(op, guidTable);
				break;
				case "Delete":
				this.OperateDelete(op, guidTable);
				break;
			}
		}

		public void OperateBounds(Operation op, Hashtable guidTable)
		{
			if (guidTable == null || !guidTable.Contains(op.Guid)) return;
			CanvasObject co = guidTable[op.Guid] as CanvasObject;
			this.canvas.Invalidate(co);
			co.MemorizeStatus();
			co.TagBounds = op.Tag;
			op.Tag = co.TagBounds;
			this.canvas.ToggledSelectItem(co);
		}

		public void OperateAngle(Operation op, Hashtable guidTable)
		{
			if (guidTable == null || !guidTable.Contains(op.Guid)) return;
			CanvasObject co = guidTable[op.Guid] as CanvasObject;
			this.canvas.Invalidate(co);
			co.MemorizeStatus();
			co.Angle =(float) op.Tag;
			op.Tag = co.MemorizedAngle;
			this.canvas.ToggledSelectItem(co);
		}

		public void OperateNew(Operation op, Hashtable guidTable)
		{
			CanvasObject co = this.DeserializeItem(op.Tag as string);
			co.CheckGuid(guidTable);
			this.canvas.Items.Insert(Math.Max(co.Order, 0), co);
			this.canvas.ToggledSelectItem(co);
			op.Name = "Delete";
			op.Tag = null;
		}

		public void OperateDelete(Operation op, Hashtable guidTable)
		{
			if (guidTable == null || !guidTable.Contains(op.Guid)) return;
			CanvasObject co = guidTable[op.Guid] as CanvasObject;
			op.Name = "New";
			op.Tag = this.SerializeItem(co);
			this.canvas.Items.Remove(co);
			this.canvas.Invalidate(co);
			if (this.canvas.SelectedItems != null) this.canvas.SelectedItems = null;
		}

		#endregion

		public virtual void Draw(PaintEventArgs e)
		{
			RectangleF r = e.ClipRectangle;
			Matrix old = this.Transform, m;
			if (this.printBox.Visible)
			{
				float zx = Math.Abs(r.Width / this.printBox.Width);
				float zy = Math.Abs(r.Height / this.printBox.Height);
				float zoom =(zx < zy) ? zx:
				zy;
				PointF cpt1 = this.printBox.CenterPoint;
				m = new Matrix();
				m.Translate(-cpt1.X, -cpt1.Y);
				Geometry.ScaleAt(m, zoom, zoom, cpt1);
				Matrix mm = m.Clone();
				mm.Invert();
				PointF cpt2 = Geometry.TransformPoint(mm, Geometry.GetCenter(r));
				mm.Dispose();
				m.Translate(cpt2.X - cpt1.X, cpt2.Y - cpt1.Y);
				r = Geometry.TransformRectangle(m, this.printBox.Bounds);
				m.RotateAt(-this.printBox.Angle, cpt1);
			}
			else
			{
				PointF cpt1 = this.CenterPointInCanvas;
				PointF cpt2 = this.PointToCanvas(Geometry.GetCenter(r));
				m = old.Clone();
				m.Translate(cpt2.X - cpt1.X, cpt2.Y - cpt1.Y);
			}
			this.Transform = m;
			e.Graphics.SetClip(r);
			CanvasObject[] sels = this.canvas.SelectedItems;
			CanvasObject sel = this.canvas.SelectedItem;
			this.canvas.SelectedItems = null;
			this.DrawCanvas(e);
			this.canvas.SetSelection(sels, sel);
			e.Graphics.ResetClip();
			this.Transform = old;
			m.Dispose();
			this.DrawOthers(e);
		}

		protected virtual void DrawCanvas(PaintEventArgs e)
		{
			this.canvas.Draw(e);
		}

		protected virtual void DrawOthers(PaintEventArgs e)
		{
		}

		public void BeginAddItem(CanvasObject co)
		{
			this.Cancel();
			this.eventStatus = "PrepareAddItem";
			this.Cursor = Cursors.Cross;
			this.SetSelBoxColor();
			this.vline.Pen.Color = this.selBox.Pen.Color;
			this.SetLines();
			this.addItem = co;
		}
	}
}
