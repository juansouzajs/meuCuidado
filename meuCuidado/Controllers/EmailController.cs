using System.Net.Mail;
using System.Net;
using meuCuidado.Dominio.Models;
using System;
using System.Web.Mvc;

namespace meuCuidado.Controllers
{
    public class EmailController : Controller
    {
        private readonly string _email = "juan.live45@gmail.com";
        private readonly string _senhaDeApp = "wcpk jfdi pmda ltmt";
        public void EnviarEmail(Ajuda model)
        {
            var mensagem = new MailMessage();
            mensagem.From = new MailAddress("72000953@aluno.faculdadecotemig.br");
            mensagem.To.Add("72000953@aluno.faculdadecotemig.br");
            mensagem.Subject = model.Titulo;
            mensagem.Body = model.Descricao 
            + "\nTelefone: " + model.Telefone
            + "\nEmail: " + model.Email
            + "\nForma de Retorno: " + model.FormaDeRetorno;
            mensagem.IsBodyHtml = false;

            using (var smtp = new SmtpClient())
            {
                smtp.Host = "smtp.gmail.com"; // mesmo host configurado no web.config
                smtp.Port = 587; // mesmo port configurado no web.config
                smtp.EnableSsl = true; // mesmo enableSsl configurado no web.config
                smtp.Credentials = new NetworkCredential(_email, _senhaDeApp);
                smtp.Send(mensagem);
            }
        }

        public int EnviarEmailAutenticacao(string email)
        {
            var mensagem = new MailMessage();
            var codigoAutenticacao = new Random().Next(10000, 99999);

            mensagem.From = new MailAddress("no-reply@gmail.com");
            mensagem.To.Add(email);
            mensagem.Subject = "meuCuidado - Código de Autenticação";
            mensagem.Body = $"Seu código de autenticação é: {codigoAutenticacao}";
            mensagem.IsBodyHtml = false;

            using (var smtp = new SmtpClient())
            {
                smtp.Host = "smtp.gmail.com"; // mesmo host configurado no web.config
                smtp.Port = 587; // mesmo port configurado no web.config
                smtp.EnableSsl = true; // mesmo enableSsl configurado no web.config
                smtp.Credentials = new NetworkCredential(_email, _senhaDeApp);
                smtp.Send(mensagem);
            }

            return codigoAutenticacao;
        }

        public void EnviarResultadoAnaliseCadastro(string email, bool aprovado, string motivo = null)
        {
            var mensagem = new MailMessage();

            mensagem.From = new MailAddress("no-reply@gmail.com");
            mensagem.To.Add(email);

            if (aprovado)
            {
                mensagem.Subject = "meuCuidado - Cadastro aprovado";
                mensagem.Body = "Seu cadastro foi aprovado! Agora você já pode acessar a plataforma.";
            }
            else
            {
                mensagem.Subject = "meuCuidado - Cadastro reprovado";
                mensagem.Body = $"Seu cadastro foi analisado, porém foi reprovado.\n\nMotivo: {motivo} \n\nCorrija os pontos citados e tente novamente!";
            }

            mensagem.IsBodyHtml = false;

            using (var smtp = new SmtpClient())
            {
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(_email, _senhaDeApp);

                smtp.Send(mensagem);
            }
        }

        public string EnviarEmailRecuperacaoSenha(string email)
        {
            var mensagem = new MailMessage();

            var codigo = Guid.NewGuid()
                .ToString("N")
                .Substring(0, 8)
                .ToUpper();

            mensagem.From = new MailAddress("no-reply@gmail.com");
            mensagem.To.Add(email);
            mensagem.Subject = "meuCuidado - Recuperação de Senha";
            mensagem.Body =
                $"Seu código para redefinir a senha é: {codigo}\n\n" +
                "Digite este código na tela de recuperação de senha.";

            mensagem.IsBodyHtml = false;

            using (var smtp = new SmtpClient())
            {
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(
                    _email,
                    _senhaDeApp
                );

                smtp.Send(mensagem);
            }

            return codigo;
        }
    }
}