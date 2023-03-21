using Girl.Xml;
using System.Drawing.Printing;
using System.IO;

namespace Girl.Windows.Forms
{
    /// <summary>
    /// 文書を管理するクラスです。
    /// </summary>
    public class Document
    {
        public string FullName;
        public bool Changed;
        public PrintDocument PrintDocument;
        public XmlObjectSerializer Serializer;

        protected virtual void Init()
        {
            Changed = false;
            PrintDocument = new PrintDocument();
            Serializer = NewSerializer;
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public Document()
        {
            FullName = "";
            Init();
        }

        public string Name => FullName == "" ? "無題" : Path.GetFileNameWithoutExtension(FullName);

        protected virtual XmlObjectSerializer NewSerializer => new XmlObjectSerializer();

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
