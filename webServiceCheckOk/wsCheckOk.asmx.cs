using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;
using webServiceCheckOk.Controle.Inteligencia;
using webServiceCheckOk.Controle.Inteligencia.Utils;
using webServiceCheckOk.Controle.ProdutosController;
using webServiceCheckOk.Model;
using webServiceCheckOk.Model.ProdutosModel;

namespace webServiceCheckOk
{
    /// <summary>
    /// Branch Local: Teste 11.03.2016
    /// AUTHOR: Domingos Ribeiro <dofari.dfr@gmail.com>
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // EN: To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // PT: Para permitir que esse Web Service a ser chamado a partir do script , utilizando ASP.NET AJAX, remova o comentário da seguinte linha.
    // [System.Web.Script.Services.ScriptService]
    public class wsCheckOk : System.Web.Services.WebService
    {
        #region veiculos

        [WebMethod]
        public OKVeiculos OkVeiculos(string codProduto, string logon, string senha, string chassi, string uf, string placa, string renavam, string nrMotor, string nrCarroceria, string nrEixoTras, string nrTercEixo, string nrCxCambio, string crlv, string ufCrlv, string cpfCnpj, string tipoPessoa, string ddd1, string telefone1, string ddd2, string telefone2, bool featRouboFurto = false, bool featSTF = false, bool featSTJ = false, bool featTST = false, bool featGravame = false, bool featPrecificador = false, bool featLeilao = false, bool featPerdaTotal = false, bool featBaseNacional = false, bool featProprietario = false)
        {
            string ip = Context.Request.ServerVariables["REMOTE_ADDR"];

            System.Text.RegularExpressions.Regex regLetras = new System.Text.RegularExpressions.Regex("[^A-Z]");
            System.Text.RegularExpressions.Regex regNumeros = new System.Text.RegularExpressions.Regex("[^0-9]");

            OKVeiculos retorno = new OKVeiculos();
            UsuarioModel dadosUsuario = new UsuarioModel();
            Veiculo paramVeiculo = new Veiculo();
            AuxParametros parametros = new AuxParametros();

            try
            {
                retorno.veiculo = null;

                #region PARAMETROS

                dadosUsuario.Logon = logon != String.Empty ? logon : null;
                dadosUsuario.Senha = senha != String.Empty ? senha : null;

                paramVeiculo.Chassi = chassi != String.Empty ? chassi : null;
                paramVeiculo.Uf = uf != String.Empty ? uf : null;
                paramVeiculo.Placa = placa != String.Empty ? placa : null;
                paramVeiculo.Renavam = renavam != String.Empty ? renavam : null;
                paramVeiculo.NrMotor = nrMotor != String.Empty ? nrMotor : null;
                paramVeiculo.Crlv = crlv != String.Empty ? crlv : null;
                paramVeiculo.UfCrlv = ufCrlv != String.Empty ? ufCrlv : null;

                paramVeiculo = Util.unSetDadosVazios<Veiculo>(paramVeiculo);
                parametros.dadosVeiculo = paramVeiculo;

                // string cpfCnpj, string tipoPessoa, string ddd1, string telefone1, string ddd2, string telefone2
                parametros.dadosPessoa = new Pessoa();
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
                
                parametros.dadosPessoa = Util.unSetDadosVazios<Pessoa>(parametros.dadosPessoa);

                parametros.features = new AuxParametrosFeatures(featRouboFurto, featSTF, featSTJ, featTST, featGravame, featPrecificador, featLeilao, featPerdaTotal, featBaseNacional, featProprietario);
                
                #endregion

                #region VALIDACAO
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
                if (featProprietario)
                {
                    Verificacao checagem = new Verificacao();
                    string receita = checagem.getReceitaLogon(logon);

                    if (!receita.Equals("56"))
                        featProprietario = false;
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

                // AUTORIZACAO
                Autorizacao autorizando = new Autorizacao(logon, senha, codProduto.Length == 2 ? "PC" + codProduto : codProduto);

                autorizando.verificaAutorizacao();
                if (autorizando.erro == null)
                    autorizando.verificaAgenda();
                if (autorizando.erro == null)
                    autorizando.verificaTravaFinanceira();

                if (autorizando.erro != null)
                {
                    retorno.Mensagem = autorizando.erro.Codigo.ToString() +": "+ autorizando.erro.Descricao.ToString();
                    return retorno;
                }
            #endregion

            #region CONSULTAS

                // LEILAO SINTETICO
                if (codProduto.Equals("03") && (placa.Length > 0 || chassi.Length > 0))
                {
                    LeilaoController dadosLeilao = new LeilaoController();
                    retorno.Cabecalho = new AuxDadosConsulta("03", "LEILAO SINTETICO", parametros);
                    retorno.Leilao = dadosLeilao.getLeilao(dadosUsuario, paramVeiculo);
                }
                // BASE NACIONAL
                else if (codProduto.Equals("14") && (placa.Length > 0 || chassi.Length > 0 || nrMotor.Length > 0 || renavam.Length > 0))
                {
                    BinController dadosBaseNacional = new BinController();
                    retorno.Cabecalho = new AuxDadosConsulta("14", "BIN NACIONAL", parametros);
                    retorno.BinNacional = dadosBaseNacional.getBinNacional(dadosUsuario, paramVeiculo, true, true);
                }
                // BASE ESTADUAL
                else if (codProduto.Equals("16") && (placa.Length > 0 || chassi.Length > 0) && (uf.Length > 0))
                {
                    BinController dadosBaseEstadual = new BinController();
                    retorno.Cabecalho = new AuxDadosConsulta("16", "BIN ESTADUAL", parametros);
                    retorno.BinEstadual = dadosBaseEstadual.getBinEstadual(dadosUsuario, paramVeiculo, true);
                }
                // BASE NACIONAL ROUBO E FURTO - BINRF
                else if (codProduto.Equals("17") && (placa.Length > 0 || chassi.Length > 0 || nrMotor.Length > 0 || renavam.Length > 0))
                {
                    BinController dadosBaseNacional = new BinController();
                    retorno.Cabecalho = new AuxDadosConsulta("17", "BIN ROUBO E FURTO", parametros);
                    retorno.BinRouboFurto = dadosBaseNacional.getBinRouboFurto(dadosUsuario, paramVeiculo, true, true);
                }
                // AGREGADOS
                else if (codProduto.Equals("15") && (placa.Length > 0 || chassi.Length > 0 || nrCarroceria.Length > 0 || nrEixoTras.Length > 0 || nrTercEixo.Length > 0 || nrCxCambio.Length > 0 || nrMotor.Length > 0))
                {
                    AgregadosController dadosAgregados = new AgregadosController();
                    retorno.Cabecalho = new AuxDadosConsulta("15", "AGREGADOS", parametros);
                    retorno.Agregados = dadosAgregados.getAgregados(dadosUsuario, paramVeiculo, false);
                    if(featPrecificador)
                    {
                        PrecificadorController dadosPrecificador = new PrecificadorController();
                        retorno.Precificador = dadosPrecificador.getPredificador(dadosUsuario, paramVeiculo);
                    }
                }
                // GRAVAME COMPLETO
                else if (codProduto.Equals("19") && (placa.Length > 0 || chassi.Length > 0))
                {
                    GravameController dadosGravame = new GravameController();
                    LeilaoController dadosLeilao = new LeilaoController();
                    PrecificadorController dadosPrecificador = new PrecificadorController();
                    SinistroController dadosPerdaTotal = new SinistroController();

                    retorno.Cabecalho = new AuxDadosConsulta("19", "GRAVAME COMPLETO", parametros);
                    retorno.Gravame = dadosGravame.getGravame(dadosUsuario, paramVeiculo, true);
                    retorno.Leilao = dadosLeilao.getLeilao(dadosUsuario, paramVeiculo, true);
                    retorno.Precificador = dadosPrecificador.getPredificador(dadosUsuario, paramVeiculo);
                    retorno.PerdaTotal = dadosPerdaTotal.getPerdaTotal(dadosUsuario, paramVeiculo, true);
                }
                // GRAVAME SIMPLES
                else if (codProduto.Equals("20") && (chassi.Length > 0))
                {
                    GravameController dadosGravame = new GravameController();
                    BinController dadosBaseNacional = new BinController();
                    retorno.Cabecalho = new AuxDadosConsulta("20", "GRAVAME SIMPLES", parametros);
                    retorno.Gravame = dadosGravame.getGravame(dadosUsuario, paramVeiculo, false);
                    retorno.BinNacional = dadosBaseNacional.getBinNacional(dadosUsuario, paramVeiculo, true);
                }
                // DECODIFICADOR CHASSI
                else if (codProduto.Equals("27") && (placa.Length > 0 || chassi.Length > 0))
                {
                    DecodChassiController dadosDecodChassi = new DecodChassiController();
                    retorno.Cabecalho = new AuxDadosConsulta("27", "DECODIFICADOR DE CHASSI", parametros);
                    retorno.DecodChassi = dadosDecodChassi.getDecodChassi(dadosUsuario, paramVeiculo, true);
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
                // ROUBO E FURTO
                else if (tipoConsulta.Equals("17") && (placa.Length > 0 || chassi.Length > 0))
                {
                    VeiculoRouboEFurto veiculorouboefurto = new VeiculoRouboEFurto(logon, senha, ip, placa, chassi);
                    xmlRetorno = veiculorouboefurto.Consultar();
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
                // DECODIFICADOR DE CHASSI
                else if (tipoConsulta.Equals("27") && (placa.Length > 0 || chassi.Length > 0))
                {
                    VeiculoDecodificadorDeChassi veiculodecodificadordechassi = new VeiculoDecodificadorDeChassi(logon, senha, ip, placa, chassi);
                    xmlRetorno = veiculodecodificadordechassi.Consultar();
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
                LogEstatico.setLogTexto("Parametros: " + logon + "/" + senha + "/" + chassi + "/" + uf + "/" + placa + "/" + renavam + "/" + nrMotor + "/" + crlv + "/" + ufCrlv + "/" + cpfCnpj + "/" + ddd1 + "/" + telefone1 + "/" + ddd2 + "/" + telefone2 + "/" + tipoPessoa + "/" + featRouboFurto + "/" + featSTF + "/" + featSTJ + "/" + featTST + "/" + featGravame + "/" + featPrecificador + "/" + featLeilao + "/" + featPerdaTotal + "/" + featBaseNacional + "/" + featProprietario + "/" + codProduto);
            }
            // TODO: CRIANDO O XML DE RETORNO
            var serializer = new XmlSerializer(typeof(OKVeiculos));

            using (StringWriter writer = new EncodingTextUTF8())
            {
                serializer.Serialize(writer, retorno);
            }

            return retorno;
        }
        #endregion
    }
}
