using meuCuidado.Dominio.Models;
using meuCuidado.Dominio.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static meuCuidado.Dominio.Extensions.EnumExtension;

namespace meuCuidado.Controllers
{
    public class CadastroController : Controller
    {
        private readonly MeuCuidadoDbContext _context = new MeuCuidadoDbContext();

        // Tela de Cadastro
        public ActionResult Cadastro()
        {
            return View();
        }

        // Método para realizar cadastro
        [HttpPost]
        public ActionResult Cadastro(CadastroViewModel pessoa)
        {
            if (ModelState.IsValid)
            {
                if (pessoa.TipoUsuario == TipoUsuario.Idoso)
                {
                    // MODEL PARA CADASTRO DE IDOSO
                    Idoso idoso = new Idoso
                    {
                        IdentificadorUnico = Guid.NewGuid(),
                        Nome = pessoa.Usuario.Nome,
                        Email = pessoa.Usuario.Email.ToLower(),
                        CPF = pessoa.Usuario.CPF,
                        Endereco = pessoa.Usuario.Endereco,
                        Telefone = pessoa.Usuario.Telefone,
                        Senha = SenhaHelper.HashSenha(pessoa.Usuario.Senha),
                        DataCadasto = DateTime.Now,
                        DataNascimento = DateTime.Now,
                        NecessidadesEspeciais = false
                    };

                    Session["Idoso"] = JsonConvert.SerializeObject(idoso);

                    return RedirectToAction("CadastroTutorEMedicoDoIdoso");
                }
                else if (pessoa.TipoUsuario == TipoUsuario.Tutor)
                {
                    // MODEL PARA CADASTRO DE TUTOR
                    Tutor tutor = new Tutor
                    {
                        IdentificadorUnico = new Guid(),
                        Nome = pessoa.Usuario.Nome,
                        Email = pessoa.Usuario.Email.ToLower(),
                        CPF = pessoa.Usuario.CPF,
                        Endereco = pessoa.Usuario.Endereco,
                        Telefone = pessoa.Usuario.Telefone,
                        Senha = SenhaHelper.HashSenha(pessoa.Usuario.Senha),
                        DataCadasto = DateTime.Now,
                        RelacaoComIdoso = "Tutor",
                        NecessidadesEspeciais = false
                    };

                    _context.Tutores.Add(tutor);
                    _context.SaveChanges();
                }
                else
                    return CadastroProfissional(pessoa);

                return RedirectToAction("Login", "Login");
            }
            else
                return View(pessoa);
        }

