// このファイルは ..\..\Girl.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Girl.Rtf
{
	/// <summary>
	/// RTF の要素を保持します。
	/// </summary>
	public class RtfObject
	{
		protected string name;
		protected ArrayList rtfObjects;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public RtfObject()
		{
			this.name = "";
			this.rtfObjects = new ArrayList();
		}

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public RtfObject(string nm)
		{
			this.name = nm;
			this.rtfObjects = new ArrayList();
		}

		#region Properties

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public ArrayList RtfObjects
		{
			get
			{
				return this.rtfObjects;
			}
		}

		public bool IsText
		{
			get
			{
				int len;

				len = this.name.Length;
				if (len < 1 || this.name[0] != '\\' || len < 2) return true;
				if (len > 2 && (this.name[1] == 'c' || this.name[1] == 'u')
					&& char.IsDigit(this.name[2])) return true;
				
				return RtfObject._IsChar(this.name[1]);
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.name == "" && this.rtfObjects.Count < 1;
			}
		}

		public bool IsFontNumber
		{
			get
			{
				return this.name.Length > 2
					&& this.name.StartsWith("\\f")
					&& char.IsDigit(this.name[2]);
			}
		}

		public string Text
		{
			get
			{
				return this.GetText(Encoding.Default);
			}
		}

		public int Value
		{
			get
			{
				StringBuilder sb;

				sb = new StringBuilder();
				foreach (char ch in this.name)
				{
					if (char.IsDigit(ch)) sb.Append(ch);
				}
				if (sb.Length < 1) return 0;
				
				return Convert.ToInt32(sb.ToString());
			}
		}

		#endregion

		public string GetText(Encoding encoding)
		{
			StringBuilder text;
			MemoryStream ms;
			StringReader sr;
			Decoder d;
			int ch;
			char ch2;

			text = new StringBuilder();
			ms = new MemoryStream();
			sr = new StringReader(this.name);
			d = encoding.GetDecoder();
			while ((ch = sr.Read()) != -1)
			{
				ch2 = (char)ch;
				if (ch2 == '\\')
				{
					this.ReadEscape(text, ms, sr, d);
				}
				else if (' ' <= ch2 && ch2 < 128)
				{
					ms.WriteByte((byte)ch2);
				}
				else
				{
					this.AppendText(text, ms, d);
					text.Append(ch2);
				}
			}
			this.AppendText(text, ms, d);
			sr.Close();
			ms.Close();
			return text.ToString();
		}

		private void ReadEscape(StringBuilder text, MemoryStream ms, StringReader sr, Decoder d)
		{
			int ch;
			char ch2;

			ch = sr.Read();
			if (ch == -1) return;
			
			ch2 = (char)ch;
			if (ch2 == '\'')
			{
				this.ReadByte(ms, sr);
			}
			else if (ch2 == 'u')
			{
				this.AppendText(text, ms, d);
				this.ReadUnicode(text, sr);
			}
			else
			{
				this.AppendText(text, ms, d);
				text.Append(ch2);
			}
		}

		private void ReadByte(MemoryStream ms, StringReader sr)
		{
			int ch;
			string hex;

			ch = sr.Read();
			if (ch == -1) return;
			
			hex = ((char)ch).ToString();
			
			ch = sr.Read();
			if (ch == -1) return;
			
			hex += (char)ch;
			
			try
			{
				ms.WriteByte(byte.Parse(hex, NumberStyles.HexNumber));
			}
			catch
			{
			}
		}

		private void ReadUnicode(StringBuilder text, StringReader sr)
		{
			StringBuilder sb;
			int ch;
			char ch2;

			sb = new StringBuilder();
			while ((ch = sr.Read()) != -1)
			{
				ch2 = (char)ch;
				if (!char.IsDigit(ch2)) break;
				sb.Append(ch2);
			}
			if (sb.Length < 1) return;
			
			try
			{
				text.Append((char)Convert.ToInt32(sb.ToString()));
			}
			catch
			{
			}
		}

		private void AppendText(StringBuilder text, MemoryStream ms, Decoder d)
		{
			Byte[] bytes;
			char[] chars;

			if (ms.Length < 1 ) return;
			
			bytes = ms.ToArray();
			ms.SetLength(0);
			
			chars = new char[d.GetCharCount(bytes, 0, bytes.Length)];
			d.GetChars(bytes, 0, bytes.Length, chars, 0);
			text.Append(chars);
		}

		public void Parse(StringReader sr)
		{
			int ch1;
			char ch;
			StringBuilder sb;
			RtfObject ro;
			bool escape;
			bool u;

			sb = new StringBuilder();
			escape = u = false;
			while ((ch1 = sr.Read()) >= 0)
			{
				ch = (char)ch1;
				if (u)
				{
					if (!char.IsDigit(ch))
					{
						this.AddRtfObject(sb);
					}
					sb.Append("\\u");
					sb.Append(ch);
					u = false;
				}
				else if (escape)
				{
					bool text = RtfObject._IsText(sb);
					if (text && ch == 'u')
					{
						u = true;
					}
					else
					{
						if (text && !RtfObject._IsChar(ch))
						{
							this.AddRtfObject(sb);
						}
						sb.Append('\\');
						sb.Append(ch);
					}
					escape = false;
				}
				else if (ch == '}')
				{
					break;
				}
				else if (ch == '{')
				{
					this.AddRtfObject(sb);
					ro = new RtfObject();
					ro.Parse(sr);
					this.AddRtfObject(ro);
				}
				else if (ch == '\\')
				{
					if (!RtfObject._IsText(sb)) this.AddRtfObject(sb);
					escape = true;
				}
				else if (ch == ' ' && !RtfObject._IsText(sb))
				{
					this.AddRtfObject(sb);
				}
				else if (ch == '\r' || ch == '\n')
				{
					this.AddRtfObject(sb);
				}
				else
				{
					sb.Append(ch);
				}
			}
			this.AddRtfObject(sb);
		}

		protected static bool _IsText(StringBuilder sb)
		{
			int len;

			len = sb.Length;
			if (len < 1 || sb[0] != '\\' || len < 2) return true;
			if (len > 2 && (sb[1] == 'c' || sb[1] == 'u')
				&& char.IsDigit(sb[2])) return true;
			
			return RtfObject._IsChar(sb[1]);
		}

		protected static bool _IsChar(char ch)
		{
			return ch == '\\' || ch == '{' || ch == '}' || ch == '\'';
		}

		public void AddRtfObject(string name)
		{
			this.AddRtfObject(new RtfObject(name));
		}

		protected RtfObject AddRtfObject(StringBuilder sb)
		{
			int len;
			RtfObject ret;

			len = sb.Length;
			if (len < 1) return null;
			
			if (this.name == "")
			{
				this.name = sb.ToString();
				ret = this;
			}
			else
			{
				ret = new RtfObject(sb.ToString());
				this.AddRtfObject(ret);
			}
			sb.Remove(0, len);
			return ret;
		}

		protected virtual void AddRtfObject(RtfObject ro)
		{
			this.rtfObjects.Add(ro);
		}

		public void ParseNode(TreeNode node)
		{
			RtfObject ro;

			this.name = node.Text;
			
			foreach (TreeNode n in node.Nodes)
			{
				ro = new RtfObject();
				ro.ParseNode(n);
				this.AddRtfObject(ro);
			}
		}

		public void GenerateRtf(StringBuilder sb)
		{
			if (this.IsEmpty) return;
			
			if (this.rtfObjects.Count < 1)
			{
				sb.Append(this.name);
				if (this.name == "\\par") sb.Append("\r\n");
				return;
			}
			
			sb.Append('{');
			sb.Append(this.name);
			
			this.GenerateRtfChild(sb);
			
			sb.Append('}');
			if (this.name == "\\colortbl" || this.name == "\\fonttbl" || this.name == "\\rtf1")
			{
				sb.Append("\r\n");
			}
		}

		protected virtual void GenerateRtfChild(StringBuilder sb)
		{
			RtfObject prev;
			RtfObject ro;

			prev = null;
			foreach (object obj in this.rtfObjects)
			{
				ro = obj as RtfObject;
				if (sb.Length > 0 && sb[sb.Length - 1] != '\n'
					&& !ro.Name.StartsWith("\\")
					&& (prev == null || !prev.IsText))
				{
					sb.Append(' ');
				}
				ro.GenerateRtf(sb);
				prev = ro;
			}
		}

		public void DisplayTreeView(TreeView treeView, TreeNode parent)
		{
			TreeNode n;

			if (this.IsEmpty) return;
			
			n = new TreeNode(this.name);
			if (parent == null)
			{
				treeView.Nodes.Add(n);
			}
			else
			{
				parent.Nodes.Add(n);
			}
			
			this.DisplayTreeViewChild(treeView, n);
		}

		protected virtual void DisplayTreeViewChild(TreeView treeView, TreeNode node)
		{
			foreach (object obj in this.rtfObjects)
			{
				(obj as RtfObject).DisplayTreeView(treeView, node);
			}
		}

		public bool Exists(string target)
		{
			if (this.name == target) return true;
			
			foreach (object obj in this.rtfObjects)
			{
				if ((obj as RtfObject).Exists(target)) return true;
			}
			
			return false;
		}

		public void Remove(string target)
		{
			ArrayList ros;
			RtfObject ro;

			ros = new ArrayList();
			foreach (object obj in this.rtfObjects)
			{
				ro = obj as RtfObject;
				if (ro.name == target) continue;
				
				ros.Add(ro);
				ro.Remove(target);
			}
			this.rtfObjects = ros;
		}

		public static string ConvertText(string text)
		{
			StringBuilder sb;

			sb = new StringBuilder();
			foreach (char ch in text)
			{
				if (ch == '\\' || ch == '{' || ch == '}')
				{
					sb.Append('\\');
				}
				if (' ' <= ch && ch < 128)
				{
					sb.Append(ch);
				}
				else
				{
					sb.Append(string.Format("\\u{0}?", (int)ch));
				}
			}
			return sb.ToString();
		}

		public static string ConvertLocalText(string text)
		{
			StringBuilder sb;
			Encoder e;
			char[] chars;
			byte[] bytes;
			char ch;

			sb = new StringBuilder();
			e = Encoding.Default.GetEncoder();
			chars = text.ToCharArray();
			bytes = new byte[e.GetByteCount(chars, 0, text.Length, false)];
			e.GetBytes(chars, 0, text.Length, bytes, 0, false);
			foreach (byte b in bytes)
			{
				ch = (char)b;
				if (ch == '\\' || ch == '{' || ch == '}')
				{
					sb.Append('\\');
				}
				if (' ' <= ch && ch < 128)
				{
					sb.Append(ch);
				}
				else
				{
					sb.Append(string.Format("\\'{0:x2}", b));
				}
			}
			return sb.ToString();
		}
	}
}
