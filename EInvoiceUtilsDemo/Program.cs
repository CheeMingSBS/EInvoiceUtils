using Microsoft.Extensions.Configuration;
using EInvoiceUtils;
using System.Text.Json;
using EInvoiceUtils.Models;

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
                                                    format: SubmitDocumentFormat.XML,
                                                    documents: new Dictionary<string, string>()
                                                    {
                                                        { "INV00001", File.ReadAllText(config["SAMPLE_EINVOICE_DOC_PATH"]) }
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

Console.ReadLine(); 