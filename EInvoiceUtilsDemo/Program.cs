using Microsoft.Extensions.Configuration;
using EInvoiceUtils;
using System.Text.Json;

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

Console.WriteLine(JsonSerializer.Serialize(await client.LoginAsTaxpayer()));
#endregion

#region Login as Intermediary System
//EInvoiceAPI client = new EInvoiceAPI(secrets["SBS_CLIENT_ID"], secrets["SBS_CLIENT_SECRET"], config["PROD_URL"]);

//Console.WriteLine(JsonSerializer.Serialize(await client.LoginAsIntermediary(config["ON_BEHALF_OF"])));
#endregion

Console.ReadLine(); 