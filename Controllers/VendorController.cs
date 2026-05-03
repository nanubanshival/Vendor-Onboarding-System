using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using VendorOnboardingSystem.Data;
using VendorOnboardingSystem.Models;
using VendorOnboardingSystem.ViewModels;

namespace VendorOnboardingSystem.Controllers
{
    public class VendorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public VendorController(ApplicationDbContext context, IWebHostEnvironment env, IConfiguration configuration)
        {
            _context = context;
            _env = env;
            _configuration = configuration;
        }

        // GET
        public IActionResult Register()
        {
            return View();
        }

        // POST
        [HttpPost]
        //public async Task<IActionResult> Register(VendorRegistrationVM model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        // Log validation errors for debugging
        //        var errors = ModelState.Values.SelectMany(v => v.Errors);
        //        foreach (var error in errors)
        //        {
        //            Console.WriteLine($"Validation Error: {error.ErrorMessage}");
        //        }

        //        // Return the model with all entered data still intact
        //        return View(model);
        //    }

        //    var token = TempData["RegistrationToken"]?.ToString();

        //    if (!string.IsNullOrEmpty(token))
        //    {
        //        var invitation = _context.RegistrationInvitations
        //            .FirstOrDefault(x => x.Token == token && !x.IsUsed && x.ExpiryDate > DateTime.Now);

        //        if (invitation == null)
        //        {
        //            ModelState.AddModelError("", "Invalid or expired registration link.");
        //            return View(model);
        //        }
        //    }

        //    if (!ModelState.IsValid)
        //        return View(model);

        //    // === Check for existing PAN ===
        //    var existingPan = _context.Vendors.FirstOrDefault(x => x.PAN == model.PAN);
        //    if (existingPan != null)
        //    {
        //        ModelState.AddModelError("PAN", "PAN already registered.");
        //        return View(model);
        //    }

        //    // === Check for existing Email ===
        //    var existingEmail = _context.Vendors.FirstOrDefault(x => x.Email == model.Email);
        //    if (existingEmail != null)
        //    {
        //        ModelState.AddModelError("Email", "Email already registered.");
        //        return View(model);
        //    }

        //    try
        //    {
        //        // Generate password
        //        var plainPassword = GenerateRandomPassword();
        //        var hashedPassword = HashPassword(plainPassword);

        //        // === Create vendor object ===
        //        var vendor = new Vendor
        //        {
        //            // Section 1
        //            LegalName = model.LegalName,
        //            TradeName = model.TradeName,
        //            VendorType = model.VendorType,
        //            VendorCategory = model.VendorCategory,
        //            Constitution = model.Constitution,
        //            DateOfIncorporation = model.DateOfIncorporation,
        //            CIN = model.CIN,

        //            // Section 2
        //            PAN = model.PAN.ToUpper(),

        //            // Section 3
        //            GSTRegistered = model.GSTRegistered,
        //            GSTIN = model.GSTRegistered == "Yes" ? model.GSTIN?.ToUpper() : null,
        //            GSTRegistrationType = model.GSTRegistrationType,
        //            StateCode = model.StateCode,
        //            PlaceOfSupply = model.PlaceOfSupply,

        //            // Section 4
        //            MSMERegistered = model.MSMERegistered,
        //            UdyamNumber = model.MSMERegistered == "Yes" ? model.UdyamNumber?.ToUpper() : null,

        //            // Section 5
        //            TDSApplicable = model.TDSApplicable,
        //            TDSSections = model.TDSApplicable == "Yes" ? string.Join(",", model.SelectedTDSSections) : null,

        //            // Section 6
        //            AccountHolderName = model.AccountHolderName,
        //            BankName = model.BankName,
        //            AccountNumber = model.AccountNumber,
        //            IFSC = model.IFSC.ToUpper(),

        //            // Section 7
        //            AddressLine1 = model.AddressLine1,
        //            AddressLine2 = model.AddressLine2,
        //            City = model.City,
        //            State = model.State,
        //            PINCode = model.PINCode,
        //            Country = model.Country,

