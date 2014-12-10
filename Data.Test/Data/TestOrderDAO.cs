using System;
using System.Collections.Generic;
using System.Linq;
//
using Spring.Context;
using Spring.Context.Support;
using Common.Logging;
//
using NUnit.Framework;
//
using Data;
using Spring_NET_Ado.Entities;

namespace Data.Test.Data
{
    [TestFixture]
    class TestOrderDAO
    {
        #region Field
        private static ILog log = LogManager.GetLogger(typeof(TestOrderDAO));
        private IApplicationContext ctx;//spring container
        private CustomerDAO customerDAO;//customer manager
        private OrderDAO orderDAO;//order manager
        private IDictionary<string, Customer> customerDic;//expected data collection
        private IDictionary<string, Order> orderDic;//expected data collection
        #endregion
        [SetUp]
        public void InitContext()
        {
            this.ctx = ContextRegistry.GetContext();
            this.orderDAO = ctx["orderDAO"] as OrderDAO;
            this.customerDAO = ctx["customerDAO"] as CustomerDAO;
            this.orderDic = new Dictionary<string, Order>();
            this.customerDic = new Dictionary<string, Customer>();
            
            //prepare customers
            Customer customer = new Customer() 
            {
                CustomerId = "C001",
                FirstName = "釋",
                LastName = "迦"
            };
            this.customerDic.Add(customer.CustomerId,customer);

            customer = new Customer()
            {
                CustomerId = "C002",
                FirstName = "蓮",
                LastName = "霧"
            };
            this.customerDic.Add(customer.CustomerId, customer);

            customer = new Customer()
            {
                CustomerId = "C003",
                FirstName = "鳳",
                LastName = "梨"
            };
            this.customerDic.Add(customer.CustomerId, customer);
            
            //
            //prepare orders
            Order order = new Order() { OrderId = "D001", OrderDate = "20140605", CustomerId = "C001" };
            this.orderDic.Add(order.OrderId,order);
            order = new Order() { OrderId = "D002", OrderDate = "20140605", CustomerId = "C002" };
            this.orderDic.Add(order.OrderId, order);
            order = new Order() { OrderId = "D003", OrderDate = "20140606", CustomerId = "C001" };
            this.orderDic.Add(order.OrderId, order);
            order = new Order() { OrderId = "D004", OrderDate = "20140606", CustomerId = "C002" };
            this.orderDic.Add(order.OrderId, order);
            
            //
            foreach (string cid in this.customerDic.Keys)
            {
                this.customerDAO.Insert(this.customerDic[cid]);
            }
            foreach (string oid in this.orderDic.Keys)
            {
                this.orderDAO.Insert(this.orderDic[oid]);
            }
            
        }
        [Test]
        public void TestExist()
        {
            foreach (string oid in this.orderDic.Keys)
            {
                bool actual = this.orderDAO.Exist(oid);
                Assert.IsTrue(actual);
            }
        }
        [Test]
        public void TestInsert()
        {
            //prepare order
            Order order = new Order()
            {
                OrderId = "D005",OrderDate="20140607",CustomerId="C002"
            };
            this.orderDic.Add(order.OrderId, order);
            log.Info("======Insert 訂單======");
            log.Info("Insert => " + order.OrderId);
            this.orderDAO.Insert(order);
            log.Info("Finding => " + order.OrderId);
            Order result = this.orderDAO.FindByPK(order.OrderId);
            log.Debug(result.ToString());

            Assert.AreEqual(order.ToString(), result.ToString());
        }
        [Test]
        public void TestUpdate()
        {
            string oid = "D002";
            Order order = this.orderDAO.FindByPK(oid);
            if (order != null)
            {
                log.Debug("======上傳前======");
                log.Debug(order.ToString());
                //motify data
                order.OrderDate = "20000101";
                order.CustomerId = "C001";
                //
                log.Debug("======開始更新======");
                orderDAO.Update(order);
                log.Debug("======更新完成======");
                log.Debug("======上傳後======");
                Order result = orderDAO.FindByPK(oid);
                log.Debug(result.ToString());
                //
                Assert.AreEqual(order.ToString(), result.ToString());
            }
            else
            {
                log.Error("無此訂單" + oid);
                Assert.False(true);
            }
        }
        [Test]
        public void TestDelete()
        {
            string oid = "D001";
            log.Info("======開始刪除======");
            orderDAO.Delete(oid);
            log.Info("======刪除完成======");
            Assert.False(this.orderDAO.Exist(oid));
        }
        [Test]
        public void TestFindAll()
        {
            log.Info("======開始全找======");
            IList<Order> orders = orderDAO.FindAll();
            foreach (Order order in orders)
            {
                log.Info(order.ToString());
            }
            log.Info("======結束全找======");
        }
        [Test]
        public void TestFindByPK()
        {
            string oid = "D003";
            log.Info("======開始搜尋======");
            Order order = orderDAO.FindByPK(oid);
            log.Info(order.ToString());
            Assert.IsNotNull(order);
            //
            oid = "D999";
            order = orderDAO.FindByPK(oid);
            Assert.IsNull(order);
            log.Info("======結束搜尋======");
        }
        [Test]
        public void TestListCustomerOrders()
        {
            string cid = "C001";
            log.Info("======開始依客戶編號" + cid + "列表======");
            IList<Order> oList = orderDAO.ListCustomerOrders(cid);
            foreach (Order order in oList)
            {
                log.Info(order.ToString());
            }
            log.Info("======結束此客戶編號" + cid + "列表======");
        }
        [Test]
        public void TestFindByT()
        {
            string fieldValue = "20140606";
            string fieldName = "OrderDate";

            log.Info("======開始找" + fieldName + "=" + fieldValue + "的列表======");
            IList<Order> oList = orderDAO.FindByT(fieldName, fieldValue);
            IEnumerable<Order> dic = this.orderDic.Where(n => n.Value.OrderDate == fieldValue).Select(n => n.Value);
            
            foreach (Order expected in dic)
            {
                foreach (Order actual in oList)
                {
                    if (expected.OrderId == actual.OrderId)
                    {
                        Assert.AreEqual(expected.ToString(), actual.ToString());
                        log.Info("取得的資料" + actual.ToString());
                    }
                }
            }
            log.Info("======結束找" + fieldName + "="+ fieldValue + "的列表======");
        }
        [TearDown]
        public void TearDown()
        {
            foreach (string orderId in this.orderDic.Keys)
            {
                if (this.orderDAO.Exist(orderId))
                {
                    this.orderDAO.Delete(orderId);
                }
            }
            foreach (string customerId in this.customerDic.Keys)
            {
                if (this.customerDAO.Exist(customerId))
                {
                    this.customerDAO.Delete(customerId);
                }
            }
        }
    }
}
