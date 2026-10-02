# FarmaTrack

Programa de consola en **C# (.NET 10)** para una única farmacia en Colombia. Permite consultar el inventario de medicamentos, ver su disponibilidad y precio final, registrar ventas y recibir inventario desde un proveedor externo (simulado en memoria, sin internet).

## Tabla de contenidos

1. [Problema](#problema)
2. [Funcionalidades](#funcionalidades)
3. [Patrones de diseño usados](#patrones-de-diseño-usados)
4. [¿Por qué cada patrón?](#por-qué-cada-patrón)
5. [Cómo ejecutar](#cómo-ejecutar)
6. [Ejemplo de salida (modo demo)](#ejemplo-de-salida-modo-demo)
7. [Estructura del proyecto](#estructura-del-proyecto)
8. [Documentación adicional](#documentación-adicional)

## Problema

La farmacia maneja varios tipos de medicamentos (venta libre, con fórmula médica y controlados), cada uno con reglas distintas (si exige fórmula, límite de unidades por venta). Crear los objetos con `new` y `if/else` dispersos acopla el código y dificulta agregar tipos. Además, el proveedor entrega el inventario en un formato incompatible con el interno, el precio final depende de ajustes combinables (IVA, descuentos, promociones) y el farmaceuta no debería conocer todas las clases internas para operar.

## Funcionalidades

1. Cargar inventario del proveedor (equivalente al paso en el que llegan los medicamentos).
2. Listar inventario: código, nombre, tipo, stock y precio base.
3. Consultar disponibilidad y precio final de un medicamento (con opciones de afiliado y promoción).
4. Vender un medicamento, con validaciones de fórmula médica, límite por venta y stock.
5. Modo `--demo`: recorrido automático que indica qué patrón participa en cada paso.

**Reglas de negocio de ejemplo:**

| Tipo | Requiere fórmula | Límite por venta |
|---|---|---|
| Venta libre | No | 20 |
| Con fórmula | Sí | 10 |
| Controlado | Sí | 2 |

**Ajustes de precio (constantes de ejemplo en `AjustesPrecio`):** IVA 19% (siempre), descuento de afiliado/tercera edad 10%, promoción 5%.

## Patrones de diseño usados

| Patrón | Tipo | Dónde | Qué resuelve |
|---|---|---|---|
| **Factory Method** | Creacional | `src/Fabricas/MedicamentoFactory.cs` (`MedicamentoFactory`, `VentaLibreFactory`, `ConFormulaFactory`, `ControladoFactory`) | Crea medicamentos de cada tipo sin `new` directo ni `if/else` dispersos. |
| **Adapter** | Estructural | `src/Adaptadores/Proveedor.cs` (`FuenteInventario`, `ProveedorExterno`, `ProveedorAdapter`) | Traduce el formato del proveedor (`cod`, `nom`, `cant`, `valor`, `tipo_prod`) al formato interno. |
| **Decorator** | Estructural | `src/Precios/Precio.cs` (`Precio`, `PrecioBase`, `ConIVA`, `ConDescuentoAfiliado`, `ConPromocion`) | Combina ajustes de precio sin crear una subclase por cada combinación. |
| **Facade** | Estructural | `src/Fachada/FarmaciaFacade.cs` (`FarmaciaFacade`) | Expone una interfaz simple; la consola solo habla con ella. |

### ¿Por qué cada patrón?

- **Factory Method (creacional):** centraliza la creación de medicamentos en factories concretas que deciden qué tipo instanciar según `tipo_prod`. Así el cliente no usa `new` directo ni `if/else` por tipo, y agregar un tipo nuevo (ej. `Homeopatico`) solo requiere crear su clase y su factory, sin tocar el resto del código.
- **Adapter (estructural):** el proveedor entrega su inventario con claves propias (`cod`, `nom`, `cant`, `valor`, `tipo_prod`), incompatibles con nuestros objetos `Medicamento`. El adapter traduce ese formato al interno sin modificar la clase del proveedor ni el resto del sistema.
- **Decorator (estructural):** el precio final es una combinación de ajustes (IVA siempre, descuento de afiliado y promoción opcionales). En vez de crear una subclase por cada combinación, cada decorador envuelve a otro `Precio` y aplica su ajuste; se combinan libremente en cadena.
- **Facade (estructural):** coordinar factories, adapter y decoradores sería complejo para el menú de consola. La facade expone solo 5 métodos simples (`CargarInventarioProveedor`, `ListarInventario`, `ConsultarDisponibilidad`, `CalcularPrecio`, `Vender`) y `Program.cs` no conoce ninguna de las clases internas.

## Cómo ejecutar

**Requisitos:** .NET SDK 10.0 o superior (`dotnet --version`). Sin dependencias NuGet externas.

```bash
cd farmatrack

# Modo demostración (ideal para la exposición)
dotnet run --project src -- --demo

# Menú interactivo
dotnet run --project src
```

También puedes abrir `FarmaciaTrack.slnx` en Visual Studio y ejecutar con `F5` (para el modo demo, agrega `--demo` en *Propiedades → Depurar → Argumentos de la aplicación*).

## Ejemplo de salida (modo demo)

```
[PASO 1] Cargar inventario del proveedor
  Patrones: Adapter (traduce el formato del proveedor) + Factory Method (crea cada Medicamento)
  -> 7 medicamentos cargados.

[PASO 2] Listar inventario
  Patrón: Facade (la consola solo llama a FarmaciaFacade)
Código  Nombre                   Tipo          Stock   Precio base
------------------------------------------------------------------
M001    Acetaminofén 500mg       Venta libre   50            2.500
M004    Amoxicilina 500mg        Con fórmula   20            8.900
M006    Clonazepam 2mg           Controlado    10           12.000
...

[PASO 3] Consultar precio de M004 (afiliado + promoción)
  Patrón: Decorator (PrecioBase envuelto con ConIVA, ConDescuentoAfiliado y ConPromocion)
  -> Precio final de Amoxicilina 500mg: $9.055

[PASO 5] Intento de venta de M004 sin fórmula médica (debe rechazarse)
  -> 'Amoxicilina 500mg' requiere fórmula médica. Venta rechazada.
```

## Estructura del proyecto

```
farmatrack/
├── openspec/
│   └── specification.md        Especificación SDD del taller
├── src/
│   ├── Program.cs              Menú de consola y modo --demo
│   ├── Modelos/
│   │   └── Medicamento.cs      Producto y subclases (Factory Method)
│   ├── Fabricas/
│   │   └── MedicamentoFactory.cs
│   ├── Adaptadores/
│   │   └── Proveedor.cs        FuenteInventario, ProveedorExterno, ProveedorAdapter
│   ├── Precios/
│   │   └── Precio.cs           Precio, PrecioBase y decoradores
│   ├── Fachada/
│   │   └── FarmaciaFacade.cs
│   └── FarmaciaTrack.csproj
├── FarmaciaTrack.slnx
├── README.md
└── .gitignore
```

## Documentación adicional

- Especificación completa (requisitos, patrones, diseño y criterios de aceptación): [`openspec/specification.md`](openspec/specification.md)
