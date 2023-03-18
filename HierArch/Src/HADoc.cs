// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Girl.Windows.Forms;

namespace Girl.HierArch
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class HADoc : Document
	{
		public HAClass ClassTreeView;
		public HAViewInfo ViewInfo;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HADoc()
		{
			this.ClassTreeView = null;
			this.ViewInfo = new HAViewInfo();
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
		
		/// <summary>
		/// サロゲートペアを 3 バイト × 2 で符号化した UTF-8 の変種から変換 
		/// </summary>
		public static String FromCESU8(byte[] bytes)
		{
			var s = new StringBuilder();
			int len = bytes.Length;
			if (bytes[0] == 0xef && bytes[1] == 0xbb && bytes[2] == 0xbf) { // UTF-8 BOM
				return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
			}
			for (int i = 0; i < len; i++)
			{
				int b1 = (int)bytes[i];
				if (b1 < 0x80)
				{
					s.Append((char)b1);
				}
				else if (b1 < 0xc0)
				{
					s.Append("�");
				}
				else if (b1 < 0xe0)
				{
					int b2 = (int)bytes[++i];
					if (b2 < 0x80 || b2 >= 0xc0)
					{
						s.Append("��");
						continue;
					}
					// C0-DF | 80-BF
					int ch = (b1 & 0x1f) << 6 | (b2 & 0x3f);
					if (ch < 0x80)
					{
						s.Append("��");
						continue;
					}
					s.Append((char)ch);
				}
				else if (b1 < 0xf0)
				{
					int b2 = (int)bytes[++i];
					if (b2 < 0x80 || b2 >= 0xc0)
					{
						s.Append("��");
						continue;
					}
					int b3 = (int)bytes[++i];
					if (b3 < 0x80 || b3 >= 0xc0)
					{
						s.Append("���");
						continue;
					}
					// E0-EF | 80-BF | 80-BF
					int ch = (b1 & 0xf) << 12 | (b2 & 0x3f) << 6 | (b3 & 0x3f);
					if (ch < 0x800)
					{
						s.Append("���");
						continue;
					}
					s.Append((char)ch);
				}
				else if (b1 < 0xf5)
				{
					int b2 = (int)bytes[++i];
					if (b2 < 0x80 || b2 >= 0xc0)
					{
						s.Append("��");
						continue;
					}
					int b3 = (int)bytes[++i];
					if (b3 < 0x80 || b3 >= 0xc0)
					{
						s.Append("���");
						continue;
					}
					int b4 = (int)bytes[++i];
					if (b3 < 0x80 || b3 >= 0xc0)
					{
						s.Append("����");
						continue;
					}
					// F0-F4 | 80-BF | 80-BF | 80-BF
					int ch = (b1 & 7) << 18 | (b2 & 0x3f) << 12 | (b3 & 0x3f) << 6 | (b4 & 0x3f);
					if (ch < 0x10000 || ch >= 0x110000)
					{
						s.Append("����");
						continue;
					}
					s.Append((char)(0xd800 + ((ch - 0x10000) >> 10)));
					s.Append((char)(0xdc00 + (ch & 0x3ff)));
				}
				else
				{
					s.Append("�");
				}
			}
			return s.ToString();
		}

		public override bool Open()
		{
			XmlTextReader xr;
			HAClassNode n;

			if (!File.Exists(this.FullName)) return false;
			try
			{
				var ext = Path.GetExtension(this.FullName).ToLower();
				if (ext == ".hds")
				{
					var bytes = File.ReadAllBytes(this.FullName);
					var sr = new StringReader(FromCESU8(bytes));
					xr = new XmlTextReader(sr);
				}
				else
				{
					xr = new XmlTextReader(this.FullName);
				}
			}
			catch
			{
				return false;
			}
			while (xr.Read())
			{
				if (xr.Name == "HAViewInfo" && xr.NodeType == XmlNodeType.Element)
				{
					this.ViewInfo = new XmlSerializer(typeof (HAViewInfo)).Deserialize(xr) as HAViewInfo;
				}
				else if (xr.Name == "HAClass" && xr.NodeType == XmlNodeType.Element)
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
					this.ViewInfo.InitHds();
				}
				else if (xr.Name == "hds" && xr.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
			}
			xr.Close();
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
			if (lfn.EndsWith(".hadoc"))
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
			XmlTextWriter xw;

			try
			{
				xw = new XmlTextWriter(this.FullName, new UTF8Encoding(false));
			}
			catch
			{
				return false;
			}
			xw.Formatting = Formatting.Indented;
			xw.Indentation = 0;
			xw.WriteStartDocument();
			xw.WriteStartElement("HAProject");
			xw.WriteAttributeString("version", Application.ProductVersion);
			XmlSerializer xs = new XmlSerializer(typeof (HAViewInfo));
			xs.Serialize(xw, this.ViewInfo);
			foreach (TreeNode n in this.ClassTreeView.Nodes)
			{
				(n as HAClassNode).ToXml(xw);
			}
			xw.WriteEndElement();
			xw.WriteEndDocument();
			xw.Flush();
			xw.Close();
			return true;
		}

		private bool SaveHds()
		{
			StreamWriter sw;

			HAClassNode n = this.ClassTreeView.SelectedNode as HAClassNode;
			if (n == null)
			{
				MessageBox.Show("クラスが選択されていません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			string msg = "HDS 形式では現在開かれているクラスだけが保存されます。\r\n" + "注釈, メンバ, 引数, 変数は保存されません。";
			if (MessageBox.Show(msg, "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
			{
				return false;
			}
			try
			{
				sw = new StreamWriter(this.FullName, false, Encoding.UTF8);
			}
			catch
			{
				return false;
			}
			sw.NewLine = "\n";
			XmlTextWriter xw = new XmlTextWriter(sw);
			xw.Formatting = Formatting.Indented;
			xw.Indentation = 0;
			xw.WriteStartDocument();
			xw.WriteStartElement("hds");
			xw.WriteAttributeString("version", "0.3.5");
			foreach (TreeNode nn in n.Body.Nodes)
			{
				(nn as HAFuncNode).ToHds(xw);
			}
			xw.WriteEndElement();
			xw.WriteEndDocument();
			xw.Flush();
			xw.Close();
			return true;
		}
	}
}
