using System;
using System.Text;

namespace Girl.HierArch
{
	/// <summary>
	/// ノードの Text から名前と属性を分離します。
	/// </summary>
	public class ObjectParser
	{
		private string m_Name = "";
		private string m_Type = "";

		public ObjectParser()
		{
		}

		public ObjectParser(string text)
		{
			this.Parse(text);
		}

		public void Parse(string text)
		{
			string[] list = text.Split(':');
			this.m_Name = ObjectParser.Strip(list[0]);
			this.m_Type = "";
			if (list.GetLength(0) < 2) return;

			this.m_Type = ObjectParser.Strip(list[1]);
		}

		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		public string Type
		{
			get
			{
				return this.m_Type;
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

		public bool IsSpecial
		{
			get
			{
				return (this.m_Name == "__CLASS" || this.m_Name == "~__CLASS");
			}
		}

		public string FunctionDeclaration
		{
			get
			{
				if (this.IsSpecial) return this.Name;
				if (this.Type == "") return "void " + this.Name;
				return this.Type + " " + this.Name;
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
