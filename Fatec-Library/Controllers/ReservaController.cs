using Fatec_Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Fatec_Library.Controllers
{
    [Authorize] 
    public class ReservaController : Controller
    {
        private readonly ContextMongodb _context;

        public ReservaController()
        {
            _context = new ContextMongodb();
        }

        // Lista Reservas
        public async Task<IActionResult> Listar()
        {
            var reservas = await _context.Reservas.Find(_ => true).ToListAsync();
            var lista = new List<dynamic>();

            foreach (var r in reservas)
            {
                var livro = await _context.Livros.Find(l => l.Id == r.Livro_Id).FirstOrDefaultAsync();
                var usuario = await _context.Usuarios.Find(u => u.Id == r.Usuario_Id).FirstOrDefaultAsync();

                lista.Add(new
                {
                    Id = r.Id,
                    Aluno = usuario?.Nome ?? "Desconhecido",
                    Livro = livro?.Titulo ?? "Desconhecido",
                    Data = r.Data_Reserva.ToString("dd/MM/yyyy")
                });
            }

            return View(lista);
        }

        // página de Cria Reserva
        [HttpGet]
        public IActionResult Criar()
        {
            return View();
        }

        // Criar Reserva
        [HttpPost]

        public async Task<IActionResult> Criar(string livroNome, int dias)

        {

            var usuarioNome = User.Identity?.Name;

            var usuario = await _context.Usuarios.Find(u => u.Nome == usuarioNome).FirstOrDefaultAsync();

            if (usuario == null) //Caso passe do Authorize 

            {

                TempData["Erro"] = "Cadastre-se primeiro antes de fazer uma reserva.";

                return RedirectToAction("Criar");

            }

            var livro = await _context.Livros

                .Find(l => l.Titulo.ToLower().Contains(livroNome.ToLower()))

                .FirstOrDefaultAsync();

            if (livro == null)

            {

                TempData["Erro"] = "Livro não encontrado.";

                return RedirectToAction("Criar");

            }

            // Buscar exemplares desse livro (pelo Código_Exemplar)

            var exemplaresDisponiveis = await _context.Exemplares

                .Find(e => livro.Codigo_Exemplar.Contains(e.Codigo_Exemplar) && e.Status_Exemplar == "Disponível")

                .ToListAsync();

            if (exemplaresDisponiveis.Count == 0)

            {

                TempData["Erro"] = "Não há exemplares disponíveis para esse livro no momento.";

                return RedirectToAction("Criar");

            }

            // Cria a reserva normalmente

            var reserva = new Reserva

            {

                Usuario_Id = usuario.Id,

                Livro_Id = livro.Id,

                Data_Reserva = DateTime.Now

            };

            await _context.Reservas.InsertOneAsync(reserva);

            TempData["Sucesso"] = $"Reserva feita com sucesso para o livro '{livro.Titulo}'!";

            return RedirectToAction("Listar");

        }



        // Remover Reserva
        [HttpPost]
        public async Task<IActionResult> Remover(string id)
        {
            var reserva = await _context.Reservas.Find(r => r.Id == id).FirstOrDefaultAsync();

            if (reserva == null)
            {
                TempData["Erro"] = "Reserva não encontrada.";
                return RedirectToAction("Listar");
            }

            await _context.Reservas.DeleteOneAsync(r => r.Id == id);
            TempData["Sucesso"] = "Reserva removida com sucesso!";
            return RedirectToAction("Listar");
        }
    }
}
