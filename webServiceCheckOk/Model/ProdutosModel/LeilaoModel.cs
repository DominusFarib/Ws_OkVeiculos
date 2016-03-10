using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using webServiceCheckOk.Controle.Inteligencia;

namespace webServiceCheckOk.Model
{
    [Serializable]
    public class LeilaoModel
    {
        [XmlElement("ID_LEILOEIRO")]
        public string IdLeiloeiro { get; set; }
        
        [XmlElement("LEILOEIRO")]
        public string Leiloeiro { get; set; }
        
        [XmlElement("ID_LEILAO")]
        public string IdLeilao { get; set; }
        
        [XmlElement("LOTE")]
        public string Lote { get; set; }
        
        [XmlElement("COD_FORNECEDOR")]
        public string CodFornecedor { get; set; }
        
        [XmlElement("ID_CONSULTA")]
        public string IdConsulta { get; set; }
        
        [XmlElement("DATA_LEILAO")]
        public string DataLeilao { get; set; }
        

        [XmlElement("COMITENTE")]
        public string Comitente { get; set; }
        
        [XmlElement("PATIO")]
        public string Patio { get; set; }

        [XmlElement("QTD_REGISTROS")]
        public string QtdRegistros { get; set; }
        
        [XmlElement("ID_VEICULO")]
        public string IdVeiculo { get; set; }
        
        [XmlElement("CONDICAO_VEICULO")]
        public string CondicaoGeral { get; set; }
        
        [XmlElement("CONDICAO_CHASSI")]
        public string CondicaoChassi { get; set; }
        
        [XmlElement("CONDICAO_MOTOR")]
        public string CondicaoMotor { get; set; }
        
        [XmlElement("AVARIAS")]
        public string Avarias { get; set; }
        
        [XmlElement("CLASSIFICACAO")]
        public string Classificacao { get; set; }
        
        [XmlElement("OBSERVACAO")]
        public string Observacao { get; set; }
        
        [XmlArray("LISTA_FOTOS")]
        [XmlArrayItem(ElementName = "FOTO", NestingLevel = 0)]
        public List<string> Fotos { get; set; }
        
        [XmlElement("MENSAGEM")]
        public Erros MsgLeilao { get; set; }
        
        [XmlElement("ERRO")]
        public Erros ErroLeilao { get; set; }

        [XmlArray("HISTORICO_LEILOES")]
        [XmlArrayItem("OCORRENCIA")]
        public List<AuxHistoricoLeiloes> HistoricoLeilao { get; set; }

        public LeilaoModel(){}

    }

#region CLASSES AUXILIARES

    [Serializable]
    public class AuxFotosVeiculo
    {
        public string chave;
        public string valor;

        public AuxFotosVeiculo(){}
        public AuxFotosVeiculo(string chave, string valor)
        {
            this.chave = chave;
            this.valor = valor;
        }
    }

    [Serializable]
    public class AuxHistoricoLeiloes
    {
        [XmlElement("DADOS_VEICULO")]
        public Veiculo carro;
        [XmlElement("DADOS_OCORRENCIA")]
        public LeilaoModel leilao;

        public AuxHistoricoLeiloes(){}
        public AuxHistoricoLeiloes(Veiculo carro, LeilaoModel leilao = null)
        {
            this.carro = carro;
            this.leilao = leilao;
        }
        public AuxHistoricoLeiloes(LeilaoModel leilao = null)
        {
            this.leilao = leilao;
        }
    }
#endregion

}