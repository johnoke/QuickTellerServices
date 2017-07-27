using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Helpers
{
    public class PaycodeTokenGeneration
    {
        public string paymentMethodTypeCode { get; set; }
        public string paymentMethodCode { get; set; }
        public string tokenLifeTimeInMinutes { get; set; }
        public string payWithMobileChannel { get; set; }
        public string amount { get; set; }
        public string ttid { get; set; }
        public string codeGeneratorChannelProvider { get; set; }
        public string oneTimePin { get; set; }
        public string accountNo { get; set; }
        public string accountType { get; set; }
        public string codeGenerationChannel { get; set; }
        public string autoEnroll { get; set; }
        public string transactionRef { get; set; }
    }
    public class CancelToken
    {
        public string transactionRef { get; set; }
        public string frontEndPartner { get; set; }
    }
    public class TokenStatus
    {
        public string paycode { get; set; }
        public string subscriberId { get; set; }
    }
}
