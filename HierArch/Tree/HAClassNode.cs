// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Reflection;
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
	public class HAClassNode : HATreeNode
	{
		public const string ext = "hacls";
		public ArrayList Members;
		public HAFuncNode Header;
		public HAFuncNode Body;
		public HAFuncNode Footer;
		public HAClassProperty Property;

		public override void Init()
		{
			base.Init();
			
			this.Members   = new ArrayList();
			this.Header    = new HAFuncNode();
			this.Body      = new HAFuncNode();
			this.Footer    = new HAFuncNode();
			this.Property  = new HAClassProperty(this);
		}

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAClassNode()
		{
		}

		public HAClassNode(string text)
		{
			this.Text = text;
		}

		public override string XmlName
		{
			get
			{
				return "HAClass";
			}
		}

		public override HATreeNode NewNode
		{
			get
			{
				return new HAClassNode();
			}
		}

		public string Namespace
		{
			get
			{
				ObjectParser op = new ObjectParser(this.Text);
				string ns = (this.IsFolder) ? op.Type : "";
				if (ns.IndexOf('.') >= 0 || this.Parent == null) return ns;
				
				string pns = (this.Parent as HAClassNode).Namespace;
				if (pns != "") return (ns != "") ? pns + "." + ns : pns;
				return ns;
			}
		}

		#region XML

		public override void WriteXml(XmlTextWriter xw)
		{
			base.WriteXml(xw);
			
			foreach (Object obj in this.Members)
			{
				if (obj is HAMemberNode) ((HAMemberNode)obj).ToXml(xw);
			}
			
			xw.WriteStartElement("Header");
			this.Header.ToXml(xw);
			xw.WriteEndElement();
			
			xw.WriteStartElement("Body");
			this.Body.ToXml(xw);
			xw.WriteEndElement();
			
			xw.WriteStartElement("Footer");
			this.Footer.ToXml(xw);
			xw.WriteEndElement();
		}

		public override void ReadXmlNode(XmlTextReader xr)
		{
			if (xr.Name == "HAObject" && xr.NodeType == XmlNodeType.Element)
			{
				HAMemberNode n = new HAMemberNode();
				Members.Add(n);
				n.FromXml(xr);
			}
			else if (xr.Name == "Header" && xr.NodeType == XmlNodeType.Element && !xr.IsEmptyElement)
			{
				while (xr.Read() && xr.NodeType == XmlNodeType.Whitespace);
				if (xr.Name == "HAFunc" && xr.NodeType == XmlNodeType.Element) this.Header.FromXml(xr);
			}
			else if (xr.Name == "Body" && xr.NodeType == XmlNodeType.Element && !xr.IsEmptyElement)
			{
				while (xr.Read() && xr.NodeType == XmlNodeType.Whitespace);
				if (xr.Name == "HAFunc" && xr.NodeType == XmlNodeType.Element) this.Body.FromXml(xr);
			}
			else if (xr.Name == "Footer" && xr.NodeType == XmlNodeType.Element && !xr.IsEmptyElement)
			{
				while (xr.Read() && xr.NodeType == XmlNodeType.Whitespace);
				if (xr.Name == "HAFunc" && xr.NodeType == XmlNodeType.Element) this.Footer.FromXml(xr);
			}
		}

		public void FromHds(XmlTextReader xr)
		{
			this.Type = HAType.Text;
			if (xr.Name != "hds" || xr.NodeType != XmlNodeType.Element || xr.IsEmptyElement) return;
			
			HAFuncNode n;
			while (xr.Read())
			{
				if (xr.Name == "node" && xr.NodeType == XmlNodeType.Element)
				{
					n = new HAFuncNode();
					this.Body.Nodes.Add(n);
					n.FromHds(xr);
				}
				else if (xr.Name == "hds" && xr.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		#endregion

		#region Generation

		public void Generate(string path)
		{
			HAType t = this.Type;
			string target = path;
			if (!target.EndsWith(@"\")) target += @"\";
			target += new ObjectParser(this.Text).Name;
			
			if (t == HAType.Comment)
			{
				return;
			}
			else if (this.IsRealFolder)
			{
				if (!new DirectoryInfo(target).Exists)
				{
					try
					{
						Directory.CreateDirectory(target);
					}
					catch
					{
						return;
					}
				}
				path = target;
				foreach (TreeNode n in this.Body.Nodes)
				{
					(n as HAFuncNode).GenerateFolder(path);
				}
			}
			else if (this.IsObject)
			{
				this.GenerateClass(target + ".cs");
			}
			else if (this.IsText)
			{
				this.GenerateText(target + ".txt");
			}
			
			foreach (TreeNode n in this.Nodes)
			{
				(n as HAClassNode).Generate(path);
			}
		}

		private void GenerateClass(string target)
		{
			FileStream fs;
			CodeWriter cw;

			try
			{
				fs = new FileStream(target, FileMode.Create);
			}
			catch
			{
				return;
			}
			
			cw = new CodeWriter(fs, Encoding.UTF8);
			ObjectParser op = new ObjectParser(this.Text);
			cw.ClassName = op.Name;
			
			// Header
			this.WriteHeader(cw);
			this.WriteLocalHeader(cw);
			if (this.Header.Comment != "")
			{
				cw.WriteBlankLine();
				cw.WriteCodes("// ", this.Header.Comment);
			}
			if (this.Header.Source != "")
			{
				cw.WriteBlankLine();
				cw.WriteCodes(this.Header.Source);
			}
			
			// Namespace
			string ns = this.Namespace;
			if (ns != "")
			{
				cw.WriteBlankLine();
				cw.WriteStartBlock("namespace " + ns);
			}
			
			// Body
			cw.WriteBlankLine();
			if (this.Body.Comment != "") cw.WriteCodes("/// ", this.Body.Comment);
			string classdecl = this.Type.ToString().ToLower() + " class " + op.Name;
			if (op.Type != "") classdecl += " : " + op.Type;
			cw.WriteStartBlock(classdecl);
			if (this.Body.Source != "")
			{
				cw.WriteCodes(cw.ReplaceKeywords(this.Body.Source));
			}
			foreach (Object obj in this.Members)
			{
				(obj as HAMemberNode).Generate(cw);
			}
			this.Body.ResetPropertyPair();
			this.Body.SearchPropertyPair(this.Body);
			foreach (Object obj in this.Body.Nodes)
			{
				(obj as HAFuncNode).GenerateClass(cw);
			}
			cw.WriteEndBlock();
			
			if (ns != "") cw.WriteEndBlock();
			
			// Footer
			if (this.Footer.Comment != "")
			{
				cw.WriteBlankLine();
				cw.WriteCodes("/// ", this.Footer.Comment);
			}
			if (this.Footer.Source != "")
			{
				cw.WriteBlankLine();
				cw.WriteCodes(this.Footer.Source);
			}
			
			cw.Close();
			fs.Close();
		}

		private void WriteHeader(CodeWriter cw)
		{
			if (this.Parent != null) (this.Parent as HAClassNode).WriteHeader(cw);
			if (!this.IsFolder) return;
			
			HAFuncNode n = this.Body.Search("Header", HAType.Comment);
			if (n == null || n.Source == "") return;
			
			cw.WriteBlankLine();
			cw.WriteCodes("// ", n.Source);
		}

		private void WriteLocalHeader(CodeWriter cw)
		{
			if (!this.IsFolder)
			{
				if (this.Parent != null)
				{
					(this.Parent as HAClassNode).WriteLocalHeader(cw);
				}
				return;
			}
			
			HAFuncNode n = this.Body.Search("LocalHeader", HAType.Comment);
			if (n == null || n.Source == "") return;
			
			cw.WriteBlankLine();
			cw.WriteCodes("// ", n.Source);
		}

		private void GenerateText(string target)
		{
			FileStream fs;
			HierArchWriter haw;
			AppDomain domain;

			try
			{
				fs = new FileStream(target, FileMode.Create);
			}
			catch
			{
				return;
			}
			
			haw = null;
			domain = null;
			try
			{
				string dll = string.Format(@"{0}\{1}.dll",
					HADoc.UserPluginDir, this.Type.ToString());
				if (File.Exists(dll))
				{
					AppDomainSetup setup = new AppDomainSetup();
					setup.ApplicationBase = HADoc.UserPluginDir;
					domain = AppDomain.CreateDomain(this.Type.ToString(), null, setup);
					haw = domain.CreateInstanceFromAndUnwrap(
						dll, "Plugin", false,
						BindingFlags.Default, null,
						new object[] {fs},
						null, null, null) as HierArchWriter;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			if (haw == null) haw = new HierArchWriter(fs, Encoding.Default);
			
			haw.WriteTitle(this.Type, this.Text);
			
			int i = 1;
			foreach (Object obj in this.Body.Nodes)
			{
				(obj as HAFuncNode).GenerateText(haw, i.ToString(), this.Type == HAType.TextBlue);
				i++;
			}
			haw.Close();
			fs .Close();
			if (domain != null) AppDomain.Unload(domain);
		}

		#endregion
	}
}
