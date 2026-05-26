# 🏛️ Sistema de Gestión Notaría Almaguer

**Autores:** Robert Alejandro Beru Patiño · Manuel Ricardo Bolaños Moya · Milton Andres Campo  
**Director:** Cristian Mendez Rodriguez  
**Institución:** Fundación Universitaria de Popayán

---

## 📋 Descripción

Sistema web para la digitalización y gestión de documentos notariales de la Notaría de Almaguer, Cauca. Permite administrar clientes, notarios, citas y documentos a través de una interfaz web conectada a una API REST.

---

## 🏗️ Arquitectura

```
NotariaAlmaguer/
├── NotariaAlmaguer.Api/          ← Backend: Minimal API + Dapper + SQLite
│   ├── Program.cs
│   ├── init.sql
│   └── Modules/
│       ├── Clientes/ClienteModule.cs
│       ├── Notarios/NotarioModule.cs
│       ├── Citas/CitaModule.cs
│       └── Documentos/DocumentoModule.cs
└── NotariaAlmaguer.Web/          ← Frontend: Blazor WebAssembly
    ├── Program.cs
    ├── Models/Models.cs
    ├── Services/ApiServices.cs
    └── Pages/
        ├── Clientes.razor
        ├── Notarios.razor
        ├── Citas.razor
        └── Documentos.razor
```

**Stack tecnológico:**
- Backend: C# .NET 9 · Minimal APIs · Dapper · SQLite · Arquitectura Limpia · Patrón Repositorio
- Frontend: Blazor WebAssembly · Bootstrap 5

---

## 🚀 Cómo ejecutar

### Requisitos
- .NET 9 SDK instalado → https://dotnet.microsoft.com/download

### 1. Ejecutar el Backend (API)

```bash
cd NotariaAlmaguer.Api
dotnet run
```
La API queda disponible en: `http://localhost:5000`

### 2. Ejecutar el Frontend (Blazor)

En otra terminal:
```bash
cd NotariaAlmaguer.Web
dotnet run
```
La app web queda en: `http://localhost:5001`

---

## 📡 Endpoints de la API

### Clientes
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET    | /api/clientes | Listar todos |
| GET    | /api/clientes/{id} | Obtener por ID |
| POST   | /api/clientes | Crear |
| PUT    | /api/clientes/{id} | Actualizar |
| DELETE | /api/clientes/{id} | Eliminar |

### Notarios
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET    | /api/notarios | Listar todos |
| GET    | /api/notarios/{id} | Obtener por ID |
| POST   | /api/notarios | Crear |
| PUT    | /api/notarios/{id} | Actualizar |
| DELETE | /api/notarios/{id} | Eliminar |

### Citas
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET    | /api/citas | Listar todas |
| GET    | /api/citas/{id} | Obtener por ID |
| POST   | /api/citas | Crear |
| PUT    | /api/citas/{id} | Actualizar |
| DELETE | /api/citas/{id} | Eliminar |

### Documentos
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET    | /api/documentos | Listar todos |
| GET    | /api/documentos/{id} | Obtener por ID |
| POST   | /api/documentos | Crear |
| PUT    | /api/documentos/{id} | Actualizar |
| DELETE | /api/documentos/{id} | Eliminar |

---

## 💡 Notas

- La base de datos `notaria.db` se crea automáticamente al iniciar el backend.
- Si el frontend está en un puerto distinto al `5001`, actualizar la `BaseAddress` en `NotariaAlmaguer.Web/Program.cs`.
