using Fatec_Library.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Fatec_Library.Controllers
{
    public class EmprestimoController : Controller
    {
        private readonly ContextMongodb _context;
        private readonly IMongoCollection<Emprestimo> _emprestimos;
        private readonly IMongoCollection<Livro> _livros;

        public EmprestimoController(IMongoClient client)
        {
            _context = new ContextMongodb();
            var database = client.GetDatabase("dbFatecLibrary");
            _emprestimos = database.GetCollection<Emprestimo>("emprestimos");
            _livros = database.GetCollection<Livro>("livros");
        }

        public async Task<IActionResult> Listar()
        {
            var emprestimo = await _context.Emprestimos.Find(p => true).ToListAsync();
            return View(emprestimo);
        }

        // GET: /Emprestimo
        public IActionResult Index()
        {
            var lista = _emprestimos.Find(_ => true).ToList();
            return View(lista);
        }


        // POST: /Emprestimo/FazerEmprestimo
        [HttpPost]
        public IActionResult FazerEmprestimo(string livroId, string raAluno, string nomeAluno, string usuarioId, int codigoExemplar)
        {
            var livro = _livros.Find(l => l.Id == livroId).FirstOrDefault();

            if (livro == null || livro.Status != "Disponível")
                return BadRequest("Livro indisponível ou não encontrado.");

            var emprestimo = new Emprestimo
            {
                Ra_Aluno = raAluno,
                Nome_Aluno = nomeAluno,
                Usuario_Id = usuarioId,
                Livro_Id = livroId,
                Codigo_Exemplar = codigoExemplar,
                Livro = livro
            };

            emprestimo.FazerEmprestimo(livro);

            // Atualiza o status do livro
            _livros.ReplaceOne(l => l.Id == livroId, livro);

            // Registra o empréstimo
            _emprestimos.InsertOne(emprestimo);

            return RedirectToAction("Index");
        }


        // POST: /Emprestimo/DevolverLivro
        [HttpPost]
        public IActionResult DevolverLivro(string emprestimoId)
        {
            var emprestimo = _emprestimos.Find(e => e.Id == emprestimoId).FirstOrDefault();

            if (emprestimo == null || emprestimo.Status_Emprestimo != "Ativo")
                return BadRequest("Empréstimo não encontrado ou já finalizado.");

            var livro = _livros.Find(l => l.Id == emprestimo.Livro_Id).FirstOrDefault();
            if (livro == null)
                return BadRequest("Livro relacionado ao empréstimo não encontrado.");

            emprestimo.Livro = livro;
            emprestimo.DevolverLivro(livro);

            // Atualiza o empréstimo
            _emprestimos.ReplaceOne(e => e.Id == emprestimo.Id, emprestimo);

            // Atualiza o status do livro
            _livros.ReplaceOne(l => l.Id == livro.Id, livro);

            return RedirectToAction("Index");
        }

        // Esse método exibe a página para realizar um novo empréstimo
        public IActionResult Novo()
        {
            var livrosDisponiveis = _livros.Find(l => l.Status == "Disponível").ToList();
            return View(livrosDisponiveis);
        }


    }
}
