using Fatec_Library.Helpers;
using Fatec_Library.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;

namespace Fatec_Library.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ContextMongodb _context;

        public UsuarioController()
        {
            _context = new ContextMongodb();
        }

        public async Task<IActionResult> Listar()
        {
            var usuarios = await _context.Usuarios.Find(p => true).ToListAsync();
            return View(usuarios);
        }

        public IActionResult Cadastrar(string tipo)
        {
            ViewBag.tipo = tipo;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(Usuario usuario, string senhaConfirmar, string tipo)
        {

            if (ModelState.IsValid)
            {
                var senha = usuario.Senha;

                var usuarioExistente = await _context.Usuarios.Find(u => u.Email == usuario.Email || u.Ra == usuario.Ra).FirstOrDefaultAsync();

                if (usuarioExistente != null)
                {

                    if (usuarioExistente.Email == usuario.Email)
                        ModelState.AddModelError("Email", "E-mail já cadastrado");

                    if (usuarioExistente.Ra == usuario.Ra)
                        ModelState.AddModelError("Ra", "RA já cadastrado.");

                    ViewBag.FazerLogin = true;
                    ViewBag.tipo = tipo;

                    return View(usuario); //retornar com os campos ja pre-preenchidos
                }

                if (senha == senhaConfirmar)
                {
                    usuario.Senha = PasswordHelper.HashPassword(senha);
                  
                    await _context.Usuarios.InsertOneAsync(usuario);

                    return RedirectToAction("Login", "Usuario");
                }
                else
                {
                    ModelState.AddModelError("senhaConfirmar", "As senhas não conferem.");
                }

            }
            
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string identificador, string senha)
        {
            //Busca no banco
            var usuario = await _context.Usuarios.Find(u => u.Email == identificador || u.Ra == identificador).FirstOrDefaultAsync();

            //se usuario for null ou senha estiver incorreta
            if (usuario == null || !PasswordHelper.VerifyPassword(senha, usuario.Senha))
            {
                return View();
            }

            // Criação do cookie de autenticação
            var claims = new List<Claim>
            {
                new Claim("UsuarioId", usuario.Id),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, usuario.Tipo)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // mantém login após fechar o navegador 
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);


            // Redireciona para a página inicial
            return RedirectToAction("Index", "Home");

        } //fim metodo Login

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Usuario");

        } //fim Logout


    } //fim classe

}

