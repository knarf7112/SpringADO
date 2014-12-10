using System.Collections.Generic;
//
using Spring_NET_Ado.Entities;
namespace Business
{
    public interface ICustomerManager
    {
        void Insert(Customer customer);
        void Delete(string customerId);
        bool Exist(string customerId);
        Customer FindByPk(string customerId);
        IList<Customer> FindAll();
        void InsertCustomers(IList<Customer> cList);
    }
}
