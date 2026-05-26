NotaryAlmaguer System
Notarial Management Web System
---

##  Description

NotariaAlmaguer System is a web-based notary management platform developed to optimize administrative, documentary, and operational processes within a notary's office.

The system is built on a modular architecture using ASP.NET Core and C#, allowing for scalability, organization, and future integrations.

The project is currently in active development and construction.
---

## Architecture

```
NotaryAlmaguer/
├── NotariaAlmaguer.Api/ Backend: Minimal API + Dapper + SQLite
│ ├── Program.cs
│ ├── init.sql
│ └── Modules/
│ ├── Clients/ClientModule.cs
│ ├── Notaries/NotarioModule.cs
│ ├── Appointments/AppointmentModule.cs
│ └── Documents/DocumentModule.cs
└── NotariaAlmaguer.Web/ Frontend: Blazor WebAssembly 
├── Program.cs

├── Models/Models.cs

├── Services/ApiServices.cs

└── Pages/

├── Clients.razor

├── Notaries.razor

├── Appointments.razor

└── Documents.razor
---
## System Objectives

Digitize notarial processes
Optimize document management
Centralize information
Improve service times
Facilitate internal administration

**Technology Stack:**
- Backend: C# .NET 9 · Minimal APIs · Dapper · SQLite · Clean Architecture · Repository Pattern
- Frontend: Blazor WebAssembly · Bootstrap 5

---

## How Run

### Requirements
- .NET 9 SDK installed → https://dotnet.microsoft.com/download

### 1. Run the Backend (API)

```bash
cd NotariaAlmaguer.Api
dotnet run
```
The API is available at: `http://localhost:5000`

### 2. Run the Frontend (Blazor)

In another terminal:
```bash
cd NotariaAlmaguer.Web
dotnet run
```
The web app is located at: `http://localhost:5001`

---

## 📡 API Endpoints

### Clients
| Method | Path | Description |

|--------|------|-------------|

| GET | /api/clientes | List all |

| GET | /api/clientes/{id} | Get by ID |

| POST | /api/clientes | Create |

| PUT | /api/clientes/{id} | Update |

| DELETE | /api/clientes/{id} | Delete |

### Notaries
| Method | Path | Description |

|--------|------|-------------|

| GET | /api/notarios | List all |

| GET | /api/notarios/{id} | Get by ID |

| POST | /api/notarios | Create |

| PUT | /api/notarios/{id} | Update |

| DELETE | /api/notarios/{id} | Delete |

### Appointments
| Method | Path | Description |

|--------|------|-------------|

| GET | /api/citas | List all |

| GET | /api/citas/{id} | Get by ID |

| POST | /api/quotes | Create |

| PUT | /api/quotes/{id} | Update |

| DELETE | /api/quotes/{id} | Delete |

### Documents
| Method | Path | Description |

|--------|------|-------------|

| GET | /api/documents | List all |

| GET | /api/documents/{id} | Get by ID |

| POST | /api/documents | Create |

| PUT | /api/documents/{id} | Update |

| DELETE | /api/documents/{id} | Delete |

---

## Note

This project is currently under active development and may undergo frequent changes.
