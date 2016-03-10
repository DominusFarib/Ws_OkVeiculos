using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace webServiceCheckOk.Model
{
    [Serializable]
    public class Veiculo
    {
        #region ATRIBUTOS

        [XmlElement("MODELO")]
        public string Modelo { get; set; }

        [XmlElement("MARCA")]
        public string Marca { get; set; }

        [XmlElement("ANO_MODELO")]
        public string AnoModelo { get; set; }

        [XmlElement("ANO_FABRICACAO")]
        public string AnoFabrica { get; set; }

        [XmlElement("PLACA")]
        public string Placa { get; set; }

        [XmlElement("RENAVAM")]
        public string Renavam { get; set; }

        [XmlElement("CATEGORIA")]
        public string Categoria { get; set; }

        [XmlElement("CHASSI")]
        public string Chassi { get; set; }
        
        [XmlElement("CODIGO_FORNECEDOR")]
        public string Codigo { get; set; }
        
        [XmlElement("COMBUSTIVEL")]
        public string Combustivel { get; set; }

        [XmlElement("COMPRIMENTO")]
        public string Comprimento { get; set; }

        [XmlElement("DISTANCIA_EIXOS")]
        public string DistEixos { get; set; }

        [XmlElement("COR")]
        public string Cor { get; set; }
        
        [XmlElement("NR_CAMBIO")]
        public string NrCambio { get; set; }
        
        [XmlElement("NR_CARROCERIA")]
        public string NrCarroceria { get; set; }
        
        [XmlElement("NR_MOTOR")]
        public string NrMotor { get; set; }
        
        [XmlElement("NR_EIXO_TRASEIRO")]
        public string NrEixoTraseiro { get; set; }

        [XmlElement("NR_TERCEIRO_EIXO")]
        public string NrTerceiroEixo { get; set; }

        [XmlElement("UF")]
        public string Uf { get; set; }
        
        [XmlElement("CRLV")]
        public string Crlv { get; set; }
        
        [XmlElement("UF_CRLV")]
        public string UfCrlv { get; set; }
        
        [XmlElement("TIPO")]
        public string Tipo { get; set; }
        
        [XmlElement("POTENCIA")]
        public string Potencia { get; set; }
        
        [XmlElement("PESO_BRUTO")]
        public string PesoBruto { get; set; }
        
        [XmlElement("CILINDRADAS")]
        public string Cilindradas { get; set; }
        
        [XmlElement("MUN_EMPLACAMENTO")]
        public string MunicipioEmplacamento { get; set; }
        
        [XmlElement("CODIGO_MUNICIPIO")]
        public string CodMunicipio { get; set; }
        
        [XmlElement("CAPACIDADE_CARGA")]
        public string CapacidadeCarga { get; set; }
        
        [XmlElement("CAPACIDADE_PASSAGEIROS")]
        public string CapacidadePassageiros { get; set; }
        
        [XmlElement("CAPACIDADE_MAXTRACAO")]
        public string CapacidadeMaximaTracao { get; set; }
        
        [XmlElement("NACIONALIDADE")]
        public string Nacionalidade { get; set; }
        
        [XmlElement("TIPO_MONTAGEM")]
        public string TipoMontagem { get; set; }

        [XmlElement("TIPO_FREIO")]
        public string TipoFreio { get; set; }

        [XmlElement("TIPO_MOTOR")]
        public string TipoMotor { get; set; }

        [XmlElement("TIPO_CARROCERIA")]
        public string TipoCarroceria { get; set; }
        
        [XmlElement("TIPO_CHASSI")]
        public string TipoChassi { get; set; }

        [XmlElement("TRANSMISSAO")]
        public string Transmissao { get; set; }

        [XmlElement("TRACAO")]
        public string Tracao { get; set; }

        [XmlElement("ESPECIE")]
        public string Especie { get; set; }

        [XmlElement("DIR_HIDRAULICA")]
        public string FlagDirHidraulica { get; set; }
        
        [XmlElement("IMPORTADO")]
        public string FlagImportado { get; set; }
        
        [XmlElement("CONSTA_LEILAO")]
        public string FlagLeilao { get; set; }
        
        [XmlElement("QTD_FPASSAGEIROS")]
        public string QtdPassageiros { get; set; }
        
        [XmlElement("QTD_EIXOS")]
        public string QtdEixos { get; set; }

        [XmlElement("PROPRIETARIO")]
        public Pessoa Proprietario { get; set; }

        [XmlElement("QTD_PORTAS")]
        public string QtdPortas { get; set; }

        [XmlElement("VERSAO")]
        public string Versao { get; set; }
        
        #endregion

        // CONSTRUTOR
        public Veiculo() { }

        #region VALIDACAO
        // CHASSI
        public static bool validaChassi(string chassi)
        {
            Regex rgx = new Regex("[IOQ]");

            try
            {
                if (string.IsNullOrEmpty(chassi.Trim())) return false;

                // 1. TAMANHO do CHASSI
                if ((chassi.Length != 17) || (rgx.IsMatch(chassi))) return false;

                chassi = chassi.ToUpper();
                // 2. REGIÃO GEOGRÁFICA
                if (chassi.Substring(0, 1) == "0") return false;

                // 3. ANO MODELO
                if ((chassi.Substring(9, 1) == "0") || (chassi.Substring(9, 1) == "Z")) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        // PLACA
        public static bool validaPlaca(string placa)
        {
            Regex regex = new Regex(@"^[a-zA-Z]{3}\d{4}$");

            // 1. VERIFICA SE STRING VAZIA
            if (string.IsNullOrEmpty(placa.Trim())) return false;

            // 2. LIMPA FORMATAÇÃO
            placa = Regex.Replace(placa, "[^0-9a-zA-Z]+", "");

            return regex.IsMatch(placa);
        }

        // RENAVAM
        public static bool validaRenavam(string renavam)
        {
            // 1. VERIFICA SE STRING VAZIA
            if (string.IsNullOrEmpty(renavam.Trim())) return false;

            int[] digitos = new int[11];
            int verificador = 0;
            string sequencia = "3298765432";

            // 2. VERIFICA SE HÁ LETRAS
            string SoNumeros = Regex.Replace(renavam, "[^0-9]", string.Empty);

            if (string.IsNullOrEmpty(SoNumeros)) return false;

            // 3. VERIFICA O TAMANHO DO RENAVAM
            if ((SoNumeros.Length != 9) && (SoNumeros.Length != 11)) return false;

            // 4. VERIFICA REPETIÇÃO DE NUMEROS
            if (new string(SoNumeros[0], SoNumeros.Length) == SoNumeros) return false;

            // 5. FORMATA PARA 11 DIGITOS
            SoNumeros = Convert.ToInt64(SoNumeros).ToString("00000000000");

            for (int i = 0; i < 11; i++)
                digitos[i] = Convert.ToInt32(SoNumeros.Substring(i, 1));

            // 6. CALCULA O DIGITO VERIFICADOR
            for (int i = 0; i < 10; i++)
                verificador += digitos[i] * Convert.ToInt32(sequencia.Substring(i, 1));

            verificador = (verificador * 10) % 11;
            verificador = (verificador != 10) ? verificador : 0;

            return (verificador == digitos[10]);
        }

        // MOTOR
        public static bool validaMotor(string motor)
        {
            return motor.Length > 21 ? false : true;
        }

        // CRLV
        public static bool validaCrlv(string crlv)
        {
            crlv = Regex.Replace(crlv, "[^0-9]", string.Empty);

            if (!crlv.Length.Equals(10) || string.IsNullOrEmpty(crlv))
                return false;

            return true;
        }

        // UF CRLV
        public static bool validaUfCrlv(string ufCrlv)
        {
            ufCrlv = Regex.Replace(ufCrlv, "[^A-Z]", string.Empty);

            if (!ufCrlv.Length.Equals(2) || string.IsNullOrEmpty(ufCrlv))
                return false;

            return true;
        }

        #endregion
    }
}