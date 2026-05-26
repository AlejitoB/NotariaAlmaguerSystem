using System.Data;
using Dapper;

namespace NotariaAlmaguer.Api.Modules.Citas;

// ─── Modelo ───────────────────────────────────────────────────────────────────
public class Cita
{
    public int IdCita { get; set; }
    public int IdCliente { get; set; }
    public int IdNotario { get; set; }
    public string FechaCita { get; set; } = string.Empty;
    public string Estado { get; set; } = "pendiente";
    public string? Descripcion { get; set; }
    public string? NombreCliente { get; set; }
    public string? NombreNotario { get; set; }
}

// ─── Repositorio ──────────────────────────────────────────────────────────────
public interface ICitaRepository
{
    Task<IEnumerable<Cita>> GetAllAsync();
    Task<Cita?> GetByIdAsync(int id);
    Task<int> CreateAsync(Cita cita);
    Task<bool> UpdateAsync(Cita cita);
    Task<bool> DeleteAsync(int id);
}

public class CitaRepository(IDbConnection db) : ICitaRepository
{
    private const string SelectJoin = @"
        SELECT c.id_cita AS IdCita, c.id_cliente AS IdCliente, c.id_notario AS IdNotario,
               c.fecha_cita AS FechaCita, c.estado AS Estado, c.descripcion AS Descripcion,
               cl.nombre AS NombreCliente, n.nombre AS NombreNotario
        FROM citas c
        JOIN clientes cl ON cl.id_cliente = c.id_cliente
        JOIN notarios n  ON n.id_notario  = c.id_notario";

    public async Task<IEnumerable<Cita>> GetAllAsync() =>
        await db.QueryAsync<Cita>($"{SelectJoin} ORDER BY c.fecha_cita DESC");

    public async Task<Cita?> GetByIdAsync(int id) =>
        await db.QueryFirstOrDefaultAsync<Cita>($"{SelectJoin} WHERE c.id_cita = @id", new { id });

    public async Task<int> CreateAsync(Cita c) =>
        await db.ExecuteScalarAsync<int>(
            "INSERT INTO citas (id_cliente, id_notario, fecha_cita, estado, descripcion) VALUES (@IdCliente, @IdNotario, @FechaCita, @Estado, @Descripcion); SELECT last_insert_rowid();", c);

    public async Task<bool> UpdateAsync(Cita c) =>
        await db.ExecuteAsync(
            "UPDATE citas SET id_cliente=@IdCliente, id_notario=@IdNotario, fecha_cita=@FechaCita, estado=@Estado, descripcion=@Descripcion WHERE id_cita=@IdCita", c) > 0;

    public async Task<bool> DeleteAsync(int id) =>
        await db.ExecuteAsync("DELETE FROM citas WHERE id_cita = @id", new { id }) > 0;
}

// ─── Endpoints ────────────────────────────────────────────────────────────────
public static class CitaEndpoints
{
    public static void MapCitaEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/citas");

        group.MapGet("/", async (ICitaRepository repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, ICitaRepository repo) =>
            await repo.GetByIdAsync(id) is { } c ? Results.Ok(c) : Results.NotFound());

        group.MapPost("/", async (Cita cita, ICitaRepository repo) =>
        {
            var id = await repo.CreateAsync(cita);
            return Results.Created($"/api/citas/{id}", cita);
        });

        group.MapPut("/{id:int}", async (int id, Cita cita, ICitaRepository repo) =>
        {
            cita.IdCita = id;
            return await repo.UpdateAsync(cita) ? Results.NoContent() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, ICitaRepository repo) =>
            await repo.DeleteAsync(id) ? Results.NoContent() : Results.NotFound());
    }
}
