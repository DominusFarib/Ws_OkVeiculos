using System;
using System.Net;

namespace webServiceCheckOk.Controle.Inteligencia.Utils
{
    public class CustomTimeOut : WebClient
    {
        // Tempo em milisegundos
        public int Intervalo { get; set; }

        public CustomTimeOut() : this(60000) { }

        public CustomTimeOut(int intervalo)
        {
            this.Intervalo = intervalo;
        }

        protected override WebRequest GetWebRequest(Uri url)
        {
            var request = base.GetWebRequest(url);

            if (request != null)
                request.Timeout = this.Intervalo;

            return request;
        }
    }
}