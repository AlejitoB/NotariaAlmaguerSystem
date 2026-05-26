using System.Data;
using Dapper;
using NotariaAlmaguer.Web.Models;

namespace NotariaAlmaguer.Web.Services;

// ─── Modelos ──────────────────────────────────────────────────────────────────
// (definidos en Models/Models.cs)

// ─── Cliente Service ──────────────────────────────────────────────────────────
public class ClienteService(IDbConnection db)
{
    public async Task<List<Cliente>> GetAllAsync() =>
        (await db.QueryAsync<Cliente>(
            "SELECT id_cliente AS IdCliente, cedula, nombre, telefono, direccion, fecha_registro AS FechaRegistro FROM clientes ORDER BY nombre"))
        .ToList();

    public async Task<int> CreateAsync(Cliente c) =>
        await db.ExecuteScalarAsync<int>(
            "INSERT INTO clientes (cedula, nombre, telefono, direccion) VALUES (@Cedula,@Nombre,@Telefono,@Direccion); SELECT last_insert_rowid();", c);

    public async Task UpdateAsync(Cliente c) =>
        await db.ExecuteAsync(
            "UPDATE clientes SET cedula=@Cedula, nombre=@Nombre, telefono=@Telefono, direccion=@Direccion WHERE id_cliente=@IdCliente", c);

    public async Task DeleteAsync(int id) =>
        await db.ExecuteAsync("DELETE FROM clientes WHERE id_cliente=@id", new { id });
}

// ─── Notario Service ──────────────────────────────────────────────────────────
public class NotarioService(IDbConnection db)
{
    public async Task<List<Notario>> GetAllAsync() =>
        (await db.QueryAsync<Notario>(
            "SELECT id_notario AS IdNotario, nombre, numero_licencia AS NumeroLicencia, telefono, email, activo FROM notarios ORDER BY nombre"))
        .ToList();

    public async Task<int> CreateAsync(Notario n) =>
        await db.ExecuteScalarAsync<int>(
            "INSERT INTO notarios (nombre, numero_licencia, telefono, email, activo) VALUES (@Nombre,@NumeroLicencia,@Telefono,@Email,@Activo); SELECT last_insert_rowid();", n);

    public async Task UpdateAsync(Notario n) =>
        await db.ExecuteAsync(
            "UPDATE notarios SET nombre=@Nombre, numero_licencia=@NumeroLicencia, telefono=@Telefono, email=@Email, activo=@Activo WHERE id_notario=@IdNotario", n);

    public async Task DeleteAsync(int id) =>
        await db.ExecuteAsync("DELETE FROM notarios WHERE id_notario=@id", new { id });
}

// ─── Cita Service ─────────────────────────────────────────────────────────────
public class CitaService(IDbConnection db)
{
    private const string SelectJoin = @"
        SELECT c.id_cita AS IdCita, c.id_cliente AS IdCliente, c.id_notario AS IdNotario,
               c.fecha_cita AS FechaCita, c.estado AS Estado, c.descripcion AS Descripcion,
               cl.nombre AS NombreCliente, n.nombre AS NombreNotario
        FROM citas c
        JOIN clientes cl ON cl.id_cliente = c.id_cliente
        JOIN notarios n  ON n.id_notario  = c.id_notario";

    public async Task<List<Cita>> GetAllAsync() =>
        (await db.QueryAsync<Cita>($"{SelectJoin} ORDER BY c.fecha_cita DESC")).ToList();

    public async Task<int> CreateAsync(Cita c) =>
        await db.ExecuteScalarAsync<int>(
            "INSERT INTO citas (id_cliente,id_notario,fecha_cita,estado,descripcion) VALUES (@IdCliente,@IdNotario,@FechaCita,@Estado,@Descripcion); SELECT last_insert_rowid();", c);

    public async Task UpdateAsync(Cita c) =>
        await db.ExecuteAsync(
            "UPDATE citas SET id_cliente=@IdCliente, id_notario=@IdNotario, fecha_cita=@FechaCita, estado=@Estado, descripcion=@Descripcion WHERE id_cita=@IdCita", c);

    public async Task DeleteAsync(int id) =>
        await db.ExecuteAsync("DELETE FROM citas WHERE id_cita=@id", new { id });
}

// ─── Documento Service ────────────────────────────────────────────────────────
public class DocumentoService(IDbConnection db)
{
    private const string SelectJoin = @"
        SELECT d.id_documento AS IdDocumento, d.id_cliente AS IdCliente, d.id_notario AS IdNotario,
               d.numero_documento AS NumeroDocumento, d.tipo_documento AS TipoDocumento,
               d.descripcion AS Descripcion, d.fecha_documento AS FechaDocumento,
               cl.nombre AS NombreCliente, n.nombre AS NombreNotario
        FROM documentos d
        JOIN clientes cl ON cl.id_cliente = d.id_cliente
        JOIN notarios n  ON n.id_notario  = d.id_notario";

    public async Task<List<Documento>> GetAllAsync() =>
        (await db.QueryAsync<Documento>($"{SelectJoin} ORDER BY d.fecha_documento DESC")).ToList();

    public async Task<List<Documento>> BuscarPorCodigoAsync(string codigo) =>
        (await db.QueryAsync<Documento>(
            $"{SelectJoin} WHERE d.numero_documento LIKE @codigo ORDER BY d.fecha_documento DESC",
            new { codigo = $"%{codigo}%" })).ToList();

    public async Task<byte[]?> GetPdfAsync(int id) =>
        await db.ExecuteScalarAsync<byte[]?>(
            "SELECT archivo_pdf FROM documentos WHERE id_documento=@id", new { id });

    public async Task<int> CreateAsync(Documento d, byte[]? pdf = null) =>
        await db.ExecuteScalarAsync<int>(
            "INSERT INTO documentos (id_cliente,id_notario,numero_documento,tipo_documento,descripcion,archivo_pdf) VALUES (@IdCliente,@IdNotario,@NumeroDocumento,@TipoDocumento,@Descripcion,@Pdf); SELECT last_insert_rowid();",
            new { d.IdCliente, d.IdNotario, d.NumeroDocumento, d.TipoDocumento, d.Descripcion, Pdf = pdf });

    public async Task UpdateAsync(Documento d) =>
        await db.ExecuteAsync(
            "UPDATE documentos SET id_cliente=@IdCliente, id_notario=@IdNotario, numero_documento=@NumeroDocumento, tipo_documento=@TipoDocumento, descripcion=@Descripcion WHERE id_documento=@IdDocumento", d);

    public async Task DeleteAsync(int id) =>
        await db.ExecuteAsync("DELETE FROM documentos WHERE id_documento=@id", new { id });
}
