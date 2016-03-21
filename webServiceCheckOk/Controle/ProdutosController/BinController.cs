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
        public string requisicaoFornecedor { get; set; }
        // BASE ESTADUAL
        public BinEstadualModel getBinEstadual(UsuarioModel usuario, Veiculo carro, string logID = "", bool isFeature = false)
        {
            // CODIGO CONSULTA: 2
            // FORNECEDORES:    '1=CHECKAUTO;2=MOTORCHECK;3=CONSULTAUTO'
            BinEstadualModel binEstadualDados = new BinEstadualModel();
            // PEGA O CODIGO DO FORNECEDOR DA CONSULTA
            int codFornecedor = Verificacao.getFornecedorConsulta(2);

            try
            {
                switch (codFornecedor)
                {
                    case 3:
                        this.logServer += "|CONSULTAUTO_BINESTADUAL";
                        FornConsultAuto binConsultAuto = new FornConsultAuto(carro);

                        binConsultAuto.dadosUsuario = usuario;
                        binConsultAuto.logLancamento = logID;
                        // PEGA RESPOSTA NO FORNECEDOR
                        binEstadualDados = binConsultAuto.getBinEstadual();
                        this.requisicaoFornecedor = binConsultAuto.urlRequisicao;
                    break;
                }

                return binEstadualDados;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    
        // BASE NACIONAL
        public BinModel getBinNacional(UsuarioModel usuario, Veiculo carro, string logID = "", bool isFeature = false, bool flagDocProprietario = false)
        {
            // CODIGO CONSULTA: 1
            // FORNECEDORES:    '1=>CHECKAUTO;2=>AUTORISCO/MOTORCHECK;6=>CONSULTAUTO;7=>CHECKPRO'
            BinModel binNacionalDados = new BinModel();
            // PEGA O CODIGO DO FORNECEDOR DA CONSULTA
            int codFornecedor = Verificacao.getFornecedorConsulta(1);

            try
            {
                // APENAS ALGUNS FORNECEDORES TRAZEM ESSA INFORMAÇÃO
                if (flagDocProprietario && codFornecedor != 6) 
                {
                    codFornecedor = 6;
                }

                switch (codFornecedor)
                {
                    case 6:
                        this.logServer += "|CONSULTAUTO_BINNACIONAL";
                        FornConsultAuto binConsultAuto = new FornConsultAuto(carro);

                        binConsultAuto.dadosUsuario = usuario;
                        binConsultAuto.logLancamento = logID;
                        // PEGA RESPOSTA NO FORNECEDOR
                        binNacionalDados = binConsultAuto.getBinNacional();
                        this.requisicaoFornecedor = binConsultAuto.urlRequisicao;
                    break;
                }

                return binNacionalDados;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // BASE NACIONAL ROUBO E FURTO
        public BinRouboFurtoModel getBinRouboFurto(UsuarioModel usuario, Veiculo carro, string logID = "", bool isFeature = false, bool flagDocProprietario = false)
        {
            // CODIGO CONSULTA: 1
            // FORNECEDORES:    '1=>CHECKAUTO 2=>AUTORISCO/MOTORCHECK 3=>AUTORISCO/GRUPOTDI 6=>CONSULTAAUTO'
            BinRouboFurtoModel binRouboFurtoDados = new BinRouboFurtoModel();
            // PEGA O CODIGO DO FORNECEDOR DA CONSULTA
            int codFornecedor = Verificacao.getFornecedorConsulta(5);

            try
            {
                switch (codFornecedor)
                {
                    case 2:
                        this.logServer += "|CONSULTAUTO_BINROUBOFURTO";
                        FornConsultAuto binConsultAuto = new FornConsultAuto(carro);
                        
                        binConsultAuto.dadosUsuario = usuario;
                        binConsultAuto.logLancamento = logID;
                        // PEGA RESPOSTA NO FORNECEDOR
                        binRouboFurtoDados = binConsultAuto.getBinRouboFurto();
                        this.requisicaoFornecedor = binConsultAuto.urlRequisicao;
                    break;
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