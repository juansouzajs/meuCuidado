using System;

namespace meuCuidado.Dominio.ViewModels
{
    public class SolicitacaoProfissionalViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string TipoProfissional { get; set; }

        public DateTime DataSolicitacao { get; set; }
    }
}
