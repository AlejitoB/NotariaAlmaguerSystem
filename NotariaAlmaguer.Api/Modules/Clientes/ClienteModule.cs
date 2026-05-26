using System.Data;
using Dapper;

namespace NotariaAlmaguer.Api.Modules.Clientes;

// ─── Modelo ───────────────────────────────────────────────────────────────────
public class Cliente
{
    public int IdCliente { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? FechaRegistro { get; set; }
}

// ─── Repositorio ──────────────────────────────────────────────────────────────
public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task<Cliente?> GetByIdAsync(int id);
    Task<int> CreateAsync(Cliente cliente);
    Task<bool> UpdateAsync(Cliente cliente);
    Task<bool> DeleteAsync(int id);
}

public class ClienteRepository(IDbConnection db) : IClienteRepository
{
    public async Task<IEnumerable<Cliente>> GetAllAsync() =>
        await db.QueryAsync<Cliente>(
            "SELECT id_cliente AS IdCliente, cedula, nombre, telefono, direccion, fecha_registro AS FechaRegistro FROM clientes ORDER BY nombre");

    public async Task<Cliente?> GetByIdAsync(int id) =>
        await db.QueryFirstOrDefaultAsync<Cliente>(
            "SELECT id_cliente AS IdCliente, cedula, nombre, telefono, direccion, fecha_registro AS FechaRegistro FROM clientes WHERE id_cliente = @id",
            new { id });

    public async Task<int> CreateAsync(Cliente c) =>
        await db.ExecuteScalarAsync<int>(
            "INSERT INTO clientes (cedula, nombre, telefono, direccion) VALUES (@Cedula, @Nombre, @Telefono, @Direccion); SELECT last_insert_rowid();", c);

    public async Task<bool> UpdateAsync(Cliente c) =>
        await db.ExecuteAsync(
            "UPDATE clientes SET cedula=@Cedula, nombre=@Nombre, telefono=@Telefono, direccion=@Direccion WHERE id_cliente=@IdCliente", c) > 0;

    public async Task<bool> DeleteAsync(int id) =>
        await db.ExecuteAsync("DELETE FROM clientes WHERE id_cliente = @id", new { id }) > 0;
}

// ─── Endpoints ────────────────────────────────────────────────────────────────
public static class ClienteEndpoints
{
    public static void MapClienteEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/clientes");

        group.MapGet("/", async (IClienteRepository repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, IClienteRepository repo) =>
            await repo.GetByIdAsync(id) is { } c ? Results.Ok(c) : Results.NotFound());

        group.MapPost("/", async (Cliente cliente, IClienteRepository repo) =>
        {
            var id = await repo.CreateAsync(cliente);
            return Results.Created($"/api/clientes/{id}", cliente);
        });

        group.MapPut("/{id:int}", async (int id, Cliente cliente, IClienteRepository repo) =>
        {
            cliente.IdCliente = id;
            return await repo.UpdateAsync(cliente) ? Results.NoContent() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, IClienteRepository repo) =>
            await repo.DeleteAsync(id) ? Results.NoContent() : Results.NotFound());
    }
}
