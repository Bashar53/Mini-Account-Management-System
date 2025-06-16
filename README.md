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

