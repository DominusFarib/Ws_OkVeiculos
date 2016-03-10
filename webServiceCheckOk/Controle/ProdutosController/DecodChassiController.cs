using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using webServiceCheckOk.BaseDados;
using webServiceCheckOk.Controle.Fornecedores;
using webServiceCheckOk.Controle.Inteligencia;
using webServiceCheckOk.Controle.Inteligencia.Utils;
using webServiceCheckOk.Model;
using webServiceCheckOk.Model.ProdutosModel;

namespace webServiceCheckOk.Controle.ProdutosController
{
    public class DecodChassiController
    {
        public DecodChassiModel getDecodChassi(UsuarioModel usuario, Veiculo carro, bool isFeature = false)
        {
            // CODIGO CONSULTA = 4
            // FORNECEDORES 1=CHECKAUTO;2=AUTORISCO;3=TECNOBANK;4=CREDIFY
            DecodChassiModel decodChassiDados = new DecodChassiModel();
            DecodChassiModel logBuffer = new DecodChassiModel();
            string logServer = string.Empty;
            string logLancamento = string.Empty;
            int codFornecedor = Verificacao.getFornecedorConsulta(4);

            try
            {
                logServer = usuario.Ip;
                logLancamento = DataBases.getLaunching();
                string subtransacao = string.Empty;
                subtransacao = isFeature ? "PC27" : "FT27";

                switch (codFornecedor)
                {
                    case 3:
                        logServer += "|TECNOBANK_DECODCHASSI";
                        FornTecnoBank respDecodChassi = new FornTecnoBank(carro);
                        // INSERE LOG DE REQUISIÇÃO
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);

                        decodChassiDados = respDecodChassi.getDecodChassi();

                        //logServer += "_" + decodChassiDados.IdConsulta;
                        logBuffer = decodChassiDados;

                        break;
                    default:
                        logServer += "|TECNOBANK_DECODCHASSI";
                        FornTecnoBank respDecodChassiDefault = new FornTecnoBank(carro);
                        // INSERE LOG DE REQUISIÇÃO
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);

                        decodChassiDados = respDecodChassiDefault.getDecodChassi();

                        //logServer += "_" + decodChassiDados.IdConsulta;
                        logBuffer = decodChassiDados;
                        break;
                }

                var serializer = new XmlSerializer(typeof(DecodChassiModel));

                using (StringWriter writer = new EncodingTextUTF8())
                {
                    serializer.Serialize(writer, logBuffer);
                }
                // INSERE LOG DE RESPOSTA
                DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), logBuffer.ToString());

                return decodChassiDados;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}