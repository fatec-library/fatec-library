using Fatec_Library.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Cryptography.X509Certificates;

namespace Fatec_Library.Controllers
{
    public class EmprestimoController : Controller
    {
        private readonly ContextMongodb _context;

        public EmprestimoController()
        {
            _context = new ContextMongodb();

        }

        public async Task<IActionResult> Index()
        {
            var livro = await _context.Livros.Find(p => true).ToListAsync();
            return View(livro);
        }

        public IActionResult Listar()
        {
            var emprestimos = _context.Emprestimos.Find(p => true).ToList();
            return View(emprestimos);
        }

        // Esse método exibe a página para realizar um novo empréstimo
        public async Task<IActionResult> NovoEmprestimo(string LivroId)
        {
            var livro = await _context.Livros.Find(l => l.Id == LivroId).FirstOrDefaultAsync();

            if (livro == null)
            {
                return NotFound();
            }

            var emprestimo = new Emprestimo
            {
                Livro_Id = livro.Id,
                Livro = livro
            };

            return View(emprestimo);
        }

        [HttpPost]
        public async Task<IActionResult> NovoEmprestimo(Emprestimo emprestimo)
        {
            if (ModelState.IsValid)
            {
                await _context.Emprestimos.InsertOneAsync(emprestimo);

                ViewBag.emprestado = "certo";
                return RedirectToAction("Listar", "Emprestimo");
            }
            else
            {
                ViewBag.emprestado = "erro;";
                return View(emprestimo);
            }
        }
        public async Task<IActionResult> DevolverEmprestimo(string id)
        {
            var emprestimo = await _context.Emprestimos.Find(e => e.Id == id).FirstOrDefaultAsync();
            return View(emprestimo);
        }

        [HttpPost]
        public async Task<IActionResult> DevolverEmprestimo(Emprestimo emprestimo, string status)
        {
            if (ModelState.IsValid)
            {
                var filter = Builders<Emprestimo>.Filter.Eq(e => e.Id, emprestimo.Id);
                var update = Builders<Emprestimo>.Update.Set(e => e.Status_Emprestimo, status);
                await _context.Emprestimos.UpdateOneAsync(filter, update);
                ViewBag.devolvido = "certo";
                return RedirectToAction("Listar", "Emprestimo");
            }
            return View(emprestimo);


        }

    }
}
