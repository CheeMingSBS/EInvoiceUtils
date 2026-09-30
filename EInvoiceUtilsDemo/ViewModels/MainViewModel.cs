using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SBS.Core.EInvoiceUtils;
using SBS.Core.EInvoiceUtils.Models;
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
        private string? loggedInAs;

        [ObservableProperty]
        private string? output;

        public RelayCommand<object> LoginCommand { get; set; }
        public RelayCommand<object> ValidateTaxpayerTINCommand { get; set; }
        public RelayCommand<object> SubmitDocumentsCommand { get; set; }
        public RelayCommand<TextBox> GetSubmissionCommand { get; set; }
        public RelayCommand<TextBox> GetDocumentDetailsCommand { get; set; }

        public RelayCommand LogoutCommand { get; set; }
        public RelayCommand<ComboBox> CloseDropdownOnSelectCommand { get; set; }
        public RelayCommand<object> UpdateMainUIOnSelectCommand { get; set; }
        public RelayCommand<TextBox> AttachFileCommand { get; set; }

        public MainViewModel()
        {
            this.LoginCommand = new RelayCommand<object>(Login, CanLogin);
            this.ValidateTaxpayerTINCommand = new RelayCommand<object>(ValidateTaxpayerTIN, CanValidateTaxpayerTIN);
            this.SubmitDocumentsCommand = new RelayCommand<object>(SubmitDocuments, CanSubmitDocuments);
            this.GetSubmissionCommand = new RelayCommand<TextBox>(GetSubmission, CanGetSubmission);
            this.GetDocumentDetailsCommand = new RelayCommand<TextBox>(GetDocumentDetails, CanGetDocumentDetails);

            this.LogoutCommand = new RelayCommand(Logout, CanLogout);
            this.CloseDropdownOnSelectCommand = new RelayCommand<ComboBox>(CloseDropdownOnSelect, CanCloseDropdownOnSelect);
            this.UpdateMainUIOnSelectCommand = new RelayCommand<object>(UpdateMainUIOnSelect, CanUpdateMainUIOnSelect);
            this.AttachFileCommand = new RelayCommand<TextBox>(AttachFile, CanAttachFile);
        }

        #region EInvoice
        #region Login
        public bool CanLogin(object? values)
        {
            return true;
        }

        public async void Login(object? values)
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
        public bool CanValidateTaxpayerTIN(object? values)
        {
            return true;
        }

        public async void ValidateTaxpayerTIN(object? values)
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
        public bool CanSubmitDocuments(object? values)
        {
            return true;
        }

        public async void SubmitDocuments(object? values)
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
        public bool CanGetSubmission(TextBox? submissionUid)
        {
            return true;
        }

        public async void GetSubmission(TextBox? submissionUid)
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
        public bool CanGetDocumentDetails(TextBox? uuid)
        {
            return true;
        }

        public async void GetDocumentDetails(TextBox? uuid)
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
        #endregion

        #region UI-related
        public bool CanLogout()
        {
            return true;
        }

        public void Logout()
        {
            this.ClientId = null;
            this.ClientSecret = null;
            this.OnBehalfOf = null;
            this.LoggedInAs = null;
        }

        public bool CanCloseDropdownOnSelect(ComboBox? comboBox)
        {
            return true;
        }

        public void CloseDropdownOnSelect(ComboBox? comboBox)
        {
            if (comboBox == null)
                return;

            comboBox.IsDropDownOpen = false;
        }

        public bool CanUpdateMainUIOnSelect(object? values)
        {
            return true;
        }

        public void UpdateMainUIOnSelect(object? values)
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

        public bool CanAttachFile(TextBox? textBlock)
        {
            return true;
        }

        public void AttachFile(TextBox? textBlock)
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
        #endregion
    }
}
