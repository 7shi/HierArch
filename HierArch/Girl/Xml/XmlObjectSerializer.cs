using System.Collections;
using System.IO;
using System.Xml;

namespace Girl.Xml
{
    /// <summary>
    /// ここにクラスの説明を書きます。
    /// </summary>
    public class XmlObjectSerializer
    {
        private static readonly Hashtable resources = new Hashtable();

        public virtual void Write(XmlWriter xw, string name, object obj)
        {
            if (obj == null)
            {
                return;
            }

            string objname = obj.GetType().Name;
            if (name == null)
            {
                name = objname;
            }

            switch (objname)
            {
                case "Boolean":
                    WriteBoolean(xw, name, (bool)obj);
                    break;
            }
        }

        public void WriteBoolean(XmlWriter xw, string name, bool b)
        {
            xw.WriteStartElement(name);
            xw.WriteAttributeString("Value", XmlConvert.ToString(b));
            xw.WriteEndElement();
        }

        public virtual object Read(XmlReader xr)
        {
            if (xr.NodeType != XmlNodeType.Element)
            {
                return null;
            }

            switch (xr.Name)
            {
                case "Boolean":
                    return ReadBoolean(xr);
            }
            return null;
        }

        public bool ReadBoolean(XmlReader xr)
        {
            return XmlConvert.ToBoolean(xr.GetAttribute("Value"));
        }

        public bool ReadNext(XmlReader xr)
        {
            while (xr.Read())
            {
                if (xr.NodeType != XmlNodeType.Whitespace)
                {
                    return true;
                }
            }
            return false;
        }

        public string GetXml(object obj)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            Write(xw, null, obj);
            xw.Close();
            sw.Close();
            return sw.ToString();
        }
    }
}
