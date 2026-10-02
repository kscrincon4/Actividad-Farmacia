using FarmaTrack.Fachada;

// Punto de entrada de FarmaTrack (consola). Esta interfaz SOLO habla con
// FarmaciaFacade: no instancia factories, adapters ni decoradores
// directamente (RNF-05). Toda la interacción es por consola (RNF-06).
//
// Uso:
//   dotnet run --project src            Menú interactivo
//   dotnet run --project src -- --demo  Recorrido automático de ejemplo

if (args.Contains("--demo"))
{
    Demo.Ejecutar();
    return;
}

var farmacia = new FarmaciaFacade();

while (true)
{
    Console.WriteLine("\n===== FarmaTrack =====");
    Console.WriteLine("1. Cargar inventario del proveedor");
    Console.WriteLine("2. Listar inventario");
    Console.WriteLine("3. Consultar disponibilidad y precio de un medicamento");
    Console.WriteLine("4. Vender un medicamento");
    Console.WriteLine("0. Salir");
    Console.Write("Elija una opción: ");
    string opcion = Console.ReadLine()?.Trim() ?? "";

    switch (opcion)
    {
        case "1":
            int cargados = farmacia.CargarInventarioProveedor();
            Console.WriteLine($"Inventario cargado: {cargados} medicamentos.");
            break;

        case "2":
            farmacia.ListarInventario();
            break;

        case "3":
            Console.Write("Código del medicamento: ");
            string codigo = Console.ReadLine()?.Trim() ?? "";
            var medicamento = farmacia.ConsultarDisponibilidad(codigo);
            if (medicamento is null)
            {
                Console.WriteLine($"No existe un medicamento con código '{codigo}'.");
            }
            else
            {
                bool afiliado = LeerSiNo("¿El cliente es afiliado/tercera edad?");
                bool promocion = LeerSiNo("¿Hay promoción vigente?");
                try
                {
                    decimal precio = farmacia.CalcularPrecio(codigo, afiliado, promocion);
                    Console.WriteLine($"{medicamento.Nombre} ({medicamento.DescripcionTipo()})");
                    Console.WriteLine($"  Stock disponible: {medicamento.Cantidad}");
                    Console.WriteLine($"  Requiere fórmula: {(medicamento.RequiereFormula() ? "Sí" : "No")}");
                    Console.WriteLine($"  Límite por venta: {medicamento.LimitePorVenta()}");
                    Console.WriteLine($"  Precio final: ${precio:N0}");
                }
                catch (ArgumentException error)
                {
                    Console.WriteLine($"Error: {error.Message}");
                }
            }
            break;

        case "4":
            Console.Write("Código del medicamento: ");
            string codVenta = Console.ReadLine()?.Trim() ?? "";
            if (!LeerCantidad(out int cantidad))
            {
                Console.WriteLine("Error: la cantidad debe ser un número entero mayor que cero.");
                break;
            }
            bool tieneFormula = LeerSiNo("¿Presenta fórmula médica?");
            var (exito, mensaje) = farmacia.Vender(codVenta, cantidad, tieneFormula);
            Console.WriteLine(mensaje);
            break;

        case "0":
            Console.WriteLine("¡Hasta luego!");
            return;

        default:
            Console.WriteLine("Opción inválida. Elija un número del menú.");
            break;
    }
}

// Lee una respuesta sí/no; devuelve true para 's', false para 'n'.
static bool LeerSiNo(string pregunta)
{
    while (true)
    {
        Console.Write(pregunta + " (s/n): ");
        string respuesta = Console.ReadLine()?.Trim().ToLower() ?? "";
        if (respuesta is "s" or "si" or "sí") return true;
        if (respuesta is "n" or "no") return false;
        Console.WriteLine("Opción inválida. Escriba 's' o 'n'.");
    }
}

// Lee una cantidad entera positiva con TryParse para no romper el programa.
static bool LeerCantidad(out int cantidad)
{
    Console.Write("Cantidad: ");
    string texto = Console.ReadLine()?.Trim() ?? "";
    return int.TryParse(texto, out cantidad) && cantidad > 0;
}

// Recorrido automático para la exposición: imprime qué patrón participa en cada paso.
static class Demo
{
    public static void Ejecutar()
    {
        var farmacia = new FarmaciaFacade();

        Console.WriteLine("\n[PASO 1] Cargar inventario del proveedor");
        Console.WriteLine("  Patrones: Adapter (traduce el formato del proveedor) + Factory Method (crea cada Medicamento)");
        int cargados = farmacia.CargarInventarioProveedor();
        Console.WriteLine($"  -> {cargados} medicamentos cargados.\n");

        Console.WriteLine("[PASO 2] Listar inventario");
        Console.WriteLine("  Patrón: Facade (la consola solo llama a FarmaciaFacade)");
        farmacia.ListarInventario();
        Console.WriteLine();

        Console.WriteLine("[PASO 3] Consultar precio de M004 (afiliado + promoción)");
        Console.WriteLine("  Patrón: Decorator (PrecioBase envuelto con ConIVA, ConDescuentoAfiliado y ConPromocion)");
        decimal precio = farmacia.CalcularPrecio("M004", afiliado: true, promocion: true);
        Console.WriteLine($"  -> Precio final de Amoxicilina 500mg: ${precio:N0}\n");

        Console.WriteLine("[PASO 4] Venta exitosa de un medicamento de venta libre (M001, 2 unidades)");
        Console.WriteLine("  Patrón: Facade coordina; las reglas del tipo las define el producto creado por Factory Method");
        var (ok1, msg1) = farmacia.Vender("M001", 2, tieneFormula: false);
        Console.WriteLine($"  -> {msg1}\n");

        Console.WriteLine("[PASO 5] Intento de venta de M004 sin fórmula médica (debe rechazarse)");
        var (ok2, msg2) = farmacia.Vender("M004", 1, tieneFormula: false);
        Console.WriteLine($"  -> {msg2}\n");

        Console.WriteLine("[PASO 6] Intento de venta de M006 (controlado) de 5 unidades: supera el límite por venta");
        var (ok3, msg3) = farmacia.Vender("M006", 5, tieneFormula: true);
        Console.WriteLine($"  -> {msg3}\n");

        Console.WriteLine("[PASO 7] Venta válida de M006 (2 unidades con fórmula)");
        var (ok4, msg4) = farmacia.Vender("M006", 2, tieneFormula: true);
        Console.WriteLine($"  -> {msg4}\n");

        Console.WriteLine("Fin del recorrido de demostración.");
    }
}
