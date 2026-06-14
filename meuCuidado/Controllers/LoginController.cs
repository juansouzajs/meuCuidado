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

                    Session["IdUsuario"] = cuidadorDeIdoso?.Id ?? fisioterapeuta?.Id ?? idoso?.Id ?? tutor?.Id;
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

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        [HttpGet]
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await HttpContext.GetOwinContext().Authentication.GetExternalLoginInfoAsync();
            if (loginInfo == null)
                return RedirectToAction("Login");

            var email = string.Empty;
            var senha = string.Empty;

            var claims = new[] { new Claim(ClaimTypes.Name, email) };
            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignIn(identity);

            authManager.SignIn(identity);

            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        public ActionResult ValidarCodigoAutenticacao(AutenticacaoViewModel model)
        {
            var codigoInserido = $"{model.Codigo1}{model.Codigo2}{model.Codigo3}{model.Codigo4}{model.Codigo5}";
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Dashboard");
        }

        public ActionResult EsqueciMinhaSenha()
        {
            return View();
        }

        public ActionResult ValidarCodigoRecuperacao()
        {
            return View();
        }

        public ActionResult RedefinirSenha()
        {
            if (Session["RecuperacaoAutorizada"] == null)
            {
                return RedirectToAction("EsqueciMinhaSenha");
            }

            return View();
        }

        [HttpPost]
        public JsonResult SolicitarRecuperacaoSenha(string email)
        {
            try
            {
                var usuarioExiste =
                    _context.CuidadoresDeIdoso.Any(x => x.Email == email) ||
                    _context.Fisioterapeutas.Any(x => x.Email == email) ||
                    _context.Idosos.Any(x => x.Email == email) ||
                    _context.Tutores.Any(x => x.Email == email);

                if (!usuarioExiste)
                {
                    return Json(new
                    {
                        success = false,
                        message = "E-mail não encontrado."
                    });
                }

                var codigo = _emailController
                    .EnviarEmailRecuperacaoSenha(email);

                Session["CodigoRecuperacaoSenha"] = codigo;
                Session["EmailRecuperacaoSenha"] = email;

                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult ValidarCodigoRecuperacao(string codigo)
        {
            var codigoSessao =
                Session["CodigoRecuperacaoSenha"]?.ToString();

            if (string.IsNullOrEmpty(codigoSessao))
            {
                return Json(new
                {
                    success = false,
                    message = "Código expirado."
                });
            }

            if (codigoSessao != codigo)
            {
                return Json(new
                {
                    success = false,
                    message = "Código inválido."
                });
            }

            Session["RecuperacaoAutorizada"] = true;

            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        public JsonResult RedefinirSenha(
            string novaSenha,
            string confirmarSenha)
        {
            try
            {
                if (Session["RecuperacaoAutorizada"] == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Recuperação não autorizada."
                    });
                }

                if (string.IsNullOrWhiteSpace(novaSenha))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Informe a nova senha."
                    });
                }

                if (novaSenha != confirmarSenha)
                {
                    return Json(new
                    {
                        success = false,
                        message = "As senhas não coincidem."
                    });
                }

                var email =
                    Session["EmailRecuperacaoSenha"]?.ToString();

                if (string.IsNullOrEmpty(email))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Sessão inválida."
                    });
                }

                var senhaHash =
                    SenhaHelper.HashSenha(novaSenha);

                var cuidador = _context.CuidadoresDeIdoso
                    .FirstOrDefault(x => x.Email == email);

                if (cuidador != null)
                {
                    cuidador.Senha = senhaHash;
                }

                var fisioterapeuta = _context.Fisioterapeutas
                    .FirstOrDefault(x => x.Email == email);

                if (fisioterapeuta != null)
                {
                    fisioterapeuta.Senha = senhaHash;
                }

                var idoso = _context.Idosos
                    .FirstOrDefault(x => x.Email == email);

                if (idoso != null)
                {
                    idoso.Senha = senhaHash;
                }

                var tutor = _context.Tutores
                    .FirstOrDefault(x => x.Email == email);

                if (tutor != null)
                {
                    tutor.Senha = senhaHash;
                }

                _context.SaveChanges();

                Session.Remove("CodigoRecuperacaoSenha");
                Session.Remove("EmailRecuperacaoSenha");
                Session.Remove("RecuperacaoAutorizada");

                return Json(new
                {
                    success = true,
                    message = "Senha alterada com sucesso."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}