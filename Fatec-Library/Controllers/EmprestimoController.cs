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

        public async Task<IActionResult> Index(string termo, string campo = "Titulo")
        {
            FilterDefinition<Livro> filtro = Builders<Livro>.Filter.Empty;

            if (!string.IsNullOrEmpty(termo))
            {
                // Usar filtro dinâmico com base no campo selecionado
                var regex = new MongoDB.Bson.BsonRegularExpression(termo, "i");

                switch (campo)
                {
                    case "Titulo":
                        filtro = Builders<Livro>.Filter.Regex(l => l.Titulo, regex);
                        break;

                    case "Autores":
                        filtro = Builders<Livro>.Filter.Regex(l => l.Autores, regex);
                        break;

                    case "Cdd":
                        filtro = Builders<Livro>.Filter.Regex(l => l.Cdd, regex);
                        break;

                    default:
                        filtro = Builders<Livro>.Filter.Regex(l => l.Titulo, regex);
                        break;
                }

            }

            var livros = await _context.Livros.Find(filtro).ToListAsync();

            ViewBag.Livros = livros;
            ViewBag.Termo = termo;
            ViewBag.Campo = campo;

            return View(livros);
        }

        public IActionResult Listar()
        {
            var emprestimos = _context.Emprestimos.Find(p => true).ToList();

            foreach (var emprestimo in emprestimos)
            {
                if (emprestimo.Status_Emprestimo != "Devolvido" &&
                    DateTime.Now.Date > emprestimo.Data_Devolucao.Date)
                {
                    emprestimo.Status_Emprestimo = "Atrasado";
                }
            }

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

            ViewBag.Livro = new { capa = livro.Capa_Livro, autores = livro.Autores };

            ViewBag.area = await _context.Areas.Find(a => a.Id == livro.AreaId).FirstOrDefaultAsync();
            ViewBag.Exemplares = await _context.Exemplares.Find(e => e.Livro_Id == livro.Id && e.Status_Exemplar == "Disponivel").ToListAsync();

            if (ViewBag.Exemplares.Count < 1)
            {
                ViewBag.SemExemplaresDisp = true;
                return View();
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
            if (emprestimo.Data_Devolucao.Date <= DateTime.Now.Date)
            {
                emprestimo.Data_Devolucao = DateTime.Now.Date.AddDays(1);
            }

            var livro = await _context.Livros.Find(l => l.Id == emprestimo.Livro_Id).FirstOrDefaultAsync();

            if (livro == null)
            {
                ModelState.AddModelError("", "Livro não encontrado.");
                ViewBag.UserNotFound = false;
                return View(emprestimo);
            }

            ViewBag.capa = livro.Capa_Livro;
            ViewBag.Autores = livro.Autores;

            var usuario = await _context.Usuarios.Find(u => u.Ra == emprestimo.Ra_Aluno || u.Nome == emprestimo.Nome_Aluno).FirstOrDefaultAsync();

            if (usuario == null)
            {
                ViewBag.UserNotFound = true;

                ViewBag.Exemplares = _context.Exemplares
                    .Find(e => e.Livro_Id == emprestimo.Livro_Id && e.Status_Exemplar == "Disponivel")
                    .ToList();

                ViewBag.area = await _context.Areas
                    .Find(a => a.Id == livro.AreaId)
                    .FirstOrDefaultAsync();

                ViewBag.Livro = new { capa = livro.Capa_Livro, autores = livro.Autores };

                return View(emprestimo);
            }

            var exemplar = _context.Exemplares.Find(e => e.Livro_Id == emprestimo.Livro_Id && e.Status_Exemplar == "Disponivel").ToList();
            ViewBag.Exemplares = exemplar;

            ViewBag.area = await _context.Areas.Find(a => a.Id == livro.AreaId).FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {

                var filter = Builders<Exemplar>.Filter.Eq(e => e.Codigo_Exemplar, emprestimo.Codigo_Exemplar);
                var update = Builders<Exemplar>.Update.Set(e => e.Status_Exemplar, "Emprestado");
                await _context.Exemplares.UpdateOneAsync(filter, update);

                var quantidadeEmprestimo = 1 + livro.QuantidadeEmprestimos;

                var filterLivro = Builders<Livro>.Filter.Eq(l => l.Id, livro.Id);
                var updateLivro = Builders<Livro>.Update.Set(l => l.QuantidadeEmprestimos, quantidadeEmprestimo);
                await _context.Livros.UpdateOneAsync(filterLivro, updateLivro);

                emprestimo.Usuario_Id = usuario.Id;

                await _context.Emprestimos.InsertOneAsync(emprestimo);

                return RedirectToAction("Listar", "Emprestimo");

            }

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
