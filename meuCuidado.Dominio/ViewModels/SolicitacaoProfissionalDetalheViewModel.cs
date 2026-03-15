using System.Collections.Generic;

namespace meuCuidado.Dominio.ViewModels
{
    public class SolicitacaoProfissionalDetalheViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string Telefone { get; set; }

        public string TipoProfissional { get; set; }

        public string CPF { get; set; }

        public List<DocumentoViewModel> Documentos { get; set; } = new List<DocumentoViewModel>();
    }
}
