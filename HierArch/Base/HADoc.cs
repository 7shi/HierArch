// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Drawing;
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
			
			this.InitUserPlugin();
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
			XmlTextReader xr;
			HAClassNode n;

			try
			{
				xr = new XmlTextReader(this.FullName);
			}
			catch
			{
				return false;
			}
			while (xr.Read())
			{
				if (xr.Name == "HAViewInfo" && xr.NodeType == XmlNodeType.Element)
				{
					this.ViewInfo = new XmlSerializer(
						typeof(HAViewInfo)).Deserialize(xr) as HAViewInfo;
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
			XmlTextWriter xw;

			try
			{
				xw = new XmlTextWriter(this.FullName, Encoding.UTF8);
			}
			catch
			{
				return false;
			}
			xw.Formatting = Formatting.Indented;
			xw.WriteStartDocument();
			xw.WriteStartElement("HAProject");
			xw.WriteAttributeString("version" , Application.ProductVersion);
			
			XmlSerializer xs = new XmlSerializer(typeof(HAViewInfo));
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
			
			string msg = "HDS 形式では現在開かれているクラスだけが保存されます。\r\n"
				+ "注釈, メンバ, 引数, 変数は保存されません。";
			
			if (MessageBox.Show(msg, "確認", MessageBoxButtons.OKCancel,
				MessageBoxIcon.Information) == DialogResult.Cancel)
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
			return true;
		}

		public static string BuildDateTime
		{
			get
			{
				return "2003/02/16 22:17:45";
			}
		}

		#region Plugin

		public static string UserDir
		{
			get
			{
				string ret = Environment.GetFolderPath(
					Environment.SpecialFolder.Personal) + @"\HierArch";
				if (!Directory.Exists(ret)) Directory.CreateDirectory(ret);
				return ret;
			}
		}

		public static string UserPluginDir
		{
			get
			{
				string ret = HADoc.UserDir + @"\Plugin";
				if (!Directory.Exists(ret)) Directory.CreateDirectory(ret);
				return ret;
			}
		}

		public static string SysPluginDir
		{
			get
			{
				return ApplicationDataManager.SearchFolder("Plugin");
			}
		}

		private void InitUserPlugin()
		{
			string dir1 = HADoc.SysPluginDir;
			if (dir1 == null) return;
			
			string dir2 = HADoc.UserPluginDir;
			
			DirectoryInfo di = new DirectoryInfo(dir1);
			foreach (FileInfo fi in di.GetFiles("*.cs"))
			{
				string source = dir2 + @"\" + fi.Name;
				if (!File.Exists(source)) File.Copy(fi.FullName, source, true);
			}
			foreach (FileInfo fi in di.GetFiles("*.miopt"))
			{
				string source = dir2 + @"\" + fi.Name;
				if (!File.Exists(source)) File.Copy(fi.FullName, source, true);
			}
			
			string dll = dir2 + @"\HierArchLib.dll";
			if (File.Exists(dll)) return;
			
			DirectoryInfo di2 = new FileInfo(Application.ExecutablePath).Directory;
			File.Copy(di2.FullName + @"\HierArchLib.dll", dll);
		}

		public void Make()
		{
			this.InitUserPlugin();
			
			DirectoryInfo di = new DirectoryInfo(HADoc.UserPluginDir);
			foreach (FileInfo fi in di.GetFiles("*.cs"))
			{
				string source = fi.FullName;
				string dll = source.Substring(0, source.Length - 2) + "dll";
				if (File.Exists(dll))
				{
					DateTime dt1 = fi.LastWriteTime;
					DateTime dt2 = File.GetLastWriteTime(dll);
					if (dt1 <= dt2) continue;
				}
				this.Compile(source, dll);
			}
		}

		public CompilerResults Compile(string source, string dll)
		{
			Microsoft.CSharp.CSharpCodeProvider codeProvider =
				new Microsoft.CSharp.CSharpCodeProvider();
			ICodeCompiler icc = codeProvider.CreateCompiler();
			CompilerParameters parameters = new CompilerParameters();
			parameters.GenerateExecutable = true;
			parameters.ReferencedAssemblies.AddRange(new string[]
				{
					"System.dll", HADoc.UserPluginDir + @"\HierArchLib.dll"
				});
			parameters.OutputAssembly = dll;
			parameters.CompilerOptions = "/target:library";
			
			return icc.CompileAssemblyFromFile(parameters, source);
		}

		#endregion
	}
}
