using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payment.Utils;
using Payment.Helpers;
namespace Payment.Drivers
{
    public class QuickTellerDriver : IQuickTellerDriver
    {
        public QuickTellerDriver(string clientId,string clientSecret, string terminalId)
        {
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
            Interswitch.Init(clientId, clientSecret);
            Token = Interswitch.GetToken();
            this.TerminalId = terminalId;
        }
        private string ClientId;
        private string ClientSecret;
        private string Token;
        private string TerminalId;
        public string GetCategories()
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> ("TerminalId", TerminalId )
            };
            var billsResponse = Interswitch.Send(Uris.categoriesUrl, Constants.GET, new object { }, hashMap: parameters).Result;
            return billsResponse;
        }
        public string GetBillers()
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> ("TerminalId", TerminalId )
            };
            var billsResponse = Interswitch.Send(Uris.billersUrl, Constants.GET, new object { }, hashMap: parameters).Result;
            return billsResponse;
        }
        public string GetBillers(string categoryId)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> ("TerminalId", TerminalId )
            };
            var billsResponse = Interswitch.Send(Uris.billersCategoriesUrl.Replace("{id}", categoryId), Constants.GET, new object { }, hashMap: parameters).Result;
            return billsResponse;
        }
        public string GetBillerItems(string billerId)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> ("TerminalId", TerminalId )
            };
            var billsResponse = Interswitch.Send(Uris.billerPaymentItemsUrl.Replace("{billerId}", billerId), Constants.GET, new object { }, hashMap: parameters).Result;
            return billsResponse;
        }
        public string PostPaymentAdvice(PaymentAdvice payment)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> ("TerminalId", TerminalId )
            };
            var billsResponse = Interswitch.Send(Uris.billPaymentNotification, Constants.POST, payment, hashMap: parameters).Result;
            return billsResponse;
        }
    }
}
