using System.ComponentModel.DataAnnotations;

namespace VendorOnboardingSystem.ViewModels
{
    public class InvitationVM
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Company Name (Optional)")]
        public string? CompanyName { get; set; }  // Keep as optional
    }
}