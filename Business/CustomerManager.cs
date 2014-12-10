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
    public class CustomerManager : ICustomerManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CustomerManager));
        //Inject DAO From Config,商業邏輯下的工人,負責接受管理者命令做事的
        public IDAO<Customer, string> CustomerDAO { get; set; }
        public void Insert(Customer customer)
        {
            //檢查是否已存在
            if (this.CustomerDAO.Exist(customer.CustomerId))
            {
                throw new Exception("Customer " + customer.CustomerId + " already exist!");
            }

            try
            {
                this.CustomerDAO.Insert(customer);
            }
            catch(Exception ex)
            {
                throw new Exception("Customer " + customer.CustomerId + " insert fail: " + ex.Message);
            }
        }

        public void Delete(string customerId)
        {
            if (!this.CustomerDAO.Exist(customerId))
            {
                throw new Exception("Customer " + customerId + " not exist!");
            }
            try
            {
                this.CustomerDAO.Delete(customerId);
            }
            catch (Exception ex)
            {
                throw new Exception("Customer " + customerId + " delete fail:" + ex.Message);
            }
        }

        public bool Exist(string customerId)
        {
            return this.CustomerDAO.Exist(customerId);
        }

        public Customer FindByPk(string customerId)
        {
            return this.CustomerDAO.FindByPK(customerId);
        }

        public IList<Customer> FindAll()
        {
            return this.CustomerDAO.FindAll();
        }

        public void InsertCustomers(IList<Customer> cList)
        {
            foreach (Customer c in cList)
            {
                this.Insert(c);
                log.Debug("Customer " + c.CustomerId + " insert OK!");
            }
        }
    }
}
