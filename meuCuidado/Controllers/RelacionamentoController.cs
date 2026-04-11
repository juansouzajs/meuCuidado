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

            var jaExiste = _context.RelacionamentosIdosoProfissional.Any(r =>
                (r.IdosoId == relacionamento.IdosoId || r.TutorId == relacionamento.TutorId) &&
                (r.CuidadorId == relacionamento.CuidadorId || r.FisioterapeutaId == relacionamento.FisioterapeutaId)
            );

            if (!jaExiste)
            {
                _context.RelacionamentosIdosoProfissional.Add(relacionamento);
                _context.SaveChanges();

                TempData["Sucesso"] = "Solicitação de conexão enviada com sucesso!";
            }
            else
            {
                TempData["Sucesso"] = "Você já solicitou conexão com este profissional.";
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
            else 
            {
                lista = query
                    .Where(r => r.CuidadorId == idUsuarioLogado || r.FisioterapeutaId == idUsuarioLogado)
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

        [HttpPost]
        public JsonResult Aceitar(int id)
        {
            var tipoUsuario = Session["TipoUsuario"]?.ToString();
            var idUsuario = Convert.ToInt32(Session["IdUsuario"]);

            var rel = _context.RelacionamentosIdosoProfissional
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

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult Rejeitar(int id)
        {
            var tipoUsuario = Session["TipoUsuario"]?.ToString();
            var idUsuario = Convert.ToInt32(Session["IdUsuario"]);

            var rel = _context.RelacionamentosIdosoProfissional
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

            return Json(new { success = true });
        }
    }
}