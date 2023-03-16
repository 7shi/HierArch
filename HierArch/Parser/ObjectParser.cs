// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

// ここにソースコードの注釈を書きます。

using System;
using System.Text;

namespace Girl.HierArch
{
	/// <summary>
	/// ノードの Text から名前と属性を分離します。
	/// </summary>
	public class ObjectParser
	{
		private string name;
		private string type;
		public HAType ObjectType;

		private void Init()
		{
			this.name = "";
			this.type = "";
			this.ObjectType = HAType.Public;
		}

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public ObjectParser()
		{
			this.Init();
		}

		public ObjectParser(string text)
		{
			this.Init();
			this.Parse(text);
		}

		public ObjectParser(string text, HAType objectType)
		{
			this.Init();
			this.Parse(text);
			this.ObjectType = objectType;
		}

		public void Parse(string text)
		{
			string [] list = text.Split(':');
			this.name = ObjectParser.Strip(list[0]);
			this.type = "";
			if (list.GetLength(0) < 2) return;
			this.type = ObjectParser.Strip(list[1]);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		public static string Strip(string text)
		{
			StringBuilder sb = new StringBuilder();
			bool space = false;
			foreach (char ch in text)
			{
				if (ch <= ' ')
				{
					space = true;
				}
				else
				{
					if (space)
					{
						if (sb.Length > 0) sb.Append(' ');
						space = false;
					}
					sb.Append(ch);
				}
			}
			return sb.ToString();
		}

		#region Category

		public bool IsConstructor
		{
			get
			{
				return this.name == "__" + "CLASS";
			}
		}

		public bool IsDestructor
		{
			get
			{
				return this.name == "~__" + "CLASS";
			}
		}

		public bool IsProperty
		{
			get
			{
				return this.IsGet || this.IsSet;
			}
		}

		public bool IsGet
		{
			get
			{
				return this.name.StartsWith("get_");
			}
		}

		public bool IsSet
		{
			get
			{
				return this.name.StartsWith("set_");
			}
		}

		#endregion

		public string PropertyName
		{
			get
			{
				return this.name.Substring(4);
			}
		}

		public string PropertyPair
		{
			get
			{
				return ((this.IsGet) ? "set_" : "get_") + this.PropertyName;
			}
		}

		public string FunctionDeclaration
		{
			get
			{
				string access = this.ObjectType.ToString().ToLower();
				if (this.IsConstructor)
				{
					return access + " " + this.Name;
				}
				else if (this.IsDestructor)
				{
					return this.Name;
				}
				else if (this.Type == "")
				{
					return access + " void " + this.Name;
				}
				else if (this.Type == "static" || this.Type == "virtual" || this.Type == "override" || this.Type == "new")
				{
					return access + " " + this.Type + " void " + this.Name;
				}
				return access + " " + this.Type + " " + this.Name;
			}
		}

		public string PropertyDeclaration
		{
			get
			{
				return this.ObjectType.ToString().ToLower() + " " + this.Type + " " + this.PropertyName;
			}
		}

		public string ObjectDeclaration
		{
			get
			{
				string t = this.Type;
				if (t == "") t = "object";
				return t + " " + this.Name;
			}
		}
	}
}
