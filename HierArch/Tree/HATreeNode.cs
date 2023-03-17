// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Girl.Windows.Forms;

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
			this.Type = HAType.Public;
			this.m_IsExpanded = false;
			this.m_IsSelected = false;
			this.LastModified = DateTime.Now;
			while (this.Nodes.Count > 0) this.Nodes[0].Remove();
		}

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HATreeNode()
		{
			this.Init();
		}

		public HATreeNode(string text)
		{
			this.Text = text;
			this.Init();
		}

		#region Properties

		public virtual string XmlName
		{
			get
			{
				return "HATree";
			}
		}

		public virtual HATreeNode NewNode
		{
			get
			{
				return new HATreeNode();
			}
		}

		public HAType Type
		{
			get
			{
				return this.m_Type;
			}

			set
			{
				this.m_Type = value;
				this.SetIcon();
			}
		}

		public bool IsObject
		{
			get
			{
				return (this.m_Type == HAType.Public || this.m_Type == HAType.Protected || this.m_Type == HAType.Private);
			}
		}

		public bool IsText
		{
			get
			{
				switch (this.m_Type)
				{
					case HAType.Text: case HAType.TextBlue: case HAType.TextBrown: case HAType.TextGray: case HAType.TextGreen: case HAType.TextRed: return true;
				}
				return false;
			}
		}

		public bool IsFolder
		{
			get
			{
				return (this.IsRealFolder || this.m_Type == HAType.FolderGray || this.m_Type == HAType.FolderGray_Open);
			}
		}

		public bool IsRealFolder
		{
			get
			{
				switch (this.m_Type)
				{
					case HAType.Folder: case HAType.Folder_Open: case HAType.FolderBlue: case HAType.FolderBule_Open: case HAType.FolderBrown: case HAType.FolderBrown_Open: case HAType.FolderGreen: case HAType.FolderGreen_Open: case HAType.FolderRed: case HAType.FolderRed_Open: return true;
				}
				return false;
			}
		}

		#endregion

		public override void SetIcon()
		{
			int t =(int) this.m_Type;
			if (this.IsFolder)
			{
				if (m_Type.ToString().EndsWith("_Open")) t--;
				if (this.Nodes.Count > 0 && this.IsExpanded) t++;
			}
			this.SelectedImageIndex = this.ImageIndex = t;
		}

		public override object Clone()
		{
			HATreeNode ret =(HATreeNode) base.Clone();
			ret.Type = this.Type;
			ret.m_IsExpanded = this.m_IsExpanded;
			ret.m_IsSelected = this.m_IsSelected;
			ret.LastModified = this.LastModified;
			return ret;
		}

		public void StoreState()
		{
			this.m_IsExpanded = this.IsExpanded;
			this.m_IsSelected = this.IsSelected;
			foreach (TreeNode n in this.Nodes)
			{
				if (n is HATreeNode)((HATreeNode) n).StoreState();
			}
		}

		public void ApplyState()
		{
			if (this.m_IsExpanded) this.Expand();
			if (this.m_IsSelected) this.TreeView.SelectedNode = this;
			this.NodeFont = null;
			foreach (TreeNode n in this.Nodes)
			{
				if (n is HATreeNode)((HATreeNode) n).ApplyState();
			}
		}

		#region XML

		public override void ToXml(XmlTextWriter xw)
		{
			XmlTextWriter xw2 = xw;
			if (xw2 != null)
			{
				xw2.WriteStartElement(this.XmlName);
				this.WriteXml(xw2);
				DnDTreeNode dn;
				foreach (TreeNode n in Nodes)
				{
					dn =(DnDTreeNode) n;
					if (dn != null) dn.ToXml(xw2);
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
			xw.WriteAttributeString("Text", this.Text);
			xw.WriteAttributeString("IsExpanded", XmlConvert.ToString(this.m_IsExpanded));
			xw.WriteAttributeString("IsSelected", XmlConvert.ToString(this.m_IsSelected));
			xw.WriteAttributeString("AllowDrag", XmlConvert.ToString(this.AllowDrag));
			xw.WriteAttributeString("LastModified", this.LastModified.ToString());
		}

		public override void FromXml(XmlTextReader xr)
		{
			if (xr.Name != this.XmlName || xr.NodeType != XmlNodeType.Element) return;
			string link = xr.GetAttribute("Link");
			XmlTextReader xr2 = xr;
			this.ReadXml(xr2);
			if (xr2.IsEmptyElement) return;
			HATreeNode n;
			while (xr2.Read())
			{
				if (xr2.Name == this.XmlName && xr2.NodeType == XmlNodeType.Element)
				{
					n = this.NewNode;
					Nodes.Add(n);
					n.FromXml(xr2);
				}
				else if (xr2.Name == this.XmlName && xr2.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				else
				{
					ReadXmlNode(xr2);
				}
			}
			if (m_IsExpanded) Expand();
			if (xr2 != xr) xr2.Close();
		}

		public virtual void ReadXml(XmlTextReader xr)
		{
			this.Type =(HAType) HAType.Parse(typeof (HAType), xr.GetAttribute("Type"));
			this.Text = xr.GetAttribute("Text");
			this.m_IsExpanded = XmlConvert.ToBoolean(xr.GetAttribute("IsExpanded"));
			this.m_IsSelected = XmlConvert.ToBoolean(xr.GetAttribute("IsSelected"));
			this.AllowDrag = XmlConvert.ToBoolean(xr.GetAttribute("AllowDrag"));
			string lastModified = xr.GetAttribute("LastModified");
			this.LastModified =(lastModified != null && lastModified.Length > 0) ? DateTime.Parse(lastModified):
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
			HATree tv = this.TreeView as HATree;
			if (tv != null) tv.OnRefreshNode(this, e);
		}
	}
}
