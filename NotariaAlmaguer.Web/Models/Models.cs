namespace NotariaAlmaguer.Web.Models;

public class Cliente
{
    public int IdCliente { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? FechaRegistro { get; set; }
}

public class Notario
{
    public int IdNotario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string NumeroLicencia { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public int Activo { get; set; } = 1;
}

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
