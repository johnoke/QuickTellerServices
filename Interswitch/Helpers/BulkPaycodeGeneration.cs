using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Helpers
{
    public class BulkPaycodeGeneration
    {
        public string frontEndPartnerId { get; set; }
        public AdditionalInfo additionalInfo { get; set; }
        public string amount { get; set; }
        public int batchSize { get; set; }
        public string channel { get; set; }
        public string defaultOneTimePin { get; set; }
        public string macData { get; set; }
        public string pinBlock { get; set; }
        public string referenceId { get; set; }
        public string secure { get; set; }
        public string subscriberId { get; set; }
        public string tokenLifeTimeInMinutes { get; set; }
        public string ttid { get; set; }
        public List<PaycodeEntry> entries { get; set; }
    }
    public class AdditionalInfo
    {
        public string type { get; set; }
    }
    public class PaycodeEntry
    {
        public string amount { get; set; }
        public string beneficiaryNumber { get; set; }
        public string oneTimePin { get; set; }
    }
}
