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
            // 
            this.customerManager = ctx["txCustomerManager"] as ICustomerManager;
            this.orderManager = ctx["txOrderManager"] as IOrderManager;
            this.customerDic = new Dictionary<string, Customer>();
            this.orderDic = new Dictionary<string, Order>();


        }


        [TearDown]
        public void TearDown()
        {

        }
    }
}
