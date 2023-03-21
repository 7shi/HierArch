using Girl.Windows.Forms;
using System;
using System.Windows.Forms;
using System.Xml;

namespace Girl.HierArch
{
    /// <summary>
    /// ここにクラスの説明を書きます。
    /// </summary>
    public class HATreeNode : DnDTreeNode
    {
        public HAType m_Type;
        public bool m_IsExpanded;
        public bool m_IsSelected;
        public DateTime LastModified;

        public virtual void Init()
        {
            Type = HAType.Text;
            m_IsExpanded = false;
            m_IsSelected = false;
            LastModified = DateTime.Now;
            while (Nodes.Count > 0)
            {
                Nodes[0].Remove();
            }
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public HATreeNode()
        {
            Init();
        }

        public HATreeNode(string text)
        {
            Text = text;
            Init();
        }

        #region Properties

        public virtual string XmlName => "HATree";

        public virtual HATreeNode NewNode => new HATreeNode();

        public HAType Type
        {
            get => m_Type;

            set
            {
                m_Type = value;
                SetIcon();
            }
        }

        public bool IsObject => false; //(this.m_Type == HAType.Public || this.m_Type == HAType.Protected || this.m_Type == HAType.Private);

        public bool IsText
        {
            get
            {
                switch (m_Type)
                {
                    case HAType.Text: case HAType.TextBlue: case HAType.TextBrown: case HAType.TextGray: case HAType.TextGreen: case HAType.TextRed: return true;
                }
                return false;
            }
        }

        public bool IsFolder => IsRealFolder || m_Type == HAType.FolderGray || m_Type == HAType.FolderGray_Open;

        public bool IsRealFolder
        {
            get
            {
                switch (m_Type)
                {
                    case HAType.Folder: case HAType.Folder_Open: case HAType.FolderBlue: case HAType.FolderBule_Open: case HAType.FolderBrown: case HAType.FolderBrown_Open: case HAType.FolderGreen: case HAType.FolderGreen_Open: case HAType.FolderRed: case HAType.FolderRed_Open: return true;
                }
                return false;
            }
        }

        #endregion

        public override void SetIcon()
        {
            int t = (int)m_Type;
            if (IsFolder)
            {
                if (m_Type.ToString().EndsWith("_Open"))
                {
                    t--;
                }

                if (Nodes.Count > 0 && IsExpanded)
                {
                    t++;
                }
            }
            SelectedImageIndex = ImageIndex = t;
        }

        public override object Clone()
        {
            HATreeNode ret = (HATreeNode)base.Clone();
            ret.Type = Type;
            ret.m_IsExpanded = m_IsExpanded;
            ret.m_IsSelected = m_IsSelected;
            ret.LastModified = LastModified;
            return ret;
        }

        public void StoreState()
        {
            m_IsExpanded = IsExpanded;
            m_IsSelected = IsSelected;
            foreach (TreeNode n in Nodes)
            {
                if (n is HATreeNode)
                {
                    ((HATreeNode)n).StoreState();
                }
            }
        }

        public void ApplyState()
        {
            if (m_IsExpanded)
            {
                Expand();
            }

            if (m_IsSelected)
            {
                TreeView.SelectedNode = this;
            }

            NodeFont = null;
            foreach (TreeNode n in Nodes)
            {
                if (n is HATreeNode)
                {
                    ((HATreeNode)n).ApplyState();
                }
            }
        }

        #region XML

        public override void ToXml(XmlTextWriter xw)
        {
            XmlTextWriter xw2 = xw;
            if (xw2 != null)
            {
                xw2.WriteStartElement(XmlName);
                WriteXml(xw2);
                DnDTreeNode dn;
                foreach (TreeNode n in Nodes)
                {
                    dn = (DnDTreeNode)n;
                    dn?.ToXml(xw2);
                }
                xw2.WriteEndElement();
                if (xw2 != xw)
                {
                    xw2.WriteEndDocument();
                    xw2.Close();
                }
            }
        }

        public virtual void WriteXml(XmlTextWriter xw)
        {
            xw.WriteAttributeString("Type", Type.ToString());
            xw.WriteAttributeString("Text", Text);
            xw.WriteAttributeString("IsExpanded", XmlConvert.ToString(m_IsExpanded));
            xw.WriteAttributeString("IsSelected", XmlConvert.ToString(m_IsSelected));
            xw.WriteAttributeString("AllowDrag", XmlConvert.ToString(AllowDrag));
            xw.WriteAttributeString("LastModified", LastModified.ToString());
        }

        public override void FromXml(XmlTextReader xr)
        {
            if (xr.Name != XmlName || xr.NodeType != XmlNodeType.Element)
            {
                return;
            }

            _ = xr.GetAttribute("Link");
            XmlTextReader xr2 = xr;
            ReadXml(xr2);
            if (xr2.IsEmptyElement)
            {
                return;
            }

            HATreeNode n;
            while (xr2.Read())
            {
                if (xr2.Name == XmlName && xr2.NodeType == XmlNodeType.Element)
                {
                    n = NewNode;
                    _ = Nodes.Add(n);
                    n.FromXml(xr2);
                }
                else if (xr2.Name == XmlName && xr2.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                else
                {
                    ReadXmlNode(xr2);
                }
            }
            if (m_IsExpanded)
            {
                Expand();
            }

            if (xr2 != xr)
            {
                xr2.Close();
            }
        }

        public virtual void ReadXml(XmlTextReader xr)
        {
            Type = (HAType)HAType.Parse(typeof(HAType), xr.GetAttribute("Type"));
            Text = xr.GetAttribute("Text");
            m_IsExpanded = XmlConvert.ToBoolean(xr.GetAttribute("IsExpanded"));
            m_IsSelected = XmlConvert.ToBoolean(xr.GetAttribute("IsSelected"));
            AllowDrag = XmlConvert.ToBoolean(xr.GetAttribute("AllowDrag"));
            string lastModified = xr.GetAttribute("LastModified");
            LastModified = (lastModified != null && lastModified.Length > 0) ? DateTime.Parse(lastModified) :
            DateTime.Now;
        }

        public virtual void ReadXmlNode(XmlTextReader xr)
        {
        }

        #endregion

        protected DialogResult Ask(string question, string caption)
        {
            Cursor cur = Cursor.Current;
            Cursor.Current = Cursors.Default;
            DialogResult ret = MessageBox.Show(question, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            Cursor.Current = cur;
            return ret;
        }

        protected virtual void OnRefreshNode(EventArgs e)
        {
            HATree tv = TreeView as HATree;
            tv?.OnRefreshNode(this, e);
        }
    }
}
