using System;
using System.Collections;
using System.Drawing;
using System.Text;
using Girl.Text;

namespace Girl.Rtf
{
	/// <summary>
	/// RTF と HTML を相互変換します。
	/// </summary>
	public class RtfHtmlConverter
	{
		public static string Convert(RtfDocument rd)
		{
			StringBuilder ret;
			RtfContext rc1;
			RtfContext rc2;
			RtfObject ro;
			RtfFont rf;
			Stack stack1;
			Stack stack2;
			RtfFont[] fonts;
			Color[] colors;

			ret = new StringBuilder();
			rc1 = new RtfContext();
			rc2 = new RtfContext();
			stack1 = new Stack();
			stack2 = new Stack();
			fonts = rd.FontTable.Fonts;
			colors = rd.ColorTable.Colors;
			foreach (object obj in rd.Document)
			{
				ro = obj as RtfObject;
				rc2 = rc1;
				rc1.Read(ro);
				RtfHtmlConverter.CheckContext(fonts, colors, stack1, rc1, rc2);
				if (ro.IsText)
				{
					RtfHtmlConverter.WriteStack(ret, stack1, stack2);
					rf = fonts[rc1.Font];
					ret.Append(ro.GetText(CharSetEncoding.GetEncoding(rf.CharSet)));
				}
				else if (ro.Name == "\\par")
				{
					RtfHtmlConverter.WriteStack(ret, stack1, stack2);
					ret.Append("<br/>\r\n");
				}
			}
			RtfHtmlConverter.WritePop(ret, stack2, null);
			// スタックを解消
			return ret.ToString();
		}

		private static void CheckContext(RtfFont[] fonts, Color[] colors, Stack stack1, RtfContext rc1, RtfContext rc2)
		{
			if (rc1.Bold != rc2.Bold)
			{
				if (rc1.Bold)
				{
					stack1.Push("b");
				}
				else
				{
					RtfHtmlConverter.WritePop(null, stack1, "b");
				}
			}
			if (rc1.Italic != rc2.Italic)
			{
				if (rc1.Italic)
				{
					stack1.Push("i");
				}
				else
				{
					RtfHtmlConverter.WritePop(null, stack1, "i");
				}
			}
			if (rc1.Underline != rc2.Underline)
			{
				if (rc1.Underline)
				{
					stack1.Push("u");
				}
				else
				{
					RtfHtmlConverter.WritePop(null, stack1, "u");
				}
			}
			if (rc1.Color != rc2.Color)
			{
				RtfHtmlConverter.WritePop(null, stack1, "font color=");
				if (rc1.Color > 0)
				{
					Color c = colors[rc1.Color];
					stack1.Push(string.Format("font color=\"#{0:x2}{1:x2}{2:x2}\"", c.R, c.G, c.B));
				}
			}
			if (rc1.Font != rc2.Font)
			{
				RtfHtmlConverter.WritePop(null, stack1, "font face=");
				if (rc1.Font > 0)
				{
					stack1.Push(string.Format("font face=\"{0}\"", fonts[rc1.Font].Name));
				}
			}
			if (rc1.FontSize != rc2.FontSize)
			{
				RtfHtmlConverter.WritePop(null, stack1, "font size=");
				if (rc1.FontSize != 18)  // 決め打ちでも問題ないと思う
				{
					stack1.Push(string.Format("font size=\"{0}\"",((double) rc1.FontSize) / 2));
				}
			}
		}

		private static void WriteStack(StringBuilder sb, Stack stack1, Stack stack2)
		{
			object[] st1;
			object[] st2;
			int common;
			string str;
			int p;
			Stack st3;

			st3 = new Stack();
			// 配列が Push 順ではなく Pop された順番（逆向き）になるのに注意！
			st1 = stack1.ToArray();
			st2 = stack2.ToArray();
			// 共通部分を調べて無視する
			int len1 = stack1.Count;
			int len2 = stack2.Count;
			int lenm = Math.Min(len1, len2);
			for (common = 0; common < lenm; common++)
			{
				// 配列が逆向きなのに注意！
				if (!st1[len1 - common - 1].Equals(st2[len2 - common - 1])) break;
			}
			// スタックを解消する
			while (stack2.Count > common)
			{
				str = stack2.Pop() as string;
				p = str.IndexOf(" ");
				if (p > 0) str = str.Substring(0, p);
				sb.AppendFormat("</{0}>", str);
			}
			// スタックを積む
			for (int i = common; i < len1; i++)
			{
				string s = st1[len1 - i - 1] as string;
				st3.Push(s);
			}
			while (st3.Count > 0)
			{
				string ss = st3.Pop() as string;
				sb.AppendFormat("<{0}>", ss);
				stack2.Push(ss);
			}
			// stack2 は stack1 と同じ内容になる
		}

		private static void WritePop(StringBuilder sb, Stack stack, string tag)
		{
			Stack st;
			string ss;
			string str;
			int p;

			st = new Stack();
			// スタックに存在するかチェック
			if (tag != null)
			{
				bool ok = false;
				foreach (object obj in stack.ToArray())
				{
					string s = obj as string;
					if (s == tag || s.StartsWith(tag))
					{
						ok = true;
						break;
					}
				}
				if (!ok) return;
			}
			while (stack.Count > 0)
			{
				ss = str = stack.Pop() as string;
				p = str.IndexOf(" ");
				if (p > 0) str = str.Substring(0, p);
				if (sb != null) sb.AppendFormat("</{0}>", str);
				if (tag != null &&(ss == tag || ss.StartsWith(tag))) break;
				st.Push(ss);
			}
			if (tag == null) return;
			// スタック解消に使うため
			// スタックを積み直す
			while (st.Count > 0)
			{
				str = st.Pop() as string;
				if (sb != null) sb.AppendFormat("<{0}>", str);
				stack.Push(str);
			}
		}
	}
}
