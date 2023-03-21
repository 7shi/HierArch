using Girl.Windows.Forms;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Girl.HierArch
{
    /// <summary>
    /// ここにクラスの説明を書きます。
    /// </summary>
    public class HADoc : Document
    {
        public HAClass ClassTreeView;
        public HAViewInfo ViewInfo;

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public HADoc()
        {
            ClassTreeView = null;
            ViewInfo = new HAViewInfo();
        }

        public string ShortName
        {
            get
            {
                string ret = Name;
                int p = ret.LastIndexOf('.');
                return p < 0 ? ret : ret.Substring(0, p);
            }
        }

        /// <summary>
        /// サロゲートペアを 3 バイト × 2 で符号化した UTF-8 の変種から変換 
        /// </summary>
        public static string FromCESU8(byte[] bytes)
        {
            StringBuilder s = new StringBuilder();
            int len = bytes.Length;
            if (len >= 3 && bytes[0] == 0xef && bytes[1] == 0xbb && bytes[2] == 0xbf)
            { // UTF-8 BOM
                return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
            }
            for (int i = 0; i < len; i++)
            {
                int b1 = bytes[i], b2, b3, b4;
                if (b1 < 0x80)
                {
                    _ = s.Append((char)b1);
                }
                else if ((b1 & 0xe0) == 0xc0 && i + 1 < len &&
                         ((b2 = bytes[i + 1]) & 0xc0) == 0x80)
                {
                    // C0-DF | 80-BF
                    int ch = ((b1 & 0x1f) << 6) | (b2 & 0x3f);
                    _ = ch < 0x80 ? s.Append("��") : s.Append((char)ch);

                    i++;
                }
                else if ((b1 & 0xf0) == 0xe0 && i + 2 < len &&
                         ((b2 = bytes[i + 1]) & 0xc0) == 0x80 &&
                         ((b3 = bytes[i + 2]) & 0xc0) == 0x80)
                {
                    // E0-EF | 80-BF | 80-BF
                    int ch = ((b1 & 0xf) << 12) | ((b2 & 0x3f) << 6) | (b3 & 0x3f);
                    _ = ch < 0x800 ? s.Append("���") : s.Append((char)ch);

                    i += 2;
                }
                else if ((b1 & 0xf8) == 0xf0 && i + 3 < len &&
                         ((b2 = bytes[i + 1]) & 0xc0) == 0x80 &&
                         ((b3 = bytes[i + 2]) & 0xc0) == 0x80 &&
                         ((b4 = bytes[i + 3]) & 0xc0) == 0x80)
                {
                    // F0-F4 | 80-BF | 80-BF | 80-BF
                    int ch = ((b1 & 7) << 18) | ((b2 & 0x3f) << 12) | ((b3 & 0x3f) << 6) | (b4 & 0x3f);
                    if (ch < 0x10000 || ch >= 0x110000)
                    {
                        _ = s.Append("����");
                    }
                    else
                    {
                        _ = s.Append((char)(0xd800 + ((ch - 0x10000) >> 10)));
                        _ = s.Append((char)(0xdc00 + (ch & 0x3ff)));
                    }
                    i += 3;
                }
                else
                {
                    _ = s.Append("�");
                }
            }
            return s.ToString();
        }

        public override bool Open()
        {
            XmlTextReader xr;
            HAClassNode n;

            if (!File.Exists(FullName))
            {
                return false;
            }

            try
            {
                string ext = Path.GetExtension(FullName).ToLower();
                if (ext == ".hds")
                {
                    byte[] bytes = File.ReadAllBytes(FullName);
                    StringReader sr = new StringReader(FromCESU8(bytes));
                    xr = new XmlTextReader(sr);
                }
                else
                {
                    xr = new XmlTextReader(FullName);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            while (xr.Read())
            {
                if (xr.Name == "HAViewInfo" && xr.NodeType == XmlNodeType.Element)
                {
                    ViewInfo = new XmlSerializer(typeof(HAViewInfo)).Deserialize(xr) as HAViewInfo;
                }
                else if (xr.Name == "HAClass" && xr.NodeType == XmlNodeType.Element)
                {
                    n = new HAClassNode();
                    _ = ClassTreeView.Nodes.Add(n);
                    n.FromXml(xr);
                }
                else if (xr.Name == "HAProject" && xr.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                else if (xr.Name == "hds" && xr.NodeType == XmlNodeType.Element)
                {
                    n = new HAClassNode
                    {
                        Text = ShortName
                    };
                    ClassTreeView.InitNode(n);
                    n.Body.Nodes.Clear();
                    _ = ClassTreeView.Nodes.Add(n);
                    n.FromHds(xr);
                    ViewInfo.InitHds();
                }
                else if (xr.Name == "hds" && xr.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
            }
            xr.Close();
            ClassTreeView.ApplyState();
            if (ClassTreeView.SelectedNode == null && ClassTreeView.Nodes.Count > 0)
            {
                ClassTreeView.SelectedNode = ClassTreeView.Nodes[0];
            }
            Changed = false;
            return true;
        }

        public override bool Save()
        {
            ClassTreeView.StoreData();
            bool ret = false;
            string lfn = FullName.ToLower();
            if (lfn.EndsWith(".hadoc"))
            {
                ret = SaveHAPrj();
            }
            else if (lfn.EndsWith(".hds"))
            {
                ret = SaveHds();
            }
            else
            {
                _ = MessageBox.Show("保存できないファイルの種類です。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (ret)
            {
                Changed = false;
            }

            return ret;
        }

        private void replaceFile(string tmp)
        {
            string bak = FullName + ".bak";
            bool exists = File.Exists(FullName);
            if (exists)
            {
                File.Move(FullName, bak);
            }

            File.Move(tmp, FullName);
            if (exists)
            {
                File.Delete(bak);
            }
        }

        private bool SaveHAPrj()
        {
            try
            {
                string tmp = FullName + ".tmp";
                using (XmlTextWriter xw = new XmlTextWriter(tmp, new UTF8Encoding(false)))
                {
                    xw.Formatting = Formatting.Indented;
                    xw.Indentation = 0;
                    xw.WriteStartDocument();
                    xw.WriteStartElement("HAProject");
                    xw.WriteAttributeString("version", Application.ProductVersion);
                    XmlSerializer xs = new XmlSerializer(typeof(HAViewInfo));
                    xs.Serialize(xw, ViewInfo);
                    foreach (TreeNode n in ClassTreeView.Nodes)
                    {
                        (n as HAClassNode).ToXml(xw);
                    }
                    xw.WriteEndElement();
                    xw.WriteEndDocument();
                }
                replaceFile(tmp);
                return true;
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool SaveHds()
        {
            if (!(ClassTreeView.SelectedNode is HAClassNode n))
            {
                _ = MessageBox.Show("クラスが選択されていません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            string msg = "HDS 形式では現在開かれている第一階層だけが保存されます。";
            if (MessageBox.Show(msg, "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
            {
                return false;
            }
            try
            {
                string tmp = FullName + ".tmp";
                using (StreamWriter sw = new StreamWriter(tmp))
                using (XmlTextWriter xw = new XmlTextWriter(sw))
                {
                    sw.NewLine = "\n";
                    xw.Formatting = Formatting.Indented;
                    xw.Indentation = 0;
                    xw.WriteStartDocument();
                    xw.WriteStartElement("hds");
                    xw.WriteAttributeString("version", "0.3.5");
                    foreach (TreeNode nn in n.Body.Nodes)
                    {
                        (nn as HAFuncNode).ToHds(xw);
                    }
                    xw.WriteEndElement();
                    xw.WriteEndDocument();
                }
                replaceFile(tmp);
                return true;
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
