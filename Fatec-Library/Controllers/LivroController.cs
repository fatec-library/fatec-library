using Fatec_Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
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
                livro.DataCadastro = DateTime.Now;

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

            var areas = await _context.Areas.Find(a => true).ToListAsync();
            ViewBag.Areas = new SelectList(areas, "Id", "Descritivo");

            return View(livro);
        }

        // 3. Editar livro - POST
        [HttpPost]
        public async Task<IActionResult> Edit(string id, Livro livroEditado, string imagemAtual, IFormFile? Imagem)
        {
            var areas = await _context.Areas.Find(a => true).ToListAsync();
            ViewBag.Areas = new SelectList(areas, "Id", "Descritivo");

            if (ModelState.IsValid)
            {
                if (Imagem != null && Imagem.Length > 0)
                {
                    // Atribui o caminho relativo da imagem no campo 'Imagem'
                    livroEditado.Capa_Livro = Path.Combine("img/capaLivros", Imagem.FileName);
                }
                else
                {
                    livroEditado.Capa_Livro = imagemAtual;
                }


                await _context.Livros.ReplaceOneAsync(l => l.Id == id, livroEditado);


                if (Imagem != null && Imagem.Length > 0)
                {
                    var imagemFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagemAtual);

                    // Verificar se o arquivo existe antes de tentar deletar
                    if (System.IO.File.Exists(imagemFilePath))
                    {
                        await Task.Run(() => System.IO.File.Delete(imagemFilePath));
                    }
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                            "wwwroot", "img", "capaLivros", Imagem.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Imagem.CopyToAsync(stream);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(livroEditado);
        }

        // 4. Detalhes - GET
        public async Task<IActionResult> Details(string id)
        {
            ViewBag.UrlAnterior = Request.Headers["Referer"].ToString();

            var livro = await _context.Livros.Find(l => l.Id == id).FirstOrDefaultAsync();

            if (livro == null)
                return NotFound();

            ViewBag.area = await _context.Areas.Find(a => a.Id == livro.AreaId).FirstOrDefaultAsync();

            var filtro = Builders<Exemplar>.Filter.And(
                 Builders<Exemplar>.Filter.Eq(e => e.Status_Exemplar, "Disponivel"),
                 Builders<Exemplar>.Filter.Eq(e => e.Livro_Id, id)
            );

            float numeroExemplares = await _context.Exemplares.Find(filtro).CountDocumentsAsync();
            var lista = await _context.Exemplares.Find(filtro).ToListAsync();

            ViewBag.numeroExemplares = numeroExemplares;
            ViewBag.lista = lista;

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
        public async Task<IActionResult> DeleteConfirmed(string id, string? imagemAtual)
        {
            await _context.Livros.DeleteOneAsync(l => l.Id == id);

            if (!string.IsNullOrEmpty(imagemAtual))
            {
                var imagemFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagemAtual);

                // Verificar se o arquivo existe antes de tentar deletar
                if (System.IO.File.Exists(imagemFilePath))
                {
                    await Task.Run(() => System.IO.File.Delete(imagemFilePath));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // 6. Acervo
        public async Task<IActionResult> Acervo()
        {
            var area = await _context.Areas.Find(a => a.Descritivo == "Tecnologia").FirstOrDefaultAsync();

            var livrosRecomendados = await _context.Livros.Find(l => true).Limit(10).ToListAsync();
            var livrosAreaTecnologia = await _context.Livros.Find(l => l.AreaId == area.Id).Limit(10).ToListAsync();
            var novidades = await _context.Livros.Find(FilterDefinition<Livro>.Empty).SortByDescending(l => l.DataCadastro).Limit(10).ToListAsync();
            var maisEmprestados = await _context.Livros.Find(FilterDefinition<Livro>.Empty).SortByDescending(l => l.QuantidadeEmprestimos).Limit(10).ToListAsync();

            ViewBag.LivrosRecomendados = livrosRecomendados;
            ViewBag.livrosAreaTecnologia = livrosAreaTecnologia;
            ViewBag.novidades = novidades;
            ViewBag.maisEmprestados = maisEmprestados;

            return View();
        }

        // 7. Buscar
        public async Task<IActionResult> Buscar(string termo, string campo = "Titulo")
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

 
            ViewBag.Termo = termo;
            ViewBag.Campo = campo;

            return View("Index", livros);
        }

        public async Task<IActionResult> BuscarNoAcervo(string termo, string campo = "Titulo")
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

            return View();

        }

    }

}
