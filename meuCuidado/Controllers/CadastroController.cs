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
            if (!ModelState.IsValid)
                return View(pessoa);

            if (pessoa.TipoUsuario == TipoUsuario.Idoso)
            {
                var idoso = new Idoso
                {
                    IdentificadorUnico = Guid.NewGuid(),
                    Nome = pessoa.Usuario.Nome,
                    Email = pessoa.Usuario.Email.ToLower(),
                    CPF = pessoa.Usuario.CPF,
                    Endereco = pessoa.Usuario.Endereco,
                    Telefone = pessoa.Usuario.Telefone,
                    Senha = SenhaHelper.HashSenha(pessoa.Usuario.Senha),
                    DataCadasto = DateTime.Now,

                    // NÃO USA DateTime.MinValue
                    DataNascimento = new DateTime(2000, 1, 1),

                    NecessidadesEspeciais = false
                };

                Session["Idoso"] = JsonConvert.SerializeObject(idoso);

                return RedirectToAction("CadastroTutorEMedicoDoIdoso");
            }

            if (pessoa.TipoUsuario == TipoUsuario.Tutor)
            {
                var tutor = new Tutor
                {
                    IdentificadorUnico = Guid.NewGuid(),
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

                return RedirectToAction("Login", "Login");
            }

            return CadastroProfissional(pessoa);
        }

        public ActionResult CadastroTutorEMedicoDoIdoso()
        {
            var json = Session["Idoso"]?.ToString();

            if (string.IsNullOrEmpty(json))
                return RedirectToAction("Cadastro");

            var idoso =
                JsonConvert.DeserializeObject<Idoso>(json);

            var vm =
                new CadastroTutorEMedicoDoIdosoViewModel
                {
                    Idoso = idoso,
                    Tutor = new Tutor(),
                    Medicos = new List<Medico>
                    {
                new Medico()
                    }
                };

            return View(vm);
        }

        [HttpPost]
        public ActionResult SalvarCadastroTutorEMedico(
     CadastroTutorEMedicoDoIdosoViewModel viewModel)
        {
            try
            {
                var jsonIdoso = Session["Idoso"]?.ToString();

                if (string.IsNullOrEmpty(jsonIdoso))
                    return RedirectToAction("Cadastro");

                var idoso =
                    JsonConvert.DeserializeObject<Idoso>(jsonIdoso);

                viewModel.Idoso = idoso;

                // ==========================
                // VALIDAÇÃO DO TUTOR
                // ==========================

                if (viewModel.Tutor == null ||
                    string.IsNullOrWhiteSpace(viewModel.Tutor.Nome) ||
                    string.IsNullOrWhiteSpace(viewModel.Tutor.Email) ||
                    string.IsNullOrWhiteSpace(viewModel.Tutor.CPF) ||
                    string.IsNullOrWhiteSpace(viewModel.Tutor.RelacaoComIdoso))
                {
                    ModelState.AddModelError(
                        "",
                        "Preencha todos os dados do tutor.");

                    return View(
                        "CadastroTutorEMedicoDoIdoso",
                        viewModel);
                }

                // ==========================
                // VALIDAÇÃO DOS MÉDICOS
                // ==========================

                if (viewModel.Medicos == null ||
                    !viewModel.Medicos.Any(x =>
                        !string.IsNullOrWhiteSpace(x.Nome)))
                {
                    ModelState.AddModelError(
                        "",
                        "Informe pelo menos um médico de referência.");

                    return View(
                        "CadastroTutorEMedicoDoIdoso",
                        viewModel);
                }

                // Remove médicos vazios
                viewModel.Medicos = viewModel.Medicos
                    .Where(x => !string.IsNullOrWhiteSpace(x.Nome))
                    .ToList();

                // ==========================
                // SALVA TUTOR
                // ==========================

                var tutor = viewModel.Tutor;

                tutor.IdentificadorUnico = Guid.NewGuid();
                tutor.DataCadasto = DateTime.Now;

                tutor.Endereco = idoso.Endereco;
                tutor.Telefone = idoso.Telefone;

                _context.Tutores.Add(tutor);
                _context.SaveChanges();

                idoso.TutorId = tutor.Id;

                // ==========================
                // SALVA IDOSO
                // ==========================

                _context.Idosos.Add(idoso);
                _context.SaveChanges();

                // ==========================
                // SALVA MÉDICOS
                // ==========================

                foreach (var medico in viewModel.Medicos)
                {
                    medico.IdosoId = idoso.Id;

                    medico.IdentificadorUnico =
                        Guid.NewGuid();

                    medico.DataCadasto =
                        DateTime.Now;

                    medico.Endereco =
                        idoso.Endereco;

                    medico.Telefone =
                        idoso.Telefone;

                    medico.Email =
                        idoso.Email.ToLower();

                    _context.Medicos.Add(medico);
                }

                _context.SaveChanges();

                // Limpa a sessão
                Session.Remove("Idoso");

                return RedirectToAction(
                    "Login",
                    "Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    "Ocorreu um erro ao finalizar o cadastro.");

                var jsonIdoso = Session["Idoso"]?.ToString();

                if (!string.IsNullOrEmpty(jsonIdoso))
                {
                    viewModel.Idoso =
                        JsonConvert.DeserializeObject<Idoso>(
                            jsonIdoso);
                }

                return View(
                    "CadastroTutorEMedicoDoIdoso",
                    viewModel);
            }
        }

        public ActionResult CadastroProfissional(CadastroViewModel pessoa)
        {
            return View("CadastroProfissional", pessoa);
        }

        [HttpPost]
        public ActionResult CadastroProfissional(
            CadastroViewModel cadastroProfissionalViewModel,
            HttpPostedFileBase FotoDocumento,
            HttpPostedFileBase Documento,
            HttpPostedFileBase CertificadoBonsAntecedentes,
            HttpPostedFileBase CertificadoDispensa)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = GetModelErrors();
                return View(cadastroProfissionalViewModel);
            }

            int? idUsuario = null;

            var email = cadastroProfissionalViewModel.Usuario.Email.ToLower();
            var cpf = cadastroProfissionalViewModel.Usuario.CPF;

            if (cadastroProfissionalViewModel.TipoUsuario == TipoUsuario.Cuidador)
            {
                var cuidadorExistente = _context.CuidadoresDeIdoso
                    .FirstOrDefault(x =>
                        x.Email.ToLower() == email ||
                        x.CPF == cpf);

                // APROVADO = BLOQUEIA
                if (cuidadorExistente != null &&
                    cuidadorExistente.EtapaAcesso == EtapaAcesso.AcessoLiberado)
                {
                    ModelState.AddModelError("", "Usuário já cadastrado.");
                    return View(cadastroProfissionalViewModel);
                }

                // REPROVADO = REAPROVEITA
                if (cuidadorExistente != null &&
                    cuidadorExistente.EtapaAcesso == EtapaAcesso.AcessoNegado)
                {
                    cuidadorExistente.Nome =
                        cadastroProfissionalViewModel.Usuario.Nome;

                    cuidadorExistente.Email = email;
                    cuidadorExistente.CPF = cpf;

                    cuidadorExistente.Endereco =
                        cadastroProfissionalViewModel.Usuario.Endereco;

                    cuidadorExistente.Telefone =
                        cadastroProfissionalViewModel.Usuario.Telefone;

                    cuidadorExistente.Senha =
                        SenhaHelper.HashSenha(
                            cadastroProfissionalViewModel.Usuario.Senha);

                    cuidadorExistente.EtapaAcesso =
                        EtapaAcesso.AguardandoAprovacao;

                    cuidadorExistente.DataCadasto = DateTime.Now;

                    _context.SaveChanges();

                    idUsuario = cuidadorExistente.Id;
                }
                else
                {
                    var cuidadorDeIdoso = new CuidadorDeIdoso
                    {
                        IdentificadorUnico = Guid.NewGuid(),
                        Nome = cadastroProfissionalViewModel.Usuario.Nome,
                        Email = email,
                        CPF = cpf,
                        Endereco = cadastroProfissionalViewModel.Usuario.Endereco,
                        Telefone = cadastroProfissionalViewModel.Usuario.Telefone,
                        Senha = SenhaHelper.HashSenha(
                            cadastroProfissionalViewModel.Usuario.Senha),
                        EtapaAcesso = EtapaAcesso.AguardandoAprovacao,
                        DataCadasto = DateTime.Now
                    };

                    _context.CuidadoresDeIdoso.Add(cuidadorDeIdoso);
                    _context.SaveChanges();

                    idUsuario = cuidadorDeIdoso.Id;
                }
            }
            else if (cadastroProfissionalViewModel.TipoUsuario == TipoUsuario.Fisioterapeuta)
            {
                var fisioterapeutaExistente = _context.Fisioterapeutas
                    .FirstOrDefault(x =>
                        x.Email.ToLower() == email ||
                        x.CPF == cpf);

                // APROVADO = BLOQUEIA
                if (fisioterapeutaExistente != null &&
                    fisioterapeutaExistente.EtapaAcesso == EtapaAcesso.AcessoLiberado)
                {
                    ModelState.AddModelError("", "Usuário já cadastrado.");
                    return View(cadastroProfissionalViewModel);
                }

                // REPROVADO = REAPROVEITA
                if (fisioterapeutaExistente != null &&
                    fisioterapeutaExistente.EtapaAcesso == EtapaAcesso.AcessoNegado)
                {
                    fisioterapeutaExistente.Nome =
                        cadastroProfissionalViewModel.Usuario.Nome;

                    fisioterapeutaExistente.Email = email;
                    fisioterapeutaExistente.CPF = cpf;

                    fisioterapeutaExistente.Endereco =
                        cadastroProfissionalViewModel.Usuario.Endereco;

                    fisioterapeutaExistente.Telefone =
                        cadastroProfissionalViewModel.Usuario.Telefone;

                    fisioterapeutaExistente.Senha =
                        SenhaHelper.HashSenha(
                            cadastroProfissionalViewModel.Usuario.Senha);

                    fisioterapeutaExistente.EtapaAcesso =
                        EtapaAcesso.AguardandoAprovacao;

                    fisioterapeutaExistente.DataCadasto = DateTime.Now;

                    _context.SaveChanges();

                    idUsuario = fisioterapeutaExistente.Id;
                }
                else
                {
                    var fisioterapeuta = new Fisioterapeuta
                    {
                        IdentificadorUnico = Guid.NewGuid(),
                        Nome = cadastroProfissionalViewModel.Usuario.Nome,
                        Email = email,
                        CPF = cpf,
                        Endereco = cadastroProfissionalViewModel.Usuario.Endereco,
                        Telefone = cadastroProfissionalViewModel.Usuario.Telefone,
                        Senha = SenhaHelper.HashSenha(
                            cadastroProfissionalViewModel.Usuario.Senha),
                        EtapaAcesso = EtapaAcesso.AguardandoAprovacao,
                        DataCadasto = DateTime.Now
                    };

                    _context.Fisioterapeutas.Add(fisioterapeuta);
                    _context.SaveChanges();

                    idUsuario = fisioterapeuta.Id;
                }
            }

            if (idUsuario.HasValue)
            {
                SalvarDocumento(
                    FotoDocumento,
                    TipoDocumento.FotoDocumento,
                    idUsuario.Value);

                SalvarDocumento(
                    Documento,
                    TipoDocumento.Documento,
                    idUsuario.Value);

                SalvarDocumento(
                    CertificadoBonsAntecedentes,
                    TipoDocumento.CertificadoBonsAntecedentes,
                    idUsuario.Value);

                SalvarDocumento(
                    CertificadoDispensa,
                    TipoDocumento.CertificadoDispensa,
                    idUsuario.Value);
            }

            return RedirectToAction("AguardandoAprovacao");
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
                    UsuarioId = usuarioId,
                    DataUpload = DateTime.Now
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
            {
                return Json(new
                {
                    existe = false,
                    reprovado = false
                });
            }

            var emailNormalizado = email.ToLower();

            var cuidador = _context.CuidadoresDeIdoso
                .FirstOrDefault(p =>
                    p.Email.ToLower() == emailNormalizado);

            var fisioterapeuta = _context.Fisioterapeutas
                .FirstOrDefault(p =>
                    p.Email.ToLower() == emailNormalizado);

            dynamic usuario = null;

            if (cuidador != null)
                usuario = cuidador;
            else if (fisioterapeuta != null)
                usuario = fisioterapeuta;

            if (usuario == null)
            {
                return Json(new
                {
                    existe = false,
                    reprovado = false
                });
            }

            bool aprovado =
                usuario.EtapaAcesso ==
                EtapaAcesso.AcessoLiberado;

            bool reprovado =
                usuario.EtapaAcesso ==
                EtapaAcesso.AcessoNegado;

            return Json(new
            {
                existe = aprovado,
                reprovado = reprovado,
                usuario = new
                {
                    nome = usuario.Nome,
                    email = usuario.Email,
                    telefone = usuario.Telefone,
                    endereco = usuario.Endereco,
                    cpf = usuario.CPF
                }
            });
        }
    }
}