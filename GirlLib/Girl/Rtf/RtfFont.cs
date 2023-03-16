// このファイルは Rtf.hacls から生成されています。

using System;
using System.Drawing;
using System.Text;

namespace Girl.Rtf
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class RtfFont
	{
		public string Name;
		public string Family;
		public int CharSet;

		private void Init()
		{
			this.Name = this.Family = "";
			this.CharSet = (int)Girl.Text.CharSet.Default;
		}

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public RtfFont()
		{
			this.Init();
		}

		public RtfFont(RtfObject font)
		{
			this.SetFont(font);
		}

		public RtfFont(string name)
		{
			this.Init();
			
			this.Name   = RtfObject.ConvertLocalText(name);
			this.Family = "nil";
		}

		public RtfFont(string name, string family)
		{
			this.Init();
			
			this.Name   = RtfObject.ConvertLocalText(name);
			this.Family = family;
		}

		public RtfFont(string name, string family, int charSet)
		{
			this.Name    = RtfObject.ConvertLocalText(name);
			this.Family  = family;
			this.CharSet = charSet;
		}

		public RtfFont(Font f)
		{
			this.Init();
			
			this.Name    = RtfObject.ConvertLocalText(f.Name);
			this.Family  = "nil";
			this.CharSet = (int)f.GdiCharSet;
		}

		public RtfFont(RtfFont font)
		{
			this.Name    = font.Name;
			this.Family  = font.Family;
			this.CharSet = font.CharSet;
		}

		public void SetFont(RtfObject font)
		{
			RtfObject ro;

			this.Init();
			if (font.RtfObjects.Count < 1) return;
			
			foreach (object obj in font.RtfObjects)
			{
				ro = obj as RtfObject;
				if (ro.Name.StartsWith(@"\fcharset"))
				{
					try
					{
						this.CharSet = Convert.ToInt32(ro.Name.Substring(9));
					}
					catch
					{
					}
				}
				else if (ro.IsText && ro.Name.EndsWith(";"))
				{
					string s = ro.Text;
					this.Name = s.Substring(0, s.Length - 1);
				}
				else if (ro.Name.StartsWith("\\f"))
				{
					this.Family = ro.Name.Substring(2);
				}
			}
		}

		public void Write(StringBuilder sb)
		{
			sb.Append(this.ToString() + "\r\n");
		}

		public override string ToString()
		{
			return string.Format("{0}, {1}, {2}",
				this.Name, this.Family, this.CharSet);
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			RtfFont rf;

			rf = obj as RtfFont;
			if (rf == null) return false;
			
			return this.Name == rf.Name
				&& this.Family == rf.Family
				&& this.CharSet == rf.CharSet;
		}
	}
}
