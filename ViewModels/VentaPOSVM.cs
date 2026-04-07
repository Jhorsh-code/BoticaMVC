namespace BoticaMVC.ViewModels
{
    public class VentaPOSVM
    {
        public string? Buscar { get; set; }
        public string TipoComprobante { get; set; } = "Boleta";

        public string? ClienteNombre { get; set; }
        public string? ClienteDocumento { get; set; }
        public string? ClienteTipoDocumento { get; set; } = "DNI";
        public string? ClienteDireccion { get; set; }

        public List<MedicamentoPOSVM> MedicamentosDisponibles { get; set; } = new();

        public string ItemsJson { get; set; } = "[]";

        public decimal SubTotal { get; set; }
        public decimal Igv { get; set; }
        public decimal Total { get; set; }
    }

    public class MedicamentoPOSVM
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Laboratorio { get; set; } = string.Empty;
        public decimal PrecioVenta { get; set; }
        public int Stock { get; set; }
    }

    public class ItemVentaPOSTVM
    {
        public int MedicamentoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
    }
}