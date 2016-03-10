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
        public Pessoa dadosPessoa;
        [XmlElement("DADOS_VEICULO")]
        public Veiculo dadosVeiculo;
        [XmlElement("FEATURES")]
        public AuxParametrosFeatures features;
    }

    // PARAMETROS ENVIADOS PELO USUARIO
    [Serializable]
    public class AuxParametrosFeatures
    {
        [XmlElement("FEAT_ROUBOFURTO")]
        public bool featRouboFurto;
	    [XmlElement("FEAT_STF")]
	    public bool featSTF;
	    [XmlElement("FEAT_STJ")]
	    public bool featSTJ;
	    [XmlElement("FEAT_TST")]
	    public bool featTST;
	    [XmlElement("FEAT_GRAVAME")]
	    public bool featGravame;
	    [XmlElement("FEAT_PRECIFICADOR")]
	    public bool featPrecificador;
	    [XmlElement("FEAT_LEILAO")]
	    public bool featLeilao;
	    [XmlElement("FEAT_PERDATOTAL")]
	    public bool featPerdaTotal;
	    [XmlElement("FEAT_BASENACIONAL")]
	    public bool featBaseNacional;
        [XmlElement("FEAT_PROPRIETARIOS")]
        public bool featProprietario;

        public AuxParametrosFeatures(bool ftRouboFurto, bool ftSTF, bool ftSTJ, bool ftTST, bool ftGravame, bool ftPrecificador, bool ftLeilao, bool ftPerdaTotal, bool ftBaseNacional, bool ftProprietario)
        {
            this.featRouboFurto = ftRouboFurto ? ftRouboFurto : featRouboFurto;
            this.featSTF = ftSTF ? ftSTF : featSTF;
            this.featSTJ = ftSTJ ? ftSTJ : featSTJ;
            this.featTST = ftTST ? ftTST : featTST;
            this.featGravame = ftGravame ? ftGravame : featGravame;
            this.featPrecificador = ftPrecificador ? ftPrecificador : featPrecificador;
            this.featLeilao = ftLeilao ? ftLeilao : featLeilao;
            this.featPerdaTotal = ftPerdaTotal ? ftPerdaTotal : featPerdaTotal;
            this.featBaseNacional = ftBaseNacional ? ftBaseNacional : featBaseNacional;
            this.featProprietario = ftProprietario ? ftProprietario : featProprietario;
        }

        public AuxParametrosFeatures(){}
    }

    #endregion
}