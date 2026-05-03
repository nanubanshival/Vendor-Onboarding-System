namespace VendorOnboardingSystem.Models
{
    public class VendorBank
    {
        public int VendorBankId { get; set; }

        public int VendorId { get; set; }

        public string AccountNumber { get; set; }

        public string IFSC { get; set; }

        public string BankName { get; set; }

        public Vendor Vendor { get; set; }
    }
}
