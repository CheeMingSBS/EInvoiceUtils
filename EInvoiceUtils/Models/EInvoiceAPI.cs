using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EInvoiceUtils.Models
{
    #region Standard Response
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
    #endregion

    #region Login As Taxpayer
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
    #endregion

    #region Login As Intermediary
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
    #endregion

    #region Submit Documents
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
    #endregion

    #region Validate Taxpayer TIN
    /// <summary>
    ///     <see href="https://sdk.myinvois.hasil.gov.my/einvoicingapi/01-validate-taxpayer-tin/#outputs">Validate Taxpayer TIN API Outputs</see>
    /// </summary>
    public class ValidateTaxpayerTINResponse
    {
        public int StatusCode { get; set; }
    }
    #endregion

    #region Get Submission
    /// <summary>
    ///     <see href="https://sdk.myinvois.hasil.gov.my/einvoicingapi/06-get-submission/#outputs"/>Get Submission API Outputs</see>
    /// </summary>
    public class GetSubmissionResponse
    {
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("documentCount")]
        public int DocumentCount { get; set; }

        [JsonPropertyName("overallStatus")]
        public string OverallStatus { get; set; }

        [JsonPropertyName("documentSummary")]
        public List<GetSubmissionResponseDocumentSummary> DocumentSummary { get; set; }

        [JsonPropertyName("error")]
        public Error Error { get; set; }
    }

    /// <summary>
    ///     <see href="https://sdk.myinvois.hasil.gov.my/einvoicingapi/06-get-submission/#document-summary">Summary of document</see> associated with the Submission UID sent in a Get Submission API call.
    /// </summary>
    public class GetSubmissionResponseDocumentSummary
    {
        [JsonPropertyName("uuid")]
        public string UUID { get; set; }

        [JsonPropertyName("longId")]
        public string LongID { get; set; }

        [JsonPropertyName("internalId")]
        public string InternalID { get; set; }

        [JsonPropertyName("totalExcludingTax")]
        public decimal TotalExcludingTax { get; set; }

        [JsonPropertyName("totalDiscount")]
        public decimal TotalDiscount { get; set; }

        [JsonPropertyName("totalNetAmount")]
        public decimal TotalNetAmount { get; set; }

        [JsonPropertyName("totalPayableAmount")]
        public decimal TotalPayableAmount { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
    #endregion
}
