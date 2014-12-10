using System;
using System.Collections.Generic;
//
using Spring.Data.Common;
using Spring.Data.Generic;
using Spring_NET_Ado.Entities;
using Common.Logging;
using System.Text.RegularExpressions;
using System.Data;
using Spring_NET_Ado.Mappings;


namespace Data
{
    /// <summary>
    /// 列舉訂單的欄位名稱
    /// </summary>
    public enum OrderField
    {
        OrderId, OrderDate, CustomerId
    }
    public class OrderDAO : AdoDaoSupport,IDAO2<Order,string,string>
    {
       
        private static ILog log = LogManager.GetLogger(typeof(OrderDAO));
        private string BindPrefix { get; set; }

        public void Insert(Order order)
        {
            string cmdText = "insert into Orders " +
                                 "(OrderId, OrderDate, CustomerId) " +
                             "values " +
                             "(%BIND_PRE%OrderId,%BIND_PRE%OrderDate, %BIND_PRE%CustomerId)";
            //%BIND_PRE%用此方法可以跨到Oracle,因為Oracle使用的參數標記為:
            //只要更改Spring設定檔(App.config)內的宣告
            cmdText = Regex.Replace(cmdText, @"%BIND_PRE%", this.BindPrefix);

            IDbParameters dbParameters = AdoTemplate.CreateDbParameters();
            dbParameters.Add("OrderId", DbType.AnsiString).Value = order.OrderId;
            dbParameters.Add("OrderDate", DbType.AnsiString).Value = order.OrderDate;
            dbParameters.Add("CustomerId", DbType.AnsiString).Value = order.CustomerId;

            AdoTemplate.ExecuteNonQuery(
                CommandType.Text,
                cmdText,
                dbParameters
                );

        }

        public void Update(Order order)
        {
            string cmdText = "update " +
                                    "Orders " +
                             "set " +
                                    "OrderDate = %BIND_PRE%OrderDate," +
                                    "CustomerId = %BIND_PRE%CustomerId " +
                             "where " +
                                    "OrderId = %BIND_PRE%OrderId";
            cmdText = Regex.Replace(cmdText, @"%BIND_PRE%", this.BindPrefix);

            IDbParameters dbParameters = AdoTemplate.CreateDbParameters();
            dbParameters.Add("OrderDate", DbType.AnsiString).Value = order.OrderDate;
            dbParameters.Add("CustomerId", DbType.AnsiString).Value = order.CustomerId;
            dbParameters.Add("OrderId", DbType.AnsiString).Value = order.OrderId;
            AdoTemplate.ExecuteNonQuery(
                CommandType.Text,
                cmdText,
                dbParameters
                );
        }

        public void Delete(string pk)
        {
            string cmdText = "delete from Orders " +
                             "where " +
                             "OrderId = %BIND_PRE%OrderId";
            cmdText = Regex.Replace(cmdText, @"%BIND_PRE%", this.BindPrefix);

            IDbParameters dbParameters = AdoTemplate.CreateDbParameters();
            dbParameters.Add("OrderId", DbType.AnsiString).Value = pk;

            AdoTemplate.ExecuteNonQuery(
                CommandType.Text,
                cmdText,
                dbParameters
                );
        }



        public Order FindByPK(string pk)
        {
            string cmdText = "select " +
                                "OrderId, OrderDate, CustomerId " +
                             "from " +
                                "Orders " +
                             "where " +
                                "OrderId = %BIND_PRE%OrderId";
            cmdText = Regex.Replace(cmdText, @"%BIND_PRE%", this.BindPrefix);

            IDbParameters dbParameters = AdoTemplate.CreateDbParameters();
            dbParameters.Add("OrderId", DbType.AnsiString).Value = pk;

            Order order = null;
            try
            {
                order = AdoTemplate.QueryForObject<Order>(
                    CommandType.Text,
                    cmdText,
                    new OrderMapping<Order>(),
                    dbParameters
                );
            }
            catch
            {
                order = null;
            }
            return order;
        }

