using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace webServiceCheckOk.Model
{
    public class PessoaJuridica : Pessoa
    {
        public string NomeFantasia { get; set; }
        public string Cnpj { get; set; }

        #region VALIDACAO
        // CNPJ
        public static bool validaCNPJ(string cnpj)
        {
            int soma = 0;
            int digito = 0;

            //  1. VERIFICA SE O DOCUMENTO É VAZIO
            if (string.IsNullOrEmpty(cnpj.Trim())) return false;

            //  2. VERIFICA O TAMANHO E LIMPA A FORMATAÇÃO DO DOCUMENTO
            cnpj = Regex.Replace(cnpj, "[^0-9]", string.Empty);
            if (cnpj.Length != 14) return false;

            //  3. CALCULA O 1º DÍGITO
            for (int i = 0; i < 12; i++)
            {
                if (i < 4)
                    soma += int.Parse(cnpj.Substring(i, 1)) * (5 - i);
                else
                    soma += int.Parse(cnpj.Substring(i, 1)) * (13 - i);
            }

            digito = 11 - soma % 11;

            if (digito > 9) digito = 0;

            if (digito != int.Parse(cnpj.Substring(12, 1))) return false;

            //  5. CALCULA O 2º DÍGITO
            digito = 0;
            soma = 0;

            for (int i = 0; i < 13; i++)
            {
                if (i < 5)
                    soma += int.Parse(cnpj.Substring(i, 1)) * (6 - i);
                else
                    soma += int.Parse(cnpj.Substring(i, 1)) * (14 - i);
            }

            digito = 11 - soma % 11;

            if (digito > 9) digito = 0;

            if (digito != int.Parse(cnpj.Substring(13, 1))) return false;

            return true;
        }

        #endregion
    }
}