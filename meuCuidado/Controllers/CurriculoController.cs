using meuCuidado.Dominio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static meuCuidado.Dominio.Extensions.EnumExtension;

namespace meuCuidado.Controllers
{
    public class CurriculoController : Controller
    {
        private readonly MeuCuidadoDbContext _context = new MeuCuidadoDbContext();

        public ActionResult EditarCurriculo()
        {
            var usuarioId = Convert.ToInt32(Session["IdUsuario"]);
            var tipoUsuarioStr = Session["TipoUsuario"]?.ToString();

            if (string.IsNullOrEmpty(tipoUsuarioStr))
                return RedirectToAction("Login", "Account");

            var tipoUsuarioEnum = (TipoUsuario)Enum.Parse(typeof(TipoUsuario), tipoUsuarioStr);

            var curriculo = _context.Curriculos
                .FirstOrDefault(x => x.UsuarioId == usuarioId && x.TipoUsuario == tipoUsuarioEnum);

            if (curriculo == null)
            {
                curriculo = new Curriculo
                {
                    UsuarioId = usuarioId,
                    TipoUsuario = tipoUsuarioEnum,
                    Cursos = new List<string>(),
                    Experiencias = new List<string>(),
                    RedesSociais = new List<string>()
                };
            }

            return View(curriculo);
        }

        [HttpPost]
        public JsonResult SalvarCurriculo(
            int AnosExperiencia,
            string Escolaridade,
            List<string> Cursos,
            List<string> Experiencias,
            string Facebook,
            string Instagram,
            string Linkedin,
            string Youtube)
        {
            try
            {
                var usuarioId = Convert.ToInt32(Session["IdUsuario"]);
                var tipoUsuarioStr = Session["TipoUsuario"]?.ToString();

                if (string.IsNullOrEmpty(tipoUsuarioStr))
                    return Json(new { success = false, erro = "Sessão inválida" });

                var tipoUsuarioEnum = (TipoUsuario)Enum.Parse(typeof(TipoUsuario), tipoUsuarioStr);

                var curriculo = _context.Curriculos
                    .FirstOrDefault(x => x.UsuarioId == usuarioId && x.TipoUsuario == tipoUsuarioEnum);

                if (curriculo == null)
                {
                    curriculo = new Curriculo
                    {
                        UsuarioId = usuarioId,
                        IdentificadorUnico = Guid.NewGuid()
                    };

                    _context.Curriculos.Add(curriculo);
                }

                curriculo.AnosExperiencia = AnosExperiencia;
                curriculo.Escolaridade = Escolaridade;
                curriculo.TipoUsuario = tipoUsuarioEnum;

                curriculo.Cursos = Cursos ?? new List<string>();
                curriculo.Experiencias = Experiencias ?? new List<string>();

                var redes = new List<string>();

                if (!string.IsNullOrWhiteSpace(Facebook)) redes.Add(Facebook);
                if (!string.IsNullOrWhiteSpace(Instagram)) redes.Add(Instagram);
                if (!string.IsNullOrWhiteSpace(Linkedin)) redes.Add(Linkedin);
                if (!string.IsNullOrWhiteSpace(Youtube)) redes.Add(Youtube);

                curriculo.RedesSociais = redes;

                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, erro = ex.Message });
            }
        }
    }
}