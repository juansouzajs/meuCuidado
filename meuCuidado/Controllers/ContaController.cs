using System.Data.Entity;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using meuCuidado.Dominio.Models;
using System.Linq;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using static meuCuidado.Dominio.Extensions.EnumExtension;
using System.Collections.Generic;
using System;
using System.IO;
using meuCuidado.Dominio.ViewModels;

namespace meuCuidado.Controllers
{
    public class ContaController : Controller
    {
        private readonly MeuCuidadoDbContext _context = new MeuCuidadoDbContext();
        private readonly EmailController _emailController = new EmailController();

        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult Cadastro()
        {
            return View();
        }

        public ActionResult CadastroProfissional()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CadastroProfissional(CadastroProfissionalViewModel cadastroProfissionalViewModel, HttpPostedFileBase FotoDocumento, HttpPostedFileBase Documento, HttpPostedFileBase CertificadoBonsAntecedentes, HttpPostedFileBase CertificadoDispensa)
        {
            if (ModelState.IsValid)
            {
                if (FotoDocumento != null && FotoDocumento.ContentLength > 0)
                {
                    var caminhoFotoDocumento = Server.MapPath("~/DocumentosAnalise/FotosDocumento/");
                    if (!Directory.Exists(caminhoFotoDocumento))
                        Directory.CreateDirectory(caminhoFotoDocumento);

                    var nomeArquivoFoto = Guid.NewGuid() + Path.GetExtension(FotoDocumento.FileName);
                    FotoDocumento.SaveAs(caminhoFotoDocumento + nomeArquivoFoto);
                }

                if (Documento != null && Documento.ContentLength > 0)
                {
                    var caminhoDocumento = Server.MapPath("~/DocumentosAnalise/Documentos/");
                    if (!Directory.Exists(caminhoDocumento))
                        Directory.CreateDirectory(caminhoDocumento);

                    var nomeArquivoDocumento = Guid.NewGuid() + Path.GetExtension(Documento.FileName);
                    Documento.SaveAs(caminhoDocumento + nomeArquivoDocumento);
                }

                if (CertificadoBonsAntecedentes != null && CertificadoBonsAntecedentes.ContentLength > 0)
                {
                    var caminhoCertificadoBonsAntecedentes = Server.MapPath("~/DocumentosAnalise/CertificadosBonsAntecedentes/");
                    if (!Directory.Exists(caminhoCertificadoBonsAntecedentes))
                        Directory.CreateDirectory(caminhoCertificadoBonsAntecedentes);

                    var nomeArquivoCertificado = Guid.NewGuid() + Path.GetExtension(CertificadoBonsAntecedentes.FileName);
                    CertificadoBonsAntecedentes.SaveAs(caminhoCertificadoBonsAntecedentes + nomeArquivoCertificado);
                }

                if (CertificadoDispensa != null && CertificadoDispensa.ContentLength > 0)
                {
                    var caminhoCertificadoDispensa = Server.MapPath("~/DocumentosAnalise/CertificadosDispensa/");
                    if (!Directory.Exists(caminhoCertificadoDispensa))
                        Directory.CreateDirectory(caminhoCertificadoDispensa);

                    var nomeArquivoDispensa = Guid.NewGuid() + Path.GetExtension(CertificadoDispensa.FileName);
                    CertificadoDispensa.SaveAs(caminhoCertificadoDispensa + nomeArquivoDispensa);
                }

                return RedirectToAction("Dashboard");
            }

            var errors = GetModelErrors();

            ViewBag.Errors = errors;

            return View(cadastroProfissionalViewModel);
        }

        private List<string> GetModelErrors()
        {
            var errors = new List<string>();

            foreach (var modelState in ModelState)
            {
                foreach (var error in modelState.Value.Errors)
                {
                    errors.Add($"{modelState.Key}: {error.ErrorMessage}");
                }
            }

            return errors;
        }

        public ActionResult Dashboard()
        {
            var usuarioLogado = User.Identity.Name;

            return View();
        }

