using meuCuidado.Dominio.Models;
using meuCuidado.Dominio.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace meuCuidado.Controllers
{
    public class RelacionamentoController : Controller
    {
        private readonly MeuCuidadoDbContext _context = new MeuCuidadoDbContext();
        private readonly EmailController _emailController = new EmailController();

        public ActionResult SolicitarConexao(Guid identificador)
        {
            var tipoUsuarioLogado = Session["TipoUsuario"]?.ToString();
            var idUsuarioLogado = Convert.ToInt32(Session["IdUsuario"]);

            var cuidador = _context.CuidadoresDeIdoso.SingleOrDefault(x => x.IdentificadorUnico == identificador);
            var fisio = _context.Fisioterapeutas.SingleOrDefault(x => x.IdentificadorUnico == identificador);

            if (cuidador == null && fisio == null)
                return HttpNotFound();

            var relacionamento = new RelacionamentoIdosoProfissional
            {
                IdentificadorUnico = Guid.NewGuid(),
                EtapaAtivacao = EtapaAtivacao.AguardandoAprovacao
            };

            if (tipoUsuarioLogado == "Idoso")
                relacionamento.IdosoId = idUsuarioLogado;

            if (tipoUsuarioLogado == "Tutor")
                relacionamento.TutorId = idUsuarioLogado;

            if (cuidador != null)
                relacionamento.CuidadorId = cuidador.Id;

            if (fisio != null)
                relacionamento.FisioterapeutaId = fisio.Id;

            bool jaExiste = false;

            if (tipoUsuarioLogado == "Idoso")
            {
                jaExiste = _context.RelacionamentosIdosoProfissional.Any(r =>
                    r.IdosoId == relacionamento.IdosoId &&
                    (
                        r.CuidadorId == relacionamento.CuidadorId ||
                        r.FisioterapeutaId == relacionamento.FisioterapeutaId
                    )
                );
            }
            else if (tipoUsuarioLogado == "Tutor")
            {
                jaExiste = _context.RelacionamentosIdosoProfissional.Any(r =>
                    r.TutorId == relacionamento.TutorId &&
                    (
                        r.CuidadorId == relacionamento.CuidadorId ||
                        r.FisioterapeutaId == relacionamento.FisioterapeutaId
                    )
                );
            }

            if (!jaExiste)
            {
                _context.RelacionamentosIdosoProfissional.Add(relacionamento);
                _context.SaveChanges();

                string emailProfissional;
                string nomeProfissional;

                if (cuidador != null)
                {
                    emailProfissional = cuidador.Email;
                    nomeProfissional = cuidador.Nome;
                }
                else
                {
                    emailProfissional = fisio.Email;
                    nomeProfissional = fisio.Nome;
                }

                string nomeSolicitante;

                if (tipoUsuarioLogado == "Idoso")
                {
                    nomeSolicitante = _context.Idosos
                        .Where(x => x.Id == idUsuarioLogado)
                        .Select(x => x.Nome)
                        .FirstOrDefault();
                }
                else
                {
                    nomeSolicitante = _context.Tutores
                        .Where(x => x.Id == idUsuarioLogado)
                        .Select(x => x.Nome)
                        .FirstOrDefault();
                }

                _emailController.EnviarSolicitacaoConexao(
                    emailProfissional,
                    nomeProfissional,
                    nomeSolicitante);

                TempData["ToastTipo"] = "sucesso";
                TempData["ToastMensagem"] = "Solicitação de conexão enviada com sucesso!";
            }
            else
            {
                TempData["ToastTipo"] = "info";
                TempData["ToastMensagem"] = "Você já solicitou conexão com este profissional.";
            }

            return RedirectToAction("PerfilDetalhado", "Perfil", new { IdentificadorUnico = identificador });
        }

        public ActionResult Conexoes()
        {
            var idUsuarioLogado = Convert.ToInt32(Session["IdUsuario"]);
            var tipoUsuarioLogado = Session["TipoUsuario"].ToString();

            var query = _context.RelacionamentosIdosoProfissional
                .Include("Cuidador")
                .Include("Fisioterapeuta")
                .Include("Idoso")
                .Include("Tutor");

            List<ConexaoViewModel> lista;

            if (tipoUsuarioLogado == "Idoso" || tipoUsuarioLogado == "Tutor")
            {
                lista = query
                    .Where(r => r.IdosoId == idUsuarioLogado || r.TutorId == idUsuarioLogado)
                    .Select(r => new ConexaoViewModel
                    {
                        Id = r.Id,

                        Nome = r.Cuidador != null
                            ? r.Cuidador.Nome
                            : r.Fisioterapeuta.Nome,

                        Tipo = r.Cuidador != null
                            ? "Cuidador"
                            : "Fisioterapeuta",

                        IdentificadorUnico = r.Cuidador != null
                            ? r.Cuidador.IdentificadorUnico
                            : r.Fisioterapeuta.IdentificadorUnico,

                        EtapaAtivacao = r.EtapaAtivacao
                    })
                    .ToList();
            }
            else if (tipoUsuarioLogado == "Cuidador")
            {
                lista = query
                    .Where(r => r.CuidadorId == idUsuarioLogado)
                    .Select(r => new ConexaoViewModel
                    {
                        Id = r.Id,

                        Nome = r.Idoso != null
                            ? r.Idoso.Nome
                            : r.Tutor.Nome,

                        Tipo = r.Idoso != null
                            ? "Idoso"
                            : "Tutor",

                        IdentificadorUnico = r.Idoso != null
                            ? r.Idoso.IdentificadorUnico
                            : r.Tutor.IdentificadorUnico,

                        EtapaAtivacao = r.EtapaAtivacao
                    })
                    .ToList();
            }
            else 
            {
                lista = query
                    .Where(r => r.FisioterapeutaId == idUsuarioLogado)
                    .Select(r => new ConexaoViewModel
                    {
                        Id = r.Id,

                        Nome = r.Idoso != null
                            ? r.Idoso.Nome
                            : r.Tutor.Nome,

                        Tipo = r.Idoso != null
                            ? "Idoso"
                            : "Tutor",

                        IdentificadorUnico = r.Idoso != null
                            ? r.Idoso.IdentificadorUnico
                            : r.Tutor.IdentificadorUnico,

                        EtapaAtivacao = r.EtapaAtivacao
                    })
                    .ToList();
            }

            ViewBag.TipoUsuario = tipoUsuarioLogado;

            return PartialView("_Conexoes", lista);
        }

        [HttpGet]
        public JsonResult ObterQuantidadePendentes()
        {
            var idUsuario = Convert.ToInt32(Session["IdUsuario"]);
            var tipoUsuario = Session["TipoUsuario"]?.ToString();

            int quantidade = 0;

            if (tipoUsuario == "Cuidador")
            {
                quantidade = _context.RelacionamentosIdosoProfissional.Count(r =>
                    r.CuidadorId == idUsuario &&
                    r.EtapaAtivacao == EtapaAtivacao.AguardandoAprovacao);
            }
            else if (tipoUsuario == "Fisioterapeuta")
            {
                quantidade = _context.RelacionamentosIdosoProfissional.Count(r =>
                    r.FisioterapeutaId == idUsuario &&
                    r.EtapaAtivacao == EtapaAtivacao.AguardandoAprovacao);
            }
            else if (tipoUsuario == "Idoso")
            {
                quantidade = _context.RelacionamentosIdosoProfissional.Count(r =>
                    r.IdosoId == idUsuario &&
                    r.EtapaAtivacao == EtapaAtivacao.AguardandoAprovacao);
            }
            else if (tipoUsuario == "Tutor")
            {
                quantidade = _context.RelacionamentosIdosoProfissional.Count(r =>
                    r.TutorId == idUsuario &&
                    r.EtapaAtivacao == EtapaAtivacao.AguardandoAprovacao);
            }

            return Json(new { quantidade }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Aceitar(int id)
        {
            var tipoUsuario = Session["TipoUsuario"]?.ToString();
            var idUsuario = Convert.ToInt32(Session["IdUsuario"]);

            var rel = _context.RelacionamentosIdosoProfissional
                .Include("Idoso")
                .Include("Tutor")
                .Include("Cuidador")
                .Include("Fisioterapeuta")
                .FirstOrDefault(r => r.Id == id);

            if (rel == null)
                return Json(new { success = false, message = "Relacionamento não encontrado" });

            var podeAceitar =
                (tipoUsuario == "Cuidador" && rel.CuidadorId == idUsuario) ||
                (tipoUsuario == "Fisioterapeuta" && rel.FisioterapeutaId == idUsuario);

            if (!podeAceitar)
                return Json(new { success = false, message = "Sem permissão" });

            rel.EtapaAtivacao = EtapaAtivacao.AtivacaoLiberada;

            _context.SaveChanges();

            string emailSolicitante =
                rel.Idoso != null
                    ? rel.Idoso.Email
                    : rel.Tutor.Email;

            string nomeProfissional =
                rel.Cuidador != null
                    ? rel.Cuidador.Nome
                    : rel.Fisioterapeuta.Nome;

            _emailController.EnviarConexaoAprovada(
                emailSolicitante,
                nomeProfissional);

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult Rejeitar(int id)
        {
            var tipoUsuario = Session["TipoUsuario"]?.ToString();
            var idUsuario = Convert.ToInt32(Session["IdUsuario"]);

            var rel = _context.RelacionamentosIdosoProfissional
                .Include("Idoso")
                .Include("Tutor")
                .Include("Cuidador")
                .Include("Fisioterapeuta")
                .FirstOrDefault(r => r.Id == id);

            if (rel == null)
                return Json(new { success = false, message = "Relacionamento não encontrado" });

            var podeRejeitar =
                (tipoUsuario == "Cuidador" && rel.CuidadorId == idUsuario) ||
                (tipoUsuario == "Fisioterapeuta" && rel.FisioterapeutaId == idUsuario);

            if (!podeRejeitar)
                return Json(new { success = false, message = "Sem permissão" });

            rel.EtapaAtivacao = EtapaAtivacao.AcessoNegado;

            _context.SaveChanges();

            string emailSolicitante =
                rel.Idoso != null
                    ? rel.Idoso.Email
                    : rel.Tutor.Email;

            string nomeProfissional =
                rel.Cuidador != null
                    ? rel.Cuidador.Nome
                    : rel.Fisioterapeuta.Nome;

            _emailController.EnviarConexaoRejeitada(
                emailSolicitante,
                nomeProfissional);

            return Json(new { success = true });
        }
    }
}