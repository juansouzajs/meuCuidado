using AutoMapper;
using meuCuidado.Dominio.Models;
using meuCuidado.Dominio.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static meuCuidado.Dominio.Extensions.EnumExtension;

namespace meuCuidado.Controllers
{
    public class PerfilController : Controller
    {
        private readonly MeuCuidadoDbContext _context = new MeuCuidadoDbContext();

        public ActionResult Perfil(
    string tipoUsuario = null,
    string dataCadastro = null,
    string localizacao = null,
    string escolaridade = null
)
        {
            var tipoUsuarioSessao = Session["TipoUsuario"]?.ToString() ?? string.Empty;
            ViewBag.TipoUsuario = tipoUsuarioSessao;

            DateTime? data = null;
            if (!string.IsNullOrEmpty(dataCadastro))
                data = DateTime.Parse(dataCadastro);

            var query = new List<Usuario>();

            if (tipoUsuarioSessao == GetEnumDescription(TipoUsuario.Cuidador) ||
                tipoUsuarioSessao == GetEnumDescription(TipoUsuario.Fisioterapeuta))
            {
                var idosos = _context.Idosos.Where(x => x.EtapaAcesso == EtapaAcesso.AcessoLiberado);
                var tutores = _context.Tutores.Where(x => x.EtapaAcesso == EtapaAcesso.AcessoLiberado);

                if (!string.IsNullOrEmpty(localizacao))
                {
                    idosos = idosos.Where(x => x.Endereco.Contains(localizacao));
                    tutores = tutores.Where(x => x.Endereco.Contains(localizacao));
                }

                if (data.HasValue)
                {
                    idosos = idosos.Where(x => x.DataCadasto >= data);
                    tutores = tutores.Where(x => x.DataCadasto >= data);
                }

                query.AddRange(idosos);
                query.AddRange(tutores);
            }
            else
            {
                var cuidadores = _context.CuidadoresDeIdoso.Where(x => x.EtapaAcesso == EtapaAcesso.AcessoLiberado);
                var fisios = _context.Fisioterapeutas.Where(x => x.EtapaAcesso == EtapaAcesso.AcessoLiberado);

                if (!string.IsNullOrEmpty(localizacao))
                {
                    cuidadores = cuidadores.Where(x => x.Endereco.Contains(localizacao));
                    fisios = fisios.Where(x => x.Endereco.Contains(localizacao));
                }

                if (data.HasValue)
                {
                    cuidadores = cuidadores.Where(x => x.DataCadasto >= data);
                    fisios = fisios.Where(x => x.DataCadasto >= data);
                }
                if (!string.IsNullOrEmpty(escolaridade))
                {
                    var idsCurriculos = _context.Curriculos
                        .Where(c => c.EscolaridadeNivel == escolaridade)
                        .Select(c => c.UsuarioId)
                        .ToList();

                    cuidadores = cuidadores.Where(x => idsCurriculos.Contains(x.Id));
                    fisios = fisios.Where(x => idsCurriculos.Contains(x.Id));
                }

                if (tipoUsuario == "1")
                    query.AddRange(cuidadores);
                else if (tipoUsuario == "2")
                    query.AddRange(fisios);
                else
                {
                    query.AddRange(cuidadores);
                    query.AddRange(fisios);
                }
            }

            var usuariosVm = MontarViewModel(query);

            return View(usuariosVm);
        }

