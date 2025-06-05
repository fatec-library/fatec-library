using Fatec_Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;

namespace Fatec_Library.Controllers
{
    [Authorize]
    public class LivroController : Controller
    {

        private readonly ContextMongodb _context;

        public LivroController()
        {
            _context = new ContextMongodb();
        }

        // 1. Listar acervo - GET
        public async Task<IActionResult> Index()
        {
            var livro = await _context.Livros.Find(p => true).ToListAsync();
            return View(livro);
        }

        // 2. Criar livro - GET
        public async Task<IActionResult> Create()
        {
            var areas = await _context.Areas.Find(a => true).ToListAsync();
            ViewBag.Areas = new SelectList(areas, "Id", "Descritivo");

            return View();
        }

        // 2. Criar livro - POST
        [HttpPost]
        public async Task<IActionResult> Create(Livro livro)
        {
            if (ModelState.IsValid)
            {
                await _context.Livros.InsertOneAsync(livro);
                return RedirectToAction(nameof(Index));
            }

            var areas = await _context.Areas.Find(a => true).ToListAsync();
            ViewBag.Areas = new SelectList(areas, "Id", "Nome");

            return View(livro);
        }

        // 3. Editar livro - GET
        public async Task<IActionResult> Edit(string id)
        {
            var livro = await _context.Livros.Find(l => l.Id == id).FirstOrDefaultAsync();
            if (livro == null)
                return NotFound();

            return View(livro);
        }

        // 3. Editar livro - POST
        [HttpPost]
        public async Task<IActionResult> Edit(string id, Livro livroEditado)
        {
            if (ModelState.IsValid)
            {
                await _context.Livros.ReplaceOneAsync(l => l.Id == id, livroEditado);
                return RedirectToAction(nameof(Index));
            }
            return View(livroEditado);
        }

        // 4. Detalhes - GET
        public async Task<IActionResult> Details(string id)
        {
            var livro = await _context.Livros.Find(l => l.Id == id).FirstOrDefaultAsync();
            if (livro == null)
                return NotFound();

            return View(livro);
        }

        // 5. Deletar - GET
        public async Task<IActionResult> Delete(string id)
        {
            var livro = await _context.Livros.Find(l => l.Id == id).FirstOrDefaultAsync();
            if (livro == null)
                return NotFound();

            return View(livro);
        }

        // 5. Deletar - POST
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _context.Livros.DeleteOneAsync(l => l.Id == id);
            return RedirectToAction(nameof(Index));
        }

    }
}
