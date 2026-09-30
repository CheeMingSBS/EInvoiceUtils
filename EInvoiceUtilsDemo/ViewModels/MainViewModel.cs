using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SBS.Core.EInvoiceUtils;
using SBS.Core.EInvoiceUtils.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SBS.Core.EInvoiceUtilsDemo.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? OnBehalfOf { get; set; }

        public EInvoiceAPI? Client { get; set; }
        public string? AccessToken { get; set; }
        public DateTime? TimeUntilAccessTokenExpiry { get; set; }

        OpenFileDialog? SubmitDocumentDialog { get; set; }

        [ObservableProperty]
        private string? selectedMode;

        [ObservableProperty]
        private string? loggedInAs = "Test";

        [ObservableProperty]
        private string? output;

        public ObservableCollection<InvoiceLineItem> InvoiceLineItems { get; set; }

        public MainViewModel()
        {
            this.InvoiceLineItems = new ObservableCollection<InvoiceLineItem>()
            {
                new InvoiceLineItem()
                {
                    Header = "Line Item 1",
                    ID = "INV-12345_1",
                    ClassificationCode = "004",
                    Description = "E_1234567_1234",
                    UnitPrice = "5.66",
                    Quantity = "1",
                    TaxableAmount = "5.66",
                    TaxAmount = "0.34",
                    TaxType = 1
                }
            };
        }

        #region EInvoice
        #region Login
        [RelayCommand]
        private async Task Login(object? values)
        {
            #region Input Validation
            if (values == null)
                return;

            object[] inputs = (object[])values;

            TextBox? clientId = inputs[0] as TextBox;
            PasswordBox? clientSecret = inputs[1] as PasswordBox;
            TextBox? onBehalfOf = inputs[2] as TextBox;
            ComboBox? environment = inputs[3] as ComboBox;
            string? apiUrl;

            if (clientId != null)
                this.ClientId = clientId.Text;

            if (clientSecret != null)
                this.ClientSecret = clientSecret.Password;

            if (string.IsNullOrWhiteSpace(this.ClientId) || string.IsNullOrWhiteSpace(this.ClientSecret))
                return;

            if (onBehalfOf != null)
                this.OnBehalfOf = onBehalfOf.Text;

            if (environment?.SelectedIndex == 0)
                apiUrl = "preprod-api.myinvois.hasil.gov.my";
            else
                apiUrl = "api.myinvois.hasil.gov.my";
            #endregion

            #region Handling
            this.Client = new EInvoiceAPI(this.ClientId, this.ClientSecret, apiUrl);
            try
            {
                if (string.IsNullOrWhiteSpace(this.OnBehalfOf))
                {
                    await this.LoginAsTaxpayer();
                    this.LoggedInAs = "Taxpayer";
                }
                else
                {
                    await this.LoginAsIntermediary();
                    this.LoggedInAs = "Intermediary";
                }
            }
            catch
            {
                // TODO: Validation message
            }
            #endregion
        }

        private async Task LoginAsTaxpayer()
        {
            LoginAsTaxpayerResponse response = await this.Client!.LoginAsTaxpayer();
            if (response.StatusCode == 200)
            {
                this.AccessToken = response.AccessToken;
                this.TimeUntilAccessTokenExpiry = DateTime.Now.AddSeconds(response.ExpiresIn);
            }
            else
            {
                // TODO: Validation message
                throw new Exception();
            }
        }

        private async Task LoginAsIntermediary()
        {
            LoginAsIntermediaryResponse response = await this.Client!.LoginAsIntermediary(this.OnBehalfOf);
            if (response.StatusCode == 200)
            {
                this.AccessToken = response.AccessToken;
                this.TimeUntilAccessTokenExpiry = DateTime.Now.AddSeconds(response.ExpiresIn);
            }
            else
            {
                // TODO: Validation message
                throw new Exception();
            }
        }
        #endregion

        #region Validate Taxpayer TIN
        [RelayCommand]
        private async Task ValidateTaxpayerTIN(object? values)
        {
            #region Input Validation
            if (values == null)
                return;

            object[] inputs = (object[])values;

            string? tin = (inputs[0] as TextBox)?.Text ?? null;
            int idType = (inputs[1] as ComboBox)?.SelectedIndex ?? -1;
            string? id = (inputs[2] as TextBox)?.Text ?? null;

            if (string.IsNullOrWhiteSpace(tin) || string.IsNullOrWhiteSpace(id))
                return;
            #endregion

            #region Handling
            if (DateTime.Now > this.TimeUntilAccessTokenExpiry)
            {
                if (string.IsNullOrWhiteSpace(this.OnBehalfOf))
                    await this.LoginAsTaxpayer();
                else
                    await this.LoginAsIntermediary();
            }

            ValidateTaxpayerTINResponse response = await this.Client!.ValidateTaxpayerTIN(this.AccessToken, tin, (TaxpayerType)(idType + 1), id);

            this.Output = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
            #endregion
        }
        #endregion

        #region Submit Documents
        [RelayCommand]
        private async Task SubmitDocuments(object? values)
        {
            #region Input Validation
            if (values == null)
                return;

            object[] inputs = (object[])values;

            string? invoiceCodeNumber = (inputs[0] as TextBox)?.Text ?? null;
            int documentType = (inputs[1] as ComboBox)?.SelectedIndex ?? -1;

            if (string.IsNullOrWhiteSpace(invoiceCodeNumber) || this.SubmitDocumentDialog == null || !this.SubmitDocumentDialog.CheckPathExists)
                return;
            #endregion

            Stream stream = this.SubmitDocumentDialog.OpenFile();
            byte[] buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer);

            #region Handling
            if (DateTime.Now > this.TimeUntilAccessTokenExpiry)
            {
                if (string.IsNullOrWhiteSpace(this.OnBehalfOf))
                    await this.LoginAsTaxpayer();
                else
                    await this.LoginAsIntermediary();
            }

            SubmitDocumentsResponse response = await this.Client!.SubmitDocuments(
                                                        this.AccessToken,
                                                        (DocumentFormat)(documentType + 1),
                                                        new Dictionary<string, string>()
                                                        {
                                                            { invoiceCodeNumber, Encoding.UTF8.GetString(buffer) }
                                                        }
                                                     );

            this.Output = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
            #endregion
        }
        #endregion

        #region Get Submission
        [RelayCommand]
        private async Task GetSubmission(TextBox? submissionUid)
        {
            #region Input Validation
            if (submissionUid == null)
                return;

            if (string.IsNullOrWhiteSpace(submissionUid.Text))
                return;
            #endregion

            #region Handling
            if (DateTime.Now > this.TimeUntilAccessTokenExpiry)
            {
                if (string.IsNullOrWhiteSpace(this.OnBehalfOf))
                    await this.LoginAsTaxpayer();
                else
                    await this.LoginAsIntermediary();
            }

            GetSubmissionResponse response = await this.Client!.GetSubmission(this.AccessToken, submissionUid.Text);

            this.Output = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
            #endregion
        }
        #endregion

        #region Get Document Details
        [RelayCommand]
        private async Task GetDocumentDetails(TextBox? uuid)
        {
            #region Input Validation
            if (uuid == null)
                return;

            if (string.IsNullOrWhiteSpace(uuid.Text))
                return;
            #endregion

            #region Handling
            if (DateTime.Now > this.TimeUntilAccessTokenExpiry)
            {
                if (string.IsNullOrWhiteSpace(this.OnBehalfOf))
                    await this.LoginAsTaxpayer();
                else
                    await this.LoginAsIntermediary();
            }

            GetDocumentDetailsResponse response = await this.Client!.GetDocumentDetails(this.AccessToken, uuid.Text);

            this.Output = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
            #endregion
        }
        #endregion

        #region CreateDocument
        [RelayCommand]
        private void CreateDocument(object? values)
        {
            if (values == null || this.InvoiceLineItems.Count == 0)
                return;

            string? id = (((object[])values)[0] as TextBox)?.Text ?? null;
            int? documentType = (((object[])values)[1] as ComboBox)?.SelectedIndex ?? -1;

            if (string.IsNullOrWhiteSpace(id))
                return;
            
            try
            {
                EInvoiceDocument document = new EInvoiceDocument(id);
                foreach (InvoiceLineItem item in this.InvoiceLineItems)
                {
                    if (string.IsNullOrWhiteSpace(item.ID))
                        throw new Exception();

                    // TODO: Check for int values only
                    if (!Enum.TryParse(item.ClassificationCode, out ClassificationCode classificationCode))
                        throw new Exception();

                    if (string.IsNullOrWhiteSpace(item.Description))
                        throw new Exception();

                    if (!decimal.TryParse(item.UnitPrice, out decimal unitPrice))
                        throw new Exception();

                    if (!decimal.TryParse(item.TaxableAmount, out decimal taxableAmount))
                        throw new Exception();

                    if (!decimal.TryParse(item.TaxAmount, out decimal taxAmount))
                        throw new Exception();

                    if (!int.TryParse(item.Quantity, out int quantity))
                        throw new Exception();

                    document.AddInvoiceLineItem(new InvoiceLineArgs(
                        item.ID, classificationCode,
                        item.Description, unitPrice,
                        new List<InvoiceLineTaxSubtotalArgs>()
                        {
                            new InvoiceLineTaxSubtotalArgs(taxableAmount, taxAmount, (TaxType)(item.TaxType! + 1))
                        }
                    )
                    {
                        Quantity = quantity
                    });
                }
            }
            catch
            {
                // TODO: Validation message
            }
        }
        #endregion
        #endregion

        #region UI-related
        [RelayCommand]
        private void Logout()
        {
            this.ClientId = null;
            this.ClientSecret = null;
            this.OnBehalfOf = null;
            this.LoggedInAs = null;
        }

        [RelayCommand]
        private void CloseDropdownOnSelect(ComboBox? comboBox)
        {
            if (comboBox == null)
                return;

            comboBox.IsDropDownOpen = false;
        }

        [RelayCommand]
        private void UpdateMainUIOnSelect(object? values)
        {
            if (values == null)
                return;

            if (((object[])values)[0] is string selection)
                this.SelectedMode = selection;

            if (((object[])values)[1] is ListView listview)
                listview.SelectedIndex = -1;

            if (((object[])values)[2] is ToggleButton popup)
                popup.IsChecked = false;
        }

        [RelayCommand]
        private void AttachFile(TextBox? textBlock)
        {
            if (textBlock == null)
                return;

            this.SubmitDocumentDialog = new OpenFileDialog()
            {
                Filter = "txt files (*.txt)|*.txt|JSON files (*.json)|*.json|XML files (*.xml)|*.xml"
            };

            bool? result = this.SubmitDocumentDialog.ShowDialog();

            if (result.HasValue && result.Value && textBlock != null)
                textBlock.Text = this.SubmitDocumentDialog.SafeFileName;
        }

        [RelayCommand]
        private void AddInvoiceLineItem()
        {
            this.InvoiceLineItems.Add(new InvoiceLineItem()
            {
                Header = $"Line Item {this.InvoiceLineItems.Count + 1}",
                ID = $"INV-12345_{this.InvoiceLineItems.Count + 1}",
                ClassificationCode = "004",
                Description = "E_1234567_1234",
                UnitPrice = "5.66",
                Quantity = "1",
                TaxableAmount = "5.66",
                TaxAmount = "0.34",
                TaxType = 1
            });
        }

        [RelayCommand]
        private void RemoveInvoiceLineItem(int num)
        {
            this.InvoiceLineItems.RemoveAt(num);
        }
        #endregion

        public class InvoiceLineItem
        {
            public string? Header { get; set; }
            public string? ID { get; set; }
            public string? ClassificationCode { get; set; }
            public string? Description { get; set; }
            public string? UnitPrice { get; set; }
            public string? Quantity { get; set; }
            public string? TaxableAmount { get; set; }
            public string? TaxAmount { get; set; }
            public int? TaxType { get; set; }
        }
    }
}
