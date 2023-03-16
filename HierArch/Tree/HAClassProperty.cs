// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Girl.HierArch
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class HAClassProperty : HANodeProperty
	{
		public HAClassProperty(HAClassNode node) : base(node)
		{
		}
		
		public class HAClassFileNameEditor : FileNameEditor
		{
			protected override void InitializeDialog(OpenFileDialog openFileDialog)
			{
				base.InitializeDialog(openFileDialog);
				openFileDialog.Filter = "HierArch クラス (*.hacls)|*.hacls|すべてのファイル (*.*)|*.*";
				openFileDialog.CheckFileExists = false;
			}
		}
		
		[Editor(typeof(HAClassFileNameEditor), typeof(UITypeEditor))]
		public string Link
		{
			get
			{
				return this.node.Link;
			}
			
			set
			{
				this.node.Link = value;
			}
		}
	}
}
