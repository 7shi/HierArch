// ここにソースコードの注釈を書きます。

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Girl.Windows.Forms.Drawing2D
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class CanvasObjectSelection
	{
		protected CanvasObject target;
		public CanvasObject[] Borders;
		public CanvasCorner[] Corners;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public CanvasObjectSelection(CanvasObject target)
		{
			this.target = target;
			target.InitSelection(this);
		}

		public void Paint(PaintEventArgs e)
		{
			foreach (CanvasObject co in this.Borders)
			{
				if (co.Visible && co.ClientRectangle.IntersectsWith(e.ClipRectangle))
				{
					co.Draw(e.Graphics);
				}
			}
			CanvasCorner[] ccs = this.Corners.Clone() as CanvasCorner[];
			Array.Reverse(ccs);
			foreach (CanvasCorner cc in ccs)
			{
				if (cc.Visible && cc.ClientRectangle.IntersectsWith(e.ClipRectangle))
				{
					cc.Draw(e.Graphics);
				}
			}
		}

		public CanvasObject Target
		{
			get
			{
				return this.target;
			}
		}

		public void Add(CanvasCorner corner, int index)
		{
			corner.Selection = this;
			int len = this.Corners.Length;
			CanvasCorner[] ccs = new CanvasCorner[len + 1];
			if (index + 1 > 0) Array.Copy(this.Corners, ccs, index + 1);
			ccs[index + 1] = corner;
			int len2 = len - index - 1;
			if (len2 > 0) Array.Copy(this.Corners, index + 1, ccs, index + 2, len2);
			this.Corners = ccs;
			for (int i = 0; i < this.Corners.Length; i++)
			{
				this.Corners[i].Index = i;
			}
		}

		public void Add(CanvasCorner corner)
		{
			this.Add(corner, this.Corners.Length - 1);
		}

		public void Remove(int index)
		{
			int len = this.Corners.Length;
			CanvasCorner[] ccs = new CanvasCorner[len - 1];
			if (index > 0) Array.Copy(this.Corners, ccs, index);
			int len2 = len - index - 1;
			if (len2 > 0) Array.Copy(this.Corners, index + 1, ccs, index, len2);
			this.Corners = ccs;
			for (int i = 0; i < this.Corners.Length; i++)
			{
				this.Corners[i].Index = i;
			}
		}
	}
}
