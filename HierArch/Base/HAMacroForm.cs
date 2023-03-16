// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

// ここにソースコードの注釈を書きます。

using System;

namespace Girl.HierArch
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class HAMacroForm : Form1
	{
		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAMacroForm()
		{
			this.mnuBuildGenerate.Enabled = false;
			this. tbBuildGenerate.Enabled = false;
			this.Open(HADoc.MacroProject);
		}

		protected override void Dispose(bool disposing)
		{
			Form1.MacroForm = null;
			base.Dispose(disposing);
		}
	}
}
