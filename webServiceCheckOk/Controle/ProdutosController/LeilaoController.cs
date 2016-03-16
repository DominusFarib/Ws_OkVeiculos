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
        public string logServer { get; set; }

        public LeilaoModel getLeilao(UsuarioModel usuario, Veiculo carro, bool isFeature = false, string logFeature = "")
        {
            // CODIGO CONSULTA: 6
            // FORNECEDORES: 1=CHECKAUTO;2=AUTORISCO;3=BOAVISTA;6=MOTORCHECK;7=ABSOLUTA;8=SEAPE;9=TDB
            LeilaoModel leilaoDados = new LeilaoModel();
            LeilaoModel logBuffer = new LeilaoModel();

            string logLancamento = string.Empty;
            string subtransacao = string.Empty;

            int codFornecedor = Verificacao.getFornecedorConsulta(6);

            try
            {
                logLancamento = String.IsNullOrEmpty(logFeature) ? DataBases.getLaunching() : logFeature;
                
                subtransacao = isFeature ? "FT22" : "PC22";

                switch (codFornecedor)
                {
                    case 9:
                        this.logServer += "|TDB_LEILAO";
                        FornTdb leilaoTDB = new FornTdb(carro,"1");

                        leilaoDados = leilaoTDB.getLeilao();

                        this.logServer += "_" + leilaoDados.CodFornecedor;
                        logBuffer = leilaoDados;
                    break;
                    
                    case 3:
                        this.logServer += "|BOAVISTA_LEILAO";
                        FornBoaVista leilaoBV = new FornBoaVista(carro);
                        leilaoBV.codConsulta = "780";

                        leilaoDados = leilaoBV.getLeilao();

                        this.logServer += "_" + leilaoDados.CodFornecedor;
                        logBuffer = leilaoDados;
                    break;
                /*
                case 4:
                    this.logServer += "|CREDIFY_GRAVAME";
                    FornCredify gravameCredify = new FornCredify(carro, usuario);

                    binNacionalDados = gravameCredify.getGravame();

                    this.logServer += "_" + binNacionalDados.CodFornecedor;
                    logBuffer = binNacionalDados;
                    break;
                case 5:
                    this.logServer += "|TDI_GRAVAME";
                    FornTdi gravameTdi = new FornTdi(carro, usuario);

                    binNacionalDados = gravameTdi.getGravame();

                    this.logServer += "_" + gravameTdi.idConsulta;
                    logBuffer = binNacionalDados;
                    break;
                */
                }

                var serializer = new XmlSerializer(typeof(LeilaoModel));

                using (StringWriter writer = new EncodingTextUTF8())
                {
                    serializer.Serialize(writer, logBuffer);
                }

                return leilaoDados;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}