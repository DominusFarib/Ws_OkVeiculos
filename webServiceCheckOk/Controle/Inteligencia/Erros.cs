using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace webServiceCheckOk.Controle.Inteligencia
{
    [Serializable]
    public class Erros
    {
        [XmlElement("CODIGO")]
        public string Codigo { get; set; }
        [XmlElement("MENSAGEM")]
        public string Descricao { get; set; }

        public Erros(){}

        public Erros(string descricao)
        {
            this.Codigo = descricao;
        }

        public Erros(string codigo, string descricao)
        {
            this.Codigo = codigo;
            this.Descricao = descricao;
        }

    }


}