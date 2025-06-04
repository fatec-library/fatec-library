using Fatec_Library.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

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
                return View();
            }
            else
            {
                ViewBag.emprestado = "erro;";
                return View(emprestimo);
            }
            
        }

    }
}
