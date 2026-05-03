using System;
using System.ComponentModel.DataAnnotations;

namespace VendorOnboardingSystem.Models
{
    public class EditRequest
    {
        [Key]
        public int Id { get; set; }

        public int VendorId { get; set; }
        public Vendor Vendor { get; set; }

        public int AdminId { get; set; }
        public AdminUser Admin { get; set; }

        public DateTime RequestDate { get; set; }
        public string Status { get; set; } // Pending, Completed, Expired

        [DataType(DataType.MultilineText)]
        public string AdminNotes { get; set; } // General notes from admin

        public DateTime? VendorResponseDate { get; set; }

        // For storing which fields need changes (simple approach)
        public string FieldsToUpdate { get; set; } // Comma-separated: "PAN,IFSC,BankName"

        // Store admin comments per field (simple JSON)
        public string FieldComments { get; set; } // Will store as JSON string
    }
}