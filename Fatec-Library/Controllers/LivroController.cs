using Fatec_Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Drawing;

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
        public async Task<IActionResult> Create(Livro livro, IFormFile Imagem)
        {
            if (ModelState.IsValid)
            {

                if (Imagem != null && Imagem.Length > 0)
                {
                    livro.Capa_Livro = Path.Combine("img/capaLivros", Imagem.FileName);

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "capaLivros", Imagem.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Imagem.CopyToAsync(stream);
                    }
                }

                long numeroExemplares = await _context.Exemplares.CountDocumentsAsync(FilterDefinition<Exemplar>.Empty);
                livro.Codigo_Exemplar.Add(numeroExemplares + 1);
                livro.Id = ObjectId.GenerateNewId().ToString();

                var exemplar = new Exemplar
                {
                    Codigo_Exemplar = numeroExemplares + 1,
                    Status_Exemplar = "Disponivel",
                    Livro_Id = livro.Id
                };

                await _context.Exemplares.InsertOneAsync(exemplar);
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
            ViewBag.numeroExemplares = await _context.Exemplares.CountDocumentsAsync(FilterDefinition<Exemplar>.Empty);
            var livro = await _context.Livros.Find(l => l.Id == id).FirstOrDefaultAsync();
            ViewBag.area = await _context.Areas.Find(a => a.Id == livro.AreaId).FirstOrDefaultAsync();
            if (livro == null)
                return NotFound();

            var filtro = Builders<Exemplar>.Filter.And(
                 Builders<Exemplar>.Filter.Eq(e => e.Status_Exemplar, "Disponivel"),
                 Builders<Exemplar>.Filter.Eq(e => e.Livro_Id, id)
            );

            ViewBag.lista = await _context.Exemplares.Find(filtro).ToListAsync();


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

        // 6. Buscar
        public async Task<IActionResult> Buscar(string termo)
        {
            var filtro = Builders<Livro>.Filter.Empty;

            if (!string.IsNullOrEmpty(termo))
            {
                filtro = Builders<Livro>.Filter.Or(
                    Builders<Livro>.Filter.Regex("Titulo", new MongoDB.Bson.BsonRegularExpression(termo, "i")),
                    Builders<Livro>.Filter.Regex("Autor", new MongoDB.Bson.BsonRegularExpression(termo, "i"))
                );
            }

            var livros = await _context.Livros.Find(filtro).ToListAsync();

            return View("Index", livros);
        }
    }
}