        public ActionResult Perfil(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string senha, string returnUrl)
        {
            var cuidadorDeIdoso = _context.CuidadoresDeIdoso.FirstOrDefault(p => p.Email == email && p.Senha == senha);
            var fisioterapeuta = _context.Fisioterapeutas.FirstOrDefault(p => p.Email == email && p.Senha == senha);
            var idoso = _context.Idosos.FirstOrDefault(p => p.Email == email && p.Senha == senha);
            var tutor = _context.Tutores.FirstOrDefault(p => p.Email == email && p.Senha == senha);

            if (cuidadorDeIdoso != null || fisioterapeuta != null || idoso != null || tutor != null)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, email) };
                var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                var authManager = HttpContext.GetOwinContext().Authentication;
                authManager.SignIn(identity);

                var codigoAutenticacao = _emailController.EnviarEmailAutenticacao(email);
                Session["CodigoAutenticacao"] = codigoAutenticacao;

                ViewBag.ShowPopup = true;
                ViewBag.PopupMessage = "Código de autenticação enviado para seu e-mail.";

                var autenticacaoViewModel = new AutenticacaoViewModel
                {
                    Codigo1 = string.Empty,
                    Codigo2 = string.Empty,
                    Codigo3 = string.Empty,
                    Codigo4 = string.Empty,
                    Codigo5 = string.Empty,
                    Email = email
                };

                return View(autenticacaoViewModel);
            }

            ViewBag.ErrorMessage = "Usuário ou senha inválidos.";
            return View();
        }

        [HttpPost]
        public ActionResult ValidarCodigoAutenticacao(AutenticacaoViewModel model)
        {
            var codigoInserido = $"{model.Codigo1}{model.Codigo2}{model.Codigo3}{model.Codigo4}{model.Codigo5}";
            var codigoCorreto = Session["CodigoAutenticacao"].ToString();

            if (codigoInserido == codigoCorreto)
                return Json(new { success = true, redirectUrl = Url.Action("Dashboard", "Conta") });

            return Json(new { success = false, message = "Código de autenticação inválido." });
        }


        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public ActionResult Cadastro(CadastroViewModel pessoa)
        {
            if (ModelState.IsValid)
            {
                if (pessoa.TipoUsuario == TipoUsuario.Idoso)
                {
                    Idoso idoso = new Idoso
                    {
                        IdentificadorUnico = Guid.NewGuid(),
                        Nome = pessoa.Usuario.Nome,
                        Email = pessoa.Usuario.Email,
                        CPF = pessoa.Usuario.CPF,
                        Endereco = pessoa.Usuario.Endereco,
                        Telefone = pessoa.Usuario.Telefone,
                        Senha = pessoa.Usuario.Senha,
                        DataCadasto = DateTime.Now,
                        DataNascimento = DateTime.Now,
                        NecessidadesEspeciais = false
                    };

                    _context.Idosos.Add(idoso);
                    _context.SaveChanges();
                }
                else if (pessoa.TipoUsuario == TipoUsuario.Tutor)
                {
                    Tutor tutor = new Tutor
                    {
                        IdentificadorUnico = new Guid(),
                        Nome = pessoa.Usuario.Nome,
                        Email = pessoa.Usuario.Email,
                        CPF = pessoa.Usuario.CPF,
                        Endereco = pessoa.Usuario.Endereco,
                        Telefone = pessoa.Usuario.Telefone,
                        Senha = pessoa.Usuario.Senha,
                        DataCadasto = DateTime.Now,
                        RelacaoComIdoso = "Tutor",
                        NecessidadesEspeciais = false
                    };

                    _context.Tutores.Add(tutor);
                    _context.SaveChanges();
                }
                else
                {
                    return RedirectToAction("CadastroProfissional");
                }

                return RedirectToAction("Login");
            }

            return View();
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
            {
                return RedirectToAction("Login");
            }

            return RedirectToAction("Login");
        }
    }
}
