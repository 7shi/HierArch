// このファイルは Rtf.hacls から生成されています。

using System;
using System.Collections;
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

			ret = new StringBuilder();
			rc1 = new RtfContext();
			rc2 = new RtfContext();
			stack1 = new Stack();
			stack2 = new Stack();
			foreach (object obj in rd.Document)
			{
				ro = obj as RtfObject;
				rc2 = rc1;
				rc1.Read(ro);
				RtfHtmlConverter.CheckContext(stack1, rc1, rc2);
				if (ro.IsText)
				{
					RtfHtmlConverter.WriteStack(ret, stack1, stack2);
					rf = rd.FontTable.Fonts[rc1.Font];
					ret.Append(ro.GetText(
						CharSetEncoding.GetEncoding(rf.CharSet)));
				}
				else if (ro.Name == "\\par")
				{
					RtfHtmlConverter.WriteStack(ret, stack1, stack2);
					ret.Append("<br>\r\n");
				}
			}
			RtfHtmlConverter.WritePop(ret, stack2, null);  // スタックを解消
			return ret.ToString();
		}

		private static void CheckContext(Stack stack1, RtfContext rc1, RtfContext rc2)
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
		}

		private static void WriteStack(StringBuilder sb, Stack stack1, Stack stack2)
		{
			object[] st1;
			object[] st2;
			int common;

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
				sb.AppendFormat("</{0}>", stack2.Pop());
			}
			
			// スタックを積む
			for (int i = common; i < len1; i++)
			{
				string s = st1[len1 - i - 1] as string;
				sb.AppendFormat("<{0}>", s);
				stack2.Push(s);
			}
			
			// stack2 は stack1 と同じ内容になる
		}

		private static void WritePop(StringBuilder sb, Stack stack, string tag)
		{
			Stack st;
			string ss;

			st = new Stack();
			while (stack.Count > 0)
			{
				ss = stack.Pop() as string;
				if (sb != null) sb.AppendFormat("</{0}>", ss);
				if (ss == tag) break;
				
				st.Push(ss);
			}
			if (tag == null) return;  // スタック解消に使うため
			
			// スタックを積み直す
			while (st.Count > 0)
			{
				ss = st.Pop() as string;
				if (sb != null) sb.AppendFormat("<{0}>", ss);
				stack.Push(ss);
			}
		}
	}
}
