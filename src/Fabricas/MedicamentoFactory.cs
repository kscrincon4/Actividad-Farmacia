using FarmaTrack.Modelos;

namespace FarmaTrack.Fabricas;

/// <summary>
/// Creador abstracto del patrón Factory Method.
/// Declara el método fábrica CrearMedicamento; cada subclase decide qué
/// producto concreto instanciar, sin que el cliente use 'new' ni if/else.
/// </summary>
public abstract class MedicamentoFactory
{
    public abstract Medicamento CrearMedicamento(string codigo, string nombre, decimal precioBase, int cantidad);
}

/// <summary>Creador concreto: instancia medicamentos de venta libre.</summary>
public class VentaLibreFactory : MedicamentoFactory
{
    public override Medicamento CrearMedicamento(string codigo, string nombre, decimal precioBase, int cantidad)
        => new VentaLibre(codigo, nombre, precioBase, cantidad);
}

/// <summary>Creador concreto: instancia medicamentos con fórmula médica.</summary>
public class ConFormulaFactory : MedicamentoFactory
{
    public override Medicamento CrearMedicamento(string codigo, string nombre, decimal precioBase, int cantidad)
        => new ConFormula(codigo, nombre, precioBase, cantidad);
}

/// <summary>Creador concreto: instancia medicamentos controlados.</summary>
public class ControladoFactory : MedicamentoFactory
{
    public override Medicamento CrearMedicamento(string codigo, string nombre, decimal precioBase, int cantidad)
        => new Controlado(codigo, nombre, precioBase, cantidad);
}
