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
        public QuickTellerDriver(string clientId,string clientSecret, string terminalId, string frontendPartnerId = "WEMA", string env = "staging")
        {
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
            this.FrontEndPartnerId = frontendPartnerId;
            Interswitch.Init(clientId, clientSecret, frontendPartnerId);
            this.TerminalId = terminalId;
            Environment = env;
            Interswitch.Environment = GetEnvironment(Environment);
        }
        private string ClientId;
        private string ClientSecret;
        private string Token;
        private string TerminalId;
        public string Environment;
        public string FrontEndPartnerId;
        public string PAN;
        public string CVV;
        public string ExpiryDate;
        public string Pin;
        public string CardType;
        private Interswitch Interswitch;
        public string GetEnvironment(string env)
        {
            switch (env)
            {
                case "production":
                    return Uris.liveenvironment;
                case "uat":
                    return Uris.uatenvironment;
                default:
                    return Uris.environment;
            }
        }
        public void SetupCardDetails(string pan, string cvv, string expiryDate, string pin, string cardType)
        {
            PAN = pan;
            CVV = cvv;
            ExpiryDate = expiryDate;
            Pin = pin;
            CardType = cardType;
        }
        public string GetPAN()
        {
            return this.PAN;
        }
        public string GetCVV()
        {
            return this.CVV;
        }
        public string GetExpiryDate()
        {
            return this.ExpiryDate;
        }
        public string GetPin()
        {
            return this.Pin;
        }
        public string GetCardType()
        {
            return this.CardType;
        }
        public async Task<string> GetTokenAsync()
        {
            string tokenEnv = GetEnvironment(Environment);
            var token = await Interswitch.GetClientAccessToken(this.ClientId, this.ClientSecret, Environment);
            return token.access_token;
        }
        public void SetToken(string token)
        {
            this.Token = token;
        }
        public Dictionary<string, string> GetCategories()
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> ("TerminalId", TerminalId )
            };
            var billsResponse = Interswitch.Send(Uris.categoriesUrl, Constants.GET, new object { }, hashMap: parameters, token: Token, environment: Environment).Result;
            return billsResponse;
        }
        public Dictionary<string, string> GetBillers()
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> ("TerminalId", TerminalId )
            };
            var billsResponse = Interswitch.Send(Uris.billersUrl, Constants.GET, new object { }, hashMap: parameters, token: Token, environment: Environment).Result;
            return billsResponse;
        }
        public Dictionary<string, string> GetBillers(string categoryId)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> ("TerminalId", TerminalId )
            };
            var billsResponse = Interswitch.Send(Uris.billersCategoriesUrl.Replace("{id}", categoryId), Constants.GET, new object { }, hashMap: parameters, token: Token, environment: Environment).Result;
            return billsResponse;
        }
        public Dictionary<string, string> GetBillerItems(string billerId)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> ("TerminalId", TerminalId )
            };
            var billsResponse = Interswitch.Send(Uris.billerPaymentItemsUrl.Replace("{billerId}", billerId), Constants.GET, new object { }, hashMap: parameters, token: Token, environment: Environment).Result;
            return billsResponse;
        }
        public Dictionary<string, string> GetReference(string requestReference)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> ("TerminalId", TerminalId )
            };
            var refResponse = Interswitch.Send(Uris.requestReferenceUrl.Replace("{transactionReference}", requestReference), Constants.GET, new object { }, hashMap: parameters, token: Token, environment: Environment).Result;
            return refResponse;
        }
        public Dictionary<string, string> PostPaymentAdvice(PaymentAdvice payment)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> ("TerminalId", TerminalId )
            };
            var billsResponse = Interswitch.Send(Uris.billPaymentNotification, Constants.POST, payment, hashMap: parameters, token: Token, environment: Environment).Result;
            return billsResponse;
        }
        public async Task<Dictionary<string, string>> PostCustomerVerification(CustomerValidation customerValidation)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> ("TerminalId", TerminalId )
            };
            var billsResponse = await Interswitch.Send(Uris.customerVerificationUrl, Constants.POST, customerValidation, hashMap: parameters, token: Token, environment: Environment);
            return billsResponse;
        }
        public async Task<Dictionary<string, string>> PostPaymentAdviceAsync(PaymentAdvice payment)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> ("TerminalId", TerminalId )
            };
            var billsResponse = await Interswitch.Send(Uris.billPaymentNotification, Constants.POST, payment, hashMap: parameters, token: Token, environment: Environment);
            return billsResponse;
        }
        public async Task<Dictionary<string, string>> GeneratePaycodeToken(PaycodeTokenGeneration request, string subscriberId)
        {
            var tokenResponse = await Interswitch.Send(Uris.paycodeGenerateToken.Replace("{subscriberId}", subscriberId), Constants.POST, request, token: Token, environment: Environment, isPaycode: true);
            return tokenResponse;
        }
        public async Task<Dictionary<string, string>> GenerateBulkPaycodeToken(BulkPaycodeGeneration request)
        {
            var tokenResponse = await Interswitch.Send(Uris.bulkPaycodeGenerateToken, Constants.POST, request, token: Token, environment: Environment, isPaycode: true);
            return tokenResponse;
        }
        public async Task<Dictionary<string, string>> CancelPaycodeToken(CancelToken request)
        {
            var tokenResponse = await Interswitch.Send(Uris.cancelToken, Constants.DELETE, request, token: Token, environment: Environment, isPaycode: true);
            return tokenResponse;
        }
        public async Task<Dictionary<string, string>> GetTokenStatus(string subscriberId, string paycode)
        {
            var tokenResponse = await Interswitch.Send(Uris.tokenStatus.Replace("{subscriberId}", subscriberId), Constants.GET, null, token: Token, environment: Environment, isPaycodeStatus: true, paycode: paycode);
            return tokenResponse;
        }
    }
}
