using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Utils
{
    class Uris
    {
        public const string categoriesUrl = "/api/v2/quickteller/categorys";
        public const string billersUrl = "/api/v2/quickteller/billers";
        public const string billersCategoriesUrl = "/api/v2/quickteller/categorys/{id}/billers";
        public const string billerPaymentItemsUrl = "/api/v2/quickteller/billers/{billerId}/paymentitems";
        public const string billPaymentNotification = "/api/v2/quickteller/sendAdviceRequest";
    }
}
