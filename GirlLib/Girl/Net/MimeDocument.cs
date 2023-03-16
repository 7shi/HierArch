using System;
using System.Collections;
using System.IO;

namespace Girl.Net
{
	/// <summary>
	/// MIME 文書を管理します。
	/// </summary>
	public class MimeDocument
	{
		private ArrayList objects;
		public string Boundary;
		public string From;
		public string Subject;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public MimeDocument()
		{
			this.objects = new ArrayList();
			
			// 本当は重複をチェックしないといけない
			this.Boundary = "----=_NextPart_000_0000";
			
			this.From = "<MimeDocument>";
			this.Subject = "MimeDocument Generated Page";
		}

		public ArrayList Objects
		{
			get
			{
				return this.objects;
			}
		}

		public void AddMimeObject(MimeObject mimeObject)
		{
			this.objects.Add(mimeObject);
		}

		public void AddMimeObjects(ArrayList objects)
		{
			foreach (object obj in objects)
			{
				this.AddMimeObject(obj as MimeObject);
			}
		}

		#region Write

		public void Write(TextWriter textWriter)
		{
			this.WriteHeader(textWriter);
			
			foreach (object obj in this.objects)
			{
				MimeObject mo = obj as MimeObject;
				mo.Write(textWriter);
				this.WriteBoundary(textWriter);
			}
		}

		public void WriteHeader(TextWriter textWriter)
		{
			textWriter.WriteLine("From: " + this.From);
			textWriter.WriteLine("Subject: " + this.Subject);
			textWriter.WriteLine("MIME-Version: 1.0");
			textWriter.WriteLine("Content-Type: multipart/related;");
			textWriter.WriteLine("\tboundary=\"" + this.Boundary + "\";");
			this.WriteBoundary(textWriter);
		}

		public void WriteBoundary(TextWriter textWriter)
		{
			textWriter.WriteLine();
			textWriter.WriteLine("--{0}", this.Boundary);
		}

		#endregion
	}
}
