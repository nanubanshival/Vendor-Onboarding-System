using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendorOnboardingSystem.Data;
using VendorOnboardingSystem.Models;
using VendorOnboardingSystem.ViewModels;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace VendorOnboardingSystem.Controllers
{
    public class VendorLoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VendorLoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vendor Login Page
        [HttpGet]
        public IActionResult Index()
        {
            // If already logged in, redirect to status page
            if (HttpContext.Session.GetString("VendorEmail") != null)
            {
                return RedirectToAction("Status");
            }
            return View();
        }

        // POST: Vendor Login
        [HttpPost]
        public async Task<IActionResult> Index(VendorLoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v => v.Email == model.Email);

            if (vendor == null)
            {
                ViewBag.Error = "Invalid email or password";
                return View(model);
            }

            // Check if account is locked
            if (vendor.IsLockedOut)
            {
                ViewBag.Error = "Your account is locked. Please contact admin.";
                return View(model);
            }

            // Verify password
            if (!VerifyPassword(model.Password, vendor.Password))
            {
                vendor.LoginAttempts++;
                if (vendor.LoginAttempts >= 5)
                {
                    vendor.IsLockedOut = true;
                }
                await _context.SaveChangesAsync();

                ViewBag.Error = "Invalid email or password";
                return View(model);
            }

            // Reset login attempts on successful login
            vendor.LoginAttempts = 0;
            vendor.LastLoginAt = DateTime.Now;
            await _context.SaveChangesAsync();

            // Set session
            HttpContext.Session.SetString("VendorEmail", vendor.Email);
            HttpContext.Session.SetInt32("VendorId", vendor.VendorId);

            return RedirectToAction("Status");
        }

        // GET: Vendor Status Page
        public IActionResult Status()
        {
            var email = HttpContext.Session.GetString("VendorEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index");
            }

            var vendor = _context.Vendors.FirstOrDefault(v => v.Email == email);
            if (vendor == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }

            var editRequests = _context.EditRequests
                .Where(r => r.VendorId == vendor.VendorId)
                .OrderBy(r => r.RequestDate)
                .ToList();

            var vm = new VendorStatusVM
            {
                Vendor = vendor,
                EditRequests = editRequests,
                Timeline = BuildTimeline(vendor, editRequests)
            };

            return View(vm);
        }

        private List<TimelineEvent> BuildTimeline(Vendor vendor, List<EditRequest> editRequests)
        {
            var events = new List<TimelineEvent>();

            events.Add(new TimelineEvent
            {
                EventDate = vendor.CreatedAt,
                EventType = "Submitted",
                Title = "Application Submitted",
                Description = $"You submitted your vendor registration. Reference: {vendor.ReferenceNumber}",
                Actor = "You",
                Icon = "bi-send-check",
                Color = "primary"
            });

            int round = 0;
            foreach (var req in editRequests)
            {
                round++;
                var sections = req.FieldsToUpdate?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
                var comments = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(req.FieldComments))
                {
                    try { comments = JsonSerializer.Deserialize<Dictionary<string, string>>(req.FieldComments); }
                    catch { comments = new Dictionary<string, string>(); }
                }

                events.Add(new TimelineEvent
                {
                    EventDate = req.RequestDate,
                    EventType = "EditsRequested",
                    Title = $"Admin Requested Changes (Round {round})",
                    Description = sections.Any()
                        ? $"Admin asked you to update {sections.Count} section(s)."
                        : "Admin asked you to review your application.",
                    Actor = "Admin",
                    Icon = "bi-pencil-square",
                    Color = "warning",
                    AffectedSections = sections,
                    SectionComments = comments,
                    Notes = req.AdminNotes,
                    EditRequestId = req.Id,
                    RoundNumber = round
                });

                if (req.Status == "Completed" && req.VendorResponseDate.HasValue)
                {
                    events.Add(new TimelineEvent
                    {
                        EventDate = req.VendorResponseDate.Value,
                        EventType = "EditsSubmitted",
                        Title = $"You Submitted Updates (Round {round})",
                        Description = "You responded to the requested changes and resubmitted for review.",
                        Actor = "You",
                        Icon = "bi-check2-square",
                        Color = "info",
                        AffectedSections = sections,
                        EditRequestId = req.Id,
                        RoundNumber = round
                    });
                }
            }

            var statusEventDate = vendor.UpdatedAt
                ?? editRequests.LastOrDefault()?.VendorResponseDate
                ?? editRequests.LastOrDefault()?.RequestDate
                ?? vendor.CreatedAt;

            switch (vendor.Status)
            {
                case "PendingSuperAdmin":
                    events.Add(new TimelineEvent
                    {
                        EventDate = statusEventDate,
                        EventType = "SentToSuperAdmin",
                        Title = "Forwarded for Final Approval",
                        Description = "Your application was reviewed and forwarded to a super admin for final approval.",
                        Actor = "Admin",
                        Icon = "bi-shield-check",
                        Color = "info"
                    });
                    break;
                case "Approved":
                    events.Add(new TimelineEvent
                    {
                        EventDate = statusEventDate,
                        EventType = "Approved",
                        Title = "Application Approved",
                        Description = string.IsNullOrEmpty(vendor.VendorCode)
                            ? "Your application was approved."
                            : $"Your application was approved. Vendor Code: {vendor.VendorCode}",
                        Actor = "Admin",
                        Icon = "bi-check-circle-fill",
                        Color = "success"
                    });
                    break;
                case "Rejected":
                    events.Add(new TimelineEvent
                    {
                        EventDate = statusEventDate,
                        EventType = "Rejected",
                        Title = "Application Rejected",
                        Description = string.IsNullOrEmpty(vendor.RejectionReason)
                            ? "Your application was rejected."
                            : $"Reason: {vendor.RejectionReason}",
                        Actor = "Admin",
                        Icon = "bi-x-circle-fill",
                        Color = "danger"
                    });
                    break;
            }

            return events.OrderByDescending(e => e.EventDate).ToList();
        }

        // GET: Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        // Helper: Generate random password (8 characters)
        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Helper: Hash password (simple - for demo only)
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // Helper: Verify password
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var hash = HashPassword(enteredPassword);
            return hash == storedHash;
        }

        // This method will be called from VendorController after registration
        public static string GenerateAndHashPassword(out string plainPassword)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            plainPassword = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(plainPassword);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}