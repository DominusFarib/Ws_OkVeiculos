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

namespace webServiceCheckOk.Model.ProdutosModel
{
    public class LeilaoController
    {
        public LeilaoModel getLeilao(UsuarioModel usuario, Veiculo carro, bool isFeature = false)
        {
            // CODIGO CONSULTA: 6
            // FORNECEDORES: 1=CHECKAUTO;2=AUTORISCO;3=BOAVISTA;6=MOTORCHECK;7=ABSOLUTA;8=SEAPE;9=TDB
            LeilaoModel leilaoDados = new LeilaoModel();
            LeilaoModel logBuffer = new LeilaoModel();

            string logServer = string.Empty;
            string logLancamento = string.Empty;

            int codFornecedor = Verificacao.getFornecedorConsulta(6);

            try
            {
                logServer = usuario.Ip;
                logLancamento = DataBases.getLaunching();
                string subtransacao = string.Empty;
                subtransacao = "FT22";

                switch (codFornecedor)
                {
                    
                    case 9:
                    logServer += "|TDB_LEILAO";
                    FornTdb leilaoTDB = new FornTdb(carro,"1");
                    // INSERE LOG DE REQUISIÇÃO
                    DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);

                    leilaoDados = leilaoTDB.getLeilao();

                    logServer += "_" + leilaoDados.CodFornecedor;
                    logBuffer = leilaoDados;

                    break;
                    
                    case 3:
                    logServer += "|BOAVISTA_LEILAO";
                    FornBoaVista leilaoBV = new FornBoaVista(carro);
                    leilaoBV.codConsulta = "780";
                    // INSERE LOG DE REQUISIÇÃO
                    DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);

                    leilaoDados = leilaoBV.getLeilao();

                    logServer += "_" + leilaoDados.CodFornecedor;
                    logBuffer = leilaoDados;

                    break;
                /*
                case 4:
                    logServer += "|CREDIFY_GRAVAME";
                    FornCredify gravameCredify = new FornCredify(carro, usuario);
                    // INSERE LOG DE REQUISIÇÃO
                    DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);

                    binNacionalDados = gravameCredify.getGravame();

                    logServer += "_" + binNacionalDados.CodFornecedor;
                    logBuffer = binNacionalDados;
                    break;
                case 5:
                    logServer += "|TDI_GRAVAME";
                    FornTdi gravameTdi = new FornTdi(carro, usuario);
                    // INSERE LOG DE REQUISIÇÃO
                    DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);

                    binNacionalDados = gravameTdi.getGravame();

                    logServer += "_" + gravameTdi.idConsulta;
                    logBuffer = binNacionalDados;
                    break;
                */
                }

                var serializer = new XmlSerializer(typeof(LeilaoModel));

                using (StringWriter writer = new EncodingTextUTF8())
                {
                    serializer.Serialize(writer, logBuffer);
                }
                // INSERE LOG DE RESPOSTA
                DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), logBuffer.ToString());

                return leilaoDados;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}