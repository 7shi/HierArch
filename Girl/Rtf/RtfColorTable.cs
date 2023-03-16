// このファイルは ..\..\Girl.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.Drawing;
using System.Text;

namespace Girl.Rtf
{
	/// <summary>
	/// RTF の \colortbl を管理します。
	/// </summary>
	public class RtfColorTable : RtfObject
	{
		public Color[] colors;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public RtfColorTable()
		{
			this.colors = null;
		}

		public void SetObject(RtfObject ro)
		{
			this.colors = null;
			this.name = ro.Name;
			this.rtfObjects = ro.RtfObjects;
		}

		public Color[] Colors
		{
			get
			{
				string v;
				int r;
				int g;
				int b;
				int i;

				if (this.colors != null) return this.colors;
				
				this.colors = new Color[this.ColorCount];
				r = g = b = i = 0;
				foreach (RtfObject ro in this.rtfObjects)
				{
					v = ro.Name;
					if (v.StartsWith("\\red"))
					{
						r = ro.Value;
					}
					else if (v.StartsWith("\\green"))
					{
						g = ro.Value;
					}
					else if (v.StartsWith("\\blue"))
					{
						b = ro.Value;
					}
					if (v.EndsWith(";"))
					{
						this.colors[i] = Color.FromArgb(r, g, b);
						i++;
					}
				}
				if (i == 0) this.colors[0] = Color.FromArgb(0);
				return this.colors;
			}
		}

		public int ColorCount
		{
			get
			{
				int ret;

				if (this.colors != null) return this.colors.GetLength(0);
				
				ret = 0;
				foreach (RtfObject ro in this.rtfObjects)
				{
					if (ro.Name.EndsWith(";")) ret++;
				}
				if (ret < 1) ret++;
				return ret;
			}
		}

		public string TableText
		{
			get
			{
				StringBuilder sb;

				sb = new StringBuilder();
				foreach (Color c in this.Colors)
				{
					sb.Append(string.Format("{0}, {1}, {2}\r\n", c.R, c.G, c.B));
				}
				return sb.ToString();
			}
		}

		public int GetIndex(Color color)
		{
			int ret;

			ret = 0;
			foreach (Color cc in this.Colors)
			{
				if (cc.R == color.R
					&& cc.G == color.G
					&& cc.B == color.B)
				{
					return ret;
				}
				ret++;
			}
			
			this.colors = null;
			if (this.name == "") this.name = "\\colortbl";
			if (this.rtfObjects.Count < 1)
			{
				this.AddRtfObject(";");
				ret = 1;
			}
			this.AddRtfObject(string.Format("\\red{0}"  , color.R));
			this.AddRtfObject(string.Format("\\green{0}", color.G));
			this.AddRtfObject(string.Format("\\blue{0};", color.B));
			return ret;
		}

		public bool Contains(Color c)
		{
			return this.GetIndex(c) != -1;
		}
	}
}
