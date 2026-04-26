using System;
using System.Collections.Generic;
using static meuCuidado.Dominio.Extensions.EnumExtension;

namespace meuCuidado.Dominio.Models
{
    public class Curriculo
    {
        public int Id { get; set; }
        public Guid IdentificadorUnico { get; set; }
        public string Nome { get; set; }
        public int AnosExperiencia { get; set; }
        public string Escolaridade { get; set; }
        public List<string> Cursos { get; set; }
        public List<string> Experiencias { get; set; }
        public List<string> RedesSociais { get; set; }
        public double AvaliacaoMedia { get; set; }
        public int NumeroAvaliacoes { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public int UsuarioId { get; set; }
    }
}
