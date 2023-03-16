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

		public void WriteCode(CodeWriter cw, string source, Hashtable replace, ArrayList history)
		{
			StringBuilder sb = new StringBuilder();
			CSharpParser csp = new CSharpParser();
			StringReader sr = new StringReader(source);
			csp.Reader = sr;
			int indent = cw.Indent;
			int prth = 0;
			bool no_space = false, is_key_word = false;
			string text;
			while (csp.Read())
			{
				text = csp.Text;
				if (replace != null && replace.Contains(text)) text = replace[text] as string;
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
			while (csp.Read())
			{
				if (csp.Text == "]")
				{
					cw.WriteCode(string.Format("// Macro: {0}", decl.ToString()));
					this.WriteMacro(cw, name.ToString(), args, history);
					return true;
				}
				decl.Append(csp.Text);
				if (csp.Text == "(")
				{
					for (;;)
					{
						string arg = this.ReadArg(csp);
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
			StringBuilder sb = new StringBuilder();
			int prth = 0;
			while (csp.Read())
			{
				if (prth < 1 && csp.Text == ",") break;
				if (csp.Text == "(")
				{
					prth++;
				}
				else if (csp.Text == ")")
				{
					if (prth < 1) break;
					prth--;
				}
				sb.Append(csp.Spacing);
				sb.Append(csp.Text);
			}
			return sb.ToString();
		}

		private void WriteMacro(CodeWriter cw, string name, StringCollection args, ArrayList history)
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
			foreach (object obj in n.Objects)
			{
				cw.WriteCode(string.Format("{0};", new ObjectParser((obj as HAObjectNode).Text).ObjectDeclaration));
			}
			this.WriteCode(cw, n.Source, replace, history);
			cw.WriteEndBlock();
		}
	}
}
