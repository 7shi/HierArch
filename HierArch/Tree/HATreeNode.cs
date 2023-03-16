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
		protected string link;
		public string Server;
		public DateTime LastModified;

		public virtual void Init()
		{
			this.Type = HAType.Public;
			this.m_IsExpanded = false;
			this.m_IsSelected = false;
			this.link = string.Empty;
			this.Server = "";
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
				return (this.m_Type == HAType.Public
					|| this.m_Type == HAType.Protected
					|| this.m_Type == HAType.Private);
			}
		}

		public bool IsText
		{
			get
			{
				switch (this.m_Type)
				{
					case HAType.Text:
					case HAType.TextBlue:
					case HAType.TextBrown:
					case HAType.TextGray:
					case HAType.TextGreen:
					case HAType.TextRed:
						return true;
				}
				return false;
			}
		}

		public bool IsFolder
		{
			get
			{
				return (this.IsRealFolder
					|| this.m_Type == HAType.FolderGray || this.m_Type == HAType.FolderGray_Open);
			}
		}

		public bool IsRealFolder
		{
			get
			{
				switch (this.m_Type)
				{
					case HAType.Folder:
					case HAType.Folder_Open:
					case HAType.FolderBlue:
					case HAType.FolderBule_Open:
					case HAType.FolderBrown:
					case HAType.FolderBrown_Open:
					case HAType.FolderGreen:
					case HAType.FolderGreen_Open:
					case HAType.FolderRed:
					case HAType.FolderRed_Open:
						return true;
				}
				return false;
			}
		}

		#endregion

		public override void SetIcon()
		{
			int t = (int)this.m_Type;
			if (this.IsFolder)
			{
				if (m_Type.ToString().EndsWith("_Open")) t--;
				if (this.Nodes.Count > 0 && this.IsExpanded) t++;
			}
			this.SelectedImageIndex = this.ImageIndex = t;
		}

		public override object Clone()
		{
			HATreeNode ret = (HATreeNode)base.Clone();
			ret.Type         = this.Type;
			ret.m_IsExpanded = this.m_IsExpanded;
			ret.m_IsSelected = this.m_IsSelected;
			ret.link         = this.link;
			ret.Server       = this.Server;
			ret.LastModified = this.LastModified;
			return ret;
		}

		public void StoreState()
		{
			this.m_IsExpanded = this.IsExpanded;
			this.m_IsSelected = this.IsSelected;
			
			foreach (TreeNode n in this.Nodes)
			{
				if (n is HATreeNode) ((HATreeNode)n).StoreState();
			}
		}

		public void ApplyState()
		{
			if (this.m_IsExpanded) this.Expand();
			if (this.m_IsSelected) this.TreeView.SelectedNode = this;
			if (this.link == string.Empty)
			{
				this.NodeFont = null;
			}
			else if (this.TreeView != null)
			{
				this.NodeFont = (this.TreeView as HAClass).LinkFont;
			}
			
			foreach (TreeNode n in this.Nodes)
			{
				if (n is HATreeNode) ((HATreeNode)n).ApplyState();
			}
		}

		#region XML

		public override void ToXml(XmlTextWriter xw)
		{
			XmlTextWriter xw2;
			if (this.link == string.Empty)
			{
				xw2 = xw;
			}
			else if (xw.BaseStream is FileStream)
			{
				try
				{
					Uri uri1 = new Uri((xw.BaseStream as FileStream).Name);
					Uri uri2 = new Uri(this.link);
			
					xw2 = new XmlTextWriter(this.link, Encoding.UTF8);
					xw2.Formatting = Formatting.Indented;
					xw2.WriteStartDocument();
			
					xw.WriteStartElement(this.XmlName);
					xw.WriteAttributeString("Link"  , uri1.MakeRelative(uri2));
					xw.WriteAttributeString("Server", this.Server);
					xw.WriteEndElement();
				}
				catch
				{
					xw2 = xw;
				}
			}
			else if (File.Exists(this.link))
			{
				// ドラッグ＆ドロップによるリンクの再現
				xw2 = null;
				xw.WriteStartElement(this.XmlName);
				xw.WriteAttributeString("Link"  , this.link);
				xw.WriteAttributeString("Server", this.Server);
				xw.WriteEndElement();
			}
			else
			{
				xw2 = xw;
			}
			
			if (xw2 != null)
			{
				xw2.WriteStartElement(this.XmlName);
			
				this.WriteXml(xw2);
			
				DnDTreeNode dn;
				foreach(TreeNode n in Nodes)
				{
					dn = (DnDTreeNode)n;
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
			xw.WriteAttributeString("AllowDrag" , XmlConvert.ToString(this.AllowDrag));
			xw.WriteAttributeString("LastModified", this.LastModified.ToString());
		}

		public override void FromXml(XmlTextReader xr)
		{
			if (xr.Name != this.XmlName || xr.NodeType != XmlNodeType.Element) return;
			
			XmlTextReader xr2;
			string link = xr.GetAttribute("Link");
			if (link == null || link.Length < 1)
			{
				xr2 = xr;
			}
			else
			{
				try
				{
					if (!this.IsValidLink(link)) throw new Exception();
			
					string uri = xr.BaseURI;
					if (uri != null && uri.Length > 0)
					{
						link = new Uri(new Uri(uri), link).LocalPath;
						if (!this.IsValidLink(link)) throw new Exception();
					}
					this.link = link;
					this.Server = xr.GetAttribute("Server");
					if (this.Server == null || this.Server.Length < 1) this.Server = "";
					this.ApplyState();
			
					xr2 = new XmlTextReader(link);
					while (xr2.Read() && xr2.Name != this.XmlName);
				}
				catch
				{
					this.Type = HAType.Comment;
					this.Text = "リンク失敗";
					return;
				}
			}
			
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
			this.Type = (HAType)HAType.Parse(typeof(HAType), xr.GetAttribute("Type"));
			this.Text = xr.GetAttribute("Text");
			this.m_IsExpanded = XmlConvert.ToBoolean(xr.GetAttribute("IsExpanded"));
			this.m_IsSelected = XmlConvert.ToBoolean(xr.GetAttribute("IsSelected"));
			this.AllowDrag    = XmlConvert.ToBoolean(xr.GetAttribute("AllowDrag"));
			
			string lastModified = xr.GetAttribute("LastModified");
			this.LastModified = (lastModified != null && lastModified.Length > 0)
				? DateTime.Parse(lastModified) : DateTime.Now;
		}

		public virtual void ReadXmlNode(XmlTextReader xr)
		{
		}

		#endregion

		#region Link

		public string Link
		{
			get
			{
				return this.link;
			}

			set
			{
				if (value == null)
				{
					this.link = string.Empty;
				}
				else if (!this.IsValidLink(value))
				{
					MessageBox.Show("リンク先が重複しています。:\r\n\r\n" + value,
						"エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else if (value != this.link)
				{
					if (File.Exists(value))
					{
						string server = this.Server;
						this.Init();
						XmlTextReader xr = new XmlTextReader(value);
						while (xr.Read() && xr.Name != this.XmlName);
						this.FromXml(xr);
						xr.Close();
						this.Server = server;
						this.OnRefreshNode(EventArgs.Empty);
					}
					this.link = value;
					this.ApplyState();
				}
			}
		}

		public string TargetFileName
		{
			get
			{
				if (this.link != string.Empty) return this.link;
				
				HATreeNode p = this.Parent as HATreeNode;
				if (p != null) return p.TargetFileName;
				
				return string.Empty;
			}
		}

		public bool IsValidLink(string link)
		{
			HATreeNode p = this.Parent as HATreeNode;
			if (p != null && !p.CheckParentLink(link)) return false;
			
			// foreach (TreeNode n in this.Nodes)
			// {
			// 	HATreeNode nn = n as HATreeNode;
			// 	if (!nn.CheckChildrenLink(link)) return false;
			// }
			
			return true;
		}

		private bool CheckParentLink(string link)
		{
			if (this.link == link) return false;
			
			HATreeNode p = this.Parent as HATreeNode;
			if (p != null) return p.CheckParentLink(link);
			
			return true;
		}

		private bool CheckChildrenLink(string link)
		{
			if (this.link == link) return false;
			
			foreach (TreeNode n in this.Nodes)
			{
				HATreeNode nn = n as HATreeNode;
				if (!nn.CheckChildrenLink(link)) return false;
			}
			
			return true;
		}

		#endregion

		#region 同期

		public bool Synchronize(HAUploaderManager manager, LinkRichTextBox output)
		{
			if (!this.SynchronizeThis(manager, output)) return false;
			
			foreach (TreeNode n in this.Nodes)
			{
				if (!(n as HAClassNode).Synchronize(manager, output)) return false;
			}
			
			return true;
		}

		private bool SynchronizeThis(HAUploaderManager manager, LinkRichTextBox output)
		{
			if (this.Server == "") return true;
			
			output.AppendLine(string.Format("{0}: {1}", this.Text, this.Server));
			if (this.link.Length < 1)
			{
				output.AppendLine("リンクが設定されていません。", Color.Red);
				output.ShowLast();
				return false;
			}
			
			HAUploaderInfo inf = manager.GetInfo(this.Server);
			if (inf.Files == null)
			{
				output.AppendLine("サーバに接続しています...");
				output.ShowLast();
				string res = inf.Dir();
				if (res != null)
				{
					output.AppendLine(string.Format("エラー: {0}", res), Color.Red);
					output.ShowLast();
					return false;
				}
			}
			
			string target = Path.GetFileName(this.link);
			HAUploaderFileInfo fi;
			FileInfo fi2 = new FileInfo(this.link);
			bool existsL = fi2.Exists;
			bool existsS = inf.Files.Contains(target);
			if (!existsL)
			{
				output.AppendLine(string.Format("{0}: ファイルがローカルに存在しません。", this.link));
			}
			if (!existsS)
			{
				output.AppendLine(string.Format("{0}: ファイルがサーバに存在しません。", target));
				if (!existsL)
				{
					output.AppendLine("エラー: リンクが解決できません。", Color.Red);
					output.ShowLast();
					return false;
				}
				fi = new HAUploaderFileInfo(inf, this.link);
				return this.Upload(fi, output);
			}
			
			fi = inf.Files[target] as HAUploaderFileInfo;
			byte[] data = null;
			if (!existsL && existsS)
			{
				return this.Download(fi, fi2, output, data);
			}
			
			if (fi.Length == fi2.Length)
			{
				// ファイルサイズが同じため内容を比較
				output.AppendLine(string.Format("{0}: 内容を比較しています...", target));
				output.ShowLast();
				data = fi.DownloadData();
				if (data == null)
				{
					output.AppendLine("エラー: ダウンロードに失敗しました。", Color.Red);
					output.ShowLast();
					return false;
				}
				bool same = true;
				try
				{
					FileStream fs = fi2.OpenRead();
					foreach (byte b in data)
					{
						int b2 = fs.ReadByte();
						if (b2 != (int)b)
						{
							same = false;
							break;
						}
					}
					fs.Close();
				}
				catch (Exception ex)
				{
					output.AppendLine("エラー: " + ex.ToString(), Color.Red);
					output.ShowLast();
					return false;
				}
				if (same)
				{
					output.AppendLine("最新です。");
					output.ShowLast();
					return true;
				}
			}
			
			// 同期
			string msg = string.Format("ローカル: {0}\r\nサーバ: {1}", fi2.LastWriteTime, fi.LastWriteTime);
			DialogResult dr;
			if (fi.LastWriteTime <= fi2.LastWriteTime)
			{
				// ローカルの方が新しい
				dr = this.Ask("アップロードしますか？\r\n\r\n" + msg, target);
				if (dr == DialogResult.Cancel)
				{
					return false;
				}
				else if (dr == DialogResult.Yes)
				{
					return this.Upload(fi, output);
				}
				return true;
			}
			
			dr = this.Ask("ダウンロードしますか？\r\n\r\n" + msg, target);
			if (dr == DialogResult.Cancel)
			{
				return false;
			}
			else if (dr == DialogResult.No)
			{
				return true;
			}
			
			// ダウンロード
			return this.Download(fi, fi2, output, data);
		}

		private bool Upload(HAUploaderFileInfo fileInfo, LinkRichTextBox output)
		{
			output.AppendLine(fileInfo.Name + ": アップロードしています...");
			output.ShowLast();
			string res = fileInfo.Upload(this.link);
			if (res == null)
			{
				fileInfo.LastWriteTime = DateTime.Now;
				return true;
			}
			
			output.AppendLine(string.Format("エラー: {0}", res), Color.Red);
			output.ShowLast();
			return false;
		}

		private bool Download(HAUploaderFileInfo fi, FileInfo fi2, LinkRichTextBox output, byte[] data)
		{
			if (data == null)
			{
				output.AppendLine(fi.Name + ": ダウンロードしています...");
				output.ShowLast();
				data = fi.DownloadData();
				if (data == null)
				{
					output.AppendLine("エラー: ダウンロードに失敗しました。", Color.Red);
					output.ShowLast();
					return false;
				}
			}
			
			output.AppendLine(fi.Name + ": 書き込んでいます...");
			output.ShowLast();
			try
			{
				DirectoryInfo di = fi2.Directory;
				if (!di.Exists) di.Create();
				FileStream fs = fi2.OpenWrite();
				fs.Write(data, 0, data.Length);
				fs.Close();
			
				string link = this.link;
				string server = this.Server;
				this.Init();
				fs = fi2.OpenRead();
				XmlTextReader xr = new XmlTextReader(fs);
				while (xr.Read() && xr.Name != this.XmlName);
				this.FromXml(xr);
				xr.Close();
				fs.Close();
				this.Link = link;
				this.Server = server;
				this.OnRefreshNode(EventArgs.Empty);
			}
			catch (Exception ex)
			{
				output.AppendLine("エラー: " + ex.ToString(), Color.Red);
				output.ShowLast();
				return false;
			}
			return true;
		}

		#endregion

		protected DialogResult Ask(string question, string caption)
		{
			Cursor cur = Cursor.Current;
			Cursor.Current = Cursors.Default;
			DialogResult ret = MessageBox.Show(question, caption,
				MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
