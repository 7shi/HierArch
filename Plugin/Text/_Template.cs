using System;
using System.IO;
using System.Text;

public class Plugin : StreamWriter
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
	public void WriteTitle(string text)
	{
		// ここの中を適宜書き換えてください。
		WriteLine("  **** {0} ****", text);
	}
	
	/// <summary>
	/// 各項目を出力します。
	/// </summary>
	public void WriteNode(string chapter, string text, string comment, string source)
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
		WriteLine(source);
	}
}