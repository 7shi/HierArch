using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Girl.Text;

namespace Girl.Rtf
{
	public struct RtfContext
	{
		public int Font, FontSize, Color;
		public bool Bold, Italic, Underline;
	
		public void Read(RtfObject ro)
		{
			switch (ro.Name)
			{
				case "\\b":
					this.Bold = true;
					return;
				case "\\b0":
					this.Bold = false;
					return;
				case "\\i":
					this.Italic = true;
					return;
				case "\\i0":
					this.Italic = false;
					return;
				case "\\ul":
					this.Underline = true;
					return;
				case "\\ulnone":
					this.Underline = false;
					return;
			}
			
			if (ro.Name.StartsWith("\\fs"))
			{
				this.FontSize = ro.Value;
			}
			else if (ro.Name.StartsWith("\\f"))
			{
				this.Font = ro.Value;
			}
			else if (ro.Name.StartsWith("\\cf"))
			{
				this.Color = ro.Value;
			}
		}
	}
}

namespace Girl.Rtf
{
	/// <summary>
	/// RTF の要素を保持します。
	/// </summary>
	public class RtfDocument : RtfObject
	{
		private RtfFontTable fontTable;
		private RtfColorTable colorTable;
		private ArrayList document;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public RtfDocument()
		{
			this.fontTable = new RtfFontTable();
			this.colorTable = new RtfColorTable();
			this.document = new ArrayList();
		}

		#region Properties

		public RtfFontTable FontTable
		{
			get
			{
				return this.fontTable;
			}
		}

		public RtfColorTable ColorTable
		{
			get
			{
				return this.colorTable;
			}
		}

		public ArrayList Document
		{
			get
			{
				return this.document;
			}
		}

		public RtfContext LastContext
		{
			get
			{
				RtfContext ret;

				ret = new RtfContext();
				foreach (object obj in this.document)
				{
					ret.Read(obj as RtfObject);
				}
				return ret;
			}
		}

		#endregion

		public static RtfDocument Parse(string rtf)
		{
			RtfDocument ret;
			StringReader sr;
			int ch1;
			char ch;

			ret = new RtfDocument();
			sr = new StringReader(rtf);
			while ((ch1 = sr.Read()) >= 0)
			{
				ch =(char) ch1;
				if (ch == '{')
				{
					ret.Parse(sr);
					break;
				}
			}
			return ret;
		}

		protected override void AddRtfObject(RtfObject ro)
		{
			if (ro.Name == "\\fonttbl")
			{
				this.fontTable.SetObject(ro);
			}
			else if (ro.Name == "\\colortbl")
			{
				this.colorTable.SetObject(ro);
			}
			else if (this.document.Count > 0 || ro.Name.StartsWith("\\viewkind") || ro.Name.StartsWith("\\uc"))
			{
				this.document.Add(ro);
			}
			else
			{
				base.AddRtfObject(ro);
			}
		}

		public static RtfDocument Parse(TreeNode node)
		{
			RtfDocument ret;

			ret = new RtfDocument();
			ret.ParseNode(node);
			return ret;
		}

		public string ToRtf()
		{
			StringBuilder sb;

			sb = new StringBuilder();
			this.GenerateRtf(sb);
			return sb.ToString();
		}

		protected override void GenerateRtfChild(StringBuilder sb)
		{
			RtfObject prev;
			RtfObject ro;

			base.GenerateRtfChild(sb);
			this.fontTable.GenerateRtf(sb);
			this.colorTable.GenerateRtf(sb);
			if (this.document.Count < 1)
			{
				sb.Append(' ');
			}
			prev = null;
			foreach (object obj in this.document)
			{
				ro = obj as RtfObject;
				if (sb.Length > 0 && sb[sb.Length - 1] != '\n' && !ro.Name.StartsWith("\\") &&(prev == null || !prev.IsText))
				{
					sb.Append(' ');
				}
				ro.GenerateRtf(sb);
				prev = ro;
			}
		}

		public override string ToString()
		{
			StringBuilder ret;
			RtfContext rc;
			RtfObject ro;
			RtfFont rf;

			ret = new StringBuilder();
			rc = new RtfContext();
			foreach (object obj in this.document)
			{
				ro = obj as RtfObject;
				rc.Read(ro);
				if (ro.IsText)
				{
					rf = this.fontTable.Fonts[rc.Font];
					ret.Append(ro.GetText(CharSetEncoding.GetEncoding(rf.CharSet)));
				}
				else if (ro.Name == "\\par")
				{
					ret.Append("\r\n");
				}
			}
			return ret.ToString();
		}

