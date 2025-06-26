using Fatec_Library.Helpers;
using Fatec_Library.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Security.Claims;
using System.Text.Json;
using static System.Data.Entity.Infrastructure.Design.Executor;

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
            var tipos = await _context.TiposUsuarios.Find(t => true).ToListAsync();

            foreach (var u in usuarios)
            {
                foreach (var t in tipos)
                {
                    if (u.TipoId == t.Id)
                    {
                        u.Tipo = t;
                    }
                }
            }

            return View(usuarios);
        }

        public IActionResult Cadastrar(string tiponome)
        {
            ViewBag.UrlAnterior = Request.Headers["Referer"].ToString();

            if (User.IsInRole("Bibliotecaria"))
            {
                ViewBag.Tipos = _context.TiposUsuarios.Find(Builders<TipoUsuario>.Filter.Empty).ToList();
            }

            var tipo = _context.TiposUsuarios.Find(t => t.Nome == tiponome).FirstOrDefault();

            ViewBag.Tipo = tipo;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(Usuario usuario, string senhaConfirmar, string tiponome)
        {

            ViewBag.UrlAnterior = Request.Headers["Referer"].ToString();

            if (!ModelState.IsValid)
            {
                ViewBag.DebugErros = ModelState
                    .Where(m => m.Value.Errors.Count > 0)
                    .Select(m => $"{m.Key}: {string.Join(", ", m.Value.Errors.Select(e => e.ErrorMessage))}")
                    .ToList();
            }

            if (User.IsInRole("Bibliotecaria"))
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

                    if (User.IsInRole("Bibliotecaria"))
                    {
                        var tipo = await _context.TiposUsuarios.Find(t => t.Id == usuario.TipoId).FirstOrDefaultAsync();
                        ViewBag.tipoNome = tipo.Nome;

                    }
                    else
                    {
                        var tipo = await _context.TiposUsuarios.Find(t => t.Nome == tiponome).FirstOrDefaultAsync();
                        ViewBag.tipoNome = tipo.Nome;

                    }

                    return View(usuario); //retornar com os campos ja pre-preenchidos
                }

                if (senha == senhaConfirmar)
                {
                    usuario.Senha = PasswordHelper.HashPassword(senha);

                    await _context.Usuarios.InsertOneAsync(usuario);

                    if (User.IsInRole("Bibliotecaria"))
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
            var tipo = await _context.TiposUsuarios.Find(t => t.Id == usuario.TipoId).FirstOrDefaultAsync();
            
            string tipoUser = tipo.Nome;

            if (string.IsNullOrEmpty(senha))
            {
                ViewBag.LoginErro = "Preencha o campo Senha.";
                return View();
            }

            //se usuario for null ou senha estiver incorreta
            if (usuario == null || !PasswordHelper.VerifyPassword(senha, usuario.Senha))
            {
                ViewBag.LoginErro = "Usuario ou Senha podem estar incorretos, verifique e tente fazer o login novamente.";
                return View();
            }

            // Criação do cookie de autenticação
            var claims = new List<Claim>
            {
                new Claim("UsuarioId", usuario.Id),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, tipoUser)
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

        //Buscar
        public async Task<IActionResult> Buscar(string termo)
        {
            var filtro = Builders<Usuario>.Filter.Empty;

            if (!string.IsNullOrEmpty(termo))
            {
                filtro = Builders<Usuario>.Filter.Or(
                    Builders<Usuario>.Filter.Regex("Nome", new MongoDB.Bson.BsonRegularExpression(termo, "i")),
                    Builders<Usuario>.Filter.Regex("Email", new MongoDB.Bson.BsonRegularExpression(termo, "i"))
                );
            }

            var usuarios = await _context.Usuarios.Find(filtro).ToListAsync();

            return View("Listar", usuarios);
        }

        private bool UsuarioExists(string? id)
        {
            return _context.Usuarios.Find(e => e.Id == id).Any();
        }

        [HttpGet]
        public async Task<IActionResult> Promover(string id)
        {
            var usuario = await _context.Usuarios.Find(u => u.Id == id).FirstOrDefaultAsync();
            ViewBag.Tipos = await _context.TiposUsuarios.Find(t => true).ToListAsync();

            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Promover(Usuario usuario)
        {

            ViewBag.Tipos = await _context.TiposUsuarios.Find(t => true).ToListAsync();

            if (!ModelState.IsValid)
            {
                ViewBag.DebugErros = ModelState
                    .Where(m => m.Value.Errors.Count > 0)
                    .Select(m => $"{m.Key}: {string.Join(", ", m.Value.Errors.Select(e => e.ErrorMessage))}")
                    .ToList();
            }

            if (ModelState.IsValid)
            {
                var filter = Builders<Usuario>.Filter.Eq(u => u.Id, usuario.Id);

                var update = Builders<Usuario>.Update
                    .Set(u => u.TipoId, usuario.TipoId);

                await _context.Usuarios.UpdateOneAsync(filter, update);

                return RedirectToAction("Listar", "Usuario");

            }

            return View(usuario);

        }

    } //fim classe

}




