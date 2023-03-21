using System.Xml;

namespace Girl.HierArch
{
    /// <summary>
    /// ここにクラスの説明を書きます。
    /// </summary>
    public class HAClassNode : HATreeNode
    {
        public const string ext = "hacls";
        public HAFuncNode Body;

        public override void Init()
        {
            base.Init();
            Body = new HAFuncNode();
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public HAClassNode()
        {
        }

        public HAClassNode(string text)
        {
            Text = text;
        }

        public override string XmlName => "HAClass";

        public override HATreeNode NewNode => new HAClassNode();

        #region XML

        public override void WriteXml(XmlTextWriter xw)
        {
            base.WriteXml(xw);
            xw.WriteStartElement("Body");
            Body.ToXml(xw);
            xw.WriteEndElement();
        }

        public override void ReadXmlNode(XmlTextReader xr)
        {
            if (xr.Name == "Body" && xr.NodeType == XmlNodeType.Element && !xr.IsEmptyElement)
            {
                while (xr.Read() && xr.NodeType == XmlNodeType.Whitespace)
                {
                    ;
                }

                if (xr.Name == "HAFunc" && xr.NodeType == XmlNodeType.Element)
                {
                    Body.FromXml(xr);
                }
            }
        }

        public void FromHds(XmlTextReader xr)
        {
            Type = HAType.Text;
            if (xr.Name != "hds" || xr.NodeType != XmlNodeType.Element || xr.IsEmptyElement)
            {
                return;
            }

            HAFuncNode n;
            while (xr.Read())
            {
                if (xr.Name == "node" && xr.NodeType == XmlNodeType.Element)
                {
                    n = new HAFuncNode();
                    _ = Body.Nodes.Add(n);
                    n.FromHds(xr);
                }
                else if (xr.Name == "hds" && xr.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
            }
        }

        #endregion
    }
}
