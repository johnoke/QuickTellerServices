using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Utils
{
    class Uris
    {
        public const string billersUrl = "/api/v1/quickteller/billers";
        public const string billersCategoriesUrl = "/api/v1/quickteller/categorys/{id}/billers";
        public const string billerPaymentItemsUrl = "/api/v1/quickteller/billers/{billerId}/paymentitems";
    }
}
