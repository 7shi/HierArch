using System;
using System.Windows.Forms;

namespace Girl.Windows.Forms.Drawing2D
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class DrawingBoxContextMenu
	{
		protected DrawingBox target;
		public ContextMenu cmnCanvas;
		public MenuItem mnuCanvasNew;
		public MenuItem mnuCanvasNewLine;
		public MenuItem mnuCanvasNewArrow;
		public MenuItem mnuCanvasNewRectangle;
		public MenuItem mnuCanvasNewEllipse;
		public MenuItem mnuCanvasNewString;
		public MenuItem mnuCanvasPaste;
		public MenuItem mnuCanvasRotateFree;
		public MenuItem mnuCanvasRotateLeft90;
		public MenuItem mnuCanvasRotateRight90;
		public MenuItem mnuCanvasRotate180;
		public ContextMenu cmnItem;
		public MenuItem mnuItemCut;
		public MenuItem mnuItemCopy;
		public MenuItem mnuItemGroup;
		public MenuItem mnuItemUngroup;
		public MenuItem mnuItemOrder;
		public MenuItem mnuItemFrontMost;
		public MenuItem mnuItemBackMost;
		public MenuItem mnuItemAlwaysFrontMost;
		public MenuItem mnuItemAlwaysBackMost;
		public MenuItem mnuItemArrange;
		public MenuItem mnuItemArrangeLeft;
		public MenuItem mnuItemArrangeCenterH;
		public MenuItem mnuItemArrangeRight;
		public MenuItem mnuItemArrangeTop;
		public MenuItem mnuItemArrangeCenterV;
		public MenuItem mnuItemArrangeBottom;
		public MenuItem mnuItemArrangeH;
		public MenuItem mnuItemArrangeV;
		public MenuItem mnuItemRotate;
		public MenuItem mnuItemRotateFree;
		public MenuItem mnuItemRotateLeft90;
		public MenuItem mnuItemRotateRight90;
		public MenuItem mnuItemRotate180;
		public MenuItem mnuItemRotateStandard;
		public MenuItem mnuItemRotateToCanvas;
		public MenuItem mnuItemRotateCanvas;
		public MenuItem mnuItemMirrorH;
		public MenuItem mnuItemMirrorV;
		public MenuItem mnuItemFixAngle;
		public MenuItem mnuItemDelete;
		public event EventHandler SetMenuEnabled;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public DrawingBoxContextMenu()
		{
			this.target = null;
			cmnCanvas = new ContextMenu(new MenuItem[] { mnuCanvasNew = new MenuItem("新規作成(&W)"), new MenuItem("-"), mnuCanvasPaste = new MenuItem("貼り付け(&P)", new EventHandler(Paste)), new MenuItem("-"), mnuCanvasRotateFree = new MenuItem("自由に回転(&T)", new EventHandler(RotateFree)), mnuCanvasRotateLeft90 = new MenuItem("左 90 度回転(&L)", new EventHandler(RotateLeft90)), mnuCanvasRotateRight90 = new MenuItem("右 90 度回転(&R)", new EventHandler(RotateRight90)), mnuCanvasRotate180 = new MenuItem("180 度回転(&2)", new EventHandler(Rotate180)), new MenuItem("-"), new MenuItem("座標に向ける(&N)", new EventHandler(RotateAxis)), new MenuItem("基準に向ける(&E)", new EventHandler(RotateStandard)), new MenuItem("原点に戻る(&O)", new EventHandler(MoveOrigin)), new MenuItem("-"), new MenuItem("現在の方向を基準にする(&S)", new EventHandler(SetStandard)) });
			cmnCanvas.Popup += new EventHandler(cmnCanvas_Popup);
			mnuCanvasNew.MenuItems.AddRange(new MenuItem[] { mnuCanvasNewLine = new MenuItem("直線(&L)", new EventHandler(NewLine)), mnuCanvasNewArrow = new MenuItem("矢印(&A)", new EventHandler(NewArrow)), mnuCanvasNewRectangle = new MenuItem("長方形(&R)", new EventHandler(NewRectangle)), mnuCanvasNewEllipse = new MenuItem("楕円(&E)", new EventHandler(NewEllipse)), mnuCanvasNewString = new MenuItem("文字列(&S)", new EventHandler(NewString)) });
			cmnItem = new ContextMenu(new MenuItem[] { mnuItemCut = new MenuItem("切り取り(&T)", new EventHandler(Cut)), mnuItemCopy = new MenuItem("コピー(&C)", new EventHandler(Copy)), new MenuItem("-"), new MenuItem("グループ化(&G)", new MenuItem[] { mnuItemGroup = new MenuItem("グループ化(&G)", new EventHandler(Group)), mnuItemUngroup = new MenuItem("グループ解除(&U)", new EventHandler(Ungroup)) }), mnuItemOrder = new MenuItem("順序(&O)"), new MenuItem("-"), mnuItemArrange = new MenuItem("配置/整列(&A)"), mnuItemRotate = new MenuItem("回転/反転(&P)"), new MenuItem("-"), mnuItemDelete = new MenuItem("削除(&D)", new EventHandler(Delete)) });
			mnuItemOrder.MenuItems.AddRange(new MenuItem[] { mnuItemFrontMost = new MenuItem("最前面へ移動(&T)", new EventHandler(FrontMost)), mnuItemBackMost = new MenuItem("最背面へ移動(&K)", new EventHandler(BackMost)), new MenuItem("-"), mnuItemAlwaysFrontMost = new MenuItem("常に前面(&N)", new EventHandler(AlwaysFrontMost)), mnuItemAlwaysBackMost = new MenuItem("常に背面(&C)", new EventHandler(AlwaysBackMost)) });
			mnuItemArrange.MenuItems.AddRange(new MenuItem[] { mnuItemArrangeLeft = new MenuItem("左揃え(&L)", new EventHandler(ArrangeLeft)), mnuItemArrangeCenterH = new MenuItem("左右中央揃え(&C)", new EventHandler(ArrangeCenterH)), mnuItemArrangeRight = new MenuItem("右揃え(&R)", new EventHandler(ArrangeRight)), new MenuItem("-"), mnuItemArrangeTop = new MenuItem("上揃え(&T)", new EventHandler(ArrangeTop)), mnuItemArrangeCenterV = new MenuItem("上下中央揃え(&M)", new EventHandler(ArrangeCenterV)), mnuItemArrangeBottom = new MenuItem("下揃え(&B)", new EventHandler(ArrangeBottom)), new MenuItem("-"), mnuItemArrangeH = new MenuItem("左右に整列(&H)", new EventHandler(ArrangeH)), mnuItemArrangeV = new MenuItem("上下に整列(&V)", new EventHandler(ArrangeV)), new MenuItem("-"), new MenuItem("左詰め(&E)", new EventHandler(ArrangePlugL)), new MenuItem("右詰め(&I)", new EventHandler(ArrangePlugR)), new MenuItem("上詰め(&P)", new EventHandler(ArrangePlugT)), new MenuItem("下詰め(&O)", new EventHandler(ArrangePlugB)), });
			mnuItemRotate.MenuItems.AddRange(new MenuItem[] { mnuItemRotateFree = new MenuItem("自由に回転(&T)", new EventHandler(RotateFree)), mnuItemRotateLeft90 = new MenuItem("左 90 度回転(&L)", new EventHandler(RotateLeft90)), mnuItemRotateRight90 = new MenuItem("右 90 度回転(&R)", new EventHandler(RotateRight90)), mnuItemRotate180 = new MenuItem("180 度回転(&2)", new EventHandler(Rotate180)), new MenuItem("-"), new MenuItem("座標に向ける(&G)", new EventHandler(RotateAxis)), mnuItemRotateStandard = new MenuItem("基準に向ける(&E)", new EventHandler(RotateStandard)), mnuItemRotateToCanvas = new MenuItem("図面に向ける(&C)", new EventHandler(RotateToCanvas)), new MenuItem("-"), mnuItemRotateCanvas = new MenuItem("図面を向ける(&O)", new EventHandler(RotateCanvas)), new MenuItem("-"), this.mnuItemMirrorH = new MenuItem("左右反転(&H)", new EventHandler(MirrorH)), this.mnuItemMirrorV = new MenuItem("上下反転(&V)", new EventHandler(MirrorV)), new MenuItem("-"), mnuItemFixAngle = new MenuItem("角度を固定する(&F)", new EventHandler(FixAngle)) });
		}

		public DrawingBox Target
		{
			get
			{
				return this.target;
			}

			set
			{
				if (this.target != value)
				{
					if (this.target != null) this.target.Cancel();
					this.target = value;
				}
				this.OnSetMenuEnabled(EventArgs.Empty);
			}
		}

		#region Context Menu

		public virtual void OnSetMenuEnabled(EventArgs e)
		{
			bool enabled = this.target != null;
			DrawingBoxContextMenu.SetEnabled(cmnCanvas, enabled);
			DrawingBoxContextMenu.SetEnabled(cmnItem, enabled);
			if (!enabled)
			{
				if (this.SetMenuEnabled != null) this.SetMenuEnabled(this, e);
				return;
			}
			CanvasObject[] sels = target.Canvas.SelectedItems;
			this.mnuItemMirrorH.Enabled = this.mnuItemMirrorV.Enabled = sels != null;
			this.mnuItemFrontMost.Enabled = this.mnuItemBackMost.Enabled = sels != null && target.Canvas.Items.Count > 1;
			bool arr = sels != null && sels.Length > 1;
			foreach (MenuItem mni in this.mnuItemArrange.MenuItems)
			{
				if (mni == this.mnuItemArrangeH || mni == this.mnuItemArrangeV)
				{
					mni.Enabled = sels != null && sels.Length > 2;
				}
				else
				{
					mni.Enabled = arr;
				}
			}
			this.mnuItemCut.Enabled = this.mnuItemCopy.Enabled = target.CanCopy;
			this.mnuCanvasPaste.Enabled = target.CanPaste;
			this.mnuItemGroup.Enabled = CanvasGroup.CanGroup(sels);
			this.mnuItemUngroup.Enabled = CanvasGroup.CanUngroup(sels);
			CanvasObject sel = target.Canvas.SelectedItem;
			this.mnuItemRotateStandard.Enabled = this.mnuItemRotateToCanvas.Enabled = this.mnuItemRotateCanvas.Enabled =(sel != null && !sel.FixAngle);
			this.mnuItemFixAngle.Checked = sel != null && sel.FixAngle;
			this.mnuItemAlwaysFrontMost.Checked = sel != null && sel.FrontMost;
			this.mnuItemAlwaysBackMost.Checked = sel != null && sel.BackMost;
			if (this.SetMenuEnabled != null) this.SetMenuEnabled(this, e);
		}

		protected void cmnCanvas_Popup(object sender, EventArgs e)
		{
			bool e1 = this.mnuCanvasPaste.Enabled;
			bool e2 = target != null && target.CanPaste;
			if (e1 != e2) this.OnSetMenuEnabled(e);
		}

		public void ResetClickPos(object sender, EventArgs e)
		{
			if (target != null) target.ResetClickPos(sender, e);
		}

		#region 回転・反転

		public void RotateFree(object sender, EventArgs e)
		{
			if (target != null) target.RotateFree(sender, e);
		}

		public void RotateLeft90(object sender, EventArgs e)
		{
			if (target != null) target.RotateLeft90(sender, e);
		}

		public void RotateRight90(object sender, EventArgs e)
		{
			if (target != null) target.RotateRight90(sender, e);
		}

		public void Rotate180(object sender, EventArgs e)
		{
			if (target != null) target.Rotate180(sender, e);
		}

		public void RotateAxis(object sender, EventArgs e)
		{
			if (target != null) target.RotateAxis(sender, e);
		}

		public void RotateCanvas(object sender, EventArgs e)
		{
			if (target != null) target.RotateCanvas(sender, e);
		}

		public void RotateToCanvas(object sender, EventArgs e)
		{
			if (target != null) target.RotateToCanvas(sender, e);
		}

		public void RotateStandard(object sender, EventArgs e)
		{
			if (target != null) target.RotateStandard(sender, e);
		}

		public void SetStandard(object sender, EventArgs e)
		{
			if (target != null) target.SetStandard(sender, e);
		}

		public void FixAngle(object sender, EventArgs e)
		{
			if (target != null) target.FixAngle(sender, e);
		}

		public void MirrorH(object sender, EventArgs e)
		{
			if (target != null) target.MirrorH(sender, e);
		}

		public void MirrorV(object sender, EventArgs e)
		{
			if (target != null) target.MirrorV(sender, e);
		}

		public void MoveOrigin(object sender, EventArgs e)
		{
			if (target != null) target.MoveOrigin(sender, e);
		}

		#endregion

		#region 配置

		public void ArrangeLeft(object sender, EventArgs e)
		{
			if (target != null) target.ArrangeLeft(sender, e);
		}

		public void ArrangeCenterH(object sender, EventArgs e)
		{
			if (target != null) target.ArrangeCenterH(sender, e);
		}

		public void ArrangeRight(object sender, EventArgs e)
		{
			if (target != null) target.ArrangeRight(sender, e);
		}

		public void ArrangeTop(object sender, EventArgs e)
		{
			if (target != null) target.ArrangeTop(sender, e);
		}

		public void ArrangeCenterV(object sender, EventArgs e)
		{
			if (target != null) target.ArrangeCenterV(sender, e);
		}

		public void ArrangeBottom(object sender, EventArgs e)
		{
			if (target != null) target.ArrangeBottom(sender, e);
		}

		public void ArrangeH(object sender, EventArgs e)
		{
			if (target != null) target.ArrangeH(sender, e);
		}

		public void ArrangeV(object sender, EventArgs e)
		{
			if (target != null) target.ArrangeV(sender, e);
		}

		public void ArrangePlugL(object sender, EventArgs e)
		{
			if (target != null) target.ArrangePlugL(sender, e);
		}

		public void ArrangePlugR(object sender, EventArgs e)
		{
			if (target != null) target.ArrangePlugR(sender, e);
		}

		public void ArrangePlugT(object sender, EventArgs e)
		{
			if (target != null) target.ArrangePlugT(sender, e);
		}

		public void ArrangePlugB(object sender, EventArgs e)
		{
			if (target != null) target.ArrangePlugB(sender, e);
		}

		#endregion

		#region 編集

		public void Undo(object sender, EventArgs e)
		{
			if (target != null) target.Undo(sender, e);
		}

		public void Redo(object sender, EventArgs e)
		{
			if (target != null) target.Redo(sender, e);
		}

		public void Cut(object sender, EventArgs e)
		{
			if (target != null) target.Cut(sender, e);
		}

		public void Copy(object sender, EventArgs e)
		{
			if (target != null) target.Copy(sender, e);
		}

		public void Paste(object sender, EventArgs e)
		{
			if (target != null) target.Paste(sender, e);
		}

		public void Delete(object sender, EventArgs e)
		{
			if (target != null) target.Delete(sender, e);
		}

		public void Group(object sender, EventArgs e)
		{
			if (target != null) target.Group(sender, e);
		}

		public void Ungroup(object sender, EventArgs e)
		{
			if (target != null) target.Ungroup(sender, e);
		}

		public void FrontMost(object sender, EventArgs e)
		{
			if (target != null) target.FrontMost(sender, e);
		}

		public void BackMost(object sender, EventArgs e)
		{
			if (target != null) target.BackMost(sender, e);
		}

		public void AlwaysFrontMost(object sender, EventArgs e)
		{
			if (target != null) target.AlwaysFrontMost(sender, e);
		}

		public void AlwaysBackMost(object sender, EventArgs e)
		{
			if (target != null) target.AlwaysBackMost(sender, e);
		}

		#endregion

		#region 印刷

		public void SelectPrint(object sender, EventArgs e)
		{
			if (target != null) target.SelectPrint(sender, e);
		}

		public void ResetPrint(object sender, EventArgs e)
		{
			if (target != null) target.ResetPrint(sender, e);
		}

		public void DisplayPrint(object sender, EventArgs e)
		{
			if (target != null) target.DisplayPrint(sender, e);
		}

		#endregion

		#region 新規作成

		public void NewLine(object sender, EventArgs e)
		{
			if (target != null) target.NewLine(sender, e);
		}

		public void NewArrow(object sender, EventArgs e)
		{
			if (target != null) target.NewArrow(sender, e);
		}

		public void NewRectangle(object sender, EventArgs e)
		{
			if (target != null) target.NewRectangle(sender, e);
		}

		public void NewEllipse(object sender, EventArgs e)
		{
			if (target != null) target.NewEllipse(sender, e);
		}

		public void NewString(object sender, EventArgs e)
		{
			if (target != null) target.NewString(sender, e);
		}

		#endregion

		#endregion

		public static void SetEnabled(Menu menu, bool enabled)
		{
			MenuItem item = menu as MenuItem;
			if (item != null) item.Enabled = enabled;
			foreach (MenuItem mni in menu.MenuItems)
			{
				DrawingBoxContextMenu.SetEnabled(mni, enabled);
			}
		}
	}
}
