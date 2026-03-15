using BCrypt.Net;
using System.Web.Helpers;

public static class SenhaHelper
{

    public static string HashSenha(string senha)
    {
        return BCrypt.Net.BCrypt.HashPassword(senha);
    }

    public static bool VerificarSenha(string senhaDigitada, string hashBanco)
    {
        return BCrypt.Net.BCrypt.Verify(senhaDigitada, hashBanco);
    }

}
