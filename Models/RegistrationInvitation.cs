using System;
using System.ComponentModel.DataAnnotations;

namespace VendorOnboardingSystem.Models
{
    public class RegistrationInvitation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Token { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool IsUsed { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; } // Pending, Used, Expired, Completed

        public string? CompanyName { get; set; }

        // New property to track link type
        public string LinkType { get; set; } // "Email" or "Manual"
    }
}