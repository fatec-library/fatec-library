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

        public async Task<IActionResult> ListarLivros()
        {
            var livro = await _context.Livros.Find(p => true).ToListAsync();
            return View(livro);
        }

        //GET
        public async Task<IActionResult> Create(string LivroId, int QuantidadeExemplares)
        {
            var livro = await _context.Livros.Find(l => l.Id == LivroId).FirstOrDefaultAsync();

            if (livro == null)
            {
                return NotFound();
            }

            ViewBag.Livro = livro;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Exemplar exemplar, int QuantidadeExemplares)
        {
            if (QuantidadeExemplares <= 0)
            {
                ModelState.AddModelError("", "A quantidade de exemplares deve ser maior que zero.");
                return View();
            }

            if (ModelState.IsValid)
            {
                long numeroExemplares = await _context.Exemplares.CountDocumentsAsync(FilterDefinition<Exemplar>.Empty);

                var listaExemplares = new List<Exemplar>();

                for (long i = numeroExemplares; i < numeroExemplares + QuantidadeExemplares; i++)
                {
                    listaExemplares.Add(new Exemplar
                    {
                        Codigo_Exemplar = i+1,
                        Status_Exemplar = "Disponivel",
                        Livro_Id = exemplar.Livro_Id
                    });
                }

                await _context.Exemplares.InsertManyAsync(listaExemplares);

                return RedirectToAction("Administracao", "Home");
            }

            return View(exemplar);

        }

    }
}

