using EInvoiceUtils.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Xml;

namespace EInvoiceUtils
{
    public partial class EInvoiceAPI
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string apiUrl;
        private readonly HttpClient client;

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
        /// <param name="apiUrl">Environment URL provided by LHDN for the APIs to be called in (Prod/Sandbox).</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public EInvoiceAPI(string clientId, string clientSecret, string apiUrl)
        {
            if (clientId == null)
                throw new ArgumentNullException($"{nameof(clientId)} cannot be NULL.", nameof(clientId));
            else if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentException($"{nameof(clientId)} cannot be empty.", nameof(clientId));

            if (clientSecret == null)
                throw new ArgumentNullException($"{nameof(clientSecret)} cannot be NULL.", nameof(clientSecret));
            else if (string.IsNullOrWhiteSpace(clientSecret))
                throw new ArgumentException($"{nameof(clientSecret)} cannot be empty.", nameof(clientSecret));

            if (apiUrl == null)
                throw new ArgumentNullException($"{nameof(apiUrl)} cannot be NULL.", nameof(apiUrl));
            else if (string.IsNullOrWhiteSpace(apiUrl))
                throw new ArgumentException($"{nameof(apiUrl)} cannot be empty.", nameof(apiUrl));


            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.apiUrl = apiUrl;

            this.client = new HttpClient();

            UriBuilder builder = new UriBuilder(this.apiUrl);
            builder.Scheme = "https";
            builder.Port = 443;
            client.BaseAddress = builder.Uri;
        }

