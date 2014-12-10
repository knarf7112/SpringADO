using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using Spring_NET_Ado.Entities;
using Common.Logging;
using Data;
namespace Business
{
    public class OrderManager :IOrderManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OrderManager));

        // Inject DAO From Config
        public OrderDAO OrderDAO { get; set; }
        public ICustomerManager CustomerManager { get; set; }

        
        public void Insert(Order order)
        {
            if (!this.CustomerManager.Exist(order.CustomerId))
            {
                throw new Exception("Customer: " + order.CustomerId + " not exist!");
            }
            if (this.OrderDAO.Exist(order.OrderId))
            {
                throw new Exception("Order: " + order.OrderId + " exist!");
            }
            this.OrderDAO.Insert(order);
        }

        public void InsertOrders(IList<Order> orders)
        {
            foreach (Order o in orders)
            {
                if (!this.CustomerManager.Exist(o.CustomerId))
                {
                    throw new Exception("客戶 " + o.CustomerId + " 不存在 ==> 回復至先前狀態!");
                }
                else if (this.OrderDAO.Exist(o.OrderId))
                {
                    throw new Exception("訂單編號重複 ==> 回復至先前狀態!");
                }
                else
                {
                    this.OrderDAO.Insert(o);
                }
            }
        }

        public void InsertCustomerAndOrders(Customer customer, IList<Order> cList)
        {
            //先新增客戶資料
            this.CustomerManager.Insert(customer);
            //
            foreach (Order order in cList)
            {
                order.CustomerId = customer.CustomerId;//客戶物件的客戶編號 ==> 訂單的客戶編號欄位
                if (this.OrderDAO.Exist(order.OrderId))
                {
                    throw new Exception("訂單編號重複: " + order.OrderId + " ==> 回復至先前狀態");
                }
                this.OrderDAO.Insert(order);
            }
        }
        /// <summary>
        /// 列舉所有此客戶編號的訂單
        /// </summary>
        /// <param name="customerId">訂單的客戶編號</param>
        /// <returns>此客戶的訂單集合</returns>
        public IList<Order> ListCustomerOrders(string customerId)
        {
            if (!this.CustomerManager.Exist(customerId))
            {
                throw new Exception("客戶 " + customerId + " 不存在!");
            }
            IList<Order> customerOrderList = this.OrderDAO.ListCustomerOrders(customerId);
            if (customerOrderList.Count > 0)
            {
                return customerOrderList;
            }
            else
            {
                return null;
            }
        }

        public Order FindByPK(string orderId)
        {
            if (!this.OrderDAO.Exist(orderId))
            {
                throw new Exception("OrderId " + orderId + "Not Exist!");
            }
            return this.OrderDAO.FindByPK(orderId);
        }

        public bool Exist(string orderId)
        {
            return this.OrderDAO.Exist(orderId);
        }

        public void Delete(string orderId)
        {
            if (!this.OrderDAO.Exist(orderId))
            {
                throw new Exception("OrderId " + orderId + "Not Exist!");
            }
            this.OrderDAO.Delete(orderId);
        }
        /// <summary>
        /// 刪除此客戶的所有訂單
        /// </summary>
        /// <param name="CustomerId">此訂單的客戶編號</param>
        public void DeleteByCustomerId(string CustomerId)
        {
            IList<Order> deleteList = this.ListCustomerOrders(CustomerId);
            if (deleteList.Count > 0)
            {
                foreach (Order order in deleteList)
                {
                    this.Delete(order.OrderId);
                    log.Debug("刪除訂單 " + order.OrderId + " 成功!");
                }
            }
            else
            {
                throw new Exception("此客戶無訂單可刪除 : " + CustomerId);
            }
        }
    }
}
