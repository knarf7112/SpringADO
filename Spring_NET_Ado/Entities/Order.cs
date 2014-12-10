using Spring_NET_Ado.Utilities;

namespace Spring_NET_Ado.Entities
{
    public class Order : AbstractDO
    {
        public virtual string OrderId { get; set; }

        public virtual string OrderDate { get; set; }

        public virtual string CustomerId { get; set; }
        /*
        public virtual Customer Customer { get; set; }
        public virtual string CustomerId
        {
            get
            {
                return this.Customer.CustomerId;
            }
            set
            {
                if (this.Customer == null)
                {
                    this.Customer = new Customer();
                }
                this.Customer.CustomerId = value;
            }
        }
        */
    }
}
