// このファイルは ..\..\Girl.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.Text;

namespace Girl.Text
{
	public enum CharSet
	{
		ANSI        =   0,
		Default     =   1,
		Symbol      =   2,
		Mac         =  77,
		ShiftJIS    = 128,
		Hangeul     = 129,
		Hangul      = 129,
		Johab       = 130,
		GB2312      = 134,
		ChineseBig5 = 136,
		Greek       = 161,
		Turkish     = 162,
		Vietnamese  = 163,
		Hebrew      = 177,
		Arabic      = 178,
		Baltic      = 186,
		Russian     = 204,
		Thai        = 222,
		EastEurope  = 238,
		OEM         = 255
	}
}

namespace Girl.Text
{
	/// <summary>
	/// キャラクタセットを Encoding に変換します。
	/// </summary>
	public class CharSetEncoding
	{
		public static Encoding GetEncoding(int charSet)
		{
			switch (charSet)
			{
				case (int)CharSet.Default:
					return Encoding.Default;
				case (int)CharSet.Mac:
					return Encoding.GetEncoding("macintosh");
				case (int)CharSet.ShiftJIS:
					return Encoding.GetEncoding("shift_jis");
				case (int)CharSet.Hangul:
					return Encoding.GetEncoding("ks_c_5601-1987");
				case (int)CharSet.Johab:
					return Encoding.GetEncoding("Johab");
				case (int)CharSet.GB2312:
					return Encoding.GetEncoding("gb2312");
				case (int)CharSet.ChineseBig5:
					return Encoding.GetEncoding("big5");
				case (int)CharSet.Greek:
					return Encoding.GetEncoding("iso-8859-7");
				case (int)CharSet.Turkish:
					return Encoding.GetEncoding("windows-1254");
				case (int)CharSet.Vietnamese:
					return Encoding.GetEncoding("windows-1258");
				case (int)CharSet.Hebrew:
					return Encoding.GetEncoding("windows-1255");
				case (int)CharSet.Arabic:
					return Encoding.GetEncoding("windows-1256");
				case (int)CharSet.Baltic:
					return Encoding.GetEncoding("windows-1257");
				case (int)CharSet.Russian:
					return Encoding.GetEncoding("windows-1251");
				case (int)CharSet.Thai:
					return Encoding.GetEncoding("windows-874");
				case (int)CharSet.EastEurope:
					return Encoding.GetEncoding("windows-1250");
				case (int)CharSet.OEM:
					return Encoding.GetEncoding("IBM437");
			}
			return Encoding.ASCII;
		}
	}
}
