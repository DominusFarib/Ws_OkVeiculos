using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using webServiceCheckOk.Controle.Inteligencia;


namespace webServiceCheckOk.Model.ProdutosModel
{
    [Serializable]
    public class BinModel
    {
        [XmlElement("COD_RETORNO")]
        public string CodRetPesquisa { get; set; }
        [XmlElement("COD_FORNECEDOR")]
        public string CodFornecedor { get; set; }
        [XmlElement("MENSAGEM")]
        public Erros MsgBinNacional { get; set; }
        [XmlElement("ERRO")]
        public Erros ErroBinNacional { get; set; }
        [XmlElement("DATA_ULTIMA_ATUALIZACAO")]
        public string DtUltimaAtualizacao { get; set; }
        [XmlElement("DOC_FATURADO")]
        public string DocFaturado { get; set; }
        [XmlElement("TIPO_DOC_FATURADO")]
        public string TipoDocFaturado { get; set; }
        [XmlElement("UF_FATURAMENTO")]
        public string UFFaturado { get; set; }
        [XmlElement("TIPO_DOC_IMPORTADOR")]
        public string TipoDocImportadora { get; set; }
        [XmlElement("DATA_LIMITE_RESTITUICAO_TRIBUTARIA")]
        public string DtLimiteRestricaoTributaria { get; set; }
        [XmlElement("TIPO_CHASSI")]
        public string SituacaoChassi { get; set; }
        [XmlElement("SITUACAO_VEICULO")]
        public string SituacaoVeiculo { get; set; }
        [XmlElement("DADOS_VEICULO")]
        public Veiculo Automovel { get; set; }
        [XmlElement("DADOS_PROPRIETARIO")]
        public Pessoa Proprietario { get; set; }
        [XmlArray("RESTRICOES")]
        [XmlArrayItem("RESTRICAO")]
        public List<AuxRestricoes> Restricoes { get; set; }

        [XmlArray("LISTA_RESTRICOES")]
        [XmlArrayItem("RESTRICAO")]
        public List<string> ListaRestricao { get; set; }

        [XmlElement("OCORRENCIA")]
        public string Ocorrencia { get; set; }
        [XmlElement("INDICA_RESTRICAO_RENAJUD")]
        public string IndicaRestricaoRenajud { get; set; }
        [XmlElement("OBSERVACAO_CONSULTA")]
        public string Obs { get; set; }

        public BinModel() { }

    }

    #region CLASSES AUXILIARES

    [Serializable]
    public class BinEstadualModel : BinModel
    {
        [XmlElement("RESTRICAO_FINANCIAMENTO")]
        public string RestFinanciamento { get; set; }

        [XmlElement("TIPO_TRANSACAO")]
        public string TipoTransacao { get; set; }

        [XmlElement("NUMERO_CONTRATO")]
        public string NrContrato { get; set; }
        
        [XmlElement("DATA_VIGENCIA")]
        public string DtVigenciaContrato { get; set; }

        [XmlElement("NOME_FINANCEIRA")]
        public string Financeira { get; set; }
        
        [XmlElement("DOC_FINANCEIRA")]
        public string DocFinanceira { get; set; }
        
        [XmlElement("NOME_FINANCIADO")]
        public string NomeFinanciado { get; set; }
        
        [XmlElement("DOC_FINANCIADO")]
        public string DocFinanciado { get; set; }

        [XmlElement("AGENTE_FINANCEIRA")]
        public string AgenteFinanceiro { get; set; }

        [XmlElement("DATA_INC_INTENCAO_TROCA")]
        public string DtIntencaoTroca { get; set; }

        [XmlElement("DATA_FIN_ATUALIZACAO")]
        public string DtFinAtualizacao { get; set; }

        [XmlElement("DEBITO_IPVA_LICENCIAMENTO")]
        public string ConstaIPVALicenciamento { get; set; }
        
        [XmlElement("DEBITO_MULTAS")]
        public string ConstaMultas { get; set; }
        
        [XmlElement("TOTAL_IPVA")]
        public string TotalIPVA { get; set; }
        
        [XmlElement("TOTAL_LICENCIAMENTO")]
        public string TotalLicenciamento { get; set; }
        
        [XmlElement("TOTAL_MULTAS")]
        public string TotalMultas { get; set; }

        [XmlElement("TOTAL_DPVAT")]
        public string TotalDpvat { get; set; }

        public BinEstadualModel() { }
    }

    [Serializable]
    public class BinRouboFurtoModel : BinModel
    {
        [XmlArray("LISTA_OCORRENCIAS")]
        [XmlArrayItem("OCORRENCIA")]
        public List<AuxOcorrencisRF> ListaRouboFurto { get; set; }

        public BinRouboFurtoModel() { }
    }
    
    [Serializable]
    public class AuxRestricoes
    {
        [XmlIgnoreAttribute]
        public string chave;
        [XmlElement("")]
        public string valor;

        public AuxRestricoes() { }
        public AuxRestricoes(string chave, string valor)
        {
            this.chave = chave;
            this.valor = valor;
        }
    }

    [Serializable]
    public class AuxOcorrencisRF
    {
        [XmlElement("CATEGORIA_OCORRENCIA")]
        public string CategOcorrencia;

        [XmlElement("NR_OCORRENCIA")]
        public string NrOcorrencia;

        [XmlElement("DT_OCORRENCIA")]
        public string DtOcorrencia;

        [XmlElement("NR_BOLETIM_OCORRENCIA")]
        public string NrBO;

        [XmlElement("ANO_BOLETIM_OCORRENCIA")]
        public string AnoBO;

        [XmlElement("UF_BOLETIM_OCORRENCIA")]
        public string UFBO;

        [XmlElement("ORGAO_SEGURANCA")]
        public string OrgaoSeguranca;

        public AuxOcorrencisRF() { }

        public AuxOcorrencisRF(string chave, string valor)
        {
            this.CategOcorrencia = chave;
            this.UFBO = valor;
        }
    }

    #endregion
}