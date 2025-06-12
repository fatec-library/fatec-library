using Fatec_Library.Helpers;
using Fatec_Library.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Data.Entity.Infrastructure;
using System.Drawing;
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

        public IActionResult Cadastrar(string tipoid)
        {
            if (User.IsInRole("684973ab308a13b813d1210c"))
            {
                ViewBag.Tipos = _context.TiposUsuarios.Find(Builders<TipoUsuario>.Filter.Empty).ToList();
            }

            ViewBag.tipoid = tipoid;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(Usuario usuario, string senhaConfirmar, string tipoid)
        {
            if (User.IsInRole("684973ab308a13b813d1210c"))
            {
                ViewBag.Tipos = _context.TiposUsuarios.Find(Builders<TipoUsuario>.Filter.Empty).ToList();
            }

            if (ModelState.IsValid)
            {


                var senha = usuario.Senha;

                var usuarioExistente = await _context.Usuarios.Find(u => u.Email == usuario.Email || u.Ra == usuario.Ra || u.Cpf == usuario.Cpf || u.Rg == usuario.Rg).FirstOrDefaultAsync();


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

                    ViewBag.FazerLogin = true; //se caso ja tiver um usuario com o email, ra, cpf ou rg, entao exibi msg para fazer login
                    ViewBag.tipoid = tipoid; //retorna o tipoid para a view, caso seja necessario

                    return View(usuario); //retornar com os campos ja pre-preenchidos
                }

                if (senha == senhaConfirmar)
                {
                    usuario.Senha = PasswordHelper.HashPassword(senha);

                    await _context.Usuarios.InsertOneAsync(usuario);

                    if (User.IsInRole("684973ab308a13b813d1210c"))
                        return RedirectToAction("Listar", "Usuario");

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
                new Claim(ClaimTypes.Role, usuario.TipoId)
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


        public async Task<IActionResult> Perfil(string usuarioid)
        {
            if (usuarioid == null)
            {

                return RedirectToAction("Login", "Usuario");
            }

            var dados = await _context.Usuarios.Find(u => u.Id == usuarioid).FirstOrDefaultAsync();

            return View(dados);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            var dados = await _context.Usuarios.Find(u => u.Id == id).FirstOrDefaultAsync();

            return View(dados);
        }

        public ContextMongodb Get_context()
        {
            return _context;
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Usuario dados)
        {


            var dadosAtuais = await _context.Usuarios.Find(i => i.Id == dados.Id).FirstOrDefaultAsync();


            if (dadosAtuais.Email != dados.Email || dadosAtuais.Ra != dados.Ra || dadosAtuais.Cpf != dados.Cpf || dadosAtuais.Rg != dados.Rg)
            {
                var usuarioExistente = await _context.Usuarios.Find(u => u.Id != dados.Id && u.Email == dados.Email || u.Ra == dados.Ra || u.Cpf == dados.Cpf || u.Rg == dados.Rg).FirstOrDefaultAsync();

                if (usuarioExistente != null)
                {
                    // Verifica se o usuário já existe com base no email, RA, CPF ou RG


                    if (usuarioExistente.Email == dados.Email && usuarioExistente.Email != dadosAtuais.Email)
                        ModelState.AddModelError("Email", "E-mail já cadastrado");

                    if (usuarioExistente.Ra == dados.Ra && usuarioExistente.Ra != dadosAtuais.Ra)
                        ModelState.AddModelError("Ra", "RA já cadastrado.");

                    if (usuarioExistente.Cpf == dados.Cpf && usuarioExistente.Cpf != dadosAtuais.Cpf)
                        ModelState.AddModelError("Cpf", "Cpf já cadastrado.");

                    if (usuarioExistente.Rg == dados.Rg && usuarioExistente.Rg != dadosAtuais.Rg)
                        ModelState.AddModelError("Rg", "Rg já cadastrado.");
                }

            }

            if (!ModelState.IsValid)
            {
                ViewBag.erro = true;
                return View(dados);
            }

            try
            {
                var filter = Builders<Usuario>.Filter.Eq(u => u.Id, dados.Id);

                var update = Builders<Usuario>.Update
                    .Set(u => u.Nome, dados.Nome)
                    .Set(u => u.Email, dados.Email)
                    .Set(u => u.Ra, dados.Ra)
                    .Set(u => u.Cpf, dados.Cpf)
                    .Set(u => u.Rg, dados.Rg)
                    .Set(u => u.DataNascimento, dados.DataNascimento)
                    .Set(u => u.Senha, dados.Senha)
                    .Set(u => u.TipoId, dados.TipoId);

                if (dados.Endereco != null)
                    update = update.Set(u => u.Endereco, dados.Endereco);

                if (dados.Telefones != null && dados.Telefones.Any())
                    update = update.Set(u => u.Telefones, dados.Telefones);

                await _context.Usuarios.UpdateOneAsync(filter, update);

                return RedirectToAction("Perfil", "Usuario", new { usuarioid = dados.Id });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(dados.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }

            }

        }

        private bool UsuarioExists(string? id)
        {
            return _context.Usuarios.Find(e => e.Id == id).Any();
        }
    } //fim classe

}

