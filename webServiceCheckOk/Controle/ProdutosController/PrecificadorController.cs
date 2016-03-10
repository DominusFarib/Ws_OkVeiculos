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
    public class PrecificadorController
    {
        public PrecificadorModel getPredificador(UsuarioModel usuario, Veiculo carro, bool isFeature = true)
        {
            // FEATURE 74
            // FORNECEDORES 3=TECNOBANK;

            PrecificadorModel precificadorDados = new PrecificadorModel();
            PrecificadorModel logBuffer = new PrecificadorModel();
            string logServer = string.Empty;
            string logLancamento = string.Empty;
            int codFornecedor = 3;

            try
            {
                logServer = usuario.Ip;
                logLancamento = DataBases.getLaunching();
                string subtransacao = string.Empty;
                subtransacao = "FT74";

                switch (codFornecedor)
                {
                    case 3:
                        logServer += "|TECNOBANK_PRECIFICADOR";
                        FornTecnoBank respPrecificador = new FornTecnoBank(carro);
                        // INSERE LOG DE REQUISIÇÃO
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);
                        precificadorDados = respPrecificador.getPrecificador();
                        logBuffer = precificadorDados;
                        break;
                    default:
                        logServer += "|TECNOBANK_PRECIFICADOR";
                        FornTecnoBank respPrecificadorDefault = new FornTecnoBank(carro);
                        // INSERE LOG DE REQUISIÇÃO
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);
                        precificadorDados = respPrecificadorDefault.getPrecificador();
                        logBuffer = precificadorDados;
                        break;
                }

                var serializer = new XmlSerializer(typeof(PrecificadorModel));

                using (StringWriter writer = new EncodingTextUTF8())
                {
                    serializer.Serialize(writer, logBuffer);
                }
                // INSERE LOG DE RESPOSTA
                DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), logBuffer.ToString());

                return precificadorDados;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}