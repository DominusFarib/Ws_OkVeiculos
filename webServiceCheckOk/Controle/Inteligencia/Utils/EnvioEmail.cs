using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace webServiceCheckOk.Controle.Inteligencia.Utils
{
    public class EnvioEmail
    {
        public string layout;

        public void EnviaEmail(string mensagem, string assunto, string email)
        {
            SmtpClient client = new SmtpClient();
            MailAddress rementente = new MailAddress(ConfigurationManager.AppSettings["emailRemetente"]);
            MailAddress para = new MailAddress(email);
            MailAddress copia = new MailAddress("yago.teixeira@checktudo.com.br");
            MailMessage mail = new MailMessage(rementente, para);

            mail.IsBodyHtml = true;
            mail.CC.Add(copia);
            mail.Subject = assunto;
            mail.Body = mensagem;

            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                LogEstatico.setLogTitulo("FALHA AO ENVIAR EMAIL: " + System.DateTime.Now);
                LogEstatico.setLogTexto(ex.Message + ex.StackTrace);
            }
        }

        public string LayoutFornecedor(string fornecedor, string produto, string parametro)
        {
            this.layout = "<html><head></head><body><font color='black' size='+1'> </font><br><br>" +
                                    "<strong><u>Atenção - Consulta Indisponível</u></strong> <br><br>" +
                                    "<strong> Fornecedor : </strong> " + fornecedor + " <br>" +
                                    "<strong> Produto    : </strong> " + produto + " <br>" +
                                    "<strong> Parâmetro  : </strong> " + parametro + " <br>" +
                                    "<strong> Data/Hora da Consulta : </strong> " + DateTime.Now + " <br>" +
                                    "</body></html>\n\n"; ;

            return this.layout;
        }
    }

}