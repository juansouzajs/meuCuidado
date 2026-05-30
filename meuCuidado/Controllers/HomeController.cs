using meuCuidado.Dominio.Models;
using System.Web.Mvc;

namespace meuCuidado.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult MeuCuidado()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        } 
        
        public ActionResult Ajuda()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Ajuda(Ajuda ajuda)
        {
            if (ModelState.IsValid)
            {
                var emailController = new EmailController();
                emailController.EnviarEmail(ajuda);

                ViewBag.Message = "Sua solicitação foi enviada com sucesso!";
                return View();
            }

            return View(ajuda);
        }
    }
}