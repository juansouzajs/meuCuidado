using meuCuidado.Dominio.Models;
using System;

namespace meuCuidado.Dominio.ViewModels
{
    public class ConexaoViewModel
    {
        public int Id { get; set; }
        public Guid IdentificadorUnico { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public EtapaAtivacao EtapaAtivacao { get; set; }
    }
}