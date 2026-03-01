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
            ViewBag.Medicamentos = medicamentos;

            var hoje = DateTime.Today;

            var lembretes = _context.Lembretes
                .Include(l => l.Medicamento)
                .Where(l => DbFunctions.TruncateTime(l.DataHora) == hoje)
                .ToList();

            return View(lembretes);
        }

        [HttpPost]
        public JsonResult Create(Lembrete lembrete)
        {
            try
            {
                lembrete.IdentificadorUnico = Guid.NewGuid();
                lembrete.RelacionamentoIdosoProfissionalId = 1;

                _context.Lembretes.Add(lembrete);
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        public ActionResult GetReminders(string date)
        {
            DateTime selectedDate;

            if (!DateTime.TryParse(date, out selectedDate))
                return PartialView("ListaLembretes", new List<Lembrete>());

            var lembretes = _context.Lembretes
                .Include(l => l.Medicamento)
                .Where(l => DbFunctions.TruncateTime(l.DataHora) == selectedDate.Date)
                .ToList();

            return PartialView("ListaLembretes", lembretes);
        }

        [HttpPost]
        public JsonResult Excluir(int id)
        {
            try
            {
                var lembrete = _context.Lembretes.Find(id);

                if (lembrete == null)
                {
                    return Json(new { success = false });
                }

                _context.Lembretes.Remove(lembrete);
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
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
                    return Json(new { success = false });

                var novo = new Lembrete
                {
                    Descricao = original.Descricao,
                    DataHora = novaDataHora,
                    MedicamentoId = original.MedicamentoId,
                    RelacionamentoIdosoProfissionalId = original.RelacionamentoIdosoProfissionalId,
                    IdentificadorUnico = Guid.NewGuid()
                };

                _context.Lembretes.Add(novo);
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
    }
}
