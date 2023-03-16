// ここにソースコードの注釈を書きます。

using System;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Girl.Windows.Forms.Drawing2D
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class CanvasPaintManager
	{
		protected Control target;
		public ArrayList CanvasCollection;
		public SmoothingMode SmoothingMode;

		public CanvasPaintManager(Control target)
		{
			this.target = target;
			this.target.Paint += new PaintEventHandler(this.target_Paint);
			this.CanvasCollection = new ArrayList();
			this.SmoothingMode = SmoothingMode.Default;
		}

		private void target_Paint(object sender, PaintEventArgs e)
		{
			SmoothingMode old = e.Graphics.SmoothingMode;
			e.Graphics.SmoothingMode = this.SmoothingMode;
			foreach (object obj in this.CanvasCollection)
			{
				(obj as Canvas).Draw(e);
			}
			e.Graphics.SmoothingMode = old;
		}

		public Control Target
		{
			get
			{
				return this.target;
			}
		}
	}
}