        //            // Section 8
        //            PrimaryContactName = model.PrimaryContactName,
        //            Designation = model.Designation,
        //            MobileNumber = model.MobileNumber,
        //            Email = model.Email,
        //            AlternateContactPerson = model.AlternateContactPerson,
        //            AadharNumber = model.AadharNumber,

        //            // System fields
        //            Status = "Pending",
        //            ReferenceNumber = GenerateReferenceNumber(),
        //            RejectionReason = "",
        //            IsLocked = false,
        //            CreatedAt = DateTime.Now,

        //            // Login New Fields
        //            Password = hashedPassword,
        //            LoginAttempts = 0,
        //            IsLockedOut = false
        //        };

        //        _context.Vendors.Add(vendor);
        //        await _context.SaveChangesAsync();

        //        // === File Upload Handling ===
        //        var vendorFolder = Path.Combine(_env.WebRootPath, "uploads", vendor.ReferenceNumber);
        //        Directory.CreateDirectory(vendorFolder);

        //        if (model.PANCopy != null)
        //        {
        //            var panFile = $"PAN_{DateTime.Now.Ticks}{Path.GetExtension(model.PANCopy.FileName)}";
        //            var panPath = Path.Combine(vendorFolder, panFile);
        //            using var panStream = new FileStream(panPath, FileMode.Create);
        //            await model.PANCopy.CopyToAsync(panStream);
        //            vendor.PANCopyPath = $"/uploads/{vendor.ReferenceNumber}/{panFile}";
        //        }

        //        if (model.GSTRegistered == "Yes" && model.GSTCertificate != null)
        //        {
        //            var gstFile = $"GST_{DateTime.Now.Ticks}{Path.GetExtension(model.GSTCertificate.FileName)}";
        //            var gstPath = Path.Combine(vendorFolder, gstFile);
        //            using var gstStream = new FileStream(gstPath, FileMode.Create);
        //            await model.GSTCertificate.CopyToAsync(gstStream);
        //            vendor.GSTCertificatePath = $"/uploads/{vendor.ReferenceNumber}/{gstFile}";
        //        }

        //        if (model.CancelledCheque != null)
        //        {
        //            var chequeFile = $"CHEQUE_{DateTime.Now.Ticks}{Path.GetExtension(model.CancelledCheque.FileName)}";
        //            var chequePath = Path.Combine(vendorFolder, chequeFile);
        //            using var chequeStream = new FileStream(chequePath, FileMode.Create);
        //            await model.CancelledCheque.CopyToAsync(chequeStream);
        //            vendor.CancelledChequePath = $"/uploads/{vendor.ReferenceNumber}/{chequeFile}";
        //        }

        //        if (model.AadharCopy != null)
        //        {
        //            var aadharFile = $"AADHAR_{DateTime.Now.Ticks}{Path.GetExtension(model.AadharCopy.FileName)}";
        //            var aadharPath = Path.Combine(vendorFolder, aadharFile);
        //            using var aadharStream = new FileStream(aadharPath, FileMode.Create);
        //            await model.AadharCopy.CopyToAsync(aadharStream);
        //            vendor.AadharCopyPath = $"/uploads/{vendor.ReferenceNumber}/{aadharFile}";
        //        }

        //        await _context.SaveChangesAsync();

        //        // === Send Email with Credentials ===
        //        await SendCredentialsEmail(model.Email, plainPassword, model.LegalName, vendor.ReferenceNumber);

        //        // === Mark invitation as used ===
        //        if (!string.IsNullOrEmpty(token))
        //        {
        //            var invitation = _context.RegistrationInvitations.FirstOrDefault(x => x.Token == token);
        //            if (invitation != null)
        //            {
        //                invitation.Status = "Completed";
        //                invitation.IsUsed = true;
        //                await _context.SaveChangesAsync();
        //            }
        //        }

        //        ViewBag.ReferenceNumber = vendor.ReferenceNumber;
        //        return View("Success");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error in registration: {ex.Message}");
        //        ModelState.AddModelError("", "An error occurred during registration. Please try again.");
        //        if (panFile != null) TempData["PANCopy_Name"] = file.FileName;
        //        if (gstFile != null) TempData["GSTCertificate_Name"] = gstFile.FileName;
        //        if (cheque != null) TempData["CancelledCheque_Name"] = cheque.FileName;
        //        if (aadhar != null) TempData["AadharCopy_Name"] = aadhar.FileName;
        //        return View(model);
        //    }
        //}

