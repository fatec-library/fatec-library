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
            tipo = string.IsNullOrEmpty(tipo) ? "Aluno" : tipo;

            var usuario = new Usuario
            {
                Enderecos = new List<Endereco> { new Endereco() },
                Telefones = new List<Telefone> { new Telefone() }
            };

            ViewBag.tipo = tipo;
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(Usuario usuario, string senhaConfirmar, string tipo)
        {
            if (usuario.Enderecos == null || usuario.Enderecos.Count == 0)
            {
                usuario.Enderecos = new List<Endereco> { new Endereco() };
                usuario.Telefones = new List<Telefone> { new Telefone() };
            }

            if (ModelState.IsValid)
            {
                var senha = usuario.Senha;

                var usuarioExistente = await _context.Usuarios.Find(u => u.Email == usuario.Email || u.Ra == usuario.Ra || u.Cpf == usuario.Cpf || u.Rg == usuario.Rg).FirstOrDefaultAsync();

                tipo = string.IsNullOrEmpty(tipo) ? "Aluno" : tipo; //se tipo for nulo ou vazio, atribui Aluno como padrão

                if (usuarioExistente != null)
                {
                    // Verifica se o usuário já existe com base no email, RA, CPF ou RG
                    if (usuarioExistente.Email == usuario.Email)
                        ModelState.AddModelError("Email", "E-mail já cadastrado");

                    if (usuarioExistente.Ra == usuario.Ra)
                        ModelState.AddModelError("Ra", "RA já cadastrado.");

                    if (usuarioExistente.Cpf == usuario.Cpf)
                        ModelState.AddModelError("Cpf", "Cpf já cadastrado.");

                    if (usuarioExistente.Rg == usuario.Rg)
                        ModelState.AddModelError("Rg", "Rg já cadastrado.");

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


        public async Task<IActionResult> Perfil(Usuario usuario)
        {
            if (usuario == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            var dados = await _context.Usuarios.Find(u => u.Id == usuario.Id).FirstOrDefaultAsync();

            return View(dados);
        }


    } //fim classe

}

