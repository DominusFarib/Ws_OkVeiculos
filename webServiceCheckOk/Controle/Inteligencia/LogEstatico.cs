using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webServiceCheckOk.Controle.Inteligencia
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;

    public static class LogEstatico
    {
        public static void setLogTexto(string texto)
        {
            try
            {
                System.IO.TextWriter log = System.IO.File.AppendText("C:\\xml_consultas\\logErro.log");

                log.WriteLine(texto);
                log.WriteLine("\n");
                log.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Falha ao escrever arquivo de log", e);
            }
        }

        public static void setLogTitulo(string titulo)
        {
            try
            {
                System.IO.TextWriter log = System.IO.File.AppendText("C:\\xml_consultas\\logErro.log");
                log.WriteLine(titulo + " \n ------------------------------------------- \n");
                log.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Falha ao escrever arquivo de log", e);
            }
        }

        public static void setLog(string texto, string nome, bool mostraData)
        {
            string diaHora = DateTime.Now.ToString("dd/MM/yyyy") + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
            try
            {
                DateTime datetime = DateTime.Today;
                string path = "D:\\xml_consultas\\" + texto + ".log";

                using (StreamWriter sw = (File.Exists(path)) ? File.AppendText(path) : File.CreateText(path))
                {
                    sw.WriteLine((mostraData ? diaHora : "") + " - " + texto);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Falha ao criar ou escrever arquivo de log", e);
            }
        }

    }
}