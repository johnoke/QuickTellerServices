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
        string GetCategories();
        string GetBillers();
        string GetBillers(string categoryId);
        string GetBillerItems(string billerId);
        string PostPaymentAdvice(PaymentAdvice payment);
    }
}
