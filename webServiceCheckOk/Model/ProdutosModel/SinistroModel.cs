using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using webServiceCheckOk.Controle.Inteligencia;

namespace webServiceCheckOk.Model.ProdutosModel
{
    [Serializable]
    [XmlRoot("ACIDENTES")]
    public class SinistroModel
    {
        [XmlElement("DESCRICAO")]
        public string Descricao { get; set; }

        [XmlElement("DT_REGISTRO")]
        public string DtRegistro { get; set; }

        [XmlElement("MENSAGEM")]
        public Erros MsgSinistro { get; set; }

        [XmlElement("CLASSIFICACAO")]
        public string Classificacao { get; set; }

        [XmlElement("ERRO")]
        public Erros ErroSinistro { get; set; }
    }
}