
using Spring_NET_Ado.Utilities;
using System.Data;
namespace Spring_NET_Ado.Entities
{
    public class Customer : AbstractDO
    {
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
