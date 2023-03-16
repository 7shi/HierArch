using System;
using System.IO;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// 文書を管理するクラスです。
	/// </summary>
	public class Document
	{
		public Document()
		{
		}

		private string m_sFullName = "";
		public string FullName
		{
			get
			{
				return m_sFullName;
			}
			
			set
			{
				m_sFullName = value;
			}
		}

		public string Name
		{
			get
			{
				if(m_sFullName == "") return "無題";
				return new FileInfo(m_sFullName).Name;
			}
		}

		private bool m_bChanged = false;
		public bool Changed
		{
			get
			{
				return m_bChanged;
			}

			set
			{
				m_bChanged = value;
			}
		}

		public virtual bool Open()
		{
			Changed = false;
			return true;
		}

		public virtual bool Save()
		{
			Changed = false;
			return true;
		}
	}
}
