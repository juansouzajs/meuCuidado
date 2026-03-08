using meuCuidado.Dominio.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace meuCuidado.Controllers
{
    public class AdministradorController : Controller
    {
        public ActionResult Administrador() { return View(); }

        // LISTA DE SOLICITAÇÕES
        public ActionResult Solicitacoes()
        {
            // Aqui normalmente viria do banco
            var solicitacoes = new List<SolicitacaoProfissionalViewModel>
            {
                new SolicitacaoProfissionalViewModel
                {
                    Id = 1,
                    Nome = "João Silva",
                    Email = "joao@email.com",
                    TipoProfissional = "Cuidador",
                    DataSolicitacao = DateTime.Now.AddDays(-1)
                }
            };

            return View(solicitacoes);
        }

        // DETALHE DA SOLICITAÇÃO
        public ActionResult DetalheSolicitacao(int id)
        {
            // Simulação (normalmente vem do banco)
            var model = new SolicitacaoProfissionalDetalheViewModel
            {
                Id = id,
                Nome = "João Silva",
                Email = "joao@email.com",
                TipoProfissional = "Cuidador",
                Telefone = "(11) 99999-0000",
                Documentos = new List<DocumentoViewModel>
                {
                    new DocumentoViewModel
                    {
                        Nome = "RG.pdf",
                        Url = "/uploads/rg.pdf"
                    },
                    new DocumentoViewModel
                    {
                        Nome = "Certificado.pdf",
                        Url = "/uploads/certificado.pdf"
                    }
                }
            };

            return View(model);
        }

        // APROVAR
        [HttpPost]
        public ActionResult AprovarSolicitacao(int id)
        {
            // lógica de aprovação no banco

            TempData["Sucesso"] = "Solicitação aprovada com sucesso.";

            return RedirectToAction("Solicitacoes");
        }

        // REPROVAR
        [HttpPost]
        public ActionResult ReprovarSolicitacao(int id)
        {
            // lógica de reprovação no banco

            TempData["Erro"] = "Solicitação reprovada.";

            return RedirectToAction("Solicitacoes");
        }
    }
}