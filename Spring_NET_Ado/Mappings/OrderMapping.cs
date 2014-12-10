using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using Spring.Data;
using Spring;
using Spring_NET_Ado.Entities;
using Spring.Data.Generic;
namespace Spring_NET_Ado.Mappings
{
    public class OrderMapping<T> : IRowMapper<T> where T : Order,new()
    {

        public T MapRow(System.Data.IDataReader reader, int rowNum)
        {
            T order = new T();
            order.OrderId = reader.GetString(0);
            order.OrderDate = reader.GetString(1);
            order.CustomerId = reader.GetString(2);
            return order;
        }
    }
}
