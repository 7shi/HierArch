// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

// ここにソースコードの注釈を書きます。

using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Girl.Coding;

namespace Girl.HierArch
{
	/// <summary>
	/// ここにクラスの説明を書きます。
	/// </summary>
	public class HAMacroForm : Form1
	{
		private Hashtable macros;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAMacroForm()
		{
			this.mnuBuildGenerate.Enabled = false;
			this.tbBuildGenerate.Enabled = false;
			this.Open(HADoc.MacroProject);
			this.macros = null;
		}

		protected override void Dispose(bool disposing)
		{
			Form1.MacroForm = null;
			base.Dispose(disposing);
		}

		public void SetMacros()
		{
			this.macros = new Hashtable();
			foreach (TreeNode n in this.view1.tvClass.Nodes)
			{
				this.ReadClass(n as HAClassNode);
			}
		}

		private void ReadClass(HAClassNode node)
		{
			foreach (TreeNode n in node.Body.Nodes)
			{
				this.ReadFunc(n as HAFuncNode);
			}
			foreach (TreeNode n in node.Nodes)
			{
				this.ReadClass(n as HAClassNode);
			}
		}

		private void ReadFunc(HAFuncNode node)
		{
			if (node.Type == HAType.Public)
			{
				this.macros[node.Text] = node;
			}
			foreach (TreeNode n in node.Nodes)
			{
				this.ReadFunc(n as HAFuncNode);
			}
		}

		public HAFuncNode GetMacro(string name)
		{
			if (macros == null || !macros.Contains(name)) return null;
			return macros[name] as HAFuncNode;
		}

		public void WriteCode(CodeWriter cw, string source, string block, Hashtable replace, ArrayList history)
		{
			StringBuilder sb = new StringBuilder();
			CSharpParser csp = new CSharpParser();
			StringReader sr = new StringReader(source);
			csp.Reader = sr;
			int indent = cw.Indent;
			int prth = 0;
			bool no_space = false, is_key_word = false;
			string text;
			Hashtable objlst = this.MakeObjectList(history);
			while (csp.Read())
			{
				text = csp.Text;
				if (replace != null && replace.Contains(text))
				{
					text = replace[text] as string;
				}
				else if (objlst != null && objlst.Contains(text))
				{
					text = objlst[text] as string;
				}
				if (prth < 1 &&(text == ";" || text == ":"))
				{
					sb.Append(text);
					this.WriteCode(cw, sb);
				}
				else if (prth < 1 && text == "{")
				{
					if (sb.Length > 0)
					{
						cw.WriteStartBlock(sb.ToString());
						sb.Length = 0;
					}
					else
					{
						cw.WriteStartBlock();
					}
				}
				else if (prth < 1 && text == "}")
				{
					this.WriteCode(cw, sb);
					cw.WriteEndBlock();
				}
				else if (text.StartsWith("//"))
				{
					if (sb.Length < 1)
					{
						cw.WriteCode(text);
					}
					else
					{
						sb.Append("  ");
						sb.Append(text);
						this.WriteCode(cw, sb);
					}
				}
				else
				{
					switch (text)
					{
						case "@":
						if (!this.WriteMacro(cw, csp, history))
						{
							cw.WriteCode("// Macro: Error");
						}
						break;
						case ".":
						sb.Append(text);
						no_space = true;
						break;
						case "(":
						case "[":
						if (is_key_word) sb.Append(' ');
						is_key_word = false;
						sb.Append(text);
						no_space = true;
						prth++;
						break;
						case ")":
						case "]":
						sb.Append(text);
						no_space = false;
						prth--;
						break;
						case "{":
						sb.Append(" {");
						no_space = false;
						prth++;
						break;
						case "}":
						sb.Append(" }");
						no_space = false;
						prth--;
						break;
						case ",":
						case ";":
						sb.Append(text);
						break;
						case "__YIELD":
						cw.WriteStartBlock("for (;;)  // __YIELD");
						if (block != null)
						{
							this.WriteCode(cw, block, null, replace, history);
						}
						cw.WriteCode("break;");
						cw.WriteEndBlock();
						break;
						default: if (sb.Length > 0 && !no_space) sb.Append(' ');
						sb.Append(text);
						no_space = false;
						is_key_word = csp.IsKeyWord;
						break;
					}
				}
			}
			sr.Close();
			this.WriteCode(cw, sb);
			while (cw.Indent > indent) cw.WriteEndBlock();
		}

		private void WriteCode(CodeWriter cw, StringBuilder sb)
		{
			if (sb.Length < 1) return;
			cw.WriteCode(sb.ToString());
			sb.Length = 0;
		}

