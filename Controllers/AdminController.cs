using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendorOnboardingSystem.Data;
using VendorOnboardingSystem.Models;
using VendorOnboardingSystem.ViewModels;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;

namespace VendorOnboardingSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AdminController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var admin = _context.AdminUsers
                .FirstOrDefault(x => x.Username == username && x.Password == password);

            if (admin != null)
            {
                HttpContext.Session.SetString("AdminUser", admin.Username);
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            var pendingVendors = _context.Vendors
                .Where(x => x.Status == "Pending" || x.Status == "Changes Submitted")
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var adminUsername = HttpContext.Session.GetString("AdminUser");
            var currentAdmin = _context.AdminUsers.FirstOrDefault(a => a.Username == adminUsername);
            ViewBag.IsSuperAdmin = currentAdmin?.IsSuperAdmin ?? false;
            ViewBag.AdminName = adminUsername;

            ViewBag.PendingCount = pendingVendors.Count(x => x.Status == "Pending");
            ViewBag.ChangesSubmittedCount = pendingVendors.Count(x => x.Status == "Changes Submitted");
            ViewBag.PendingSuperAdminCount = _context.Vendors.Count(x => x.Status == "PendingSuperAdmin");
            ViewBag.ApprovedCount = _context.Vendors.Count(x => x.Status == "Approved");
            ViewBag.RejectedCount = _context.Vendors.Count(x => x.Status == "Rejected");
            ViewBag.ChangesRequestedCount = _context.Vendors.Count(x => x.Status == "Changes Requested");
            ViewBag.TotalVendors = _context.Vendors.Count();

            return View(pendingVendors);
        }

        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            var vendor = _context.Vendors.FirstOrDefault(x => x.VendorId == id);
            var bank = _context.VendorBanks.FirstOrDefault(x => x.VendorId == id);
            var docs = _context.VendorDocuments.Where(x => x.VendorId == id).ToList();

            ViewBag.Bank = bank;
            ViewBag.Documents = docs;

            return View(vendor);
        }

        private string GenerateVendorCode()
        {
            var count = _context.Vendors.Count(x => x.Status == "Approved") + 1;
            return "VND" + count.ToString("D4");
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            var vendor = _context.Vendors.FirstOrDefault(x => x.VendorId == id);
            if (vendor == null) return NotFound();

            // Get current admin
            var adminUsername = HttpContext.Session.GetString("AdminUser");
            var currentAdmin = _context.AdminUsers.FirstOrDefault(a => a.Username == adminUsername);
            bool isSuperAdmin = currentAdmin?.IsSuperAdmin ?? false;

            if (vendor.Status == "Approved")
                return RedirectToAction("Dashboard");

            // If vendor is already in pending super admin state, super admin can final approve
            if (vendor.Status == "PendingSuperAdmin" && isSuperAdmin)
            {
                vendor.Status = "Approved";
                vendor.IsLocked = true;
                vendor.VendorCode = GenerateVendorCode();
                vendor.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Vendor approved successfully.";
                return RedirectToAction("Dashboard");
            }

            // Normal pending vendor
            if (vendor.Status == "Pending")
            {
                if (isSuperAdmin)
                {
                    // Super admin approves directly
                    vendor.Status = "Approved";
                    vendor.IsLocked = true;
                    vendor.VendorCode = GenerateVendorCode();
                }
                else
                {
                    // Regular admin approves -> move to pending super admin
                    vendor.Status = "PendingSuperAdmin";
                }
                vendor.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                TempData["SuccessMessage"] = isSuperAdmin ? "Vendor approved." : "Vendor sent for super admin approval.";
                return RedirectToAction("Dashboard");
            }

            return RedirectToAction("Dashboard");
        }

        public IActionResult PendingSuperAdmin()
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            // Check if current admin is super admin
            var adminUsername = HttpContext.Session.GetString("AdminUser");
            var currentAdmin = _context.AdminUsers.FirstOrDefault(a => a.Username == adminUsername);
            if (currentAdmin == null )
                return RedirectToAction("Dashboard");

            var pendingVendors = _context.Vendors
                .Where(x => x.Status == "PendingSuperAdmin")
                .ToList();

            return View(pendingVendors);
        }

        [HttpPost]
        public IActionResult Reject(int id, string reason)
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            var vendor = _context.Vendors.FirstOrDefault(x => x.VendorId == id);

            if (vendor == null)
                return NotFound();

            vendor.Status = "Rejected";
            vendor.RejectionReason = reason;
            vendor.IsLocked = false;
            vendor.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        public IActionResult Approved()
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            var approved = _context.Vendors
                .Where(x => x.Status == "Approved")
                .ToList();

            return View(approved);
        }

        public IActionResult Rejected()
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            var rejected = _context.Vendors
                .Where(x => x.Status == "Rejected")
                .ToList();

            return View(rejected);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // New Invitation Methods
        public IActionResult InviteVendor()
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InviteVendor(InvitationVM model)
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Check if email already has a pending invitation
                var existingInvitation = _context.RegistrationInvitations
                    .FirstOrDefault(x => x.Email == model.Email && x.Status == "Pending" && x.ExpiryDate > DateTime.Now);

                if (existingInvitation != null)
                {
                    ViewBag.Message = "An invitation has already been sent to this email.";
                    return View(model);
                }

                // Generate unique token
                var token = GenerateUniqueToken();

                var invitation = new RegistrationInvitation
                {
                    Email = model.Email,
                    Token = token,
                    CompanyName = model.CompanyName ?? "",
                    ExpiryDate = DateTime.Now.AddDays(7),
                    IsUsed = false,
                    CreatedAt = DateTime.Now,
                    Status = "Pending",
                    LinkType = "Email"
                };

                _context.RegistrationInvitations.Add(invitation);

                // TRY TO SAVE AND CATCH THE FULL ERROR
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    // Get the inner exception details
                    var errorMessage = dbEx.Message;
                    if (dbEx.InnerException != null)
                    {
                        errorMessage += " | Inner: " + dbEx.InnerException.Message;
                        if (dbEx.InnerException.InnerException != null)
                        {
                            errorMessage += " | Deep: " + dbEx.InnerException.InnerException.Message;
                        }
                    }

                    ViewBag.Error = $"Database error: {errorMessage}";
                    return View(model);
                }

                // Generate registration link
                var registrationLink = Url.Action("RegisterWithToken", "Vendor",
                    new { token = token }, protocol: HttpContext.Request.Scheme);

                // Send email
                await SendInvitationEmail(model.Email, registrationLink, model.CompanyName);

                ViewBag.Message = "Invitation sent successfully!";
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(model);
            }
        }

        private string GenerateUniqueToken()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            string token;

            do
            {
                token = new string(Enumerable.Repeat(chars, 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            while (_context.RegistrationInvitations.Any(x => x.Token == token));

            return token;
        }

        private async Task SendInvitationEmail(string toEmail, string registrationLink, string companyName = "")
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");

                // Log email settings (for debugging)
                Console.WriteLine($"SMTP Server: {emailSettings["SmtpServer"]}");
                Console.WriteLine($"SMTP Port: {emailSettings["SmtpPort"]}");
                Console.WriteLine($"Sender Email: {emailSettings["SenderEmail"]}");

                var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
                {
                    Port = int.Parse(emailSettings["SmtpPort"]),
                    Credentials = new NetworkCredential(emailSettings["SenderEmail"], emailSettings["SenderPassword"]),
                    EnableSsl = bool.Parse(emailSettings["EnableSsl"]),
                    Timeout = 10000 // 10 seconds timeout
                };

                var companyText = string.IsNullOrEmpty(companyName) ? "" : $" for {companyName}";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailSettings["SenderEmail"]),
                    Subject = "Vendor Registration Invitation",
                    Body = $@"
                <h2>Vendor Registration Invitation</h2>
                <p>You have been invited to register as a vendor{companyText}.</p>
                <p>Please click the link below to complete your registration:</p>
                <p><a href='{registrationLink}'>Register as Vendor</a></p>
                <p><strong>This link will expire in 7 days.</strong></p>
                <p>If you did not request this invitation, please ignore this email.</p>
                <hr>
                <p style='color: #666; font-size: 12px;'>This is an automated message, please do not reply.</p>
            ",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException smtpEx)
            {
                // Log the SMTP error
                Console.WriteLine($"SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
                throw new Exception($"Email failed: {smtpEx.Message}", smtpEx);
            }
            catch (Exception ex)
            {
                // Log the general error
                Console.WriteLine($"Email sending failed: {ex.Message}");
                throw new Exception($"Email failed: {ex.Message}", ex);
            }
        }

        public IActionResult Invitations()
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            var invitations = _context.RegistrationInvitations
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return View(invitations);
        }

        [HttpPost]
        public async Task<IActionResult> ResendInvitation(int id)
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            var invitation = _context.RegistrationInvitations.Find(id);
            if (invitation == null)
                return NotFound();

            invitation.Token = GenerateUniqueToken();
            invitation.ExpiryDate = DateTime.Now.AddDays(7);
            invitation.CreatedAt = DateTime.Now;
            invitation.Status = "Pending";
            invitation.IsUsed = false;

            await _context.SaveChangesAsync();

            var registrationLink = Url.Action("RegisterWithToken", "Vendor",
                new { token = invitation.Token }, protocol: HttpContext.Request.Scheme);

            await SendInvitationEmail(invitation.Email, registrationLink, invitation.CompanyName);

            TempData["SuccessMessage"] = "Invitation resent successfully!";
            return RedirectToAction("Invitations");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteInvitation(int id)
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            var invitation = _context.RegistrationInvitations.Find(id);
            if (invitation != null)
            {
                _context.RegistrationInvitations.Remove(invitation);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Invitation deleted successfully!";
            }

            return RedirectToAction("Invitations");
        }

        public IActionResult TestEmail()
        {
            try
            {
                var testLink = "https://localhost:7001/Vendor/RegisterWithToken?token=test123";
                SendInvitationEmail("your-test-email@gmail.com", testLink, "Test Company").Wait();
                return Content("Email sent successfully! Check your inbox.");
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }

        //Generate Link Manually

        // GET: Generate Manual Link
        public IActionResult GenerateLink()
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            return View(new GenerateLinkVM());
        }

        // POST: Generate Manual Link
        [HttpPost]
        public async Task<IActionResult> GenerateLink(GenerateLinkVM model)
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Generate unique token
                var token = GenerateUniqueToken();

                // Create invitation with manual link type (no email)
                var invitation = new RegistrationInvitation
                {
                    Email = "manual-" + Guid.NewGuid().ToString() + "@manual.link", // Dummy email for manual links
                    Token = token,
                    CompanyName = model.CompanyName ?? "",
                    ExpiryDate = DateTime.Now.AddDays(model.ExpiryDays),
                    IsUsed = false,
                    CreatedAt = DateTime.Now,
                    Status = "Pending",
                    LinkType = "Manual"
                };

                _context.RegistrationInvitations.Add(invitation);
                await _context.SaveChangesAsync();

                // Generate registration link
                var registrationLink = Url.Action("RegisterWithToken", "Vendor",
                    new { token = token }, protocol: HttpContext.Request.Scheme);

                // Store link in TempData to show on next page
                TempData["GeneratedLink"] = registrationLink;
                TempData["ExpiryDate"] = invitation.ExpiryDate.ToString("dd-MMM-yyyy HH:mm");
                TempData["Token"] = token;

                return RedirectToAction("LinkGenerated");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error generating link: {ex.Message}");
                return View(model);
            }
        }

        // Show generated link
        public IActionResult LinkGenerated()
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            if (TempData["GeneratedLink"] == null)
                return RedirectToAction("GenerateLink");

            return View();
        }

        // API endpoint to get link details (for AJAX copy)
        [HttpGet]
        public IActionResult GetLinkDetails(string token)
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return Unauthorized();

            var invitation = _context.RegistrationInvitations
                .FirstOrDefault(x => x.Token == token);

            if (invitation == null)
                return NotFound();

            var registrationLink = Url.Action("RegisterWithToken", "Vendor",
                new { token = token }, protocol: HttpContext.Request.Scheme);

            return Json(new
            {
                link = registrationLink,
                expiryDate = invitation.ExpiryDate.ToString("dd-MMM-yyyy HH:mm"),
                companyName = invitation.CompanyName,
                status = invitation.Status
            });
        }

        // Request edits from vendor

        [HttpPost]
        public async Task<IActionResult> CreateEditRequest(EditRequestVM model)
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            try
            {
                var vendor = await _context.Vendors.FindAsync(model.VendorId);
                if (vendor == null)
                    return NotFound();

                // Check if there's already a pending edit request
                var existingRequest = await _context.EditRequests
                    .FirstOrDefaultAsync(r => r.VendorId == model.VendorId && r.Status == "Pending");

                if (existingRequest != null)
                {
                    TempData["ErrorMessage"] = "This vendor already has a pending edit request.";
                    return RedirectToAction("Details", new { id = model.VendorId });
                }

                // Get current admin
                var adminUsername = HttpContext.Session.GetString("AdminUser");
                var admin = _context.AdminUsers.FirstOrDefault(a => a.Username == adminUsername);

                // Build fields string and comments - USING GROUPED FIELDS
                var fields = new List<string>();
                var comments = new Dictionary<string, string>();

                if (model.LegalInfo) { fields.Add("LegalInfo"); comments["LegalInfo"] = model.LegalInfoComment; }
                if (model.PANDetails) { fields.Add("PANDetails"); comments["PANDetails"] = model.PANDetailsComment; }
                if (model.GSTDetails) { fields.Add("GSTDetails"); comments["GSTDetails"] = model.GSTDetailsComment; }
                if (model.MSMEDetails) { fields.Add("MSMEDetails"); comments["MSMEDetails"] = model.MSMEDetailsComment; }
                if (model.TDSDetails) { fields.Add("TDSDetails"); comments["TDSDetails"] = model.TDSDetailsComment; }
                if (model.BankDetails) { fields.Add("BankDetails"); comments["BankDetails"] = model.BankDetailsComment; }
                if (model.AddressDetails) { fields.Add("AddressDetails"); comments["AddressDetails"] = model.AddressDetailsComment; }
                if (model.ContactDetails) { fields.Add("ContactDetails"); comments["ContactDetails"] = model.ContactDetailsComment; }

                var editRequest = new EditRequest
                {
                    VendorId = model.VendorId,
                    AdminId = admin.AdminUserId,
                    RequestDate = DateTime.Now,
                    Status = "Pending",
                    AdminNotes = model.Comments,
                    FieldsToUpdate = string.Join(",", fields),
                    FieldComments = System.Text.Json.JsonSerializer.Serialize(comments)
                };

                // Update vendor status
                vendor.Status = "Changes Requested";
                vendor.HasPendingEditRequest = true;
                vendor.UpdatedAt = DateTime.Now;

                _context.EditRequests.Add(editRequest);
                await _context.SaveChangesAsync();

                // Send email to vendor
                await SendEditRequestEmail(vendor.Email, vendor.LegalName, editRequest.Id);

                TempData["SuccessMessage"] = "Edit request sent to vendor successfully!";
                return RedirectToAction("Details", new { id = model.VendorId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("Details", new { id = model.VendorId });
            }
        }



        private async Task SendEditRequestEmail(string email, string name, int requestId)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
                {
                    Port = int.Parse(emailSettings["SmtpPort"]),
                    Credentials = new NetworkCredential(emailSettings["SenderEmail"], emailSettings["SenderPassword"]),
                    EnableSsl = bool.Parse(emailSettings["EnableSsl"])
                };

                var link = Url.Action("EditRequest", "Vendor", new { id = requestId }, Request.Scheme);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailSettings["SenderEmail"]),
                    Subject = "Action Required: Changes Requested for Your Vendor Application",
                    Body = $@"
                <h2>Hello {name},</h2>
                <p>Admin has reviewed your vendor application and requested some changes.</p>
                <p>Please click the button below to review and update your information:</p>
                <p><a href='{link}' style='background-color: #ffc107; color: #000; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Review Changes</a></p>
                <p>If you have any questions, please contact admin.</p>
                <hr>
                <p style='color: #666;'>This is an automated message, please do not reply.</p>
            ",
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email error: {ex.Message}");
            }
        }

        public async Task<IActionResult> ReviewSubmittedEdits(int id)
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null) return NotFound();

            // Get the most recent completed edit request for this vendor
            var editRequest = await _context.EditRequests
                .Where(r => r.VendorId == id && r.Status == "Completed")
                .OrderByDescending(r => r.RequestDate)
                .FirstOrDefaultAsync();

            if (editRequest == null)
            {
                TempData["ErrorMessage"] = "No edit submission found for this vendor.";
                return RedirectToAction("Dashboard");
            }

            // Get the fields that were requested to be edited
            var requestedFields = editRequest.FieldsToUpdate?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

            ViewBag.RequestedFields = requestedFields;
            ViewBag.EditRequest = editRequest;

            return View(vendor);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveSubmittedEdits(int id)
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null) return NotFound();

            if (vendor.Status != "Changes Submitted")
            {
                TempData["ErrorMessage"] = "This vendor is not in submitted edits state.";
                return RedirectToAction("Dashboard");
            }

            // Move back to pending for final approval (or you can directly approve here)
            vendor.Status = "Pending";
            vendor.UpdatedAt = DateTime.Now;

            // Optional: Add a note that edits were approved
            var editRequest = await _context.EditRequests
                .Where(r => r.VendorId == id && r.Status == "Completed")
                .OrderByDescending(r => r.RequestDate)
                .FirstOrDefaultAsync();

            if (editRequest != null)
            {
                editRequest.AdminNotes = (editRequest.AdminNotes ?? "") + " | Edits approved on " + DateTime.Now.ToString("dd-MMM-yyyy");
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Vendor edits approved. Vendor moved to pending queue for final approval.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> RejectSubmittedEdits(int id, string reason)
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null) return NotFound();

            vendor.Status = "Changes Requested"; // Send back for more changes
            vendor.RejectionReason = reason;
            vendor.UpdatedAt = DateTime.Now;

            // Create a new edit request or update existing?
            var editRequest = await _context.EditRequests
                .Where(r => r.VendorId == id && r.Status == "Completed")
                .OrderByDescending(r => r.RequestDate)
                .FirstOrDefaultAsync();

            if (editRequest != null)
            {
                
                var newEditRequest = new EditRequest
                {
                    VendorId = vendor.VendorId,
                    AdminId = editRequest.AdminId,
                    RequestDate = DateTime.Now,
                    Status = "Pending",
                    AdminNotes = reason ?? "Please review and make the required changes",
                    FieldsToUpdate = editRequest.FieldsToUpdate, 
                    FieldComments = editRequest.FieldComments
                };

                _context.EditRequests.Add(newEditRequest);
                vendor.HasPendingEditRequest = true;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Edits rejected. Vendor notified to make more changes.";
            return RedirectToAction("Dashboard");
        }

        public IActionResult PendingEdits()
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToAction("Login");

            var vendorsWithEditRequests = _context.Vendors
                .Where(v => v.Status == "Changes Requested")
                .OrderByDescending(v => v.UpdatedAt ?? v.CreatedAt)
                .ToList();

            return View(vendorsWithEditRequests);
        }



        // List all admins (super admin only)
        public IActionResult ManageAdmins()
        {
            if (!IsSuperAdmin()) return RedirectToAction("Dashboard");
            var admins = _context.AdminUsers.ToList();
            return View(admins);
        }

        // Create new admin (GET)
        public IActionResult CreateAdmin()
        {
            if (!IsSuperAdmin()) return RedirectToAction("Dashboard");
            return View();
        }

        // Create new admin (POST)
        [HttpPost]
        public async Task<IActionResult> CreateAdmin(string username, string password)
        {
            if (!IsSuperAdmin()) return RedirectToAction("Dashboard");
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Username and password required";
                return View();
            }
            // Check if username exists
            var existing = _context.AdminUsers.FirstOrDefault(a => a.Username == username);
            if (existing != null)
            {
                ViewBag.Error = "Username already exists";
                return View();
            }
            var admin = new AdminUser
            {
                Username = username,
                Password = password, // In production, hash password!
                IsSuperAdmin = false
            };
            _context.AdminUsers.Add(admin);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Admin created successfully";
            return RedirectToAction("ManageAdmins");
        }

        // Delete admin
        [HttpPost]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            if (!IsSuperAdmin()) return RedirectToAction("Dashboard");
            var admin = _context.AdminUsers.Find(id);
            if (admin == null) return NotFound();
            // Prevent deleting yourself
            var currentUsername = HttpContext.Session.GetString("AdminUser");
            if (admin.Username == currentUsername)
            {
                TempData["Error"] = "Cannot delete yourself";
                return RedirectToAction("ManageAdmins");
            }
            _context.AdminUsers.Remove(admin);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Admin deleted";
            return RedirectToAction("ManageAdmins");
        }

        private bool IsSuperAdmin()
        {
            var username = HttpContext.Session.GetString("AdminUser");
            var admin = _context.AdminUsers.FirstOrDefault(a => a.Username == username);
            return admin != null && admin.IsSuperAdmin;
        }
    }
}