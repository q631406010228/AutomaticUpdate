using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutomaticUpdate.ViewModel
{
    class XmlDiff
    {
        public static void XmlDiffMain()
        {
            XElement xe = XElement.Load("C:\\Users\\秦\\Pictures\\Client.xml");
            Console.WriteLine(xe);
            IEnumerable<XElement> elements = from ele in xe.Elements("fileInfo") select ele;
            ShowInfoByElements(elements);
        }

        private static void ShowInfoByElements(IEnumerable<XElement> elements)
        {
            foreach (var ele in elements)
            {
                Console.WriteLine(ele.Attribute("file_name").Value);
                Console.WriteLine(11);
            }
        }
    }
}