        public async Task<IActionResult> Register(VendorRegistrationVM model)
        {
            if (!ModelState.IsValid)
            {
                // Preserve uploaded file names in TempData so the view can show "previously uploaded" badges
                if (model.PANCopy != null) TempData["PANCopy_Name"] = model.PANCopy.FileName;
                if (model.GSTCertificate != null) TempData["GSTCertificate_Name"] = model.GSTCertificate.FileName;
                if (model.CancelledCheque != null) TempData["CancelledCheque_Name"] = model.CancelledCheque.FileName;
                if (model.AadharCopy != null) TempData["AadharCopy_Name"] = model.AadharCopy.FileName;

                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");

                return View(model);
            }

            var token = TempData["RegistrationToken"]?.ToString();

            if (!string.IsNullOrEmpty(token))
            {
                var invitation = _context.RegistrationInvitations
                    .FirstOrDefault(x => x.Token == token && !x.IsUsed && x.ExpiryDate > DateTime.Now);

                if (invitation == null)
                {
                    ModelState.AddModelError("", "Invalid or expired registration link.");
                    return View(model);
                }
            }

            // === Check for existing PAN ===
            var existingPan = _context.Vendors.FirstOrDefault(x => x.PAN == model.PAN);
            if (existingPan != null)
            {
                ModelState.AddModelError("PAN", "PAN already registered.");
                if (model.PANCopy != null) TempData["PANCopy_Name"] = model.PANCopy.FileName;
                return View(model);
            }

            // === Check for existing Email ===
            var existingEmail = _context.Vendors.FirstOrDefault(x => x.Email == model.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Email already registered.");
                return View(model);
            }

            try
            {
                // Generate password
                var plainPassword = GenerateRandomPassword();
                var hashedPassword = HashPassword(plainPassword);

                // === Create vendor object ===
                var vendor = new Vendor
                {
                    // Section 1
                    LegalName = model.LegalName,
                    TradeName = model.TradeName,
                    VendorType = model.VendorType,
                    VendorCategory = model.VendorCategory,
                    Constitution = model.Constitution,
                    DateOfIncorporation = model.DateOfIncorporation,
                    CIN = model.CIN,

                    // Section 2
                    PAN = model.PAN.ToUpper(),

                    // Section 3
                    GSTRegistered = model.GSTRegistered,
                    GSTIN = model.GSTRegistered == "Yes" ? model.GSTIN?.ToUpper() : null,
                    GSTRegistrationType = model.GSTRegistrationType,
                    StateCode = model.StateCode,
                    PlaceOfSupply = model.PlaceOfSupply,

                    // Section 4
                    MSMERegistered = model.MSMERegistered,
                    UdyamNumber = model.MSMERegistered == "Yes" ? model.UdyamNumber?.ToUpper() : null,

                    // Section 5
                    TDSApplicable = model.TDSApplicable,
                    TDSSections = model.TDSApplicable == "Yes" ? string.Join(",", model.SelectedTDSSections) : null,

                    // Section 6
                    AccountHolderName = model.AccountHolderName,
                    BankName = model.BankName,
                    AccountNumber = model.AccountNumber,
                    IFSC = model.IFSC.ToUpper(),

                    // Section 7
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    City = model.City,
                    State = model.State,
                    PINCode = model.PINCode,
                    Country = model.Country,

                    // Section 8
                    PrimaryContactName = model.PrimaryContactName,
                    Designation = model.Designation,
                    MobileNumber = model.MobileNumber,
                    Email = model.Email,
                    AlternateContactPerson = model.AlternateContactPerson,
                    AadharNumber = model.AadharNumber,

                    // System fields
                    Status = "Pending",
                    ReferenceNumber = GenerateReferenceNumber(),
                    RejectionReason = "",
                    IsLocked = false,
                    CreatedAt = DateTime.Now,

                    // Login fields
                    Password = hashedPassword,
                    LoginAttempts = 0,
                    IsLockedOut = false
                };

                _context.Vendors.Add(vendor);
                await _context.SaveChangesAsync();

                // === File Upload Handling ===
                var vendorFolder = Path.Combine(_env.WebRootPath, "uploads", vendor.ReferenceNumber);
                Directory.CreateDirectory(vendorFolder);

                if (model.PANCopy != null)
                {
                    var panFileName = $"PAN_{DateTime.Now.Ticks}{Path.GetExtension(model.PANCopy.FileName)}";
                    var panPath = Path.Combine(vendorFolder, panFileName);
                    using var panStream = new FileStream(panPath, FileMode.Create);
                    await model.PANCopy.CopyToAsync(panStream);
                    vendor.PANCopyPath = $"/uploads/{vendor.ReferenceNumber}/{panFileName}";
                }

                if (model.GSTRegistered == "Yes" && model.GSTCertificate != null)
                {
                    var gstFileName = $"GST_{DateTime.Now.Ticks}{Path.GetExtension(model.GSTCertificate.FileName)}";
                    var gstPath = Path.Combine(vendorFolder, gstFileName);
                    using var gstStream = new FileStream(gstPath, FileMode.Create);
                    await model.GSTCertificate.CopyToAsync(gstStream);
                    vendor.GSTCertificatePath = $"/uploads/{vendor.ReferenceNumber}/{gstFileName}";
                }

                if (model.CancelledCheque != null)
                {
                    var chequeFileName = $"CHEQUE_{DateTime.Now.Ticks}{Path.GetExtension(model.CancelledCheque.FileName)}";
                    var chequePath = Path.Combine(vendorFolder, chequeFileName);
                    using var chequeStream = new FileStream(chequePath, FileMode.Create);
                    await model.CancelledCheque.CopyToAsync(chequeStream);
                    vendor.CancelledChequePath = $"/uploads/{vendor.ReferenceNumber}/{chequeFileName}";
                }

                if (model.AadharCopy != null)
                {
                    var aadharFileName = $"AADHAR_{DateTime.Now.Ticks}{Path.GetExtension(model.AadharCopy.FileName)}";
                    var aadharPath = Path.Combine(vendorFolder, aadharFileName);
                    using var aadharStream = new FileStream(aadharPath, FileMode.Create);
                    await model.AadharCopy.CopyToAsync(aadharStream);
                    vendor.AadharCopyPath = $"/uploads/{vendor.ReferenceNumber}/{aadharFileName}";
                }

                await _context.SaveChangesAsync();

                // === Send Email with Credentials ===
                await SendCredentialsEmail(model.Email, plainPassword, model.LegalName, vendor.ReferenceNumber);

                // === Mark invitation as used ===
                if (!string.IsNullOrEmpty(token))
                {
                    var invitation = _context.RegistrationInvitations.FirstOrDefault(x => x.Token == token);
                    if (invitation != null)
                    {
                        invitation.Status = "Completed";
                        invitation.IsUsed = true;
                        await _context.SaveChangesAsync();
                    }
                }

                ViewBag.ReferenceNumber = vendor.ReferenceNumber;
                return View("Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in registration: {ex.Message}");
                ModelState.AddModelError("", "An error occurred during registration. Please try again.");

                // Preserve file names so the view can show badges even after a server-side exception
                if (model.PANCopy != null) TempData["PANCopy_Name"] = model.PANCopy.FileName;
                if (model.GSTCertificate != null) TempData["GSTCertificate_Name"] = model.GSTCertificate.FileName;
                if (model.CancelledCheque != null) TempData["CancelledCheque_Name"] = model.CancelledCheque.FileName;
                if (model.AadharCopy != null) TempData["AadharCopy_Name"] = model.AadharCopy.FileName;

                return View(model);
            }
        }

