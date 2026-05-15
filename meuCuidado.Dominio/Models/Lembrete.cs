using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace meuCuidado.Dominio.Models
{
    [Table("meuCuidado_Lembrete")]
    public class Lembrete
    {
        [Key]
        public int Id { get; set; }

        public Guid IdentificadorUnico { get; set; }

        public int? PessoaId { get; set; }

        [ForeignKey("PessoaId")]
        public virtual Pessoa Pessoa { get; set; }

        public int? MedicamentoId { get; set; }

        [ForeignKey("MedicamentoId")]
        public virtual Medicamento Medicamento { get; set; }

        public string Descricao { get; set; }

        public DateTime? DataHoraPrimeiroAlerta { get; set; }

        [Required]
        public DateTime DataHora { get; set; }

        [Required]
        public bool Repete { get; set; } = false;
    }
}