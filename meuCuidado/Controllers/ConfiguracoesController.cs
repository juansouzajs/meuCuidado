using meuCuidado.Dominio.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static meuCuidado.Dominio.Extensions.EnumExtension;

namespace meuCuidado.Controllers
{
    public class ConfiguracoesController : Controller
    {
        public ActionResult Configuracoes()
        {
            string tipoUsuario = Session["TipoUsuario"]?.ToString();

            var configuracoes = new List<ConfiguracoesViewModel> {
                new ConfiguracoesViewModel { Titulo = "Perfil", Descricao = "Editar informações do perfil", Link = Url.Action("EditarPerfil", "Perfil", new { id = 2 }) }            };

            if (tipoUsuario == GetEnumDescription(TipoUsuario.Cuidador) || tipoUsuario == GetEnumDescription(TipoUsuario.Fisioterapeuta))
            {
                configuracoes.Add(new ConfiguracoesViewModel { Titulo = "Currículo", Descricao = "Editar ou visualizar currículo", Link = Url.Action("EditarCurriculo", "Curriculo") });
                configuracoes.Add(new ConfiguracoesViewModel { Titulo = "Avaliações", Descricao = "Visualizar avaliações recebidas", Link = Url.Action("MinhasAvaliacoes", "Avaliacao") });
            }

            configuracoes.Add(new ConfiguracoesViewModel { Titulo = "Sair da conta", Descricao = "Encerrar sessão e voltar para o login", Link = Url.Action("Logout", "Login") });

            return View(configuracoes);
        }

        public ActionResult Perfil()
        {
            // Código para obter dados do perfil
            return View();
        }

        public ActionResult Curriculo()
        {
            // Código para obter dados do currículo
            return View();
        }

        public ActionResult Avaliacoes()
        {
            // Código para obter avaliações
            return View();
        }
    }
}
