using BoticaMVC.Data;
using BoticaMVC.Models;
using BoticaMVC.ViewModels;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using BoticaMVC.ViewModels;

namespace BoticaMVC.Controllers
{
    public class MedicamentosController : Controller
    {
        private readonly BoticaDbContext _context;

        public MedicamentosController(BoticaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q, string orden = "codigo", bool soloActivos = true, int pagina = 1)
        {
            int tamPagina = 10;

            IQueryable<Medicamento> query = _context.Medicamentos;

            if (soloActivos)
                query = query.Where(m => m.Activo);

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(m =>
                    m.Nombre.Contains(q) ||
                    m.Codigo.Contains(q) ||
                    m.Lote.Contains(q) ||
                    m.Laboratorio.Contains(q));
            }

            query = orden switch
            {
                "codigo" => query.OrderBy(m => m.Codigo),
                "stock" => query.OrderBy(m => m.Stock),
                "vence" => query.OrderBy(m => m.FechaVencimiento),
                _ => query.OrderBy(m => m.Nombre)
            };

            int totalRegistros = await query.CountAsync();
            int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamPagina);

            if (totalPaginas == 0)
                totalPaginas = 1;

            if (pagina < 1)
                pagina = 1;

            if (pagina > totalPaginas)
                pagina = totalPaginas;

            var lista = await query
                .Skip((pagina - 1) * tamPagina)
                .Take(tamPagina)
                .AsNoTracking()
                .ToListAsync();

            var vm = new MedicamentosIndexVM
            {
                Items = lista,
                Q = q ?? "",
                Orden = orden,
                SoloActivos = soloActivos,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = totalRegistros,
                TamPagina = tamPagina
            };

            ViewBag.Q = vm.Q;
            ViewBag.Orden = vm.Orden;
            ViewBag.SoloActivos = vm.SoloActivos;
            ViewBag.PaginaActual = vm.PaginaActual;
            ViewBag.TotalPaginas = vm.TotalPaginas;

