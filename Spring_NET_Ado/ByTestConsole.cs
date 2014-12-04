using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Spring_NET_Ado
{
    class ByTestConsole
    {
        static void Main()
        {
            
            qoo q = new qoo() {  LastName = "last", QooName = "Qoo" };
            PropertyInfo[] pi = q.GetType().GetProperties();

            object obj = pi[0].GetValue(q);//, null);//ID的值


            //私有或保護的屬性無法取得(GetValue)數據-----------------------------------------
            string s = "asdfghjkl;wqeyertyio";
            PropertyInfo p = typeof(string).GetProperty("Chars");
            
            object[] indexArgs = { 6 };
            object value = p.GetValue(null, null);//indexArgs);
            Console.WriteLine(Convert.ToString(value));
            object v3 = p.GetValue(s);
            object v2 = p.GetValue(s, null);
            Console.ReadKey();
        }
    }

    public class qoo
    {
        public int ID { get; set; }

        public String QooName { get; set; }

        public string LastName { get; set; }
    }
}
