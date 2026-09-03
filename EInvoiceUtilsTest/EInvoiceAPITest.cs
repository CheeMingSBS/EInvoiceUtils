using Microsoft.Extensions.Configuration;

namespace EInvoiceUtilsTest
{
    [TestClass]
    public sealed class EInvoiceAPITest
    {
        static IConfigurationRoot secrets = new ConfigurationBuilder().AddUserSecrets<EInvoiceAPITest>()
                                                               .Build();
        static IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                              .Build();

        static EInvoiceAPI client;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            client = new EInvoiceAPI(secrets["CLIENT_ID"], secrets["CLIENT_SECRET"], config["SANDBOX_URL"]);
        }

        [TestMethod]
        public async Task LoginAsTaxpayer()
        {
            LoginAsTaxpayerResponse response = await client.LoginAsTaxpayer();
            Assert.AreEqual(response.statusCode, 200);
        }

        [TestMethod]
        public async Task LoginAsIntermediary()
        {
            EInvoiceAPI prodClient = new EInvoiceAPI(secrets["SBS_CLIENT_ID"], secrets["SBS_CLIENT_SECRET"], config["PROD_URL"]);
            LoginAsIntermediaryResponse response = await prodClient.LoginAsIntermediary(config["ON_BEHALF_OF"]);
            Assert.AreEqual(response.statusCode, 200);
        }
    }
}