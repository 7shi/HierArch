using System;
using System.IO;
using System.Text;
using Girl.HierArch;

public class Plugin : HierArchWriter
{
	/// <summary>
	/// コンストラクタです。
	/// </summary>
	public Plugin(FileStream stream) : base(stream, Encoding.Default)
	{
		// 通常は変更しないでください。
	}
	
	/// <summary>
	/// タイトルを出力します。
	/// </summary>
	public override void WriteTitle(HAType type, string text)
	{
		// ここの中を適宜書き換えてください。
		WriteLine("  **** {0} ****", text);
	}
	
	/// <summary>
	/// 各項目を出力します。
	/// </summary>
	public override void WriteNode(HAType type, string chapter, string text, string comment, string source)
	{
		// ここの中を適宜書き換えてください。
		
		// 2行空けます。
		WriteLine();
		WriteLine();
		
		// 見出しを出力します。
		WriteLine("  {0} {1}", chapter, text);
		
		// 1行空けます。
		WriteLine();
		
		// 内容を出力します。
		StringReader sr = new StringReader(source);
		string line;
		// 1行ずつ調べます。
		while ((line = sr.ReadLine()) != null)
		{
			// 空行なら
			if (line == "")
			{
				// 改行します。
				WriteLine();
			}
			// それ以外は
			else
			{
				// そのまま出力します。
				Write(line);
			}
		}
		sr.Close();
		
		// 最後に改行します。
		WriteLine();
	}
}
