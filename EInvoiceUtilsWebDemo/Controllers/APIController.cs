using EInvoiceUtilsWebDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using EInvoiceUtils;
using EInvoiceUtils.Models;
using System.Text;

namespace EInvoiceUtilsWebDemo.Controllers
{
    public class APIController : Controller
    {
        private readonly ILogger<APIController> _logger;

        public APIController(ILogger<APIController> logger)
        {
            _logger = logger;
        }

        public IActionResult LoginAsTaxpayer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsTaxpayer(string clientId, string clientSecret)
        {
            EInvoiceAPI client = new EInvoiceAPI(clientId, clientSecret, "https://preprod-api.myinvois.hasil.gov.my");

            LoginAsTaxpayerResponse response = await client.LoginAsTaxpayer();
            return Json(response);
        }

        public IActionResult LoginAsIntermediary()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsIntermediary(string clientId, string clientSecret, string onBehalfOf)
        {
            EInvoiceAPI client = new EInvoiceAPI(clientId, clientSecret, "https://api.myinvois.hasil.gov.my");

            LoginAsIntermediaryResponse response = await client.LoginAsIntermediary(onBehalfOf);
            return Json(response);
        }

        public IActionResult ValidateTaxpayerTIN()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidateTaxpayerTIN(string idType, string tin, string id)
        {
            EInvoiceAPI client = new EInvoiceAPI("8f2c48da-fac9-460b-b2fa-7a724a175962", "0a594eff-e4b1-415b-b5dc-2bc9d9161ea0", "https://preprod-api.myinvois.hasil.gov.my");

            TaxpayerType type;
            if (idType == "BRN")
                type = TaxpayerType.BRN;
            else if (idType == "PASSPORT")
                type = TaxpayerType.PASSPORT;
            else if (idType == "ARMY")
                type = TaxpayerType.ARMY;
            else
                type = TaxpayerType.NRIC;

            LoginAsTaxpayerResponse loginAsTaxpayerResponse = await client.LoginAsTaxpayer();
            ValidateTaxpayerTINResponse validateTaxpayerTINResponse = await client.ValidateTaxpayerTIN(loginAsTaxpayerResponse.AccessToken, tin, type, id);
            return Json(validateTaxpayerTINResponse);
        }

        public IActionResult SubmitDocuments()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitDocuments(string referenceNo, string format, IFormFile file)
        {
            DocumentFormat documentFormat;

            if (format == "XML")
                documentFormat = DocumentFormat.XML;
            else
                documentFormat = DocumentFormat.JSON;

            Stream stream = file.OpenReadStream();
            byte[] buffer = new byte[file.Length];
            await stream.ReadAsync(buffer);

            EInvoiceAPI client = new EInvoiceAPI("8f2c48da-fac9-460b-b2fa-7a724a175962", "0a594eff-e4b1-415b-b5dc-2bc9d9161ea0", "https://preprod-api.myinvois.hasil.gov.my");
            LoginAsTaxpayerResponse loginAsTaxpayerResponse = await client.LoginAsTaxpayer();
            SubmitDocumentsResponse submitDocumentsResponse = await client.SubmitDocuments(
                                                                loginAsTaxpayerResponse.AccessToken,
                                                                documentFormat,
                                                                new Dictionary<string, string>()
                                                                {
                                                                    { referenceNo,  Encoding.UTF8.GetString(buffer) }
                                                                }
                                                              );

            return Json(submitDocumentsResponse);
        }

        public IActionResult GetSubmission()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetSubmission(string submissionUid)
        {
            EInvoiceAPI client = new EInvoiceAPI("8f2c48da-fac9-460b-b2fa-7a724a175962", "0a594eff-e4b1-415b-b5dc-2bc9d9161ea0", "https://preprod-api.myinvois.hasil.gov.my");

            LoginAsTaxpayerResponse loginAsTaxpayerResponse = await client.LoginAsTaxpayer();
            GetSubmissionResponse getSubmissionResponse = await client.GetSubmission(loginAsTaxpayerResponse.AccessToken, submissionUid);
            return Json(getSubmissionResponse);
        }

        public IActionResult GetDocumentDetails()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetDocumentDetails(string uuid)
        {
            EInvoiceAPI client = new EInvoiceAPI("8f2c48da-fac9-460b-b2fa-7a724a175962", "0a594eff-e4b1-415b-b5dc-2bc9d9161ea0", "https://preprod-api.myinvois.hasil.gov.my");

            LoginAsTaxpayerResponse loginAsTaxpayerResponse = await client.LoginAsTaxpayer();
            GetDocumentDetailsResponse getDocumentDetailsResponse = await client.GetDocumentDetails(loginAsTaxpayerResponse.AccessToken, uuid);
            return Json(getDocumentDetailsResponse);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
