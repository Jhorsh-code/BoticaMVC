using BoticaMVC.Models;

namespace BoticaMVC.Data
{
    public static class SeedData
    {
        public static void Inicializar(BoticaDbContext context)
        {
            if (context.Medicamentos.Any()) return;

            context.Medicamentos.AddRange(
                new Medicamento { Codigo = "MED-0001", Nombre = "Paracetamol 500 mg", Laboratorio = "Genfar", Presentacion = "Tabletas", Lote = "LOTE-A12", FechaVencimiento = new DateTime(2026, 11, 15), Ubicacion = "Estante A-2", Stock = 120, StockMinimo = 20, PrecioCompra = 0.20m, PrecioVenta = 0.50m, RequiereReceta = false, Activo = true },
                new Medicamento { Codigo = "MED-0002", Nombre = "Ibuprofeno 400 mg", Laboratorio = "Bayer", Presentacion = "Tabletas", Lote = "LOTE-B07", FechaVencimiento = new DateTime(2026, 10, 10), Ubicacion = "Estante A-3", Stock = 60, StockMinimo = 15, PrecioCompra = 0.35m, PrecioVenta = 0.80m, RequiereReceta = false, Activo = true },
                new Medicamento { Codigo = "MED-0003", Nombre = "Amoxicilina 500 mg", Laboratorio = "Sandoz", Presentacion = "Cápsulas", Lote = "LOTE-C21", FechaVencimiento = new DateTime(2026, 9, 5), Ubicacion = "Estante B-1", Stock = 8, StockMinimo = 20, PrecioCompra = 0.60m, PrecioVenta = 1.20m, RequiereReceta = true, Activo = true },
                new Medicamento { Codigo = "MED-0004", Nombre = "Loratadina 10 mg", Laboratorio = "Teva", Presentacion = "Tabletas", Lote = "LOTE-D03", FechaVencimiento = new DateTime(2027, 8, 20), Ubicacion = "Estante A-1", Stock = 45, StockMinimo = 10, PrecioCompra = 0.25m, PrecioVenta = 0.60m, RequiereReceta = false, Activo = true },
                new Medicamento { Codigo = "MED-0005", Nombre = "Omeprazol 20 mg", Laboratorio = "Abbott", Presentacion = "Cápsulas", Lote = "LOTE-E18", FechaVencimiento = new DateTime(2027, 3, 12), Ubicacion = "Estante B-2", Stock = 30, StockMinimo = 10, PrecioCompra = 0.40m, PrecioVenta = 0.90m, RequiereReceta = false, Activo = true },
                new Medicamento { Codigo = "MED-0008", Nombre = "Diclofenaco 50 mg", Laboratorio = "MK", Presentacion = "Tabletas", Lote = "LOTE-H11", FechaVencimiento = new DateTime(2024, 2, 10), Ubicacion = "Estante A-4", Stock = 18, StockMinimo = 10, PrecioCompra = 0.30m, PrecioVenta = 0.70m, RequiereReceta = false, Activo = true },
                new Medicamento { Codigo = "MED-0010", Nombre = "Metformina 850 mg", Laboratorio = "Merck", Presentacion = "Tabletas", Lote = "LOTE-J15", FechaVencimiento = new DateTime(2027, 9, 18), Ubicacion = "Estante B-3", Stock = 22, StockMinimo = 15, PrecioCompra = 0.45m, PrecioVenta = 1.00m, RequiereReceta = true, Activo = true }
            );

            context.SaveChanges();
        }
    }
}
