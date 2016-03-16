using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace webServiceCheckOk.Model.ProdutosModel
{
    [Serializable]
    [XmlRoot("OK_VEICULOS")]
    public class OKVeiculos
    {
        [XmlElement("DADOS_CONSULTA")]
        public AuxDadosConsulta Cabecalho { get; set; }

        [XmlElement("MENSAGEM")]
        public string Mensagem { get; set; }

        [XmlElement("VEICULO")]
        public Veiculo veiculo { get; set; }

        [XmlElement("LEILAO")]
        public LeilaoModel Leilao { get; set; }

        [XmlElement("GRAVAME")]
        public GravameModel Gravame { get; set; }

        [XmlElement("REGISTRO_FEDERAL")]
        public BinModel BinNacional { get; set; }

        [XmlElement("REGISTRO_ESTADUAL")]
        public BinEstadualModel BinEstadual { get; set; }

        [XmlElement("ROUBO_FURTO_FEDERAL")]
        public BinRouboFurtoModel BinRouboFurto { get; set; }

        [XmlElement("AGREGADOS")]
        public AgregadosModel Agregados { get; set; }

        [XmlElement("PRECIFICADOR")]
        public PrecificadorModel Precificador { get; set; }

        [XmlElement("DECODIFICADOR_CHASSI")]
        public DecodChassiModel DecodChassi { get; set; }

        [XmlElement("SINISTRO")]
        public SinistroModel PerdaTotal { get; set; }

        [XmlArray("SINISTROS")]
        [XmlArrayItem("REGISTRO")]
        public List<SinistroModel> Sinistros { get; set; }

        public OKVeiculos()
        {
            this.veiculo = new Veiculo();
        }
    }

    #region AUXILIARES
    
    [Serializable]
    public class AuxDadosConsulta
    {
        [XmlElement("CODIGO")]
        public string Codigo { get; set; }
        
        [XmlElement("NOME")]
        public string Nome { get; set; }

        [XmlElement("DATA")]
        public string Data { get; set; }

        [XmlElement("HORA")]
        public string Hora { get; set; }

        [XmlElement("PARAMETROS")]
        public AuxParametros Parametros { get; set; }

        public AuxDadosConsulta(string codigo = null, string nome = null, AuxParametros parametros = null)
        {
            this.Codigo = codigo;
            this.Nome = nome;
            this.Hora = DateTime.Now.ToString("HH:mm:ss");
            this.Data = DateTime.Now.ToString("dd/MM/yyyy");
            this.Parametros = parametros;
        }

        public AuxDadosConsulta(){ }
    }

    // PARAMETROS ENVIADOS PELO USUARIO
    [Serializable]
    public class AuxParametros
    {
        [XmlElement("DADOS_PESSOA")]
        public Pessoa dadosPessoa { get; set; }
        [XmlElement("DADOS_VEICULO")]
        public Veiculo dadosVeiculo { get; set; }
        [XmlElement("FEATURES_ADICIONAIS")]
        public AuxParametrosFeatures features { get; set; }
    }

    // PARAMETROS ENVIADOS PELO USUARIO
    [Serializable]
    public class AuxParametrosFeatures
    {
        [XmlElement("FEAT_ROUBOFURTO")]
        public string featRouboFurto { get; set; }
	    [XmlElement("FEAT_STF")]
        public string featSTF { get; set; }
	    [XmlElement("FEAT_STJ")]
        public string featSTJ { get; set; }
	    [XmlElement("FEAT_TST")]
        public string featTST { get; set; }
	    [XmlElement("FEAT_GRAVAME")]
        public string featGravame { get; set; }
	    [XmlElement("FEAT_PRECIFICADOR")]
        public string featPrecificador { get; set; }
	    [XmlElement("FEAT_LEILAO")]
        public string featLeilao { get; set; }
	    [XmlElement("FEAT_PERDATOTAL")]
        public string featPerdaTotal { get; set; }
	    [XmlElement("FEAT_BASENACIONAL")]
        public string featBaseNacional { get; set; }
        [XmlElement("FEAT_PROPRIETARIOS")]
        public string featProprietario { get; set; }

        public AuxParametrosFeatures(bool ftRouboFurto, bool ftSTF, bool ftSTJ, bool ftTST, bool ftGravame, bool ftPrecificador, bool ftLeilao, bool ftPerdaTotal, bool ftBaseNacional, bool ftProprietario)
        {
            this.featRouboFurto = ftRouboFurto ? "SIM" : null;
            this.featSTF = ftSTF ? "SIM" : null;
            this.featSTJ = ftSTJ ? "SIM" : null;
            this.featTST = ftTST ? "SIM" : null;
            this.featGravame = ftGravame ? "SIM" : null;
            this.featPrecificador = ftPrecificador ? "SIM" : null;
            this.featLeilao = ftLeilao ? "SIM" : null;
            this.featPerdaTotal = ftPerdaTotal ? "SIM" : null;
            this.featBaseNacional = ftBaseNacional ? "SIM" : null;
            this.featProprietario = ftProprietario ? "SIM" : null;
        }

        public AuxParametrosFeatures(){}
    }

    #endregion
}