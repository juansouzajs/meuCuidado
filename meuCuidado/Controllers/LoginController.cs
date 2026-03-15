using meuCuidado.Dominio.Models;
using meuCuidado.Dominio.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using static meuCuidado.Dominio.Extensions.EnumExtension;

namespace meuCuidado.Controllers
{
    public class LoginController : Controller
    {
        private readonly MeuCuidadoDbContext _context = new MeuCuidadoDbContext();
        private readonly EmailController _emailController = new EmailController();

        // Tela de Login
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult RealizarLogin(string email, string senha, string returnUrl)
        {
            try
            {
                var cuidadorDeIdoso = _context.CuidadoresDeIdoso.FirstOrDefault(p => p.Email == email);
                var fisioterapeuta = _context.Fisioterapeutas.FirstOrDefault(p => p.Email == email);
                var idoso = _context.Idosos.FirstOrDefault(p => p.Email == email);
                var tutor = _context.Tutores.FirstOrDefault(p => p.Email == email);

                bool senhaValida = false;

                if (cuidadorDeIdoso != null)
                    senhaValida = SenhaHelper.VerificarSenha(senha, cuidadorDeIdoso.Senha);

                else if (fisioterapeuta != null)
                    senhaValida = SenhaHelper.VerificarSenha(senha, fisioterapeuta.Senha);

                else if (idoso != null)
                    senhaValida = SenhaHelper.VerificarSenha(senha, idoso.Senha);

                else if (tutor != null)
                    senhaValida = SenhaHelper.VerificarSenha(senha, tutor.Senha);

                if (senhaValida)
                {
                    if (cuidadorDeIdoso?.EtapaAcesso != EtapaAcesso.AcessoLiberado &&
                        fisioterapeuta?.EtapaAcesso != EtapaAcesso.AcessoLiberado &&
                        idoso?.EtapaAcesso != EtapaAcesso.AcessoLiberado &&
                        tutor?.EtapaAcesso != EtapaAcesso.AcessoLiberado)
                        return Json(new { success = false, message = "Acesso negado! Aguarde a liberação." });

                    var claims = new[] { new Claim(ClaimTypes.Name, email) };
                    var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                    var authManager = HttpContext.GetOwinContext().Authentication;
                    authManager.SignIn(identity);

                    var codigoAutenticacao = _emailController.EnviarEmailAutenticacao(email);

                    Session["CodigoAutenticacao"] = codigoAutenticacao;
                    Session["SenhaCodificada"] = senha;

                    if (cuidadorDeIdoso != null)
                        Session["TipoUsuario"] = GetEnumDescription(TipoUsuario.Cuidador);
                    else if (fisioterapeuta != null)
                        Session["TipoUsuario"] = GetEnumDescription(TipoUsuario.Fisioterapeuta);
                    else if (tutor != null)
                        Session["TipoUsuario"] = GetEnumDescription(TipoUsuario.Tutor);
                    else if (idoso != null)
                        Session["TipoUsuario"] = GetEnumDescription(TipoUsuario.Idoso);

                    var autenticacaoViewModel = new AutenticacaoViewModel
                    {
                        Codigo1 = string.Empty,
                        Codigo2 = string.Empty,
                        Codigo3 = string.Empty,
                        Codigo4 = string.Empty,
                        Codigo5 = string.Empty,
                        Email = email
                    };

                    return Json(new
                    {
                        success = true,
                        redirectUrl = Url.Action("Autenticacao", "Login", new { email = email })
                    });
                }

                return Json(new { success = false, message = "Usuário ou senha inválidos." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro interno: " + ex.Message });
            }
        }


        public ActionResult Autenticacao(AutenticacaoViewModel autenticacaoViewModel)
        {
            return View(autenticacaoViewModel);
        }

        [HttpPost]
        public ActionResult ReenviarCodigoAutenticacao(string email, string senha, string returnUrl)
        {
            senha = Session["SenhaCodificada"].ToString();
            return RealizarLogin(email, senha, returnUrl);
        }

        // Métodos para login com Google
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Solicitar redirecionamento para o provedor de autenticação externa
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        // Método de callback após autenticação externa
        [HttpGet]
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await HttpContext.GetOwinContext().Authentication.GetExternalLoginInfoAsync();
            if (loginInfo == null)
                return RedirectToAction("Login");

            var email = string.Empty;
            var senha = string.Empty;

            //var usuario = _context.Pessoas.SingleOrDefault(p => p.Email == loginInfo.Email);
            //if (usuario == null)
            //{
            //    // Se não existir, você pode criar um novo usuário aqui
            //    usuario = new Usuario
            //    {
            //        Email = loginInfo.Email,
            //        // Preencha outros campos necessários
            //    };
            //    _context.Pessoas.Add(usuario);
            //    _context.SaveChanges();
            //}

            var claims = new[] { new Claim(ClaimTypes.Name, email) };
            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignIn(identity);

            //Realizar login
            authManager.SignIn(identity);

            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        public ActionResult ValidarCodigoAutenticacao(AutenticacaoViewModel model)
        {
            // Concatenar os códigos em um só
            var codigoInserido = $"{model.Codigo1}{model.Codigo2}{model.Codigo3}{model.Codigo4}{model.Codigo5}";
            // Pega o código da sessão
            var codigoCorreto = Session["CodigoAutenticacao"].ToString();

            if (codigoInserido == codigoCorreto)
            {
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Dashboard", "Dashboard")
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Código de autenticação inválido."
                });
            }
        }

        [HttpPost]
        public ActionResult FecharPopup()
        {
            ViewBag.ShowPopup = false;
            return View("Login");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login", "Login");
        }

        // Método para redirecionar após o login
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Dashboard");
        }
    }
}