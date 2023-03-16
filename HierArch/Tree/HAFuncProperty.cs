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
	public class HAFuncProperty : HANodeProperty
	{
		public HAFuncProperty(HAFuncNode node) : base(node)
		{
		}
		
		public class HAFuncFileNameEditor : FileNameEditor
		{
			protected override void InitializeDialog(OpenFileDialog openFileDialog)
			{
				base.InitializeDialog(openFileDialog);
				openFileDialog.Filter = "HierArch 関数 (*.hafnc)|*.hafnc|すべてのファイル (*.*)|*.*";
				openFileDialog.CheckFileExists = false;
			}
		}
		
		[Editor(typeof(HAFuncFileNameEditor), typeof(UITypeEditor))]
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

		public bool EnableRtf
		{
			get
			{
				return (this.node as HAFuncNode).EnableRtf;
			}

			set
			{
				(this.node as HAFuncNode).EnableRtf = value;
			}
		}
	}
}
