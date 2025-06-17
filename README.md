# 🧾 Account Management System

A mini web-based accounting application built with ASP.NET Core Razor Pages. It supports user role management, a hierarchical chart of accounts, voucher entries and Excel report of vouchers.

---

## 🚀 Core Features

### 1. 👥 User Roles & Permissions
- Manage user roles: **Admin**, **Accountant**, **Viewer**
- Assign granular module-based permissions (Create, Read, Update, Delete)
- Uses stored procedures for permission assignment and verification
- Access control is enforced per-page using `PermissionService
<img width="951" alt="image" src="https://github.com/user-attachments/assets/677db1da-8c1b-494e-8963-e49a98cf0ac5" />
<img width="846" alt="image" src="https://github.com/user-attachments/assets/0fd9e617-3503-444c-8b4d-2c921585a9e5" />
<img width="838" alt="image" src="https://github.com/user-attachments/assets/34ff3b44-484c-42df-9484-b011d4611b0e" />
<img width="880" alt="image" src="https://github.com/user-attachments/assets/4137cbe9-6841-4094-86a7-e672b1adc9c3" />


 
### 2. 🧱 Chart of Accounts
- Fully dynamic account tree (Parent-Child relationships)
- Operations:
  - **Create**, **Edit**, **Delete** accounts
- Uses stored procedure:  
  `EXEC sp_ManageChartOfAccounts @Action, @Id, @Name, @ParentId`
- UI renders a **hierarchical tree view** using account levels (Lvl)
<img width="498" alt="image" src="https://github.com/user-attachments/assets/9e43c0a2-2fd0-4cc2-a1cc-7e0fa967fd4c" />
<img width="866" alt="image" src="https://github.com/user-attachments/assets/525f00ca-da90-4076-8eb5-6cebec6dbe9b" />
---

### 3. 📄 Voucher Entry Module
Supports entry and posting of the following financial transactions:

- **Journal Voucher**
- **Payment Voucher**
- **Receipt Voucher**

Each voucher form supports:
- 📅 Date
- 🔢 Reference Number
- ➕➖ Multiple Debit & Credit Lines
- 📘 Dropdown-based Account Selection

Data is saved via:  
`EXEC sp_SaveVoucher @VoucherType, @Date, @Reference, @Lines...`
<img width="862" alt="image" src="https://github.com/user-attachments/assets/a6476c4a-3647-4bf0-8b06-4ca34a3783a0" />

---

## 💼 Excel Report 
- Export vouchers report  to **Excel** format
  - Button-triggered export
  - Uses EPPlus
  - <img width="846" alt="image" src="https://github.com/user-attachments/assets/2c454f65-42d2-4e2d-b149-9d93cd58b384" />

  <img width="479" alt="image" src="https://github.com/user-attachments/assets/18ab2192-9f37-4558-96aa-bd325c6f0eed" />


---

## 🛠 Technologies Used
- ASP.NET Core Razor Pages
- Entity Framework Core
- Microsoft Identity
- Stored Procedures (SQL Server)
- Bootstrap 5 (UI)
- EPPlus (Excel Export)

---

## 🚀 Features

- 🧾 Chart of Accounts with Tree View
- 📥 Voucher Entry (Journal, Payment, Receipt)
- 📤 Voucher Export (HTML, Excel)
- 🔐 ASP.NET Core Identity integration
- 🎛️ Role-based and permission-based access control
- 🧩 Modular resource and permission management
- 📦 Database interaction via stored procedures

---

## 📂 Project Structure

```text
Mini_Account_Management_System/
│
├── .github/
│   └── workflows/
│
├── Areas/
│   └── Identity/
│       └── Pages/
│           ├── _ValidationScriptsPartial.cshtml
│           ├── _ViewImports.cshtml
│           ├── _ViewStart.cshtml
│           └── Account/
│               ├── Login.cshtml / Login.cshtml.cs
│               ├── Logout.cshtml / Logout.cshtml.cs
│               ├── Register.cshtml / Register.cshtml.cs
│               └── _ViewImports.cshtml
│
├── DbConnection/
│   └── ApplicationDbContext.cs
│
├── Migrations/
│   ├── 20250602165519_IdentityMigration.cs
│   ├── 20250602165519_IdentityMigration.Designer.cs
│   └── ApplicationDbContextModelSnapshot.cs
│
├── Models/
│   ├── AccountsChart.cs
│   ├── AppPermission.cs
│   ├── AppResource.cs
│   ├── VoucherEntry.cs
│   └── VoucherReportDto.cs
│
├── Pages/
│   ├── Index.cshtml / Index.cshtml.cs
│   ├── Privacy.cshtml / Privacy.cshtml.cs
│   ├── Error.cshtml / Error.cshtml.cs
│   ├── _ViewImports.cshtml
│   ├── _ViewStart.cshtml
│   │
│   ├── AccountsChart/
│   │   ├── Index.cshtml / Index.cshtml.cs
│   │   ├── Upsert.cshtml / Upsert.cshtml.cs
│   │   └── _AccountTree.cshtml / _AccountTree.cshtml.cs
│   │
│   ├── Admin/
│   │   ├── AssignPermissions.cshtml / .cs
│   │   ├── CreateRole.cshtml / .cs
│   │   ├── DeleteRole.cshtml / .cs
│   │   ├── EditRole.cshtml / .cs
│   │   └── RoleList.cshtml / .cs
│   │
│   ├── AppResources/
│   │   └── ResourcesList.cshtml / .cs
│   │
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   ├── _Layout.cshtml.css
│   │   ├── _LoginPartial.cshtml
│   │   ├── _Menu.cshtml / _Menu.cshtml.cs
│   │   └── _ValidationScriptsPartial.cshtml
│   │
│   └── Voucher/
│       ├── ExportVoucher.cshtml / .cs
│       └── VoucherEntry.cshtml / .cs
│
├── Properties/
│   └── launchSettings.json
│
├── Service/
│   ├── AccountsChartService.cs
│   ├── MenuService.c





