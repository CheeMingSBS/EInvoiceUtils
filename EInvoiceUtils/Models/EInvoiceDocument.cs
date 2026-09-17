using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EInvoiceUtils.Models
{
    public class EInvoiceDocument
    {
        // Universal Business Language (UBL) schema in XML namespace
        public string _D { get; } = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
        public string _A { get; } = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
        public string _B { get; } = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";

        public List<Invoice> Invoice { get; set; }

        public EInvoiceDocument(
            string id,
            DateTime? issueDateTime = null,
            InvoiceType type = InvoiceType.INVOICE,
            InvoiceVersion version = InvoiceVersion.V_1_0,
            CurrencyCode currencyCode = CurrencyCode.MALAYSIAN_RINGGIT
        )
        {
            this.Invoice = new List<Invoice>() { new Invoice() };

            this.SetInvoiceID(id);

            if (issueDateTime == null)
                issueDateTime = DateTime.Now;

            this.SetIssueDate(issueDateTime.Value);
            this.SetIssueTime(issueDateTime.Value);
            this.SetInvoiceTypeCode(type, version);
            this.SetDocumentCurrencyCode(currencyCode);
            this.SetTaxCurrencyCode(currencyCode);

        }

        #region e-Invoice Code / Number (Invoice\:ID)
        public void SetInvoiceID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("E-Invoice Code / Number is invalid.", nameof(id));

            if (this.Invoice[0].ID == null)
                this.Invoice[0].ID = new List<InvoiceID>() { new InvoiceID() };

            this.Invoice[0].ID[0]._ = id;
        }
        #endregion

        #region e-Invoice Date (IssueDate)
        public void SetIssueDate(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
                throw new ArgumentException("E-Invoice Date is invalid.", nameof(date));

            if (this.Invoice[0].IssueDate == null)
                this.Invoice[0].IssueDate = new List<IssueDate>() { new IssueDate() };

            this.Invoice[0].IssueDate[0]._ = date;
        }

        public void SetIssueDate(DateTime date)
        {
            if (date == null)
                throw new ArgumentNullException("E-Invoice Date is invalid.", nameof(date));

            if (this.Invoice[0].IssueDate == null)
                this.Invoice[0].IssueDate = new List<IssueDate>() { new IssueDate() };

            this.Invoice[0].IssueDate[0]._ = date.ToUniversalTime().ToString("yyyy-MM-dd");
        }
        #endregion

        #region e-Invoice Time (IssueTime)
        public void SetIssueTime(string time)
        {
            if (string.IsNullOrWhiteSpace(time))
                throw new ArgumentException("E-Invoice Time is invalid", nameof(time));

            if (this.Invoice[0].IssueTime == null)
                this.Invoice[0].IssueTime = new List<IssueTime>() { new IssueTime() };

            this.Invoice[0].IssueTime[0]._ = time;
        }

        public void SetIssueTime(DateTime time)
        {
            if (time == null)
                throw new ArgumentNullException("E-Invoice Date is invalid.", nameof(time));

            if (this.Invoice[0].IssueTime == null)
                this.Invoice[0].IssueTime = new List<IssueTime>() { new IssueTime() };

            this.Invoice[0].IssueTime[0]._ = time.ToUniversalTime().ToString("HH:mm:ss") + "Z";
        }
        #endregion

        #region e-Invoice Type Code (InvoiceTypeCode)
        public void SetInvoiceTypeCode(string type, string listVersionId)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("E-Invoice Type Code is invalid", nameof(type));

            if (string.IsNullOrWhiteSpace(listVersionId))
                throw new ArgumentException("E-Invoice Version is invalid", nameof(listVersionId));

            if (this.Invoice[0].InvoiceTypeCode == null)
                this.Invoice[0].InvoiceTypeCode = new List<InvoiceTypeCode>() { new InvoiceTypeCode() };

            this.Invoice[0].InvoiceTypeCode[0]._ = type;
            this.Invoice[0].InvoiceTypeCode[0].ListVersionID = listVersionId;
        }

        public void SetInvoiceTypeCode(InvoiceType type, InvoiceVersion version)
        {
            string listVersionId;

            switch (version)
            {
                case InvoiceVersion.V_1_0:
                    listVersionId = "1.0";
                    break;

                case InvoiceVersion.V_1_1:
                    listVersionId = "1.1";
                    break;

                default:
                    listVersionId = "1.0";
                    break;
            }

            if (this.Invoice[0].InvoiceTypeCode == null)
                this.Invoice[0].InvoiceTypeCode = new List<InvoiceTypeCode>() { new InvoiceTypeCode() };

            this.Invoice[0].InvoiceTypeCode[0]._ = ((int)type).ToString().PadLeft(2, '0');
            this.Invoice[0].InvoiceTypeCode[0].ListVersionID = listVersionId;
        }
        #endregion

        #region Invoice Currency Code (DocumentCurrencyCode)
        public void SetDocumentCurrencyCode(string currencyCode)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("E-Invoice Currency Code is invalid", nameof(currencyCode));

            if (this.Invoice[0].DocumentCurrencyCode == null)
                this.Invoice[0].DocumentCurrencyCode = new List<DocumentCurrencyCode>() { new DocumentCurrencyCode() };

            this.Invoice[0].DocumentCurrencyCode[0]._ = currencyCode;
        }

        public void SetDocumentCurrencyCode(CurrencyCode currencyCode)
        {
            if (this.Invoice[0].DocumentCurrencyCode == null)
                this.Invoice[0].DocumentCurrencyCode = new List<DocumentCurrencyCode>() { new DocumentCurrencyCode() };

            this.Invoice[0].DocumentCurrencyCode[0]._ = EnumUtils.ConvertCurrencyNameToCode(currencyCode);
        }
        #endregion

        #region Invoice Currency Code (TaxCurrencyCode)
        public void SetTaxCurrencyCode(string currencyCode)
        {
            if (this.Invoice[0].TaxCurrencyCode == null)
                this.Invoice[0].TaxCurrencyCode = new List<TaxCurrencyCode>() { new TaxCurrencyCode() };

            this.Invoice[0].TaxCurrencyCode[0]._ = currencyCode;
        }

        public void SetTaxCurrencyCode(CurrencyCode? currencyCode)
        {
            if (this.Invoice[0].TaxCurrencyCode == null)
                this.Invoice[0].TaxCurrencyCode = new List<TaxCurrencyCode>() { new TaxCurrencyCode() };

            this.Invoice[0].TaxCurrencyCode[0]._ = EnumUtils.ConvertCurrencyNameToCode(currencyCode.Value);
        }

        public void RemoveTaxCurrencyCode()
        {
            if (this.Invoice[0].TaxCurrencyCode != null)
                this.Invoice[0].TaxCurrencyCode = null;
        }
        #endregion

        #region Billing Period Start Date (StartDate)
        public void SetInvoicePeriodStartDate(string date)
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.InvoicePeriod == null)
                invoice.InvoicePeriod = new List<InvoicePeriod>() { new InvoicePeriod() };

            if (invoice.InvoicePeriod[0].StartDate == null)
            {
                invoice.InvoicePeriod[0].StartDate = new List<StartDate>() { new StartDate() };
                invoice.InvoicePeriod[0].children++;
            }

            invoice.InvoicePeriod[0].StartDate[0]._ = date;
        }

        public void SetInvoicePeriodStartDate(DateTime date)
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.InvoicePeriod == null)
                invoice.InvoicePeriod = new List<InvoicePeriod>() { new InvoicePeriod() };

            if (invoice.InvoicePeriod[0].StartDate == null)
            {
                invoice.InvoicePeriod[0].StartDate = new List<StartDate>() { new StartDate() };
                invoice.InvoicePeriod[0].children++;
            }

            invoice.InvoicePeriod[0].StartDate[0]._ = date.ToUniversalTime().ToString("yyyy-MM-dd");
        }

        public void RemoveInvoicePeriodStartDate()
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.InvoicePeriod != null && invoice.InvoicePeriod[0].StartDate != null)
            {
                invoice.InvoicePeriod[0].StartDate = null;
                invoice.InvoicePeriod[0].children--;

                if (invoice.InvoicePeriod[0].children == 0)
                    invoice.InvoicePeriod = null;
            }
        }
        #endregion

        #region Billing Period End Date (EndDate)
        public void SetInvoicePeriodEndDate(string date)
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.InvoicePeriod == null)
                invoice.InvoicePeriod = new List<InvoicePeriod>() { new InvoicePeriod() };

            if (invoice.InvoicePeriod[0].EndDate == null)
            {
                invoice.InvoicePeriod[0].EndDate = new List<EndDate>() { new EndDate() };
                invoice.InvoicePeriod[0].children++;
            }

            invoice.InvoicePeriod[0].EndDate[0]._ = date;
        }

        public void SetInvoicePeriodEndDate(DateTime date)
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.InvoicePeriod == null)
                invoice.InvoicePeriod = new List<InvoicePeriod>() { new InvoicePeriod() };

            if (invoice.InvoicePeriod[0].EndDate == null)
            {
                invoice.InvoicePeriod[0].EndDate = new List<EndDate>() { new EndDate() };
                invoice.InvoicePeriod[0].children++;
            }

            invoice.InvoicePeriod[0].EndDate[0]._ = date.ToUniversalTime().ToString("yyyy-MM-dd");
        }

        public void RemoveInvoicePeriodEndDate()
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.InvoicePeriod != null && invoice.InvoicePeriod[0].EndDate != null)
            {
                invoice.InvoicePeriod[0].EndDate = null;
                invoice.InvoicePeriod[0].children--;

                if (invoice.InvoicePeriod[0].children == 0)
                    invoice.InvoicePeriod = null;
            }
        }
        #endregion

        #region Frequency of Billing (Description)
        public void SetInvoicePeriodDescription(string description)
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.InvoicePeriod == null)
                invoice.InvoicePeriod = new List<InvoicePeriod>() { new InvoicePeriod() };

            if (invoice.InvoicePeriod[0].Description == null)
            {
                invoice.InvoicePeriod[0].Description = new List<Description>() { new Description() };
                invoice.InvoicePeriod[0].children++;
            }

            invoice.InvoicePeriod[0].Description[0]._ = description;
        }

        public void RemoveInvoicePeriodDescription()
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.InvoicePeriod != null && invoice.InvoicePeriod[0].Description != null)
            {
                invoice.InvoicePeriod[0].Description = null;
                invoice.InvoicePeriod[0].children--;

                if (invoice.InvoicePeriod[0].children == 0)
                    invoice.InvoicePeriod = null;
            }
        }
        #endregion

        #region Bill Reference Number (BillingReference:AdditionalDocumentReference:ID)
        public void SetBillingReference(string id)
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.BillingReference == null)
                invoice.BillingReference = new List<BillingReference>() { new BillingReference() };

            if (invoice.BillingReference[0].AdditionalDocumentReference == null)
                invoice.BillingReference[0].AdditionalDocumentReference = new List<AdditionalDocumentReference>() { new AdditionalDocumentReference() };

            if (invoice.BillingReference[0].AdditionalDocumentReference[0].ID == null)
                invoice.BillingReference[0].AdditionalDocumentReference[0].ID = new List<AdditionalDocumentReferenceID>() { new AdditionalDocumentReferenceID() };

            invoice.BillingReference[0].AdditionalDocumentReference[0].ID[0]._ = id;
        }

        public void RemoveBillingReference()
        {
            if (this.Invoice[0].BillingReference != null)
                this.Invoice[0].BillingReference = null;
        }
        #endregion

        #region Supplier (AccountingSupplierParty)
        public void SetAccountingSupplierParty(AccountingSupplierPartyArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.Name))
                throw new ArgumentException("Supplier Name is invalid.", nameof(args.Name));

            if (string.IsNullOrWhiteSpace(args.TIN))
                throw new ArgumentException("Supplier TIN is invalid.", nameof(args.TIN));

            if (string.IsNullOrWhiteSpace(args.ID))
                throw new ArgumentException("Supplier ID is invalid.", nameof(args.ID));

            if (string.IsNullOrWhiteSpace(args.Name))
                throw new ArgumentException("Supplier Name is invalid.", nameof(args.Name));

            if (string.IsNullOrWhiteSpace(args.CityName))
                throw new ArgumentException("Supplier City Name is invalid.", nameof(args.CityName));

            if (string.IsNullOrWhiteSpace(args.ContactNumber))
                throw new ArgumentException("Supplier Contact Number is invalid.", nameof(args.ContactNumber));

            if (this.Invoice[0].AccountingSupplierParty == null)
                this.Invoice[0].AccountingSupplierParty = new List<AccountingSupplierParty>() { new AccountingSupplierParty() };

            if (this.Invoice[0].AccountingSupplierParty[0].Party == null)
                this.Invoice[0].AccountingSupplierParty[0].Party = new List<Party>() { new Party() };

            Party supplierParty = this.Invoice[0].AccountingSupplierParty[0].Party[0];

            if (supplierParty.PartyLegalEntity == null)
                supplierParty.PartyLegalEntity = new List<PartyLegalEntity>() { new PartyLegalEntity() { RegistrationName = new List<RegistrationName>() { new RegistrationName() { _ = args.Name } } } };

            #region Supplier's MSIC Code (AccountingSupplierParty:Party:IndustryClassificationCode)
            if (supplierParty.IndustryClassificationCode == null)
                supplierParty.IndustryClassificationCode = new List<IndustryClassificationCode>() { new IndustryClassificationCode() };

            supplierParty.IndustryClassificationCode[0]._ = ((int)args.MSIC).ToString().PadLeft(5, '0');

            if (!string.IsNullOrWhiteSpace(args.BusinessDescription))
                supplierParty.IndustryClassificationCode[0].Name = args.BusinessDescription;
            else
                supplierParty.IndustryClassificationCode[0].Name = EnumUtils.ConvertMSICNameToDescription(args.MSIC);
            #endregion

            #region Supplier's Registration / Identification Number / Passport Number (AccountingSupplierParty:Party:PartyIdentification)
            if (supplierParty.PartyIdentification == null)
                supplierParty.PartyIdentification = new List<PartyIdentification>();

            supplierParty.PartyIdentification.Add(new PartyIdentification() { ID = new List<PartyIdentificationID>() { new PartyIdentificationID() { _ = args.TIN, SchemeID = "TIN" } } });
            supplierParty.PartyIdentification.Add(new PartyIdentification() { ID = new List<PartyIdentificationID>() { new PartyIdentificationID() { _ = args.ID, SchemeID = args.IDType.ToString() } } });
            supplierParty.PartyIdentification.Add(new PartyIdentification() { ID = new List<PartyIdentificationID>() { new PartyIdentificationID() { _ = args.SST, SchemeID = "SST" } } });
            supplierParty.PartyIdentification.Add(new PartyIdentification() { ID = new List<PartyIdentificationID>() { new PartyIdentificationID() { _ = args.TTX, SchemeID = "TTX" } } });
            #endregion

            #region Supplier's Address (AccountingSupplierParty:Party:Address)
            if (supplierParty.PostalAddress == null)
                supplierParty.PostalAddress = new List<PostalAddress>() { new PostalAddress() };

            PostalAddress supplierPartyPostalAddress = supplierParty.PostalAddress[0];

            if (supplierPartyPostalAddress.AddressLine == null)
            {
                supplierPartyPostalAddress.AddressLine = new List<AddressLine>() { new AddressLine() };
                supplierPartyPostalAddress.AddressLine[0].Line = new List<Line>() { new Line() };
            }

            if (string.IsNullOrWhiteSpace(args.AddressLine0))
                args.AddressLine0 = "NA";

            supplierPartyPostalAddress.AddressLine[0].Line[0]._ = args.AddressLine0;

            if (args.AddressLine0 != "NA" && !string.IsNullOrWhiteSpace(args.AddressLine1))
            {
                if (supplierPartyPostalAddress.AddressLine.Count > 1)
                    supplierPartyPostalAddress.AddressLine[1].Line[0]._ = args.AddressLine1;
                else
                    supplierPartyPostalAddress.AddressLine.Add(new AddressLine() { Line = new List<Line>() { new Line() { _ = args.AddressLine1 } } });

                if (!string.IsNullOrWhiteSpace(args.AddressLine2))
                {
                    if (supplierPartyPostalAddress.AddressLine.Count > 2)
                        supplierPartyPostalAddress.AddressLine[2].Line[0]._ = args.AddressLine2;
                    else
                        supplierPartyPostalAddress.AddressLine.Add(new AddressLine() { Line = new List<Line>() { new Line() { _ = args.AddressLine2 } } });
                }
            }

            if (!string.IsNullOrWhiteSpace(args.PostalZone))
            {
                if (supplierPartyPostalAddress.PostalZone == null)
                    supplierPartyPostalAddress.PostalZone = new List<PostalZone>() { new PostalZone() };

                supplierPartyPostalAddress.PostalZone[0]._ = args.PostalZone;
            }

            if (supplierPartyPostalAddress.CityName == null)
                supplierPartyPostalAddress.CityName = new List<CityName>() { new CityName() };

            supplierPartyPostalAddress.CityName[0]._ = args.CityName;

            if (supplierPartyPostalAddress.CountrySubentityCode == null)
                supplierPartyPostalAddress.CountrySubentityCode = new List<CountrySubentityCode>() { new CountrySubentityCode() };

            supplierPartyPostalAddress.CountrySubentityCode[0]._ = ((int)args.State).ToString().PadLeft(2, '0');

            if (supplierPartyPostalAddress.Country == null)
                supplierPartyPostalAddress.Country = new List<Country> { new Country() { IdentificationCode = new List<IdentificationCode>() { new IdentificationCode() } } };

            supplierPartyPostalAddress.Country[0].IdentificationCode[0]._ = EnumUtils.ConvertCountryNameToCode(args.Country);
            supplierPartyPostalAddress.Country[0].IdentificationCode[0].ListID = "ISO3166-1";
            supplierPartyPostalAddress.Country[0].IdentificationCode[0].ListAgencyID = "6";
            #endregion

            #region Supplier's Contact Number (AccountingSupplierParty:Party:Contact)
            if (supplierParty.Contact == null)
                supplierParty.Contact = new List<Contact>() { new Contact() { Telephone = new List<Telephone>() { new Telephone() } } };

            supplierParty.Contact[0].Telephone[0]._ = args.ContactNumber;

            if (!string.IsNullOrWhiteSpace(args.Email))
            {
                if (supplierParty.Contact[0].ElectronicMail == null)
                    supplierParty.Contact[0].ElectronicMail = new List<ElectronicMail>() { new ElectronicMail() };

                supplierParty.Contact[0].ElectronicMail[0]._ = args.Email;
            }
            #endregion
        }

        public void RemoveAccountingSupplierPartyAddressLine2()
        {
            if (this.Invoice[0].AccountingSupplierParty != null && this.Invoice[0].AccountingSupplierParty[0].Party[0].PostalAddress[0].AddressLine.Count > 1)
                this.Invoice[0].AccountingSupplierParty[0].Party[0].PostalAddress[0].AddressLine.RemoveRange(1, this.Invoice[0].AccountingSupplierParty[0].Party[0].PostalAddress[0].AddressLine.Count - 1);
        }

        public void RemoveAccountingSupplierPartyAddressLine3()
        {
            if (this.Invoice[0].AccountingSupplierParty != null && this.Invoice[0].AccountingSupplierParty[0].Party[0].PostalAddress[0].AddressLine.Count > 2)
                this.Invoice[0].AccountingSupplierParty[0].Party[0].PostalAddress[0].AddressLine.RemoveRange(2, this.Invoice[0].AccountingSupplierParty[0].Party[0].PostalAddress[0].AddressLine.Count - 1);
        }

        public void RemoveAccountingSupplierPartyPostalZone()
        {
            if (this.Invoice[0].AccountingSupplierParty != null && this.Invoice[0].AccountingSupplierParty[0].Party[0].PostalAddress[0].PostalZone != null)
                this.Invoice[0].AccountingSupplierParty[0].Party[0].PostalAddress[0].PostalZone = null;
        }

        public void RemoveAccountingSupplierPartyEmail()
        {
            if (this.Invoice[0].AccountingSupplierParty != null && this.Invoice[0].AccountingSupplierParty[0].Party[0].Contact[0].ElectronicMail != null)
                this.Invoice[0].AccountingSupplierParty[0].Party[0].Contact[0].ElectronicMail = null;
        }
        #endregion

        #region Buyer (AccountingCustomerParty)
        public void SetAccountingCustomerParty(AccountingCustomerPartyArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.Name))
                throw new ArgumentException("Buyer Name is invalid.", nameof(args.Name));

            if (string.IsNullOrWhiteSpace(args.TIN))
                throw new ArgumentException("Buyer TIN is invalid.", nameof(args.TIN));

            if (string.IsNullOrWhiteSpace(args.ID))
                throw new ArgumentException("Buyer ID is invalid.", nameof(args.ID));

            if (string.IsNullOrWhiteSpace(args.Name))
                throw new ArgumentException("Buyer Name is invalid.", nameof(args.Name));

            if (string.IsNullOrWhiteSpace(args.CityName))
                throw new ArgumentException("Buyer City Name is invalid.", nameof(args.CityName));

            if (string.IsNullOrWhiteSpace(args.ContactNumber))
                throw new ArgumentException("Buyer Contact Number is invalid.", nameof(args.ContactNumber));

            if (this.Invoice[0].AccountingCustomerParty == null)
                this.Invoice[0].AccountingCustomerParty = new List<AccountingCustomerParty>() { new AccountingCustomerParty() };

            if (this.Invoice[0].AccountingCustomerParty[0].Party == null)
                this.Invoice[0].AccountingCustomerParty[0].Party = new List<Party>() { new Party() };

            Party buyerParty = this.Invoice[0].AccountingCustomerParty[0].Party[0];

            if (buyerParty.PartyLegalEntity == null)
                buyerParty.PartyLegalEntity = new List<PartyLegalEntity>() { new PartyLegalEntity() { RegistrationName = new List<RegistrationName>() { new RegistrationName() { _ = args.Name } } } };

            #region Buyer's Registration / Identification Number / Passport Number (AccountingCustomerParty:Party:PartyIdentification)
            if (buyerParty.PartyIdentification == null)
                buyerParty.PartyIdentification = new List<PartyIdentification>();

            buyerParty.PartyIdentification.Add(new PartyIdentification() { ID = new List<PartyIdentificationID>() { new PartyIdentificationID() { _ = args.TIN, SchemeID = "TIN" } } });
            buyerParty.PartyIdentification.Add(new PartyIdentification() { ID = new List<PartyIdentificationID>() { new PartyIdentificationID() { _ = args.ID, SchemeID = args.IDType.ToString() } } });
            buyerParty.PartyIdentification.Add(new PartyIdentification() { ID = new List<PartyIdentificationID>() { new PartyIdentificationID() { _ = args.SST, SchemeID = "SST" } } });
            buyerParty.PartyIdentification.Add(new PartyIdentification() { ID = new List<PartyIdentificationID>() { new PartyIdentificationID() { _ = args.TTX, SchemeID = "TTX" } } });
            #endregion

            #region Buyer's Address (AccountingCustomerParty:Party:Address)
            if (buyerParty.PostalAddress == null)
                buyerParty.PostalAddress = new List<PostalAddress>() { new PostalAddress() };

            PostalAddress buyerPartyPostalAddress = buyerParty.PostalAddress[0];

            if (buyerPartyPostalAddress.AddressLine == null)
            {
                buyerPartyPostalAddress.AddressLine = new List<AddressLine>() { new AddressLine() };
                buyerPartyPostalAddress.AddressLine[0].Line = new List<Line>() { new Line() };
            }

            if (string.IsNullOrWhiteSpace(args.AddressLine0))
                args.AddressLine0 = "NA";

            buyerPartyPostalAddress.AddressLine[0].Line[0]._ = args.AddressLine0;

            if (args.AddressLine0 != "NA" && !string.IsNullOrWhiteSpace(args.AddressLine1))
            {
                if (buyerPartyPostalAddress.AddressLine.Count > 1)
                    buyerPartyPostalAddress.AddressLine[1].Line[0]._ = args.AddressLine1;
                else
                    buyerPartyPostalAddress.AddressLine.Add(new AddressLine() { Line = new List<Line>() { new Line() { _ = args.AddressLine1 } } });

                if (!string.IsNullOrWhiteSpace(args.AddressLine2))
                {
                    if (buyerPartyPostalAddress.AddressLine.Count > 2)
                        buyerPartyPostalAddress.AddressLine[2].Line[0]._ = args.AddressLine2;
                    else
                        buyerPartyPostalAddress.AddressLine.Add(new AddressLine() { Line = new List<Line>() { new Line() { _ = args.AddressLine2 } } });
                }
            }

            if (!string.IsNullOrWhiteSpace(args.PostalZone))
            {
                if (buyerPartyPostalAddress.PostalZone == null)
                    buyerPartyPostalAddress.PostalZone = new List<PostalZone>() { new PostalZone() };

                buyerPartyPostalAddress.PostalZone[0]._ = args.PostalZone;
            }

            if (buyerPartyPostalAddress.CityName == null)
                buyerPartyPostalAddress.CityName = new List<CityName>() { new CityName() };

            buyerPartyPostalAddress.CityName[0]._ = args.CityName;

            if (buyerPartyPostalAddress.CountrySubentityCode == null)
                buyerPartyPostalAddress.CountrySubentityCode = new List<CountrySubentityCode>() { new CountrySubentityCode() };

            buyerPartyPostalAddress.CountrySubentityCode[0]._ = ((int)args.State).ToString().PadLeft(2, '0');

            if (buyerPartyPostalAddress.Country == null)
                buyerPartyPostalAddress.Country = new List<Country> { new Country() { IdentificationCode = new List<IdentificationCode>() { new IdentificationCode() } } };

            buyerPartyPostalAddress.Country[0].IdentificationCode[0]._ = EnumUtils.ConvertCountryNameToCode(args.Country);
            buyerPartyPostalAddress.Country[0].IdentificationCode[0].ListID = "ISO3166-1";
            buyerPartyPostalAddress.Country[0].IdentificationCode[0].ListAgencyID = "6";
            #endregion

            #region Buyer's Contact Number (AccountingCustomerParty:Party:Contact)
            if (buyerParty.Contact == null)
                buyerParty.Contact = new List<Contact>() { new Contact() { Telephone = new List<Telephone>() { new Telephone() } } };

            buyerParty.Contact[0].Telephone[0]._ = args.ContactNumber;

            if (!string.IsNullOrWhiteSpace(args.Email))
            {
                if (buyerParty.Contact[0].ElectronicMail == null)
                    buyerParty.Contact[0].ElectronicMail = new List<ElectronicMail>() { new ElectronicMail() };

                buyerParty.Contact[0].ElectronicMail[0]._ = args.Email;
            }
            #endregion
        }

        public void RemoveAccountingCustomerPartyAddressLine2()
        {
            if (this.Invoice[0].AccountingCustomerParty != null && this.Invoice[0].AccountingCustomerParty[0].Party[0].PostalAddress[0].AddressLine.Count > 1)
                this.Invoice[0].AccountingCustomerParty[0].Party[0].PostalAddress[0].AddressLine.RemoveRange(1, this.Invoice[0].AccountingCustomerParty[0].Party[0].PostalAddress[0].AddressLine.Count - 1);
        }

        public void RemoveAccountingCustomerPartyAddressLine3()
        {
            if (this.Invoice[0].AccountingCustomerParty != null && this.Invoice[0].AccountingCustomerParty[0].Party[0].PostalAddress[0].AddressLine.Count > 2)
                this.Invoice[0].AccountingCustomerParty[0].Party[0].PostalAddress[0].AddressLine.RemoveRange(2, this.Invoice[0].AccountingCustomerParty[0].Party[0].PostalAddress[0].AddressLine.Count - 1);
        }

        public void RemoveAccountingCustomerPartyPostalZone()
        {
            if (this.Invoice[0].AccountingCustomerParty != null && this.Invoice[0].AccountingCustomerParty[0].Party[0].PostalAddress[0].PostalZone != null)
                this.Invoice[0].AccountingCustomerParty[0].Party[0].PostalAddress[0].PostalZone = null;
        }

        public void RemoveAccountingCustomerPartyEmail()
        {
            if (this.Invoice[0].AccountingCustomerParty != null && this.Invoice[0].AccountingCustomerParty[0].Party[0].Contact[0].ElectronicMail != null)
                this.Invoice[0].AccountingCustomerParty[0].Party[0].Contact[0].ElectronicMail = null;
        }
        #endregion

        #region Total Excluding Tax (TaxExclusiveAmount)
        private void CalculateTaxExclusiveAmount()
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.LegalMonetaryTotal == null)
                invoice.LegalMonetaryTotal = new List<LegalMonetaryTotal>() { new LegalMonetaryTotal() };

            if (invoice.LegalMonetaryTotal[0].TaxExclusiveAmount == null)
            {
                invoice.LegalMonetaryTotal[0].TaxExclusiveAmount = new List<TaxExclusiveAmount> { new TaxExclusiveAmount() };
                invoice.LegalMonetaryTotal[0].TaxExclusiveAmount[0].CurrencyID = invoice.DocumentCurrencyCode[0]._;
            }

            decimal totalNetAmount = invoice.LegalMonetaryTotal[0].LineExtensionAmount?[0]?._ ?? 0.0m;
            decimal totalAllowance = invoice.LegalMonetaryTotal[0].AllowanceTotalAmount?[0]?._ ?? 0.0m;
            decimal totalCharge = invoice.LegalMonetaryTotal[0].ChargeTotalAmount?[0]?._ ?? 0.0m;

            invoice.LegalMonetaryTotal[0].TaxExclusiveAmount[0]._ = totalNetAmount - totalAllowance + totalCharge;
        }
        #endregion

        #region Total Including Tax (TaxInclusiveAmount)
        private void CalculateTaxInclusiveAmount()
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.LegalMonetaryTotal == null)
                invoice.LegalMonetaryTotal = new List<LegalMonetaryTotal>() { new LegalMonetaryTotal() };

            if (invoice.LegalMonetaryTotal[0].TaxInclusiveAmount == null)
            {
                invoice.LegalMonetaryTotal[0].TaxInclusiveAmount = new List<TaxInclusiveAmount> { new TaxInclusiveAmount() };
                invoice.LegalMonetaryTotal[0].TaxInclusiveAmount[0].CurrencyID = invoice.DocumentCurrencyCode[0]._;
            }

            decimal taxExclusiveAmount = invoice.LegalMonetaryTotal[0].TaxExclusiveAmount?[0]?._ ?? 0.0m;
            decimal taxAmount = invoice.TaxTotal?[0].TaxAmount?[0]?._ ?? 0.0m;

            invoice.LegalMonetaryTotal[0].TaxInclusiveAmount[0]._ = taxExclusiveAmount + taxAmount;
        }
        #endregion

        #region Total Payable Amount (PayableAmount)
        private void CalculatePayableAmount()
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.LegalMonetaryTotal == null)
                invoice.LegalMonetaryTotal = new List<LegalMonetaryTotal>() { new LegalMonetaryTotal() };

            if (invoice.LegalMonetaryTotal[0].PayableAmount == null)
            {
                invoice.LegalMonetaryTotal[0].PayableAmount = new List<PayableAmount> { new PayableAmount() };
                invoice.LegalMonetaryTotal[0].PayableAmount[0].CurrencyID = invoice.DocumentCurrencyCode[0]._;
            }

            decimal taxInclusiveAmount = invoice.LegalMonetaryTotal[0].TaxInclusiveAmount?[0]?._ ?? 0.0m;
            decimal paidAmount = invoice.PrepaidPayment?[0].PaidAmount?[0]?._ ?? 0.0m;
            decimal payableRoundingAmount = invoice.LegalMonetaryTotal[0].PayableRoundingAmount?[0]?._ ?? 0.0m;

            invoice.LegalMonetaryTotal[0].PayableAmount[0]._ = taxInclusiveAmount - paidAmount + payableRoundingAmount;
        }
        #endregion

        #region Total Net Amount (Invoice:LegalMonetaryTotal:LineExtensionAmount)
        private void AddTotalNetAmount(decimal amount)
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.LegalMonetaryTotal == null)
                invoice.LegalMonetaryTotal = new List<LegalMonetaryTotal>() { new LegalMonetaryTotal() };

            if (invoice.LegalMonetaryTotal[0].LineExtensionAmount == null)
            {
                invoice.LegalMonetaryTotal[0].LineExtensionAmount = new List<LineExtensionAmount> { new LineExtensionAmount() };
                invoice.LegalMonetaryTotal[0].LineExtensionAmount[0].CurrencyID = invoice.DocumentCurrencyCode[0]._;
            }

            invoice.LegalMonetaryTotal[0].LineExtensionAmount[0]._ += amount;
        }

        public void RemoveTotalNetAmount()
        {
            if (this.Invoice[0].LegalMonetaryTotal != null && this.Invoice[0].LegalMonetaryTotal[0].LineExtensionAmount != null)
                this.Invoice[0].LegalMonetaryTotal[0].LineExtensionAmount = null;
        }
        #endregion

        #region Total Discount Value (AllowanceTotalAmount)
        public void AddAllowanceTotalAmount(decimal amount)
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.LegalMonetaryTotal == null)
                invoice.LegalMonetaryTotal = new List<LegalMonetaryTotal>() { new LegalMonetaryTotal() };

            if (invoice.LegalMonetaryTotal[0].AllowanceTotalAmount == null)
            {
                invoice.LegalMonetaryTotal[0].AllowanceTotalAmount = new List<AllowanceTotalAmount> { new AllowanceTotalAmount() };
                invoice.LegalMonetaryTotal[0].AllowanceTotalAmount[0].CurrencyID = invoice.DocumentCurrencyCode[0]._;
            }

            invoice.LegalMonetaryTotal[0].AllowanceTotalAmount[0]._ += amount;
        }

        public void RemoveAllowanceTotalAmount()
        {
            if (this.Invoice[0].LegalMonetaryTotal != null && this.Invoice[0].LegalMonetaryTotal[0].AllowanceTotalAmount != null)
                this.Invoice[0].LegalMonetaryTotal[0].AllowanceTotalAmount = null;
        }
        #endregion

        #region Total Fee / Charge Amount (ChargeTotalAmount)
        public void AddChargeTotalAmount(decimal amount)
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.LegalMonetaryTotal == null)
                invoice.LegalMonetaryTotal = new List<LegalMonetaryTotal>() { new LegalMonetaryTotal() };

            if (invoice.LegalMonetaryTotal[0].ChargeTotalAmount == null)
            {
                invoice.LegalMonetaryTotal[0].ChargeTotalAmount = new List<ChargeTotalAmount> { new ChargeTotalAmount() };
                invoice.LegalMonetaryTotal[0].ChargeTotalAmount[0].CurrencyID = invoice.DocumentCurrencyCode[0]._;
            }

            invoice.LegalMonetaryTotal[0].ChargeTotalAmount[0]._ += amount;
        }

        public void RemoveChargeTotalAmount()
        {
            if (this.Invoice[0].LegalMonetaryTotal != null && this.Invoice[0].LegalMonetaryTotal[0].ChargeTotalAmount != null)
                this.Invoice[0].LegalMonetaryTotal[0].ChargeTotalAmount = null;
        }
        #endregion

        #region Total Tax Amount (TaxAmount)
        private void CalculateTaxAmount()
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.TaxTotal == null)
                invoice.TaxTotal = new List<TaxTotal>() { new TaxTotal() };

            if (invoice.TaxTotal[0].TaxAmount == null)
            {
                invoice.TaxTotal[0].TaxAmount = new List<TaxAmount> { new TaxAmount() };
                invoice.TaxTotal[0].TaxAmount[0].CurrencyID = invoice.DocumentCurrencyCode[0]._;
            }

            if (invoice.TaxTotal[0].TaxSubtotal != null)
            {
                foreach (TaxSubtotal subtotal in invoice.TaxTotal[0].TaxSubtotal)
                {
                    if (subtotal.TaxCategory?[0]?.ID?[0]?._ == "E")
                        continue;

                    invoice.TaxTotal[0].TaxAmount[0]._ += subtotal.TaxAmount[0]._;
                }
            }
        }
        #endregion

        #region Rounding Amount (PayableRoundingAmount)
        public void SetPayableRoundingAmount(decimal amount)
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.LegalMonetaryTotal == null)
                invoice.LegalMonetaryTotal = new List<LegalMonetaryTotal>() { new LegalMonetaryTotal() };

            if (invoice.LegalMonetaryTotal[0].PayableRoundingAmount == null)
            {
                invoice.LegalMonetaryTotal[0].PayableRoundingAmount = new List<PayableRoundingAmount> { new PayableRoundingAmount() };
                invoice.LegalMonetaryTotal[0].PayableRoundingAmount[0].CurrencyID = invoice.DocumentCurrencyCode[0]._;
            }

            invoice.LegalMonetaryTotal[0].PayableRoundingAmount[0]._ += amount;
        }

        public void RemovePayableRoundingAmount()
        {
            if (this.Invoice[0].LegalMonetaryTotal != null && this.Invoice[0].LegalMonetaryTotal[0].PayableRoundingAmount != null)
                this.Invoice[0].LegalMonetaryTotal[0].PayableRoundingAmount = null;
        }
        #endregion

        #region Total Taxable Amount Per Tax Type, Tax Type (Invoice:TaxTotal:TaxSubtotal)
        public void AddTaxSubtotal(decimal taxAmount, decimal taxableAmount, TaxType taxType)
        {
            Invoice invoice = this.Invoice[0];
            string type = EnumUtils.ConvertTaxTypeToString(taxType);
            string currencyId = this.Invoice[0].DocumentCurrencyCode[0]._;

            if (invoice.TaxTotal == null)
                invoice.TaxTotal = new List<TaxTotal>() { new TaxTotal() };

            if (invoice.TaxTotal[0].TaxSubtotal == null)
                invoice.TaxTotal[0].TaxSubtotal = new List<TaxSubtotal>();

            TaxSubtotal subtotal = invoice.TaxTotal[0].TaxSubtotal.Find(match => match.TaxCategory != null && match.TaxCategory[0].ID[0]._ == type);

            if (subtotal == null)
            {
                subtotal = new TaxSubtotal()
                {
                    TaxAmount = new List<TaxAmount>() { new TaxAmount() { _ = taxAmount, CurrencyID = currencyId } },
                    TaxCategory = new List<TaxCategory>()
                    {
                        new TaxCategory()
                        {
                            ID = new List<TaxCategoryID>() { new TaxCategoryID() { _ = type } },
                            TaxScheme = new List<TaxScheme>() { new TaxScheme() { ID = new List<TaxSchemeID>() { new TaxSchemeID() } } }
                        }
                    },
                    TaxableAmount = new List<TaxableAmount>() { new TaxableAmount() { _ = taxableAmount, CurrencyID = currencyId } }
                };

                invoice.TaxTotal[0].TaxSubtotal.Add(subtotal);
            }
            else
            {
                subtotal.TaxAmount[0]._ += taxAmount;

                if (subtotal.TaxableAmount == null)
                    subtotal.TaxableAmount = new List<TaxableAmount>() { new TaxableAmount() { _ = taxableAmount, CurrencyID = currencyId } };
                else
                    subtotal.TaxableAmount[0]._ += taxableAmount;
            }
        }

        public void RemoveTaxableAmount(TaxType taxType)
        {
            string type = EnumUtils.ConvertTaxTypeToString(taxType);

            if (this.Invoice[0].TaxTotal != null && this.Invoice[0].TaxTotal[0].TaxSubtotal != null)
            {
                TaxSubtotal subtotal = this.Invoice[0].TaxTotal[0].TaxSubtotal.Find(match => match.TaxCategory != null && match.TaxCategory[0].ID[0]._ == type);

                if (subtotal != null && subtotal.TaxableAmount != null)
                    subtotal.TaxableAmount = null;
            }
        }
        #endregion

        #region Invoice Additional Discount / Fee Amount (Invoice:AllowanceCharge)
        public void AddAllowanceCharge(bool additionalCharge, decimal amount, string reason)
        {
            Invoice invoice = this.Invoice[0];

            if (invoice.AllowanceCharge == null)
                invoice.AllowanceCharge = new List<AllowanceCharge>() { new AllowanceCharge() { } };

            invoice.AllowanceCharge.Add(new AllowanceCharge()
            {
                ChargeIndicator = new List<ChargeIndicator>()
                {
                    new ChargeIndicator() { _ = additionalCharge }
                },
                Amount = new List<Amount>()
                {
                    new Amount() { _ = amount, CurrencyID = invoice.DocumentCurrencyCode[0]._ }
                },
                AllowanceChargeReason = new List<AllowanceChargeReason>()
                {
                    new AllowanceChargeReason() { _ = reason }
                }
            });

            if (invoice.LegalMonetaryTotal == null)
                invoice.LegalMonetaryTotal = new List<LegalMonetaryTotal>() { new LegalMonetaryTotal() };

            if (additionalCharge)
            {
                if (invoice.LegalMonetaryTotal[0].ChargeTotalAmount == null)
                {
                    invoice.LegalMonetaryTotal[0].ChargeTotalAmount = new List<ChargeTotalAmount>() { new ChargeTotalAmount() };
                    invoice.LegalMonetaryTotal[0].ChargeTotalAmount[0].CurrencyID = invoice.DocumentCurrencyCode[0]._;
                }

                invoice.LegalMonetaryTotal[0].ChargeTotalAmount[0]._ += amount;
            }
            else
            {
                if (invoice.LegalMonetaryTotal[0].AllowanceTotalAmount == null)
                {
                    invoice.LegalMonetaryTotal[0].AllowanceTotalAmount = new List<AllowanceTotalAmount>() { new AllowanceTotalAmount() };
                    invoice.LegalMonetaryTotal[0].AllowanceTotalAmount[0].CurrencyID = invoice.DocumentCurrencyCode[0]._;
                }

                invoice.LegalMonetaryTotal[0].AllowanceTotalAmount[0]._ += amount;
            }
        }
        #endregion

        #region Invoice Line Item (InvoiceLineItem)
        public void AddInvoiceLineItem(InvoiceLineArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.ID))
                throw new ArgumentException("Invoice Line ID is invalid.", nameof(args.ID));

            if (string.IsNullOrWhiteSpace(args.Description))
                throw new ArgumentException("Invoice Line Description is invalid.", nameof(args.Description));

            if (args.TaxSubtotal.Count == 0)
                throw new ArgumentException("Invoice Line must have at least one TaxSubtotal", nameof(args.TaxSubtotal));

            string currencyId = this.Invoice[0].DocumentCurrencyCode[0]._;

            InvoiceLineID id = new InvoiceLineID() { _ = args.ID };

            Item item = new Item()
            {
                Description = new List<Description>() { new Description() { _ = args.Description } },
                CommodityClassification = new List<CommodityClassification>()
                {
                    new CommodityClassification()
                    {
                        ItemClassificationCode = new List<ItemClassificationCode>()
                        {
                            new ItemClassificationCode()
                            {
                                _ = ((int)args.ClassificationCode).ToString().PadLeft(3, '0'),
                                ListID = "CLASS"
                            }
                        }
                    }
                },
            };

            Price price = new Price()
            {
                PriceAmount = new List<PriceAmount>()
                {
                    new PriceAmount() { _ = args.UnitPrice, CurrencyID = currencyId }
                }
            };

            decimal taxAmount = 0.0m;
            List<TaxSubtotal> taxSubtotal = new List<TaxSubtotal>();
            foreach (InvoiceLineTaxSubtotalArgs input in args.TaxSubtotal)
            {
                TaxSubtotal newTaxSubtotal = new TaxSubtotal()
                {
                    TaxableAmount = new List<TaxableAmount>() { new TaxableAmount() { _ = input.TaxableAmount, CurrencyID = currencyId } },
                    TaxCategory = new List<TaxCategory>()
                    {
                        new TaxCategory()
                        {
                            ID = new List<TaxCategoryID>() { new TaxCategoryID() { _ = EnumUtils.ConvertTaxTypeToString(input.TaxType) } },
                            TaxScheme = new List<TaxScheme>() { new TaxScheme() { ID = new List<TaxSchemeID>() { new TaxSchemeID() } } }
                        }
                    }
                };

                if (input.TaxRateType != null && input.TaxRate != null)
                {
                    if (input.TaxRateType == TaxRateType.PERCENTAGE)
                    {
                        newTaxSubtotal.Percent = new List<Percent>() { new Percent() { _ = input.TaxRate.Value } };
                    }
                    else if (input.TaxRateType == TaxRateType.FIXED_RATE &&
                             input.NumberOfUnits != null &&
                             !string.IsNullOrWhiteSpace(input.UnitCode))
                    {
                        newTaxSubtotal.PerUnitAmount = new List<PerUnitAmount>() { new PerUnitAmount() { _ = input.TaxRate.Value, CurrencyID = currencyId } };
                        newTaxSubtotal.BaseUnitMeasure = new List<BaseUnitMeasure>() { new BaseUnitMeasure() { _ = input.NumberOfUnits.Value, UnitCode = input.UnitCode } };
                    }
                }

                if (input.TaxType == TaxType.TAX_EXEMPTION)
                {
                    if (!string.IsNullOrWhiteSpace(input.TaxExemptionReason))
                        newTaxSubtotal.TaxCategory[0].TaxExemptionReason = new List<TaxExemptionReason>() { new TaxExemptionReason() { _ = input.TaxExemptionReason } };
                    else
                        throw new ArgumentException("Invoice Line TaxExemptionReason is invalid.", nameof(input.TaxExemptionReason));
                }

                newTaxSubtotal.TaxAmount = new List<TaxAmount>() { new TaxAmount() { _ = input.TaxAmount, CurrencyID = currencyId } };
                taxAmount += input.TaxAmount;
                taxSubtotal.Add(newTaxSubtotal);

                this.AddTaxSubtotal(
                    taxAmount: newTaxSubtotal.TaxAmount[0]._,
                    taxableAmount: newTaxSubtotal.TaxableAmount[0]._,
                    taxType: newTaxSubtotal.TaxCategory[0].ID[0]._ == "E" ? TaxType.TAX_EXEMPTION : (TaxType)int.Parse(newTaxSubtotal.TaxCategory[0].ID[0]._)
                );
            }

            ItemPriceExtension subtotal = new ItemPriceExtension()
            {
                Amount = new List<Amount>()
                {
                    new Amount()
                    {
                        _ = args.Quantity == null ? args.UnitPrice : args.UnitPrice * args.Quantity.Value,
                        CurrencyID = currencyId
                    }
                }
            };

            LineExtensionAmount totalExcludingTax = new LineExtensionAmount() { _ = subtotal.Amount[0]._, CurrencyID = currencyId };
            this.AddTotalNetAmount(subtotal.Amount[0]._);

            InvoiceLine invoiceLine = new InvoiceLine()
            {
                InvoiceLineID = new List<InvoiceLineID>() { id },
                LineExtensionAmount = new List<LineExtensionAmount>() { totalExcludingTax },
                TaxTotal = new List<TaxTotal>()
                {
                    new TaxTotal()
                    {
                        TaxAmount = new List<TaxAmount>() { new TaxAmount() { _ = taxAmount, CurrencyID = currencyId } },
                        TaxSubtotal = taxSubtotal
                    }
                },
                Item = new List<Item>() { item },
                Price = new List<Price>() { price },
                ItemPriceExtension = new List<ItemPriceExtension>() { subtotal }
            };

            if (args.Quantity != null)
                invoiceLine.InvoicedQuantity = new List<InvoicedQuantity>() { new InvoicedQuantity() { _ = args.Quantity.Value } };

            if (args.Measurement != null)
            {
                if (invoiceLine.InvoicedQuantity != null)
                    invoiceLine.InvoicedQuantity[0].UnitCode = args.Measurement;
                else
                    invoiceLine.InvoicedQuantity = new List<InvoicedQuantity>() { new InvoicedQuantity() { UnitCode = args.Measurement } };
            }

            if (args.Discount != null)
            {
                foreach (InvoiceLineDiscountArgs discountArgs in args.Discount)
                {
                    bool hasDiscount = false;
                    AllowanceCharge discount = new AllowanceCharge()
                    {
                        ChargeIndicator = new List<ChargeIndicator>() { new ChargeIndicator() { _ = false } }
                    };

                    if (discountArgs.DiscountRate != null)
                    {
                        discount.MultiplierFactorNumeric = new List<MultiplierFactorNumeric>() { new MultiplierFactorNumeric() { _ = discountArgs.DiscountRate.Value } };
                        hasDiscount = true;
                    }

                    if (discountArgs.DiscountAmount != null && !string.IsNullOrWhiteSpace(discountArgs.DiscountReason))
                    {
                        discount.Amount = new List<Amount>() { new Amount() { _ = discountArgs.DiscountAmount.Value, CurrencyID = currencyId } };
                        discount.AllowanceChargeReason = new List<AllowanceChargeReason>() { new AllowanceChargeReason() { _ = discountArgs.DiscountReason } };
                        hasDiscount = true;
                    }

                    if (hasDiscount)
                    {
                        if (invoiceLine.AllowanceCharge == null)
                            invoiceLine.AllowanceCharge = new List<AllowanceCharge>();

                        invoiceLine.AllowanceCharge.Add(discount);
                        invoiceLine.ItemPriceExtension[0].Amount[0]._ -= discountArgs.DiscountAmount.GetValueOrDefault();

                        this.AddAllowanceCharge(
                            additionalCharge: false,
                            amount: discount.Amount[0]._,
                            reason: discount.AllowanceChargeReason[0]._
                        );
                    }
                }
            }

            if (args.Charge != null)
            {
                foreach (InvoiceLineChargeArgs chargeArgs in args.Charge)
                {
                    bool hasCharge = false;
                    AllowanceCharge charge = new AllowanceCharge()
                    {
                        ChargeIndicator = new List<ChargeIndicator>() { new ChargeIndicator() { _ = true } }
                    };

                    if (chargeArgs.ChargeRate != null)
                    {
                        charge.MultiplierFactorNumeric = new List<MultiplierFactorNumeric>() { new MultiplierFactorNumeric() { _ = chargeArgs.ChargeRate.Value } };
                        hasCharge = true;
                    }

                    if (chargeArgs.ChargeAmount != null && !string.IsNullOrWhiteSpace(chargeArgs.ChargeReason))
                    {
                        hasCharge = true;
                        charge.Amount = new List<Amount>() { new Amount() { _ = chargeArgs.ChargeAmount.Value, CurrencyID = currencyId } };
                        charge.AllowanceChargeReason = new List<AllowanceChargeReason>() { new AllowanceChargeReason() { _ = chargeArgs.ChargeReason } };
                    }

                    if (hasCharge)
                    {
                        if (invoiceLine.AllowanceCharge == null)
                            invoiceLine.AllowanceCharge = new List<AllowanceCharge>();

                        invoiceLine.AllowanceCharge.Add(charge);
                        invoiceLine.ItemPriceExtension[0].Amount[0]._ += chargeArgs.ChargeAmount.GetValueOrDefault();

                        this.AddAllowanceCharge(
                            additionalCharge: true,
                            amount: charge.Amount[0]._,
                            reason: charge.AllowanceChargeReason[0]._
                        );
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(args.TariffCode))
            {
                invoiceLine.Item[0].CommodityClassification.Add(new CommodityClassification()
                {
                    ItemClassificationCode = new List<ItemClassificationCode>()
                    {
                        new ItemClassificationCode()
                        {
                            _ = args.TariffCode,
                            ListID = "PTC"
                        }
                    }
                });
            }

            if (args.CountryOfOrigin != null)
            {
                invoiceLine.Item[0].OriginCountry = new List<OriginCountry>()
                {
                    new OriginCountry()
                    {
                        IdentificationCode = new List<IdentificationCode>()
                        {
                            new IdentificationCode() { _ = EnumUtils.ConvertCountryNameToCode(args.CountryOfOrigin.Value) }
                        }
                    }
                };
            }

            if (this.Invoice[0].InvoiceLine == null)
                this.Invoice[0].InvoiceLine = new List<InvoiceLine>();

            this.Invoice[0].InvoiceLine.Add(invoiceLine);
        }
        #endregion

        public string Export(DocumentFormat format, bool indented = false)
        {
            this.CalculateTaxExclusiveAmount();
            this.CalculateTaxInclusiveAmount();
            this.CalculatePayableAmount();
            this.CalculateTaxAmount();

            switch (format)
            {
                default:
                    JsonSerializerOptions options = new JsonSerializerOptions()
                    {
                        WriteIndented = indented,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    };
                    options.Converters.Add(new TaxSubtotalListConverter());
                    options.Converters.Add(new AllowanceChargeListConverter());

                    return JsonSerializer.Serialize(this, options);
            }
        }
    }

    #region E-Invoice Document Model
    public class Invoice
    {
        public List<InvoiceID> ID { get; set; }
        public List<IssueDate> IssueDate { get; set; }
        public List<IssueTime> IssueTime { get; set; }
        public List<InvoiceTypeCode> InvoiceTypeCode { get; set; }
        public List<DocumentCurrencyCode> DocumentCurrencyCode { get; set; }
        public List<TaxCurrencyCode> TaxCurrencyCode { get; set; }
        public List<InvoicePeriod> InvoicePeriod { get; set; }
        public List<BillingReference> BillingReference { get; set; }
        public List<AccountingSupplierParty> AccountingSupplierParty { get; set; }
        public List<AccountingCustomerParty> AccountingCustomerParty { get; set; }
        public List<Delivery> Delivery { get; set; }
        public List<PaymentMeans> PaymentMeans { get; set; }
        public List<PaymentTerms> PaymentTerms { get; set; }
        public List<PrepaidPayment> PrepaidPayment { get; set; }

        // NOTE: LHDN format requires multiple keys with the same name, rather than a single key with multiple values in an array
        public List<AllowanceCharge> AllowanceCharge { get; set; }

        public List<TaxTotal> TaxTotal { get; set; }
        public List<LegalMonetaryTotal> LegalMonetaryTotal { get; set; }
        public List<InvoiceLine> InvoiceLine { get; set; }
    }

    public class InvoiceID
    {
        public string _ { get; set; }
    }

    public class IssueDate
    {
        public string _ { get; set; }
    }

    public class IssueTime
    {
        public string _ { get; set; }
    }

    public class InvoiceTypeCode
    {
        public string _ { get; set; }

        [JsonPropertyName("listVersionID")]
        public string ListVersionID { get; set; }
    }

    public class DocumentCurrencyCode
    {
        public string _ { get; set; }
    }

    public class TaxCurrencyCode
    {
        public string _ { get; set; }
    }

    public class InvoicePeriod
    {
        public List<StartDate> StartDate { get; set; }
        public List<EndDate> EndDate { get; set; }
        public List<Description> Description { get; set; }
        public int children = 0;
    }

    public class StartDate
    {
        public string _ { get; set; }
    }

    public class EndDate
    {
        public string _ { get; set; }
    }

    public class Description
    {
        public string _ { get; set; }
    }

    public class BillingReference
    {
        public List<AdditionalDocumentReference> AdditionalDocumentReference { get; set; }
    }

    public class AdditionalDocumentReference
    {
        public List<AdditionalDocumentReferenceID> ID { get; set; }
        public List<DocumentType> DocumentType { get; set; }
    }

    public class AdditionalDocumentReferenceID
    {
        public string _ { get; set; }
    }

    public class DocumentType
    {
        public string _ { get; set; }
    }

    public class AccountingSupplierParty
    {
        public List<AdditionalAccountID> AdditionalAccountID { get; set; }
        public List<Party> Party { get; set; }
    }

    public class AdditionalAccountID
    {
        public string _ { get; set; }

        [JsonPropertyName("schemeAgencyName")]
        public string SchemeAgencyName { get; set; }
    }

    public class AccountingCustomerParty
    {
        public List<Party> Party { get; set; }
    }

    public class Party
    {
        public List<IndustryClassificationCode> IndustryClassificationCode { get; set; }
        public List<PartyIdentification> PartyIdentification { get; set; }
        public List<PostalAddress> PostalAddress { get; set; }
        public List<PartyLegalEntity> PartyLegalEntity { get; set; }
        public List<Contact> Contact { get; set; }
    }

    public class IndustryClassificationCode
    {
        public string _ { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Delivery
    {
        public List<DeliveryParty> DeliveryParty { get; set; }
        public List<Shipment> Shipment { get; set; }
    }

    public class DeliveryParty
    {
        public List<PartyLegalEntity> PartyLegalEntity { get; set; }
        public List<PostalAddress> PostalAddress { get; set; }
        public List<PartyIdentification> PartyIdentification { get; set; }
    }

    public class PartyIdentification
    {
        public List<PartyIdentificationID> ID { get; set; }
    }

    public class PartyIdentificationID
    {
        public string _ { get; set; }

        [JsonPropertyName("schemeID")]
        public string SchemeID { get; set; }
    }

    public class PostalAddress
    {
        public List<CityName> CityName { get; set; }
        public List<PostalZone> PostalZone { get; set; }
        public List<CountrySubentityCode> CountrySubentityCode { get; set; }
        public List<AddressLine> AddressLine { get; set; }
        public List<Country> Country { get; set; }
    }

    public class CityName
    {
        public string _ { get; set; }
    }

    public class PostalZone
    {
        public string _ { get; set; }
    }

    public class CountrySubentityCode
    {
        public string _ { get; set; }
    }

    public class AddressLine
    {
        public List<Line> Line { get; set; }
    }

    public class Line
    {
        public string _ { get; set; }
    }

    public class Country
    {
        public List<IdentificationCode> IdentificationCode { get; set; }
    }

    public class IdentificationCode
    {
        public string _ { get; set; }

        [JsonPropertyName("listID")]
        public string ListID { get; set; }

        [JsonPropertyName("listAgencyID")]
        public string ListAgencyID { get; set; }
    }

    public class PartyLegalEntity
    {
        public List<RegistrationName> RegistrationName { get; set; }
    }

    public class RegistrationName
    {
        public string _ { get; set; }
    }

    public class Contact
    {
        public List<Telephone> Telephone { get; set; }
        public List<ElectronicMail> ElectronicMail { get; set; }
    }

    public class Telephone
    {
        public string _ { get; set; }
    }

    public class ElectronicMail
    {
        public string _ { get; set; }
    }

    public class Shipment
    {
        public List<ShipmentID> ID { get; set; }
        public List<FreightAllowanceCharge> FreightAllowanceCharge { get; set; }
    }

    public class ShipmentID
    {
        public string _ { get; set; }
    }

    public class FreightAllowanceCharge
    {
        public List<ChargeIndicator> ChargeIndicator { get; set; }
        public List<AllowanceChargeReason> AllowanceChargeReason { get; set; }
        public List<Amount> Amount { get; set; }
    }

    public class ChargeIndicator
    {
        public bool _ { get; set; }
    }

    public class AllowanceChargeReason
    {
        public string _ { get; set; }
    }

    public class Amount
    {
        public decimal _ { get; set; }

        [JsonPropertyName("currencyID")]
        public string CurrencyID { get; set; }
    }

    public class PaymentMeans
    {
        public List<PaymentMeansCode> PaymentMeansCode { get; set; }
        public List<PayeeFinancialAccount> PayeeFinancialAccount { get; set; }
    }

    public class PaymentMeansCode
    {
        public string _ { get; set; }
    }

    public class PayeeFinancialAccount
    {
        public List<PayeeFinancialAccountID> ID { get; set; }
    }

    public class PayeeFinancialAccountID
    {
        public string _ { get; set; }
    }

    public class PaymentTerms
    {
        public List<Note> Note { get; set; }
    }

    public class Note
    {
        public string _ { get; set; }
    }

    public class PrepaidPayment
    {
        public List<PrepaidPaymentID> ID { get; set; }
        public List<PaidAmount> PaidAmount { get; set; }
        public List<PaidDate> PaidDate { get; set; }
        public List<PaidTime> PaidTime { get; set; }
    }

    public class PrepaidPaymentID
    {
        public string _ { get; set; }
    }

    public class PaidAmount
    {
        public decimal _ { get; set; }

        [JsonPropertyName("currencyID")]
        public string CurrencyID { get; set; }
    }

    public class PaidDate
    {
        public string _ { get; set; }
    }

    public class PaidTime
    {
        public string _ { get; set; }
    }

    public class AllowanceCharge
    {
        public List<ChargeIndicator> ChargeIndicator { get; set; }
        public List<AllowanceChargeReason> AllowanceChargeReason { get; set; }
        public List<MultiplierFactorNumeric> MultiplierFactorNumeric { get; set; }
        public List<Amount> Amount { get; set; }
    }

    public class MultiplierFactorNumeric
    {
        public decimal _ { get; set; }
    }

    public class TaxTotal
    {
        public List<TaxAmount> TaxAmount { get; set; }

        // NOTE: LHDN format requires multiple keys with the same name, rather than a single key with multiple values in an array
        public List<TaxSubtotal> TaxSubtotal { get; set; }
    }

    public class TaxAmount
    {
        public decimal _ { get; set; } = 0.0m;

        [JsonPropertyName("currencyID")]
        public string CurrencyID { get; set; }
    }

    public class TaxSubtotal
    {
        public List<TaxableAmount> TaxableAmount { get; set; }
        public List<TaxAmount> TaxAmount { get; set; }
        public List<TaxCategory> TaxCategory { get; set; }
        public List<Percent> Percent { get; set; }
        public List<PerUnitAmount> PerUnitAmount { get; set; }
        public List<BaseUnitMeasure> BaseUnitMeasure { get; set; }
    }

    public class TaxableAmount
    {
        public decimal _ { get; set; }

        [JsonPropertyName("currencyID")]
        public string CurrencyID { get; set; }
    }

    public class TaxCategory
    {
        public List<TaxCategoryID> ID { get; set; }
        public List<TaxExemptionReason> TaxExemptionReason { get; set; }
        public List<TaxScheme> TaxScheme { get; set; }
    }

    public class TaxCategoryID
    {
        public string _ { get; set; }
    }

    public class TaxExemptionReason
    {
        public string _ { get; set; }
    }

    public class TaxScheme
    {
        public List<TaxSchemeID> ID { get; set; }
    }

    public class TaxSchemeID
    {
        public string _ { get; set; } = "OTH";

        [JsonPropertyName("schemeID")]
        public string SchemeID { get; set; } = "UN/ECE 5153";

        [JsonPropertyName("schemeAgencyID")]
        public string SchemeAgencyID { get; set; } = "6";
    }

    public class Percent
    {
        public decimal _ { get; set; }
    }

    public class PerUnitAmount
    {
        public decimal _ { get; set; }

        [JsonPropertyName("currencyID")]
        public string CurrencyID { get; set; }
    }

    public class BaseUnitMeasure
    {
        public decimal _ { get; set; }

        [JsonPropertyName("unitCode")]
        public string UnitCode { get; set; }
    }

    public class LegalMonetaryTotal
    {
        public List<LineExtensionAmount> LineExtensionAmount { get; set; }
        public List<TaxExclusiveAmount> TaxExclusiveAmount { get; set; }
        public List<TaxInclusiveAmount> TaxInclusiveAmount { get; set; }
        public List<AllowanceTotalAmount> AllowanceTotalAmount { get; set; }
        public List<ChargeTotalAmount> ChargeTotalAmount { get; set; }
        public List<PayableRoundingAmount> PayableRoundingAmount { get; set; }
        public List<PayableAmount> PayableAmount { get; set; }
    }

    public class LineExtensionAmount
    {
        public decimal _ { get; set; } = 0.0m;

        [JsonPropertyName("currencyID")]
        public string CurrencyID { get; set; }
    }

    public class TaxExclusiveAmount
    {
        public decimal _ { get; set; } = 0.0m;

        [JsonPropertyName("currencyID")]
        public string CurrencyID { get; set; }
    }

    public class TaxInclusiveAmount
    {
        public decimal _ { get; set; } = 0.0m;

        [JsonPropertyName("currencyID")]
        public string CurrencyID { get; set; }
    }

    public class AllowanceTotalAmount
    {
        public decimal _ { get; set; } = 0.0m;

        [JsonPropertyName("currencyID")]
        public string CurrencyID { get; set; }
    }

    public class ChargeTotalAmount
    {
        public decimal _ { get; set; } = 0.0m;

        [JsonPropertyName("currencyID")]
        public string CurrencyID { get; set; }
    }

    public class PayableRoundingAmount
    {
        public decimal _ { get; set; } = 0.0m;

        [JsonPropertyName("currencyID")]
        public string CurrencyID { get; set; }
    }

    public class PayableAmount
    {
        public decimal _ { get; set; } = 0.0m;

        [JsonPropertyName("currencyID")]
        public string CurrencyID { get; set; }
    }

    public class InvoiceLine
    {
        [JsonPropertyName("ID")]
        public List<InvoiceLineID> InvoiceLineID { get; set; }
        public List<InvoicedQuantity> InvoicedQuantity { get; set; }
        public List<LineExtensionAmount> LineExtensionAmount { get; set; }

        // NOTE: LHDN format requires multiple keys with the same name, rather than a single key with multiple values in an array
        public List<AllowanceCharge> AllowanceCharge { get; set; }

        public List<TaxTotal> TaxTotal { get; set; }
        public List<Item> Item { get; set; }
        public List<Price> Price { get; set; }
        public List<ItemPriceExtension> ItemPriceExtension { get; set; }
    }

    public class InvoiceLineID
    {
        public string _ { get; set; }
    }

    public class InvoicedQuantity
    {
        public decimal _ { get; set; }

        [JsonPropertyName("unitCode")]
        public string UnitCode { get; set; }
    }

    public class Item
    {
        public List<CommodityClassification> CommodityClassification { get; set; }
        public List<Description> Description { get; set; }
        public List<OriginCountry> OriginCountry { get; set; }
    }

    public class CommodityClassification
    {
        public List<ItemClassificationCode> ItemClassificationCode { get; set; }
    }

    public class ItemClassificationCode
    {
        public string _ { get; set; }

        [JsonPropertyName("listID")]
        public string ListID { get; set; }
    }

    public class OriginCountry
    {
        public List<IdentificationCode> IdentificationCode { get; set; }
    }

    public class Price
    {
        public List<PriceAmount> PriceAmount { get; set; }
    }

    public class PriceAmount
    {
        public decimal _ { get; set; }

        [JsonPropertyName("currencyID")]
        public string CurrencyID { get; set; }
    }

    public class ItemPriceExtension
    {
        public List<Amount> Amount { get; set; }
    }
    #endregion

    #region Method parameters
    public class AccountingPartyArgs
    {
        public string Name { get; set; }
        public string TIN { get; set; }
        public TaxpayerType IDType { get; set; }
        public string ID { get; set; }
        public string SST { get; set; } = "NA";
        public string TTX { get; set; } = "NA";
        public string AddressLine0 { get; set; } = "NA";
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostalZone { get; set; }
        public string CityName { get; set; }
        public State State { get; set; }
        public CountryCode Country { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }

        protected AccountingPartyArgs() { }

        protected AccountingPartyArgs(
            string name,
            string tin,
            TaxpayerType idType,
            string id,
            string cityName,
            State state,
            CountryCode country,
            string contactNumber
        )
        {
            this.Name = name;
            this.TIN = tin;
            this.IDType = idType;
            this.ID = id;
            this.CityName = cityName;
            this.State = state;
            this.Country = country;
            this.ContactNumber = contactNumber;
        }
    }

    public class AccountingSupplierPartyArgs : AccountingPartyArgs
    {
        public MSIC MSIC { get; set; }
        public string BusinessDescription { get; set; }

        private AccountingSupplierPartyArgs() { }

        public AccountingSupplierPartyArgs(
            string name,
            string tin,
            TaxpayerType idType,
            string id,
            MSIC msic,
            string cityName,
            State state,
            CountryCode country,
            string contactNumber
        ) : base(name, tin, idType, id, cityName, state, country, contactNumber)
        {
            this.MSIC = msic;
        }
    }

    public class AccountingCustomerPartyArgs : AccountingPartyArgs
    {
        private AccountingCustomerPartyArgs() { }

        public AccountingCustomerPartyArgs(
            string name,
            string tin,
            TaxpayerType idType,
            string id,
            string cityName,
            State state,
            CountryCode country,
            string contactNumber
        ) : base(name, tin, idType, id, cityName, state, country, contactNumber) { }
    }

    public class InvoiceLineArgs
    {
        public string ID { get; set; }
        public ClassificationCode ClassificationCode { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public List<InvoiceLineTaxSubtotalArgs> TaxSubtotal { get; set; }
        public decimal? Quantity { get; set; }
        public string Measurement { get; set; }
        public List<InvoiceLineDiscountArgs> Discount { get; set; }
        public List<InvoiceLineChargeArgs> Charge { get; set; }
        public string TariffCode { get; set; }
        public CountryCode? CountryOfOrigin { get; set; }

        private InvoiceLineArgs() { }

        public InvoiceLineArgs(
            string id,
            ClassificationCode classificationCode,
            string description,
            decimal unitPrice,
            List<InvoiceLineTaxSubtotalArgs> taxSubtotal
        )
        {
            this.ID = id;
            this.ClassificationCode = classificationCode;
            this.Description = description;
            this.UnitPrice = unitPrice;
            this.TaxSubtotal = taxSubtotal;
        }
    }

    public class InvoiceLineTaxSubtotalArgs
    {
        public decimal TaxableAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public TaxType TaxType { get; set; }
        public TaxRateType? TaxRateType { get; set; }
        public decimal? TaxRate { get; set; }
        public string TaxExemptionReason { get; set; }
        public decimal? NumberOfUnits { get; set; }
        public string UnitCode { get; set; }

        private InvoiceLineTaxSubtotalArgs() { }

        public InvoiceLineTaxSubtotalArgs(decimal taxableAmount, decimal taxAmount, TaxType taxType)
        {
            this.TaxableAmount = taxableAmount;
            this.TaxAmount = taxAmount;
            this.TaxType = taxType;
        }
    }

    public class InvoiceLineDiscountArgs
    {
        public decimal? DiscountRate { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string DiscountReason { get; set; }
    }

    public class InvoiceLineChargeArgs
    {
        public decimal? ChargeRate { get; set; }
        public decimal? ChargeAmount { get; set; }
        public string ChargeReason { get; set; }
    }
    #endregion
}