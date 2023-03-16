// ここにソースコードの注釈を書きます。

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Girl.Windows.Forms.Drawing2D
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class CanvasGroup
	{
		protected Guid guid;
		public CanvasGroup Group;
		protected bool visible;

		public CanvasGroup()
		{
			this.guid = Guid.NewGuid();
			this.Group = null;
			this.visible = true;
		}

		public static bool CanGroup(CanvasObject[] cos)
		{
			if (cos == null || cos.Length < 2) return false;
			CanvasGroup gr = cos[0].RootGroup;
			if (gr == null) return true;
			foreach (CanvasObject co in cos)
			{
				if (co.RootGroup != gr) return true;
			}
			return false;
		}

		public static bool CanUngroup(CanvasObject[] cos)
		{
			if (cos == null) return false;
			foreach (CanvasObject co in cos)
			{
				if (co.Group != null) return true;
			}
			return false;
		}

		public CanvasGroup RootGroup
		{
			get
			{
				if (this.Group == null) return this;
				return this.Group.RootGroup;
			}
		}

		public bool Visible
		{
			get
			{
				if (!this.visible) return false;
				if (this.Group == null) return true;
				return this.Group.Visible;
			}

			set
			{
				this.visible = value;
			}
		}

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		public bool IsAncestorOf(CanvasGroup gr)
		{
			if (gr == this) return true;
			if (this.Group == null) return false;
			return this.Group.IsAncestorOf(gr);
		}

		public void Ungroup(CanvasGroup gr)
		{
			if (this.Group == null) return;
			if (this.Group == gr)
			{
				this.Group = null;
				return;
			}
			this.Group.Ungroup(gr);
		}
	}
}