        public ActionResult Filtrar(
            string tipoUsuario = null,
            string dataCadastro = null,
            string localizacao = null,
            string escolaridade = null
        )
        {
            var tipoUsuarioSessao = Session["TipoUsuario"]?.ToString() ?? string.Empty;
            ViewBag.TipoUsuario = tipoUsuarioSessao;

            DateTime? data = null;
            if (!string.IsNullOrEmpty(dataCadastro))
                data = DateTime.Parse(dataCadastro);

            var query = new List<Usuario>();

            if (tipoUsuarioSessao == GetEnumDescription(TipoUsuario.Cuidador) ||
                tipoUsuarioSessao == GetEnumDescription(TipoUsuario.Fisioterapeuta))
            {
                var idosos = _context.Idosos.Where(x => x.EtapaAcesso == EtapaAcesso.AcessoLiberado);
                var tutores = _context.Tutores.Where(x => x.EtapaAcesso == EtapaAcesso.AcessoLiberado);

                if (!string.IsNullOrEmpty(localizacao))
                {
                    idosos = idosos.Where(x => x.Endereco.Contains(localizacao));
                    tutores = tutores.Where(x => x.Endereco.Contains(localizacao));
                }

                if (data.HasValue)
                {
                    idosos = idosos.Where(x => x.DataCadasto >= data);
                    tutores = tutores.Where(x => x.DataCadasto >= data);
                }

                query.AddRange(idosos);
                query.AddRange(tutores);
            }
            else
            {
                var cuidadores = _context.CuidadoresDeIdoso.Where(x => x.EtapaAcesso == EtapaAcesso.AcessoLiberado);
                var fisios = _context.Fisioterapeutas.Where(x => x.EtapaAcesso == EtapaAcesso.AcessoLiberado);

                if (!string.IsNullOrEmpty(localizacao))
                {
                    cuidadores = cuidadores.Where(x => x.Endereco.Contains(localizacao));
                    fisios = fisios.Where(x => x.Endereco.Contains(localizacao));
                }

                if (data.HasValue)
                {
                    cuidadores = cuidadores.Where(x => x.DataCadasto >= data);
                    fisios = fisios.Where(x => x.DataCadasto >= data);
                }

                if (!string.IsNullOrEmpty(escolaridade))
                {
                    var idsCurriculos = _context.Curriculos
                        .Where(c => c.EscolaridadeNivel == escolaridade)
                        .Select(c => c.UsuarioId)
                        .ToList();

                    cuidadores = cuidadores.Where(x => idsCurriculos.Contains(x.Id));
                    fisios = fisios.Where(x => idsCurriculos.Contains(x.Id));
                }

                if (tipoUsuario == "1")
                    query.AddRange(cuidadores);
                else if (tipoUsuario == "2")
                    query.AddRange(fisios);
                else
                {
                    query.AddRange(cuidadores);
                    query.AddRange(fisios);
                }
            }

            var usuariosVm = MontarViewModel(query);

            return PartialView("_UsuariosLista", usuariosVm);
        }

        private List<UsuarioCardViewModel> MontarViewModel(List<Usuario> usuarios)
        {
            var fotos = _context.Documentos
                .Where(d => d.TipoDocumento == TipoDocumento.FotoDocumento && d.Descricao == "Perfil")
                .ToList()
                .GroupBy(d => new { d.UsuarioId, d.TipoUsuario })
                .ToDictionary(
                    g => (g.Key.UsuarioId, g.Key.TipoUsuario),
                    g => g.FirstOrDefault()?.Caminho
                );

            var result = usuarios.Select(u =>
            {
                TipoUsuario tipo = DetectarTipoUsuario(u);

                fotos.TryGetValue((u.Id, tipo), out var foto);

                return new UsuarioCardViewModel
                {
                    Id = u.Id,
                    IdentificadorUnico = u.IdentificadorUnico,
                    Nome = u.Nome,
                    Email = u.Email,
                    TipoUsuario = tipo,
                    FotoUrl = foto
                };
            }).ToList();

            return result;
        }

        private TipoUsuario DetectarTipoUsuario(Usuario u)
        {
            if (_context.Idosos.Any(x => x.Id == u.Id))
                return TipoUsuario.Idoso;

            if (_context.Tutores.Any(x => x.Id == u.Id))
                return TipoUsuario.Tutor;

            if (_context.CuidadoresDeIdoso.Any(x => x.Id == u.Id))
                return TipoUsuario.Cuidador;

            return TipoUsuario.Fisioterapeuta;
        }

