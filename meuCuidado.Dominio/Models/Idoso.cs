using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace meuCuidado.Dominio.Models
{
    [Table("meuCuidado_Idoso")]
    public class Idoso : Usuario
    {
        [Required]
        public DateTime DataNascimento { get; set; }

        public bool NecessidadesEspeciais { get; set; }

        public string DescricaoNecessidadesEspeciais { get; set; }

        [Required]
        public int TutorId { get; set; }

        [ForeignKey(nameof(TutorId))]
        public virtual Tutor Tutor { get; set; }

        public virtual IList<Medico> Medicos { get; set; }

        public virtual RelacionamentoIdosoProfissional RelacionamentoIdosoProfissional { get; set; }
    }
}