namespace EInvoiceUtilsTest
{
    [TestClass]
    public sealed class EInvoiceDocumentTest
    {
        [TestMethod]
        public async Task CreateDocument()
        {
            EInvoiceDocument document = new EInvoiceDocument(id: "JSON-INV12345");

            AccountingSupplierPartyArgs supplier = new AccountingSupplierPartyArgs(
                                                    name: "Test",
                                                    tin: "IG28566784100",
                                                    idType: TaxpayerType.NRIC,
                                                    id: "980427145943",
                                                    msic: MSIC.OPERATION_OF_PARKING_FACILITIES_FOR_MOTOR_VEHICLES_PARKING_LOTS,
                                                    cityName: "Kuala Lumpur",
                                                    state: State.WILAYAH_PERSEKUTUAN_KUALA_LUMPUR,
                                                    country: CountryCode.MALAYSIA,
                                                    contactNumber: "0123456789"
                                                   )
            {
                PostalZone = "50490",
                AddressLine0 = "X Unit 07-06, Vertical Tower A",
                AddressLine1 = "No. 8 Jalan Kerinchi, Bangsar South",
                Email = "test@test.com",
                BusinessDescription = "Carpark Operator"
            };

            document.SetAccountingSupplierParty(supplier);

            AccountingCustomerPartyArgs customer = new AccountingCustomerPartyArgs(
                                                    name: "General Public",
                                                    tin: "EI00000000010",
                                                    idType: TaxpayerType.NRIC,
                                                    id: "NA",
                                                    cityName: "NA",
                                                    state: State.NOT_APPLICABLE,
                                                    country: CountryCode.MALAYSIA,
                                                    contactNumber: "NA"
                                                   );

            document.SetAccountingCustomerParty(customer);

            InvoiceLineTaxSubtotalArgs taxSubtotal = new InvoiceLineTaxSubtotalArgs(
                                                        taxableAmount: 5.66m,
                                                        taxAmount: 0.34m,
                                                        taxType: TaxType.SERVICE_TAX
                                                     );

            InvoiceLineArgs invoiceLine = new InvoiceLineArgs(
                                            id: "INV00001",
                                            classificationCode: ClassificationCode.CONSOLIDATED_EINVOICE,
                                            description: "E_1234567_1234",
                                            unitPrice: 5.66m,
                                            taxSubtotal: new List<InvoiceLineTaxSubtotalArgs>() { taxSubtotal }
                                          )
            {
                Quantity = 1
            };

            InvoiceLineArgs invoiceLine2 = new InvoiceLineArgs(
                                            id: "INV00002",
                                            classificationCode: ClassificationCode.CONSOLIDATED_EINVOICE,
                                            description: "E_1234567_4567",
                                            unitPrice: 5.66m,
                                            taxSubtotal: new List<InvoiceLineTaxSubtotalArgs>() { taxSubtotal }
                                          )
            {
                Quantity = 1
            };

            document.AddInvoiceLineItem(invoiceLine);
            document.AddInvoiceLineItem(invoiceLine2);

            // TODO: Assert necessary elements
        }
    }
}
