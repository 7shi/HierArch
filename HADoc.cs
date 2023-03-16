using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Girl.Windows.Forms;

namespace Girl.HierarchyArchitect
{
	/// <summary>
	/// HADoc の概要の説明です。
	/// </summary>
	public class HADoc : Document
	{
		public HAClass ClassTreeView = null;

		public HADoc()
		{
			// 
			// TODO: コンストラクタ ロジックをここに追加してください。
			//
		}

		public override bool Open()
		{
			FileStream fs;
			try
			{
				fs = new FileStream(FullName, FileMode.Open);
			}
			catch
			{
				return false;
			}
			XmlTextReader xr = new XmlTextReader(fs);
			HAClassNode n;
			while (xr.Read())
			{
				if (xr.Name == "HAClass" && xr.NodeType == XmlNodeType.Element)
				{
					n = new HAClassNode();
					this.ClassTreeView.Nodes.Add(n);
					n.FromXml(xr);
				}
				else if (xr.Name == "HAProject" && xr.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
			}
			xr.Close();
			fs.Close();
			this.ClassTreeView.ApplyState();
			Changed = false;
			return true;
		}

		public override bool Save()
		{
			this.ClassTreeView.StoreData();
			FileStream fs;
			try
			{
				fs = new FileStream(FullName, FileMode.Create);
			}
			catch
			{
				return false;
			}
			// UTF-8 決め打ち
			XmlTextWriter xw = new XmlTextWriter(fs, null);
			xw.Formatting = Formatting.Indented;
			xw.WriteStartElement("HAProject");
			foreach (TreeNode n in ClassTreeView.Nodes)
			{
				if (n is HAClassNode) ((HAClassNode)n).ToXml(xw);
			}
			xw.WriteEndElement();
			xw.Close();
			fs.Close();
			Changed = false;
			return true;
		}
	}
}
