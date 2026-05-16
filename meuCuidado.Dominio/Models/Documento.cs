using meuCuidado.Dominio.Models;
using System;
using static meuCuidado.Dominio.Extensions.EnumExtension;

public class Documento
{
    public Guid Id { get; set; }  // Identificador único do documento
    public TipoDocumento TipoDocumento { get; set; }  // Tipo do documento (Foto, Documento, Certificado, etc.)
    public TipoExtensaoDocumento Extensao { get; set; }  // Tipo da extensão (.PNG, .PDF)
    public string Caminho { get; set; }  // Caminho onde o arquivo foi salvo no servidor
    public int UsuarioId { get; set; }  // Chave estrangeira para associar o documento ao usuário
    public string Descricao { get; set; }  // Descrição do documento (FOTO DE PERFIL)
    public TipoUsuario TipoUsuario { get; set; }  // Tipo do usuário associado ao documento (Cuidador, Fisioterapeuta, Idoso, Tutor)
    public DateTime DataUpload { get; set; }  // Data e hora do upload do documento
}
