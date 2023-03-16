// このファイルは Rtf.hacls から生成されています。

using System;
using System.Text;

namespace Girl.Rtf
{
	/// <summary>
	/// RTF の \fonttbl を管理します。
	/// </summary>
	public class RtfFontTable : RtfObject
	{
		public RtfFont[] fonts;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public RtfFontTable()
		{
			this.fonts = null;
		}

		public void SetObject(RtfObject ro)
		{
			this.fonts = null;
			this.name = ro.Name;
			this.rtfObjects = ro.RtfObjects;
		}

		public RtfFont[] Fonts
		{
			get
			{
				int i;

				if (this.fonts != null) return this.fonts;
				
				this.fonts = new RtfFont[this.rtfObjects.Count];
				
				i = 0;
				foreach (RtfObject ro in this.rtfObjects)
				{
					this.fonts[i] = new RtfFont();
					this.fonts[i].SetFont(ro);
					i++;
				}
				return this.fonts;
			}
		}

		public string TableText
		{
			get
			{
				StringBuilder sb;

				sb = new StringBuilder();
				this.Write(sb);
				return sb.ToString();
			}
		}

		public void Write(StringBuilder sb)
		{
			foreach (RtfFont rf in this.Fonts)
			{
				rf.Write(sb);
			}
		}

		public int GetIndex(RtfFont rf)
		{
			int ret;
			RtfObject ro;

			ret = 0;
			foreach (RtfFont rf2 in this.Fonts)
			{
				if (rf2.Equals(rf)) return ret;
				ret++;
			}
			
			this.fonts = null;
			if (this.name == "") this.name = "\\fonttbl";
			ro = new RtfObject(string.Format("\\f{0}", ret));
			ro.AddRtfObject(string.Format("\\f{0}"       , rf.Family ));
			if (rf.CharSet >= 0)
			{
				ro.AddRtfObject(string.Format("\\fcharset{0}", rf.CharSet));
			}
			ro.AddRtfObject(string.Format("{0};"         , rf.Name   ));
			this.AddRtfObject(ro);
			return ret;
		}

		public bool Contains(RtfFont rf)
		{
			return this.GetIndex(rf) != -1;
		}
	}
}
