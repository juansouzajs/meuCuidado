using meuCuidado.Dominio.Models;
using System.Web.Mvc;

namespace meuCuidado.Controllers
{
    public class DashboardController : Controller
    {
        public ActionResult Dashboard()
        {
            var usuarioLogado = User.Identity.Name;
            var tipoUsuario = Session["TipoUsuario"]?.ToString();

            ViewBag.TipoUsuario = tipoUsuario;

            return View(new Usuario());
        }

        public ActionResult Farmacias()
        {
            return PartialView();
        }
    }
}