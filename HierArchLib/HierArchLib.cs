using System;
using System.IO;
using System.Text;

namespace Girl.HierArch
{
	/// <summary>
	/// 出力用 Plugin のベースクラスです。
	/// </summary>
	public class HierArchWriter : StreamWriter
	{
		public HierArchWriter(FileStream stream, Encoding encoding) : base(stream, encoding)
		{
		}

		public virtual void WriteTitle(HAType type, string text)
		{
			// ここの中を適宜書き換えてください。
			WriteLine("  **** {0} ****", text);
		}

		public virtual void WriteNode(HAType type, string chapter, string text, string comment, string source)
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

	public enum HAType
	{
		Public,
		Protected,
		Private,
		Class,
		Module,
		Comment,
		Smile,
		Folder,
		Folder_Open,
		FolderBlue,
		FolderBule_Open,
		FolderBrown,
		FolderBrown_Open,
		FolderGray,
		FolderGray_Open,
		FolderGreen,
		FolderGreen_Open,
		FolderRed,
		FolderRed_Open,
		Text,
		TextBlue,
		TextBrown,
		TextGray,
		TextGreen,
		TextRed,
		Point,
		PointRed
	}
}