        public ActionResult CadastroTutorEMedicoDoIdoso()
        {
            // Desserializa os dados do idoso
            var idoso = JsonConvert.DeserializeObject<Idoso>(Session["Idoso"]?.ToString());

            if (idoso == null)
                return RedirectToAction("Cadastro");

            // Cria um ViewModel para o cadastro de Tutor e Médico
            var viewModel = new CadastroTutorEMedicoDoIdosoViewModel
            {
                Idoso = idoso,
                Tutor = new Tutor(),
                Medicos = new List<Medico>() // Inicializa a lista de médicos
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SalvarCadastroTutorEMedico(CadastroTutorEMedicoDoIdosoViewModel viewModel)
        {
            try
            {
                // Desserializa os dados do idoso
                var idoso = JsonConvert.DeserializeObject<Idoso>(Session["Idoso"]?.ToString());

                if (idoso == null)
                    return RedirectToAction("Cadastro");

                viewModel.Idoso = idoso;

                if (viewModel.Tutor != null)
                {
                    var tutor = viewModel.Tutor;
                    tutor.IdentificadorUnico = Guid.NewGuid();
                    tutor.DataCadasto = DateTime.Now;
                    tutor.Endereco = idoso.Endereco;
                    tutor.Telefone = idoso.Telefone;
                    _context.Tutores.Add(tutor);
                    _context.SaveChanges();
                    idoso.TutorId = tutor.Id;
                }
                else
                    throw new Exception("Os dados do Tutor não foram preenchidos.");

                _context.Idosos.Add(idoso);
                _context.SaveChanges();

                // Adiciona o médico (caso tenha sido inserido)

                foreach (var medico in viewModel.Medicos)
                {
                    medico.IdosoId = idoso.Id;
                    medico.IdentificadorUnico = Guid.NewGuid();
                    medico.DataCadasto = DateTime.Now;
                    medico.Endereco = idoso.Endereco;
                    medico.Telefone = idoso.Telefone;
                    medico.Email = idoso.Email.ToLower();
                    _context.Medicos.Add(medico);
                }

                _context.SaveChanges();

                return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult CadastroProfissional(CadastroViewModel pessoa)
        {
            return View("CadastroProfissional", pessoa);
        }

        [HttpPost]
        public ActionResult CadastroProfissional(CadastroViewModel cadastroProfissionalViewModel, HttpPostedFileBase FotoDocumento, HttpPostedFileBase Documento, HttpPostedFileBase CertificadoBonsAntecedentes, HttpPostedFileBase CertificadoDispensa)
        {
            if (ModelState.IsValid)
            {
                int? idUsuario = null;

                if (cadastroProfissionalViewModel.TipoUsuario == TipoUsuario.Cuidador)
                {
                    CuidadorDeIdoso cuidadorDeIdoso = new CuidadorDeIdoso
                    {
                        IdentificadorUnico = Guid.NewGuid(),
                        Nome = cadastroProfissionalViewModel.Usuario.Nome,
                        Email = cadastroProfissionalViewModel.Usuario.Email.ToLower(),
                        CPF = cadastroProfissionalViewModel.Usuario.CPF,
                        Endereco = cadastroProfissionalViewModel.Usuario.Endereco,
                        Telefone = cadastroProfissionalViewModel.Usuario.Telefone,
                        Senha = SenhaHelper.HashSenha(cadastroProfissionalViewModel.Usuario.Senha),
                        EtapaAcesso = EtapaAcesso.AguardandoAprovacao,
                        DataCadasto = DateTime.Now
                    };

                    _context.CuidadoresDeIdoso.Add(cuidadorDeIdoso);
                    _context.SaveChanges();
                    idUsuario = cuidadorDeIdoso.Id;
                }
                else if (cadastroProfissionalViewModel.TipoUsuario == TipoUsuario.Fisioterapeuta)
                {
                    Fisioterapeuta fisioterapeuta = new Fisioterapeuta
                    {
                        IdentificadorUnico = new Guid(),
                        Nome = cadastroProfissionalViewModel.Usuario.Nome,
                        Email = cadastroProfissionalViewModel.Usuario.Email.ToLower(),
                        CPF = cadastroProfissionalViewModel.Usuario.CPF,
                        Endereco = cadastroProfissionalViewModel.Usuario.Endereco,
                        Telefone = cadastroProfissionalViewModel.Usuario.Telefone,
                        Senha = SenhaHelper.HashSenha(cadastroProfissionalViewModel.Usuario.Senha),
                        EtapaAcesso = EtapaAcesso.AguardandoAprovacao,
                        DataCadasto = DateTime.Now
                    };

                    _context.Fisioterapeutas.Add(fisioterapeuta);
                    _context.SaveChanges();
                    idUsuario = fisioterapeuta.Id;
                }

                if (idUsuario != null && idUsuario != 0)
                {
                    SalvarDocumento(FotoDocumento, TipoDocumento.FotoDocumento, idUsuario.Value);
                    SalvarDocumento(Documento, TipoDocumento.Documento, idUsuario.Value);
                    SalvarDocumento(CertificadoBonsAntecedentes, TipoDocumento.CertificadoBonsAntecedentes, idUsuario.Value);
                    SalvarDocumento(CertificadoDispensa, TipoDocumento.CertificadoDispensa, idUsuario.Value);
                }

                return RedirectToAction("AguardandoAprovacao");
            }

            // Pega os erros da model
            var errors = GetModelErrors();

            // Aqui você pode fazer algo com os erros, como logar ou enviar para a view
            ViewBag.Errors = errors;

            return View(cadastroProfissionalViewModel);
        }


        private void SalvarDocumento(HttpPostedFileBase arquivo, TipoDocumento tipoDocumento, int usuarioId)
        {
            if (arquivo != null && arquivo.ContentLength > 0)
            {
                // Verificar a extensão do arquivo
                var extensao = Path.GetExtension(arquivo.FileName).ToLower();
                TipoExtensaoDocumento tipoExtensao;

                switch (extensao)
                {
                    case ".jpg":
                    case ".jpeg":
                        tipoExtensao = TipoExtensaoDocumento.JPG;
                        break;
                    case ".png":
                        tipoExtensao = TipoExtensaoDocumento.PNG;
                        break;
                    case ".pdf":
                        tipoExtensao = TipoExtensaoDocumento.PDF;
                        break;
                    case ".docx":
                        tipoExtensao = TipoExtensaoDocumento.DOCX;
                        break;
                    case ".xlsx":
                        tipoExtensao = TipoExtensaoDocumento.XLSX;
                        break;
                    default:
                        throw new InvalidOperationException("Tipo de arquivo não suportado.");
                }

                // Caminho onde o arquivo será salvo
                var caminho = Server.MapPath("~/DocumentosAnalise/");

                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                var nomeArquivo = Guid.NewGuid() + extensao;
                var caminhoCompleto = Path.Combine(caminho, nomeArquivo);

                // Salvar o arquivo no servidor
                arquivo.SaveAs(caminhoCompleto);

                // Criar a instância do documento e associá-lo ao usuário
                var documento = new Documento
                {
                    Id = Guid.NewGuid(),
                    TipoDocumento = tipoDocumento,
                    Extensao = tipoExtensao,
                    Caminho = caminhoCompleto,
                    UsuarioId = usuarioId
                };

                _context.Documentos.Add(documento);
                _context.SaveChanges();
            }
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

        public ActionResult AguardandoAprovacao()
        {
            return View();
        }

        [HttpPost]
        public JsonResult VerificarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Json(new { existe = false });

            var emailNormalizado = email.ToLower();

            bool existe =
                _context.CuidadoresDeIdoso.Any(p => p.Email.ToLower() == emailNormalizado && p.EtapaAcesso == EtapaAcesso.AcessoLiberado) ||
                _context.Fisioterapeutas.Any(p => p.Email.ToLower() == emailNormalizado && p.EtapaAcesso == EtapaAcesso.AcessoLiberado) ||
                _context.Idosos.Any(p => p.Email.ToLower() == emailNormalizado && p.EtapaAcesso == EtapaAcesso.AcessoLiberado) ||
                _context.Tutores.Any(p => p.Email.ToLower() == emailNormalizado && p.EtapaAcesso == EtapaAcesso.AcessoLiberado);

            return Json(new
            {
                existe = existe
            });
        }
    }
}