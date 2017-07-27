using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payment.Helpers;
namespace Payment.Drivers
{
    public interface IQuickTellerDriver
    {
        void SetupCardDetails(string pan, string cvv, string expiryDate, string pin, string cardType);
        string GetPAN();
        string GetCVV();
        string GetExpiryDate();
        string GetPin();
        string GetCardType();
        Task<string> GetTokenAsync();
        void SetToken(string token);
        Dictionary<string, string> GetCategories();
        Dictionary<string, string> GetBillers();
        Dictionary<string, string> GetBillers(string categoryId);
        Dictionary<string, string> GetBillerItems(string billerId);
        Dictionary<string, string> PostPaymentAdvice(PaymentAdvice payment);
        Dictionary<string, string> GetReference(string requestReference);
        Task<Dictionary<string, string>> PostPaymentAdviceAsync(PaymentAdvice payment);
        Task<Dictionary<string, string>> PostCustomerVerification(CustomerValidation customerValidation);
        Task<Dictionary<string, string>> GeneratePaycodeToken(PaycodeTokenGeneration request, string subscriberId);
        Task<Dictionary<string, string>> GenerateBulkPaycodeToken(BulkPaycodeGeneration request);
        Task<Dictionary<string, string>> CancelPaycodeToken(CancelToken request);
        Task<Dictionary<string, string>> GetTokenStatus(string subscriberId, string paycode);
    }
}
