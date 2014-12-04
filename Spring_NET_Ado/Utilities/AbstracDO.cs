#region Imports
using System;
using System.Collections.Generic;
using System.Reflection;//
using System.Xml;
#endregion
namespace Spring_NET_Ado.Utilities
{
    /// <summary>
    /// 將子類別instance內容以xml方式列出
    /// </summary>
    [Serializable]
    public abstract class AbstractDO : IComparable
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(AbstractDO))
        protected Dictionary<String, int> SkipDic = null;

        /// <summary>
        /// Empty constructor for future use
        /// </summary>
        public AbstractDO()
        {
            //
        }
        /// <summary>
        /// 指定property名稱,在列表時跳過不顯示
        /// </summary>
        /// <param name="fName">欲跳過不顯示的欄位名稱</param>
        public virtual void addSkipField(String fName)
        {
            if (SkipDic == null)
            {
                this.SkipDic = new Dictionary<string, int>();
            }
            this.SkipDic.Add(fName, 1);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            //this物件不為參數物件的實現
            if (!this.GetType().IsInstanceOfType(obj))
                return false;

            //取得此物件所有的屬性
            PropertyInfo[] pi = this.GetType().GetProperties();
            try
            {
                foreach (PropertyInfo p in pi)
                {
                    //忽略清單不為空且屬性名稱包含在忽略清單內
                    if ((this.SkipDic != null) && (this.SkipDic.ContainsKey(p.Name)))
                        continue;
                    //TODO.....p.GetValue(this, null)其實等於p.GetValue(this) 用意在取物件的XX屬性的值若沒有則傳回預設值
                    if (p.GetValue(this, null) == null && p.GetValue(obj, null) == null)
                        continue;

                    if(p.GetValue(this, null).Equals(p.GetValue(obj, null)))
                    {
                        //System.Console.WriteLine(p.Name + ":" + p.GetValue(this, null) + ":" + p.GetValue(obj, null) + ":");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// 將物件property內容組成XmlElement(遞迴組成)
        /// </summary>
        /// <returns>XmlElement</returns>
        protected virtual XmlElement GetElement()
        {
            XmlDocument xd = new XmlDocument();
            //write down the XML declaration
            //XmlDeclaration xmlDeclaration = xd.CreateXmlDeclaration("1.0","big5",null);
            //xd.InsertBefore( xmlDeclaration, xd.DocumentElement);
            XmlElement root = xd.CreateElement("", this.GetType().Name, "");
            PropertyInfo[] pi = this.GetType().GetProperties();
            try
            {
                foreach (PropertyInfo p in pi)
                {
                    XmlElement xe = xd.CreateElement("", p.Name, "");//定義元素名稱<p.Name>
                    object tmpObj = p.GetValue(this, null);//取得屬性內的值=>Customer1.FirstName = "XXX" => 取得"XXX"
                    //Console.WriteLine(p.Name + ":" + ( (tmpObj != null) ? tmpObj.GetType().ToString() : "NULL") );

                    if (tmpObj != null)
                    {
                        //如果屬性名稱有忽略清單且在忽略清單內
                        if((this.SkipDic != null) && (this.SkipDic.ContainsKey(p.Name)))
                        {
                            xe.InnerText = "Skipped!";
                        }
                        else if (tmpObj is AbstractDO)
                        {
                            //xe.InnerText = "AbstractDO";
                            //遞迴方式插入所有的節點(XmlNode)
                            xe.AppendChild(xd.ImportNode(((AbstractDO)tmpObj).GetElement(), true));
                        }
                        else
                        {
                            //System.Console.WriteLine(tmpObj.GetType());
                            xe.InnerText = tmpObj.ToString();
                        }
                    }
                    root.AppendChild(xe);
                }
            }
            catch (Exception)
            {
                return null;
            }
            return root;
        }
        public override string ToString()
        {
            //log.Info("Beging tostring...");
            XmlElement xd = this.GetElement();
            System.IO.StringWriter sw = new System.IO.StringWriter();
            using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(sw))
            {
                writer.Indentation = 2;//縮排
                writer.Formatting = Formatting.Indented;
                //
                xd.WriteTo(writer);//將xd取得的XmlElement寫入到writer內
                writer.Close();
            }
            // 輸出格式化的XML
            string res = sw.ToString();
            sw.Close();
            return "\n" + res;
        }
        /// <summary>
        /// 傳出該 Dependent Object之排序字串值
        /// </summary>
        /// <returns>Dependent Object 之排序字串</returns>
        public virtual string getSortKey()
        {
            PropertyInfo p = (this.GetType().GetProperties())[0];
            return p.GetValue(this, null).ToString();
        }
        /// <summary>
        /// 根據排序字串比較該Dependent Obejct與傳入物件之大小
        /// </summary>
        /// <param name="obj">傳入物件,須為相同之Dependent Object</param>
        /// <returns>int 傳回值 負值:本物件較小; 0:兩物件相同; 正值:本物件較大</returns>
        public int CompareTo(object obj)
        {
            //檢查obj物件的Type是否跟目前的一樣
            if ((obj != null) && (this.GetType().IsInstanceOfType(obj)))
            {
                AbstractDO dst = obj as AbstractDO;//cast
                return this.getSortKey().CompareTo(dst.getSortKey());
            }
            else
            {
                throw new Exception(
                        "傳入物件型別錯誤,無法比較: " +
                        "parameter class " +
                         this.GetType().Name +
                         " is not the same of my class " +
                        obj.GetType().Name
                        );
            }
        }
    }
}
