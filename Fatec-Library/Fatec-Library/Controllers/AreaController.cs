using Fatec_Library.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Fatec_Library.Controllers
{

    public class AreaController : Controller
    {
        private readonly ContextMongodb _context;

        public AreaController()
        {
            _context = new ContextMongodb();
        }

        public async Task<IActionResult> Listar()
        {
            var area = await _context.Areas.Find(p => true).ToListAsync();
            return View(area);
        }
    }

}
