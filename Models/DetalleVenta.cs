using System.ComponentModel.DataAnnotations;

namespace BoticaMVC.Models
{
    public class DetalleVenta
    {
        public int Id { get; set; }

        public int VentaId { get; set; }
        public Venta Venta { get; set; } = null!;

        public int MedicamentoId { get; set; }
        public Medicamento Medicamento { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PrecioUnitario { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Importe { get; set; }
    }
}