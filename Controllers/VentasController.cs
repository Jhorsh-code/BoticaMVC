using BoticaMVC.Data;
using BoticaMVC.Models;
using BoticaMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BoticaMVC.Controllers
{
    public class VentasController : Controller
    {
        private readonly BoticaDbContext _context;
        private const decimal IGV_RATE = 0.18m;

        public VentasController(BoticaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new VentaPOSVM
            {
                MedicamentosDisponibles = await _context.Medicamentos
                    .AsNoTracking()
                    .Where(m => m.Activo && m.Stock > 0)
                    .OrderBy(m => m.Nombre)
                    .Select(m => new MedicamentoPOSVM
                    {
                        Id = m.Id,
                        Codigo = m.Codigo,
                        Nombre = m.Nombre,
                        Laboratorio = m.Laboratorio,
                        PrecioVenta = m.PrecioVenta,
                        Stock = m.Stock
                    })
                    .ToListAsync()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarVenta(VentaPOSVM vm)
        {
            vm.MedicamentosDisponibles = await _context.Medicamentos
                .AsNoTracking()
                .Where(m => m.Activo && m.Stock > 0)
                .OrderBy(m => m.Nombre)
                .Select(m => new MedicamentoPOSVM
                {
                    Id = m.Id,
                    Codigo = m.Codigo,
                    Nombre = m.Nombre,
                    Laboratorio = m.Laboratorio,
                    PrecioVenta = m.PrecioVenta,
                    Stock = m.Stock
                })
                .ToListAsync();

            List<ItemVentaPOSTVM>? items;

            try
            {
                items = JsonSerializer.Deserialize<List<ItemVentaPOSTVM>>(
                vm.ItemsJson,
                new JsonSerializerOptions
                {
                PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                ModelState.AddModelError("", "Los datos de la venta no tienen un formato válido.");
                return View("Index", vm);
            }

            if (items == null || !items.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un medicamento a la venta.");
                return View("Index", vm);
            }

            foreach (var item in items)
            {
                if (item.Cantidad <= 0)
                {
                    ModelState.AddModelError("", $"La cantidad del medicamento {item.Nombre} debe ser mayor a cero.");
                    return View("Index", vm);
                }
            }

            var medicamentosIds = items.Select(x => x.MedicamentoId).Distinct().ToList();

            var medicamentosDb = await _context.Medicamentos
                .Where(m => medicamentosIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id);

            foreach (var item in items)
            {
                if (!medicamentosDb.ContainsKey(item.MedicamentoId))
                {
                    ModelState.AddModelError("", $"El medicamento {item.Nombre} ya no existe.");
                    return View("Index", vm);
                }

                var med = medicamentosDb[item.MedicamentoId];

                if (!med.Activo)
                {
                    ModelState.AddModelError("", $"El medicamento {med.Nombre} está inactivo.");
                    return View("Index", vm);
                }

                if (item.Cantidad > med.Stock)
                {
                    ModelState.AddModelError("", $"Stock insuficiente para {med.Nombre}. Disponible: {med.Stock}.");
                    return View("Index", vm);
                }
            }

            Cliente? cliente = null;

            if (!string.IsNullOrWhiteSpace(vm.ClienteNombre))
            {
                cliente = new Cliente
                {
                    NombreCompleto = vm.ClienteNombre.Trim(),
                    Documento = vm.ClienteDocumento?.Trim() ?? "",
                    TipoDocumento = string.IsNullOrWhiteSpace(vm.ClienteTipoDocumento) ? "DNI" : vm.ClienteTipoDocumento,
                    Direccion = vm.ClienteDireccion?.Trim()
                };

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
            }

            decimal subtotal = 0m;
            var detalles = new List<DetalleVenta>();

            foreach (var item in items)
            {
                var med = medicamentosDb[item.MedicamentoId];
                var importe = med.PrecioVenta * item.Cantidad;
                subtotal += importe;

                detalles.Add(new DetalleVenta
                {
                    MedicamentoId = med.Id,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = med.PrecioVenta,
                    Importe = importe
                });

                med.Stock -= item.Cantidad;
            }

            decimal igv = Math.Round(subtotal * IGV_RATE, 2);
            decimal total = subtotal + igv;

            var venta = new Venta
            {
                Fecha = DateTime.Now,
                TipoComprobante = vm.TipoComprobante,
                Serie = vm.TipoComprobante == "Factura" ? "F001" : "B001",
                NumeroComprobante = await GenerarNumeroComprobante(vm.TipoComprobante),
                ClienteId = cliente?.Id,
                SubTotal = subtotal,
                Igv = igv,
                Total = total,
                Detalles = detalles
            };

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            TempData["ok"] = "Venta registrada correctamente.";
            return RedirectToAction(nameof(Comprobante), new { id = venta.Id });
        }

        public async Task<IActionResult> Comprobante(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Medicamento)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
                return NotFound();

            return View(venta);
        }

        public async Task<IActionResult> Historial()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Cliente)
                .AsNoTracking()
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();

            return View(ventas);
        }

        private async Task<string> GenerarNumeroComprobante(string tipo)
        {
            string serie = tipo == "Factura" ? "F001" : "B001";

            int ultimo = await _context.Ventas
                .Where(v => v.Serie == serie)
                .OrderByDescending(v => v.Id)
                .Select(v => v.Id)
                .FirstOrDefaultAsync();

            return (ultimo + 1).ToString("D6");
        }
    }
}