using System;
using System.Web.Mvc;
using meuCuidado.Dominio.Models;

namespace meuCuidado.Controllers
{
    public class MedicamentoController : Controller
    {
        private readonly MeuCuidadoDbContext _context;

        public MedicamentoController()
        {
            _context = new MeuCuidadoDbContext();
        }

        public ActionResult AdicionarMedicamento()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdicionarMedicamento(Medicamento medicamento)
        {
            if (ModelState.IsValid)
            {
                medicamento.IdentificadorUnico = Guid.NewGuid();
                _context.Medicamentos.Add(medicamento);
                _context.SaveChanges();

                return RedirectToAction("Dashboard", "Dashboard");
            }

            return View(medicamento);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
