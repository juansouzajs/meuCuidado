using meuCuidado.Dominio.Models;
using meuCuidado.Dominio.ViewModels;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Web.Mvc;
using static meuCuidado.Dominio.Extensions.EnumExtension;
using AutoMapper;

namespace meuCuidado.Controllers
{
    public class PerfilController : Controller
    {
        private readonly MeuCuidadoDbContext _context = new MeuCuidadoDbContext();

        public ActionResult Perfil(string tipoUsuario = null, string dataCadastro = null, string localizacao = null)
        {
            var tipoUsuarioSessao = Session["TipoUsuario"]?.ToString() ?? string.Empty;

            IEnumerable<Usuario> usuariosParaExibir = new List<Usuario>();

            // Cuidador ou Fisioterapeuta visualizam Idosos e Tutores
            if (tipoUsuarioSessao == GetEnumDescription(TipoUsuario.Cuidador) ||
                tipoUsuarioSessao == GetEnumDescription(TipoUsuario.Fisioterapeuta))
            {
                ViewBag.ExibirFiltros = false;

                var idosos = _context.Idosos.ToList();
                var tutores = _context.Tutores.ToList();

                usuariosParaExibir = idosos.Concat<Usuario>(tutores);
            }

            // Idoso ou Tutor visualizam Profissionais
            else if (tipoUsuarioSessao == GetEnumDescription(TipoUsuario.Idoso) ||
                     tipoUsuarioSessao == GetEnumDescription(TipoUsuario.Tutor))
            {
                ViewBag.ExibirFiltros = true;

                var cuidadores = _context.CuidadoresDeIdoso.ToList();
                var fisioterapeutas = _context.Fisioterapeutas.ToList();

                // Filtro por localização
                if (!string.IsNullOrEmpty(localizacao))
                {
                    cuidadores = cuidadores.Where(u => u.Endereco.Contains(localizacao)).ToList();
                    fisioterapeutas = fisioterapeutas.Where(u => u.Endereco.Contains(localizacao)).ToList();
                }

                // Filtro por data
                if (!string.IsNullOrEmpty(dataCadastro))
                {
                    var data = DateTime.Parse(dataCadastro);

                    cuidadores = cuidadores.Where(u => u.DataCadasto >= data).ToList();
                    fisioterapeutas = fisioterapeutas.Where(u => u.DataCadasto >= data).ToList();
                }

                // Filtro por tipo de profissional
                if (tipoUsuario == GetEnumDescription(TipoUsuario.Cuidador))
                {
                    usuariosParaExibir = cuidadores;
                }
                else if (tipoUsuario == GetEnumDescription(TipoUsuario.Fisioterapeuta))
                {
                    usuariosParaExibir = fisioterapeutas;
                }
                else
                {
                    usuariosParaExibir = cuidadores.Concat<Usuario>(fisioterapeutas);
                }
            }

            return View(usuariosParaExibir);
        }

        public ActionResult PerfilDetalhado(int id)
        {
            var perfilDetalhado = new PerfilDetalhadoViewModel()
            {
                CuidadorDeIdoso = _context.CuidadoresDeIdoso.SingleOrDefault(p => p.Id == id),
                Fisioterapeuta = _context.Fisioterapeutas.SingleOrDefault(p => p.Id == id)
            };

            if (perfilDetalhado.CuidadorDeIdoso == null && perfilDetalhado.Fisioterapeuta == null)
            {
                return HttpNotFound();
            }
            else
            {
                perfilDetalhado.Curriculo = _context.Curriculos.SingleOrDefault(p => p.Id == id); // colocar relação com usuário
            }

            return View(perfilDetalhado);
        }

        public ActionResult EditarPerfil(int id)
        {
            var cuidadorDeIdoso = _context.CuidadoresDeIdoso.SingleOrDefault(p => p.Id == id);
            var fisioterapeuta = _context.Fisioterapeutas.SingleOrDefault(p => p.Id == id);
            var idoso = _context.Idosos.SingleOrDefault(p => p.Id == id);
            var tutor = _context.Tutores.SingleOrDefault(p => p.Id == id);

            Usuario usuario = MvcApplication.Mapper.Map<Usuario>(tutor);
            return View(); // TODO: Passar todos os valores
        }
    }
}