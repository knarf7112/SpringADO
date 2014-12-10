
using Spring_NET_Ado.Utilities;
using System.Data;
namespace Spring_NET_Ado.Entities
{
    public class Customer : AbstractDO
    {
        public virtual string CustomerId { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
    }
}
