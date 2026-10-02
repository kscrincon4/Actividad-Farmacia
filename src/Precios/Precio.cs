namespace FarmaTrack.Precios;

/// <summary>
/// Componente del patrón Decorator: interfaz común de todo cálculo de precio.
/// </summary>
public interface Precio
{
    decimal Calcular();
}

/// <summary>Componente concreto: precio sin ningún ajuste.</summary>
public class PrecioBase : Precio
{
    private readonly decimal _precioBase;

    public PrecioBase(decimal precioBase) => _precioBase = precioBase;

    public decimal Calcular() => _precioBase;
}

/// <summary>Decorador concreto: agrega el IVA al precio envuelto.</summary>
public class ConIVA : Precio
{
    private readonly Precio _precio;

    public ConIVA(Precio precio) => _precio = precio;

    public decimal Calcular() => _precio.Calcular() * (1 + AjustesPrecio.Iva);
}

/// <summary>Decorador concreto: aplica el descuento de afiliado/tercera edad.</summary>
public class ConDescuentoAfiliado : Precio
{
    private readonly Precio _precio;

    public ConDescuentoAfiliado(Precio precio) => _precio = precio;

    public decimal Calcular() => _precio.Calcular() * (1 - AjustesPrecio.DescuentoAfiliado);
}

/// <summary>Decorador concreto: aplica el descuento por promoción.</summary>
public class ConPromocion : Precio
{
    private readonly Precio _precio;

    public ConPromocion(Precio precio) => _precio = precio;

    public decimal Calcular() => _precio.Calcular() * (1 - AjustesPrecio.DescuentoPromocion);
}

/// <summary>
/// Constantes configurables de los ajustes. Son valores de EJEMPLO:
/// en Colombia el IVA general es 19%, el descuento de afiliado 10%
/// y la promoción 5%.
/// </summary>
public static class AjustesPrecio
{
    public const decimal Iva = 0.19m;
    public const decimal DescuentoAfiliado = 0.10m;
    public const decimal DescuentoPromocion = 0.05m;
}
