using System;
using System.ComponentModel.DataAnnotations;

namespace VendorOnboardingSystem.Models
{
    public class VendorEdit
    {
        [Key]
        public int Id { get; set; }

        public int EditRequestId { get; set; }
        public EditRequest EditRequest { get; set; }

        public DateTime SubmittedDate { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected

        // Store updated values as JSON
        public string UpdatedData { get; set; }

        public string AdminResponse { get; set; }
    }
}