        public ActionResult PerfilDetalhado(Guid IdentificadorUnico)
        {
            var tipoUsuarioSessao = Session["TipoUsuario"]?.ToString() ?? string.Empty;
            var idUsuarioLogado = Convert.ToInt32(Session["IdUsuario"]);

            var perfilDetalhado = new PerfilDetalhadoViewModel()
            {
                CuidadorDeIdoso = _context.CuidadoresDeIdoso
                    .SingleOrDefault(p => p.IdentificadorUnico == IdentificadorUnico),

                Fisioterapeuta = _context.Fisioterapeutas
                    .SingleOrDefault(p => p.IdentificadorUnico == IdentificadorUnico),

                Tutor = _context.Tutores
                    .SingleOrDefault(p => p.IdentificadorUnico == IdentificadorUnico),

                Idoso = _context.Idosos
                    .SingleOrDefault(p => p.IdentificadorUnico == IdentificadorUnico),

                TipoUsuario = tipoUsuarioSessao,
                IdUsuario = idUsuarioLogado.ToString()
            };

            if (perfilDetalhado.CuidadorDeIdoso == null &&
                perfilDetalhado.Fisioterapeuta == null &&
                perfilDetalhado.Tutor == null &&
                perfilDetalhado.Idoso == null)
            {
                return HttpNotFound();
            }

            int? idAlvo = null;
            TipoUsuario tipoPerfil;

            if (perfilDetalhado.CuidadorDeIdoso != null)
            {
                idAlvo = perfilDetalhado.CuidadorDeIdoso.Id;
                tipoPerfil = TipoUsuario.Cuidador;
            }
            else if (perfilDetalhado.Fisioterapeuta != null)
            {
                idAlvo = perfilDetalhado.Fisioterapeuta.Id;
                tipoPerfil = TipoUsuario.Fisioterapeuta;
            }
            else if (perfilDetalhado.Idoso != null)
            {
                idAlvo = perfilDetalhado.Idoso.Id;
                tipoPerfil = TipoUsuario.Idoso;
            }
            else
            {
                idAlvo = perfilDetalhado.Tutor.Id;
                tipoPerfil = TipoUsuario.Tutor;
            }

            perfilDetalhado.Curriculo = _context.Curriculos
                .FirstOrDefault(c =>
                    c.UsuarioId == idAlvo &&
                    c.TipoUsuario == tipoPerfil
                )
                ?? new Curriculo()
                {
                    AnosExperiencia = 0,
                    Cursos = new List<string>(),
                    Experiencias = new List<string>(),
                    RedesSociais = new List<string>()
                };

            perfilDetalhado.TemConexaoAtiva = false;

            if (idAlvo.HasValue)
            {
                perfilDetalhado.TemConexaoAtiva = _context.RelacionamentosIdosoProfissional.Any(r =>
                    r.EtapaAtivacao == EtapaAtivacao.AtivacaoLiberada &&
                    (
                        (r.IdosoId == idUsuarioLogado && r.CuidadorId == idAlvo) ||
                        (r.IdosoId == idUsuarioLogado && r.FisioterapeutaId == idAlvo) ||

                        (r.TutorId == idUsuarioLogado && r.CuidadorId == idAlvo) ||
                        (r.TutorId == idUsuarioLogado && r.FisioterapeutaId == idAlvo) ||

                        (r.CuidadorId == idUsuarioLogado && r.IdosoId == idAlvo) ||
                        (r.CuidadorId == idUsuarioLogado && r.TutorId == idAlvo) ||

                        (r.FisioterapeutaId == idUsuarioLogado && r.IdosoId == idAlvo) ||
                        (r.FisioterapeutaId == idUsuarioLogado && r.TutorId == idAlvo)
                    )
                );

                perfilDetalhado.ConexaoPendente = _context.RelacionamentosIdosoProfissional.Any(r =>
                    r.EtapaAtivacao == EtapaAtivacao.AguardandoAprovacao &&
                    (
                        (r.IdosoId == idUsuarioLogado && r.CuidadorId == idAlvo) ||
                        (r.IdosoId == idUsuarioLogado && r.FisioterapeutaId == idAlvo) ||

                        (r.TutorId == idUsuarioLogado && r.CuidadorId == idAlvo) ||
                        (r.TutorId == idUsuarioLogado && r.FisioterapeutaId == idAlvo) ||

                        (r.CuidadorId == idUsuarioLogado && r.IdosoId == idAlvo) ||
                        (r.CuidadorId == idUsuarioLogado && r.TutorId == idAlvo) ||

                        (r.FisioterapeutaId == idUsuarioLogado && r.IdosoId == idAlvo) ||
                        (r.FisioterapeutaId == idUsuarioLogado && r.TutorId == idAlvo)
                    )
                );

                perfilDetalhado.Avaliacaos = new List<Avaliacao>();

                if (tipoPerfil == TipoUsuario.Cuidador)
                {
                    perfilDetalhado.Avaliacaos = _context.Avaliacoes
                        .Where(a => a.RelacionamentoIdosoProfissional.CuidadorId == idAlvo)
                        .OrderByDescending(a => a.Id)
                        .ToList();
                }
                else if (tipoPerfil == TipoUsuario.Fisioterapeuta)
                {
                    perfilDetalhado.Avaliacaos = _context.Avaliacoes
                        .Where(a => a.RelacionamentoIdosoProfissional.FisioterapeutaId == idAlvo)
                        .ToList();
                }
                else if (tipoPerfil == TipoUsuario.Idoso)
                {
                    perfilDetalhado.Avaliacaos = _context.Avaliacoes
                        .Where(a => a.RelacionamentoIdosoProfissional.IdosoId == idAlvo)
                        .ToList();
                }
                else
                {
                    perfilDetalhado.Avaliacaos = _context.Avaliacoes
                        .Where(a => a.RelacionamentoIdosoProfissional.TutorId == idAlvo)
                        .ToList();
                }

                perfilDetalhado.JaAvaliou = _context.Avaliacoes.Any(a =>
                    (
                        a.RelacionamentoIdosoProfissional.CuidadorId == idAlvo ||
                        a.RelacionamentoIdosoProfissional.FisioterapeutaId == idAlvo ||
                        a.RelacionamentoIdosoProfissional.IdosoId == idAlvo ||
                        a.RelacionamentoIdosoProfissional.TutorId == idAlvo
                    )
                    &&
                    (
                        a.RelacionamentoIdosoProfissional.IdosoId == idUsuarioLogado ||
                        a.RelacionamentoIdosoProfissional.TutorId == idUsuarioLogado ||
                        a.RelacionamentoIdosoProfissional.CuidadorId == idUsuarioLogado ||
                        a.RelacionamentoIdosoProfissional.FisioterapeutaId == idUsuarioLogado
                    )
                );

                var foto = _context.Documentos
                    .Where(d =>
                        d.UsuarioId == idAlvo &&
                        d.TipoDocumento == TipoDocumento.FotoDocumento &&
                        d.TipoUsuario == tipoPerfil &&
                        d.Descricao == "Perfil"
                    )
                    .OrderByDescending(d => d.DataUpload)
                    .Select(d => d.Caminho)
                    .FirstOrDefault();

                perfilDetalhado.FotoUrl = foto;
            }

            return View(perfilDetalhado);
        }

