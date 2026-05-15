using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using meuCuidado.Dominio.Models;

namespace meuCuidado.Controllers
{
    public class LembreteController : Controller
    {
        private readonly MeuCuidadoDbContext _context;

        public LembreteController()
        {
            _context = new MeuCuidadoDbContext();
        }

        public ActionResult Lembrete()
        {
            var medicamentos = _context.Medicamentos.ToList();

            var pessoas = _context.Pessoas
                .OrderBy(p => p.Nome)
                .ToList();

            ViewBag.Pessoas = pessoas;

            ViewBag.Medicamentos = medicamentos;

            var hoje = DateTime.Today;

            var lembretes = _context.Lembretes
                .Include(l => l.Medicamento)
                .Include(l => l.Pessoa)
                .Where(l => DbFunctions.TruncateTime(l.DataHora) == hoje)
                .ToList();

            return View(lembretes);
        }


        [HttpPost]
        public JsonResult CreatePessoa(Pessoa pessoa)
        {
            try
            {
                if (pessoa == null || string.IsNullOrWhiteSpace(pessoa.Nome))
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
                            DataHora = lembrete.DataHora,
                            MedicamentoId = lembrete.MedicamentoId,
                            Repete = lembrete.Repete
                        };

                        _context.Lembretes.Add(novo);
                    }
                }
                else
                {
                    lembrete.IdentificadorUnico = Guid.NewGuid();

                    _context.Lembretes.Add(lembrete);
                }

                _context.SaveChanges();

                return Json(new { success = true });
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

        public ActionResult GetReminders(string date, int? pessoaId)
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
                var lembrete = _context.Lembretes.Find(id);

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

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var lembrete = _context.Lembretes.Find(id);

            _context.Lembretes.Remove(lembrete);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult Duplicar(int id, DateTime novaDataHora)
        {
            try
            {
                var original = _context.Lembretes
                    .FirstOrDefault(l => l.Id == id);

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
                    DataHoraPrimeiroAlerta = original.DataHoraPrimeiroAlerta,
                    IdentificadorUnico = Guid.NewGuid()
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
    }
}