		protected override void DisplayTreeViewChild(TreeView treeView, TreeNode node)
		{
			base.DisplayTreeViewChild(treeView, node);
			this.fontTable.DisplayTreeView(treeView, node);
			this.colorTable.DisplayTreeView(treeView, node);
			foreach (object obj in this.document)
			{
				(obj as RtfObject).DisplayTreeView(treeView, node);
			}
		}

		#region Font

		public Font Font
		{
			set
			{
				this.FontName = value.Name;
				this.FontSize = value.Size;
			}
		}

		public string FontName
		{
			get
			{
				string fn;
				RtfObject ro;
				string v;

				fn = "";
				foreach (object obj in this.document)
				{
					ro = obj as RtfObject;
					if (ro.IsFontNumber)
					{
						v = this.fontTable.Fonts[ro.Value].Name;
						if (fn != "" && fn != v) return "";
						fn = v;
					}
				}
				return fn;
			}

			set
			{
				ArrayList ros;
				RtfObject ro;
				RtfFont rf;

				ros = new ArrayList();
				foreach (object obj in this.document)
				{
					ro = obj as RtfObject;
					if (ro.IsFontNumber)
					{
						rf = new RtfFont(value);
						ro = new RtfObject(string.Format("\\f{0}", this.fontTable.GetIndex(rf)));
					}
					ros.Add(ro);
				}
				this.document = ros;
			}
		}

		public float FontSize
		{
			get
			{
				int ret;
				RtfObject ro;
				int v;

				ret = 0;
				foreach (object obj in this.document)
				{
					ro = obj as RtfObject;
					if (ro.Name.StartsWith("\\fs"))
					{
						v = ro.Value;
						if (ret > 0 && ret != v) return 0;
						ret = v;
					}
				}
				return ((float) ret) / 2;
			}

			set
			{
				int len;
				RtfObject ro;

				this.RemoveFontSize();
				len = this.document.Count;
				for (int i = 0; i < len; i++)
				{
					ro = this.document[i] as RtfObject;
					if (ro.IsText)
					{
						this.document.Insert(i, new RtfObject(string.Format("\\fs{0}",(int) (value * 2))));
						return;
					}
				}
			}
		}

		public void RemoveFontSize()
		{
			ArrayList ros;
			RtfObject ro;

			ros = new ArrayList();
			foreach (object obj in this.document)
			{
				ro = obj as RtfObject;
				if (ro.Name.StartsWith("\\fs")) continue;
				ros.Add(ro);
			}
			this.document = ros;
		}

		public Font CurrentFont
		{
			set
			{
				this.CurrentFontName = value.Name;
				this.CurrentFontSize = value.Size;
			}
		}

		public string CurrentFontName
		{
			get
			{
				return this.fontTable.Fonts[this.LastContext.Font].Name;
			}

			set
			{
				this.AddDocument(string.Format("\\f{0}", this.fontTable.GetIndex(new RtfFont(value))));
			}
		}

		public float CurrentFontSize
		{
			get
			{
				return ((float) this.LastContext.FontSize) / 2;
			}

			set
			{
				this.AddDocument(string.Format("\\fs{0}",(int) (value * 2)));
			}
		}

		#endregion

		#region Style

		public bool ExistsInDocument(string target)
		{
			foreach (object obj in this.document)
			{
				if ((obj as RtfObject).Exists(target)) return true;
			}
			return false;
		}

		public void RemoveFromDocument(string target)
		{
			ArrayList ros;
			RtfObject ro;

			ros = new ArrayList();
			foreach (object obj in this.document)
			{
				ro = obj as RtfObject;
				if (ro.Name == target) continue;
				ros.Add(ro);
				ro.Remove(target);
			}
			this.document = ros;
		}

		public bool IsBold
		{
			get
			{
				return this.ExistsInDocument("\\b");
			}

			set
			{
				this.RemoveFromDocument("\\b");
				this.RemoveFromDocument("\\b0");
				if (value) this.AddStyle("\\b");
			}
		}

		public bool IsItalic
		{
			get
			{
				return this.ExistsInDocument("\\i");
			}

			set
			{
				this.RemoveFromDocument("\\i");
				this.RemoveFromDocument("\\i0");
				if (value) this.AddStyle("\\i");
			}
		}