            return View(vm);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var medicamento = await _context.Medicamentos
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicamento == null) return NotFound();

            return View(medicamento);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medicamento medicamento)
        {
            ValidacionesDeNegocio(medicamento);

            bool codigoExiste = await _context.Medicamentos.AnyAsync(m => m.Codigo == medicamento.Codigo);
            if (codigoExiste)
                ModelState.AddModelError(nameof(Medicamento.Codigo), "Ya existe un medicamento con ese código.");

            if (!ModelState.IsValid)
                return View(medicamento);

            _context.Add(medicamento);
            await _context.SaveChangesAsync();

            TempData["ok"] = "Medicamento registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var medicamento = await _context.Medicamentos.FindAsync(id);
            if (medicamento == null) return NotFound();

            return View(medicamento);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Medicamento medicamento)
        {
            if (id != medicamento.Id) return NotFound();

            ValidacionesDeNegocio(medicamento);

 
            bool codigoExiste = await _context.Medicamentos.AnyAsync(m => m.Codigo == medicamento.Codigo && m.Id != medicamento.Id);
            if (codigoExiste)
                ModelState.AddModelError(nameof(Medicamento.Codigo), "Ya existe otro medicamento con ese código.");

            if (!ModelState.IsValid)
                return View(medicamento);

            try
            {
                medicamento.FechaActualizacion = DateTime.Now;
                _context.Update(medicamento);
                await _context.SaveChangesAsync();

                TempData["ok"] = "Medicamento actualizado correctamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicamentoExists(medicamento.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var medicamento = await _context.Medicamentos
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicamento == null) return NotFound();

            return View(medicamento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicamento = await _context.Medicamentos.FindAsync(id);
            if (medicamento == null) return NotFound();

            medicamento.Activo = false;
            medicamento.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["ok"] = "Medicamento desactivado (borrado lógico).";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reporte()
        {
            var lista = await _context.Medicamentos
                .AsNoTracking()
                .Where(m => m.Activo)
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            return View(lista);
        }
        public async Task<IActionResult> ReporteExcel()
        {
            var lista = await _context.Medicamentos
                .AsNoTracking()
                .Where(m => m.Activo)
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Medicamentos");

            ws.Cell(1, 1).Value = "Código";
            ws.Cell(1, 2).Value = "Nombre";
            ws.Cell(1, 3).Value = "Laboratorio";
            ws.Cell(1, 4).Value = "Presentación";
            ws.Cell(1, 5).Value = "Lote";
            ws.Cell(1, 6).Value = "Vence";
            ws.Cell(1, 7).Value = "Stock";
            ws.Cell(1, 8).Value = "Stock Mín";
            ws.Cell(1, 9).Value = "Compra";
            ws.Cell(1, 10).Value = "Venta";
            ws.Cell(1, 11).Value = "Receta";
            ws.Cell(1, 12).Value = "Ubicación";
            ws.Cell(1, 13).Value = "Activo";

            int fila = 2;
            foreach (var m in lista)
            {
                ws.Cell(fila, 1).Value = m.Codigo;
                ws.Cell(fila, 2).Value = m.Nombre;
                ws.Cell(fila, 3).Value = m.Laboratorio;
                ws.Cell(fila, 4).Value = m.Presentacion;
                ws.Cell(fila, 5).Value = m.Lote;
                ws.Cell(fila, 6).Value = m.FechaVencimiento.ToString("dd/MM/yyyy");
                ws.Cell(fila, 7).Value = m.Stock;
                ws.Cell(fila, 8).Value = m.StockMinimo;
                ws.Cell(fila, 9).Value = m.PrecioCompra;
                ws.Cell(fila, 10).Value = m.PrecioVenta;
                ws.Cell(fila, 11).Value = m.RequiereReceta ? "Sí" : "No";
                ws.Cell(fila, 12).Value = m.Ubicacion;
                ws.Cell(fila, 13).Value = m.Activo ? "Sí" : "No";
                fila++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"ReporteMedicamentos_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }

        public async Task<IActionResult> ReportePdf()
        {
            var lista = await _context.Medicamentos
                .AsNoTracking()
                .Where(m => m.Activo)
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            var hoy = DateTime.Today;

            byte[] pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(25);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("BOTICA MVC - REPORTE DE MEDICAMENTOS").Bold().FontSize(16);
                        col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(10).FontColor(Colors.Grey.Darken1);
                        col.Item().LineHorizontal(1);
                    });

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(60);  // Código
                            columns.RelativeColumn(2);   // Nombre
                            columns.RelativeColumn(1);   // Lab
                            columns.ConstantColumn(60);  // Vence
                            columns.ConstantColumn(40);  // Stock
                            columns.ConstantColumn(45);  // Min
                            columns.ConstantColumn(55);  // Venta
                            columns.ConstantColumn(60);  // Estado
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Código").Bold();
                            header.Cell().Text("Nombre").Bold();
                            header.Cell().Text("Lab.").Bold();
                            header.Cell().Text("Vence").Bold();
                            header.Cell().AlignRight().Text("Stock").Bold();
                            header.Cell().AlignRight().Text("Min").Bold();
                            header.Cell().AlignRight().Text("Venta").Bold();
                            header.Cell().Text("Estado").Bold();

                            header.Cell().ColumnSpan(8).LineHorizontal(1);
                        });

                        foreach (var m in lista)
                        {
                            var vencido = m.FechaVencimiento < hoy;
                            var porVencer = !vencido && m.FechaVencimiento <= hoy.AddDays(30);
                            var stockBajo = m.Stock <= m.StockMinimo;

                            string estado = vencido ? "VENCIDO"
                                           : porVencer ? "POR VENCER"
                                           : stockBajo ? "STOCK BAJO"
                                           : "OK";

                            table.Cell().Text(m.Codigo);
                            table.Cell().Text(m.Nombre);
                            table.Cell().Text(m.Laboratorio);
                            table.Cell().Text(m.FechaVencimiento.ToString("dd/MM/yyyy"));
                            table.Cell().AlignRight().Text(m.Stock.ToString());
                            table.Cell().AlignRight().Text(m.StockMinimo.ToString());
                            table.Cell().AlignRight().Text(m.PrecioVenta.ToString("0.00"));
                            table.Cell().Text(estado);
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf();

            return File(pdf, "application/pdf",
                $"ReporteMedicamentos_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }

        private void ValidacionesDeNegocio(Medicamento m)
        {
            if (m.PrecioVenta < m.PrecioCompra)
                ModelState.AddModelError(nameof(Medicamento.PrecioVenta),
                    "El precio de venta no puede ser menor al precio de compra.");

            if (m.FechaVencimiento < DateTime.Today.AddYears(-1))
                ModelState.AddModelError(nameof(Medicamento.FechaVencimiento),
                    "La fecha de vencimiento no parece válida.");
        }

        private bool MedicamentoExists(int id)
        {
            return _context.Medicamentos.Any(e => e.Id == id);
        }

    }
}
