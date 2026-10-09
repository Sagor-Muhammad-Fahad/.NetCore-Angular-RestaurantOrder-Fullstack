# Restaurant Order Management System

A full-stack web application to manage restaurant orders, tables, menu items, and payment receipts.
Built with **ASP.NET Core Web API** on the backend and **Angular** on the frontend. 
Supports nested order items, receipt image uploads, and real-time order listing.

---

## 📌 Features

- ✅ Create, Read, Update, Delete Orders
- ✅ Add multiple Menu Items to a single Order
- ✅ Restaurant Table assignment for each order
- ✅ Upload payment receipt images (JPEG / PNG, max 2MB)
- ✅ Track Paid / Unpaid status
- ✅ Searchable dropdowns for Tables and Menu Items
- ✅ Responsive UI with Bootstrap 5
- ✅ Angular Signals for reactive state management
- ✅ Static file serving for receipt images

---

## 🧱 Tech Stack

### Backend
| Technology | Purpose |
|---|---|
| ASP.NET Core 8 Web API | REST API |
| Entity Framework Core | ORM |
| SQL Server | Database |
| Newtonsoft.Json | JSON serialization (reference loop handling) |
| Swagger / OpenAPI | API documentation |

### Frontend
| Technology | Purpose |
|---|---|
| Angular 17+ (standalone) | SPA framework |
| Angular Signals | State management |
| Reactive Forms | Nested form handling |
| RxJS | HTTP / async operations |
| Bootstrap 5 | Styling |

---

## 📁 Project Structure

```
restaurant-app/
├── backend/                         # Core_API_AG
│   ├── Controllers/
│   │   └── OrdersController.cs
│   ├── Models/
│   │   ├── RestaurantDbContext.cs
│   │   ├── Order.cs
│   │   ├── OrderItem.cs
│   │   ├── MenuItem.cs
│   │   ├── RestaurantTable.cs
│   │   └── UploadResponse.cs
│   ├── wwwroot/images/              # uploaded receipts (gitignored)
│   ├── appsettings.json
│   ├── Core_API_AG.csproj
│   └── Program.cs
│
├── frontend/                        # Angular app
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/order-components/
│   │   │   ├── models/
│   │   │   ├── services/order-service.ts
│   │   │   ├── app.routes.ts
│   │   │   └── app.config.ts
│   │   ├── environments/
│   │   └── main.ts
│   ├── angular.json
│   └── package.json
│
├── .gitignore
└── README.md
