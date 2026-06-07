using meuCuidado.Dominio.Models;
using meuCuidado.Dominio.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace meuCuidado.Controllers
{
    public class AdministradorController : Controller
    {
        private readonly MeuCuidadoDbContext _context = new MeuCuidadoDbContext();
        private readonly EmailController _emailController = new EmailController();
        public ActionResult Administrador()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string usuario, string senha)
        {
            if (usuario == "admin" && senha == "admin123")
            {
                return RedirectToAction("Solicitacoes");
            }

            ViewBag.Erro = "Usuário ou senha inválidos.";
            return View("Administrador");
        }

        public ActionResult Solicitacoes()
        {
            var solicitacoes = new List<SolicitacaoProfissionalViewModel>();

            var cuidadores = _context.CuidadoresDeIdoso
                .Where(c => c.EtapaAcesso == EtapaAcesso.AguardandoAprovacao)
                .ToList();

            foreach (var cuidador in cuidadores)
            {
                solicitacoes.Add(new SolicitacaoProfissionalViewModel
                {
                    Id = cuidador.Id,
                    Nome = cuidador.Nome,
                    Email = cuidador.Email,
                    TipoProfissional = "Cuidador de Idoso",
                    DataSolicitacao = cuidador.DataCadasto
                });
            }

            var fisioterapeutas = _context.Fisioterapeutas
                .Where(f => f.EtapaAcesso == EtapaAcesso.AguardandoAprovacao)
                .ToList();

            foreach (var fisio in fisioterapeutas)
            {
                solicitacoes.Add(new SolicitacaoProfissionalViewModel
                {
                    Id = fisio.Id,
                    Nome = fisio.Nome,
                    Email = fisio.Email,
                    TipoProfissional = "Fisioterapeuta",
                    DataSolicitacao = fisio.DataCadasto
                });
            }

            return View(solicitacoes);
        }

        public ActionResult DetalheSolicitacao(int id)
        {
            var cuidador = _context.CuidadoresDeIdoso.FirstOrDefault(c => c.Id == id);
            var fisioterapeuta = _context.Fisioterapeutas.FirstOrDefault(f => f.Id == id);

            if (cuidador == null && fisioterapeuta == null)
                return HttpNotFound();

            string nome;
            string email;
            string telefone;
            string tipoProfissional;
            string cpf;

            if (cuidador != null)
            {
                nome = cuidador.Nome;
                email = cuidador.Email;
                cpf = cuidador.CPF;
                telefone = cuidador.Telefone;
                tipoProfissional = "Cuidador";
            }
            else
            {
                nome = fisioterapeuta.Nome;
                email = fisioterapeuta.Email;
                telefone = fisioterapeuta.Telefone;
                cpf = fisioterapeuta.CPF;
                tipoProfissional = "Fisioterapeuta";
            }

            var documentosBanco = _context.Documentos
                .Where(d => d.UsuarioId == id)
                .ToList();

            var documentos = new List<DocumentoViewModel>();

            foreach (var doc in documentosBanco)
            {
                documentos.Add(new DocumentoViewModel
                {
                    Nome = doc.TipoDocumento.ToString(),
                    Url = "/DocumentosAnalise/" + System.IO.Path.GetFileName(doc.Caminho)
                });
            }

            var model = new SolicitacaoProfissionalDetalheViewModel
            {
                Id = id,
                Nome = nome,
                Email = email,
                Telefone = telefone,
                TipoProfissional = tipoProfissional,
                CPF = cpf,
                Documentos = documentos
            };

            return View("SolicitacoesDetalhadas", model);
        }

        [HttpPost]
        public ActionResult AprovarSolicitacao(int id)
        {
            string email = null;

            var cuidador = _context.CuidadoresDeIdoso.FirstOrDefault(c => c.Id == id);

            if (cuidador != null)
            {
                cuidador.EtapaAcesso = EtapaAcesso.AcessoLiberado;
                cuidador.DataAtualizacao = DateTime.Now;
                email = cuidador.Email;
            }
            else
            {
                var fisio = _context.Fisioterapeutas.FirstOrDefault(f => f.Id == id);

                if (fisio != null)
                {
                    fisio.EtapaAcesso = EtapaAcesso.AcessoLiberado;
                    fisio.DataAtualizacao = DateTime.Now;
                    email = fisio.Email;
                }
            }

            _context.SaveChanges();

            if (!string.IsNullOrEmpty(email))
                _emailController.EnviarResultadoAnaliseCadastro(email, true);

            return RedirectToAction("Solicitacoes");
        }

        [HttpPost]
        public ActionResult ReprovarSolicitacao(int id, string motivo)
        {
            string email = null;

            var cuidador = _context.CuidadoresDeIdoso.FirstOrDefault(c => c.Id == id);

            if (cuidador != null)
            {
                cuidador.EtapaAcesso = EtapaAcesso.AcessoNegado;
                cuidador.DataAtualizacao = DateTime.Now;
                email = cuidador.Email;
            }
            else
            {
                var fisio = _context.Fisioterapeutas.FirstOrDefault(f => f.Id == id);

                if (fisio != null)
                {
                    fisio.EtapaAcesso = EtapaAcesso.AcessoNegado;
                    fisio.DataAtualizacao = DateTime.Now;
                    email = fisio.Email;
                }
            }

            _context.SaveChanges();

            if (!string.IsNullOrEmpty(email))
                _emailController.EnviarResultadoAnaliseCadastro(email, false, motivo);

            return RedirectToAction("Solicitacoes");
        }

    }
}