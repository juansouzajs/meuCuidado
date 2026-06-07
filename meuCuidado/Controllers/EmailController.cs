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
            mensagem.Body = GerarTemplateEmail(
                "Código de Autenticação",
                $@"
                    <p>Olá!</p>

                    <p>Utilize o código abaixo para concluir sua autenticação:</p>

                    <div style='
                        text-align:center;
                        font-size:42px;
                        font-weight:bold;
                        color:#4db6ac;
                        margin:30px 0;'>

                        {codigoAutenticacao}

                    </div>

                    <p>Se você não solicitou este código, ignore este e-mail.</p>",
                                "🔐");

            mensagem.IsBodyHtml = true;

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
                mensagem.Body = GerarTemplateEmail(
                    "Cadastro aprovado",
                    @"
                        <p>
                            Sua documentação foi analisada e aprovada.
                        </p>

                        <p>
                            Agora você já pode acessar a plataforma
                            normalmente e utilizar todos os recursos disponíveis.
                        </p>",
                    "✅");
            }
            else
            {
                mensagem.Subject = "meuCuidado - Cadastro reprovado";
                mensagem.Body = GerarTemplateEmail(
                    "Cadastro não aprovado",
                    $@"
                        <p>
                            Sua documentação foi analisada, porém
                            não foi aprovada nesta etapa.
                        </p>

                        <p>
                            <strong>Motivo:</strong><br/>
                            {motivo}
                        </p>

                        <p>
                            Corrija os pontos informados e realize
                            um novo envio para análise.
                        </p>",
                    "⚠️");
            }

            mensagem.IsBodyHtml = true;

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
            mensagem.Body = GerarTemplateEmail(
                 "Recuperação de senha",
                     $@"
                        <p>
                            Recebemos uma solicitação para redefinição de senha.
                        </p>

                        <div style='
                            text-align:center;
                            font-size:42px;
                            font-weight:bold;
                            color:#4db6ac;
                            margin:30px 0;'>

                            {codigo}

                        </div>

                        <p>
                            Digite este código na tela de recuperação de senha.
                        </p>

                        <p>
                            Caso não tenha feito esta solicitação,
                            ignore este e-mail.
                        </p>",
                 "🔑");

            mensagem.IsBodyHtml = true;

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

        public void EnviarSolicitacaoConexao(
            string emailDestino,
            string nomeProfissional,
            string nomeSolicitante)
        {
            var mensagem = new MailMessage();

            mensagem.From = new MailAddress("no-reply@gmail.com");
            mensagem.To.Add(emailDestino);

            mensagem.Subject = "meuCuidado - Solicitação de conexão";

            mensagem.Body = GerarTemplateEmail(
                "Nova solicitação de conexão",
                    $@"
                    <p>
                        <strong>{nomeSolicitante}</strong>
                        enviou uma solicitação de conexão.
                    </p>

                    <p>
                        Acesse a plataforma para visualizar
                        e responder a solicitação.
                    </p>",
                "🤝");

            mensagem.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(_email, _senhaDeApp);

                smtp.Send(mensagem);
            }
        }

        public void EnviarConexaoAprovada(
            string emailDestino,
            string nomeProfissional)
        {
            var mensagem = new MailMessage();

            mensagem.From = new MailAddress("no-reply@gmail.com");
            mensagem.To.Add(emailDestino);

            mensagem.Subject = "meuCuidado - Conexão aprovada";

            mensagem.Body = GerarTemplateEmail(
                 "Conexão aprovada",
                     $@"
                    <p>
                        Sua solicitação de conexão com
                        <strong>{nomeProfissional}</strong>
                        foi aprovada.
                    </p>

                    <p>
                        Agora vocês já podem interagir através
                        dos recursos disponíveis na plataforma.
                    </p>",
                 "🎊");

            mensagem.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(_email, _senhaDeApp);

                smtp.Send(mensagem);
            }
        }

        public void EnviarConexaoRejeitada(
            string emailDestino,
            string nomeProfissional)
        {
            var mensagem = new MailMessage();

            mensagem.From = new MailAddress("no-reply@gmail.com");
            mensagem.To.Add(emailDestino);

            mensagem.Subject = "meuCuidado - Conexão recusada";

            mensagem.Body = GerarTemplateEmail(
                 "Conexão recusada",
                     $@"
                    <p>
                        Sua solicitação de conexão com
                        <strong>{nomeProfissional}</strong>
                        não foi aprovada.
                    </p>

                    <p>
                        Você pode continuar buscando outros
                        profissionais disponíveis na plataforma.
                    </p>",
                 "📩");

            mensagem.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(_email, _senhaDeApp);

                smtp.Send(mensagem);
            }
        }

        public void EnviarEmailCadastroConcluido(string email, string nome)
        {
            var mensagem = new MailMessage();

            mensagem.From = new MailAddress("no-reply@gmail.com");
            mensagem.To.Add(email);

            mensagem.Subject = "meuCuidado - Cadastro realizado com sucesso";

            mensagem.Body = GerarTemplateEmail(
                 "Cadastro realizado com sucesso",
                     $@"
                    <p>Olá <strong>{nome}</strong>,</p>

                    <p>
                        Seu cadastro foi criado com sucesso na plataforma
                        <strong>meuCuidado</strong>.
                    </p>

                    <p>
                        Agora você já pode acessar sua conta e encontrar
                        profissionais qualificados para oferecer mais segurança,
                        conforto e qualidade de vida.
                    </p>

                    <p>
                        Seja bem-vindo(a)!
                    </p>",
                 "🎉");

            mensagem.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(_email, _senhaDeApp);

                smtp.Send(mensagem);
            }
        }

        public void EnviarEmailCadastroEmAnalise(string email, string nome)
        {
            var mensagem = new MailMessage();

            mensagem.From = new MailAddress("no-reply@gmail.com");
            mensagem.To.Add(email);

            mensagem.Subject = "meuCuidado - Cadastro em análise";

            mensagem.Body = GerarTemplateEmail(
                 "Cadastro recebido",
                 $@"
                    <p>Olá <strong>{nome}</strong>,</p>

                    <p>
                        Recebemos seu cadastro e sua documentação com sucesso.
                    </p>

                    <p>
                        Nossa equipe realizará uma análise para garantir
                        a segurança e a qualidade dos profissionais cadastrados.
                    </p>

                    <p>
                        Assim que a análise for concluída, você receberá
                        um novo e-mail com o resultado.
                    </p>",
                 "⏳");

            mensagem.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(_email, _senhaDeApp);

                smtp.Send(mensagem);
            }
        }

        private string GerarTemplateEmail(
            string titulo,
            string mensagem,
            string icone = "💚")
        {
            return $@"
                    <!DOCTYPE html>
                    <html lang='pt-BR'>
                    <head>
                    <meta charset='utf-8'>
                    </head>

                    <body style='margin:0;padding:0;background:#f8fbff;font-family:Segoe UI,Arial,sans-serif;'>

                        <table width='100%' cellpadding='0' cellspacing='0' style='background:#f8fbff;padding:40px 20px;'>

                            <tr>
                                <td align='center'>

                                    <table width='650' cellpadding='0' cellspacing='0'
                                           style='background:white;border-radius:20px;overflow:hidden;
                                                  box-shadow:0 10px 30px rgba(0,0,0,.08);'>

                                        <!-- HEADER -->

                                        <tr>
                                            <td align='center'
                                                style='background:linear-gradient(135deg,#4db6ac,#67c9bf);
                                                       padding:40px;'>

                                                <h1 style='margin:0;color:white;font-size:40px;font-weight:800;'>
                                                    meuCuidado
                                                </h1>

                                                <p style='margin-top:10px;color:white;font-size:16px;'>
                                                    Conectando famílias aos melhores profissionais de cuidado.
                                                </p>

                                            </td>
                                        </tr>

                                        <!-- CONTEÚDO -->

                                        <tr>
                                            <td style='padding:50px;'>

                                                <div style='text-align:center;font-size:55px;'>
                                                    {icone}
                                                </div>

                                                <h2 style='
                                                    color:#2f6f68;
                                                    text-align:center;
                                                    margin-top:20px;
                                                    font-size:32px;'>
                                                    {titulo}
                                                </h2>

                                                <div style='
                                                    color:#555;
                                                    font-size:17px;
                                                    line-height:1.8;
                                                    margin-top:25px;'>

                                                    {mensagem}

                                                </div>

                                            </td>
                                        </tr>

                                        <!-- BENEFÍCIOS -->

                                        <tr>
                                            <td style='padding:0 50px 40px 50px;'>

                                                <div style='
                                                    background:#f7fbfb;
                                                    border-radius:15px;
                                                    padding:25px;'>

                                                    <h3 style='margin-top:0;color:#2f6f68;'>
                                                        Por que utilizar o meuCuidado?
                                                    </h3>

                                                    <p style='margin:8px 0;color:#555;'>
                                                        ✓ Profissionais verificados
                                                    </p>

                                                    <p style='margin:8px 0;color:#555;'>
                                                        ✓ Conexões seguras entre famílias e especialistas
                                                    </p>

                                                    <p style='margin:8px 0;color:#555;'>
                                                        ✓ Mais segurança, conforto e qualidade de vida
                                                    </p>

                                                </div>

                                            </td>
                                        </tr>

                                        <!-- LGPD -->

                                        <tr>
                                            <td style='padding:0 50px 40px 50px;'>

                                                <div style='
                                                    border-left:4px solid #4db6ac;
                                                    background:#f8fbff;
                                                    padding:20px;
                                                    color:#666;
                                                    font-size:14px;
                                                    line-height:1.7;'>

                                                    🔒 Seus dados são tratados de acordo com a LGPD e
                                                    utilizados exclusivamente para o funcionamento da
                                                    plataforma meuCuidado.

                                                </div>

                                            </td>
                                        </tr>

                                        <!-- FOOTER -->

                                        <tr>
                                            <td align='center'
                                                style='background:#233a38;
                                                       padding:30px;
                                                       color:white;'>

                                                <p style='margin:0;font-size:18px;font-weight:600;'>
                                                    Equipe meuCuidado
                                                </p>

                                                <p style='margin-top:10px;font-size:13px;opacity:.85;'>

                                                    Este é um e-mail automático.
                                                    Não responda esta mensagem.

                                                </p>

                                                <p style='margin-top:15px;font-size:12px;opacity:.7;'>

                                                    © {DateTime.Now.Year} meuCuidado

                                                </p>

                                            </td>
                                        </tr>

                                    </table>

                                </td>
                            </tr>

                        </table>

                    </body>
                    </html>"; 
        }
    }
}