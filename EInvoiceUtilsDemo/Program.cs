using Microsoft.Extensions.Configuration;
using EInvoiceUtils;
using System.Text.Json;
using EInvoiceUtils.Models;
using System.Text;

IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                      .Build();

IConfigurationRoot secrets = new ConfigurationBuilder().AddUserSecrets<Program>()
                                                       .Build();

#region Login as Taxpayer System
EInvoiceAPI client = new EInvoiceAPI(
                            clientId: secrets["CLIENT_ID"],
                            clientSecret: secrets["CLIENT_SECRET"],
                            apiUrl: config["SANDBOX_URL"]
                         );

LoginAsTaxpayerResponse loginAsTaxpayerResponse = await client.LoginAsTaxpayer();
Console.WriteLine(JsonSerializer.Serialize(loginAsTaxpayerResponse) + "\n");
#endregion

#region Login as Intermediary System
//EInvoiceAPI client = new EInvoiceAPI(secrets["SBS_CLIENT_ID"], secrets["SBS_CLIENT_SECRET"], config["PROD_URL"]);

//Console.WriteLine(JsonSerializer.Serialize(await client.LoginAsIntermediary(config["ON_BEHALF_OF"])) + "\n");
#endregion

#region Submit Documents
SubmitDocumentsResponse submitDocumentsResponse = await client.SubmitDocuments(
                                                    accessToken: loginAsTaxpayerResponse.AccessToken,
                                                    format: DocumentFormat.XML,
                                                    documents: new Dictionary<string, string>()
                                                    {
                                                        { "INV00001", File.ReadAllText(config["INVOICE_XML"]) }
                                                    }
                                                  );
Console.WriteLine(JsonSerializer.Serialize(submitDocumentsResponse) + "\n");
#endregion

#region Validate Taxpayer TIN
ValidateTaxpayerTINResponse validateTaxpayerTINResponse = await client.ValidateTaxpayerTIN(
                                                            accessToken: loginAsTaxpayerResponse.AccessToken,
                                                            tin: secrets["TIN"],
                                                            idType: TaxpayerType.NRIC,
                                                            idValue: secrets["ID"]
                                                          );
Console.WriteLine(JsonSerializer.Serialize(validateTaxpayerTINResponse) + "\n");
#endregion

#region Get Submission
GetSubmissionResponse getSubmissionResponse = await client.GetSubmission(
                                                accessToken: loginAsTaxpayerResponse.AccessToken,
                                                submissionUid: submitDocumentsResponse.SubmissionUid
                                              );

Console.WriteLine(JsonSerializer.Serialize(getSubmissionResponse) + "\n");

Thread.Sleep(1000);

getSubmissionResponse = await client.GetSubmission(
                                accessToken: loginAsTaxpayerResponse.AccessToken,
                                submissionUid: submitDocumentsResponse.SubmissionUid
                              );

Console.WriteLine(JsonSerializer.Serialize(getSubmissionResponse) + "\n");
#endregion

#region Get Document Details
GetDocumentDetailsResponse getDocumentDetailsResponse = await client.GetDocumentDetails(
                                                          accessToken: loginAsTaxpayerResponse.AccessToken,
                                                          uuid: submitDocumentsResponse.AcceptedDocuments[0].Uuid
                                                        );
Console.WriteLine(JsonSerializer.Serialize(getDocumentDetailsResponse) + "\n");
#endregion

#region Create Document
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

FileStream output = File.Open("C:\\Users\\SBS\\Desktop\\Projects\\EInvoiceUtils\\EInvoiceUtilsDemo\\Test.json", FileMode.Truncate);

await output.WriteAsync(Encoding.UTF8.GetBytes(document.Export(DocumentFormat.JSON, true)));
output.Close();

submitDocumentsResponse = await client.SubmitDocuments(
                            accessToken: loginAsTaxpayerResponse.AccessToken,
                            format: DocumentFormat.JSON,
                            documents: new Dictionary<string, string>()
                            {
                                { "INV00002", document.Export(DocumentFormat.JSON) }
                            }
                          );
Console.WriteLine(JsonSerializer.Serialize(submitDocumentsResponse) + "\n");
#endregion

//Console.ReadLine(); 