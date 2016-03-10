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
    public class GravameController
    {
        public GravameModel getGravame(UsuarioModel usuario, Veiculo carro, bool flagCompleto = false)
        {
            // CODIGO CONSULTA = 8
            // FORNECEDORES 2=>SEAP 3=>BOA VISTA 4=>CREDIFY
            GravameModel gravameDados = new GravameModel();
            GravameModel logBuffer = new GravameModel();
            string logServer = string.Empty;
            string logLancamento = string.Empty;
            int codFornecedor = Verificacao.getFornecedorConsulta(8);
            
            try
            {
                logServer = usuario.Ip;
                logLancamento = DataBases.getLaunching();
                string subtransacao = string.Empty;
                subtransacao = flagCompleto ? "PC19" : "PC20";

                switch (codFornecedor)
                {
                    case 2:
                        logServer += "|SEAPE_GRAVAME";
                        FornSeape gravameSeape = new FornSeape(carro);
                        // INSERE LOG DE REQUISIÇÃO
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);
                        
                        if (flagCompleto)
                            gravameDados = gravameSeape.getGravameCompleto();
                        else
                            gravameDados = gravameSeape.getGravameSimples();

                        logServer += "_" + gravameDados.IdConsulta;
                        logBuffer = gravameDados;
                       
                        break;
                    case 3:
                        logServer += "|BOAVISTA_GRAVAME";
                        FornBoaVista gravameBV = new FornBoaVista(carro);
                        gravameBV.codConsulta = "905";
                        // INSERE LOG DE REQUISIÇÃO
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);

                        gravameDados = gravameBV.getGravame();

                        logServer += "_" + gravameDados.CodFornecedor;
                        logBuffer = gravameDados;

                        break;
                    case 4:
                        logServer += "|CREDIFY_GRAVAME";
                        FornCredify gravameCredify = new FornCredify(carro, usuario);
                        // INSERE LOG DE REQUISIÇÃO
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);

                        gravameDados = gravameCredify.getGravame();

                        logServer += "_" + gravameDados.CodFornecedor;
                        logBuffer = gravameDados;
                        break;
                    case 5:
                        logServer += "|TDI_GRAVAME";
                        FornTdi gravameTdi = new FornTdi(carro, usuario);
                        // INSERE LOG DE REQUISIÇÃO
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);

                        gravameDados = gravameTdi.getGravame();

                        logServer += "_" + gravameTdi.idConsulta;
                        logBuffer = gravameDados;
                        break;
                    case 6:
                        logServer += "|SINALIZA_GRAVAME";
                        FornSinaliza gravameSinaliza = new FornSinaliza(carro, usuario, "62");
                        // INSERE LOG DE REQUISIÇÃO
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);

                        gravameDados = gravameSinaliza.getGravame();

                        logServer += "_" + gravameDados.IdConsulta;
                        logBuffer = gravameDados;
                        break;
                    default:
                        logServer += "|SINALIZA_GRAVAME";
                        FornSinaliza gravameDefault = new FornSinaliza(carro, usuario, "62");
                        // INSERE LOG DE REQUISIÇÃO
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);

                        gravameDados = gravameDefault.getGravame();

                        logServer += "_62";
                        logBuffer = gravameDados;
                        break;
                }

                var serializer = new XmlSerializer(typeof(GravameModel));

                using (StringWriter writer = new EncodingTextUTF8())
                {
                    serializer.Serialize(writer, logBuffer);
                }
                // INSERE LOG DE RESPOSTA
                DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), logBuffer.ToString());

                return gravameDados;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}