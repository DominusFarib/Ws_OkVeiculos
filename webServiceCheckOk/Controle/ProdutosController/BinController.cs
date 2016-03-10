using System;
using System.IO;
using System.Xml.Serialization;
using webServiceCheckOk.BaseDados;
using webServiceCheckOk.Controle.Fornecedores;
using webServiceCheckOk.Controle.Inteligencia;
using webServiceCheckOk.Controle.Inteligencia.Utils;
using webServiceCheckOk.Model;
using webServiceCheckOk.Model.ProdutosModel;

namespace webServiceCheckOk.Controle.ProdutosController
{
    public class BinController
    {
        // BASE ESTADUAL
        public BinEstadualModel getBinEstadual(UsuarioModel usuario, Veiculo carro, bool isFeature = false)
        {
            // CODIGO CONSULTA: 2
            // FORNECEDORES:    '1=CHECKAUTO;2=MOTORCHECK;3=CONSULTAUTO'
            BinEstadualModel binEstadualDados = new BinEstadualModel();
            BinEstadualModel logBuffer = new BinEstadualModel();
            string logServer = string.Empty;
            string logLancamento = string.Empty;
            // PEGA O CODIGO DO FORNECEDOR DA CONSULTA
            int codFornecedor = Verificacao.getFornecedorConsulta(2);

            try
            {
                logServer = usuario.Ip;
                logLancamento = DataBases.getLaunching();
                string subtransacao = string.Empty;
                subtransacao = isFeature ? "FT16" : "PC16";

                switch (codFornecedor)
                {
                    case 3:
                        logServer += "|CONSULTAUTO_BINESTADUAL";
                        FornConsultAuto binConsultAuto = new FornConsultAuto(carro);
                        // INSERE LOG DE REQUISIÇÃO
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);
                        binConsultAuto.dadosUsuario = usuario;
                        binConsultAuto.logLancamento = logLancamento;
                        // PEGA RESPOSTA NO FORNECEDOR
                        binEstadualDados = binConsultAuto.getBinEstadual();
                        logBuffer = binEstadualDados;
                    break;
                }

                var serializer = new XmlSerializer(typeof(BinEstadualModel));

                using (StringWriter writer = new EncodingTextUTF8())
                {
                    serializer.Serialize(writer, logBuffer);
                }
                // INSERE LOG DE RESPOSTA
                DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), logBuffer.ToString());

                return binEstadualDados;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    
        // BASE NACIONAL
        public BinModel getBinNacional(UsuarioModel usuario, Veiculo carro, bool isFeature = false, bool flagDocProprietario = false)
        {
            // CODIGO CONSULTA: 1
            // FORNECEDORES:    '1=>CHECKAUTO;2=>AUTORISCO/MOTORCHECK;6=>CONSULTAUTO;7=>CHECKPRO'
            BinModel binNacionalDados = new BinModel();
            BinModel logBuffer = new BinModel();
            string logServer = string.Empty;
            string logLancamento = string.Empty;
            // PEGA O CODIGO DO FORNECEDOR DA CONSULTA
            int codFornecedor = Verificacao.getFornecedorConsulta(1);

            try
            {
                logServer = usuario.Ip;
                logLancamento = DataBases.getLaunching();
                string subtransacao = string.Empty;
                subtransacao = isFeature ? "FT14" : "PC14";

                switch (codFornecedor)
                {
                    case 6:
                        logServer += "|CONSULTAUTO_BINNACIONAL";
                        FornConsultAuto binConsultAuto = new FornConsultAuto(carro);
                        // INSERE LOG DE REQUISIÇÃO
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);
                        binConsultAuto.flagDocProprietario = flagDocProprietario;
                        binConsultAuto.dadosUsuario = usuario;
                        binConsultAuto.logLancamento = logLancamento;
                        // PEGA RESPOSTA NO FORNECEDOR
                        binNacionalDados = binConsultAuto.getBinNacional();
                        logBuffer = binNacionalDados;
                    break;
                }

                var serializer = new XmlSerializer(typeof(BinModel));

                using (StringWriter writer = new EncodingTextUTF8())
                {
                    serializer.Serialize(writer, logBuffer);
                }
                // INSERE LOG DE RESPOSTA
                DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), logBuffer.ToString());

                return binNacionalDados;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // BASE NACIONAL ROUBO E FURTO
        public BinRouboFurtoModel getBinRouboFurto(UsuarioModel usuario, Veiculo carro, bool isFeature = false, bool flagDocProprietario = false)
        {
            // CODIGO CONSULTA: 1
            // FORNECEDORES:    '1=>CHECKAUTO 2=>AUTORISCO/MOTORCHECK 3=>AUTORISCO/GRUPOTDI 6=>CONSULTAAUTO'
            BinRouboFurtoModel binRouboFurtoDados = new BinRouboFurtoModel();
            BinRouboFurtoModel logBuffer = new BinRouboFurtoModel();
            string logServer = string.Empty;
            string logLancamento = string.Empty;
            // PEGA O CODIGO DO FORNECEDOR DA CONSULTA
            int codFornecedor = Verificacao.getFornecedorConsulta(5);

            try
            {
                logServer = usuario.Ip;
                logLancamento = DataBases.getLaunching();
                string subtransacao = string.Empty;
                subtransacao = isFeature ? "FT17" : "PC17";

                switch (codFornecedor)
                {
                    case 2:
                        logServer += "|CONSULTAUTO_BINROUBOFURTO";
                        FornConsultAuto binConsultAuto = new FornConsultAuto(carro);
                        // INSERE LOG DE REQUISIÇÃO
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);
                        
                        binConsultAuto.flagDocProprietario = flagDocProprietario;
                        binConsultAuto.dadosUsuario = usuario;
                        binConsultAuto.logLancamento = logLancamento;
                        // PEGA RESPOSTA NO FORNECEDOR
                        binRouboFurtoDados = binConsultAuto.getBinRouboFurto();
                        logBuffer = binRouboFurtoDados;
                        break;
                }

                var serializer = new XmlSerializer(typeof(BinRouboFurtoModel));

                using (StringWriter writer = new EncodingTextUTF8())
                {
                    serializer.Serialize(writer, logBuffer);
                }
                // INSERE LOG DE RESPOSTA
                DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), logBuffer.ToString());

                return binRouboFurtoDados;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    

    }
}