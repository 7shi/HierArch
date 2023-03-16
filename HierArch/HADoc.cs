// このファイルは ..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
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
	public class HADoc : Document
	{
		public HAClass ClassTreeView;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HADoc()
		{
			this.ClassTreeView = null;
		}

		public string ShortName
		{
			get
			{
				string ret = this.Name;
				int p = ret.LastIndexOf('.');
				if (p < 0) return ret;
				return ret.Substring(0, p);
			}
		}

		public override bool Open()
		{
			FileStream fs;
			XmlTextReader xr;
			HAClassNode n;

			try
			{
				fs = new FileStream(FullName, FileMode.Open);
			}
			catch
			{
				return false;
			}
			xr = new XmlTextReader(fs);
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
				else if (xr.Name == "hds" && xr.NodeType == XmlNodeType.Element)
				{
					n = new HAClassNode();
					n.Text = this.ShortName;
					this.ClassTreeView.InitNode(n);
					n.Body.Nodes.Clear();
					this.ClassTreeView.Nodes.Add(n);
					n.FromHds(xr);
				}
				else if (xr.Name == "hds" && xr.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
			}
			xr.Close();
			fs.Close();
			this.ClassTreeView.ApplyState();
			if (this.ClassTreeView.SelectedNode == null && this.ClassTreeView.Nodes.Count > 0)
			{
				this.ClassTreeView.SelectedNode = this.ClassTreeView.Nodes[0];
			}
			Changed = false;
			return true;
		}

		public override bool Save()
		{
			this.ClassTreeView.StoreData();
			bool ret = false;
			string lfn = this.FullName.ToLower();
			if (lfn.EndsWith(".haprj"))
			{
				ret = SaveHAPrj();
			}
			else if (lfn.EndsWith(".hds"))
			{
				ret = SaveHds();
			}
			else
			{
				MessageBox.Show("保存できないファイルの種類です。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			if (ret) Changed = false;
			return ret;
		}

		private bool SaveHAPrj()
		{
			FileStream fs;

			try
			{
				fs = new FileStream(this.FullName, FileMode.Create);
			}
			catch
			{
				return false;
			}
			XmlTextWriter xw = new XmlTextWriter(fs, null);
			xw.Formatting = Formatting.Indented;
			xw.WriteStartDocument();
			xw.WriteStartElement("HAProject");
			xw.WriteAttributeString("version" , Application.ProductVersion);
			foreach (TreeNode n in this.ClassTreeView.Nodes)
			{
				(n as HAClassNode).ToXml(xw);
			}
			xw.WriteEndElement();
			xw.WriteEndDocument();
			xw.Flush();
			xw.Close();
			fs.Close();
			return true;
		}

		private bool SaveHds()
		{
			HAClassNode n = this.ClassTreeView.SelectedNode as HAClassNode;
			if (n == null)
			{
				MessageBox.Show("クラスが選択されていません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			
			if (MessageBox.Show("HDS 形式では現在開かれているクラスだけが保存されます。", "確認",
				MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
			{
				return false;
			}
			
			FileStream fs;
			try
			{
				fs = new FileStream(this.FullName, FileMode.Create);
			}
			catch
			{
				return false;
			}
			StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
			sw.NewLine = "\n";
			XmlTextWriter xw = new XmlTextWriter(sw);
			xw.Formatting = Formatting.Indented;
			xw.WriteStartDocument();
			xw.WriteStartElement("hds");
			xw.WriteAttributeString("version" , "0.3.5");
			foreach (TreeNode nn in n.Body.Nodes)
			{
				(nn as HAFuncNode).ToHds(xw);
			}
			xw.WriteEndElement();
			xw.WriteEndDocument();
			xw.Flush();
			xw.Close();
			fs.Close();
			return true;
		}

		public static string BuildDateTime
		{
			get
			{
				return "2003/01/06 22:33:07";
			}
		}
	}
}
