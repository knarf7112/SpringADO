using Spring.Data.Generic;
using Spring_NET_Ado.Entities;
namespace Spring_NET_Ado.Mappings
{
    //IRowMapper<T>為Spring.Data的介面,他會Mapping資料庫帶出的資料利用DataReader,再用自定義的物件去接住資料並傳出物件
    //自定義的物件在後面宣告,會自動帶入到T,所以T型別會被辨識成Customer
    public class CustomerMapping<T> :IRowMapper<T> where T:Customer,new()
    {
        //把DB中撈出的東西照select順序和型別帶入POCO(即物件)
        public T MapRow(System.Data.IDataReader reader, int rowNum)
        {
            T customer = new T();
            customer.CustomerId = reader.GetString(0);
            customer.FirstName = reader.GetString(1);
            customer.LastName = reader.GetString(2);
            return customer;
        }
    }
}
