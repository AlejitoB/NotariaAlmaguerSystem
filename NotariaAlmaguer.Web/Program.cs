using Microsoft.Data.Sqlite;
using System.Data;
using Dapper;
using NotariaAlmaguer.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ─── SQLite directo (Blazor Server no necesita HTTP) ─────────────────────────
var dbPath = Path.Combine(AppContext.BaseDirectory, "notaria.db");
builder.Services.AddScoped<IDbConnection>(_ =>
{
    var conn = new SqliteConnection($"Data Source={dbPath}");
    conn.Open();
    conn.Execute("PRAGMA foreign_keys = ON;");
    return conn;
});

// ─── Servicios de datos ───────────────────────────────────────────────────────
builder.Services.AddSingleton<Session>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<NotarioService>();
builder.Services.AddScoped<CitaService>();
builder.Services.AddScoped<DocumentoService>();

var app = builder.Build();

// ─── Inicializar BD ───────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IDbConnection>();
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
            archivo_pdf BLOB,
            FOREIGN KEY (id_cliente) REFERENCES clientes(id_cliente) ON DELETE CASCADE,
            FOREIGN KEY (id_notario) REFERENCES notarios(id_notario) ON DELETE CASCADE);
    ");
    // Datos de prueba
    try {
        db.Execute("INSERT OR IGNORE INTO notarios (nombre, numero_licencia, telefono, email) VALUES ('Dr. Carlos Muñoz','NOT-001','3001234567','cmunoz@notaria.com')");
        db.Execute("INSERT OR IGNORE INTO clientes (cedula, nombre, telefono, direccion) VALUES ('12345678','Ana María López','3109876543','Calle 5 #3-20, Almaguer')");
    } catch { }
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<NotariaAlmaguer.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
