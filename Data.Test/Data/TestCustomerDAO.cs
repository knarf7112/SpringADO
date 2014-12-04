using System;
using System.Collections.Generic;
using System.Linq;
//
using Data;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;
using Common.Logging;
using Spring_NET_Ado.Entities;

namespace Data.Test.Data
{
    [TestFixture]
    public class TestCustomerDAO
    {
        private IApplicationContext ctx;
        private CustomerDAO customerDAO;
        private static readonly ILog log = LogManager.GetLogger(typeof(TestCustomerDAO));
        private IDictionary<string, Customer> customerDic;

        //初始化=>塞資料
        [SetUp]
        public void InitContext()
        {
            this.ctx = ContextRegistry.GetContext();
            this.customerDAO = ctx["customerDAO"] as CustomerDAO;
            this.customerDic = new Dictionary<string, Customer>();
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
            customer = new Customer()
            {
                CustomerId = "C003",
                FirstName = "西瓜",
                LastName = "陳"
            };
            this.customerDic.Add(customer.CustomerId, customer);
        }
        [Test]
        public void TestInsert()
        {
            log.Debug("======Insert Customer: ");
            //
            foreach (string cid in this.customerDic.Keys)
            {
                log.Info("======Insert: " + cid);
                customerDAO.Insert(this.customerDic[cid]);
                log.Info("======Finding: " + cid);
                Customer result = this.customerDAO.FindByPK(cid);
                log.Debug(result);//會執行物件的toString()方法(有改寫過)

                Assert.AreEqual(customerDic[cid].ToString(), result.ToString());
            }
        }

        [Test]
        public void TestUpdate()
        {
            string cid = "C002";
            //先插入一筆資料
            customerDAO.Insert(this.customerDic[cid]);
            //取出修改前的資料
            Customer customer = customerDAO.FindByPK(cid);
            log.Debug("======Before update====== ");
            log.Debug(customer);
            //更新資料
            customer.FirstName = "波多野";
            customer.LastName = "結依";
            log.Info("======Update======");
            customerDAO.Update(customer);
            //再取剛剛更新的資料
            log.Debug("======After Update======");
            Customer result = customerDAO.FindByPK(cid);
            log.Debug(result);
            //比較
            Assert.AreEqual(customer.ToString(), result.ToString());
        }

        [Test]
        public void TestDelete()
        {
            string cid = "C001";
            //插入一筆資料
            customerDAO.Insert(this.customerDic[cid]);
            //刪除此筆資料
            log.Info("======Start Delete======");
            customerDAO.Delete(cid);
            log.Info("======End Delete======");
            Customer customer = this.customerDAO.FindByPK(cid);
            Assert.IsNull(customer);
        }

        //檢查兩件事: 總數 資料 是否一致
        [Test]
        public void TestFindAll()
        {
            log.Debug("======Insert: Customers:");
            //
            foreach (string cid in this.customerDic.Keys)
            {
                log.Info("Insert: " + cid);
                customerDAO.Insert(this.customerDic[cid]);
            }

            log.Info("======Start Find All======");
            IList<Customer> customers = customerDAO.FindAll();
            Assert.AreEqual(this.customerDic.Count, customers.Count);

            foreach (Customer c in customers)
            {
                Assert.AreEqual(this.customerDic[c.CustomerId].ToString(), c.ToString());
                log.Info(c);
            }
            log.Info("======End Find All======");
        }

        [Test]
        public void TestFindByPk()
        {
            log.Debug("======Insert======");
            foreach (string cid in this.customerDic.Keys)
            {
                log.Info("======Insert: " + cid);
                customerDAO.Insert(this.customerDic[cid]);
            }
            string id = "C001";
            log.Info("Start Find By PK: " + id);
            Customer customer = customerDAO.FindByPK(id);
            log.Info(customer);
            Assert.AreEqual(customer.CustomerId, id);
            log.Info("======End Find By PK======");
        }

        [Test]
        public void TestExist()
        {
            foreach (string cid in this.customerDic.Keys)
            {
                Assert.IsFalse(customerDAO.Exist(cid));
                log.Info("======Insert: " + cid);
                customerDAO.Insert(this.customerDic[cid]);

                Assert.IsTrue(customerDAO.Exist(cid));
            }
        }

        //每次Test完一個方法都會做一次
        [TearDown]
        public void TearDown()
        {
            foreach (string cid in this.customerDic.Keys)
            {
                if (this.customerDAO.Exist(cid))
                {
                    this.customerDAO.Delete(cid);
                }
            }
        }
    }
}
