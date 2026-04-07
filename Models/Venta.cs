using System.ComponentModel.DataAnnotations;

namespace BoticaMVC.Models
{
    public class Venta
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string TipoComprobante { get; set; } = "Boleta";

        [Required]
        [StringLength(10)]
        public string Serie { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string NumeroComprobante { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal SubTotal { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Igv { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Total { get; set; }

        public int? ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public List<DetalleVenta> Detalles { get; set; } = new();
    }
}