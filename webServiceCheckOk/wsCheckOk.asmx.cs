using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;
using webServiceCheckOk.BaseDados;
using webServiceCheckOk.Controle.Inteligencia;
using webServiceCheckOk.Controle.Inteligencia.Utils;
using webServiceCheckOk.Controle.ProdutosController;
using webServiceCheckOk.Model;
using webServiceCheckOk.Model.ProdutosModel;

namespace webServiceCheckOk
{
    /// <summary>
    /// Branch Local
    /// AUTHOR: Domingos Ribeiro <dofari.dfr@gmail.com>
    /// DOFARI: Novo teste 11.03.2016
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // EN: To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // PT: Para permitir que esse Web Service a ser chamado a partir do script , utilizando ASP.NET AJAX, remova o comentário da seguinte linha.
    // [System.Web.Script.Services.ScriptService]
    public class wsCheckOk : System.Web.Services.WebService
    {
        [WebMethod]
        public OKVeiculos OkVeiculos(string codProduto, string logon, string senha, string chassi, string uf, string placa, string renavam, string nrMotor, string nrCarroceria, string nrEixoTras, string nrTercEixo, string nrCxCambio, string crlv, string ufCrlv, string cpfCnpj, string tipoPessoa, string ddd1, string telefone1, string ddd2, string telefone2, bool featRouboFurto = false, bool featSTF = false, bool featSTJ = false, bool featTST = false, bool featGravame = false, bool featPrecificador = false, bool featLeilao = false, bool featPerdaTotal = false, bool featBaseNacional = false, bool featDocProprietarios = false)
        {
            string ip = Context.Request.ServerVariables["REMOTE_ADDR"];
            string logServer = ip;
            string logLancamento = string.Empty;
            string subTrans = string.Empty;
            string tmpReqFornecedor = string.Empty;
            string tmpBuffer = string.Empty;
            decimal tmpLog = 0;
            decimal tmpLog1 = 1;

            System.Text.RegularExpressions.Regex regLetras = new System.Text.RegularExpressions.Regex("[^A-Z]");
            System.Text.RegularExpressions.Regex regNumeros = new System.Text.RegularExpressions.Regex("[^0-9]");

            XmlSerializer serializer;
            OKVeiculos retorno = new OKVeiculos();
            UsuarioModel dadosUsuario = new UsuarioModel();
            AuxParametros parametros = new AuxParametros();
            PrecificadorController dadosPrecificador;
            AgregadosController dadosAgregados;
            LeilaoController dadosLeilao;
            GravameController dadosGravame;
            BinController dadosBaseNacional;
            BinController dadosBaseEstadual;
            SinistroController dadosPerdaTotal;
            DecodChassiController dadosDecodChassi;

            try
            {
                retorno.veiculo = null;
                #region PARAMETROS

                dadosUsuario.Logon = logon != String.Empty ? logon : null;
                dadosUsuario.Senha = senha != String.Empty ? senha : null;

                parametros.dadosPessoa = new Pessoa();
                parametros.dadosVeiculo = new Veiculo();
                parametros.features = new AuxParametrosFeatures(featRouboFurto, featSTF, featSTJ, featTST, featGravame, featPrecificador, featLeilao, featPerdaTotal, featBaseNacional, featDocProprietarios);

                parametros.dadosVeiculo.Chassi = chassi != String.Empty ? chassi : null;
                parametros.dadosVeiculo.Uf = uf != String.Empty ? uf : null;
                parametros.dadosVeiculo.Placa = placa != String.Empty ? placa : null;
                parametros.dadosVeiculo.Renavam = renavam != String.Empty ? renavam : null;
                parametros.dadosVeiculo.NrMotor = nrMotor != String.Empty ? nrMotor : null;
                parametros.dadosVeiculo.Crlv = crlv != String.Empty ? crlv : null;
                parametros.dadosVeiculo.UfCrlv = ufCrlv != String.Empty ? ufCrlv : null;

                parametros.dadosPessoa.Documento = cpfCnpj;
                parametros.dadosPessoa.Tipo = tipoPessoa;

                if ((!String.IsNullOrEmpty(telefone1)) && (!String.IsNullOrEmpty(telefone2)))
                {
                    parametros.dadosPessoa.Telefones = new List<Telefone>();
                    parametros.dadosPessoa.Telefones.Add(new Telefone(ddd1, telefone1));
                    parametros.dadosPessoa.Telefones.Add(new Telefone(ddd2, telefone2));
                    parametros.dadosPessoa.Telefones = Util.unSetListaVazia<Telefone>(parametros.dadosPessoa.Telefones);
                }
                else if ((!String.IsNullOrEmpty(telefone1)) || (!String.IsNullOrEmpty(telefone2)))
                {
                    if (!String.IsNullOrEmpty(telefone1))
                        parametros.dadosPessoa.Telefone = new Telefone(ddd1, telefone1);
                    else
                        parametros.dadosPessoa.Telefone = new Telefone(ddd2, telefone2);
                }

                parametros.dadosVeiculo = Util.unSetDadosVazios<Veiculo>(parametros.dadosVeiculo);
                parametros.dadosPessoa = Util.unSetDadosVazios<Pessoa>(parametros.dadosPessoa);

                if ((String.IsNullOrEmpty(telefone1)) && (String.IsNullOrEmpty(telefone2)) && String.IsNullOrEmpty(cpfCnpj) && String.IsNullOrEmpty(tipoPessoa))
                {
                    parametros.dadosPessoa = null;
                }

                parametros.features = Util.unSetDadosVazios<AuxParametrosFeatures>(parametros.features);
                
                #endregion

                #region VALIDACAO

                // AUTORIZACAO
                Autorizacao autorizando = new Autorizacao(logon, senha, codProduto.Length == 2 ? "PC" + codProduto : codProduto);

                autorizando.verificaAutorizacao();
                if (autorizando.erro == null)
                    autorizando.verificaAgenda();
                if (autorizando.erro == null)
                    autorizando.verificaTravaFinanceira();

                if (autorizando.erro != null)
                {
                    retorno.Mensagem = autorizando.erro.Codigo.ToString() + ": " + autorizando.erro.Descricao.ToString();
                    return retorno;
                }
                // IP INTERNO X INTEGRADOR
                if (ConfigurationManager.AppSettings["HABILITA_IP_INTERNO"] == "S")
                {
                    if(!Verificacao.verificaIntegrador(logon))
                    {
                        retorno.Mensagem = "Integrador não cadastrado";
                        return retorno;
                    }
                }
                // VERIFICA SE É RECEITA CHECKOK
                if (featDocProprietarios)
                {
                    Verificacao checagem = new Verificacao();
                    string receita = checagem.getReceitaLogon(logon);

                    if (!receita.Equals("56"))  featDocProprietarios = false;
                }
                // CHASSI
                if (chassi.Trim().Length > 0)
                {
                    chassi = chassi.ToUpper().Trim();

                    if (!Veiculo.validaChassi(chassi))
                    {
                        retorno.Mensagem = "Chassi inválido";
                        return retorno;
                    }
                }
                // UF
                if (uf.Trim().Length > 0)
                {
                    uf = uf.ToUpper().Trim();

                    if (regLetras.IsMatch(uf) || !uf.Length.Equals(2))
                    {
                        retorno.Mensagem = "UF inválida";
                    }
                }
                // PLACA
                if (placa.Trim().Length > 0)
                {
                    placa = placa.ToUpper().Trim();

                    if (!Veiculo.validaPlaca(placa))
                    {
                        retorno.Mensagem = "Placa do veículo inválida";
                        return retorno;
                    }
                }
                // RENAVAM
                if (renavam.Trim().Length > 0)
                {
                    renavam = renavam.ToUpper().Trim();

                    if (!Veiculo.validaRenavam(renavam))
                    {
                        retorno.Mensagem = "Renavam inválidoa";
                        return retorno;
                    }
                }
                // MOTOR
                if (nrMotor.Trim().Length > 0)
                {
                    nrMotor = nrMotor.ToUpper().Trim();

                    if (!Veiculo.validaMotor(nrMotor))
                    {
                        retorno.Mensagem = "Motor inválido";
                        return retorno;
                    }
                }
                // CRLV
                if (crlv.Trim().Length > 0)
                {
                    crlv = crlv.ToUpper().Trim();

                    if (!Veiculo.validaCrlv(crlv))
                    {
                        retorno.Mensagem = "CRLV inválido";
                        return retorno;
                    }
                }
                // UF CRLV
                if (ufCrlv.Trim().Length > 0)
                {
                    ufCrlv = ufCrlv.ToUpper().Trim();

                    if (!Veiculo.validaUfCrlv(ufCrlv))
                    {
                        retorno.Mensagem = "UF CRLV inválida";
                        return retorno;
                    }
                }
                // CPF/CNPJ
                if (cpfCnpj.Trim().Length > 0)
                {
                    cpfCnpj = cpfCnpj.ToUpper().Trim();

                    if (!cpfCnpj.Length.Equals(11) && !cpfCnpj.Length.Equals(14) && regNumeros.IsMatch(cpfCnpj))
                    {
                        retorno.Mensagem = "CPF/CNPJ inválido";
                        return retorno;
                    }
                    
                    if(cpfCnpj.Length.Equals(11))
                    {
                        if(!PessoaFisica.validaCPF(cpfCnpj))
                        {
                            retorno.Mensagem = "CPF/CNPJ inválido";
                            return retorno;
                        }
                    }else if(cpfCnpj.Length.Equals(14))
                    {
                        if(!PessoaJuridica.validaCNPJ(cpfCnpj))
                        {
                            retorno.Mensagem = "CPF/CNPJ inválido";
                            return retorno;
                        }
                    }
                }
                // DDD1
                if (ddd1.Trim().Length > 0)
                {
                    ddd1 = ddd1.ToUpper().Trim();

                    if (!ddd1.Length.Equals(2) || regNumeros.IsMatch(ddd1))
                    {
                        retorno.Mensagem = "DDD1 inválido";
                    }
                }
                // TELEFONE1
                if (telefone1.Trim().Length > 0)
                {
                    telefone1 = telefone1.ToUpper().Trim();

                    if (telefone1.Length < 8 || regNumeros.IsMatch(telefone1))
                    {
                        retorno.Mensagem = "Telefone 1 inválido";
                    }
                }
                // DDD2
                if (ddd2.Trim().Length > 0)
                {
                    ddd2 = ddd1.ToUpper().Trim();

                    if (!ddd2.Length.Equals(2) || regNumeros.IsMatch(ddd2))
                    {
                        retorno.Mensagem = "DDD2 inválido";
                    }
                }
                // TELEFONE2
                if (telefone2.Trim().Length > 0)
                {
                    telefone2 = telefone2.ToUpper().Trim();

                    if (telefone2.Length < 8 || regNumeros.IsMatch(telefone2))
                    {
                        retorno.Mensagem = "Telefone2 inválido";
                    }
                }
                // TIPO PESSOA
                if (tipoPessoa.Trim().Length > 0)
                {
                    tipoPessoa = tipoPessoa.ToUpper().Trim();

                    if (!tipoPessoa.Length.Equals(1) || regLetras.IsMatch(tipoPessoa))
                    {
                        retorno.Mensagem = "Tipo Pessoa inválido";
                    }
                }

                #region VALIDA O PRODUTO/CONSULTA SOLICITADO

                switch (codProduto)
                {
                    // LEILAO SINTETICO
                    case "03":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Leilao Sintetico é necessário informar uma das chaves de consulta: Placa, Chassi!";
                            return retorno;
                        }
                        break;
                    // CONSULTA NACIONAL
                    case "14":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0) && renavam.Trim().Length.Equals(0) && nrMotor.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Nacional é necessário informar uma das chaves de consulta: Placa, Chassi, Renavam, Motor!";
                            return retorno;
                        }
                        break;
                    // NACIONAL - GRAVAME
                    case "15":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0) && renavam.Trim().Length.Equals(0) && nrMotor.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Nacional - Gravame é necessário informar uma das chaves de consulta: Placa, Chassi, Renavam, Motor!";
                            return retorno;
                        }
                        break;
                    // ESTADUAL
                    case "16":
                        if ((placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0)) || uf.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Estadual é necessário informar uma das chaves de consulta: Placa, Chassi, UF";
                            return retorno;
                        }
                        break;
                    // BIN NACIONAL ROUBO FURTO
                    case "17":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Roubo Furto é necessário informar uma das chaves de consulta: Placa, Chassi!";
                            return retorno;
                        }
                        break;
                    // AGREGADOS
                    case "18":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0) && renavam.Trim().Length.Equals(0) && nrMotor.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Agregados é necessário informar uma das chaves de consulta: Placa, Chassi, Renavam ou Motor!";
                            return retorno;
                        }
                        break;
                    // GRAVAME COMPLETO
                    case "19":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Gravame Completo é necessário informar uma das chaves de consulta: Placa, Chassi!";
                            return retorno;
                        }
                        break;
                    // GRAVAME SIMPLES
                    case "20":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Gravame Simples é necessário informar uma das chaves de consulta: Placa, Chassi!";
                            return retorno;
                        }
                        
                        if (featDocProprietarios && !featBaseNacional)
                        {
                            retorno.Mensagem = "Para obter a feature DOCUMENTO DE PROPRIETARIOS é preciso selecionar também a feature BASE NACIONAL";
                            return retorno;
                        }
                        break;
                    // PERDA TOTAL
                    case "21":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Perda Total é necessário informar uma das chaves de consulta: Placa!";
                            return retorno;
                        }
                        break;
                    // LEILAO SIMPLES
                    case "22":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Leilão Simples é necessário informar uma das chaves de consulta: Placa, Chassi!";
                            return retorno;
                        }
                        break;
                    // LEILAO COMPLETO
                    case "23":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Leilão Completo é necessário informar uma das chaves de consulta: Placa!";
                            return retorno;
                        }
                        break;
                    // RECALL
                    case "24":
                        if (placa.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Recall é necessário informar uma das chaves de consulta: Placa, UF!";
                            return retorno;
                        }
                        break;
                    // VEICULO TOTAL
                    case "25":
                        if ((placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0)) || uf.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Veiculo Total é necessário informar uma das chaves de consulta: Placa, Chassi, UF";
                            return retorno;
                        }
                        break;
					// PROPRIETARIOS ANTERIORES
                    case "68":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0) && cpfCnpj.Trim().Length.Equals(0) && tipoPessoa.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Proprietarios Anteriores é necessário informar uma das chaves de consulta: Placa, Chassi, CPFCNPJ e Tipo Pessoa";
                            return retorno;
                        }
                        
						if (cpfCnpj.Trim().Length > 0 && tipoPessoa.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Proprietarios Anteriores por documento é necessário informar o Tipo Pessoa";
                            return retorno;
                        }
                        break;
                    // AVALIACAO / AVALIAUTO
                    case "26":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Avaliação é necessário informar uma das chaves de consulta: Placa, Chassi!";
                            return retorno;
                        }
                        break;
                    // VEICULOS DECODIFICADOR DE CHASSI
                    case "27":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Veículos é necessário informar uma das chaves de consulta: Placa, Chassi!";
                            return retorno;
                        }
                        break;
                    // DEBITOS VEICULARES
                    case "28":
                        if ((placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0)) || uf.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Débito Veiculares é necessário informar uma das chaves de consulta: Placa, Chassi, UF";
                            return retorno;
                        }
                        break;
                    // CONCESSIONARIA
                    case "41":
                        if ((placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0)) || uf.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Concessionaria é necessário informar uma das chaves de consulta: Placa, Chassi, UF";
                            return retorno;
                        }
                        break;
                    // DIRIJA ESPECIAL
                    case "59":
                        if ((placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0)) || uf.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Dirija Especial é necessário informar uma das chaves de consulta: Placa, Chassi, UF";
                            return retorno;
                        }
                        break;
                    // DIRIJA ROUBO E FURTO
                    case "60":
                        if ((placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0)) || uf.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Dirija Roubo e Furto é necessário informar uma das chaves de consulta: Placa, Chassi, UF";
                            return retorno;
                        }
                        break;
                    // DIRIJA SEM ROUBO E FURTO
                    case "61":
                        if ((placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0)) || uf.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Dirija sem Roubo e Furto é necessário informar uma das chaves de consulta: Placa, Chassi, UF";
                            return retorno;
                        }
                        break;
                    // VEICULO TOTAL SEM UF
                    case "62":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Veiculo Total é necessário informar uma das chaves de consulta: Placa, Chassi";
                            return retorno;
                        }
                        break;
                    // CONCESSIONARIA SEM UF
                    case "63":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Concessionaria é necessário informar uma das chaves de consulta: Placa, Chassi";
                            return retorno;
                        }
                        break;
                    // DIRIJA ESPECIAL SEM UF
                    case "64":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Dirija Especial é necessário informar uma das chaves de consulta: Placa, Chassi";
                            return retorno;
                        }
                        break;
                    // DIRIJA ROUBO E FURTO SEM UF
                    case "65":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Dirija Roubo e Furto é necessário informar uma das chaves de consulta: Placa, Chassi";
                            return retorno;
                        }
                        break;
                    // TESTES BOA VISTA
                    case "71":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Dirija sem Roubo e Furto é necessário informar uma das chaves de consulta: Placa, Chassi";
                            return retorno;
                        }
                        break;
                    // DIRIJA SEM ROUBO E FURTO SEM UF
                    case "66":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Dirija sem Roubo e Furto é necessário informar uma das chaves de consulta: Placa, Chassi";
                            return retorno;
                        }
                        break;
                    // PRECIFICADOR I
                    case "70":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Precificador é necessário informar uma das chaves de consulta: Placa, Chassi";
                            return retorno;
                        }
                        break;
                    // PRECIFICADOR II
                    case "72":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta Precificador é necessário informar uma das chaves de consulta: Placa, Chassi";
                            return retorno;
                        }
                        break;
                    // RAIOX I
                    case "75":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta RAIO X é necessário informar uma das chaves de consulta: Placa, Chassi";
                            return retorno;
                        }
                        break;
                    // RAIOX II
                    case "76":
                        if (placa.Trim().Length.Equals(0) && chassi.Trim().Length.Equals(0))
                        {
                            retorno.Mensagem = "Para a consulta RAIO X 2 é necessário informar uma das chaves de consulta: Placa, Chassi";
                            return retorno;
                        }
                        break;

                    default:
                        retorno.Mensagem = "Tipo de consulta inválida";
                        return retorno;
                }

                #endregion

                #endregion

                #region CONSULTAS

                logLancamento = DataBases.getLaunching();

                switch (codProduto)
                {
                    // LEILAO SINTETICO
                    case "03":
                        #region LEILAO SINTETICO
                        dadosLeilao = new LeilaoController();
                        dadosAgregados = new AgregadosController();
                        dadosPerdaTotal = new SinistroController();

                        subTrans = "PC03";
                        retorno.Cabecalho = new AuxDadosConsulta("03", "LEILAO SINTETICO", parametros);
                        // RESPOSTA FORNECEDOR
                        retorno.Leilao = dadosLeilao.getLeilao(dadosUsuario, parametros.dadosVeiculo,3);
                        
                        // BILHETA  REQUISICAO
                        tmpReqFornecedor = String.IsNullOrEmpty(dadosLeilao.requisicaoFornecedor) ? (placa + "/" + chassi) : dadosLeilao.requisicaoFornecedor;
                        tmpReqFornecedor = tmpReqFornecedor.Replace('\'', '\"').Replace('\t', ' ').Replace('\n', ' ');
                        tmpReqFornecedor = tmpReqFornecedor.Length > 3999 ? tmpReqFornecedor.Substring(0, 3999) : tmpReqFornecedor;
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", subTrans, "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog1, tmpReqFornecedor.ToString());

                        logServer += dadosLeilao.logServer;

                        // RESPOSTA AGREGADOS
                        retorno.Agregados = dadosAgregados.getAgregados(dadosUsuario, parametros.dadosVeiculo, false);
                        logServer += dadosAgregados.logServer;

                        // RESPOSTA SINISTRO: PERDA TOTAL
                        retorno.PerdaTotal = dadosPerdaTotal.getPerdaTotal(dadosUsuario, parametros.dadosVeiculo, true);
                        logServer = ip + "|" + dadosPerdaTotal.logServer;
                    
                        // BILHETA RESPOSTA
                        serializer = new XmlSerializer(typeof(OKVeiculos));

                        using (StringWriter writer = new EncodingTextUTF8())
                        {
                            serializer.Serialize(writer, retorno);
                            tmpBuffer = writer.ToString();
                        }

                        tmpBuffer = tmpBuffer.Length > 3999 ? tmpBuffer.Replace('\'', '\"').Substring(0, 3999) : tmpBuffer.Replace('\'', '\"');

                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", subTrans, "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog, tmpBuffer.ToString());


                        #endregion
                    break;
                    // BASE NACIONAL
                    case "14":
                        #region BASE NACIONAL
                        dadosBaseNacional = new BinController();
                        subTrans = "PC14";
                        retorno.Cabecalho = new AuxDadosConsulta("14", "BIN NACIONAL", parametros);
                        // RESPOSTA FORNECEDOR
                        retorno.BinNacional = dadosBaseNacional.getBinNacional(dadosUsuario, parametros.dadosVeiculo, logLancamento);
                        // BILHETA  REQUISICAO
                        tmpReqFornecedor = String.IsNullOrEmpty(dadosBaseNacional.requisicaoFornecedor) ? (placa + "/" + chassi) : dadosBaseNacional.requisicaoFornecedor;
                        tmpReqFornecedor = tmpReqFornecedor.Replace('\'', '\"').Replace('\t', ' ').Replace('\n', ' ');
                        tmpReqFornecedor = tmpReqFornecedor.Length > 3999 ? tmpReqFornecedor.Substring(0, 3999) : tmpReqFornecedor;

                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", subTrans, "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog1, tmpReqFornecedor.ToString());

                        logServer += dadosBaseNacional.logServer;
                        // BILHETA RESPOSTA
                        serializer = new XmlSerializer(typeof(OKVeiculos));

                        using (StringWriter writer = new EncodingTextUTF8())
                        {
                            serializer.Serialize(writer, retorno);
                            tmpBuffer = writer.ToString();
                        }

                        tmpBuffer = tmpBuffer.Length > 3999 ? tmpBuffer.Replace('\'', '\"').Substring(0, 3999) : tmpBuffer.Replace('\'', '\"');

                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", subTrans, "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog, tmpBuffer.ToString());
                        #endregion
                    break;
                    // BASE ESTADUAL
                    case "16":
                        #region BASE ESTADUAL
                        dadosBaseEstadual = new BinController();
                        subTrans = "PC16";
                        retorno.Cabecalho = new AuxDadosConsulta("16", "BIN ESTADUAL", parametros);
                        // RESPOSTA FORNECEDOR
                        retorno.BinEstadual = dadosBaseEstadual.getBinEstadual(dadosUsuario, parametros.dadosVeiculo, logLancamento, false);
                        // BILHETA  REQUISICAO
                        tmpReqFornecedor = String.IsNullOrEmpty(dadosBaseEstadual.requisicaoFornecedor) ? (placa + "/" + chassi) : dadosBaseEstadual.requisicaoFornecedor;
                        tmpReqFornecedor = tmpReqFornecedor.Replace('\'', '\"').Replace('\t', ' ').Replace('\n', ' ');
                        tmpReqFornecedor = tmpReqFornecedor.Length > 3999 ? tmpReqFornecedor.Substring(0, 3999) : tmpReqFornecedor;

                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", subTrans, "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog1, tmpReqFornecedor.ToString());

                        logServer += dadosBaseEstadual.logServer;
                        // BILHETA RESPOSTA
                        serializer = new XmlSerializer(typeof(OKVeiculos));

                        using (StringWriter writer = new EncodingTextUTF8())
                        {
                            serializer.Serialize(writer, retorno);
                            tmpBuffer = writer.ToString();
                        }

                        tmpBuffer = tmpBuffer.Length > 3999 ? tmpBuffer.Replace('\'', '\"').Substring(0, 3999) : tmpBuffer.Replace('\'', '\"');

                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", subTrans, "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog, tmpBuffer.ToString());

                        #endregion
                    break;
                    // BASE NACIONAL ROUBO E FURTO - BINRF
                    case "17":
                        #region BASE NACIONAL ROUBO E FURTO - BINRF
                        dadosBaseNacional = new BinController();
                        subTrans = "PC17";

                        retorno.Cabecalho = new AuxDadosConsulta("17", "BIN ROUBO E FURTO", parametros);
                        // RESPOSTA FORNECEDOR
                        retorno.BinRouboFurto = dadosBaseNacional.getBinRouboFurto(dadosUsuario, parametros.dadosVeiculo, logLancamento, false, true);
                        // BILHETA  REQUISICAO
                        tmpReqFornecedor = String.IsNullOrEmpty(dadosBaseNacional.requisicaoFornecedor) ? (placa + "/" + chassi) : dadosBaseNacional.requisicaoFornecedor;
                        tmpReqFornecedor = tmpReqFornecedor.Length > 3999 ? tmpReqFornecedor.Replace('\'', '\"').Substring(0, 3999) : tmpReqFornecedor.Replace('\'', '\"');
                        
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", subTrans, "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog1, tmpReqFornecedor);

                        logServer += dadosBaseNacional.logServer;
                        // BILHETA RESPOSTA
                        serializer = new XmlSerializer(typeof(OKVeiculos));

                        using (StringWriter writer = new EncodingTextUTF8())
                        {
                            serializer.Serialize(writer, retorno);
                            tmpBuffer = writer.ToString();
                        }

                        tmpBuffer = tmpBuffer.Length > 3999 ? tmpBuffer.Replace('\'', '\"').Substring(0, 3999) : tmpBuffer.Replace('\'', '\"');

                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", subTrans, "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog, tmpBuffer.ToString());
                        #endregion
                    break;
                    // AGREGADOS
                    case "15":
                        #region AGREGADOS
                        dadosAgregados = new AgregadosController();
                        subTrans = "SI15";

                        retorno.Cabecalho = new AuxDadosConsulta("15", "AGREGADOS", parametros);
                        // RESPOSTA FORNECEDOR
                        retorno.Agregados = dadosAgregados.getAgregados(dadosUsuario, parametros.dadosVeiculo, false);
                        // BILHETA  REQUISICAO
                        tmpReqFornecedor = String.IsNullOrEmpty(dadosAgregados.requisicaoFornecedor) ? (placa + "/" + chassi) : dadosAgregados.requisicaoFornecedor;
                        tmpReqFornecedor = tmpReqFornecedor.Length > 3999 ? tmpReqFornecedor.Replace('\'', '\"').Substring(0, 3999) : tmpReqFornecedor.Replace('\'', '\"');
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", subTrans, "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog1, tmpReqFornecedor);
                        
                        logServer += dadosAgregados.logServer;
                        
                        // BILHETA RESPOSTA
                        serializer = new XmlSerializer(typeof(OKVeiculos));

                        using (StringWriter writer = new EncodingTextUTF8())
                        {
                            serializer.Serialize(writer, retorno);
                            tmpBuffer = writer.ToString();
                        }
                        
                        tmpBuffer = tmpBuffer.Length > 3999 ? tmpBuffer.Replace('\'', '\"').Substring(0, 3999) : tmpBuffer.Replace('\'', '\"');

                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", subTrans, "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog, tmpBuffer.ToString());

                        if(featPrecificador)
                        {
                            dadosPrecificador = new PrecificadorController();
                            retorno.Precificador = dadosPrecificador.getPredificador(dadosUsuario, parametros.dadosVeiculo);

                            DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", "FT74", "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog1, "-");
                            DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", "FT74", "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog, "-");
                        }
                        #endregion
                    break;
                    // GRAVAME COMPLETO
                    case "19":
                        #region GRAVAME COMPLETO
                        dadosGravame = new GravameController();
                        dadosDecodChassi = new DecodChassiController();
                        dadosBaseNacional = new BinController();
                        subTrans = "PC19";

                        retorno.Cabecalho = new AuxDadosConsulta("19", "GRAVAME COMPLETO", parametros);
                        // BILHETA  REQUISICAO
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", subTrans, "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog1, chassi + "/" + placa);
                        // RESPOSTA GRAVAME
                        retorno.Gravame = dadosGravame.getGravame(dadosUsuario, parametros.dadosVeiculo, true);
                        logServer += dadosGravame.logServer;

                        // RESPOSTA BIN NACIONAL
                        retorno.BinNacional = dadosBaseNacional.getBinNacional(dadosUsuario, parametros.dadosVeiculo, logLancamento, true, featDocProprietarios ? true : false);
                        logServer += dadosBaseNacional.logServer;
                        // RESPOSTA DECOD CHASSI
                        retorno.DecodChassi = dadosDecodChassi.getDecodChassi(dadosUsuario, parametros.dadosVeiculo, true);
                        logServer += dadosDecodChassi.logServer;

                        // BILHETA RESPOSTA
                        serializer = new XmlSerializer(typeof(OKVeiculos));

                        using (StringWriter writer = new EncodingTextUTF8())
                        {
                            serializer.Serialize(writer, retorno);
                            tmpBuffer = writer.ToString();
                        }

                        tmpBuffer = tmpBuffer.Length > 3999 ? tmpBuffer.Replace('\'', '\"').Substring(0, 3999) : tmpBuffer.Replace('\'', '\"');

                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", subTrans, "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog, tmpBuffer.ToString());

                        // FEATURES
                        if (featLeilao)
                        {
                            dadosLeilao = new LeilaoController();
                            retorno.Leilao = dadosLeilao.getLeilao(dadosUsuario, parametros.dadosVeiculo, true);
                            
                            logServer = ip + "|" + dadosLeilao.logServer;

                            DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", "FT22", "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog1, "-");
                            DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", "FT22", "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog, "-");
                        }

                        if(featPerdaTotal)
                        {
                            dadosPerdaTotal = new SinistroController();
                            retorno.PerdaTotal = dadosPerdaTotal.getPerdaTotal(dadosUsuario, parametros.dadosVeiculo, true);

                            logServer = ip + "|" + dadosPerdaTotal.logServer;

                            DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", "FT21", "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog1, "-");
                            DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", "FT21", "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog, "-");
                        }

                        if (featPrecificador)
                        {
                            dadosPrecificador = new PrecificadorController();
                            retorno.Precificador = dadosPrecificador.getPredificador(dadosUsuario, parametros.dadosVeiculo);

                            logServer = ip + "|" + dadosPrecificador.logServer;

                            DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", "FT74", "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog1, "-");
                            DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", "FT74", "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog, "-");
                        }

                        if (featDocProprietarios)
                        {
                            var serverid_feat = ip + "|FT_PROPRIETARIO";
                            DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", "FT68", "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog1, "-");
                            DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", "FT68", "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog, "-");
                        }

                        #endregion
                    break;
                    // GRAVAME SIMPLES
                    case "20":
                        #region GRAVAME SIMPLES
                        subTrans = "PC20";
                        dadosGravame = new GravameController();
                        dadosBaseNacional = new BinController();
                        retorno.Cabecalho = new AuxDadosConsulta("20", "GRAVAME SIMPLES", parametros);
                        // RESPOSTA FORNECEDOR
                        retorno.Gravame = dadosGravame.getGravame(dadosUsuario, parametros.dadosVeiculo, false);
                        logServer += dadosGravame.logServer;

                        // BILHETA  REQUISICAO
                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", subTrans, "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog1, chassi + "/" + placa);

                        // BILHETA RESPOSTA
                        serializer = new XmlSerializer(typeof(OKVeiculos));
                        
                        using (StringWriter writer = new EncodingTextUTF8())
                        {
                            serializer.Serialize(writer, retorno);
                            tmpBuffer = writer.ToString();
                        }

                        tmpBuffer = tmpBuffer.Length > 3999 ? tmpBuffer.Replace('\'', '\"').Substring(0, 3999) : tmpBuffer.Replace('\'', '\"');

                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", subTrans, "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog, tmpBuffer.ToString());

                        // FEATURES
                        if (featBaseNacional)
                        {
                            retorno.BinNacional = dadosBaseNacional.getBinNacional(dadosUsuario, parametros.dadosVeiculo, logLancamento, true);
                            
                            logServer = ip + "|" + dadosBaseNacional.logServer;

                            DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", "FT14", "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog1, "-");
                            DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", "FT14", "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog, "-");
                            
                            // NÃO RETORNA DADOS DO PROPRIETARIO SE NÃO SELECIONAR A FEATURE
                            if (!featDocProprietarios)
                            {
                                retorno.BinNacional.Automovel.Proprietario = null;
                                retorno.BinNacional.Obs = "DOCUMENTO DO PROPRIETARIO NAO SELECIONADO";
                            }
                            else
                            {
                                var serverid_feat = ip + "|FT_PROPRIETARIO";
                                DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", "FT68", "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog1, "-");
                                DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", "FT68", "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog, "-");
                            }
                        }
                        

                        #endregion
                    break;
                    // DECODIFICADOR CHASSI
                    case "27":
                        #region DECODIFICADOR CHASSI
                        dadosDecodChassi = new DecodChassiController();
                        subTrans = "PC27";
                        retorno.Cabecalho = new AuxDadosConsulta("27", "DECODIFICADOR DE CHASSI", parametros);
                        // RESPOSTA FORNECEDOR
                        retorno.DecodChassi = dadosDecodChassi.getDecodChassi(dadosUsuario, parametros.dadosVeiculo, true);
                        // BILHETA  REQUISICAO
                        tmpReqFornecedor = String.IsNullOrEmpty(dadosDecodChassi.requisicaoFornecedor) ? (placa + "/" + chassi) : dadosDecodChassi.requisicaoFornecedor;
                        tmpReqFornecedor = tmpReqFornecedor.Replace('\'', '\"').Replace('\t', ' ').Replace('\n', ' ');
                        tmpReqFornecedor = tmpReqFornecedor.Length > 3999 ? tmpReqFornecedor.Substring(0, 3999) : tmpReqFornecedor;

                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", subTrans, "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog1, tmpReqFornecedor.ToString());

                        logServer += dadosDecodChassi.logServer;
                        // BILHETA RESPOSTA
                        serializer = new XmlSerializer(typeof(OKVeiculos));

                        using (StringWriter writer = new EncodingTextUTF8())
                        {
                            serializer.Serialize(writer, retorno);
                            tmpBuffer = writer.ToString();
                        }

                        tmpBuffer = tmpBuffer.Length > 3999 ? tmpBuffer.Replace('\'', '\"').Substring(0, 3999) : tmpBuffer.Replace('\'', '\"');

                        DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", subTrans, "", "", DateTime.Now, logServer, tmpLog, tmpLog, tmpLog, tmpLog, tmpLog, tmpBuffer.ToString());
                        #endregion
                    break;
                }
                 /* 
                // VEÍCULO DIRIJA
                else if ((tipoConsulta.Equals("67")))
                {
                    VeiculoDirija veiculodirija = new VeiculoDirija(logon, senha, ip, placa, chassi);
                    xmlRetorno = veiculodirija.Consultar();
                }
                // DEBITOS VEICULARES
                else if (tipoConsulta.Equals("28") && (placa.Length > 0 || chassi.Length > 0) && (uf.Length > 0))
                {

                    VeiculoDebitoVeicular veiculoDebitoVeicular = new VeiculoDebitoVeicular(logon, senha, ip, placa, chassi, uf);
                    xmlRetorno = veiculoDebitoVeicular.Consultar();
                }
                // PERDA TOTAL
                else if (tipoConsulta.Equals("21") && (placa.Length > 0 || chassi.Length > 0))
                {
                    VeiculoPerdaTotal veiculoperdatotal = new VeiculoPerdaTotal(logon, senha, ip, chassi, placa);
                    xmlRetorno = veiculoperdatotal.Consultar();
                }
                // LEILAO SIMPLES CARSYS OU ABSOLUTA
                else if (tipoConsulta.Equals("22") && (placa.Length > 0 && chassi.Length > 0))
                {
                    VeiculoLeilaoSimples veiculoleilaosimples = new VeiculoLeilaoSimples(logon, senha, ip, placa, chassi);
                    xmlRetorno = veiculoleilaosimples.Consultar();
                }
                // LEILAO COMPLETO
                else if (tipoConsulta.Equals("23") && (placa.Length > 0 || chassi.Length > 0))
                {
                    VeiculoLeilaoCompleto veiculoleilaocompleto = new VeiculoLeilaoCompleto(logon, senha, ip, placa, chassi, featRouboFurto, featPrecificador, featGravame, featProprietario);
                    xmlRetorno = veiculoleilaocompleto.Consultar();
                }
                else if (tipoConsulta.Equals("24") && (placa.Length > 0 || chassi.Length > 0))
                {
                    Veiculo_Recall veiculoRecall = new Veiculo_Recall(logon, senha, ip, placa, chassi);
                    xmlRetorno = veiculoRecall.Consultar();
                }
                else if ((tipoConsulta.Equals("65") || (tipoConsulta.Equals("60"))))
                {
                    if (logs.consultaFornecedor(18, 1))
                    {
                        VeiculoConcessionariaRF veiculoConcessionariaRF = new VeiculoConcessionariaRF(logon, senha, ip, chassi, uf, placa, renavam, motor, crlv, ufCrlv, cpfCnpj, ddd1, telefone1, ddd2, telefone2, tipoPessoa, tipoConsulta);
                        xmlRetorno = veiculoConcessionariaRF.Consultar();

                    }
                    else
                    {
                        okveiculos = new OKVeiculos(logon, senha, ip, chassi, uf, placa, renavam, motor, crlv, ufCrlv, cpfCnpj, ddd1, telefone1, ddd2, telefone2, tipoPessoa, tipoConsulta);
                        xmlRetorno = okveiculos.Consultar();
                    }
                }
                else if ((tipoConsulta.Equals("63") || (tipoConsulta.Equals("41"))))
                {
                    if (logs.consultaFornecedor(17, 1))
                    {
                        VeiculoConcessionaria veiculoConcessionaria = new VeiculoConcessionaria(logon, senha, ip, chassi, uf, placa, renavam, motor, crlv, ufCrlv, cpfCnpj, ddd1, telefone1, ddd2, telefone2, tipoPessoa, tipoConsulta);
                        xmlRetorno = veiculoConcessionaria.Consultar();

                    }
                    else
                    {
                        okveiculos = new OKVeiculos(logon, senha, ip, chassi, uf, placa, renavam, motor, crlv, ufCrlv, cpfCnpj, ddd1, telefone1, ddd2, telefone2, tipoPessoa, tipoConsulta);
                        xmlRetorno = okveiculos.Consultar();

                    }
                }
                else if (tipoConsulta.Equals("68") && (placa.Length > 0))
                {
                    VeiculoProprietarios veiculoProprietarios = new VeiculoProprietarios(logon, senha, ip, placa);
                    xmlRetorno = veiculoProprietarios.Consultar();
                }
                //VEICULOS PROPRIETARIOS POR DOCUMENTO E CHASSI
                else if (tipoConsulta.Equals("68") && ((cpfCnpj.Length > 0 && tipoPessoa.Length > 0) || chassi.Length > 0))
                {
                    VeiculoProprietariosII VeiculoProprietarios = new VeiculoProprietariosII(logon, senha, ip, cpfCnpj, tipoPessoa, chassi);
                    xmlRetorno = VeiculoProprietarios.Consultar();
                }
                else if ((tipoConsulta.Equals("62") || (tipoConsulta.Equals("25"))))
                {
                    if (logs.consultaFornecedor(16, 1))
                    {
                        VeiculoTotal veiculoToTal = new VeiculoTotal(logon, senha, ip, chassi, uf, placa, renavam, motor, crlv, ufCrlv, cpfCnpj, ddd1, telefone1, ddd2, telefone2, tipoPessoa, featRouboFurto, featSTF, featSTJ, featTST, featProprietario, tipoConsulta);
                        xmlRetorno = veiculoToTal.Consultar();
                    }
                    else
                    {
                        okveiculos = new OKVeiculos(logon, senha, ip, chassi, uf, placa, renavam, motor, crlv, ufCrlv, cpfCnpj, ddd1, telefone1, ddd2, telefone2, tipoPessoa, tipoConsulta);
                        xmlRetorno = okveiculos.Consultar();
                    }
                }
                //TESTES DA BOA VISTA 71 - DPVAT 72 - ROUBO E FURTO 73 - LEILAO
                else if (tipoConsulta.Equals("71") && (placa != "" || chassi != ""))
                {
                    if (placa != "")
                        throw new Exception("DPVAT só pode ser consultado por Placa");

                    VeiculoDpvat vdpvat = new VeiculoDpvat(logon, senha, ip, chassi, placa);
                    xmlRetorno = vdpvat.Consultar();
                }
                // Precificador I
                else if (tipoConsulta.Equals("70") && (placa.Length > 0 || chassi.Length > 0))
                {
                    VeiculoPrecificador veiculoPrecificador = new VeiculoPrecificador(logon, senha, ip, placa, chassi);
                    xmlRetorno = veiculoPrecificador.Consultar();
                }
                // Precificador II
                else if (tipoConsulta.Equals("72") && (placa.Length > 0 || chassi.Length > 0))
                {
                    VeiculoPrecificadorII veiculoPrecificador = new VeiculoPrecificadorII(logon, senha, ip, placa, chassi);
                    xmlRetorno = veiculoPrecificador.Consultar();
                }

                else
                {
                    okveiculos = new OKVeiculos(logon, senha, ip, chassi, uf, placa, renavam, motor, crlv, ufCrlv, cpfCnpj, ddd1, telefone1, ddd2, telefone2, tipoPessoa, tipoConsulta);
                    xmlRetorno = okveiculos.Consultar();
                    //xmlRetorno = okveiculos.ConsultarModulo();
                }
                */
                #endregion
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message + ex.StackTrace;

                LogEstatico.setLogTitulo("ERRO VEICULO -> " + System.DateTime.Now);
                LogEstatico.setLogTexto(ex.Message + ex.StackTrace);
                LogEstatico.setLogTexto("Parametros: " + logon + "/" + senha + "/" + chassi + "/" + uf + "/" + placa + "/" + renavam + "/" + nrMotor + "/" + crlv + "/" + ufCrlv + "/" + cpfCnpj + "/" + ddd1 + "/" + telefone1 + "/" + ddd2 + "/" + telefone2 + "/" + tipoPessoa + "/" + featRouboFurto + "/" + featSTF + "/" + featSTJ + "/" + featTST + "/" + featGravame + "/" + featPrecificador + "/" + featLeilao + "/" + featPerdaTotal + "/" + featBaseNacional + "/" + featDocProprietarios + "/" + codProduto);
            }
            //// TODO: CRIANDO O XML DE RETORNO
            //serializer = new XmlSerializer(typeof(OKVeiculos));

            //using (StringWriter writer = new EncodingTextUTF8())
            //{
            //    serializer.Serialize(writer, retorno);
            //}

            // INSERE LOG DE RESPOSTA
            // DataBases.InsertLog(Convert.ToDecimal(logLancamento), logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), logBuffer.ToString());

            return retorno;
        }
    }
}
