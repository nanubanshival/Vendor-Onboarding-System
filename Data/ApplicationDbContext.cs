using Microsoft.EntityFrameworkCore;
using VendorOnboardingSystem.Models;

namespace VendorOnboardingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<VendorBank> VendorBanks { get; set; }
        public DbSet<VendorDocument> VendorDocuments { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<RegistrationInvitation> RegistrationInvitations { get; set; }
        public DbSet<EditRequest> EditRequests { get; set; }
        public DbSet<VendorEdit> VendorEdits { get; set; }

    }
}