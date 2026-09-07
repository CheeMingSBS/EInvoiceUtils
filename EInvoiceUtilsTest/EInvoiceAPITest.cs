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
        string accessToken;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            const string CLIENT_ID = "CLIENT_ID";
            const string CLIENT_SECRET = "CLIENT_SECRET";
            const string SANDBOX_URL = "SANDBOX_URL";

            Assert.IsFalse(string.IsNullOrEmpty(secrets[CLIENT_ID]), $"Secret {CLIENT_ID} is invalid.");
            Assert.IsFalse(string.IsNullOrEmpty(secrets[CLIENT_SECRET]), $"Secret {CLIENT_SECRET} is invalid.");
            Assert.IsFalse(string.IsNullOrEmpty(secrets[SANDBOX_URL]), $"Config {SANDBOX_URL} is invalid.");

            client = new EInvoiceAPI(secrets[CLIENT_ID], secrets[CLIENT_SECRET], config[SANDBOX_URL]);
        }

        [TestMethod]
        public async Task LoginAsTaxpayer()
        {
            LoginAsTaxpayerResponse response = await client.LoginAsTaxpayer();
            this.accessToken = response.AccessToken;

            Assert.AreEqual(response.StatusCode, 200);
        }

        [TestMethod]
        public async Task LoginAsIntermediary()
        {
            const string SBS_CLIENT_ID = "SBS_CLIENT_ID";
            const string SBS_CLIENT_SECRET = "SBS_CLIENT_SECRET";
            const string PROD_URL = "PROD_URL";
            const string ON_BEHALF_OF = "ON_BEHALF_OF";

            Assert.IsFalse(string.IsNullOrEmpty(secrets[SBS_CLIENT_ID]), $"Secret {SBS_CLIENT_ID} is invalid.");
            Assert.IsFalse(string.IsNullOrEmpty(secrets[SBS_CLIENT_SECRET]), $"Secret {SBS_CLIENT_SECRET} is invalid.");
            Assert.IsFalse(string.IsNullOrEmpty(secrets[PROD_URL]), $"Config {PROD_URL} is invalid.");
            Assert.IsFalse(string.IsNullOrEmpty(secrets[ON_BEHALF_OF]), $"Config {ON_BEHALF_OF} is invalid.");

            EInvoiceAPI prodClient = new EInvoiceAPI(secrets[SBS_CLIENT_ID], secrets[SBS_CLIENT_SECRET], secrets[PROD_URL]);
            LoginAsIntermediaryResponse response = await prodClient.LoginAsIntermediary(secrets[ON_BEHALF_OF]);

            Assert.AreEqual(response.StatusCode, 200);
        }

        [TestMethod]
        public async Task SubmitDocuments()
        {
            const string SAMPLE_EINVOICE_DOC_PATH = "SAMPLE_EINVOICE_DOC_PATH";

            Assert.IsFalse(string.IsNullOrEmpty(config[SAMPLE_EINVOICE_DOC_PATH]), $"Config {SAMPLE_EINVOICE_DOC_PATH} is invalid.");

            if (this.accessToken == null)
                await LoginAsTaxpayer();

            SubmitDocumentsResponse response = await client.SubmitDocuments(
                                                this.accessToken,
                                                SubmitDocumentFormat.XML,
                                                new Dictionary<string, string>()
                                                {
                                                    { "INV00001", File.ReadAllText(secrets[SAMPLE_EINVOICE_DOC_PATH]!) }
                                                }
                                               );

            Assert.AreEqual(response.StatusCode, 202);
        }

        [TestMethod]
        public async Task SubmitDuplicateDocuments()
        {
            const string SAMPLE_EINVOICE_DOC_PATH = "SAMPLE_EINVOICE_DOC_PATH";

            Assert.IsFalse(string.IsNullOrEmpty(config[SAMPLE_EINVOICE_DOC_PATH]), $"Config {SAMPLE_EINVOICE_DOC_PATH} is invalid.");

            if (this.accessToken == null)
                await LoginAsTaxpayer();

            SubmitDocumentsResponse response = await client.SubmitDocuments(
                                                this.accessToken,
                                                SubmitDocumentFormat.XML,
                                                new Dictionary<string, string>()
                                                {
                                                    { "INV00001", File.ReadAllText(secrets[SAMPLE_EINVOICE_DOC_PATH]!) }
                                                }
                                               );

            response = await client.SubmitDocuments(
                                this.accessToken,
                                SubmitDocumentFormat.XML,
                                new Dictionary<string, string>()
                                {
                                    { "INV00001", File.ReadAllText(secrets[SAMPLE_EINVOICE_DOC_PATH]!) }
                                }
                             );

            Assert.AreEqual(response.StatusCode, 422);
        }

        [TestMethod]
        public async Task ValidateTaxpayerTIN()
        {
            const string TIN = "TIN";
            const string ID = "ID";

            Assert.IsFalse(string.IsNullOrEmpty(secrets[TIN]), $"Secret {TIN} is invalid.");
            Assert.IsFalse(string.IsNullOrEmpty(secrets[ID]), $"Secret {ID} is invalid.");

            if (this.accessToken == null)
                await LoginAsTaxpayer();

            ValidateTaxpayerTINResponse response = await client.ValidateTaxpayerTIN(this.accessToken, secrets[TIN], TaxpayerType.NRIC, secrets[ID]);

            Assert.AreEqual(response.statusCode, 200);
        }
    }
}