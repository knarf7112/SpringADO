using System;
using System.Collections.Generic;
//
using Spring.Data;
using System.Data;
using Spring.Data.Generic;
using Spring_NET_Ado.Mappings;
using Spring_NET_Ado;
using Common.Logging;
using Spring.Data.Common;
using Spring_NET_Ado.Entities;
using System.Text.RegularExpressions;

namespace Data
{
    public class CustomerDAO : AdoDaoSupport,IDAO<Customer, string>
    {
        private static ILog log = LogManager.GetLogger(typeof(CustomerDAO));
        public string BindPrefix { get; set; }
        public void Insert(Customer customer)
        {
            //這邊塞進SQL指令 //@"insert into "+this.TableName+ @"(...)" 這邊以後的Customers用this.TableName取代，注意空白
            string cmdText = "insert into " +
                                 "Customers " +
                                 "(CustomerId, FirstName, LastName) " +
                             "values " +
                                @"(%BIND_PRE%CustomerId, %BIND_PRE%FirstName, %BIND_PRE%LastName)";//CustomerId是欄位，@CustomerId是變數 特別注意變數前的 @
            cmdText = Regex.Replace(cmdText, @"%BIND_PRE%", this.BindPrefix);
            IDbParameters dbParameters = AdoTemplate.CreateDbParameters();
            dbParameters.Add("CustomerId", DbType.AnsiString).Value = customer.CustomerId;
            dbParameters.Add("FirstName", DbType.AnsiString).Value = customer.FirstName;
            dbParameters.Add("LastName", DbType.AnsiString).Value = customer.LastName;
            AdoTemplate.ExecuteNonQuery(//執行insert //這邊的例外是確保如果SQL亂寫，SQL不會被執行
                CommandType.Text,
                cmdText,
                dbParameters
            );
        }

        public void Update(Customer customer)
        {
            string cmdText = "update " + 
                                     "Customers " +
                             "set " +
                                    @"FirstName=%BIND_PRE%FirstName, " +
                                    @"LastName=%BIND_PRE%LastName " +
                             "where " +
                                    @"CustomerId=%BIND_PRE%CustomerId";
            cmdText = Regex.Replace(cmdText, @"%BIND_PRE%", this.BindPrefix);

            IDbParameters dbParameters = AdoTemplate.CreateDbParameters();//須注意Oracle要照欄位順序來列
            dbParameters.Add("CustomerId", DbType.AnsiString).Value = customer.CustomerId;
            dbParameters.Add("FirstName", DbType.AnsiString).Value = customer.FirstName;
            dbParameters.Add("LastName", DbType.AnsiString).Value = customer.LastName;
            AdoTemplate.ExecuteNonQuery(
                CommandType.Text,
                cmdText,
                dbParameters
            );
        }

        public void Delete(string pk)
        {
            string cmdText = "delete from " +
                                  "Customers " +
                             "where " +
                                 @"CustomerId = %BIND_PRE%CustomerId";
            cmdText = Regex.Replace(cmdText, @"%BIND_PRE%", this.BindPrefix);

            IDbParameters dbParameters = AdoTemplate.CreateDbParameters();
            dbParameters.Add("CustomerId", DbType.AnsiString).Value = pk;
            AdoTemplate.ExecuteNonQuery(
                CommandType.Text,
                cmdText,
                dbParameters
                );
        }

        public Customer FindByPK(string pk)
        {
            //須注意select要照順序 //select的順序要跟Mapper一致，且PK一定要有
            string cmdText = "select " +
                                 "CustomerId, FirstName, LastName " +
                             "from " +
                                    "Customers " +
                             "where " +
                                   @"CustomerId = %BIND_PRE%CustomerId";
            cmdText = Regex.Replace(cmdText, @"%BIND_PRE%", this.BindPrefix);

            IDbParameters dbParameters = AdoTemplate.CreateDbParameters();
            dbParameters.Add("CustomerId", DbType.AnsiString).Value = pk;
            Customer customer = null;
            try
            {
                //QueryForObject只找一筆record
                customer = AdoTemplate.QueryForObject<Customer>(
                    CommandType.Text,
                    cmdText,
                    new CustomerMapping<Customer>(),//委託給CustomerRowMapper來幫做Mapper
                    dbParameters
                    );
            }
            catch (Exception ex)//如果找不到(null)
            {
                log.Debug(ex.StackTrace);
                log.Debug(ex.Message);
                customer = null;
            }
            return customer;
        }

        //每個欄位都撈，回傳一個List
        public IList<Customer> FindAll()
        {
            //select的東西都是DB欄位(大寫)
            string cmdText = "select " +
                                  "CustomerId, FirstName, LastName " +
                              "from " +
                                  "Customers " +
                              "order by CustomerId";
            //把整個欄位都轉為POCO的List，這個很常用
            return AdoTemplate.QueryWithRowMapper<Customer>(
                CommandType.Text,
                cmdText,
                new CustomerMapping<Customer>()
                );
        }

        public bool Exist(string pk)
        {
            string cmdText = "select " +
                                @"CustomerId " +
                             "from " +
                                @"Customers " +
                             "where " +
                                @"CustomerId = %BIND_PRE%CustomerId";
            cmdText = Regex.Replace(cmdText, @"%BIND_PRE%", this.BindPrefix);

            IDbParameters dbParameters = AdoTemplate.CreateDbParameters();
            dbParameters.Add("CustomerId", DbType.AnsiString).Value = pk;
            try
            {
                //Query取得CustomerId,如果沒有則回傳null(先利用客戶帳號去資料庫select有無此帳號再來回比對一次傳入參數,感覺檢查過當)
                string resultId = AdoTemplate.QueryForObjectDelegate<string>(
                    CommandType.Text,
                    cmdText,
                    delegate(IDataReader idr, int rowNo)
                    {
                        return idr.GetString(0);//使用委派RowMapper方法取得回傳的CustomerId
                    },
                    dbParameters
                    );
                return pk.Equals(resultId);//比較
            }
            catch
            {
                return false;
            }
        }
    }
}
