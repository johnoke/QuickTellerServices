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
        public const string customerVerificationUrl = "/api/v2/quickteller/customers/validations";
        public const string requestReferenceUrl = "/api/v2/quickteller/transactions?requestreference={transactionReference}";
        public const string paycodeGenerateToken = "/api/v1/pwm/subscribers/{subscriberId}/tokens";
        public const string bulkPaycodeGenerateToken = "/api/v1/pwm/bulk/tokens";
        public const string cancelToken = "/api/v1/pwm/tokens";
        public const string tokenStatus = "/api/v1/pwm/info/{subscriberId}/tokens";
        public const string environment = "https://sandbox.interswitchng.com";
        public const string liveenvironment = "https://saturn.interswitchng.com";
        public const string passportenvironment = "https://passport.interswitchng.com";
        public const string uatenvironment = "http://172.26.40.115:9080";
    }
}
