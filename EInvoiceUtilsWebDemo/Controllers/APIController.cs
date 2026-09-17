using EInvoiceUtilsWebDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using EInvoiceUtils;
using EInvoiceUtils.Models;

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
        public async Task<IActionResult> LoginAsTaxpayer([FromBody] LoginAsTaxpayer body)
        {
            EInvoiceAPI client = new EInvoiceAPI(body.clientId, body.clientSecret, "https://preprod-api.myinvois.hasil.gov.my");

            LoginAsTaxpayerResponse response = await client.LoginAsTaxpayer();
            return Json(response);
        }

        public IActionResult LoginAsIntermediary()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsIntermediary([FromBody] LoginAsIntermediary body)
        {
            EInvoiceAPI client = new EInvoiceAPI(body.clientId, body.clientSecret, "https://api.myinvois.hasil.gov.my");

            LoginAsIntermediaryResponse response = await client.LoginAsIntermediary(body.onBehalfOf);
            return Json(response);
        }

        public IActionResult ValidateTaxpayerTIN()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidateTaxpayerTIN([FromBody] ValidateTaxpayerTIN body)
        {
            EInvoiceAPI client = new EInvoiceAPI("8f2c48da-fac9-460b-b2fa-7a724a175962", "0a594eff-e4b1-415b-b5dc-2bc9d9161ea0", "https://preprod-api.myinvois.hasil.gov.my");

            LoginAsTaxpayerResponse loginAsTaxpayerResponse = await client.LoginAsTaxpayer();
            ValidateTaxpayerTINResponse validateTaxpayerTINResponse = await client.ValidateTaxpayerTIN(loginAsTaxpayerResponse.AccessToken, body.tin, TaxpayerType.NRIC, body.nric);
            return Json(validateTaxpayerTINResponse);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
