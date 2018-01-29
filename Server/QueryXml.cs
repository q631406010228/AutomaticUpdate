using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server
{
    class QueryXml
    {
        public static List<DeTailFile> QueryXmlMain(String ClientPath)
        {
            List<DeTailFile> listdf = new List<DeTailFile>();
            XDocument addList = XDocument.Load(ClientPath);
            var text = from v in addList.Descendants("sysconfig")
                       from x in v.Elements()
                       select x;        
            foreach (var node in text)
            {
                DeTailFile df = new DeTailFile
                {
                    Name = node.Attribute("file_name").Value,
                    FileMD5 = node.Attribute("file_MD5").Value
                };
                listdf.Add(df);
            }
            return listdf;
        }
    }
}