		private bool WriteMacro(CodeWriter cw, CSharpParser csp, ArrayList history)
		{
			if (!csp.Read() || csp.Text != "[") return false;
			StringCollection args = new StringCollection();
			StringBuilder name = new StringBuilder();
			StringBuilder decl = new StringBuilder();
			StringBuilder blck = null;
			while (csp.Read())
			{
				if (csp.Text == "]")
				{
					cw.WriteCode(string.Format("// Macro: {0}", decl.ToString()));
					string block =(blck == null) ? null:
					blck.ToString();
					this.WriteMacro(cw, name.ToString(), args, block, history);
					return true;
				}
				if (csp.Text == "{")
				{
					if (blck == null) blck = new StringBuilder();
					this.ReadBlock(blck, csp);
					continue;
				}
				decl.Append(csp.Text);
				if (csp.Text == "(")
				{
					for (;;)
					{
						string arg = this.ReadArg(csp);
						if (arg == null) break;
						args.Add(arg);
						decl.Append(arg);
						decl.Append(csp.Text);
						if (csp.Text == ")") break;
					}
				}
				else
				{
					name.Append(csp.Text);
				}
			}
			return false;
		}

		private string ReadArg(CSharpParser csp)
		{
			StringBuilder sb = null;
			int prth = 0;
			while (csp.Read())
			{
				if (prth < 1 && csp.Text == ",") break;
				switch (csp.Text)
				{
					case "(":
					case "[":
					case "{":
					prth++;
					break;
					case ")":
					case "]":
					case "}":
					prth--;
					break;
				}
				if (prth < 0) break;
				if (sb == null) sb = new StringBuilder();
				sb.Append(csp.Spacing);
				sb.Append(csp.Text);
			}
			return (sb != null) ? sb.ToString():
			null;
		}

		private void WriteMacro(CodeWriter cw, string name, StringCollection args, string block, ArrayList history)
		{
			if (!this.macros.Contains(name)) return;
			HAFuncNode n = this.macros[name] as HAFuncNode;
			if (history == null)
			{
				history = new ArrayList();
			}
			else if (history.Contains(n))
			{
				cw.WriteCode("// Macro: 循環を検出しました。");
				return;
			}
			history.Add(n);
			Hashtable replace = new Hashtable();
			int c1 = args.Count;
			int c2 = n.Args.Count;
			cw.WriteStartBlock();
			for (int i = 0; i < c1 && i < c2; i++)
			{
				ObjectParser op = new ObjectParser((n.Args[i] as HAObjectNode).Text);
				if (i == c2 - 1 && c1 > c2)
				{
					StringBuilder sb = new StringBuilder();
					for (int j = i; j < c1; j++)
					{
						if (sb.Length > 0) sb.Append(", ");
						sb.Append(args[j]);
					}
					replace.Add(op.Name, sb.ToString());
				}
				else
				{
					replace.Add(op.Name, args[i]);
				}
			}
			int level = history.Count - 1;
			int num = 0;
			foreach (object obj in n.Objects)
			{
				string type = new ObjectParser((obj as HAObjectNode).Text).Type;
				if (type != null && type.Length > 0)
				{
					cw.WriteCode(string.Format("{0} __{1}_{2};", type, level, num));
				}
				num++;
			}
			this.WriteCode(cw, n.Source, block, replace, history);
			cw.WriteEndBlock();
		}

		private void ReadBlock(StringBuilder sb, CSharpParser csp)
		{
			int level = 0;
			while (csp.Read())
			{
				if (csp.Text == "}")
				{
					if (level < 1) return;
					level--;
				}
				else if (csp.Text == "{")
				{
					level++;
				}
				sb.Append(csp.Spacing);
				sb.Append(csp.Text);
			}
		}

		private Hashtable MakeObjectList(ArrayList history)
		{
			if (history == null) return null;
			Hashtable ret = new Hashtable();
			int len = history.Count;
			for (int i = len - 1; i >= 0; i--)
			{
				int num = 0;
				HAFuncNode n = history[i] as HAFuncNode;
				foreach (object obj in n.Objects)
				{
					this.MakeObjectList(ret, obj as HAObjectNode, i, ref num);
				}
			}
			return ret;
		}

		private void MakeObjectList(Hashtable list, HAObjectNode node, int level, ref int num)
		{
			string name = new ObjectParser(node.Text).Name;
			if (!list.Contains(name))
			{
				list[name] = string.Format("__{0}_{1}", level, num);
				num++;
			}
			foreach (TreeNode n in node.Nodes)
			{
				this.MakeObjectList(list, n as HAObjectNode, level, ref num);
			}
		}
	}
}
