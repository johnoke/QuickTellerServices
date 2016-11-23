using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Drivers
{
    public interface IQuickTellerDriver
    {
        string GetBillers();
        string GetBillers(string categoryId);
        string GetBillerItems(string billerId);
    }
}
