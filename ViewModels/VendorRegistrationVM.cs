using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace VendorOnboardingSystem.ViewModels
{
    public class VendorRegistrationVM
    {
        // === SECTION 1 — Basic Legal Information ===
        [Required(ErrorMessage = "Legal Name is required")]
        [Display(Name = "Vendor Legal Name (as per PAN)")]
        public string LegalName { get; set; }

        [Display(Name = "Trade Name")]
        public string? TradeName { get; set; }

        [Required(ErrorMessage = "Vendor Type is required")]
        [Display(Name = "Vendor Type")]
        public string VendorType { get; set; }

        [Required(ErrorMessage = "Vendor Category is required")]
        [Display(Name = "Vendor Category")]
        public string VendorCategory { get; set; }

        [Required(ErrorMessage = "Constitution of Business is required")]
        [Display(Name = "Constitution of Business")]
        public string Constitution { get; set; }

        [Display(Name = "Date of Incorporation")]
        [DataType(DataType.Date)]
        public DateTime? DateOfIncorporation { get; set; }

        [Display(Name = "CIN (if Company/LLP)")]
        public string? CIN { get; set; }

        // === SECTION 2 — PAN Details ===
        [Required(ErrorMessage = "PAN Number is required")]
        [Display(Name = "PAN Number")]
        [RegularExpression(@"^[A-Za-z]{5}[0-9]{4}[A-Za-z]{1}$", ErrorMessage = "Invalid PAN format")]
        public string PAN { get; set; }

        [Required(ErrorMessage = "PAN Copy is required")]
        [Display(Name = "PAN Copy Upload")]
        public IFormFile PANCopy { get; set; }

        // === SECTION 3 — GST Details ===
        [Required(ErrorMessage = "Please specify if GST registered")]
        [Display(Name = "GST Registered?")]
        public string GSTRegistered { get; set; }

        [Display(Name = "GSTIN")]
        [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}[Z]{1}[0-9A-Z]{1}$", ErrorMessage = "Invalid GSTIN format")]
        public string? GSTIN { get; set; }

        [Display(Name = "GST Registration Type")]
        public string? GSTRegistrationType { get; set; }

        [Display(Name = "State Code")]
        public string? StateCode { get; set; }

        [Display(Name = "Place of Supply")]
        public string? PlaceOfSupply { get; set; }

        [Display(Name = "GST Certificate Upload")]
        public IFormFile? GSTCertificate { get; set; }

        // === SECTION 4 — MSME Details ===
        [Required(ErrorMessage = "Please specify if MSME registered")]
        [Display(Name = "MSME Registered?")]
        public string MSMERegistered { get; set; }

        [Display(Name = "Udyam Registration Number")]
        [RegularExpression(@"^UDYAM-[A-Z]{2}-\d{2}-\d{7}$", ErrorMessage = "Invalid Udyam format")]
        public string? UdyamNumber { get; set; }

        // === SECTION 5 — TDS Details ===
        [Required(ErrorMessage = "Please specify if TDS applicable")]
        [Display(Name = "TDS Applicable?")]
        public string TDSApplicable { get; set; }

        [Display(Name = "TDS Sections Applicable")]
        public List<string> SelectedTDSSections { get; set; } = new List<string>();

        // === SECTION 6 — Bank Details ===
        [Required(ErrorMessage = "Account Holder Name is required")]
        [Display(Name = "Account Holder Name")]
        public string AccountHolderName { get; set; }

        [Required(ErrorMessage = "Bank Name is required")]
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "Account Number is required")]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "IFSC Code is required")]
        [Display(Name = "IFSC Code")]
        [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC format")]
        public string IFSC { get; set; }

        [Required(ErrorMessage = "Cancelled Cheque is required")]
        [Display(Name = "Cancelled Cheque Upload")]
        public IFormFile CancelledCheque { get; set; }

        // === SECTION 7 — Address Details ===
        [Required(ErrorMessage = "Address Line 1 is required")]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string? AddressLine2 { get; set; }

        [Required(ErrorMessage = "City is required")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required(ErrorMessage = "PIN Code is required")]
        [Display(Name = "PIN Code")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "PIN Code must be 6 digits")]
        public string PINCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [Display(Name = "Country")]
        public string Country { get; set; }

        // === SECTION 8 — Contact Information ===
        [Required(ErrorMessage = "Primary Contact Name is required")]
        [Display(Name = "Primary Contact Name")]
        public string PrimaryContactName { get; set; }

        [Display(Name = "Designation")]
        public string? Designation { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [Display(Name = "Mobile Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile Number must be 10 digits")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Email ID is required")]
        [Display(Name = "Email ID")]
        [EmailAddress(ErrorMessage = "Invalid Email format")]
        public string Email { get; set; }

        [Display(Name = "Alternate Contact Person")]
        public string? AlternateContactPerson { get; set; }

        [Display(Name = "Aadhar Number")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Aadhar Number must be 12 digits")]
        public string? AadharNumber { get; set; }

        [Display(Name = "Aadhar Copy Upload")]
        public IFormFile? AadharCopy { get; set; }
    }
}