using Fatec_Library.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Fatec_Library.Controllers
{
    public class ExemplarController : Controller
    {
        private readonly ContextMongodb _context;

        public ExemplarController()
        {
            _context = new ContextMongodb();
        }

        public async Task<IActionResult> Listar()
        {
            var exemplar = await _context.Exemplares.Find(p => true).ToListAsync();
            return View(exemplar);
        }
    }
}
