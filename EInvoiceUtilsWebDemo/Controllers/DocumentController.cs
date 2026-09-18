using EInvoiceUtils;
using EInvoiceUtils.Models;
using EInvoiceUtilsWebDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace EInvoiceUtilsWebDemo.Controllers
{
    public class DocumentController : Controller
    {
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(ILogger<DocumentController> logger)
        {
            _logger = logger;
        }

        public IActionResult CreateDocument()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentModel body)
        {
            EInvoiceDocument document = new EInvoiceDocument(id: body.documentId);

            AccountingSupplierPartyArgs supplier = new AccountingSupplierPartyArgs(
                                                    name: body.supplier.name,
                                                    tin: body.supplier.tin,
                                                    idType: (TaxpayerType)body.supplier.idType,
                                                    id: body.supplier.id,
                                                    msic: MSIC.OPERATION_OF_PARKING_FACILITIES_FOR_MOTOR_VEHICLES_PARKING_LOTS,
                                                    cityName: body.supplier.cityName,
                                                    state: (State)body.supplier.state,
                                                    country: CountryCode.MALAYSIA,
                                                    contactNumber: body.supplier.contactNumber
                                                   )
            {
                PostalZone = body.supplier.postalZone,
                AddressLine0 = body.supplier.address0,
                AddressLine1 = body.supplier.address1,
                Email = body.supplier.email,
                BusinessDescription = body.supplier.businessDesc
            };

            AccountingCustomerPartyArgs customer = new AccountingCustomerPartyArgs(
                                                        name: body.customer.name,
                                                        tin: body.customer.tin,
                                                        idType: (TaxpayerType)body.customer.idType,
                                                        id: body.customer.id,
                                                        cityName: body.customer.cityName,
                                                        state: (State)body.customer.state,
                                                        country: CountryCode.MALAYSIA,
                                                        contactNumber: body.customer.contactNumber
                                                       );

            document.SetAccountingSupplierParty(supplier);
            document.SetAccountingCustomerParty(customer);

            foreach (EInvoiceUtilsWebDemo.Models.InvoiceLine line in body.invoiceLines)
            {
                InvoiceLineTaxSubtotalArgs taxSubtotal = new InvoiceLineTaxSubtotalArgs(
                                            taxableAmount: decimal.Parse(line.taxableAmount),
                                            taxAmount: decimal.Parse(line.taxAmount),
                                            taxType: TaxType.SERVICE_TAX
                                         );

                InvoiceLineArgs invoiceLine = new InvoiceLineArgs(
                                                id: line.id,
                                                classificationCode: ClassificationCode.CONSOLIDATED_EINVOICE,
                                                description: line.description,
                                                unitPrice: decimal.Parse(line.unitPrice),
                                                taxSubtotal: new List<InvoiceLineTaxSubtotalArgs>() { taxSubtotal }
                                              )
                {
                    Quantity = 1
                };

                document.AddInvoiceLineItem(invoiceLine);
            }

            return Json(JsonSerializer.Deserialize<object>(document.Export(DocumentFormat.JSON, true)));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
