using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace webServiceCheckOk.Model
{
    public class PessoaFisica : Pessoa
    {
        public string Cpf { get; set; }
        public string Rg { get; set; }
        public string Cnh { get; set; }
        public string Pis { get; set; }
        public string Ctps { get; set; }
        public string Cor { get; set; }
        public string Signo { get; set; }
        public string Sexo { get; set; }
        public PessoaFisica Mae { get; set; }
        public PessoaFisica Conjuge { get; set; }

        #region VALIDACAO
        // CPF
        public static bool validaCPF(string cpf)
        {
            int soma = 0;
            int digito = 0;

            //  1. VERIFICA SE O DOCUMENTO É VAZIO
            if (string.IsNullOrEmpty(cpf.Trim())) return false;

            //  2. VERIFICA O TAMANHO E LIMPA A FORMATAÇÃO DO DOCUMENTO
            cpf = Regex.Replace(cpf, "[^0-9]", string.Empty);

            if (cpf.Length != 11) return false;

            //  3. CALCULA O 1º DÍGITO
            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse((cpf.Substring(i, 1))) * (10 - i);
            }

            digito = 11 - soma % 11;

            if (digito > 9) digito = 0;

            if (digito != int.Parse(cpf.Substring(9, 1)))
                return false;

            //  4. CALCULA O 2º DÍGITO
            digito = 0;
            soma = 0;

            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse((cpf.Substring(i, 1))) * (11 - i);
            }

            digito = 11 - soma % 11;

            if (digito > 9) digito = 0;

            if (digito != int.Parse(cpf.Substring(10, 1)))
                return false;

            return true;
        }

        #endregion
    }
}