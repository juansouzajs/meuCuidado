using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace meuCuidado.Dominio.Models
{
    [Table("meuCuidado_Medicamento")]
    public class Medicamento
    {
        public Medicamento()
        {
            Lembretes = new HashSet<Lembrete>();
        }

        [Key]
        public int Id { get; set; }

        public Guid IdentificadorUnico { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Dosagem { get; set; }

        public string FormaFarmaceutica { get; set; }

        public int DuracaoEmDias { get; set; }

        public string Observacoes { get; set; }

        public virtual ICollection<Lembrete> Lembretes { get; set; }
    }
}