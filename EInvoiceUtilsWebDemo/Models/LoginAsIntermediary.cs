namespace EInvoiceUtilsWebDemo.Models
{
    public class LoginAsIntermediary
    {
        public string clientId { get; set; }
        public string clientSecret { get; set; }
        public string onBehalfOf { get; set; }
    }
}
