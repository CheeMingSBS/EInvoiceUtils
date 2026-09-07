using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EInvoiceUtils.Models
{
    /// <summary>
    ///     <see href="https://sdk.myinvois.hasil.gov.my/standard-error-response/#error-response-structure">Standard Error Response</see>
    /// </summary>
    public class Error
    {
        [JsonPropertyName("propertyName")]
        public string PropertyName { get; set; }

        [JsonPropertyName("propertyPath")]
        public string PropertyPath { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("details")]
        public List<Error> Details { get; set; }
    }

    /// <summary>
    ///     <see href="https://sdk.myinvois.hasil.gov.my/api/07-login-as-taxpayer-system/#outputs">Login as Taxpayer API Outputs</see>
    /// </summary>
    public class LoginAsTaxpayerResponse
    {
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }

        [JsonPropertyName("error_uri")]
        public string ErrorUri { get; set; }
    }

    /// <summary>
    ///     <see href="https://sdk.myinvois.hasil.gov.my/api/08-login-as-intermediary-system/#outputs">Login as Intermediary API Outputs</see>
    /// </summary>
    public class LoginAsIntermediaryResponse
    {
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }

        [JsonPropertyName("error_uri")]
        public string ErrorUri { get; set; }
    }

    public class SubmitDocumentsRequest
    {
        [JsonPropertyName("documents")]
        public List<SubmitDocumentsDocument> Documents { get; set; }
    }

    /// <summary>
    ///     <see href="https://sdk.myinvois.hasil.gov.my/einvoicingapi/02-submit-documents/#single-document-consists-of">Document</see> to be attached to request body during a Submit Documents API call
    /// </summary>
    public class SubmitDocumentsDocument
    {
        [JsonPropertyName("format")]
        public string Format { get; set; }

        [JsonPropertyName("document")]
        public string Document { get; set; }

        [JsonPropertyName("documentHash")]
        public string DocumentHash { get; set; }

        [JsonPropertyName("codeNumber")]
        public string CodeNumber { get; set; }
    }

    /// <summary>
    ///     <see href="https://sdk.myinvois.hasil.gov.my/einvoicingapi/02-submit-documents/#outputs">Submit Documents API Outputs</see>
    /// </summary>
    public class SubmitDocumentsResponse
    {
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("submissionUid")]
        public string SubmissionUid { get; set; }

        [JsonPropertyName("acceptedDocuments")]
        public List<SubmitDocumentsResponseAcceptedDocuments> AcceptedDocuments { get; set; }

        [JsonPropertyName("rejectedDocuments")]
        public List<SubmitDocumentsResponseRejectedDocuments> RejectedDocuments { get; set; }

        [JsonPropertyName("error")]
        public Error Error { get; set; }
    }

    /// <summary>
    ///     <see href="https://sdk.myinvois.hasil.gov.my/einvoicingapi/02-submit-documents/#accepted-documents">Accepted Documents</see> returned from a successful Submit Documents API call
    /// </summary>
    public class SubmitDocumentsResponseAcceptedDocuments
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("invoiceCodeNumber")]
        public string InvoiceCodeNumber { get; set; }
    }

    /// <summary>
    ///     <see href="https://sdk.myinvois.hasil.gov.my/einvoicingapi/02-submit-documents/#rejected-documents">Rejected Documents</see> returned from a successful Submit Documents API call
    /// </summary>
    public class SubmitDocumentsResponseRejectedDocuments
    {
        [JsonPropertyName("invoiceCodeNumber")]
        public string InvoiceCodeNumber { get; set; }

        [JsonPropertyName("error")]
        public Error Error { get; set; }
    }

    /// <summary>
    ///     <see href="https://sdk.myinvois.hasil.gov.my/einvoicingapi/01-validate-taxpayer-tin/#outputs">Validate Taxpayer TIN API Outputs</see>
    /// </summary>
    public class ValidateTaxpayerTINResponse
    {
        public int statusCode { get; set; }
    }
}
