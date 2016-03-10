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
    public class AgregadosController
    {
        public AgregadosModel getAgregados(UsuarioModel usuario, Veiculo carro, bool isFeature = false)
        {
            // CODIGO CONSULTA = 15
            // FORNECEDORES 1=WLLM;2=MOTORCHECK;3=INFOCAR;4=CREDIFY;5=SEAPE;
            AgregadosModel agregadosDados = new AgregadosModel();
            AgregadosModel logBuffer = new AgregadosModel();
            string logServer = string.Empty;
            string logLancamento = string.Empty;
            int codFornecedor = Verificacao.getFornecedorConsulta(15);

            try
            {
                logServer = usuario.Ip;
                logLancamento = DataBases.getLaunching();
                string subtransacao = string.Empty;
                subtransacao = isFeature ? "ST15" : "FT15";

                switch (codFornecedor)
                {
                    case 2:
                        logServer += "|MOTORCHECK_AGREGADOS";
                        FornMotorCheck respAgregados = new FornMotorCheck(carro);
                        // INSERE LOG DE REQUISIÇÃO
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);

                        agregadosDados = respAgregados.getAgregados();

                        logServer += "_" + agregadosDados.IdConsulta;
                        logBuffer = agregadosDados;

                        break;
                    default:
                        logServer += "|MOTORCHECK_AGREGADOS";
                        FornMotorCheck respDefault = new FornMotorCheck(carro);
                        // INSERE LOG DE REQUISIÇÃO
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);

                        agregadosDados = respDefault.getAgregados();

                        logServer += "_" + agregadosDados.IdConsulta;
                        logBuffer = agregadosDados;

                        break;
                }

                var serializer = new XmlSerializer(typeof(AgregadosModel));

                using (StringWriter writer = new EncodingTextUTF8())
                {
                    serializer.Serialize(writer, logBuffer);
                }

                // INSERE LOG DE RESPOSTA
                DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), logBuffer.ToString());

                return agregadosDados;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}