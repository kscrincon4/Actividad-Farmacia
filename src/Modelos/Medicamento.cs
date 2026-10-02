namespace FarmaTrack.Modelos;

/// <summary>
/// Producto base del patrón Factory Method.
/// Representa un medicamento genérico del inventario y define las reglas
/// (si exige fórmula y su límite por venta) que cada subclase ajusta.
/// </summary>
public class Medicamento
{
    public string Codigo { get; }
    public string Nombre { get; }
    public decimal PrecioBase { get; }
    public int Cantidad { get; set; }

    public Medicamento(string codigo, string nombre, decimal precioBase, int cantidad)
    {
        Codigo = codigo;
        Nombre = nombre;
        PrecioBase = precioBase;
        Cantidad = cantidad;
    }

    /// <summary>Indica si este tipo de medicamento exige fórmula médica para venderse.</summary>
    public virtual bool RequiereFormula() => false;

    /// <summary>Máximo de unidades que se pueden vender en una sola transacción.</summary>
    public virtual int LimitePorVenta() => 999; // valor alto por defecto: sin límite práctico

    public virtual string DescripcionTipo() => "Genérico";
}

/// <summary>Producto concreto: medicamento de venta libre (no exige fórmula).</summary>
public class VentaLibre : Medicamento
{
    public VentaLibre(string codigo, string nombre, decimal precioBase, int cantidad)
        : base(codigo, nombre, precioBase, cantidad) { }

    public override int LimitePorVenta() => 20; // ejemplo: hasta 20 unidades por venta

    public override string DescripcionTipo() => "Venta libre";
}

/// <summary>Producto concreto: medicamento que exige fórmula médica.</summary>
public class ConFormula : Medicamento
{
    public ConFormula(string codigo, string nombre, decimal precioBase, int cantidad)
        : base(codigo, nombre, precioBase, cantidad) { }

    public override bool RequiereFormula() => true;

    public override int LimitePorVenta() => 10; // ejemplo: máximo 10 unidades por venta

    public override string DescripcionTipo() => "Con fórmula";
}

/// <summary>Producto concreto: medicamento controlado (límite más estricto).</summary>
public class Controlado : Medicamento
{
    public Controlado(string codigo, string nombre, decimal precioBase, int cantidad)
        : base(codigo, nombre, precioBase, cantidad) { }

    public override bool RequiereFormula() => true;

    public override int LimitePorVenta() => 2; // ejemplo: máximo 2 unidades por venta

    public override string DescripcionTipo() => "Controlado";
}
