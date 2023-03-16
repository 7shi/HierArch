// このファイルは ..\PluginEditor.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Girl.Windows.Forms;

/// <summary>
/// ここにクラスの説明を書きます。
/// </summary>
public class PluginEditorDoc : Document
{
	public string Text;

	/// <summary>
	/// コンストラクタです。
	/// </summary>
	public PluginEditorDoc()
	{
		this.Text = "";
	}

	public override bool Open()
	{
		FileStream fs;
		bool isUtf8 = false;
		try
		{
			// UTF-8 決め打ち
			fs = new FileStream(FullName, FileMode.Open);
			if (fs.Length > 2)
			{
				byte[] bom = new byte[3];
				fs.Read(bom, 0, 3);
				if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
				{
					isUtf8 = true;
				}
				else
				{
					fs.Position = 0;
				}
			}
		}
		catch
		{
			return false;
		}
		
		Cursor cur = Cursor.Current;
		Cursor.Current = Cursors.WaitCursor;
		StreamReader sr = new StreamReader(fs, isUtf8 ? Encoding.UTF8 : Encoding.Default);
		this.Text = sr.ReadToEnd();
		sr.Close();
		fs.Close();
		Changed = false;
		Cursor.Current = cur;
		return true;
	}

	public override bool Save()
	{
		FileStream fs;
		try
		{
			// UTF-8 決め打ち
			fs = new FileStream(FullName, FileMode.Create);
		}
		catch
		{
			return false;
		}
		
		Cursor cur = Cursor.Current;
		Cursor.Current = Cursors.WaitCursor;
		StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
		sw.Write(this.Text);
		sw.Close();
		fs.Close();
		Changed = false;
		Cursor.Current = cur;
		return true;
	}

	public static string BuildDateTime
	{
		get
		{
			return "2002/12/30 15:56:08";
		}
	}
}