        public IList<Order> FindAll()
        {
            string cmdText = "select " +
                                "OrderId, OrderDate, CustomerId " +
                             "from " +
                                "Orders " +
                             "order by OrderId";
            return AdoTemplate.QueryWithRowMapper<Order>(
                CommandType.Text,
                cmdText,
                new OrderMapping<Order>()
                );
        }

        public bool Exist(string pk)
        {
            string cmdText = "select " +
                                "OrderId " +
                            "from " +
                                "Orders " +
                            "where " +
                                "OrderId = %BIND_PRE%OrderId";
            cmdText = Regex.Replace(cmdText, @"%BIND_PRE%", this.BindPrefix);

            IDbParameters dbParameters = AdoTemplate.CreateDbParameters();
            dbParameters.Add("OrderId", DbType.AnsiString).Value = pk;
            try
            {
                string result = AdoTemplate.QueryForObjectDelegate<string>
                (
                    CommandType.Text,
                    cmdText,
                    delegate(IDataReader reader, int rowNo)
                    {
                        return reader.GetString(0);
                    },
                    dbParameters
                );
                return pk.Equals(result);
            }
            catch
            {
                return false;
            }
        }

        public IList<Order> ListCustomerOrders(string customerId)
        {
            string cmdText = "select " +
                                "OrderId, OrderDate, CustomerId " +
                             "from " +
                                "Orders " +
                             "where " +
                                "CustomerId = %BIND_PRE%CustomerId " +
                             "order by OrderId";
            cmdText = Regex.Replace(cmdText, @"%BIND_PRE%", this.BindPrefix);

            IDbParameters dbParameters = AdoTemplate.CreateDbParameters();
            dbParameters.Add("CustomerId", DbType.AnsiString).Value = customerId;
            //Order r2 = AdoTemplate.QueryForObjectDelegate<Order>(
            //    CommandType.Text,
            //    cmdText,
            //    delegate(IDataReader reader, int i)
            //    {
            //        Order c = new Order();
            //        c.OrderId = reader.GetString(0);
            //        return c;
            //    },
            //    dbParameters
            //    );
            return AdoTemplate.QueryWithRowMapper<Order>(
                CommandType.Text,
                cmdText,
                new OrderMapping<Order>(),
                dbParameters
            );
        }

        /// <summary>
        /// 輸入要搜尋的欄位名稱,條件值,找訂單
        /// </summary>
        /// <param name="t">表示要搜尋的欄位名稱</param>
        /// <param name="pk">表示要搜尋的數據</param>
        /// <returns></returns>
        public IList<Order> FindByT(string fieldName, string fieldValue)
        {
            string cmdText = "select " +
                                "OrderId, OrderDate, CustomerId " +
                             "from " +
                                "Orders " +
                             "where " +
                                "%BIND_PRE%T = %BIND_PRE%CustomerId " +
                             "order by OrderId";
            cmdText = Regex.Replace(cmdText, @"%BIND_PRE%", this.BindPrefix);

            IDbParameters dbParameters = AdoTemplate.CreateDbParameters();
            bool checkField = false;
            //檢查輸入欄位名稱
            foreach(string ch in Enum.GetNames(typeof(OrderField)))
            {
                if (ch == fieldName)
                {
                    checkField = true;
                    break;
                }
            }
            if (checkField)
            {
                dbParameters.Add("T", DbType.AnsiString).Value = fieldName;
                dbParameters.Add("CustomerId", DbType.AnsiString).Value = fieldValue;
            }
            else
            {
                throw new Exception(" 輸入的欄位名稱錯誤 :" + fieldName);
            }
            return AdoTemplate.QueryWithRowMapper<Order>(
                CommandType.Text,
                cmdText,
                new OrderMapping<Order>(),
                dbParameters
            );
        }
    }
}
