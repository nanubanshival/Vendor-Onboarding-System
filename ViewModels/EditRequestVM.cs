using System.ComponentModel.DataAnnotations;

namespace VendorOnboardingSystem.ViewModels
{
    public class EditRequestVM
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorEmail { get; set; }

        // GROUP 1: Legal Information
        public bool LegalInfo { get; set; }
        public string LegalInfoComment { get; set; }

        // GROUP 2: PAN Details
        public bool PANDetails { get; set; }
        public string PANDetailsComment { get; set; }

        // GROUP 3: GST Details
        public bool GSTDetails { get; set; }
        public string GSTDetailsComment { get; set; }

        // GROUP 4: MSME Details
        public bool MSMEDetails { get; set; }
        public string MSMEDetailsComment { get; set; }

        // GROUP 5: TDS Details
        public bool TDSDetails { get; set; }
        public string TDSDetailsComment { get; set; }

        // GROUP 6: Bank Details
        public bool BankDetails { get; set; }
        public string BankDetailsComment { get; set; }

        // GROUP 7: Address Details
        public bool AddressDetails { get; set; }
        public string AddressDetailsComment { get; set; }

        // GROUP 8: Contact Information
        public bool ContactDetails { get; set; }
        public string ContactDetailsComment { get; set; }

        // General Comments for Admin
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }
    }
}