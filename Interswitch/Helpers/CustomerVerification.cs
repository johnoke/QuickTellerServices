using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Helpers
{
    public class CustomerVerification
    {
        public string customerId { get; set; }
        public string paymentCode { get; set; }
    }
    public class CustomerValidation
    {
        public List<CustomerVerification> customers { get; set; }
    }
}
