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
            ViewBag.Livro = new { capa = livro.Capa_Livro, autores = livro.Autores };

            ViewBag.area = await _context.Areas.Find(a => a.Id == livro.AreaId).FirstOrDefaultAsync();
            ViewBag.Exemplares = await _context.Exemplares.Find(e => e.Livro_Id == livro.Id && e.Status_Exemplar == "Disponivel").ToListAsync();

            if (ViewBag.Exemplares.Count < 1)
            {
                ViewBag.SemExemplaresDisp = true;
                return View();
            }

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
            var livro = await _context.Livros.Find(l => l.Id == emprestimo.Livro_Id).FirstOrDefaultAsync();
            ViewBag.capa = livro.Capa_Livro;
            ViewBag.Autores = livro.Autores;

            var usuario = await _context.Usuarios.Find(u => u.Ra == emprestimo.Ra_Aluno || u.Nome == emprestimo.Nome_Aluno).FirstOrDefaultAsync();

            var exemplar = _context.Exemplares.Find(e => e.Livro_Id == emprestimo.Livro_Id && e.Status_Exemplar == "Disponivel").ToList();
            ViewBag.Exemplares = exemplar;

            ViewBag.area = await _context.Areas.Find(a => a.Id == livro.AreaId).FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {

                if (usuario != null)
                {
                    var filter = Builders<Exemplar>.Filter.Eq(e => e.Codigo_Exemplar, emprestimo.Codigo_Exemplar);
                    var update = Builders<Exemplar>.Update.Set(e => e.Status_Exemplar, "Emprestado");
                    await _context.Exemplares.UpdateOneAsync(filter, update);

                    await _context.Emprestimos.InsertOneAsync(emprestimo);

                    return RedirectToAction("Listar", "Emprestimo");
                }

            }

            ViewBag.UserNotFound = true;
            return View(emprestimo);

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

                var filter2 = Builders<Exemplar>.Filter.Eq(e => e.Codigo_Exemplar, emprestimo.Codigo_Exemplar);
                var update2 = Builders<Exemplar>.Update.Set(e => e.Status_Exemplar, "Disponivel");
                await _context.Exemplares.UpdateOneAsync(filter2, update2);

                return RedirectToAction("Listar", "Emprestimo");
            }
            return View(emprestimo);


        }

    }
}
