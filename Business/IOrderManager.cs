using Spring_NET_Ado.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IOrderManager
    {
        void Insert(Order order);//插入一筆訂單
        void InsertOrders(IList<Order> orders);//插入多筆訂單
        void InsertCustomerAndOrders(Customer customer, IList<Order> cList);//插入一筆新客戶與新客戶的訂單
        IList<Order> ListCustomerOrders(string customerId);//找某客戶的所有訂單
        Order FindByPK(string orderId);//找某訂單
        bool Exist(string orderId);//檢查訂單是否存在
        void Delete(string orderId);//刪除訂單
        void DeleteByCustomerId(string CustomerId);//刪除某客戶的所有訂單
    }
}
