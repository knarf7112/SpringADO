using System;
using System.Collections.Generic;
using System.Linq;
//
using System.Data;
//
using Spring.Context;
using Spring.Context.Support;
using Common.Logging;
//
using NUnit.Framework;
//
using Spring_NET_Ado.Entities;
using Data;
using Business;

namespace Data.Test.Business
{
    [TestFixture]
    class TestCustomerManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TestCustomerManager));
        //prepare container
        private IApplicationContext ctx;//Spring container
        private ICustomerManager customerManager;
        //prepare compare dic
        private IDictionary<string, Customer> customerDic;

        [SetUp]
        public void InitContext()
        {
            this.ctx = ContextRegistry.GetContext();
            this.customerManager = ctx["txCustomerManager"] as ICustomerManager;

            //prepare customers
            this.customerDic = new Dictionary<string, Customer>();
            Customer customer = new Customer()
            {
                CustomerId = "C001",
                FirstName = "釋迦",
                LastName = "羊"
            };
            this.customerDic.Add(customer.CustomerId, customer);

            this.customerManager.Insert(customer);
        }
        [Test]
        public void TestExist()
        {
            string cid = "C001";
            bool actual = this.customerManager.Exist(cid);
            Assert.IsTrue(actual);
        }
        [Test]
        public void TestFindByPK()
        {
            foreach(string cid in this.customerDic.Keys)
            {
                Customer actual = this.customerManager.FindByPk(cid);
                log.Info("======搜尋" + cid + "編號======");
                if (actual != null)
                {
                    log.Info(actual.ToString());
                    Assert.AreEqual(this.customerDic[cid].ToString(),actual.ToString() );
                }
                else
                {
                    Assert.IsNotNull(actual);
                }
            }
        }
        [Test]
        public void TestFindAll()
        {
            //再多插入兩筆資料
            Customer customer = new Customer()
            {
                CustomerId = "C002",
                FirstName = "榴槤",
                LastName = "張"
            };
            this.customerDic.Add(customer.CustomerId, customer);
            this.customerManager.Insert(customer);
            customer = new Customer()
            {
                CustomerId = "C003",
                FirstName = "西瓜",
                LastName = "陳"
            };
            this.customerDic.Add(customer.CustomerId, customer);
            this.customerManager.Insert(customer);
            //----------------------------------------------------
            IList<Customer> cList = this.customerManager.FindAll();
            log.Info("======客戶全部列表======");
            log.Info(" 客戶總筆數 = " + this.customerDic.Count);
            foreach (Customer c in cList)
            {
                Assert.AreEqual(this.customerDic[c.CustomerId].ToString(), c.ToString());
                log.Info(c.ToString());
            }
            log.Info("======客戶列表結束======");
        }
        [Test]
        public void TestInsert()
        {
            Customer expected = new Customer()
            {
                CustomerId = "C002",
                FirstName = "榴槤",
                LastName = "張"
            };
            this.customerDic.Add(expected.CustomerId, expected);
            log.Info("======開始插入資料======");
            this.customerManager.Insert(expected);
            log.Info(expected.ToString());
            log.Info("======結束插入資料======");

            Customer actual = this.customerManager.FindByPk(expected.CustomerId);
            log.Info("======取得剛加入的資料======");
            log.Info(actual.ToString());
            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [Test]
        public void TestInserCustomers()
        {
            IList<Customer> cList = new List<Customer>();
            Customer customer = new Customer()
            {
                CustomerId = "C002",
                FirstName = "榴槤",
                LastName = "張"
            };
            cList.Add(customer);
            this.customerDic.Add(customer.CustomerId, customer);
            
            customer = new Customer()
            {
                //Id=001故意加一筆Id衝突的資料測試兩種Transaction,一個是不獨立Transaction,一個是獨立Transaction
                CustomerId = "C001",
                FirstName = "西瓜",
                LastName = "陳"
            };
            cList.Add(customer);
            //this.customerDic.Add(customer.CustomerId, customer);

            this.customerManager.InsertCustomers(cList);
            //
            foreach (string cid in this.customerDic.Keys)
            {
                customer = this.customerManager.FindByPk(cid);
                log.Info("======取得剛加入的資料======");
                log.Info(customer.ToString());
                Assert.AreEqual(customer.ToString(), this.customerDic[cid].ToString());
            }

        }
        
        [TearDown]
        public void TearDown()
        {
            foreach (string cid in this.customerDic.Keys)
            {
                try
                {
                    if (this.customerManager.Exist(cid))
                        this.customerManager.Delete(cid);
                    log.Debug("刪除" + cid + "完成");
                }
                catch (Exception ex)
                {
                    log.Error("刪除" + cid + "失敗:"  + ex.Message);
                }
            }
        }
    }
}

