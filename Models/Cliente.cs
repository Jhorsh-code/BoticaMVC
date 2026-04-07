using System.ComponentModel.DataAnnotations;

namespace BoticaMVC.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        public string NombreCompleto { get; set; } = string.Empty;

        [StringLength(15)]
        public string TipoDocumento { get; set; } = "DNI"; // DNI o RUC

        [StringLength(20)]
        public string Documento { get; set; } = string.Empty;

        [StringLength(180)]
        public string? Direccion { get; set; }

        [StringLength(100)]
        public string? Correo { get; set; }

        public bool Activo { get; set; } = true;
    }
}