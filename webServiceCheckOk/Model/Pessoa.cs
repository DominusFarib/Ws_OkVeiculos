using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;

namespace webServiceCheckOk.Model
{
    [Serializable]
    public class Pessoa
    {
        [XmlElement("NOME")]
        public string Nome { get; set; }

        [XmlElement("DOCUMENTO")]
        public string Documento { get; set; }

        [XmlElement("TIPO_PESSOA")]
        public string Tipo { get; set; }

        [XmlElement("EMAIL")]
        public string Email { get; set; }

        [XmlElement("WEBSITE")]
        public string WebSite { get; set; }

        [XmlElement("DT_NASCIMENTO")]
        public string DtNascimento { get; set; }

        [XmlElement("IDADE")]
        public string Idade { get; set; }

        [XmlElement("RENDA_APROXIMADA")]
        public string Renda { get; set; }

        [XmlElement("OBSERVACOES")]
        public string Mensagem { get; set; }

        //public LeilaoModel Leilao { get; set; }
        [XmlElement("TELEFONE")]
        public Telefone Telefone { get; set; }

        [XmlElement("ENDERECO")]
        public EnderecoModel Endereco { get; set; }

        [XmlArray("TELEFONES")]
        [XmlArrayItem("TELEFONE")]
        public List<Telefone> Telefones { get; set; }

        [XmlElement("ENDERECOS")]
        public List<EnderecoModel> Enderecos { get; set; }

        #region VALIDACAO
        // CNPJ/CPF
        public static bool validaDocumento(string CpfCnpj)
        {
            // VERIFICA O TAMANHO E LIMPA A FORMATAÇÃO DO DOCUMENTO
            CpfCnpj = Regex.Replace(CpfCnpj, "[^0-9]", string.Empty);

            if ((CpfCnpj.Length != 14) && (CpfCnpj.Length != 11)) return false;

            return CpfCnpj.Length == 11 ? PessoaFisica.validaCPF(CpfCnpj) : PessoaJuridica.validaCNPJ(CpfCnpj);
        }

        #endregion
    }
}