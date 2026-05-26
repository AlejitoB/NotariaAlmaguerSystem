using Microsoft.Data.Sqlite;
using System.Data;
using Dapper;
using NotariaAlmaguer.Api.Modules.Clientes;
using NotariaAlmaguer.Api.Modules.Notarios;
using NotariaAlmaguer.Api.Modules.Citas;
using NotariaAlmaguer.Api.Modules.Documentos;

var builder = WebApplication.CreateBuilder(args);

// ─── CORS ─────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ─── Conexión SQLite ─────────────────────────────────────────────────────────
var dbPath = Path.Combine(AppContext.BaseDirectory, "notaria.db");
builder.Services.AddScoped<IDbConnection>(_ =>
{
    var conn = new SqliteConnection($"Data Source={dbPath}");
    conn.Open();
    conn.Execute("PRAGMA foreign_keys = ON;");
    return conn;
});

// ─── Repositorios ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<INotarioRepository, NotarioRepository>();
builder.Services.AddScoped<ICitaRepository, CitaRepository>();
builder.Services.AddScoped<IDocumentoRepository, DocumentoRepository>();

// Puerto fijo para evitar conflictos
builder.WebHost.UseUrls("http://localhost:5050");

var app = builder.Build();

// ─── Inicializar base de datos ────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IDbConnection>();

    // Buscar init.sql en varias ubicaciones posibles
    var posiblesPaths = new[]
    {
        Path.Combine(AppContext.BaseDirectory, "init.sql"),
        Path.Combine(Directory.GetCurrentDirectory(), "init.sql"),
    };

    var sqlPath = posiblesPaths.FirstOrDefault(File.Exists);

    if (sqlPath != null)
    {
        var sql = await File.ReadAllTextAsync(sqlPath);
        // Ejecutar sentencia por sentencia para evitar errores
        foreach (var stmt in sql.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var clean = stmt.Trim();
            if (!string.IsNullOrWhiteSpace(clean))
            {
                try { db.Execute(clean); } catch { /* ignorar errores de datos duplicados */ }
            }
        }
    }
    else
    {
        // Crear tablas mínimas si no hay init.sql
        db.Execute(@"
            CREATE TABLE IF NOT EXISTS clientes (
                id_cliente INTEGER PRIMARY KEY AUTOINCREMENT,
                cedula TEXT NOT NULL UNIQUE, nombre TEXT NOT NULL,
                telefono TEXT, direccion TEXT, fecha_registro DATE DEFAULT CURRENT_DATE);
            CREATE TABLE IF NOT EXISTS notarios (
                id_notario INTEGER PRIMARY KEY AUTOINCREMENT,
                nombre TEXT NOT NULL, numero_licencia TEXT NOT NULL UNIQUE,
                telefono TEXT, email TEXT, activo INTEGER NOT NULL DEFAULT 1);
            CREATE TABLE IF NOT EXISTS citas (
                id_cita INTEGER PRIMARY KEY AUTOINCREMENT,
                id_cliente INTEGER NOT NULL, id_notario INTEGER NOT NULL,
                fecha_cita DATETIME NOT NULL, estado TEXT NOT NULL DEFAULT 'pendiente',
                descripcion TEXT,
                FOREIGN KEY (id_cliente) REFERENCES clientes(id_cliente) ON DELETE CASCADE,
                FOREIGN KEY (id_notario) REFERENCES notarios(id_notario) ON DELETE CASCADE);
            CREATE TABLE IF NOT EXISTS documentos (
                id_documento INTEGER PRIMARY KEY AUTOINCREMENT,
                id_cliente INTEGER NOT NULL, id_notario INTEGER NOT NULL,
                numero_documento TEXT NOT NULL, tipo_documento TEXT NOT NULL,
                descripcion TEXT, fecha_documento DATE DEFAULT CURRENT_DATE,
                FOREIGN KEY (id_cliente) REFERENCES clientes(id_cliente) ON DELETE CASCADE,
                FOREIGN KEY (id_notario) REFERENCES notarios(id_notario) ON DELETE CASCADE);
        ");
    }
}

app.UseCors();

// ─── Endpoints ───────────────────────────────────────────────────────────────
app.MapClienteEndpoints();
app.MapNotarioEndpoints();
app.MapCitaEndpoints();
app.MapDocumentoEndpoints();

// Endpoint raíz para confirmar que la API está viva
app.MapGet("/", () => Results.Ok(new { mensaje = "API Notaría Almaguer funcionando", version = "1.0" }));

app.Run();
