using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Fatec_Library.Models;
using MongoDB.Driver;

namespace Fatec_Library.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ContextMongodb _context;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        _context = new ContextMongodb();

    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Administracao()
    {
        var usuarioId = User.FindFirst("UsuarioId")?.Value; // pega o ID do usuário

        Usuario? usuario = null;
        if (usuarioId != null)
        {
            // Buscar o usuário no banco, por exemplo
            usuario = await _context.Usuarios.Find(u => u.Id == usuarioId).FirstOrDefaultAsync();

            if (User.IsInRole("Bibliotecaria"))
                return View();
            
        }

        
        return NotFound();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
