using BoticaMVC.Data;
using BoticaMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoticaMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly BoticaDbContext _context;

        public HomeController(BoticaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Today;
            var limite30 = hoy.AddDays(30);

            var vm = new DashboardVM
            {
                TotalMedicamentos = await _context.Medicamentos.CountAsync(m => m.Activo),
                StockBajo = await _context.Medicamentos.CountAsync(m => m.Activo && m.Stock <= m.StockMinimo),
                Vencidos = await _context.Medicamentos.CountAsync(m => m.Activo && m.FechaVencimiento < hoy),
                PorVencer30Dias = await _context.Medicamentos.CountAsync(m =>
                    m.Activo && m.FechaVencimiento >= hoy && m.FechaVencimiento <= limite30)
            };

            return View(vm);
        }
    }
}