        /// <summary>
        ///     Login as a specific taxpayer based on the supplied client ID and secret when constructing the object.
        ///     <br/>
        ///     See <see href="https://sdk.myinvois.hasil.gov.my/api/07-login-as-taxpayer-system/">Login as Taxpayer System</see>.
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
                requestMessage.RequestUri = new Uri("/connect/token", UriKind.Relative);
                requestMessage.Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("client_id", this.clientId),
                    new KeyValuePair<string, string>("client_secret", this.clientSecret),
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("scope", "InvoicingAPI")
                });

                HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

                response = JsonSerializer.Deserialize<LoginAsTaxpayerResponse>(await responseMessage.Content.ReadAsStringAsync());
                response.StatusCode = (int)responseMessage.StatusCode;
            }
            return response;
        }

        /// <summary>
        ///     Login on behalf of a specific taxpayer based on the given TIN.
        ///     <br/>
        ///     See <see href="https://sdk.myinvois.hasil.gov.my/api/08-login-as-intermediary-system/">Login as Intermediary System</see>.
        /// </summary>
        /// <returns>
        ///     <see cref="LoginAsIntermediaryResponse"/>
        /// </returns>
        /// <param name="onBehalfOf">TIN of the taxpayer to login on behalf of.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public async Task<LoginAsIntermediaryResponse> LoginAsIntermediary(string onBehalfOf)
        {
            if (onBehalfOf == null)
                throw new ArgumentNullException($"{nameof(onBehalfOf)} cannot be NULL.", nameof(onBehalfOf));
            else if (string.IsNullOrWhiteSpace(onBehalfOf))
                throw new ArgumentException($"{nameof(onBehalfOf)} cannot be empty.", nameof(onBehalfOf));

            LoginAsIntermediaryResponse response;

            using (HttpRequestMessage requestMessage = new HttpRequestMessage())
            {
                requestMessage.Method = HttpMethod.Post;
                requestMessage.RequestUri = new Uri("/connect/token", UriKind.Relative);
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
                response.StatusCode = (int)responseMessage.StatusCode;
            }
            return response;
        }

        /// <summary>
        ///     Submit well-formed EInvoice document(s) to LHDN.
        ///     <br/>
        ///     See <see href="https://sdk.myinvois.hasil.gov.my/einvoicingapi/02-submit-documents/">Submit Documents</see>.
        /// </summary>
        /// <param name="accessToken">Acess token from one of the Login APIs.</param>
        /// <param name="format">Format of the documents to be submitted.</param>
        /// <param name="documents">A dictionary of invoice code numbers each mapped to a well-formed EInvoice document following the format specified.</param>
        /// <returns>
        ///     <see cref="SubmitDocumentsResponse"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public async Task<SubmitDocumentsResponse> SubmitDocuments(
            string accessToken,
            SubmitDocumentFormat format,
            Dictionary<string, string> documents
        )
        {
            if (accessToken == null)
                throw new ArgumentNullException($"{nameof(accessToken)} cannot be NULL.", nameof(accessToken));
            else if (string.IsNullOrWhiteSpace(accessToken))
                throw new ArgumentException($"{nameof(accessToken)} cannot be empty.", nameof(accessToken));

            if (documents == null)
                throw new ArgumentNullException($"{nameof(documents)} cannot be NULL.", nameof(documents));
            else if (documents.Count == 0)
                throw new ArgumentException($"{nameof(documents)} cannot be empty.", nameof(documents));

            SubmitDocumentsResponse response;
            List<SubmitDocumentsDocument> processedDocuments = new List<SubmitDocumentsDocument>();

            using (SHA256 sha256 = SHA256.Create())
            {
                foreach (string codeNumber in documents.Keys)
                {
                    if (format == SubmitDocumentFormat.XML)
                    {
                        XmlDocument xml = new XmlDocument();
                        xml.PreserveWhitespace = false;
                        xml.LoadXml(documents[codeNumber]);

                        documents[codeNumber] = xml.OuterXml;
                    }

                    processedDocuments.Add(new SubmitDocumentsDocument()
                    {
                        Format = Enum.GetName(typeof(SubmitDocumentFormat), format),
                        CodeNumber = codeNumber,
                        Document = Convert.ToBase64String(Encoding.UTF8.GetBytes(documents[codeNumber])),
                        DocumentHash = ConvertToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(documents[codeNumber])))
                    });

                }
            }

            using (HttpRequestMessage requestMessage = new HttpRequestMessage())
            {
                requestMessage.Method = HttpMethod.Post;
                requestMessage.RequestUri = new Uri("/api/v1.0/documentsubmissions/", UriKind.Relative);
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                requestMessage.Content = JsonContent.Create(new SubmitDocumentsRequest() { Documents = processedDocuments });

                HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

                JsonSerializerOptions options = new JsonSerializerOptions();
                options.Converters.Add(new ErrorConverter());

                response = JsonSerializer.Deserialize<SubmitDocumentsResponse>(await responseMessage.Content.ReadAsStringAsync(), options);
                response.StatusCode = (int)responseMessage.StatusCode;
            }
            return response;
        }

        /// <summary>
        ///     Validate the given combination of TIN and ID.
        ///     <br/>
        ///     See <see href="https://sdk.myinvois.hasil.gov.my/einvoicingapi/01-validate-taxpayer-tin/"/>.
        /// </summary>
        /// <param name="accessToken">Acess token from one of the Login APIs.</param>
        /// <param name="tin">TIN of taxpayer to be validated.</param>
        /// <param name="idType">Identity type of taxpayer to be validated.</param>
        /// <param name="idValue">Identification number of taxpayer to be validated.</param>
        /// <returns>
        ///     <see cref="ValidateTaxpayerTINResponse"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public async Task<ValidateTaxpayerTINResponse> ValidateTaxpayerTIN(
            string accessToken,
            string tin,
            TaxpayerType idType,
            string idValue
        )
        {
            if (accessToken == null)
                throw new ArgumentNullException($"{nameof(accessToken)} cannot be NULL.", nameof(accessToken));
            else if (string.IsNullOrWhiteSpace(accessToken))
                throw new ArgumentException($"{nameof(accessToken)} cannot be empty.", nameof(accessToken));

            if (tin == null)
                throw new ArgumentNullException($"{nameof(tin)} cannot be NULL.", nameof(tin));
            else if (string.IsNullOrWhiteSpace(tin))
                throw new ArgumentException($"{nameof(tin)} cannot be empty.", nameof(tin));

            if (idValue == null)
                throw new ArgumentNullException($"{nameof(idValue)} cannot be NULL.", nameof(idValue));
            else if (string.IsNullOrWhiteSpace(idValue))
                throw new ArgumentException($"{nameof(idValue)} cannot be empty.", nameof(idValue));

            ValidateTaxpayerTINResponse response;

            using (HttpRequestMessage requestMessage = new HttpRequestMessage())
            {
                requestMessage.Method = HttpMethod.Get;
                requestMessage.RequestUri = new Uri($"/api/v1.0/taxpayer/validate/{tin}?idType={Enum.GetName(typeof(TaxpayerType), idType)}&idValue={idValue}", UriKind.Relative);
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

                response = new ValidateTaxpayerTINResponse();
                response.statusCode = (int)responseMessage.StatusCode;
            }
            return response;
        }

        private string ConvertToHexString(byte[] buffer)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Byte b in buffer)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }
    }
}
