using Fatec_Library.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Fatec_Library.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ContextMongodb _context;

        public UsuarioController()
        {
            _context = new ContextMongodb();
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios.Find(p => true).ToListAsync();
            return View(usuarios);
        }
    }
}
