using iTextSharp.text;
using iTextSharp.text.pdf;
using meuCuidado.Dominio.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using static meuCuidado.Dominio.Extensions.EnumExtension;

namespace meuCuidado.Controllers
{
    public class LembreteController : Controller
    {
        private readonly MeuCuidadoDbContext _context;

        public LembreteController()
        {
            _context = new MeuCuidadoDbContext();
        }

        private int UsuarioIdLogado
        {
            get
            {
                if (Session["IdUsuario"] == null)
                {
                    return 0;
                }

                int usuarioId;

                if (int.TryParse(
                    Session["IdUsuario"].ToString(),
                    out usuarioId))
                {
                    return usuarioId;
                }

                return 0;
            }
        }

        private int TipoUsuarioLogado
        {
            get
            {
                if (Session["TipoUsuario"] == null)
                {
                    return 0;
                }

                var tipo = Session["TipoUsuario"]
                    .ToString()
                    .Trim()
                    .ToLower();

                switch (tipo)
                {
                    case "idoso":
                        return 1;

                    case "tutor":
                        return 2;

                    case "cuidador":
                        return 3;

                    case "fisioterapeuta":
                        return 4;

                    default:
                        return 0;
                }
            }
        }

        public ActionResult Lembrete()
        {
            var medicamentos = _context.Medicamentos
                .ToList();

            var pessoas = _context.Pessoas
                .OrderBy(p => p.Nome)
                .ToList();

            ViewBag.Pessoas = pessoas;

            ViewBag.Medicamentos = medicamentos;

            var hoje = DateTime.Today;

            var lembretes = _context.Lembretes
                .Include(l => l.Medicamento)
                .Include(l => l.Pessoa)
                .Where(l =>
                    DbFunctions.TruncateTime(l.DataHora) == hoje
                    &&
                    l.UsuarioId == UsuarioIdLogado
                    &&
                    l.TipoUsuario == TipoUsuarioLogado
                )
                .OrderBy(l => l.DataHora)
                .ToList();

            return View(lembretes);
        }

        [HttpPost]
        public JsonResult CreatePessoa(Pessoa pessoa)
        {
            try
            {
                if (pessoa == null ||
                    string.IsNullOrWhiteSpace(pessoa.Nome))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Nome inválido"
                    });
                }

                pessoa.Nome = pessoa.Nome.Trim();

                _context.Pessoas.Add(pessoa);

                _context.SaveChanges();

