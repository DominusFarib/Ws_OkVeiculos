using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using webServiceCheckOk.Controle.Inteligencia;

namespace webServiceCheckOk.Model.ProdutosModel
{
    [Serializable]
    public class DecodChassiModel:Veiculo
    {
        [XmlElement("COD_RETORNO")]
        public string CodRetorno { get; set; }

        [XmlElement("DATA_CONSULTA")]
        public string DataConsulta { get; set; }

        [XmlElement("FABRICANTE")]
        public string Fabricante { get; set; }

        [XmlElement("LOCAL_FABRICACAO")]
        public string LocalFabricacao { get; set; }

        [XmlElement("DESCRICAO")]
        public string Descricao { get; set; }

        [XmlElement("REGIAO_GEOGRAFICA")]
        public string Regiao { get; set; }

        [XmlElement("PAIS")]
        public string Pais { get; set; }

        [XmlElement("CCMOTOR")]
        public string CCMotor { get; set; }

        [XmlElement("NR_SERIE")]
        public string NrSerie { get; set; }
        
        [XmlElement("MENSAGEM")]
        public Erros MsgDecodChassi { get; set; }

        [XmlElement("ERRO")]
        public Erros ErroDecodChassi { get; set; }

        public DecodChassiModel() { }
    }
}