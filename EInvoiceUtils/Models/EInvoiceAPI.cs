using System.Text.Json.Serialization;

namespace EInvoiceUtils.Models
{
    /// <summary>
    ///     <see href="https://sdk.myinvois.hasil.gov.my/api/07-login-as-taxpayer-system/#outputs">Login as Taxpayer API Output</see>
    /// </summary>
    public class LoginAsTaxpayerResponse
    {
        public int statusCode { get; set; }

        [JsonPropertyName("access_token")]
        public string accessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int expiresIn { get; set; }

        public string error { get; set; }

        [JsonPropertyName("error_description")]
        public string errorDescription { get; set; }

        [JsonPropertyName("error_uri")]
        public string errorUri { get; set; }
    }

    /// <summary>
    ///     <see href="https://sdk.myinvois.hasil.gov.my/api/08-login-as-intermediary-system/#outputs">Login as Intermediary API Output</see>
    /// </summary>
    public class LoginAsIntermediaryResponse
    {
        public int statusCode { get; set; }

        [JsonPropertyName("access_token")]
        public string accessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int expiresIn { get; set; }

        public string error { get; set; }

        [JsonPropertyName("error_description")]
        public string errorDescription { get; set; }

        [JsonPropertyName("error_uri")]
        public string errorUri { get; set; }
    }
}
