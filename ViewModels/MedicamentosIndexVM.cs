using BoticaMVC.Models;
using System.Collections.Generic;

namespace BoticaMVC.ViewModels
{
    public class MedicamentosIndexVM
    {
        public IEnumerable<Medicamento> Items { get; set; } = new List<Medicamento>();
        public string? Q { get; set; }
        public string Orden { get; set; } = "codigo";
        public bool SoloActivos { get; set; } = true;
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public int TamPagina { get; set; } = 10;
    }
}