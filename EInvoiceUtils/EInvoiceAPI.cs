using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using EInvoiceUtils.Models;

namespace EInvoiceUtils
{
    public partial class EInvoiceAPI
    {
        private string clientId;
        private string clientSecret;
        private string apiUrl;
        private HttpClient client;

        /// <summary>
        ///     Constructs an object that exposes <see href="https://sdk.myinvois.hasil.gov.my/api/">LHDN Platform and EInvoice APIs</see>.
        ///     <example>
        ///     <code>
        ///     EInvoiceAPI client = new EInvoiceAPI("e2032d40-173a-4caf-ac4b-99c392c581cf",
        ///                                          "38e9fe23-e100-4da0-878c-c1e53de3a3dc",
        ///                                          "preprod-api.myinvois.hasil.gov.my");
        ///     </code>
        ///     </example>
        /// </summary>
        /// <param name="clientId">UUIDv4 string representing client ID for the ERP system, obtained from LHDN MyInvois portal.</param>
        /// <param name="clientSecret">UUIDv4 string representing client secret for the ERP system, obtained from LHDN MyInvois portal.</param>
        /// <param name="apiUrl">Environment URL provided by LHDN for the APIs to be called in (Prod/Sandbox)</param>
        public EInvoiceAPI(string clientId, string clientSecret, string apiUrl)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.apiUrl = apiUrl;

            this.client = new HttpClient();

            UriBuilder builder = new UriBuilder("https", this.apiUrl, 443);
            client.BaseAddress = builder.Uri;
        }

        /// <summary>
        ///     <see href="https://sdk.myinvois.hasil.gov.my/api/07-login-as-taxpayer-system/">
        ///         Login as Taxpayer System
        ///     </see>
        /// </summary>
        /// <returns>
        ///     <see cref="LoginAsTaxpayerResponse"/>
        /// </returns>
        public async Task<LoginAsTaxpayerResponse>  LoginAsTaxpayer()
        {
            LoginAsTaxpayerResponse response;

            using (HttpRequestMessage requestMessage = new HttpRequestMessage())
            {
                requestMessage.Method = HttpMethod.Post;
                requestMessage.RequestUri = new Uri("connect/token", UriKind.Relative);
                requestMessage.Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("client_id", this.clientId),
                    new KeyValuePair<string, string>("client_secret", this.clientSecret),
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("scope", "InvoicingAPI")
                });

                HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

                response = JsonSerializer.Deserialize<LoginAsTaxpayerResponse>(await responseMessage.Content.ReadAsStringAsync());
                response.statusCode = (int)responseMessage.StatusCode;
            }
            return response;
        }

        /// <summary>
        ///     <see href="https://sdk.myinvois.hasil.gov.my/api/08-login-as-intermediary-system/">
        ///         Login as Intermediary System
        ///     </see>
        /// </summary>
        /// <returns>
        ///     <see cref="LoginAsIntermediaryResponse"/>
        /// </returns>
        public async Task<LoginAsIntermediaryResponse> LoginAsIntermediary(string onBehalfOf)
        {
            LoginAsIntermediaryResponse response;

            using (HttpRequestMessage requestMessage = new HttpRequestMessage())
            {
                requestMessage.Method = HttpMethod.Post;
                requestMessage.RequestUri = new Uri("connect/token", UriKind.Relative);
                requestMessage.Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("client_id", this.clientId),
                    new KeyValuePair<string, string>("client_secret", this.clientSecret),
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("scope", "InvoicingAPI")
                });
                requestMessage.Headers.Add("onbehalfof", onBehalfOf);

                HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

                response = JsonSerializer.Deserialize<LoginAsIntermediaryResponse>(await responseMessage.Content.ReadAsStringAsync());
                response.statusCode = (int)responseMessage.StatusCode;
            }
            return response;
        }
    }
}
