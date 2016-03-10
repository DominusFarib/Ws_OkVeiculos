using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using webServiceCheckOk.Controle.Inteligencia;

namespace webServiceCheckOk.Model.ProdutosModel
{
    [Serializable]
    public class PrecificadorModel:Veiculo
    {
        [XmlElement("COD_RETORNO")]
        public string CodRetorno { get; set; }

        [XmlElement("DATA_CONSULTA")]
        public string DataConsulta { get; set; }
        
        [XmlElement("MENSAGEM")]
        public Erros MsgPrecificador { get; set; }

        [XmlElement("ERRO")]
        public Erros ErroPrecificador { get; set; }

        [XmlElement("COD_FIPE")]
        public string CodFipe { get; set; }

        [XmlElement("VALOR")]
        public string Valor { get; set; }

        [XmlArray("OCORRENCIAS")]
        [XmlArrayItem("REGISTRO")]
        public List<PrecificadorModel> RegistrosPrecos { get; set; }

        public PrecificadorModel() { }
    }
}