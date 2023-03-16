// このファイルは ExComboBox.hacls から生成されています。

using System;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// ComboBox 内での Enter キーと Escape キーの入力を監視します。
	/// </summary>
	public class ExComboBox : ComboBox
	{
		public event EventHandler EnterKeyPress;
		public event EventHandler EscapeKeyPress;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public ExComboBox()
		{
		}

		public override bool PreProcessMessage(ref Message m)
		{
			const int WM_KEYDOWN = 256;
			if (m.Msg == WM_KEYDOWN && !this.DroppedDown)
			{
				switch ((int)m.WParam)
				{
					case 13:
						if (this.EscapeKeyPress != null) this.EnterKeyPress(this, EventArgs.Empty);
						return true;
					case 27:
						if (this.EscapeKeyPress != null) this.EscapeKeyPress(this, EventArgs.Empty);
						return true;
				}
			}
			
			return base.PreProcessMessage(ref m);
		}
	}
}
