using meuCuidado.Dominio.Models;
using System;
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
    }
}