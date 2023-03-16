// このファイルは ..\..\..\Girl.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// フォント名を列挙する ComboBox です。
	/// </summary>
	public class FontComboBox : ExComboBox
	{
		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public FontComboBox()
		{
			this.DrawMode = DrawMode.OwnerDrawVariable;
		}

		public void InitializeItems()
		{
			this.Items.Clear();
			FontFamily[] ffs = FontFamily.Families;
			ArrayList al = new ArrayList();
			foreach (FontFamily ff in ffs)
			{
				if (ff.IsStyleAvailable(FontStyle.Regular)) al.Add(ff.Name);
				ff.Dispose();
			}
			al.Sort();
			this.Items.AddRange(al.ToArray());
		}

		protected override void OnMeasureItem(MeasureItemEventArgs e)
		{
			base.OnMeasureItem(e);
			
			Font f = new Font(this.Items[e.Index].ToString(), this.Font.Size);
			e.ItemHeight = (int)e.Graphics.MeasureString(f.Name, f).Height;
			f.Dispose();
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			base.OnDrawItem(e);
			
			Font f = new Font(this.Items[e.Index].ToString(), this.Font.Size);
			Brush b = (e.State == DrawItemState.Selected)
				? SystemBrushes.HighlightText : SystemBrushes.WindowText;
			e.DrawBackground();
			e.Graphics.DrawString(f.Name, f, b, e.Bounds);
			f.Dispose();
		}
	}
}
