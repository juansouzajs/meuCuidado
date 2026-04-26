using meuCuidado.Dominio.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using static meuCuidado.Dominio.Extensions.EnumExtension;

namespace meuCuidado.Controllers
{
    public class CurriculoController : Controller
    {
        private readonly MeuCuidadoDbContext _context = new MeuCuidadoDbContext();

        public ActionResult Curriculo()
        {
            return View(new Curriculo());
        }

        public ActionResult EditarCurriculo()
        {
            var usuarioId = Convert.ToInt32(Session["IdUsuario"]);

            var curriculo = _context.Curriculos
                .FirstOrDefault(x => x.UsuarioId == usuarioId);

            if (curriculo == null)
            {
                curriculo = new Curriculo
                {
                    UsuarioId = usuarioId,
                    Cursos = new List<string>(),
                    Experiencias = new List<string>(),
                    RedesSociais = new List<string>()
                };
            }

            return View(curriculo);
        }

        [HttpPost]
        public JsonResult SalvarCurriculo(Curriculo model)
        {
            try
            {
                var usuarioId = Convert.ToInt32(Session["IdUsuario"]);
                var tipoUsuarioStr = Session["TipoUsuario"]?.ToString();

                if (string.IsNullOrEmpty(tipoUsuarioStr))
                    return Json(new { success = false, erro = "Sessão inválida" });

                var tipoUsuarioEnum = (TipoUsuario)Enum.Parse(typeof(TipoUsuario), tipoUsuarioStr);

                var curriculo = _context.Curriculos
                    .FirstOrDefault(x => x.UsuarioId == usuarioId);

                if (curriculo == null)
                {
                    curriculo = new Curriculo
                    {
                        UsuarioId = usuarioId,
                        IdentificadorUnico = Guid.NewGuid()
                    };

                    _context.Curriculos.Add(curriculo);
                }

                // DADOS
                curriculo.AnosExperiencia = model.AnosExperiencia;
                curriculo.Escolaridade = model.Escolaridade;
                curriculo.TipoUsuario = tipoUsuarioEnum;

                // 🔥 AQUI TÁ OUTRO ERRO SEU (binding não funciona automático)
                curriculo.Cursos = Request["CursosRaw"]?
                    .Split(',')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList() ?? new List<string>();

                curriculo.Experiencias = Request["ExperienciasRaw"]?
                    .Split(',')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList() ?? new List<string>();

                curriculo.RedesSociais = Request["RedesRaw"]?
                    .Split(',')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList() ?? new List<string>();

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