        public ActionResult EditarPerfil()
        {
            var idUsuario = Convert.ToInt32(Session["IdUsuario"]);
            var tipoUsuarioStr = Session["TipoUsuario"]?.ToString();

            if (string.IsNullOrEmpty(tipoUsuarioStr))
                return RedirectToAction("Login", "Account");

            var tipoUsuarioEnum = (TipoUsuario)Enum.Parse(typeof(TipoUsuario), tipoUsuarioStr);

            var foto = _context.Documentos
                .Where(d =>
                    d.UsuarioId == idUsuario &&
                    d.TipoDocumento == TipoDocumento.FotoDocumento &&
                    d.TipoUsuario == tipoUsuarioEnum &&
                    d.Descricao == "Perfil"
                )
                .OrderByDescending(d => d.DataUpload)
                .Select(d => d.Caminho)
                .FirstOrDefault();

            var viewModel = new PerfilDetalhadoViewModel
            {
                FotoUrl = foto
            };

            if (tipoUsuarioStr == "Cuidador")
            {
                viewModel.CuidadorDeIdoso = _context.CuidadoresDeIdoso.FirstOrDefault(x => x.Id == idUsuario);
            }
            else if (tipoUsuarioStr == "Fisioterapeuta")
            {
                viewModel.Fisioterapeuta = _context.Fisioterapeutas.FirstOrDefault(x => x.Id == idUsuario);
            }
            else if (tipoUsuarioStr == "Idoso")
            {
                viewModel.Idoso = _context.Idosos.FirstOrDefault(x => x.Id == idUsuario);
            }
            else if (tipoUsuarioStr == "Tutor")
            {
                viewModel.Tutor = _context.Tutores.FirstOrDefault(x => x.Id == idUsuario);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditarPerfil(
            HttpPostedFileBase FotoPerfil,
            string LinkWhatsapp,
            bool? NecessidadesEspeciais,
            string DescricaoNecessidadesEspeciais
        )
        {
            try
            {
                var idUsuario = Convert.ToInt32(Session["IdUsuario"]);
                var tipoUsuarioStr = Session["TipoUsuario"]?.ToString();

                if (string.IsNullOrEmpty(tipoUsuarioStr))
                    throw new Exception("Tipo de usuário não encontrado na sessão.");

                var tipoUsuarioEnum = (TipoUsuario)Enum.Parse(typeof(TipoUsuario), tipoUsuarioStr);

                // FOTO
                if (FotoPerfil != null && FotoPerfil.ContentLength > 0)
                {
                    SalvarDocumento(FotoPerfil, TipoDocumento.FotoDocumento, idUsuario, tipoUsuarioEnum);
                }

                // WHATSAPP (para todos)
                Usuario usuario = null;

                if (tipoUsuarioStr == "Cuidador")
                    usuario = _context.CuidadoresDeIdoso.FirstOrDefault(x => x.Id == idUsuario);
                else if (tipoUsuarioStr == "Fisioterapeuta")
                    usuario = _context.Fisioterapeutas.FirstOrDefault(x => x.Id == idUsuario);
                else if (tipoUsuarioStr == "Idoso")
                    usuario = _context.Idosos.FirstOrDefault(x => x.Id == idUsuario);
                else if (tipoUsuarioStr == "Tutor")
                    usuario = _context.Tutores.FirstOrDefault(x => x.Id == idUsuario);

                if (usuario != null)
                {
                    if (!string.IsNullOrWhiteSpace(LinkWhatsapp))
                    {
                        LinkWhatsapp = LinkWhatsapp.Trim();

                        // se não vier com http, força padrão do WhatsApp
                        if (!LinkWhatsapp.StartsWith("http"))
                        {
                            // remove tudo que não for número
                            var numeros = new string(LinkWhatsapp.Where(char.IsDigit).ToArray());

                            if (!string.IsNullOrEmpty(numeros))
                                LinkWhatsapp = $"https://wa.me/{numeros}";
                        }

                        usuario.LinkWhatsapp = LinkWhatsapp;
                    }
                    else
                    {
                        usuario.LinkWhatsapp = null;
                    }
                }

                // NECESSIDADES ESPECIAIS
                if (tipoUsuarioStr == "Tutor")
                {
                    var tutor = usuario as Tutor;

                    if (tutor != null && NecessidadesEspeciais.HasValue)
                    {
                        tutor.NecessidadesEspeciais = NecessidadesEspeciais.Value;

                        tutor.DescricaoNecessidadesEspeciais =
                            tutor.NecessidadesEspeciais ? DescricaoNecessidadesEspeciais : null;
                    }
                }
                else if (tipoUsuarioStr == "Idoso")
                {
                    var idoso = usuario as Idoso;

                    if (idoso != null && NecessidadesEspeciais.HasValue)
                    {
                        idoso.NecessidadesEspeciais = NecessidadesEspeciais.Value;

                        idoso.DescricaoNecessidadesEspeciais =
                            idoso.NecessidadesEspeciais ? DescricaoNecessidadesEspeciais : null;
                    }
                }

                _context.SaveChanges();

                TempData["ToastMensagem"] = "Perfil atualizado com sucesso!";
                TempData["ToastTipo"] = "sucesso";

                return RedirectToAction("EditarPerfil");
            }
            catch (Exception ex)
            {
                TempData["ToastMensagem"] = ex.Message;
                TempData["ToastTipo"] = "erro";

                return RedirectToAction("EditarPerfil");
            }
        }

        private void SalvarDocumento(HttpPostedFileBase arquivo, TipoDocumento tipoDocumento, int usuarioId, TipoUsuario tipoUsuario)
        {
            if (arquivo != null && arquivo.ContentLength > 0)
            {
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
                    default:
                        throw new Exception("Formato inválido (use JPG ou PNG)");
                }

                var caminho = Server.MapPath("~/ImagensPerfil/");

                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                var nomeArquivo = Guid.NewGuid() + extensao;
                var caminhoCompleto = Path.Combine(caminho, nomeArquivo);

                arquivo.SaveAs(caminhoCompleto);

                var antigos = _context.Documentos
                    .Where(d =>
                        d.UsuarioId == usuarioId &&
                        d.TipoDocumento == TipoDocumento.FotoDocumento &&
                        d.TipoUsuario == tipoUsuario &&
                        d.Descricao == "Perfil"
                    );

                _context.Documentos.RemoveRange(antigos);
                _context.SaveChanges();

                var documento = new Documento
                {
                    Id = Guid.NewGuid(),
                    TipoDocumento = tipoDocumento,
                    Extensao = tipoExtensao,
                    Caminho = "/ImagensPerfil/" + nomeArquivo,
                    UsuarioId = usuarioId,
                    TipoUsuario = tipoUsuario,
                    Descricao = "Perfil",
                    DataUpload = DateTime.Now
                };

                _context.Documentos.Add(documento);
                _context.SaveChanges();
            }
        }
    }
}