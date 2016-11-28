using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Payment;
using Newtonsoft.Json;
using Payment.Drivers;
using Payment.Helpers;
namespace SampleProject
{
    public class Example
    {
        static void Main(string[] args)
        {
            IQuickTellerDriver driver = new QuickTellerDriver("IKIAF6C068791F465D2A2AA1A3FE88343B9951BAC9C3", "FTbMeBD7MtkGBQJw1XoM74NaikuPL13Sxko1zb0DMjI=", "3IWP0001");
            //PaymentAdvice advice = new PaymentAdvice { amount ="10000", customerId = "08124266856", customerMobile = "08124266856", customerEmail = "08124266856", paymentCode = "90101", requestReference = "102920202", TerminalId = "3IWP0001" };
            Console.WriteLine(driver.GetCategories());
            Console.ReadKey();
        }
    }
}
