namespace VendorOnboardingSystem.Models
{
    public class VendorDocument
    {
        public int VendorDocumentId { get; set; }
        public int VendorId { get; set; }
        public string DocumentType { get; set; }
        public string FilePath { get; set; }
        public Vendor Vendor { get; set; }
    }
}
