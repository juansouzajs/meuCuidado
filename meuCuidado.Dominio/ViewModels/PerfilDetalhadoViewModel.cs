using meuCuidado.Dominio.Models;
using System.Collections.Generic;
using static meuCuidado.Dominio.Extensions.EnumExtension;

namespace meuCuidado.Dominio.ViewModels
{
    public class PerfilDetalhadoViewModel
    {
        public string FotoUrl { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Youtube { get; set; }
        public List<Avaliacao> Avaliacaos { get; set; }
        public bool TemConexaoAtiva { get; set; }
        public bool ConexaoPendente { get; set; }
        public bool JaAvaliou { get; set; }
        public string TipoUsuario { get; set; }
        public string IdUsuario { get; set; }
        public Curriculo Curriculo { get; set; }
        public Fisioterapeuta Fisioterapeuta { get; set; }
        public CuidadorDeIdoso CuidadorDeIdoso { get; set; }
        public Tutor Tutor { get; set; }
        public Idoso Idoso { get; set; }
    }
}
