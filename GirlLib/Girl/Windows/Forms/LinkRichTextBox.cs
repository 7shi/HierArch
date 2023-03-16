// このファイルは LinkRichTextBox.hacls から生成されています。

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// リンクをサポートした読み取り専用の RichTextBox です。
	/// </summary>
	public class LinkRichTextBox : ExRichTextBox
	{
		public struct LinkTargetInfo 
		{
			public int Start, Length;
			public object Target;
		
			public LinkTargetInfo(int start, int length, object target)
			{
				this.Start  = start;
				this.Length = length;
				this.Target = target;
			}
		}
		
		public class LinkTargetEventArgs : EventArgs
		{
			public object Target;
		
			public LinkTargetEventArgs(object target)
			{
				this.Target = target;
			}
		}
		
		public delegate void LinkTargetEventHandler(object sender, LinkTargetEventArgs e);
		public event LinkTargetEventHandler LinkTargetNotify;
		public event LinkTargetEventHandler LinkTargetClicked;
		private Cursor oldCursor;
		private Color linkColor;
		private ArrayList links;
		private object linkTarget;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public LinkRichTextBox()
		{
			this.ReadOnly   = true;
			this.oldCursor  = this.Cursor;
			this.linkColor  = Color.Blue;
			this.links      = new ArrayList();
			this.linkTarget = null;
		}

		public Color LinkColor
		{
			get
			{
				return this.linkColor;
			}

			set
			{
				this.linkColor = value;
			}
		}

		#region Link

		public new void Clear()
		{
			this.SetTarget(null);
			this.links.Clear();
			base.Clear();
		}

		public void AppendLink(string text, object target)
		{
			this.links.Add(new LinkTargetInfo(this.TextLength, text.Length, target));
			this.AppendText(text, this.LinkColor, FontStyle.Underline);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			
			int pos = this.GetCharIndexFromPosition(new Point(e.X, e.Y));
			object target = null;
			LinkTargetInfo li;
			foreach (object obj in this.links)
			{
				li = (LinkTargetInfo)obj;
				if (li.Start <= pos && pos < li.Start + li.Length)
				{
					target = li.Target;
					break;
				}
			}
			this.SetTarget(target);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			this.SetTarget(null);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (this.linkTarget == null || this.LinkTargetClicked == null) return;
			
			this.LinkTargetClicked(this, new LinkTargetEventArgs(this.linkTarget));
		}

		private void SetTarget(object target)
		{
			if (target == this.linkTarget) return;
			
			this.linkTarget = target;
			this.Cursor = (target == null) ? this.oldCursor : Cursors.Hand;
			if (this.LinkTargetNotify == null) return;
			
			this.LinkTargetNotify(this, new LinkTargetEventArgs(target));
		}

		#endregion
	}
}
