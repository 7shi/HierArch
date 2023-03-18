using System;
using System.Windows.Forms;
using System.Xml;

namespace Girl.HierArch
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class HAFuncNode : HATreeNode
	{
		public const string ext = "hafnc";
		public string Source;
		public int SourceSelectionStart;
		public int SourceSelectionLength;

		public override void Init()
		{
			base.Init();
			this.Source = "";
			this.SourceSelectionStart = 0;
			this.SourceSelectionLength = 0;
		}

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAFuncNode()
		{
		}

		public HAFuncNode(string text)
		{
			this.Text = text;
		}

		public override string XmlName
		{
			get
			{
				return "HAFunc";
			}
		}

		public override HATreeNode NewNode
		{
			get
			{
				return new HAFuncNode();
			}
		}

		public string Color
		{
			get
			{
				switch (this.Type)
				{
					case HAType.FolderBlue: case HAType.FolderBule_Open: case HAType.TextBlue: return "blue";
					case HAType.FolderBrown: case HAType.FolderBrown_Open: case HAType.TextBrown: return "brown";
					case HAType.FolderGray: case HAType.FolderGray_Open: case HAType.TextGray: return "gray";
					case HAType.FolderGreen: case HAType.FolderGreen_Open: case HAType.TextGreen: return "green";
					case HAType.FolderRed: case HAType.FolderRed_Open: case HAType.TextRed: return "red";
				}
				return "";
			}
		}

		public override object Clone()
		{
			HAFuncNode ret = base.Clone() as HAFuncNode;
			ret.Source = this.Source;
			ret.SourceSelectionStart = this.SourceSelectionStart;
			ret.SourceSelectionLength = this.SourceSelectionLength;
			return ret;
		}

		#region Search

		public HAFuncNode Search(string text)
		{
			if (this.Text == text) return this;
			HAFuncNode ret = null;
			foreach (TreeNode n in this.Nodes)
			{
				ret =(n as HAFuncNode).Search(text);
				if (ret != null) break;
			}
			return ret;
		}

		public HAFuncNode Search(string text, HAType type)
		{
			if (this.Text == text && this.Type == type) return this;
			HAFuncNode ret = null;
			foreach (TreeNode n in this.Nodes)
			{
				ret =(n as HAFuncNode).Search(text, type);
				if (ret != null) break;
			}
			return ret;
		}

		#endregion

		#region XML

		public override void WriteXml(XmlTextWriter xw)
		{
			base.WriteXml(xw);
			xw.WriteStartElement("Source");
			xw.WriteAttributeString("SelectionStart", XmlConvert.ToString(this.SourceSelectionStart));
			xw.WriteAttributeString("SelectionLength", XmlConvert.ToString(this.SourceSelectionLength));
			xw.WriteString("\r\n" + this.Source);
			xw.WriteEndElement();
		}
		
		public static string StripHeadLine(string text)
		{
			if (!text.StartsWith("\r\n")) return text;
			return text.Substring(2, text.Length - 2);
		}

		public override void ReadXmlNode(XmlTextReader xr)
		{
			if (xr.NodeType == XmlNodeType.Element)
			{
				if (xr.Name == "Source")
				{
					this.SourceSelectionStart = XmlConvert.ToInt32(xr.GetAttribute("SelectionStart"));
					this.SourceSelectionLength = XmlConvert.ToInt32(xr.GetAttribute("SelectionLength"));
					if (!xr.IsEmptyElement && xr.Read()) this.Source = StripHeadLine(xr.ReadString());
				}
			}
		}

		public void ToHds(XmlTextWriter xw)
		{
			xw.WriteStartElement("node");
			xw.WriteAttributeString("title", this.Text);
			if (this.m_IsExpanded) xw.WriteAttributeString("open", "true");
			string c = this.Color;
			if (c != "") xw.WriteAttributeString("icon", c);
			xw.WriteStartElement("para");
			xw.WriteString("\n" + this.Source.Replace("\r\n", "\n"));
			xw.WriteEndElement();
			foreach (TreeNode n in this.Nodes)
			{
				(n as HAFuncNode).ToHds(xw);
			}
			xw.WriteEndElement();
		}

		public void FromHds(XmlTextReader xr)
		{
			this.Type = HAType.Text;
			if (xr.Name != "node" || xr.NodeType != XmlNodeType.Element) return;
			this.Text = xr.GetAttribute("title");
			this.m_IsExpanded =(xr.GetAttribute("open") == "true");
			string icon = xr.GetAttribute("icon");
			if (xr.IsEmptyElement) return;
			HAFuncNode n;
			while (xr.Read())
			{
				if (xr.Name == "node" && xr.NodeType == XmlNodeType.Element)
				{
					n = new HAFuncNode();
					this.Nodes.Add(n);
					n.FromHds(xr);
				}
				else if (xr.Name == "node" && xr.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				else if (xr.Name == "para" && xr.NodeType == XmlNodeType.Element && !xr.IsEmptyElement && xr.Read())
				{
					string text = xr.ReadString().Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
					this.Source = StripHeadLine(text);
				}
			}
			if (this.Nodes.Count > 0)
			{
				if (this.m_IsExpanded) Expand();
				this.m_Type =(HAType) Enum.Parse(typeof (HAType), "folder" + icon, true);
			}
			else
			{
				this.m_Type =(HAType) Enum.Parse(typeof (HAType), "text" + icon, true);
			}
			this.SetIcon();
		}

		#endregion
	}
}
