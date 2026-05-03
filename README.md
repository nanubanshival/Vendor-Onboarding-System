# 🚀 Vendor Onboarding System

A full-stack **Vendor Onboarding System** built with ASP.NET Core MVC and SQL Server, simulating a real-world enterprise workflow with multi-level approvals, document management, and a vendor self-service portal.

---

## 🚀 Features

- 🔹 **8-step guided vendor registration**  
  Legal Info, PAN, GST, MSME, TDS, Bank, Address, Contact (with validation & conditional flows)

- 🔹 **Two invitation modes**  
  Email invite or shareable link (WhatsApp / Email / QR)

- 🔹 **Document uploads**  
  PAN, GST, Aadhar, Cancelled Cheque (PDF/JPG/PNG)

- 🔹 **Two-level approval workflow**  
  Admin → Super Admin → Final approval with auto-generated vendor codes

- 🔹 **Edit request system**  
  Admin can request specific corrections with comments

- 🔹 **Vendor self-service portal**  
  Track status, view submissions, and activity timeline

- 🔹 **Admin dashboard**  
  Status tiles, search, filters, vendor list

- 🔹 **Role-based access control**  
  Admin vs Super Admin

- 🔹 **Email notifications (SMTP)**  
  Invitations, credentials, edit updates

- 🔹 **Auto-generated reference IDs**

---

## 🛠️ Tech Stack

- **Backend:** ASP.NET Core MVC (.NET 8), C#  
- **ORM:** Entity Framework Core (Code First)  
- **Database:** SQL Server Express  
- **Frontend:** Razor Views, Bootstrap 5.3, jQuery  
- **Design:** Custom CSS Design System  
- **Email:** SMTP (Gmail)  
- **Authentication:** Session-based  
- **Storage:** Local (`wwwroot/uploads/`)  

---

## 📚 What I Learned

- Designing real-world workflows beyond CRUD  
- Implementing multi-step forms with validation  
- Managing approval systems with multiple states  
- Handling structured relational data in EF Core  
- Sending transactional emails from backend systems  
- Building scalable UI with reusable design systems  

---

## 📸 Screenshots

<img width="1903" height="876" alt="6" src="https://github.com/user-attachments/assets/090b37c5-cc11-4610-8038-a94abe4e577b" />
<img width="1900" height="876" alt="5" src="https://github.com/user-attachments/assets/8d2e8e2b-30dc-408f-9d29-99b55a98e66d" />
<img width="1891" height="875" alt="4" src="https://github.com/user-attachments/assets/64d6c237-1c7a-44ef-964f-7d0da3c9e7d0" />
<img width="1917" height="881" alt="3" src="https://github.com/user-attachments/assets/f0954bb4-052a-468a-a64c-606f7530aff2" />
<img width="1902" height="872" alt="2" src="https://github.com/user-attachments/assets/8717d420-c84e-48a4-8ea0-9392b8cd14b3" />
<img width="1898" height="876" alt="1" src="https://github.com/user-attachments/assets/bf41d95c-8ebd-44e1-a895-f6039a6fbe4f" />
<img width="1897" height="882" alt="66" src="https://github.com/user-attachments/assets/54f0bd8b-aee3-4b5a-a9f2-7047761a2d92" />
<img width="1897" height="877" alt="11" src="https://github.com/user-attachments/assets/e3692e3a-3522-4f44-8904-4bd9c21aebb3" />
<img width="1905" height="876" alt="10" src="https://github.com/user-attachments/assets/82389016-0a6f-4e5a-9e0a-d65d7e37f673" />
<img width="1902" height="878" alt="9" src="https://github.com/user-attachments/assets/5e4b247e-8d34-4cf6-8a8b-a6f5cdcd638b" />
<img width="1920" height="878" alt="8" src="https://github.com/user-attachments/assets/4b663bd5-3f90-4e1e-a985-66c7698f5eb6" />


---

## ⚙️ Setup Instructions

### 1. Clone the repository
```bash
git clone https://github.com/nanubanshival/Vendor-Onboarding-System.git
cd Vendor-Onboarding-System
