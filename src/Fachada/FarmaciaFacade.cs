using FarmaTrack.Adaptadores;
using FarmaTrack.Modelos;
using FarmaTrack.Precios;

namespace FarmaTrack.Fachada;

/// <summary>
/// Fachada del patrón Facade: expone una interfaz simple y única y coordina
/// internamente el Adapter, el Factory Method y el Decorator. La consola
/// solo habla con esta clase (RNF-05).
/// </summary>
public class FarmaciaFacade
{
    private readonly ProveedorAdapter _adapter = new(new ProveedorExterno());
    private readonly Dictionary<string, Medicamento> _inventario = new();

    /// <summary>RF-01: carga el inventario del proveedor. Devuelve cuántos items cargó.</summary>
    public int CargarInventarioProveedor()
    {
        _inventario.Clear();
        foreach (var m in _adapter.ObtenerMedicamentos())
            _inventario[m.Codigo] = m;
        return _inventario.Count;
    }

    /// <summary>RF-02: muestra código, nombre, tipo, stock y precio base.</summary>
    public void ListarInventario()
    {
        if (_inventario.Count == 0)
        {
            Console.WriteLine("El inventario está vacío. Cargue primero el inventario del proveedor.");
            return;
        }
        Console.WriteLine($"{"Código",-8}{"Nombre",-25}{"Tipo",-14}{"Stock",-7}{"Precio base",12}");
        Console.WriteLine(new string('-', 66));
        foreach (var m in _inventario.Values)
            Console.WriteLine($"{m.Codigo,-8}{m.Nombre,-25}{m.DescripcionTipo(),-14}{m.Cantidad,-7}{m.PrecioBase,12:N0}");
    }

    /// <summary>RF-08: devuelve el medicamento si existe, o null si el código no existe.</summary>
    public Medicamento? ConsultarDisponibilidad(string codigo)
        => _inventario.TryGetValue(codigo, out var m) ? m : null;

    /// <summary>
    /// RF-06: calcula el precio final combinando decoradores. El IVA se aplica
    /// siempre; el descuento de afiliado y la promoción solo si aplican.
    /// </summary>
    public decimal CalcularPrecio(string codigo, bool afiliado = false, bool promocion = false)
    {
        var medicamento = ConsultarDisponibilidad(codigo)
            ?? throw new ArgumentException($"No existe un medicamento con código '{codigo}'.");

        Precio precio = new PrecioBase(medicamento.PrecioBase);
        precio = new ConIVA(precio); // el IVA siempre aplica
        if (afiliado) precio = new ConDescuentoAfiliado(precio);
        if (promocion) precio = new ConPromocion(precio);
        return precio.Calcular();
    }

    /// <summary>
    /// RF-03, RF-04, RF-05, RF-07: registra una venta con sus validaciones.
    /// Devuelve (Exito, Mensaje). Si la venta es exitosa, descuenta el stock.
    /// </summary>
    public (bool Exito, string Mensaje) Vender(string codigo, int cantidad, bool tieneFormula)
    {
        var medicamento = ConsultarDisponibilidad(codigo);
        if (medicamento is null)
            return (false, $"No existe un medicamento con código '{codigo}'.");

        // RF-03: fórmula obligatoria según las reglas del tipo de medicamento
        if (medicamento.RequiereFormula() && !tieneFormula)
            return (false, $"'{medicamento.Nombre}' requiere fórmula médica. Venta rechazada.");

        // RF-04: límite por venta del tipo de medicamento
        if (cantidad > medicamento.LimitePorVenta())
            return (false, $"Límite por venta excedido: '{medicamento.Nombre}' permite máximo {medicamento.LimitePorVenta()} unidades.");

        // RF-05: stock suficiente
        if (cantidad > medicamento.Cantidad)
            return (false, $"Stock insuficiente: solo hay {medicamento.Cantidad} unidades de '{medicamento.Nombre}'.");

        // RF-07: descontar stock
        medicamento.Cantidad -= cantidad;
        return (true, $"Venta exitosa: {cantidad} x '{medicamento.Nombre}'. Stock restante: {medicamento.Cantidad}.");
    }
}
