using System;
using System.Drawing;
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
			this.opaque = true;
		}

		public bool Opaque
		{
			get
			{
				return this.opaque;
			}

			set
			{
				this.opaque = true;
			}
		}

		protected override void OnSplitterMoving(SplitterEventArgs e)
		{
			if (this.opaque)
			{
				int a;
				int pos = this.SplitPosition;
				switch (Dock)
				{
					case DockStyle.Left: a = this.Left - pos;
					pos = e.SplitX - a;
					break;
					case DockStyle.Right: a = this.Left + pos;
					pos = a - e.SplitX;
					break;
					case DockStyle.Top: a = this.Top - pos;
					pos = e.SplitY - a;
					break;
					case DockStyle.Bottom: a = this.Top + pos;
					pos = a - e.SplitY;
					break;
				}
				if (this.SplitPosition != pos) this.SplitPosition = pos;
			}
			base.OnSplitterMoving(e);
		}
	}
}
