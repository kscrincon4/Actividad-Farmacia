using FarmaTrack.Fabricas;
using FarmaTrack.Modelos;

namespace FarmaTrack.Adaptadores;

/// <summary>
/// Target del patrón Adapter: interfaz que el sistema espera para obtener
/// los medicamentos ya en el formato interno.
/// </summary>
public interface FuenteInventario
{
    List<Medicamento> ObtenerMedicamentos();
}

/// <summary>
/// Adaptee del patrón Adapter. Simula al proveedor externo con datos fijos
/// en su formato propio (diccionarios con claves cod, nom, cant, valor,
/// tipo_prod). NO hay conexión real a internet.
/// Valores posibles de tipo_prod: "libre", "formula", "controlado".
/// </summary>
public class ProveedorExterno
{
    public List<Dictionary<string, object>> Entregar()
    {
        return new List<Dictionary<string, object>>
        {
            new() { ["cod"] = "M001", ["nom"] = "Acetaminofén 500mg", ["cant"] = 50, ["valor"] = 2500m, ["tipo_prod"] = "libre" },
            new() { ["cod"] = "M002", ["nom"] = "Ibuprofeno 400mg", ["cant"] = 30, ["valor"] = 3200m, ["tipo_prod"] = "libre" },
            new() { ["cod"] = "M003", ["nom"] = "Omeprazol 20mg", ["cant"] = 25, ["valor"] = 4500m, ["tipo_prod"] = "libre" },
            new() { ["cod"] = "M004", ["nom"] = "Amoxicilina 500mg", ["cant"] = 20, ["valor"] = 8900m, ["tipo_prod"] = "formula" },
            new() { ["cod"] = "M005", ["nom"] = "Losartán 50mg", ["cant"] = 40, ["valor"] = 5600m, ["tipo_prod"] = "formula" },
            new() { ["cod"] = "M006", ["nom"] = "Clonazepam 2mg", ["cant"] = 10, ["valor"] = 12000m, ["tipo_prod"] = "controlado" },
            new() { ["cod"] = "M007", ["nom"] = "Tramadol 50mg", ["cant"] = 8, ["valor"] = 9800m, ["tipo_prod"] = "controlado" },
        };
    }
}

/// <summary>
/// Adapter del patrón Adapter: traduce el formato del proveedor al formato
/// interno. Para crear cada Medicamento usa el Factory Method (según tipo_prod
/// elige la factory concreta). Así el proveedor se integra sin modificar su
/// clase ni el resto del sistema.
/// </summary>
public class ProveedorAdapter : FuenteInventario
{
    private readonly ProveedorExterno _proveedor;

    // Mapa tipo_prod -> factory concreta (aplicación del Factory Method)
    private static readonly Dictionary<string, MedicamentoFactory> Factories = new()
    {
        ["libre"] = new VentaLibreFactory(),
        ["formula"] = new ConFormulaFactory(),
        ["controlado"] = new ControladoFactory(),
    };

    public ProveedorAdapter(ProveedorExterno proveedor)
    {
        _proveedor = proveedor;
    }

    public List<Medicamento> ObtenerMedicamentos()
    {
        var medicamentos = new List<Medicamento>();
        foreach (var item in _proveedor.Entregar())
        {
            string tipo = item["tipo_prod"].ToString()!;
            if (!Factories.TryGetValue(tipo, out var factory))
            {
                // Tipo desconocido: se omite con aviso en lugar de romper el programa
                Console.WriteLine($"Aviso: tipo de producto desconocido '{tipo}', se omite {item["cod"]}.");
                continue;
            }
            medicamentos.Add(factory.CrearMedicamento(
                item["cod"].ToString()!,
                item["nom"].ToString()!,
                Convert.ToDecimal(item["valor"]),
                Convert.ToInt32(item["cant"])));
        }
        return medicamentos;
    }
}
