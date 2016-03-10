using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using webServiceCheckOk.Controle.Inteligencia;

namespace webServiceCheckOk.Model.ProdutosModel
{
    [Serializable]
    [XmlRoot("DEBITOS_MULTAS")]
    public class DebitMultasModel
    {
        [XmlElement("ULTIMO_LICENCIAMENTO")]
        public string AnoLicenciamento { get; set; }

        [XmlElement("COMUNICADO_VENDA")]
        public string ComunicadoVenda { get; set; }
        
        [XmlElement("MENSAGEM")]
        public Erros MsgPrecificador { get; set; }

        [XmlElement("ERRO")]
        public Erros ErroPrecificador { get; set; }

        [XmlElement("CLONAGEM")]
        public string Clonagem { get; set; }

        [XmlElement("IMPEDIMENTO")]
        public string Impedimento { get; set; }

        [XmlElement("TOTAL_DEBITOS")]
        public string TotalDebitos { get; set; }

        [XmlArray("IPVA")]
        [XmlArrayItem("REGISTRO")]
        public List<AuxDebitos> Ipva { get; set; }

        [XmlArray("DPVAT")]
        [XmlArrayItem("REGISTRO")]
        public List<AuxDebitos> Dpvat { get; set; }

        [XmlArray("LICENCIAMENTO")]
        [XmlArrayItem("REGISTRO")]
        public List<AuxDebitos> Licenciamento { get; set; }

        [XmlArray("MULTAS")]
        [XmlArrayItem("REGISTRO")]
        public List<AuxInfracao> Multas { get; set; }

        public DebitMultasModel() { }

    }
    
#region Classes Auxiliares

    public class AuxDebitos
    {
        [XmlElement("ANO")]
        public string Ano { get; set; }

        [XmlElement("DT_VENCIMENTO")]
        public string DtVencimento { get; set; }

        [XmlElement("VALOR_DEBITO")]
        public string Valor { get; set; }

        [XmlElement("DESCRICAO")]
        public string Descricao { get; set; }

        [XmlElement("QUANTIDADE")]
        public string Quantidade { get; set; }

        public AuxDebitos(string ano, string dtVencimento, string valor) 
        {
            this.Ano = ano;
            this.DtVencimento = dtVencimento;
            this.Valor = valor;
        }

        public AuxDebitos() { }
    }

    public class AuxInfracao
    {
        [XmlElement("DT_INFRACAO")]
        public string Data { get; set; }

        [XmlElement("INFRACAO")]
        public string Infracao { get; set; }

        [XmlElement("COD_INFRACAO")]
        public string CodInfracao { get; set; }

        [XmlElement("TIPO")]
        public string Tipo { get; set; }

        [XmlElement("GRAVIDADE")]
        public string Gravidade { get; set; }
        
        [XmlElement("PONTUACAO")]
        public string Pontos { get; set; }

        [XmlElement("VALOR")]
        public string Valor { get; set; }

        [XmlElement("SITUACAO")]
        public string Situacao { get; set; }

        public AuxInfracao(string data, string infracao, string valor)
        {
            this.Data = data;
            this.Infracao = infracao;
            this.Valor = valor;
        }

        public AuxInfracao() { }
    }
#endregion
}