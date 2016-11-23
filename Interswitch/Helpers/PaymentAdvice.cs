namespace Payment.Helpers
{
    public class PaymentAdvice
    {
        public string TerminalId { get; set; }
        public string paymentCode { get; set; }
        public string customerId { get; set; }
        public string customerMobile { get; set; }
        public string customerEmail { get; set; }
        public string amount { get; set; }
        public string requestReference { get; set; }
    }
}
