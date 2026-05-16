using System;
using static meuCuidado.Dominio.Extensions.EnumExtension;

public class UsuarioCardViewModel
{
    public int Id { get; set; }
    public Guid IdentificadorUnico { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public TipoUsuario TipoUsuario { get; set; }
    public string FotoUrl { get; set; }
}