                return Json(new
                {
                    success = true,
                    id = pessoa.Id,
                    nome = pessoa.Nome
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult Create(Lembrete lembrete)
        {
            try
            {
                if (lembrete == null)
                {
                    return Json(new
                    {
                        success = false,
                        error = "Lembrete inválido"
                    });
                }

                int quantidadeDias = 1;

                if (lembrete.MedicamentoId.HasValue)
                {
                    var medicamento = _context.Medicamentos
                        .FirstOrDefault(m =>
                            m.Id == lembrete.MedicamentoId.Value);

                    if (medicamento != null &&
                        medicamento.DuracaoEmDias > 0)
                    {
                        quantidadeDias = medicamento.DuracaoEmDias;
                    }
                }

                for (int i = 0; i < quantidadeDias; i++)
                {
                    var dataLembrete =
                        lembrete.DataHora.AddDays(i);

                    if (lembrete.PessoaId == null)
                    {
                        var pessoas = _context.Pessoas.ToList();

                        foreach (var pessoa in pessoas)
                        {
                            var novo = new Lembrete
                            {
                                IdentificadorUnico = Guid.NewGuid(),

                                PessoaId = pessoa.Id,

                                Descricao = lembrete.Descricao,

                                DataHora = dataLembrete,

                                MedicamentoId = lembrete.MedicamentoId,

                                Repete = lembrete.Repete,

                                UsuarioId = UsuarioIdLogado,

                                TipoUsuario = TipoUsuarioLogado
                            };

                            _context.Lembretes.Add(novo);
                        }
                    }
                    else
                    {
                        var novo = new Lembrete
                        {
                            IdentificadorUnico = Guid.NewGuid(),

                            PessoaId = lembrete.PessoaId,

                            Descricao = lembrete.Descricao,

                            DataHora = dataLembrete,

                            MedicamentoId = lembrete.MedicamentoId,

                            Repete = lembrete.Repete,

                            UsuarioId = UsuarioIdLogado,

                            TipoUsuario = TipoUsuarioLogado
                        };

                        _context.Lembretes.Add(novo);
                    }
                }

                _context.SaveChanges();

                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        public ActionResult GetReminders(
            string date,
            int? pessoaId
        )
        {
            DateTime selectedDate;

            if (!DateTime.TryParse(date, out selectedDate))
            {
                return PartialView(
                    "ListaLembretes",
                    new List<Lembrete>()
                );
            }

            var query = _context.Lembretes
                .Include(l => l.Medicamento)
                .Include(l => l.Pessoa)
                .Where(l =>
                    DbFunctions.TruncateTime(l.DataHora)
                    == selectedDate.Date

                    &&

                    l.UsuarioId == UsuarioIdLogado

                    &&

                    l.TipoUsuario == TipoUsuarioLogado
                );

            if (pessoaId.HasValue)
            {
                query = query.Where(l =>
                    l.PessoaId == pessoaId.Value
                );
            }

            var lembretes = query
                .OrderBy(l => l.DataHora)
                .ToList();

            return PartialView(
                "ListaLembretes",
                lembretes
            );
        }

        [HttpPost]
        public JsonResult Excluir(int id)
        {
            try
            {
                var lembrete = _context.Lembretes
                    .FirstOrDefault(l =>
                        l.Id == id
                        &&
                        l.UsuarioId == UsuarioIdLogado
                        &&
                        l.TipoUsuario == TipoUsuarioLogado
                    );

                if (lembrete == null)
                {
                    return Json(new
                    {
                        success = false
                    });
                }

                _context.Lembretes.Remove(lembrete);

                _context.SaveChanges();

                return Json(new
                {
                    success = true
                });
            }
            catch
            {
                return Json(new
                {
                    success = false
                });
            }
        }

        [HttpPost]
        public JsonResult Duplicar(
            int id,
            DateTime novaDataHora
        )
        {
            try
            {
                var original = _context.Lembretes
                    .FirstOrDefault(l =>
                        l.Id == id
                        &&
                        l.UsuarioId == UsuarioIdLogado
                        &&
                        l.TipoUsuario == TipoUsuarioLogado
                    );

                if (original == null)
                {
                    return Json(new
                    {
                        success = false
                    });
                }

                var novo = new Lembrete
                {
                    Descricao = original.Descricao,

                    DataHora = novaDataHora,

                    MedicamentoId = original.MedicamentoId,

                    PessoaId = original.PessoaId,

                    Repete = original.Repete,

                    DataHoraPrimeiroAlerta =
                        original.DataHoraPrimeiroAlerta,

                    IdentificadorUnico = Guid.NewGuid(),

                    UsuarioId = UsuarioIdLogado,

                    TipoUsuario = TipoUsuarioLogado
                };

                _context.Lembretes.Add(novo);

                _context.SaveChanges();

                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        public FileResult Exportar(
            int? pessoaId,
            DateTime? dataInicio,
            DateTime? dataFim,
            string formato = "csv"
        )
        {
            var query = _context.Lembretes
                .Include(l => l.Medicamento)
                .Include(l => l.Pessoa)
                .Where(l =>
                    l.UsuarioId == UsuarioIdLogado
                    &&
                    l.TipoUsuario == TipoUsuarioLogado
                );

            if (pessoaId.HasValue)
            {
                query = query.Where(l =>
                    l.PessoaId == pessoaId.Value
                );
            }

            if (dataInicio.HasValue)
            {
                var inicio = dataInicio.Value.Date;

                query = query.Where(l =>
                    DbFunctions.TruncateTime(l.DataHora)
                    >= inicio
                );
            }

            if (dataFim.HasValue)
            {
                var fim = dataFim.Value.Date;

                query = query.Where(l =>
                    DbFunctions.TruncateTime(l.DataHora)
                    <= fim
                );
            }

            var lembretes = query
                .OrderBy(l => l.DataHora)
                .ToList();

            string nomePessoa = "todos";

            if (pessoaId.HasValue)
            {
                nomePessoa = _context.Pessoas
                    .Where(p => p.Id == pessoaId.Value)
                    .Select(p => p.Nome)
                    .FirstOrDefault() ?? "pessoa";

                nomePessoa = nomePessoa
                    .Trim()
                    .Replace(" ", "_")
                    .ToLower();
            }

            string periodo =
                $"{dataInicio:yyyy-MM-dd}_{dataFim:yyyy-MM-dd}";

            if (formato.ToLower() == "csv")
            {
                var linhas = new List<string>();

                linhas.Add(
                    "Pessoa;Descricao;Data;Hora;Medicamento"
                );

                foreach (var item in lembretes)
                {
                    linhas.Add(
                        $"{item.Pessoa?.Nome};" +
                        $"{item.Descricao};" +
                        $"{item.DataHora:dd/MM/yyyy};" +
                        $"{item.DataHora:HH:mm};" +
                        $"{item.Medicamento?.Nome} {item.Medicamento?.Dosagem}"
                    );
                }

                var csv = string.Join(
                    Environment.NewLine,
                    linhas
                );

                var bytes =
                    Encoding.UTF8.GetBytes(csv);

                return File(
                    bytes,
                    "text/csv",
                    $"{nomePessoa}_lembretes_{periodo}.csv"
                );
            }

            using (var memoryStream = new MemoryStream())
            {
                var document = new Document(
                    PageSize.A4,
                    20,
                    20,
                    20,
                    20
                );

                PdfWriter.GetInstance(
                    document,
                    memoryStream
                );

                document.Open();

                var titulo = new Paragraph(
                    "Relatório de Lembretes"
                );

                titulo.SpacingAfter = 20;

                document.Add(titulo);

                PdfPTable tabela = new PdfPTable(5);

                tabela.WidthPercentage = 100;

                tabela.AddCell("Pessoa");
                tabela.AddCell("Descrição");
                tabela.AddCell("Data");
                tabela.AddCell("Hora");
                tabela.AddCell("Medicamento");

                foreach (var item in lembretes)
                {
                    tabela.AddCell(
                        item.Pessoa?.Nome ?? "-"
                    );

                    tabela.AddCell(
                        item.Descricao ?? "-"
                    );

                    tabela.AddCell(
                        item.DataHora.ToString(
                            "dd/MM/yyyy"
                        )
                    );

                    tabela.AddCell(
                        item.DataHora.ToString(
                            "HH:mm"
                        )
                    );

                    tabela.AddCell(
                        $"{item.Medicamento?.Nome} {item.Medicamento?.Dosagem}"
                    );
                }

                document.Add(tabela);

                document.Close();

                return File(
                    memoryStream.ToArray(),
                    "application/pdf",
                    $"{nomePessoa}_lembretes_{periodo}.pdf"
                );
            }
        }

        [HttpPost]
        public JsonResult AdicionarAoQuadro(string nome)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nome))
                {
                    return Json(new
                    {
                        sucesso = false,
                        mensagem = "Nome inválido."
                    });
                }

                nome = nome.Trim();

                bool existe = _context.Pessoas.Any(p =>
                    p.Nome == nome);

                if (existe)
                {
                    return Json(new
                    {
                        sucesso = false,
                        mensagem = "Pessoa já está no quadro."
                    });
                }

                var pessoa = new Pessoa
                {
                    Nome = nome
                };

                _context.Pessoas.Add(pessoa);

                _context.SaveChanges();

                return Json(new
                {
                    sucesso = true,
                    id = pessoa.Id,
                    nome = pessoa.Nome
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = ex.Message
                });
            }
        }
    }
}