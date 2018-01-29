using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AutomaticUpdate.ViewModel
{
    class CreatXml
    {
        public static void CreateXML(Dictionary<String, String> fileInfos,string XmlPath)
        {
            //2、创建一个 xml 文档
            XmlDocument xml = new XmlDocument();
            //3、创建一行声明信息，并添加到 xml 文档顶部
            XmlDeclaration decl = xml.CreateXmlDeclaration("1.0", "utf-8", null);
            xml.AppendChild(decl);
            //4、创建根节点
            XmlElement rootEle = xml.CreateElement("UPGRADE");
            xml.AppendChild(rootEle);
            XmlElement sysconfigEle = xml.CreateElement("sysconfig");
            rootEle.AppendChild(sysconfigEle);
            //循环创建文件信息节点
            foreach (var fileInfo in fileInfos)
            {
                XmlElement fileEle = xml.CreateElement("fileInfo");
                //为节点增加属性
                fileEle.SetAttribute("file_name", fileInfo.Key);
                fileEle.SetAttribute("file_MD5", fileInfo.Value);
                sysconfigEle.AppendChild(fileEle);
            }
            xml.Save(XmlPath);
        }

    }
}
