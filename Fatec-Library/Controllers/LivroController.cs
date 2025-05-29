using Fatec_Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Listar()
        {
            var livro = await _context.Livros.Find(p => true).ToListAsync();
            return View(livro);
        }

        public async Task<IActionResult> InserirLivroTeste()
        {
            var livro = new Livro
            {
                Titulo = "Estruturas de Dados em C#",
                Autores = new List<string> { "Fulano de Tal", "Beltrano da Silva" },
                Editora = "Editora Fatec",
                Descricao = "Livro de teste sobre estruturas de dados.",
                Ano_Publicacao = 2023,
                Isbn = "978-85-333-1234-5",
                Idioma = "Português",
                Num_Paginas = 350,
                Capa_Livro = "https://examplofoda.com/capa.jpg",
                Area = null,
                Codigo_Exemplar = new List<int> { 1001, 1002, 1003 }
            };




            await _context.Livros.InsertOneAsync(livro);

            TempData["Sucesso"] = "Livro de teste cadastrado com sucesso!";
            return RedirectToAction("Listar");
        }
    }
}