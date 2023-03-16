using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Girl.Xml;

namespace Girl.Net
{
	/// <summary>
	/// MIME に格納されるファイルを処理します。
	/// </summary>
	public class MimeObject
	{
		private string contentType;
		private string location;
		private byte[] data;

		private void Init()
		{
			this.contentType = null;
			this.location = null;
			this.data = null;
		}

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public MimeObject()
		{
			this.Init();
		}

		public MimeObject(string url)
		{
			this.Read(url);
		}

		public MimeObject(string url, TextBoxBase textBox)
		{
			this.Read(url, textBox);
		}

		#region Properties

		public string ContentType
		{
			get
			{
				return this.contentType;
			}
		}

		public string Location
		{
			get
			{
				return this.location;
			}
		}

		public byte[] Data
		{
			get
			{
				return this.data;
			}
		}

		#endregion

		public bool Read(string url, TextBoxBase textBox)
		{
			int b;

			this.Init();
			if (textBox != null)
			{
				this.AppendText(textBox, url + "\r\n");
				this.AppendText(textBox, "サーバに接続しています...\r\n");
			}
			this.location = url;
			HttpWebRequest req = null;
			for (int i = 0; req == null && i < 3; i++)
			{
				try
				{
					req = WebRequest.Create(url) as HttpWebRequest;
				}
				catch
				{
					req = null;
					this.AppendText(textBox, "再試行しています。\r\n");
					Thread.Sleep(1000);
				}
			}
			if (req == null)
			{
				this.AppendText(textBox, "接続に失敗しました。\r\n");
				return false;
			}
			HttpWebResponse res = req.GetResponse() as HttpWebResponse;
			if (res == null) return false;
			if (textBox != null)
			{
				this.AppendText(textBox, "データを読み取っています...\r\n");
			}
			this.contentType = res.ContentType;
			Stream str = res.GetResponseStream();
			MemoryStream ms = new MemoryStream();
			while ((b = str.ReadByte()) != -1)
			{
				ms.WriteByte((byte) b);
			}
			ms.Close();
			str.Close();
			res.Close();
			this.data = ms.ToArray();
			if (textBox != null)
			{
				this.AppendText(textBox, "完了しました。\r\n");
			}
			return true;
		}

		public bool Read(string url)
		{
			return this.Read(url, null);
		}

		public void Write(TextWriter textWriter)
		{
			if (this.contentType == null || !this.contentType.StartsWith("text/"))
			{
				this.WriteBinary(textWriter);
			}
			else
			{
				this.WriteText(textWriter);
			}
		}

		private void WriteBinary(TextWriter textWriter)
		{
			textWriter.WriteLine("Content-Type: " + this.contentType);
			textWriter.WriteLine("Content-Transfer-Encoding: base64");
			textWriter.WriteLine("Content-Location: " + this.location);
			textWriter.WriteLine();
			int len = this.data.Length;
			for (int p = 0; p < len; p += 57)
			{
				int len2 = len - p;
				if (len2 > 57) len2 = 57;
				textWriter.WriteLine(Convert.ToBase64String(this.data, p, len2));
			}
		}

		private void WriteText(TextWriter textWriter)
		{
			textWriter.WriteLine("Content-Type: " + this.contentType);
			textWriter.WriteLine("Content-Transfer-Encoding: quoted-printable");
			textWriter.WriteLine("Content-Location: " + this.location);
			textWriter.WriteLine();
			char ch = '\0', prev;
			foreach (byte b in this.data)
			{
				prev = ch;
				ch =(char) b;
				if (ch == '\r')
				{
					textWriter.WriteLine();
				}
				else if (ch == '\n')
				{
					if (prev != '\r') textWriter.WriteLine();
				}
				else if (ch == '=')
				{
					textWriter.Write("=3D");
				}
				else if (ch > 127)
				{
					textWriter.Write("={0:X2}", b);
				}
				else
				{
					textWriter.Write(ch);
				}
			}
		}

		public ArrayList ParseHtml(TextBoxBase textBox)
		{
			ArrayList ret;
			object target;

			ret = new ArrayList();
			MemoryStream ms = new MemoryStream(this.data, false);
			XmlParser xp = new XmlParser(ms);
			xp.dec = Encoding.GetEncoding("windows-1250").GetDecoder();
			while (xp.Read())
			{
				target = null;
				if (xp.Name == "img" && xp.Attributes.Contains("src"))
				{
					target = xp.Attributes["src"];
				}
				else if (xp.Name == "link" && xp.Attributes.Contains("href"))
				{
					target = xp.Attributes["href"];
				}
				if (target != null)
				{
					this.AddObject(ret, target.ToString(), textBox);
				}
			}
			xp.Close();
			ms.Close();
			return ret;
		}

		public ArrayList ParseHtml()
		{
			return this.ParseHtml(null);
		}

		private void AddObject(ArrayList arrayList, string url, TextBoxBase textBox)
		{
			StringBuilder sb;
			string url2;
			string target;

			sb = new StringBuilder();
			foreach (char ch in url)
			{
				if (ch != '\r' && ch != '\n') sb.Append(ch);
			}
			url2 = sb.ToString();
			foreach (object obj in arrayList)
			{
				if ((obj as MimeObject).location == url2) return;
			}
			if (!url2.StartsWith("http://"))
			{
				target = new Uri(new Uri(this.location), url2).AbsoluteUri;
			}
			else
			{
				target = url2;
			}
			MimeObject mo = new MimeObject(target, textBox);
			if (mo.data == null) return;
			arrayList.Add(mo);
		}

		public void AppendText(TextBoxBase textBox, string text)
		{
			textBox.AppendText(text);
			if (textBox is RichTextBox)
			{
				textBox.Focus();
				textBox.ScrollToCaret();
			}
			textBox.Refresh();
		}
	}
}
