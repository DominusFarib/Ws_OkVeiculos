using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using webServiceCheckOk.Controle.Inteligencia;

namespace webServiceCheckOk.Model.ProdutosModel
{
    [Serializable]
    public class GravameModel
    {
        [XmlElement("ID_CONSULTA")]
        public string IdConsulta { get; set; }
        
        [XmlElement("MENSAGEM")]
        public Erros MsgGravame { get; set; }
        
        [XmlElement("DADOS_VEICULO")]
        public Veiculo Carro { get; set; }

        [XmlArray("GRAVAMES ")]
        [XmlArrayItem("OCORRENCIA ")]
        public List<AuxHistoricoGravames> HistoricoGravames { get; set; }

        [XmlArray("HISTORICO_GRAVAMES")]
        [XmlArrayItem("OCORRENCIA")]
        public List<GravameModel> OcorrenciasGravame { get; set; }

        [XmlElement("STATUS")]
        public string StatusVeiculo { get; set; }

        [XmlElement("NOME_FINANCIADO")]
        public string NomeFinanciado { get; set; }
        
        [XmlElement("DOCUMENTO_FINANCIADO")]
        public string DocumentoFinanciado { get; set; }

        [XmlElement("NOME_AGENTE")]
        public string NomeAgente { get; set; }
        
        [XmlElement("DOCUMENTO_AGENTE")]
        public string DocumentoAgente { get; set; }
        
        [XmlElement("NR_GRAVAME")]
        public string NrGravame { get; set; }
        
        [XmlElement("NR_CONTRATO")]
        public string NrContrato { get; set; }
        
        [XmlElement("UF_GRAVAME")]
        public string UFGravame { get; set; }

        [XmlElement("DATA_CONTRATO")]
        public string DataContrato { get; set; }

        [XmlElement("DATA_INCLUSAO_GRAVAME")]
        public string DataInclusaoGravame { get; set; }
        
        [XmlElement("REMARCACAO_CHASSI")]
        public string RemarcacaoChassi { get; set; }
        
        [XmlElement("UF_PLACA")]
        public string UFPlaca { get; set; }
        
        [XmlElement("UF_LICENCIAMENTO")]
        public string UFLicenciamento { get; set; }
        
        [XmlElement("CODIGO_CONSULTA")]
        public string CodConsulta { get; set; }

        [XmlElement("CODIGO_FORNECEDOR")]
        public string CodFornecedor { get; set; }
        
        [XmlElement("ERRO")]
        public Erros ErroGravame { get; set; }

        [XmlElement("ASSINATURA_ELETRONICA")]
        public string AssEletronica { get; set; }

        public GravameModel(){}
    }

    #region CLASSES AUXILIARES

    [Serializable]
    public class AuxHistoricoGravames
    {
        [XmlElement("DADOS_VEICULO")]
        public Veiculo veiculo;
        [XmlElement("DADOS_OCORRENCIA")]
        public GravameModel gravame;

        public AuxHistoricoGravames() { }

        public AuxHistoricoGravames(Veiculo carro = null, GravameModel gravame = null)
        {
            this.veiculo = carro;
            this.gravame = gravame;
        }

        public AuxHistoricoGravames(GravameModel gravame = null)
        {
            this.gravame = gravame;
        }

    }

    #endregion
}