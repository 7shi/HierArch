// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

// ここにソースコードの注釈を書きます。

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Girl.Coding;
using Girl.Windows.Forms;

namespace Girl.HierArch
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class HAFuncNode : HATreeNode
	{
		public ArrayList Args;
		public ArrayList Objects;
		public string Comment;
		public string Source;
		public int CommentSelectionStart;
		public int CommentSelectionLength;
		public int SourceSelectionStart;
		public int SourceSelectionLength;
		public HAFuncNode PropertyPair;

		private void Init()
		{
			this.Args    = new ArrayList();
			this.Objects = new ArrayList();
			this.Comment = "";
			this.Source  = "";
			this.CommentSelectionStart  = 0;
			this.CommentSelectionLength = 0;
			this.SourceSelectionStart   = 0;
			this.SourceSelectionLength  = 0;
			this.PropertyPair = null;
		}

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAFuncNode()
		{
			this.Init();
		}

		public HAFuncNode(string text)
		{
			this.Init();
			
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
					case HAType.FolderBlue:
					case HAType.FolderBule_Open:
					case HAType.TextBlue:
						return "blue";
					case HAType.FolderBrown:
					case HAType.FolderBrown_Open:
					case HAType.TextBrown:
						return "brown";
					case HAType.FolderGray:
					case HAType.FolderGray_Open:
					case HAType.TextGray:
						return "gray";
					case HAType.FolderGreen:
					case HAType.FolderGreen_Open:
					case HAType.TextGreen:
						return "green";
					case HAType.FolderRed:
					case HAType.FolderRed_Open:
					case HAType.TextRed:
						return "red";
				}
				return "";
			}
		}

		public override object Clone()
		{
			HAFuncNode ret = base.Clone() as HAFuncNode;
			ret.Args    = this.Args.Clone() as ArrayList;
			ret.Objects = this.Objects.Clone() as ArrayList;
			ret.Comment = this.Comment;
			ret.Source  = this.Source;
			ret.CommentSelectionStart  = this.CommentSelectionStart;
			ret.CommentSelectionLength = this.CommentSelectionLength;
			ret.SourceSelectionStart   = this.SourceSelectionStart;
			ret.SourceSelectionLength  = this.SourceSelectionLength;
			return ret;
		}

		#region Search

		public HAFuncNode Search(string text)
		{
			if (this.Text == text) return this;
			
			HAFuncNode ret = null;
			foreach (TreeNode n in this.Nodes)
			{
				ret = (n as HAFuncNode).Search(text);
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
				ret = (n as HAFuncNode).Search(text, type);
				if (ret != null) break;
			}
			return ret;
		}

		public HAFuncNode SearchProperty(string text)
		{
			if (this.Type == HAType.Comment) return null;
			if (this.IsObject && this.PropertyPair == null)
			{
				ObjectParser op = new ObjectParser(this.Text);
				if (op.Name == text) return this;
			}
			
			HAFuncNode ret = null;
			foreach (TreeNode n in this.Nodes)
			{
				ret = (n as HAFuncNode).SearchProperty(text);
				if (ret != null) break;
			}
			return ret;
		}

		#endregion

		#region Property Pair

		public void ResetPropertyPair()
		{
			this.PropertyPair = null;
			foreach (TreeNode n in this.Nodes)
			{
				(n as HAFuncNode).ResetPropertyPair();
			}
		}

		public void SearchPropertyPair(HAFuncNode body)
		{
			if (this.Type == HAType.Comment) return;
			
			if (this.IsObject)
			{
				ObjectParser op = new ObjectParser(this.Text);
				if (op.IsProperty)
				{
					HAFuncNode n = body.SearchProperty(op.PropertyPair);
					if (n != null) this.PropertyPair = n.PropertyPair = n;
				}
			}
			
			foreach (TreeNode n in this.Nodes)
			{
				(n as HAFuncNode).SearchPropertyPair(body);
			}
		}

		#endregion

		#region XML

		public override void WriteXml(XmlTextWriter xw)
		{
			base.WriteXml(xw);
			
			xw.WriteStartElement("Arguments");
			foreach (Object obj in this.Args)
			{
				if (obj is HAObjectNode) (obj as HAObjectNode).ToXml(xw);
			}
			xw.WriteEndElement();
			
			foreach (Object obj in this.Objects)
			{
				if (obj is HAObjectNode) (obj as HAObjectNode).ToXml(xw);
			}
			
			xw.WriteStartElement("Comment");
			xw.WriteAttributeString("SelectionStart" , XmlConvert.ToString(this.CommentSelectionStart));
			xw.WriteAttributeString("SelectionLength", XmlConvert.ToString(this.CommentSelectionLength));
			xw.WriteString(this.Comment);
			xw.WriteEndElement();
			
			xw.WriteStartElement("Source");
			xw.WriteAttributeString("SelectionStart" , XmlConvert.ToString(this.SourceSelectionStart));
			xw.WriteAttributeString("SelectionLength", XmlConvert.ToString(this.SourceSelectionLength));
			xw.WriteString(this.Source);
			xw.WriteEndElement();
		}

		public override void ReadXmlNode(XmlTextReader xr)
		{
			if (xr.Name == "Arguments" && xr.NodeType == XmlNodeType.Element && !xr.IsEmptyElement)
			{
				while (xr.Read())
				{
					if (xr.Name == "Arguments" && xr.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					else if (xr.Name == "HAObject" && xr.NodeType == XmlNodeType.Element)
					{
						HAObjectNode n = new HAObjectNode();
						this.Args.Add(n);
						n.FromXml(xr);
					}
				}
			}
			else if (xr.Name == "HAObject" && xr.NodeType == XmlNodeType.Element)
			{
				HAObjectNode n = new HAObjectNode();
				this.Objects.Add(n);
				n.FromXml(xr);
			}
			else if (xr.Name == "Comment" && xr.NodeType == XmlNodeType.Element)
			{
				this.CommentSelectionStart  = XmlConvert.ToInt32(xr.GetAttribute("SelectionStart"));
				this.CommentSelectionLength = XmlConvert.ToInt32(xr.GetAttribute("SelectionLength"));
				if (!xr.IsEmptyElement && xr.Read()) this.Comment = xr.ReadString();
			}
			else if (xr.Name == "Source" && xr.NodeType == XmlNodeType.Element)
			{
				this.SourceSelectionStart  = XmlConvert.ToInt32(xr.GetAttribute("SelectionStart"));
				this.SourceSelectionLength = XmlConvert.ToInt32(xr.GetAttribute("SelectionLength"));
				if (!xr.IsEmptyElement && xr.Read()) this.Source = xr.ReadString();
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
			this.m_IsExpanded = (xr.GetAttribute("open") == "true");
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
				else if (xr.Name == "para" && xr.NodeType == XmlNodeType.Element
					&& !xr.IsEmptyElement && xr.Read())
				{
					string text = xr.ReadString();
					if (text.IndexOf("\r\n") < 0)
					{
						if (text.IndexOf("\n") >= 0)
						{
							text = text.Replace("\n", "\r\n");
						}
						else
						{
							text = text.Replace("\r", "\r\n");
						}
					}
					if (!text.StartsWith("\r\n"))
					{
						this.Source = text;
					}
					else
					{
						this.Source = text.Substring(2, text.Length - 2);
					}
				}
			}
			
			if (this.Nodes.Count > 0)
			{
				if (this.m_IsExpanded) Expand();
				this.m_Type = (HAType)Enum.Parse(typeof(HAType), "folder" + icon, true);
			}
			else
			{
				this.m_Type = (HAType)Enum.Parse(typeof(HAType), "text" + icon, true);
			}
			this.SetIcon();
		}

		#endregion

		#region Generation

		public void GenerateClass(CodeWriter cw)
		{
			HAType t = this.Type;
			if (t == HAType.Comment)
			{
				return;
			}
			else if (this.IsObject)
			{
				this.GenerateFunc(cw);
			}
			else if (t.ToString().StartsWith("Folder"))
			{
				cw.WriteBlankLine();
				cw.WriteCode("#region " + this.Text);
			}
			
			foreach (TreeNode n in this.Nodes)
			{
				(n as HAFuncNode).GenerateClass(cw);
			}
			
			if (t.ToString().StartsWith("Folder"))
			{
				cw.WriteBlankLine();
				cw.WriteCode("#endregion");
			}
		}

		private void GenerateFunc(CodeWriter cw)
		{
			ObjectParser op = new ObjectParser(this.Text, this.Type);
			if (op.IsProperty && this.PropertyPair == this) return;
			
			cw.WriteBlankLine();
			if (this.Comment != "") cw.WriteCodes("/// ", this.Comment);
			
			if (op.IsProperty)
			{
				cw.WriteStartBlock(op.PropertyDeclaration);
				this.GenerateProperty(cw);
				cw.WriteEndBlock();
			}
			else
			{
				this.GenerateFunction(cw, op);
			}
		}

		private void GenerateProperty(CodeWriter cw)
		{
			cw.WriteStartBlock(this.Text.Substring(0, 3));
			this.GenerateFuncCode(cw);
			cw.WriteEndBlock();
			if (this.PropertyPair == null || this.PropertyPair == this) return;
			
			cw.WriteBlankLine();
			this.PropertyPair.GenerateProperty(cw);
		}

		private void GenerateFunction(CodeWriter cw, ObjectParser op)
		{
			string code = op.FunctionDeclaration + "(";
			StringBuilder sb = new StringBuilder();
			foreach (Object obj in this.Args)
			{
				(obj as HAObjectNode).Generate(cw, sb);
			}
			code += sb.ToString() + ")";
			cw.WriteStartBlock(cw.ReplaceKeywords(code));
			
			this.GenerateFuncCode(cw);
			
			cw.WriteEndBlock();
		}

		private void GenerateFuncCode(CodeWriter cw)
		{
			cw.SetStart();
			foreach (Object obj in this.Objects)
			{
				(obj as HAObjectNode).Generate(cw);
			}
			if (this.Source != "")
			{
				cw.WriteBlankLine();
				cw.WriteCodes(cw.ReplaceKeywords(this.Source));
			}
		}

		public void GenerateFolder(string path)
		{
			if (this.Type == HAType.Comment)
			{
				return;
			}
			else if (this.IsText)
			{
				string target = path;
				if (!target.EndsWith("\\")) target += "\\";
				target += new ObjectParser(this.Text).Name;
				this.GenerateFile(target);
			}
			
			foreach (TreeNode n in this.Nodes)
			{
				(n as HAFuncNode).GenerateFolder(path);
			}
		}

		public void GenerateFile(string target)
		{
			FileStream fs;
			StreamWriter sw;

			try
			{
				fs = new FileStream(target, FileMode.Create);
			}
			catch
			{
				return;
			}
			
			sw = new StreamWriter(fs, Encoding.Default);
			sw.Write(this.Source);
			sw.Close();
			fs.Close();
		}

		public void GenerateText(StreamWriter sw, string chapter, bool concat)
		{
			sw.WriteLine();
			sw.WriteLine();
			sw.WriteLine(string.Format("  {0} {1}", chapter, this.Text));
			sw.WriteLine();
			if (concat)
			{
				StringReader sr = new StringReader(this.Source);
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					if (line == "")
					{
						sw.WriteLine();
					}
					else
					{
						sw.Write(line);
					}
				}
				sr.Close();
				sw.WriteLine();
			}
			else
			{
				sw.WriteLine(this.Source);
			}
			
			int i = 1;
			foreach (TreeNode n in this.Nodes)
			{
				(n as HAFuncNode).GenerateText(sw, chapter + "." + i.ToString(), concat);
				i++;
			}
		}

		#endregion
	}
}
