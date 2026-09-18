namespace EInvoiceUtilsWebDemo.Models
{
    public class CreateDocumentModel
    {
        public string documentId { get; set; }
        public Party supplier { get; set; }
        public Party customer { get; set; }
        public List<InvoiceLine> invoiceLines { get; set; }
    }

    public class Party
    {
        public string name { get; set; }
        public string tin { get; set; }
        public int idType { get; set; }
        public string id { get; set; }
        public string businessDesc { get; set; }
        public string address0 { get; set; }
        public string address1 { get; set; }
        public string cityName { get; set; }
        public string postalZone { get; set; }
        public int state { get; set; }
        public string email { get; set; }
        public string contactNumber { get; set; }
    }

    public class InvoiceLine
    {
        public string id { get; set; }
        public string classificationCode { get; set; }
        public string description { get; set; }
        public string unitPrice { get; set; }
        public string taxableAmount { get; set; }
        public string taxAmount { get; set; }
    }
}
