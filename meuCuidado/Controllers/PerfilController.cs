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

            var usuarios = new List<Usuario>();

            DateTime? data = null;
            if (!string.IsNullOrEmpty(dataCadastro))
                data = DateTime.Parse(dataCadastro);

            // PROFISSIONAIS vendo idosos/tutores
            if (tipoUsuarioSessao == GetEnumDescription(TipoUsuario.Cuidador) ||
                tipoUsuarioSessao == GetEnumDescription(TipoUsuario.Fisioterapeuta))
            {
                var idosos = _context.Idosos.Where(p => p.EtapaAcesso == EtapaAcesso.AcessoLiberado);
                var tutores = _context.Tutores.Where(p => p.EtapaAcesso == EtapaAcesso.AcessoLiberado);

                if (!string.IsNullOrEmpty(localizacao))
                {
                    idosos = idosos.Where(x => x.Endereco.Contains(localizacao));
                    tutores = tutores.Where(x => x.Endereco.Contains(localizacao));
                }

                if (data.HasValue)
                {
                    idosos = idosos.Where(x => x.DataCadasto >= data);
                    tutores = tutores.Where(x => x.DataCadasto >= data);
                }

                usuarios.AddRange(idosos.ToList());
                usuarios.AddRange(tutores.ToList());
            }
            // IDOSO/TUTOR vendo profissionais
            else
            {
                var cuidadores = _context.CuidadoresDeIdoso.Where(p => p.EtapaAcesso == EtapaAcesso.AcessoLiberado);
                var fisios = _context.Fisioterapeutas.Where(p => p.EtapaAcesso == EtapaAcesso.AcessoLiberado);

                if (!string.IsNullOrEmpty(localizacao))
                {
                    cuidadores = cuidadores.Where(x => x.Endereco.Contains(localizacao));
                    fisios = fisios.Where(x => x.Endereco.Contains(localizacao));
                }

                if (data.HasValue)
                {
                    cuidadores = cuidadores.Where(x => x.DataCadasto >= data);
                    fisios = fisios.Where(x => x.DataCadasto >= data);
                }

                if (tipoUsuario == "1") // Cuidador
                {
                    usuarios.AddRange(cuidadores.ToList());
                }
                else if (tipoUsuario == "2") // Fisio
                {
                    usuarios.AddRange(fisios.ToList());
                }
                else
                {
                    usuarios.AddRange(cuidadores.ToList());
                    usuarios.AddRange(fisios.ToList());
                }
            }

            return PartialView("_UsuariosLista", usuarios);
        }

        public ActionResult PerfilDetalhado(Guid IdentificadorUnico)
        {
            var tipoUsuarioSessao = Session["TipoUsuario"]?.ToString() ?? string.Empty;

            var perfilDetalhado = new PerfilDetalhadoViewModel()
            {
                CuidadorDeIdoso = _context.CuidadoresDeIdoso.SingleOrDefault(p => p.IdentificadorUnico == IdentificadorUnico),
                Fisioterapeuta = _context.Fisioterapeutas.SingleOrDefault(p => p.IdentificadorUnico == IdentificadorUnico),
                Tutor = _context.Tutores.SingleOrDefault(p => p.IdentificadorUnico == IdentificadorUnico),
                Idoso = _context.Idosos.SingleOrDefault(p => p.IdentificadorUnico == IdentificadorUnico),
                TipoUsuario = tipoUsuarioSessao,
                IdUsuario = Session["IdUsuario"]?.ToString() ?? string.Empty
            };

            perfilDetalhado.Curriculo = _context.Curriculos.SingleOrDefault(p => p.IdentificadorUnico == IdentificadorUnico) ?? new Curriculo() { Cursos = new List<string>(), Experiencias = new List<string>(), RedesSociais = new List<string>() }; // colocar relação com usuário
            if (perfilDetalhado.CuidadorDeIdoso == null && perfilDetalhado.Fisioterapeuta == null && perfilDetalhado.Tutor == null && perfilDetalhado.Idoso == null)
            {
                return HttpNotFound();
            }
            else if(tipoUsuarioSessao == GetEnumDescription(TipoUsuario.Idoso) ||
                     tipoUsuarioSessao == GetEnumDescription(TipoUsuario.Tutor))
            {
                perfilDetalhado.Curriculo = _context.Curriculos.SingleOrDefault(p => p.IdentificadorUnico == IdentificadorUnico) ?? new Curriculo() { Cursos = new List<string>(), Experiencias = new List<string>(), RedesSociais = new List<string>() }; // colocar relação com usuário
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