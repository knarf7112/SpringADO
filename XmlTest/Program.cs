using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlTest
{
    class Program
    {
        
        enum Test
        {
            OrderId , OrderDate, CustomerId
        }
        static void Main(string[] args)
        {

            //System.Collections.Specialized.NameValueCollection e = new System.Collections.Specialized.NameValueCollection();
            //Spring.Data.Support.NullMappingDataReader d = new Spring.Data.Support.NullMappingDataReader();
            //AdoTemplate.DataReaderWrapperType = Type.GetType("Spring.Data.Support.NullMappingDataReader");
            //
            try
            {
                int x = 1;
                int y = 0;
                int z = x / y;
            }
            catch
            {
                throw;
            }
            //--------------
            Console.WriteLine(Test.CustomerId);
            foreach (string ch in Enum.GetNames(typeof(Test)))
            {
                Console.WriteLine(ch.ToString());//確定列舉型別輸出的文字(列舉轉字串)
            }
            Test qoo;
            var ss = Enum.TryParse<Test>("OrderId", false,out qoo);
            Console.WriteLine(ss.ToString());
            Console.ReadKey();
            //-----------------------------------------------
            A a1 = new A() { a = "a", b = 1 };
            A a2 = new A() { a = "a", b = 1 };
            Console.WriteLine(a1.Equals(a2));
            Console.WriteLine(a1 == a2);
            //建立XmlDocument.
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<bookstore>" +
                        "<book genre='novel' ISBN='1-861001-57-5'>" +
                        "<title>Qoo Test</title>" +
                        "</book>" +
                        "</bookstore>");
            //建立另一個XmlDocument載入xml檔
            XmlDocument doc2 = new XmlDocument();
            doc2.Load("books.xml");

            //載入另一個Xml的最後一個節點的數據(XmlNode)
            XmlNode newBook = doc.ImportNode(doc2.DocumentElement.LastChild, true);
            doc.DocumentElement.AppendChild(newBook);

            Console.WriteLine("Display the modified XML...");
            doc.Save(Console.Out);//儲存到控制台上
            Console.ReadKey();
        }

        class A
        {
            public string a { get; set; }
            public int b { get; set; }

            public override string ToString()
            {
                PropertyInfo[] pi = this.GetType().GetProperties();
                string tmpObj = null;
                foreach(PropertyInfo p in pi){
                    object obj = p.GetValue(this);
                    tmpObj += obj.ToString() + ":";
                }
                return tmpObj;
            }
            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if (!this.GetType().IsInstanceOfType(obj)) return false;

                PropertyInfo[] pi = this.GetType().GetProperties();
                PropertyInfo[] obj2 = obj.GetType().GetProperties();
                foreach (PropertyInfo p in pi)
                {
                    foreach (PropertyInfo p2 in obj2)
                    {
                        //如果屬性名稱一樣但屬性值不一樣表示不一樣
                        if (p.Name == p2.Name )
                            if(p.GetValue(this).ToString() != p2.GetValue(this).ToString())
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
    }
}