        private string GenerateReferenceNumber()
        {
            return "VR-" + DateTime.Now.Ticks.ToString().Substring(10);
        }

        // Generate 8-character random password
        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Hash password using SHA256
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // Send email with credentials
        private async Task SendCredentialsEmail(string email, string password, string name, string referenceNumber)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");

                // Check if email settings are configured
                if (string.IsNullOrEmpty(emailSettings["SmtpServer"]))
                {
                    Console.WriteLine("Email settings not configured. Skipping email send.");
                    return;
                }

                var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
                {
                    Port = int.Parse(emailSettings["SmtpPort"] ?? "587"),
                    Credentials = new NetworkCredential(emailSettings["SenderEmail"], emailSettings["SenderPassword"]),
                    EnableSsl = bool.Parse(emailSettings["EnableSsl"] ?? "true"),
                    Timeout = 10000
                };

                var loginUrl = Url.Action("Index", "VendorLogin", null, Request.Scheme);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailSettings["SenderEmail"] ?? "noreply@vendor.com"),
                    Subject = "Vendor Registration Successful - Your Login Credentials",
                    Body = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                            <div style='background-color: #0d6efd; color: white; padding: 20px; text-align: center;'>
                                <h2>Welcome {name}!</h2>
                            </div>
                            
                            <div style='padding: 30px; background-color: #f8f9fa;'>
                                <p>Your vendor registration has been submitted successfully.</p>
                                
                                <div style='background-color: white; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                                    <h3 style='color: #0d6efd; margin-top: 0;'>Your Login Credentials:</h3>
                                    <p><strong>Username:</strong> {email}</p>
                                    <p><strong>Password:</strong> <span style='font-family: monospace; font-size: 18px; background-color: #e9ecef; padding: 5px 10px; border-radius: 3px;'>{password}</span></p>
                                    <p><strong>Reference Number:</strong> {referenceNumber}</p>
                                </div>
                                
                                <p>You can login to check your application status:</p>
                                <p style='text-align: center;'>
                                    <a href='{loginUrl}' style='background-color: #0d6efd; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                                        Login to Check Status
                                    </a>
                                </p>
                                
                                <p style='color: #dc3545; font-size: 14px;'>
                                    <strong>Important:</strong> Please save this password. For security reasons, we cannot show it again.
                                </p>
                            </div>
                            
                            <div style='padding: 20px; text-align: center; color: #6c757d; font-size: 12px;'>
                                <p>This is an automated message, please do not reply.</p>
                                <p>&copy; {DateTime.Now.Year} Vendor Onboarding System</p>
                            </div>
                        </div>
                    ",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);

                Console.WriteLine($"Credentials email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                // Log error but don't stop registration
                Console.WriteLine($"Failed to send credentials email to {email}: {ex.Message}");
            }
        }

        private async Task SaveDocument(IFormFile file, int vendorId, string docType)
        {
            if (file == null) return;

            var folderPath = Path.Combine(_env.WebRootPath, "uploads", "vendors", vendorId.ToString());

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var doc = new VendorDocument
            {
                VendorId = vendorId,
                DocumentType = docType,
                FilePath = filePath
            };

            _context.VendorDocuments.Add(doc);
            await _context.SaveChangesAsync();
        }

        public IActionResult RegisterWithToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Register");

            var invitation = _context.RegistrationInvitations
                .FirstOrDefault(x => x.Token == token &&
                                    x.Status == "Pending" &&
                                    !x.IsUsed &&
                                    x.ExpiryDate > DateTime.Now);

            if (invitation == null)
            {
                ViewBag.Error = "Invalid or expired registration link. Please contact admin.";
                return View("InvalidToken");
            }

            // Store token in session or TempData to verify during registration
            TempData["RegistrationToken"] = token;

            // Return empty form - no pre-filled data
            var model = new VendorRegistrationVM();

            // Optional: passing company name in ViewBag in case needed to show it somewhere
            ViewBag.InvitedCompany = invitation.CompanyName; // Just for reference, not pre-filled

            return View("Register", model);
        }


        //edit request

        [HttpGet]
        public async Task<IActionResult> EditRequest(int id)
        {
            // Check if vendor is logged in
            var vendorEmail = HttpContext.Session.GetString("VendorEmail");
            if (string.IsNullOrEmpty(vendorEmail))
            {
                return RedirectToAction("Index", "VendorLogin");
            }

            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.Email == vendorEmail);
            if (vendor == null)
                return RedirectToAction("Index", "VendorLogin");

            var editRequest = await _context.EditRequests
                .FirstOrDefaultAsync(r => r.Id == id && r.VendorId == vendor.VendorId);

            if (editRequest == null)
                return NotFound();

            if (editRequest.Status != "Pending")
            {
                TempData["Message"] = "This edit request has already been processed.";
                return RedirectToAction("Status", "VendorLogin");
            }

            // Parse fields and comments
            var fields = editRequest.FieldsToUpdate?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
            var comments = string.IsNullOrEmpty(editRequest.FieldComments)
                ? new Dictionary<string, string>()
                : JsonSerializer.Deserialize<Dictionary<string, string>>(editRequest.FieldComments);

            var model = new VendorEditVM
            {
                EditRequestId = editRequest.Id,
                AdminNotes = editRequest.AdminNotes,

                // Set show flags based on fields
                ShowPANDetails = fields.Contains("PANDetails"),
                PANDetailsComment = comments.ContainsKey("PANDetails") ? comments["PANDetails"] : "",
                PAN = vendor.PAN,

                ShowGSTDetails = fields.Contains("GSTDetails"),
                GSTDetailsComment = comments.ContainsKey("GSTDetails") ? comments["GSTDetails"] : "",
                GSTIN = vendor.GSTIN,

                ShowBankDetails = fields.Contains("BankDetails"),
                BankDetailsComment = comments.ContainsKey("BankDetails") ? comments["BankDetails"] : "",
                BankName = vendor.BankName,
                AccountNumber = vendor.AccountNumber,
                IFSC = vendor.IFSC,

                ShowAddressDetails = fields.Contains("AddressDetails"),
                AddressDetailsComment = comments.ContainsKey("AddressDetails") ? comments["AddressDetails"] : "",
                AddressLine1 = vendor.AddressLine1,
                AddressLine2 = vendor.AddressLine2,
                City = vendor.City,
                State = vendor.State,
                PINCode = vendor.PINCode,

                ShowContactDetails = fields.Contains("ContactDetails"),
                ContactDetailsComment = comments.ContainsKey("ContactDetails") ? comments["ContactDetails"] : "",
                PrimaryContactName = vendor.PrimaryContactName,
                Designation = vendor.Designation,
                MobileNumber = vendor.MobileNumber,
                Email = vendor.Email,
                AlternateContactPerson = vendor.AlternateContactPerson,
                AadharNumber = vendor.AadharNumber,

                ShowLegalInfo = fields.Contains("LegalInfo"),
                LegalInfoComment = comments.ContainsKey("LegalInfo") ? comments["LegalInfo"] : "",

                ShowMSMEDetails = fields.Contains("MSMEDetails"),
                MSMEDetailsComment = comments.ContainsKey("MSMEDetails") ? comments["MSMEDetails"] : "",
                MSMERegistered = vendor.MSMERegistered,
                UdyamNumber = vendor.UdyamNumber,

                ShowTDSDetails = fields.Contains("TDSDetails"),
                TDSDetailsComment = comments.ContainsKey("TDSDetails") ? comments["TDSDetails"] : "",
                TDSApplicable = vendor.TDSApplicable,
                TDSSections = vendor.TDSSections
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRequest(VendorEditVM model)
        {
            var vendorEmail = HttpContext.Session.GetString("VendorEmail");
            if (string.IsNullOrEmpty(vendorEmail))
            {
                return RedirectToAction("Index", "VendorLogin");
            }

            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.Email == vendorEmail);
            if (vendor == null)
                return RedirectToAction("Index", "VendorLogin");

            var editRequest = await _context.EditRequests
                .FirstOrDefaultAsync(r => r.Id == model.EditRequestId && r.VendorId == vendor.VendorId);

            if (editRequest == null || editRequest.Status != "Pending")
                return NotFound();

            // Update vendor fields based on what was requested
            var fields = editRequest.FieldsToUpdate?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

            // Create uploads folder
            var vendorFolder = Path.Combine(_env.WebRootPath, "uploads", vendor.ReferenceNumber);
            if (!Directory.Exists(vendorFolder))
                Directory.CreateDirectory(vendorFolder);

            // Update PAN if requested
            if (fields.Contains("PANDetails"))
            {
                if (!string.IsNullOrEmpty(model.PAN))
                    vendor.PAN = model.PAN.ToUpper();

                if (model.NewPANCopy != null)
                {
                    var fileName = $"PAN_{DateTime.Now.Ticks}{Path.GetExtension(model.NewPANCopy.FileName)}";
                    var filePath = Path.Combine(vendorFolder, fileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await model.NewPANCopy.CopyToAsync(stream);
                    vendor.PANCopyPath = $"/uploads/{vendor.ReferenceNumber}/{fileName}";
                }
            }

            // Update GST if requested
            if (fields.Contains("GSTDetails"))
            {
                if (!string.IsNullOrEmpty(model.GSTIN))
                    vendor.GSTIN = model.GSTIN.ToUpper();

                if (model.NewGSTCertificate != null)
                {
                    var fileName = $"GST_{DateTime.Now.Ticks}{Path.GetExtension(model.NewGSTCertificate.FileName)}";
                    var filePath = Path.Combine(vendorFolder, fileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await model.NewGSTCertificate.CopyToAsync(stream);
                    vendor.GSTCertificatePath = $"/uploads/{vendor.ReferenceNumber}/{fileName}";
                }
            }

            // Update Bank if requested
            if (fields.Contains("BankDetails"))
            {
                if (!string.IsNullOrEmpty(model.BankName))
                    vendor.BankName = model.BankName;
                if (!string.IsNullOrEmpty(model.AccountNumber))
                    vendor.AccountNumber = model.AccountNumber;
                if (!string.IsNullOrEmpty(model.IFSC))
                    vendor.IFSC = model.IFSC.ToUpper();

                if (model.NewCancelledCheque != null)
                {
                    var fileName = $"CHEQUE_{DateTime.Now.Ticks}{Path.GetExtension(model.NewCancelledCheque.FileName)}";
                    var filePath = Path.Combine(vendorFolder, fileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await model.NewCancelledCheque.CopyToAsync(stream);
                    vendor.CancelledChequePath = $"/uploads/{vendor.ReferenceNumber}/{fileName}";
                }
            }

            // Update Address if requested
            if (fields.Contains("AddressDetails"))
            {
                if (!string.IsNullOrEmpty(model.AddressLine1))
                    vendor.AddressLine1 = model.AddressLine1;
                if (!string.IsNullOrEmpty(model.AddressLine2))
                    vendor.AddressLine2 = model.AddressLine2;
                if (!string.IsNullOrEmpty(model.City))
                    vendor.City = model.City;
                if (!string.IsNullOrEmpty(model.State))
                    vendor.State = model.State;
                if (!string.IsNullOrEmpty(model.PINCode))
                    vendor.PINCode = model.PINCode;
            }

            // Update Contact if requested
            if (fields.Contains("ContactDetails"))
            {
                if (!string.IsNullOrEmpty(model.PrimaryContactName))
                    vendor.PrimaryContactName = model.PrimaryContactName;
                if (!string.IsNullOrEmpty(model.MobileNumber))
                    vendor.MobileNumber = model.MobileNumber;
                if (!string.IsNullOrEmpty(model.Email))
                    vendor.Email = model.Email;
                if (!string.IsNullOrEmpty(model.AadharNumber))
                    vendor.AadharNumber = model.AadharNumber;

                if (model.NewAadharCopy != null)
                {
                    var fileName = $"AADHAR_{DateTime.Now.Ticks}{Path.GetExtension(model.NewAadharCopy.FileName)}";
                    var filePath = Path.Combine(vendorFolder, fileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await model.NewAadharCopy.CopyToAsync(stream);
                    vendor.AadharCopyPath = $"/uploads/{vendor.ReferenceNumber}/{fileName}";
                }
            }


            vendor.Status = "Changes Submitted";
            vendor.HasPendingEditRequest = false;
            vendor.UpdatedAt = DateTime.Now;

            // Update edit request status
            editRequest.Status = "Completed";
            editRequest.VendorResponseDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Your changes have been submitted for admin review.";
            return RedirectToAction("Status", "VendorLogin");
        }
    }
}