using System;
using System.Collections.Generic;
//
using System.Data;
//以下使用Spring插件與Log插件
using Spring.Context;
using Spring.Context.Support;
using Common.Logging;
//以下用NUnit作單元測試
using NUnit.Framework;
//以下為自己的CODE
using Data;
using Spring_NET_Ado.Entities;
using Business;

namespace Data.Test.Business
{
    [TestFixture]
    class TestOrderManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TestOrderManager));
        //prepare spring container
        private IApplicationContext ctx;
        //prepare test container
        private ICustomerManager customerManager;
        private IOrderManager orderManager;
        //prepare compare dictionary
        private IDictionary<string, Customer> customerDic;
        private IDictionary<string, Order> orderDic;

        [SetUp]
        public void InitContext()
        {
            // 初始化App.config內容
            this.ctx = ContextRegistry.GetContext();
            // prepare container
            this.customerManager = ctx["txCustomerManager"] as ICustomerManager;
            this.orderManager = ctx["txOrderManager"] as IOrderManager;
            // prepare compare data
            this.customerDic = new Dictionary<string, Customer>();
            this.orderDic = new Dictionary<string, Order>();

            //prepare customers
            Customer customer = new Customer()
            {
                CustomerId = "C001",
                FirstName = "釋迦",
                LastName = "羊"
            };
            this.customerDic.Add(customer.CustomerId, customer);//將建立的物件暫放在dictionary內用來比較
            customer = new Customer()
            {
                CustomerId = "C002",
                FirstName = "榴槤",
                LastName = "張"
            };
            this.customerDic.Add(customer.CustomerId, customer);
            // add C001 and C002 in Customers Table
            foreach (string cid in this.customerDic.Keys)
            {
                this.customerManager.Insert(this.customerDic[cid]);
            }

            //prepare orders
            Order order = new Order() { OrderId = "D001", OrderDate = "20140605", CustomerId = "C001" };
            this.orderDic.Add(order.OrderId, order);
            order = new Order() { OrderId = "D002", OrderDate = "20140605", CustomerId = "C002" };
            this.orderDic.Add(order.OrderId, order);
            // add D001 and D00
            foreach (string oid in this.orderDic.Keys)
            {
                this.orderManager.Insert(this.orderDic[oid]);
            }
        }
        public void TestInsertOrders()
        {
            IList<Order> oList = new List<Order>();
            Order order = new Order
            {
                OrderId = "D003",
                OrderDate = "20120316",
                CustomerId = "C001"
            };
            oList.Add(order);
            this.orderDic.Add(order.OrderId, order);
            //
            order = new Order
            {
                OrderId = "D004",
                OrderDate = "20120316",
                CustomerId = "C002"
            };
            oList.Add(order);
            this.orderDic.Add(order.OrderId, order);

            //
            order = new Order
            {
                OrderId = "D005",
                OrderDate = "20120229",
                CustomerId = "C001"
            };
            oList.Add(order);
            this.orderDic.Add(order.OrderId, order);

            log.Debug("Insert Orders:");
            this.orderManager.InsertOrders(oList);
            foreach (string cid in this.customerDic.Keys)
            {
                IList<Order> odList = this.orderManager.ListCustomerOrders(cid);
                log.Debug("///////");
                log.Debug(odList.ToString());
            }
        }
        [Test]
        public void TestListCustomerOrders()
        {
            string cid = "C002";
            IList<Order> oList = this.orderManager.ListCustomerOrders(cid);
            foreach (Order order in oList)
            {
                log.Debug(order.ToString());
            }
        }

        [Test]
        public void TestInsertCustomerAndOrders()
        {
            Customer customer = new Customer
            {
                CustomerId = "C003",
                FirstName = "Jeremy",
                LastName = "Lin"
            };
            this.customerDic.Add(customer.CustomerId, customer);
            //
            IList<Order> oList = new List<Order>();
            Order order = new Order
            {
                OrderId = "D003",
                OrderDate = "20120316"
            };
            oList.Add(order);
            this.orderDic.Add(order.OrderId, order);
            //
            order = new Order
            {
                OrderId = "D004",
                OrderDate = "20120316"
            };
            oList.Add(order);
            this.orderDic.Add(order.OrderId, order);
            //
            order = new Order
            {
                OrderId = "D005",
                OrderDate = "20120229"
            };
            oList.Add(order);
            this.orderDic.Add(order.OrderId, order);
            //
            this.orderManager.InsertCustomerAndOrders(customer, oList);
        }
        [TearDown]
        public void TearDown()
        {
            foreach (string orderId in this.orderDic.Keys)
            {
                try
                {
                    this.orderManager.Delete(orderId);
                    log.Debug("Delete Order " + orderId);
                }
                catch
                {
                    log.Debug("Order " + orderId + " not exist!");
                }
            }

            foreach (string customerId in this.customerDic.Keys)
            {
                try
                {
                    this.customerManager.Delete(customerId);
                    log.Debug("Delete Customer " + customerId);
                }
                catch
                {
                    log.Debug("Customer " + customerId + " not exist!");
                }
            }            
        }
    }
}
