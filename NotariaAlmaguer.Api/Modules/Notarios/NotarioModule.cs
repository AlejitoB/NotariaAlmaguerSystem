using System.Data;
using Dapper;

namespace NotariaAlmaguer.Api.Modules.Notarios;

// ─── Modelo ───────────────────────────────────────────────────────────────────
public class Notario
{
    public int IdNotario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string NumeroLicencia { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public int Activo { get; set; } = 1;
}

// ─── Repositorio ──────────────────────────────────────────────────────────────
public interface INotarioRepository
{
    Task<IEnumerable<Notario>> GetAllAsync();
    Task<Notario?> GetByIdAsync(int id);
    Task<int> CreateAsync(Notario notario);
    Task<bool> UpdateAsync(Notario notario);
    Task<bool> DeleteAsync(int id);
}

public class NotarioRepository(IDbConnection db) : INotarioRepository
{
    public async Task<IEnumerable<Notario>> GetAllAsync() =>
        await db.QueryAsync<Notario>(
            "SELECT id_notario AS IdNotario, nombre, numero_licencia AS NumeroLicencia, telefono, email, activo FROM notarios ORDER BY nombre");

    public async Task<Notario?> GetByIdAsync(int id) =>
        await db.QueryFirstOrDefaultAsync<Notario>(
            "SELECT id_notario AS IdNotario, nombre, numero_licencia AS NumeroLicencia, telefono, email, activo FROM notarios WHERE id_notario = @id",
            new { id });

    public async Task<int> CreateAsync(Notario n) =>
        await db.ExecuteScalarAsync<int>(
            "INSERT INTO notarios (nombre, numero_licencia, telefono, email, activo) VALUES (@Nombre, @NumeroLicencia, @Telefono, @Email, @Activo); SELECT last_insert_rowid();", n);

    public async Task<bool> UpdateAsync(Notario n) =>
        await db.ExecuteAsync(
            "UPDATE notarios SET nombre=@Nombre, numero_licencia=@NumeroLicencia, telefono=@Telefono, email=@Email, activo=@Activo WHERE id_notario=@IdNotario", n) > 0;

    public async Task<bool> DeleteAsync(int id) =>
        await db.ExecuteAsync("DELETE FROM notarios WHERE id_notario = @id", new { id }) > 0;
}

// ─── Endpoints ────────────────────────────────────────────────────────────────
public static class NotarioEndpoints
{
    public static void MapNotarioEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/notarios");

        group.MapGet("/", async (INotarioRepository repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, INotarioRepository repo) =>
            await repo.GetByIdAsync(id) is { } n ? Results.Ok(n) : Results.NotFound());

        group.MapPost("/", async (Notario notario, INotarioRepository repo) =>
        {
            var id = await repo.CreateAsync(notario);
            return Results.Created($"/api/notarios/{id}", notario);
        });

        group.MapPut("/{id:int}", async (int id, Notario notario, INotarioRepository repo) =>
        {
            notario.IdNotario = id;
            return await repo.UpdateAsync(notario) ? Results.NoContent() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, INotarioRepository repo) =>
            await repo.DeleteAsync(id) ? Results.NoContent() : Results.NotFound());
    }
}
