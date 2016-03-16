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
        public string logServer { get; set; }
        // BASE ESTADUAL
        public BinEstadualModel getBinEstadual(UsuarioModel usuario, Veiculo carro, bool isFeature = false)
        {
            // CODIGO CONSULTA: 2
            // FORNECEDORES:    '1=CHECKAUTO;2=MOTORCHECK;3=CONSULTAUTO'
            BinEstadualModel binEstadualDados = new BinEstadualModel();
            BinEstadualModel logBuffer = new BinEstadualModel();
            string logLancamento = string.Empty;
            // PEGA O CODIGO DO FORNECEDOR DA CONSULTA
            int codFornecedor = Verificacao.getFornecedorConsulta(2);

            try
            {
                logLancamento = DataBases.getLaunching();
                string subtransacao = string.Empty;
                subtransacao = isFeature ? "FT16" : "PC16";

                switch (codFornecedor)
                {
                    case 3:
                        this.logServer += "|CONSULTAUTO_BINESTADUAL";
                        FornConsultAuto binConsultAuto = new FornConsultAuto(carro);

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

                return binEstadualDados;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    
        // BASE NACIONAL
        public BinModel getBinNacional(UsuarioModel usuario, Veiculo carro, bool isFeature = false)
        {
            // CODIGO CONSULTA: 1
            // FORNECEDORES:    '1=>CHECKAUTO;2=>AUTORISCO/MOTORCHECK;6=>CONSULTAUTO;7=>CHECKPRO'
            BinModel binNacionalDados = new BinModel();
            BinModel logBuffer = new BinModel();
            string logLancamento = string.Empty;
            // PEGA O CODIGO DO FORNECEDOR DA CONSULTA
            int codFornecedor = Verificacao.getFornecedorConsulta(1);

            try
            {
                logLancamento = DataBases.getLaunching();
                string subtransacao = string.Empty;
                subtransacao = isFeature ? "FT14" : "PC14";

                switch (codFornecedor)
                {
                    case 6:
                        this.logServer += "|CONSULTAUTO_BINNACIONAL";
                        FornConsultAuto binConsultAuto = new FornConsultAuto(carro);

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
            string logLancamento = string.Empty;
            // PEGA O CODIGO DO FORNECEDOR DA CONSULTA
            int codFornecedor = Verificacao.getFornecedorConsulta(5);

            try
            {
                logLancamento = DataBases.getLaunching();
                string subtransacao = string.Empty;
                subtransacao = isFeature ? "FT17" : "PC17";

                switch (codFornecedor)
                {
                    case 2:
                        this.logServer += "|CONSULTAUTO_BINROUBOFURTO";
                        FornConsultAuto binConsultAuto = new FornConsultAuto(carro);
                        
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

                return binRouboFurtoDados;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    
    }
}