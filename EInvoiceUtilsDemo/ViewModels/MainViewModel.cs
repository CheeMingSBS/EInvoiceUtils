using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EInvoiceUtilsDemo;
using Microsoft.Win32;
using SBS.Core.EInvoiceUtils;
using SBS.Core.EInvoiceUtils.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SBS.Core.EInvoiceUtilsDemo.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        public EInvoiceAPI? Client { get; set; }
        public string? AccessToken { get; set; }
        public DateTime? TimeUntilAccessTokenExpiry { get; set; }

        #region UI Input & Bindings
        #region Login
        [ObservableProperty]
        private string? clientId;

        private string? ClientSecret { get; set; }

        [ObservableProperty]
        private string? onBehalfOf;

        [ObservableProperty]
        private int? environment;
        #endregion

        #region Main UI
        [ObservableProperty]
        private string? selectedMode;

        [ObservableProperty]
        private string? loggedInAs;

        [ObservableProperty]
        private string? output;
        #endregion

        #region Validate Taxpayer TIN
        [ObservableProperty]
        private string? validateTaxpayerTINInput;

        [ObservableProperty]
        private int? validateTaxpayerIDTypeInput;

        [ObservableProperty]
        private string? validateTaxpayerIDInput;
        #endregion

        #region Submit Documents
        [ObservableProperty]
        private string? submitDocumentsCodeNumberInput;

        [ObservableProperty]
        private int? submitDocumentsDocumentFormatInput;

        [ObservableProperty]
        private string? submitDocumentsFileInput;

        OpenFileDialog? SubmitDocumentDialog { get; set; }
        #endregion

        #region Get Submission
        [ObservableProperty]
        private string? getSubmissionUIDInput;
        #endregion

        #region Get Document Details
        [ObservableProperty]
        private string? getDocumentDetailsUUIDInput;
        #endregion

        #region Create Document
        [ObservableProperty]
        private string? createDocumentIDInput;

        [ObservableProperty]
        private string? createDocumentSupplierNameInput;

        [ObservableProperty]
        private string? createDocumentSupplierTINInput;

        [ObservableProperty]
        private int? createDocumentSupplierIDTypeInput;

        [ObservableProperty]
        private string? createDocumentSupplierIDInput;

        [ObservableProperty]
        private string? createDocumentSupplierBusinessDescriptionInput;

        [ObservableProperty]
        private string? createDocumentSupplierAddressLine1Input;

        [ObservableProperty]
        private string? createDocumentSupplierAddressLine2Input;

        [ObservableProperty]
        private string? createDocumentSupplierAddressLine3Input;

        [ObservableProperty]
        private string? createDocumentSupplierCityNameInput;

        [ObservableProperty]
        private string? createDocumentSupplierPostalZoneInput;

        [ObservableProperty]
        private int? createDocumentSupplierStateInput;

        [ObservableProperty]
        private string? createDocumentSupplierEmailInput;

        [ObservableProperty]
        private string? createDocumentSupplierContactNumberInput;

        [ObservableProperty]
        private string? createDocumentCustomerNameInput;

        [ObservableProperty]
        private string? createDocumentCustomerTINInput;

        [ObservableProperty]
        private int? createDocumentCustomerIDTypeInput;

        [ObservableProperty]
        private string? createDocumentCustomerIDInput;

        [ObservableProperty]
        private string? createDocumentCustomerAddressLine1Input;

        [ObservableProperty]
        private string? createDocumentCustomerAddressLine2Input;

        [ObservableProperty]
        private string? createDocumentCustomerAddressLine3Input;

        [ObservableProperty]
        private string? createDocumentCustomerCityNameInput;

        [ObservableProperty]
        private string? createDocumentCustomerPostalZoneInput;

        [ObservableProperty]
        private int? createDocumentCustomerStateInput;

        [ObservableProperty]
        private string? createDocumentCustomerEmailInput;

        [ObservableProperty]
        private string? createDocumentCustomerContactNumberInput;

        public ObservableCollection<InvoiceLineItem>? InvoiceLineItems { get; set; }
        #endregion
        #endregion

        public MainViewModel()
        {
            this.Logout();
            this.Reset();
            this.LoggedInAs = "Test";
        }

        #region Library Usage Logic
        #region Login
        [RelayCommand]
        private async Task Login(PasswordBox? clientSecret)
        {
            #region Input Validation
            if (clientSecret == null)
                return;

            if (string.IsNullOrWhiteSpace(this.ClientId) || string.IsNullOrWhiteSpace(clientSecret.Password))
            {
                clientSecret.Password = string.Empty;
                return;
            }
            #endregion

            this.ClientSecret = clientSecret.Password;
            string apiUrl = this.Environment == 0 ? "preprod-api.myinvois.hasil.gov.my" : "api.myinvois.hasil.gov.my";

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
            clientSecret.Password = string.Empty;
            #endregion
        }

        private async Task LoginAsTaxpayer()
        {
            LoginAsTaxpayerResponse response = await this.Client!.LoginAsTaxpayer();

            // TODO: Use a Windows manager/service?
            LoginResponseWindow window = new LoginResponseWindow()
            {
                DataContext = new LoginResponseViewModel(JsonSerializer.Serialize(response, new JsonSerializerOptions() { WriteIndented = true }))
            };
            window.Show();

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

            // TODO: Use a Windows manager/service?
            LoginResponseWindow window = new LoginResponseWindow()
            {
                DataContext = new LoginResponseViewModel(JsonSerializer.Serialize(response, new JsonSerializerOptions() { WriteIndented = true }))
            };
            window.Show();

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
        private async Task ValidateTaxpayerTIN()
        {
            #region Input Validation
            if (string.IsNullOrWhiteSpace(this.ValidateTaxpayerTINInput) || string.IsNullOrWhiteSpace(this.ValidateTaxpayerIDInput))
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

            ValidateTaxpayerTINResponse response = await this.Client!.ValidateTaxpayerTIN(
                                                        this.AccessToken,
                                                        this.ValidateTaxpayerTINInput,
                                                        (TaxpayerType)this.ValidateTaxpayerIDTypeInput! + 1,
                                                        this.ValidateTaxpayerIDInput
                                                   );

            this.Output = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
            #endregion
        }
        #endregion

        #region Submit Documents
        [RelayCommand]
        private async Task SubmitDocuments()
        {
            #region Input Validation
            if (string.IsNullOrWhiteSpace(this.SubmitDocumentsCodeNumberInput) || this.SubmitDocumentDialog == null || !this.SubmitDocumentDialog.CheckPathExists)
                return;
            #endregion

            #region Handling
            using (StreamReader reader = new StreamReader(this.SubmitDocumentDialog.OpenFile()))
            {
                if (DateTime.Now > this.TimeUntilAccessTokenExpiry)
                {
                    if (string.IsNullOrWhiteSpace(this.OnBehalfOf))
                        await this.LoginAsTaxpayer();
                    else
                        await this.LoginAsIntermediary();
                }

                SubmitDocumentsResponse response = await this.Client!.SubmitDocuments(
                                                            this.AccessToken,
                                                            (DocumentFormat)this.SubmitDocumentsDocumentFormatInput! + 1,
                                                            new Dictionary<string, string>()
                                                            {
                                                                { this.SubmitDocumentsCodeNumberInput, await reader.ReadToEndAsync() }
                                                            }
                                                         );

                this.Output = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
            }
            #endregion
        }
        #endregion

        #region Get Submission
        [RelayCommand]
        private async Task GetSubmission()
        {
            #region Input Validation
            if (string.IsNullOrWhiteSpace(this.GetSubmissionUIDInput))
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

            GetSubmissionResponse response = await this.Client!.GetSubmission(this.AccessToken, this.GetSubmissionUIDInput);

            this.Output = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
            #endregion
        }
        #endregion

        #region Get Document Details
        [RelayCommand]
        private async Task GetDocumentDetails()
        {
            #region Input Validation
            if (string.IsNullOrWhiteSpace(this.GetDocumentDetailsUUIDInput))
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

            GetDocumentDetailsResponse response = await this.Client!.GetDocumentDetails(this.AccessToken, this.GetDocumentDetailsUUIDInput);

            this.Output = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
            #endregion
        }
        #endregion

        #region CreateDocument
        [RelayCommand]
        private void CreateDocument()
        {
            if (string.IsNullOrWhiteSpace(this.CreateDocumentIDInput))
                return;

            if (string.IsNullOrWhiteSpace(this.CreateDocumentSupplierNameInput) || string.IsNullOrWhiteSpace(this.CreateDocumentCustomerNameInput))
                return;

            if (string.IsNullOrWhiteSpace(this.CreateDocumentSupplierTINInput) || string.IsNullOrWhiteSpace(this.CreateDocumentCustomerTINInput))
                return;

            if (string.IsNullOrWhiteSpace(this.CreateDocumentSupplierIDInput) || string.IsNullOrWhiteSpace(this.CreateDocumentCustomerIDInput))
                return;

            if (string.IsNullOrWhiteSpace(this.CreateDocumentSupplierBusinessDescriptionInput))
                return;

            if (string.IsNullOrWhiteSpace(this.CreateDocumentSupplierCityNameInput) || string.IsNullOrWhiteSpace(this.CreateDocumentCustomerCityNameInput))
                return;

            if (string.IsNullOrWhiteSpace(this.CreateDocumentSupplierContactNumberInput) || string.IsNullOrWhiteSpace(this.CreateDocumentCustomerContactNumberInput))
                return;
            
            try
            {
                EInvoiceDocument document = new EInvoiceDocument(this.CreateDocumentIDInput);

                document.SetAccountingSupplierParty(new AccountingSupplierPartyArgs(
                    this.CreateDocumentSupplierNameInput, this.CreateDocumentSupplierTINInput,
                    (TaxpayerType)this.CreateDocumentSupplierIDTypeInput! + 1, this.CreateDocumentSupplierIDInput,
                    MSIC.OPERATION_OF_PARKING_FACILITIES_FOR_MOTOR_VEHICLES_PARKING_LOTS, this.CreateDocumentSupplierCityNameInput,
                    (State)this.CreateDocumentSupplierStateInput! + 1, CountryCode.MALAYSIA,
                    this.CreateDocumentSupplierContactNumberInput
                )
                {
                    BusinessDescription = this.CreateDocumentSupplierBusinessDescriptionInput,
                    PostalZone = this.CreateDocumentSupplierPostalZoneInput,
                    AddressLine0 = this.CreateDocumentSupplierAddressLine1Input,
                    AddressLine1 = this.CreateDocumentSupplierAddressLine2Input,
                    AddressLine2 = this.CreateDocumentSupplierAddressLine3Input,
                    Email = this.CreateDocumentSupplierEmailInput
                });

                document.SetAccountingCustomerParty(new AccountingCustomerPartyArgs(
                    this.CreateDocumentCustomerNameInput, this.CreateDocumentCustomerTINInput,
                    (TaxpayerType)this.CreateDocumentCustomerIDTypeInput! + 1, this.CreateDocumentCustomerIDInput,
                    this.CreateDocumentCustomerCityNameInput, (State)this.CreateDocumentCustomerStateInput! + 1,
                    CountryCode.MALAYSIA, this.CreateDocumentCustomerContactNumberInput
                )
                {
                    PostalZone = this.CreateDocumentCustomerPostalZoneInput,
                    AddressLine0 = this.CreateDocumentCustomerAddressLine1Input,
                    AddressLine1 = this.CreateDocumentCustomerAddressLine2Input,
                    AddressLine2 = this.CreateDocumentCustomerAddressLine3Input,
                    Email = this.CreateDocumentCustomerEmailInput
                });

                foreach (InvoiceLineItem item in this.InvoiceLineItems!)
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
                            new InvoiceLineTaxSubtotalArgs(taxableAmount, taxAmount, (TaxType)item.TaxType! + 1)
                        }
                    )
                    {
                        Quantity = quantity
                    });
                }

                this.Output = document.Export(DocumentFormat.JSON, true);
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
            this.Environment = (int)TaxpayerType.NRIC - 1;
            this.LoggedInAs = null;
            this.Output = null;
            this.Reset();
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

            if (((object[])values)[0] is ListView listView && listView.SelectedItem is ListViewItem item)
            {
                this.SelectedMode = item.Content as string;
                listView.SelectedIndex = -1;
            }

            if (((object[])values)[1] is ListView listview)
                listview.SelectedIndex = -1;

            if (((object[])values)[2] is ToggleButton popup)
                popup.IsChecked = false;

            this.Reset();
        }

        [RelayCommand]
        private void AttachFile()
        {
            this.SubmitDocumentDialog = new OpenFileDialog()
            {
                Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt|JSON files (*.json)|*.json|XML files (*.xml)|*.xml"
            };

            bool? result = this.SubmitDocumentDialog.ShowDialog();

            if (result.HasValue && result.Value)
                this.SubmitDocumentsFileInput = this.SubmitDocumentDialog.SafeFileName;
        }

        [RelayCommand]
        private void AddInvoiceLineItem()
        {
            this.InvoiceLineItems!.Add(new InvoiceLineItem()
            {
                Num = this.InvoiceLineItems.Count,
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
            this.InvoiceLineItems!.RemoveAt(num);

            for (int i = num; i <  this.InvoiceLineItems.Count; i++)
            {
                this.InvoiceLineItems[i].Num = i;
                this.InvoiceLineItems[i].Header = $"Line Item {num + 1}";
            }

            this.InvoiceLineItems = new ObservableCollection<InvoiceLineItem>(this.InvoiceLineItems);
            OnPropertyChanged(nameof(this.InvoiceLineItems));
        }

        private void Reset()
        {
            this.ValidateTaxpayerTINInput = null;
            this.ValidateTaxpayerIDTypeInput = (int)TaxpayerType.NRIC - 1; ;
            this.ValidateTaxpayerIDInput = null;

            this.SubmitDocumentsCodeNumberInput = null;
            this.SubmitDocumentsDocumentFormatInput = (int)DocumentFormat.JSON - 1;
            this.SubmitDocumentsFileInput = null;
            this.SubmitDocumentDialog = null;

            this.GetSubmissionUIDInput = null;

            this.GetDocumentDetailsUUIDInput = null;

            this.CreateDocumentIDInput = "INV-12345";
            this.CreateDocumentSupplierNameInput = "Test";
            this.CreateDocumentSupplierTINInput = null;
            this.CreateDocumentSupplierIDTypeInput = (int)TaxpayerType.NRIC - 1;
            this.CreateDocumentSupplierIDInput = null;
            this.CreateDocumentSupplierBusinessDescriptionInput = "Carpark Operator";
            this.CreateDocumentSupplierAddressLine1Input = "X Unit 07-06, Vertical Tower A";
            this.CreateDocumentSupplierAddressLine2Input = "No. 8 Jalan Kerinchi, Bangsar South";
            this.CreateDocumentSupplierAddressLine3Input = null;
            this.CreateDocumentSupplierCityNameInput = "Kuala Lumpur";
            this.CreateDocumentSupplierPostalZoneInput = "50490";
            this.CreateDocumentSupplierStateInput = (int)State.WILAYAH_PERSEKUTUAN_KUALA_LUMPUR - 1;
            this.CreateDocumentSupplierEmailInput = "test@test.com";
            this.CreateDocumentSupplierContactNumberInput = "01234567890";
            this.CreateDocumentCustomerNameInput = "Test";
            this.CreateDocumentCustomerTINInput = null;
            this.CreateDocumentCustomerIDTypeInput = (int)TaxpayerType.NRIC - 1;
            this.CreateDocumentCustomerIDInput = null;
            this.CreateDocumentCustomerAddressLine1Input = "X Unit 07-06, Vertical Tower A";
            this.CreateDocumentCustomerAddressLine2Input = "No. 8 Jalan Kerinchi, Bangsar South";
            this.CreateDocumentCustomerAddressLine3Input = null;
            this.CreateDocumentCustomerCityNameInput = "Kuala Lumpur";
            this.CreateDocumentCustomerPostalZoneInput = "50490";
            this.CreateDocumentCustomerStateInput = (int)State.WILAYAH_PERSEKUTUAN_KUALA_LUMPUR - 1;
            this.CreateDocumentCustomerEmailInput = "test@test.com";
            this.CreateDocumentCustomerContactNumberInput = "01234567890";

            if (this.InvoiceLineItems == null || this.InvoiceLineItems.Count > 1)
            {
                this.InvoiceLineItems = new ObservableCollection<InvoiceLineItem>()
                {
                    new InvoiceLineItem()
                    {
                        Num = 0,
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
        }

        [RelayCommand]
        private void UseConsolidatedPreset()
        {
            this.CreateDocumentCustomerNameInput = "General Public";
            this.CreateDocumentCustomerTINInput = "EI00000000010";
            this.CreateDocumentCustomerIDTypeInput = (int)TaxpayerType.NRIC - 1;
            this.CreateDocumentCustomerIDInput = "NA";
            this.CreateDocumentCustomerAddressLine1Input = "NA";
            this.CreateDocumentCustomerAddressLine2Input = null;
            this.CreateDocumentCustomerAddressLine3Input = null;
            this.CreateDocumentCustomerCityNameInput = null;
            this.CreateDocumentCustomerPostalZoneInput = null;
            this.CreateDocumentCustomerStateInput = (int)State.WILAYAH_PERSEKUTUAN_KUALA_LUMPUR - 1;
            this.CreateDocumentCustomerEmailInput = null;
            this.CreateDocumentCustomerContactNumberInput = "NA";
        }

        [RelayCommand]
        private void UseIndividualPreset()
        {
            this.CreateDocumentCustomerNameInput = "Test";
            this.CreateDocumentCustomerTINInput = null;
            this.CreateDocumentCustomerIDTypeInput = (int)TaxpayerType.NRIC - 1;
            this.CreateDocumentCustomerIDInput = null;
            this.CreateDocumentCustomerAddressLine1Input = "X Unit 07-06, Vertical Tower A";
            this.CreateDocumentCustomerAddressLine2Input = "No. 8 Jalan Kerinchi, Bangsar South";
            this.CreateDocumentCustomerAddressLine3Input = null;
            this.CreateDocumentCustomerCityNameInput = "Kuala Lumpur";
            this.CreateDocumentCustomerPostalZoneInput = "50490";
            this.CreateDocumentCustomerStateInput = (int)State.WILAYAH_PERSEKUTUAN_KUALA_LUMPUR - 1;
            this.CreateDocumentCustomerEmailInput = "test@test.com";
            this.CreateDocumentCustomerContactNumberInput = "01234567890";
        }
        #endregion

        public class InvoiceLineItem
        {
            public int? Num { get; set; }
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
