using System.ComponentModel.DataAnnotations;

namespace BoticaMVC.Models
{
    public class Medicamento
    {
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string Codigo { get; set; } = string.Empty;   // MED-0001

        [Required, StringLength(80)]
        public string Nombre { get; set; } = string.Empty;

        [Required, StringLength(60)]
        public string Laboratorio { get; set; } = string.Empty;

        [Required, StringLength(30)]
        public string Presentacion { get; set; } = "Tabletas";

        [Required, StringLength(20)]
        public string Lote { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        [Range(0, 999999)]
        public int Stock { get; set; }

        [Range(0, 999999)]
        public int StockMinimo { get; set; } = 10;

        [Range(0, 999999)]
        public decimal PrecioCompra { get; set; }

        [Range(0, 999999)]
        public decimal PrecioVenta { get; set; }

        public bool RequiereReceta { get; set; } = false;

        [StringLength(30)]
        public string Ubicacion { get; set; } = "Almacén";

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public DateTime? FechaActualizacion { get; set; }
    }
}
