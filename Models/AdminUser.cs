using System.ComponentModel.DataAnnotations;

namespace VendorOnboardingSystem.Models
{
    public class AdminUser
    {
        public int AdminUserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
        public bool IsSuperAdmin { get; set; } = false;
    }
}