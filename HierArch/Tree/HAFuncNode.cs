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
            Source = "";
            SourceSelectionStart = 0;
            SourceSelectionLength = 0;
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public HAFuncNode()
        {
        }

        public HAFuncNode(string text)
        {
            Text = text;
        }

        public override string XmlName => "HAFunc";

        public override HATreeNode NewNode => new HAFuncNode();

        public string Color
        {
            get
            {
                switch (Type)
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
            ret.Source = Source;
            ret.SourceSelectionStart = SourceSelectionStart;
            ret.SourceSelectionLength = SourceSelectionLength;
            return ret;
        }

        #region Search

        public HAFuncNode Search(string text)
        {
            if (Text == text)
            {
                return this;
            }

            HAFuncNode ret = null;
            foreach (TreeNode n in Nodes)
            {
                ret = (n as HAFuncNode).Search(text);
                if (ret != null)
                {
                    break;
                }
            }
            return ret;
        }

        public HAFuncNode Search(string text, HAType type)
        {
            if (Text == text && Type == type)
            {
                return this;
            }

            HAFuncNode ret = null;
            foreach (TreeNode n in Nodes)
            {
                ret = (n as HAFuncNode).Search(text, type);
                if (ret != null)
                {
                    break;
                }
            }
            return ret;
        }

        #endregion

        public string GetText()
        {
            return Source.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        #region XML

        public override void WriteXml(XmlTextWriter xw)
        {
            base.WriteXml(xw);
            xw.WriteStartElement("Source");
            xw.WriteAttributeString("SelectionStart", XmlConvert.ToString(SourceSelectionStart));
            xw.WriteAttributeString("SelectionLength", XmlConvert.ToString(SourceSelectionLength));
            xw.WriteString("\r\n" + GetText().Replace("\n", "\r\n"));
            xw.WriteEndElement();
        }

        public static string StripHeadLine(string text)
        {
            return !text.StartsWith("\r\n") ? text : text.Substring(2, text.Length - 2);
        }

        public override void ReadXmlNode(XmlTextReader xr)
        {
            if (xr.NodeType == XmlNodeType.Element)
            {
                if (xr.Name == "Source")
                {
                    SourceSelectionStart = XmlConvert.ToInt32(xr.GetAttribute("SelectionStart"));
                    SourceSelectionLength = XmlConvert.ToInt32(xr.GetAttribute("SelectionLength"));
                    if (!xr.IsEmptyElement && xr.Read())
                    {
                        Source = StripHeadLine(xr.ReadString());
                    }
                }
            }
        }

        public void ToHds(XmlTextWriter xw)
        {
            xw.WriteStartElement("node");
            xw.WriteAttributeString("title", Text);
            if (m_IsExpanded)
            {
                xw.WriteAttributeString("open", "true");
            }

            string c = Color;
            if (c != "")
            {
                xw.WriteAttributeString("icon", c);
            }

            xw.WriteStartElement("para");
            xw.WriteString("\n" + GetText());
            xw.WriteEndElement();
            foreach (TreeNode n in Nodes)
            {
                (n as HAFuncNode).ToHds(xw);
            }
            xw.WriteEndElement();
        }

        public void FromHds(XmlTextReader xr)
        {
            Type = HAType.Text;
            if (xr.Name != "node" || xr.NodeType != XmlNodeType.Element)
            {
                return;
            }

            Text = xr.GetAttribute("title");
            m_IsExpanded = xr.GetAttribute("open") == "true";
            string icon = xr.GetAttribute("icon");
            if (xr.IsEmptyElement)
            {
                return;
            }

            HAFuncNode n;
            while (xr.Read())
            {
                if (xr.Name == "node" && xr.NodeType == XmlNodeType.Element)
                {
                    n = new HAFuncNode();
                    _ = Nodes.Add(n);
                    n.FromHds(xr);
                }
                else if (xr.Name == "node" && xr.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                else if (xr.Name == "para" && xr.NodeType == XmlNodeType.Element && !xr.IsEmptyElement && xr.Read())
                {
                    string text = xr.ReadString().Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
                    Source = StripHeadLine(text);
                }
            }
            if (Nodes.Count > 0)
            {
                if (m_IsExpanded)
                {
                    Expand();
                }

                m_Type = (HAType)Enum.Parse(typeof(HAType), "folder" + icon, true);
            }
            else
            {
                m_Type = (HAType)Enum.Parse(typeof(HAType), "text" + icon, true);
            }
            SetIcon();
        }

        #endregion
    }
}
