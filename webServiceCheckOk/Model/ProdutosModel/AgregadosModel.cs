using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using webServiceCheckOk.Controle.Inteligencia;

namespace webServiceCheckOk.Model.ProdutosModel
{
    [Serializable]
    public class AgregadosModel
    {
        [XmlElement("COD_RETORNO")]
        public string CodRetorno { get; set; }

        [XmlElement("ID_CONSULTA")]
        public string IdConsulta { get; set; }

        [XmlElement("DATA_CONSULTA")]
        public string DataConsulta { get; set; }

        [XmlElement("MENSAGEM")]
        public Erros MsgAgregados { get; set; }

        [XmlElement("ERRO")]
        public Erros ErroAgregados { get; set; }

        [XmlArray("OCORRENCIAS")]
        [XmlArrayItem("REGISTRO")]
        public List<Veiculo> ListaVeiculos { get; set; }

        public AgregadosModel() { }
    }
}