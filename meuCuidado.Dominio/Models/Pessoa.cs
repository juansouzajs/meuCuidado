using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static meuCuidado.Dominio.Extensions.EnumExtension;

namespace meuCuidado.Dominio.Models
{
    [Table("meuCuidado_Pessoa")]
    public class Pessoa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        public int? UsuarioId { get; set; }

        public TipoUsuario? TipoUsuario { get; set; }

        public virtual ICollection<Lembrete> Lembretes { get; set; }
    }
}