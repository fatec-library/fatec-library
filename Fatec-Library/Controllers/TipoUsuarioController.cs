using Fatec_Library.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;


namespace Fatec_Library.Controllers
{
    public class TipoUsuarioController : Controller
    {
        private readonly ContextMongodb _context;

        public TipoUsuarioController()
        {
            _context = new ContextMongodb();
        }

        public async Task<IActionResult> Listar()
        {
            var tiposUsuario = await _context.TiposUsuarios.Find(p => true).ToListAsync();
            return View(tiposUsuario);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TipoUsuario tipoUsuario)
        {
            if (ModelState.IsValid)
            {
                await _context.TiposUsuarios.InsertOneAsync(tipoUsuario);
                return RedirectToAction(nameof(Listar));
            }

            return View(tipoUsuario);
        }
    }
}
