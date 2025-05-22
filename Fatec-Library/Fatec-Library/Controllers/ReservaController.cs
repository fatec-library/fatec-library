using Fatec_Library.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Fatec_Library.Controllers
{
    public class ReservaController : Controller
    {

        private readonly ContextMongodb _context;

        public ReservaController()
        {
            _context = new ContextMongodb();
        }

        public async Task<IActionResult> Listar()
        {
            var reserva = await _context.Reservas.Find(p => true).ToListAsync();
            return View(reserva);
        }
    }
}
