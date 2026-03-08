using System.Net.Mail;
using System.Net;
using meuCuidado.Dominio.Models;
using System;
using System.Web.Mvc;

namespace meuCuidado.Controllers
{
    public class EmailController : Controller
    {
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
                smtp.Credentials = new NetworkCredential("diovan.taylor@gmail.com", "jdit yrav nsjw qjwj");
                smtp.Send(mensagem);
            }
        }

        public int EnviarEmailAutenticacao(string email)
        {
            var mensagem = new MailMessage();
            var codigoAutenticacao = new Random().Next(10000, 99999);

            mensagem.From = new MailAddress("no-reply@gmail.com");
            mensagem.To.Add(email);
            mensagem.Subject = "Código de Autenticação";
            mensagem.Body = $"Seu código de autenticação é: {codigoAutenticacao}";
            mensagem.IsBodyHtml = false;

            using (var smtp = new SmtpClient())
            {
                smtp.Host = "smtp.gmail.com"; // mesmo host configurado no web.config
                smtp.Port = 587; // mesmo port configurado no web.config
                smtp.EnableSsl = true; // mesmo enableSsl configurado no web.config
                smtp.Credentials = new NetworkCredential("juan.live45@gmail.com", "wcpk jfdi pmda ltmt");
                smtp.Send(mensagem);
            }

            return codigoAutenticacao;
        }
    }
}