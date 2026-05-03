using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace VendorOnboardingSystem.ViewModels
{
    public class VendorEditVM
    {
        public int EditRequestId { get; set; }
        public string AdminNotes { get; set; }

        // Group 1: Legal Information
        public bool ShowLegalInfo { get; set; }
        public string LegalInfoComment { get; set; }
        public string LegalName { get; set; }
        public string TradeName { get; set; }
        public string VendorType { get; set; }
        public string VendorCategory { get; set; }
        public string Constitution { get; set; }

        // Group 2: PAN Details
        public bool ShowPANDetails { get; set; }
        public string PANDetailsComment { get; set; }
        public string PAN { get; set; }
        public string ExistingPANCopyPath { get; set; }
        public IFormFile NewPANCopy { get; set; }  // For re-upload

        // Group 3: GST Details
        public bool ShowGSTDetails { get; set; }
        public string GSTDetailsComment { get; set; }
        public string GSTIN { get; set; }
        public string ExistingGSTCertificatePath { get; set; }
        public IFormFile NewGSTCertificate { get; set; }  // For re-upload

        // Group 4: MSME Details
        public bool ShowMSMEDetails { get; set; }
        public string MSMEDetailsComment { get; set; }
        public string MSMERegistered { get; set; }
        public string UdyamNumber { get; set; }

        // Group 5: TDS Details
        public bool ShowTDSDetails { get; set; }
        public string TDSDetailsComment { get; set; }
        public string TDSApplicable { get; set; }
        public string TDSSections { get; set; }

        // Group 6: Bank Details
        public bool ShowBankDetails { get; set; }
        public string BankDetailsComment { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSC { get; set; }
        public string ExistingCancelledChequePath { get; set; }
        public IFormFile NewCancelledCheque { get; set; }  // For re-upload

        // Group 7: Address Details
        public bool ShowAddressDetails { get; set; }
        public string AddressDetailsComment { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PINCode { get; set; }

        // Group 8: Contact Details
        public bool ShowContactDetails { get; set; }
        public string ContactDetailsComment { get; set; }
        public string PrimaryContactName { get; set; }
        public string Designation { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string AlternateContactPerson { get; set; }
        public string AadharNumber { get; set; }
        public string ExistingAadharCopyPath { get; set; }
        public IFormFile NewAadharCopy { get; set; }  // For re-upload
    }
}