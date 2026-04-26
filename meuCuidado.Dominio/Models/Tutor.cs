using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace meuCuidado.Dominio.Models
{
    [Table("meuCuidado_Tutor")]
    public class Tutor : Usuario
    {
        [Required]
        public string RelacaoComIdoso { get; set; }

        public virtual IList<Idoso> Idosos { get; set; }

        public bool NecessidadesEspeciais { get; set; }

        public string DescricaoNecessidadesEspeciais { get; set; }

        public virtual IList<Medico> Medicos { get; set; }
    }
}
