using System.ComponentModel.DataAnnotations;

namespace VendorOnboardingSystem.ViewModels
{
    public class GenerateLinkVM
    {
        [Display(Name = "Company Name (Optional)")]
        public string? CompanyName { get; set; }

        [Display(Name = "Expiry Days")]
        [Range(1, 30, ErrorMessage = "Expiry days must be between 1 and 30")]
        public int ExpiryDays { get; set; } = 3;

        [Display(Name = "Notes (Optional)")]
        public string? Notes { get; set; }
    }
}