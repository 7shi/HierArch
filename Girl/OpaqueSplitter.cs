using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// サイズ変更中にすぐ反映される Splitter です。
	/// </summary>
	public class OpaqueSplitter : System.Windows.Forms.Splitter
	{
		private bool m_bOpaque = true;

		public OpaqueSplitter()
		{
		}

		protected override void OnSplitterMoving(SplitterEventArgs se)
		{
			if (Opaque)
			{
				int a;
				int pos = SplitPosition;
				switch (Dock)
				{
					case DockStyle.Left:
						a = Left - pos;
						pos = se.SplitX - a;
						break;
					case DockStyle.Right:
						a = Left + pos;
						pos = a - se.SplitX;
						break;
					case DockStyle.Top:
						a = Top - pos;
						pos = se.SplitY - a;
						break;
					case DockStyle.Bottom:
						a = Top + pos;
						pos = a - se.SplitY;
						break;
				}
				if (SplitPosition != pos) SplitPosition = pos;
			} 
			base.OnSplitterMoving(se);
		}

		public bool Opaque
		{
			get
			{
				return m_bOpaque;
			}
			set
			{
				m_bOpaque = value;
			}
		}
	}
}
