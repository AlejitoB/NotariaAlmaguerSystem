using System.Data;
using Dapper;

namespace NotariaAlmaguer.Api.Modules.Documentos;

// ─── Modelo ───────────────────────────────────────────────────────────────────
public class Documento
{
    public int IdDocumento { get; set; }
    public int IdCliente { get; set; }
    public int IdNotario { get; set; }
    public string NumeroDocumento { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? FechaDocumento { get; set; }
    public string? NombreCliente { get; set; }
    public string? NombreNotario { get; set; }
}

// ─── Repositorio ──────────────────────────────────────────────────────────────
public interface IDocumentoRepository
{
    Task<IEnumerable<Documento>> GetAllAsync();
    Task<Documento?> GetByIdAsync(int id);
    Task<int> CreateAsync(Documento doc);
    Task<bool> UpdateAsync(Documento doc);
    Task<bool> DeleteAsync(int id);
}

public class DocumentoRepository(IDbConnection db) : IDocumentoRepository
{
    private const string SelectJoin = @"
        SELECT d.id_documento AS IdDocumento, d.id_cliente AS IdCliente, d.id_notario AS IdNotario,
               d.numero_documento AS NumeroDocumento, d.tipo_documento AS TipoDocumento,
               d.descripcion AS Descripcion, d.fecha_documento AS FechaDocumento,
               cl.nombre AS NombreCliente, n.nombre AS NombreNotario
        FROM documentos d
        JOIN clientes cl ON cl.id_cliente = d.id_cliente
        JOIN notarios n  ON n.id_notario  = d.id_notario";

    public async Task<IEnumerable<Documento>> GetAllAsync() =>
        await db.QueryAsync<Documento>($"{SelectJoin} ORDER BY d.fecha_documento DESC");

    public async Task<Documento?> GetByIdAsync(int id) =>
        await db.QueryFirstOrDefaultAsync<Documento>($"{SelectJoin} WHERE d.id_documento = @id", new { id });

    public async Task<int> CreateAsync(Documento d) =>
        await db.ExecuteScalarAsync<int>(
            "INSERT INTO documentos (id_cliente, id_notario, numero_documento, tipo_documento, descripcion) VALUES (@IdCliente, @IdNotario, @NumeroDocumento, @TipoDocumento, @Descripcion); SELECT last_insert_rowid();", d);

    public async Task<bool> UpdateAsync(Documento d) =>
        await db.ExecuteAsync(
            "UPDATE documentos SET id_cliente=@IdCliente, id_notario=@IdNotario, numero_documento=@NumeroDocumento, tipo_documento=@TipoDocumento, descripcion=@Descripcion WHERE id_documento=@IdDocumento", d) > 0;

    public async Task<bool> DeleteAsync(int id) =>
        await db.ExecuteAsync("DELETE FROM documentos WHERE id_documento = @id", new { id }) > 0;
}

// ─── Endpoints ────────────────────────────────────────────────────────────────
public static class DocumentoEndpoints
{
    public static void MapDocumentoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/documentos");

        group.MapGet("/", async (IDocumentoRepository repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, IDocumentoRepository repo) =>
            await repo.GetByIdAsync(id) is { } d ? Results.Ok(d) : Results.NotFound());

        group.MapPost("/", async (Documento doc, IDocumentoRepository repo) =>
        {
            var id = await repo.CreateAsync(doc);
            return Results.Created($"/api/documentos/{id}", doc);
        });

        group.MapPut("/{id:int}", async (int id, Documento doc, IDocumentoRepository repo) =>
        {
            doc.IdDocumento = id;
            return await repo.UpdateAsync(doc) ? Results.NoContent() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, IDocumentoRepository repo) =>
            await repo.DeleteAsync(id) ? Results.NoContent() : Results.NotFound());
    }
}
