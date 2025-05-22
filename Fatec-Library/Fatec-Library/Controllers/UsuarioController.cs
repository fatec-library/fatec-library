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

        public async Task<IActionResult> Listar()
        {
            var usuarios = await _context.Usuarios.Find(p => true).ToListAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> Criar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Usuario usuario)
        {

            if (ModelState.IsValid)
            {

                await _context.Usuarios.InsertOneAsync(usuario);

                ViewBag.Message = "Usuário Cadastrado com sucesso";

            }

            return View();
        }
    }
}
 