		public bool IsUnderline
		{
			get
			{
				return this.ExistsInDocument("\\ul");
			}

			set
			{
				this.RemoveFromDocument("\\ul");
				this.RemoveFromDocument("\\ulnone");
				if (value) this.AddStyle("\\ul");
			}
		}

		public void AddStyle(string style)
		{
			int len;
			RtfObject ro;

			len = this.document.Count;
			for (int i = 0; i < len; i++)
			{
				ro = this.document[i] as RtfObject;
				if (ro.Name.StartsWith("\\f") || ro.IsText)
				{
					this.document.Insert(i, new RtfObject(style));
					return;
				}
			}
		}

		#endregion

		#region Text Manipulation

		public void SetDocument()
		{
			if (this.name == "") this.name = "\\rtf1";
			if (this.rtfObjects.Count < 1)
			{
				this.AddRtfObject(new RtfObject("\\ansi"));
				this.AddRtfObject(new RtfObject("\\deff0"));
			}
			if (this.document.Count < 1)
			{
				// 無限ループになるので this.AddDocument() は使わない
				this.document.Add(new RtfObject("\\uc1"));
				this.document.Add(new RtfObject("\\pard"));
			}
		}

		public void AddDocument(string name)
		{
			this.SetDocument();
			this.document.Add(new RtfObject(name));
		}

		public void AppendText(string text)
		{
			StringBuilder sb;
			char prev;

			if (text.Length < 1) return;
			sb = new StringBuilder();
			prev = '\0';
			foreach (char ch in text)
			{
				if (ch == '\r' ||(prev != '\r' && ch == '\n'))
				{
					if (sb.Length > 0)
					{
						this.AddDocument(RtfObject.ConvertText(sb.ToString()));
						sb.Remove(0, sb.Length);
					}
					this.AppendLine();
				}
				else if (ch != '\n')
				{
					sb.Append(ch);
				}
				prev = ch;
			}
			if (sb.Length > 0)
			{
				this.AddDocument(RtfObject.ConvertText(sb.ToString()));
			}
		}

		public void AppendText(string text, Color color)
		{
			int c;

			if (text.Length < 1) return;
			c = this.colorTable.GetIndex(color);
			if (c != 0) this.AddDocument(string.Format("\\cf{0}", c));
			this.AppendText(text);
			if (c != 0) this.AddDocument("\\cf0");
		}

		public void AppendText(string text, FontStyle fontStyle)
		{
			if (text.Length < 1) return;
			if ((fontStyle & FontStyle.Bold) == FontStyle.Bold)
			{
				this.AddDocument("\\b");
			}
			if ((fontStyle & FontStyle.Italic) == FontStyle.Italic)
			{
				this.AddDocument("\\i");
			}
			if ((fontStyle & FontStyle.Underline) == FontStyle.Underline)
			{
				this.AddDocument("\\ul");
			}
			this.AppendText(text);
			if ((fontStyle & FontStyle.Underline) == FontStyle.Underline)
			{
				this.AddDocument("\\ulnone");
			}
			if ((fontStyle & FontStyle.Italic) == FontStyle.Italic)
			{
				this.AddDocument("\\i0");
			}
			if ((fontStyle & FontStyle.Bold) == FontStyle.Bold)
			{
				this.AddDocument("\\b0");
			}
		}

		public void AppendText(string text, Color color, FontStyle fontStyle)
		{
			int c;

			if (text.Length < 1) return;
			c = this.colorTable.GetIndex(color);
			if (c != 0) this.AddDocument(string.Format("\\cf{0}", c));
			this.AppendText(text, fontStyle);
			if (c != 0) this.AddDocument("\\cf0");
		}

		public void AppendLine()
		{
			this.AddDocument("\\par");
		}

		public void AppendLine(string text)
		{
			this.AppendText(text);
			this.AppendLine();
		}

		public void AppendLine(string text, Color color)
		{
			this.AppendText(text, color);
			this.AppendLine();
		}

		public void AppendLine(string text, FontStyle fontStyle)
		{
			this.AppendText(text, fontStyle);
			this.AppendLine();
		}

		public void AppendLine(string text, Color color, FontStyle fontStyle)
		{
			this.AppendText(text, color, fontStyle);
			this.AppendLine();
		}

		#endregion
	}
}
