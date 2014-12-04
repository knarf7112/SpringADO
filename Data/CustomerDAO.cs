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

namespace Data
{
    public class CustomerDAO : AdoDaoSupport,IDAO<Customer, string>
    {
        private static ILog log = LogManager.GetLogger(typeof(CustomerDAO));
        public string TableName { private get; set; }
        public void Insert(Customer customer)
        {
            //這邊塞進SQL指令 //@"insert into "+this.TableName+ @"(...)" 這邊以後的Customers用this.TableName取代，注意空白
            string cmdText = "insert into " + this.TableName + " (" +
                                 "CustomerId, FirstName, LastName)" +
                            " values (" +
                                @"@CustomerId, @FirstName, @LastName" +
                            ")";//CustomerId是欄位，@CustomerId是變數 特別注意變數前的 @
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
            string cmdText = @" update " + this.TableName +
                             @" set " + 
                                    @"FirstName=@FirstName, " + 
                                    @"LastName=@LastName" +
                             @" where " + 
                                    @"CustomerId=@CustomerId";
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
            string cmdText = "delete from " + this.TableName +
                            " where " +
                                 @"CustomerId = @CustomerId";
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
                                    this.TableName +
                            " where " +
                                    @"CustomerId = @CustomerId";
            IDbParameters dbParameters = AdoTemplate.CreateDbParameters();
            dbParameters.Add("CustomerId", DbType.AnsiString).Value = pk;
            Customer customer = null;
            try
            {
                //QueryForObject只找一筆record
                customer = AdoTemplate.QueryForObject<Customer>(
                    CommandType.Text,
                    cmdText,
                    new CustomerMapping<Customer>(),//委託給CustomerRowMapper來幫我做Mapper
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
                                        this.TableName +
                             " order by CustomerId";
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
                                "count(*) " +
                            "from " +
                                 this.TableName +
                           " where" +
                               " CustomerId = @CustomerId";
            IDbParameters dbParameters = AdoTemplate.CreateDbParameters();
            dbParameters.Add("CustomerId", DbType.AnsiString).Value = pk;

            //傳回數值，大於0表示存在，須注意在Oracle需用decimal，而不是int32
            long result = (int)AdoTemplate.ExecuteScalar(
                CommandType.Text,
                cmdText,
                dbParameters
            );
            return (result > 0);
        }
    }
}
