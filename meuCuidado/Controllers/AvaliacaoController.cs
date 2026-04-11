using meuCuidado.Dominio.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace meuCuidado.Controllers
{
    public class AvaliacaoController : Controller
    {
        private readonly MeuCuidadoDbContext _context = new MeuCuidadoDbContext();

        [HttpPost]
        public JsonResult Criar(AvaliacaoRequest request)
        {
            try
            {
                var idUsuarioLogado = Convert.ToInt32(Session["IdUsuario"]);
                var tipoUsuarioLogado = Session["TipoUsuario"]?.ToString();

                if (tipoUsuarioLogado != "Tutor" && tipoUsuarioLogado != "Idoso")
                {
                    return Json(new { success = false, message = "Apenas tutores e idosos podem avaliar." });
                }

                if (request == null || request.Nota <= 0)
                {
                    return Json(new { success = false, message = "Dados inválidos" });
                }

                var relacionamento = _context.RelacionamentosIdosoProfissional
                    .FirstOrDefault(r =>
                        r.EtapaAtivacao == EtapaAtivacao.AtivacaoLiberada &&
                        (
                            (r.IdosoId == idUsuarioLogado || r.TutorId == idUsuarioLogado)
                            &&
                            (
                                r.CuidadorId == request.IdAvaliado ||
                                r.FisioterapeutaId == request.IdAvaliado
                            )
                        )
                    );

                if (relacionamento == null)
                {
                    return Json(new { success = false, message = "Você só pode avaliar conexões ativas." });
                }

                var jaAvaliou = _context.Avaliacoes.Any(a =>
                    a.RelacionamentoIdosoProfissionalId == relacionamento.Id &&
                    a.IdentificadorUnico == relacionamento.IdentificadorUnico
                );

                if (jaAvaliou)
                {
                    return Json(new { success = false, message = "Você já avaliou este usuário." });
                }

                var avaliacao = new Avaliacao
                {
                    Nota = request.Nota,
                    Comentario = request.Comentario,
                    RelacionamentoIdosoProfissionalId = relacionamento.Id,
                    IdentificadorUnico = relacionamento.IdentificadorUnico
                };

                _context.Avaliacoes.Add(avaliacao);
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}