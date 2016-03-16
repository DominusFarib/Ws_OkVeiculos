using System;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using webServiceCheckOk.Model;
using webServiceCheckOk.Model.ProdutosModel;
using webServiceCheckOk.Controle.Inteligencia;
using webServiceCheckOk.Controle.Inteligencia.Utils;

namespace webServiceCheckOk.Controle.Fornecedores
{
    public class FornConsultAuto
    {
        public UsuarioModel dadosUsuario = new UsuarioModel();

        public string codConsulta { get; set; }
        public string tipoConsulta { get; set; }
        public string urlRequisicao { get; set; }
        public string logLancamento { get; set; }

        private string retornoWs = string.Empty;
        private string acesLogin = string.Empty;
        private string acesPassword = string.Empty;
        
        private BinModel binNacional;
        private BinEstadualModel binEstadual;
        private BinRouboFurtoModel binRouboFurto;

        private Veiculo carro;
        private List<Veiculo> dadosCarro { get; set; }

        // CONSTRUTOR
        public FornConsultAuto(Veiculo carro)
        {
            this.carro = carro;
            this.acesLogin = "CHECKOK";
            this.acesPassword = "chkok1977x";
        }

        // BIN ESTADUAL RESPOSTA
        public BinEstadualModel getBinEstadual()
        {
            try
            {
                // MONTA A URL DO FORNECEDOR
                setUrlRequisicao("BE");

                this.binEstadual = new BinEstadualModel();
                // FAZ A CONSULTA NO FORNECEDOR
                var request = new CustomTimeOut();
                var relogio = Stopwatch.StartNew();
                this.retornoWs = request.DownloadString(this.urlRequisicao);
                var decorrido = relogio.ElapsedMilliseconds;
                // RETORNO PADRÃO DO FORNECEDOR
                // this.retornoWs = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><RESPOSTA><RETORNO><EXISTE_ERRO>0</EXISTE_ERRO><MSG_ERRO></MSG_ERRO><MSG_ERRO2/><CHAVERETORNO>160204165411839</CHAVERETORNO><UF>SP</UF><CHASSI>9BFZF26P897161916</CHASSI><PLACA>DZB2487</PLACA><RENAVAM>00934938658</RENAVAM><MUNICIPIO>SAO PAULO</MUNICIPIO><MODELO>FIESTA SEDAN1.6FLEX</MODELO><MARCA>FORD</MARCA><COR>PRETA</COR><VEIANOFABR>2007</VEIANOFABR><VEIANOMODELO>2008</VEIANOMODELO><COMBUSTIVEL>ALCOOL/GASOLINA</COMBUSTIVEL><TIPO>AUTOMOVEL</TIPO><ESPECIE>PASSAGEIRO</ESPECIE><CATEGORIA>PARTICULAR</CATEGORIA><CARROCERIA>NAO APLICAVEL</CARROCERIA><TIPOREMARCACAOCHASSI>NORMAL</TIPOREMARCACAOCHASSI><VEIPROCEDENCIA>NACIONAL</VEIPROCEDENCIA><RESTRICAO1>ALIENACAO FIDUCIARIA</RESTRICAO1><RESTRICAO2>NAO CONSTA COMUNICACAO DE VENDAS</RESTRICAO2><RESTRICAO3>NADA CONSTA</RESTRICAO3><RESTRICAO4>NADA CONSTA</RESTRICAO4><NOMEPROPRIETARIO>DOUGLAS FARAHT RIBBAH</NOMEPROPRIETARIO><CPFFATURADO>04878806000138</CPFFATURADO><TIPODOCUMENTOFATURADO>FISICA</TIPODOCUMENTOFATURADO><SITUACAOVEICULO>CIRCULACAO</SITUACAOVEICULO><DEBSIPVALICENCIAMENTO>EXISTE DEBITO DE IPVA/NAO EXISTE DEBITO DE LICENCIAMENTO</DEBSIPVALICENCIAMENTO><DEBSMULTA>NAO EXISTE DEBITO DE MULTA</DEBSMULTA><VALORTOTALDEBIPVA>730,39</VALORTOTALDEBIPVA><VALORTOTALDEBLIC>0,00</VALORTOTALDEBLIC><VALORTOTALMULTAS>0,00</VALORTOTALMULTAS><VALORTOTALDPVAT>105,65</VALORTOTALDPVAT><DATAATUALIZACAO>08/05/2015</DATAATUALIZACAO><CAPPASS>5</CAPPASS><CMT>0,00</CMT><PBT>0,00</PBT><EIXOS>2</EIXOS><POTENCIA>111</POTENCIA><TIPOTRANSACAO></TIPOTRANSACAO><RESTRICAOFINAN>ALIENACAO FIDUCIARIA</RESTRICAOFINAN><NOMEAGENTEFINAN>AYMORE CRED FIN INV SA</NOMEAGENTEFINAN><CGCFINAN>123.654.123-51</CGCFINAN><NOMEFINANCIADO>DOUGLAS FARAHT RIBBAH</NOMEFINANCIADO><DATAINCLUSAOINTENCAOTROCA>19/05/2014</DATAINCLUSAOINTENCAOTROCA><NUMEROCONTRATO></NUMEROCONTRATO><AGENTEFINANCEIRO></AGENTEFINANCEIRO><DATAVIGENCIACONTRATO></DATAVIGENCIACONTRATO><CAPACIDADECARGA>0,00</CAPACIDADECARGA><MOTOR>QFJA88161916</MOTOR><CILINDRADA>1598</CILINDRADA><ULTIMAATUALIZACAO>27/11/2012</ULTIMAATUALIZACAO><NUMERO_CAIXACAMBIO></NUMERO_CAIXACAMBIO><TIPOMONTAGEM>COMPLETA</TIPOMONTAGEM><CPF_CNPJ_PROPRIETARIO>12365412351</CPF_CNPJ_PROPRIETARIO></RETORNO></RESPOSTA>";

                // GRAVA O TEMPO DECORRIDO EM ARQUIVO DE LOG
                try
                {
                    LogEstatico.setLogTitulo("TEMPO BIN ESTADUAL: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                    LogEstatico.setLogTexto("CHAVE: " + this.logLancamento);
                    LogEstatico.setLogTexto("FORNECEDOR: CONSULTAUTO");
                    LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                    LogEstatico.setLogTexto("TEMPO: " + decorrido.ToString().PadRight(10, ' ') + "|" + "".ToString().PadRight(7, ' '));
                    LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                    LogEstatico.setLogTexto("PLACA: " + this.carro.Placa);
                }
                catch (Exception ex)
                {
                    LogEstatico.setLogTitulo("ERRO LOG CRONOMETRO: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                    LogEstatico.setLogTexto("MENSAGEM: " + ex.Message + ex.StackTrace);
                    this.binEstadual.ErroBinNacional = new Erros("0", "BIN ESTADUAL: TIMEOUT: Falha ao acessar fornecedor: " + ex.ToString());
                    return this.binEstadual;
                }

                if (String.IsNullOrEmpty(this.retornoWs))
                {
                    this.binEstadual.ErroBinNacional = new Inteligencia.Erros("0", "CONSULTAUTO: CONSULTA INDISPONIVEL");
                    LogEstatico.setLogTitulo("ERRO BIN ESTADUAL CONSULTAUTO" + System.DateTime.Now);
                    LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                    LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                    return this.binEstadual;
                }

                this.retornoWs = this.retornoWs.Trim();

                XmlDocument arrayResposta = new XmlDocument();
                arrayResposta.LoadXml(this.retornoWs);

                // PLACA NÃO ENCONTRADOS NA BASES CONSULTADAS
                if (this.retornoWs.Contains("PLACA NAO CADASTRADA"))
                {
                    this.binEstadual.MsgBinNacional = new Erros("0", "PLACA: INFORMACAO NAO ENCONTRADA NAS BASES CONSULTADAS");
                    return this.binEstadual;
                }

                // FALHA NA CONSULTA
                if (this.retornoWs.Contains("ERROR"))
                {
                    this.binEstadual.ErroBinNacional = new Erros("0", arrayResposta.SelectNodes("/RETORNO/MENSAGEM").Item(0).InnerText.Replace("ERROR:", ""));
                    return this.binEstadual;
                }
                // REGISTROS NÃO ENCONTRADOS NA BASES CONSULTADAS
                if (this.retornoWs.Contains("NENHUM REGISTRO ENCONTRADO"))
                {
                    this.binEstadual.MsgBinNacional = new Erros("0", "INFORMACAO NAO ENCONTRADA NAS BASES CONSULTADAS");
                    return this.binEstadual;
                }
                // PARAMETRO INCORRETO
                if (this.retornoWs.Contains("PARAMETRO PLACA INFORMADO INCORRETAMENTE"))
                {
                    this.binEstadual.MsgBinNacional = new Erros("2", "PARAMETROS: PLACA INFORMADA INCORRETAMENTE");
                    return this.binEstadual;
                }
                // SISTEMA INDISPONIVEL
                if (this.retornoWs.Contains("INDISPONIVEL"))
                {
                    this.binEstadual.MsgBinNacional = new Erros("3", "SISTEMA INDISPONIVEL TEMPORARIAMENTE");
                    return this.binEstadual;
                }
                // SISTEMA INDISPONIVEL
                if (this.retornoWs.Contains("PROBLEMAS NA EXECUCAO"))
                {
                    this.binEstadual.MsgBinNacional = new Erros("3", "SISTEMA INDISPONIVEL TEMPORARIAMENTE");
                    return this.binEstadual;
                }
                // PARAMETROS INDISPONIVEIS
                if (arrayResposta.SelectNodes("/RETORNO/MENSAGEM").Count > 0)
                {
                    this.binEstadual.MsgBinNacional = new Erros("4", "CONSULTA REALIZADA POR 'RENAVAM OU MOTOR' ESTÁ DESATIVADA TEMPORARIAMENTE POR NOSSOS FORNECEDORES");
                    return this.binEstadual;
                }

                try
                {
                    this.binEstadual.CodFornecedor = "_" + arrayResposta.SelectNodes("/RESPOSTA/RETORNO/CHAVERETORNO").Item(0).InnerText.PadLeft(10, '0');
                }
                catch
                {
                    this.binEstadual.CodFornecedor = "_ERRO";

                    if (!this.retornoWs.Contains("NENHUM REGISTRO ENCONTRADO PARA O DADO INFORMADO"))
                    {
                        LogEstatico.setLogTitulo("CONSULTAUTO BIN NACIONAL INDISPONIVEL: " + System.DateTime.Now);
                        LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                        LogEstatico.setLogTexto("RETORNO: " + this.retornoWs);
                        LogEstatico.setLogTexto("PARAMETROS: LOGON: " + this.dadosUsuario.Logon + "; PLACA: " + this.carro.Placa);
                    }
                    this.binEstadual.ErroBinNacional = new Erros("0", "CONSULTAUTO BIN ESTADUAL INDISPONIVEL");
                    return this.binEstadual;
                }

                // INFORMACAO ENCONTRADA
                this.binEstadual.MsgBinNacional = new Erros("1", "CONSTA REGISTRO ESTADUAL");
                this.binEstadual.Automovel = new Veiculo();

                this.binEstadual.SituacaoVeiculo = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/SITUACAOVEICULO").Item(0).InnerText;
                this.binEstadual.DocFaturado = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/CPFFATURADO").Item(0).InnerText.Trim();
                this.binEstadual.TipoDocFaturado = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/TIPODOCUMENTOFATURADO").Item(0).InnerText;
                this.binEstadual.TipoTransacao = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/TIPOTRANSACAO").Item(0).InnerText;
                this.binEstadual.DtUltimaAtualizacao = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/ULTIMAATUALIZACAO").Item(0).InnerText;
                // DADOS DE FINANCIAMENTO
                this.binEstadual.RestFinanciamento = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/RESTRICAOFINAN").Item(0).InnerText;
                this.binEstadual.Financeira = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/NOMEAGENTEFINAN").Item(0).InnerText;
                this.binEstadual.NrContrato = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/NUMEROCONTRATO").Item(0).InnerText;
                this.binEstadual.DtVigenciaContrato = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/DATAVIGENCIACONTRATO").Item(0).InnerText;
                this.binEstadual.AgenteFinanceiro = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/AGENTEFINANCEIRO").Item(0).InnerText;
                this.binEstadual.NomeFinanciado = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/NOMEFINANCIADO").Item(0).InnerText;
                this.binEstadual.DocFinanciado = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/CGCFINAN").Item(0).InnerText;
                this.binEstadual.DtIntencaoTroca = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/DATAINCLUSAOINTENCAOTROCA").Item(0).InnerText;
                this.binEstadual.DtFinAtualizacao = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/DATAATUALIZACAO").Item(0).InnerText;
                // DADOS DE DEBITOS/MULTAS
                this.binEstadual.ConstaIPVALicenciamento = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/DEBSIPVALICENCIAMENTO").Item(0).InnerText;
                this.binEstadual.ConstaMultas = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/DEBSMULTA").Item(0).InnerText;
                this.binEstadual.TotalIPVA = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/VALORTOTALDEBIPVA").Item(0).InnerText;
                this.binEstadual.TotalMultas = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/VALORTOTALMULTAS").Item(0).InnerText;
                this.binEstadual.TotalLicenciamento = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/VALORTOTALDEBLIC").Item(0).InnerText;
                this.binEstadual.TotalDpvat = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/VALORTOTALDPVAT").Item(0).InnerText;
                
                this.binEstadual.ListaRestricao = new List<string>();

                for (int x = 1; x < 5; x++)
                {
                    string aux = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/RESTRICAO" + x.ToString()).Item(0).InnerText;
                    if (aux.Trim() != string.Empty && aux != "NADA CONSTA")
                        this.binEstadual.ListaRestricao.Add(aux);
                }

                this.binEstadual.Automovel.AnoModelo = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/VEIANOMODELO").Item(0).InnerText;
                this.binEstadual.Automovel.AnoFabrica = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/VEIANOFABR").Item(0).InnerText;
                this.binEstadual.Automovel.Chassi = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/CHASSI").Item(0).InnerText.Trim();
                this.binEstadual.Automovel.CapacidadeMaximaTracao = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/CMT").Item(0).InnerText;
                this.binEstadual.Automovel.CapacidadeCarga = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/CAPACIDADECARGA").Item(0).InnerText;
                this.binEstadual.Automovel.Cilindradas = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/CILINDRADA").Item(0).InnerText;
                this.binEstadual.Automovel.Cor = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/COR").Item(0).InnerText;
                this.binEstadual.Automovel.Categoria = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/CATEGORIA").Item(0).InnerText;
                this.binEstadual.Automovel.Combustivel = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/COMBUSTIVEL").Item(0).InnerText;
                this.binEstadual.Automovel.CapacidadePassageiros = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/CAPPASS").Item(0).InnerText;
                this.binEstadual.Automovel.Especie = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/ESPECIE").Item(0).InnerText;
                this.binEstadual.Automovel.MunicipioEmplacamento = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/MUNICIPIO").Item(0).InnerText;
                this.binEstadual.Automovel.Modelo = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/MODELO").Item(0).InnerText;
                this.binEstadual.Automovel.NrMotor = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/MOTOR").Item(0).InnerText;
                this.binEstadual.Automovel.Nacionalidade = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/VEIPROCEDENCIA").Item(0).InnerText;
                this.binEstadual.Automovel.NrCambio = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/NUMERO_CAIXACAMBIO").Item(0).InnerText;
                this.binEstadual.Automovel.Placa = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/PLACA").Item(0).InnerText;
                this.binEstadual.Automovel.PesoBruto = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/PBT").Item(0).InnerText;
                this.binEstadual.Automovel.Potencia = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/POTENCIA").Item(0).InnerText;
                this.binEstadual.Automovel.QtdEixos = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/EIXOS").Item(0).InnerText;
                this.binEstadual.Automovel.Renavam = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/RENAVAM").Item(0).InnerText;
                this.binEstadual.Automovel.TipoChassi = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/TIPOREMARCACAOCHASSI").Item(0).InnerText;
                this.binEstadual.Automovel.Tipo = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/TIPO").Item(0).InnerText;
                this.binEstadual.Automovel.TipoMontagem = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/TIPOMONTAGEM").Item(0).InnerText;
                this.binEstadual.Automovel.TipoCarroceria = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/CARROCERIA").Item(0).InnerText;
                this.binEstadual.Automovel.Uf = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/UF").Item(0).InnerText;

                if (arrayResposta.SelectNodes("/RESPOSTA/RETORNO/NOMEPROPRIETARIO").Item(0).InnerText.Length > 0)
                {
                    this.binEstadual.Automovel.Proprietario = new Pessoa();
                    this.binEstadual.Automovel.Proprietario.Nome = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/NOMEPROPRIETARIO").Item(0).InnerText;
                    this.binEstadual.Automovel.Proprietario.Documento = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/CPF_CNPJ_PROPRIETARIO").Item(0).InnerText;

                    if (this.binEstadual.Automovel.Proprietario.Documento.Length == 11)
                        this.binEstadual.Automovel.Proprietario.Tipo = "PESSOA FISICA";
                    else if (this.binEstadual.Automovel.Proprietario.Documento.Length > 11)
                        this.binEstadual.Automovel.Proprietario.Tipo = "PESSOA JURIDICA";
                }

                // ANULA VALORES EM BRANCO
                this.binEstadual.Automovel = Util.unSetDadosVazios<Veiculo>(this.binEstadual.Automovel);
                binEstadual = Util.unSetDadosVazios<BinEstadualModel>(binEstadual);

                return this.binEstadual;
            }
            catch (Exception ex)
            {
                EnvioEmail SendMail = new EnvioEmail();
                SendMail.EnviaEmail(SendMail.LayoutFornecedor("CONSULTAUTO", "BINNACIONAL", this.carro.Placa + this.carro.Chassi), "CONSULTA INDISPONIVEL", "antonio.carlos@emepar.com.br");

                this.binEstadual.ErroBinNacional = new Erros("0", "BIN NACIONAL: Falha ao acessar fornecedor: " + ex.ToString());

                if (ex.Message.Contains("timed out"))
                {
                    try
                    {
                        LogEstatico.setLogTitulo("TEMPO BIN NACIONAL: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                        LogEstatico.setLogTexto("CHAVE: " + this.logLancamento);
                        LogEstatico.setLogTexto("FORNECEDOR: CONSULTAUTO");
                        LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                        LogEstatico.setLogTexto("TIMEOUT");
                        LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                        LogEstatico.setLogTexto("PLACA: " + this.carro.Placa);
                    }
                    catch (Exception e)
                    {
                        LogEstatico.setLogTitulo("ERRO LOG CRONOMETRO: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                        LogEstatico.setLogTexto("MENSAGEM: " + e.Message + e.StackTrace);
                    }
                }

                LogEstatico.setLogTitulo("ERRO CONSULTAUTO BINNACIONAL: " + System.DateTime.Now);
                LogEstatico.setLogTexto(ex.Message + ex.StackTrace);
                LogEstatico.setLogTexto("PARAMETROS: LOGON : " + this.dadosUsuario.Logon + "; PLACA: " + this.carro.Placa);
            }

            return this.binEstadual;
        }

        // BIN NACIONAL RESPOSTA
        public BinModel getBinNacional()
        {
            try
            {
                if (this.carro.Renavam != null && this.carro.Renavam != "")
                    setUrlRequisicao("BNR");
                else
                    setUrlRequisicao("BN");

                this.binNacional = new BinModel();
                // FAZ A CONSULTA NO FORNECEDOR
                
                var request = new CustomTimeOut();
                var relogio = Stopwatch.StartNew();
                // this.retornoWs = request.DownloadString(this.urlRequisicao);
                var decorrido = relogio.ElapsedMilliseconds;
                
                // RETORNO PADRÃO DO FORNECEDOR
                this.retornoWs = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><RESPOSTA><BINXML><SITUACAOVEICULO> CIRCULACAO</SITUACAOVEICULO><OCORRENCIA>VEICULO NAO INDICA OCORRENCIA DE ROUBO/FURTO</OCORRENCIA><PLACA>DKC2274</PLACA><MUNICIPIO>SAO PAULO</MUNICIPIO><UF>SP</UF><RENAVAM>00734935698</RENAVAM><PROPRIETARIO>25421387889</PROPRIETARIO><DATAULTIMAATUALIZACAO>2015-05-08</DATAULTIMAATUALIZACAO><CHASSI>9BFZF26P828161874</CHASSI><SITUACAOCHASSI>NORMAL</SITUACAOCHASSI><MARCAMODELO>FORD/FIESTA SEDAN1.6FLEX</MARCAMODELO><ANOFABRICACAO>2007</ANOFABRICACAO><ANOMODELO>2008</ANOMODELO><TIPOVEICULO>AUTOMOVEL</TIPOVEICULO><TIPOMONTAGEM>COMPLETA</TIPOMONTAGEM><TIPOCARROCERIA>INEXISTENTE</TIPOCARROCERIA><COR>PRETA</COR><ESPECIE>PASSAGEIRO</ESPECIE><COMBUSTIVEL>ALCOOL/GASOLINA</COMBUSTIVEL><POTENCIA>111</POTENCIA><CILINDRADA>1598</CILINDRADA><CAPACIDADECARGA>0,00</CAPACIDADECARGA><PROCEDENCIA>NACIONAL</PROCEDENCIA><CAPACIDADEPASSAGEIRO>5</CAPACIDADEPASSAGEIRO><NUMEROMOTOR>QFJA88161916</NUMEROMOTOR><NUMEROCAIXACAMBIO></NUMEROCAIXACAMBIO><NUMEROCARROCERIA></NUMEROCARROCERIA><NUMEROEIXOTRASEIRO></NUMEROEIXOTRASEIRO><NUMEROTERCEIROEIXO></NUMEROTERCEIROEIXO><CMT>0</CMT><PBT>1,55</PBT><EIXOS>2</EIXOS><TIPODOCUMENTOFATURADO>JURIDICA</TIPODOCUMENTOFATURADO><CPFCNPJFATURADO>04878806000138</CPFCNPJFATURADO><UFFATURADO>SP</UFFATURADO><TIPODOCUMENTOIMPORTADORA></TIPODOCUMENTOIMPORTADORA><RESTRICAO1>ALIENACAO FIDUCIARIA</RESTRICAO1><RESTRICAO2>NADA CONSTA</RESTRICAO2><RESTRICAO3>NADA CONSTA</RESTRICAO3><RESTRICAO4>NADA CONSTA</RESTRICAO4><INDICADORCOMUNICACAODEVENDAS>NAO</INDICADORCOMUNICACAODEVENDAS><INDICADORRESTRICAORENAJUD>NAO</INDICADORRESTRICAORENAJUD><NUMERO_DI>0000000000</NUMERO_DI><CODIGORETORNOPESQUISA>16020849</CODIGORETORNOPESQUISA><DATALIMITERESTRICAOTRIBUTARIA></DATALIMITERESTRICAOTRIBUTARIA></BINXML></RESPOSTA>";
                
                // GRAVA O TEMPO DECORRIDO EM ARQUIVO DE LOG
                try
                {
                    LogEstatico.setLogTitulo("TEMPO BIN NACIONAL: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                    LogEstatico.setLogTexto("CHAVE: " + this.logLancamento);
                    LogEstatico.setLogTexto("FORNECEDOR: CONSULTAUTO");
                    LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                    LogEstatico.setLogTexto("TEMPO: " + decorrido.ToString().PadRight(10, ' ') + "|" + "".ToString().PadRight(7, ' '));
                    LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                    LogEstatico.setLogTexto("PLACA: " + this.carro.Placa);
                }
                catch (Exception ex)
                {
                    LogEstatico.setLogTitulo("ERRO LOG CRONOMETRO: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                    LogEstatico.setLogTexto("MENSAGEM: " + ex.Message + ex.StackTrace);
                    this.binNacional.ErroBinNacional = new Erros("0", "BIN NACIONAL: TIMEOUT: Falha ao acessar fornecedor: " + ex.ToString());
                    return this.binNacional;
                }

                if (String.IsNullOrEmpty(this.retornoWs))
                {
                    this.binNacional.ErroBinNacional = new Inteligencia.Erros("0", "CONSULTAUTO: CONSULTA INDISPONIVEL");
                    LogEstatico.setLogTitulo("ERRO BIN NACIONAL CONSULTAUTO" + System.DateTime.Now);
                    LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                    LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                    return this.binNacional;
                }
                
                this.retornoWs = this.retornoWs.Trim();

                XmlDocument arrayResposta = new XmlDocument();
                arrayResposta.LoadXml(this.retornoWs);
                
                // RENAVAM NÃO ENCONTRADOS NA BASES CONSULTADAS
                if (this.retornoWs.Contains("RENAVAM NAO LOCALIZADO NAS BASES CONSULTADAS"))
                {
                    this.binNacional.MsgBinNacional = new Erros("0", "RENAVAM: INFORMACAO NAO ENCONTRADA NAS BASES CONSULTADAS");
                    return this.binNacional;
                }

                // FALHA NA CONSULTA
                if (this.retornoWs.Contains("ERROR"))
                {
                    this.binNacional.ErroBinNacional = new Erros("0", arrayResposta.SelectNodes("/RETORNO/MENSAGEM").Item(0).InnerText.Replace("ERROR:", ""));
                    return this.binNacional;
                }
                // REGISTROS NÃO ENCONTRADOS NA BASES CONSULTADAS
                if (this.retornoWs.Contains("NENHUM REGISTRO ENCONTRADO"))
                {
                    this.binNacional.MsgBinNacional = new Erros("0", "INFORMACAO NAO ENCONTRADA NAS BASES CONSULTADAS");
                    return this.binNacional;
                }
                // PARAMETRO INCORRETO
                if (this.retornoWs.Contains("PARAMETRO PLACA INFORMADO INCORRETAMENTE"))
                {
                    this.binNacional.MsgBinNacional = new Erros("2", "PARAMETROS: PLACA INFORMADA INCORRETAMENTE");
                    return this.binNacional;
                }
                // SISTEMA INDISPONIVEL
                if (this.retornoWs.Contains("INDISPONIVEL"))
                {
                    this.binNacional.MsgBinNacional = new Erros("3", "SISTEMA INDISPONIVEL TEMPORARIAMENTE");
                    return this.binNacional;
                }
                // SISTEMA INDISPONIVEL
                if (this.retornoWs.Contains("PROBLEMAS NA EXECUCAO"))
                {
                    this.binNacional.MsgBinNacional = new Erros("3", "SISTEMA INDISPONIVEL TEMPORARIAMENTE");
                    return this.binNacional;
                }
                // PARAMETROS INDISPONIVEIS
                if (arrayResposta.SelectNodes("/RETORNO/MENSAGEM").Count > 0)
                {
                    this.binNacional.MsgBinNacional = new Erros("4", "CONSULTA REALIZADA POR 'RENAVAM OU MOTOR' ESTÁ DESATIVADA TEMPORARIAMENTE POR NOSSOS FORNECEDORES");
                    return this.binNacional;
                }

                try
                {
                    this.binNacional.CodFornecedor = "_" + arrayResposta.SelectNodes("/RESPOSTA/BINXML/CODIGORETORNOPESQUISA").Item(0).InnerText.PadLeft(10, '0');
                }
                catch
                {
                    this.binNacional.CodFornecedor = "_ERRO";

                    if (!this.retornoWs.Contains("NENHUM REGISTRO ENCONTRADO PARA O DADO INFORMADO"))
                    {
                        LogEstatico.setLogTitulo("CONSULTAUTO BIN NACIONAL INDISPONIVEL: " + System.DateTime.Now);
                        LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                        LogEstatico.setLogTexto("RETORNO: " + this.retornoWs);
                        LogEstatico.setLogTexto("PARAMETROS: LOGON: " + this.dadosUsuario.Logon + "; PLACA: " + this.carro.Placa);
                    }
                    this.binNacional.ErroBinNacional = new Erros("0", "CONSULTAUTO BIN NACIONAL INDISPONIVEL");
                    return this.binNacional;
                }

                // INFORMACAO ENCONTRADA
                this.binNacional.MsgBinNacional = new Erros("1", "CONSTA REGISTRO FEDERAL");
                string tempData = string.Empty;
                string[] tempDigitos;

                this.binNacional.Automovel = new Veiculo();

                this.binNacional.SituacaoVeiculo = arrayResposta.SelectNodes("/RESPOSTA/BINXML/SITUACAOVEICULO").Item(0).InnerText;
                this.binNacional.SituacaoChassi = arrayResposta.SelectNodes("/RESPOSTA/BINXML/SITUACAOCHASSI").Item(0).InnerText;
                this.binNacional.Ocorrencia = arrayResposta.SelectNodes("/RESPOSTA/BINXML/OCORRENCIA").Item(0).InnerText.Trim();
                this.binNacional.DocFaturado = arrayResposta.SelectNodes("/RESPOSTA/BINXML/CPFCNPJFATURADO").Item(0).InnerText.Trim();
                this.binNacional.TipoDocFaturado = arrayResposta.SelectNodes("/RESPOSTA/BINXML/TIPODOCUMENTOFATURADO").Item(0).InnerText;
                this.binNacional.TipoDocImportadora = arrayResposta.SelectNodes("/RESPOSTA/BINXML/TIPODOCUMENTOIMPORTADORA").Item(0).InnerText;
                this.binNacional.UFFaturado = arrayResposta.SelectNodes("/RESPOSTA/BINXML/UFFATURADO").Item(0).InnerText;
                this.binNacional.IndicaRestricaoRenajud = arrayResposta.SelectNodes("/RESPOSTA/BINXML/INDICADORRESTRICAORENAJUD").Item(0).InnerText;

                tempData = arrayResposta.SelectNodes("/RESPOSTA/BINXML/DATALIMITERESTRICAOTRIBUTARIA").Item(0).InnerText;
                
                if (tempData.Contains("-"))
                {
                    tempDigitos = tempData.Split('-');
                    tempData = tempDigitos[2] + "/" + tempDigitos[1] + "/" + tempDigitos[0];
                }
                
                this.binNacional.DtLimiteRestricaoTributaria = tempData;

                tempData = arrayResposta.SelectNodes("/RESPOSTA/BINXML/DATAULTIMAATUALIZACAO").Item(0).InnerText;

                if (tempData.Contains("-"))
                {
                    tempDigitos = tempData.Split('-');
                    tempData = tempDigitos[2] + "/" + tempDigitos[1] + "/" + tempDigitos[0];
                }

                this.binNacional.DtUltimaAtualizacao = tempData;
                this.binNacional.ListaRestricao = new List<string>();

                for (int x = 1; x < 5; x++)
                {
                    string aux = arrayResposta.SelectNodes("/RESPOSTA/BINXML/RESTRICAO" + x.ToString()).Item(0).InnerText;
                    if (aux.Trim() != string.Empty && aux != "NADA CONSTA")
                        this.binNacional.ListaRestricao.Add(aux);
                }
               
                this.binNacional.Automovel.Chassi = arrayResposta.SelectNodes("/RESPOSTA/BINXML/CHASSI").Item(0).InnerText.Trim();
                this.binNacional.Automovel.Renavam = arrayResposta.SelectNodes("/RESPOSTA/BINXML/RENAVAM").Item(0).InnerText;
                this.binNacional.Automovel.Placa = arrayResposta.SelectNodes("/RESPOSTA/BINXML/PLACA").Item(0).InnerText;
                this.binNacional.Automovel.Uf = arrayResposta.SelectNodes("/RESPOSTA/BINXML/UF").Item(0).InnerText;
                this.binNacional.Automovel.MunicipioEmplacamento = arrayResposta.SelectNodes("/RESPOSTA/BINXML/MUNICIPIO").Item(0).InnerText;
                this.binNacional.Automovel.Modelo = arrayResposta.SelectNodes("/RESPOSTA/BINXML/MARCAMODELO").Item(0).InnerText;
                this.binNacional.Automovel.Cor = arrayResposta.SelectNodes("/RESPOSTA/BINXML/COR").Item(0).InnerText;
                this.binNacional.Automovel.Tipo = arrayResposta.SelectNodes("/RESPOSTA/BINXML/TIPOVEICULO").Item(0).InnerText;
                this.binNacional.Automovel.Especie = arrayResposta.SelectNodes("/RESPOSTA/BINXML/ESPECIE").Item(0).InnerText;
                this.binNacional.Automovel.Combustivel = arrayResposta.SelectNodes("/RESPOSTA/BINXML/COMBUSTIVEL").Item(0).InnerText;
                this.binNacional.Automovel.CapacidadePassageiros = arrayResposta.SelectNodes("/RESPOSTA/BINXML/CAPACIDADEPASSAGEIRO").Item(0).InnerText;
                this.binNacional.Automovel.NrMotor = arrayResposta.SelectNodes("/RESPOSTA/BINXML/NUMEROMOTOR").Item(0).InnerText;
                this.binNacional.Automovel.AnoModelo = arrayResposta.SelectNodes("/RESPOSTA/BINXML/ANOMODELO").Item(0).InnerText;
                this.binNacional.Automovel.AnoFabrica = arrayResposta.SelectNodes("/RESPOSTA/BINXML/ANOFABRICACAO").Item(0).InnerText;
                this.binNacional.Automovel.TipoMontagem = arrayResposta.SelectNodes("/RESPOSTA/BINXML/TIPOMONTAGEM").Item(0).InnerText;
                this.binNacional.Automovel.Potencia = arrayResposta.SelectNodes("/RESPOSTA/BINXML/POTENCIA").Item(0).InnerText;
                this.binNacional.Automovel.Cilindradas = arrayResposta.SelectNodes("/RESPOSTA/BINXML/CILINDRADA").Item(0).InnerText;
                this.binNacional.Automovel.NrCarroceria = arrayResposta.SelectNodes("/RESPOSTA/BINXML/NUMEROCARROCERIA").Item(0).InnerText;
                this.binNacional.Automovel.Nacionalidade = arrayResposta.SelectNodes("/RESPOSTA/BINXML/PROCEDENCIA").Item(0).InnerText;
                this.binNacional.Automovel.CapacidadeMaximaTracao = arrayResposta.SelectNodes("/RESPOSTA/BINXML/CMT").Item(0).InnerText;
                this.binNacional.Automovel.CapacidadeCarga = arrayResposta.SelectNodes("/RESPOSTA/BINXML/CAPACIDADECARGA").Item(0).InnerText;
                this.binNacional.Automovel.NrCambio = arrayResposta.SelectNodes("/RESPOSTA/BINXML/NUMEROCAIXACAMBIO").Item(0).InnerText;
                this.binNacional.Automovel.NrEixoTraseiro = arrayResposta.SelectNodes("/RESPOSTA/BINXML/NUMEROEIXOTRASEIRO").Item(0).InnerText;
                this.binNacional.Automovel.PesoBruto = arrayResposta.SelectNodes("/RESPOSTA/BINXML/PBT").Item(0).InnerText;
                this.binNacional.Automovel.QtdEixos = arrayResposta.SelectNodes("/RESPOSTA/BINXML/EIXOS").Item(0).InnerText;
                this.binNacional.Automovel.TipoCarroceria = arrayResposta.SelectNodes("/RESPOSTA/BINXML/TIPOCARROCERIA").Item(0).InnerText;
                
                // DOC PROPRIETARIO
                if (arrayResposta.SelectNodes("/RESPOSTA/BINXML/PROPRIETARIO").Item(0).InnerText.Length > 0)
                {
                    this.binNacional.Automovel.Proprietario = new Pessoa();
                    this.binNacional.Automovel.Proprietario.Documento = arrayResposta.SelectNodes("/RESPOSTA/BINXML/PROPRIETARIO").Item(0).InnerText;

                    if (this.binNacional.Automovel.Proprietario.Documento.Length == 11)
                    {
                        this.binNacional.Automovel.Proprietario.Tipo = "PESSOA FISICA";
                    }
                    else if (this.binNacional.Automovel.Proprietario.Documento.Length > 11)
                    {
                        this.binNacional.Automovel.Proprietario.Tipo = "PESSOA JURIDICA";
                    }
                }

                // ANULA VALORES EM BRANCO
                this.binNacional.Automovel = Util.unSetDadosVazios<Veiculo>(this.binNacional.Automovel);
                binNacional = Util.unSetDadosVazios<BinModel>(binNacional);

                return this.binNacional;
            }
            catch (Exception ex)
            {
                EnvioEmail SendMail = new EnvioEmail();
                SendMail.EnviaEmail(SendMail.LayoutFornecedor("CONSULTAUTO", "BINNACIONAL", this.carro.Placa + this.carro.Chassi), "CONSULTA INDISPONIVEL", "antonio.carlos@emepar.com.br");

                this.binNacional.ErroBinNacional = new Erros("0", "BIN NACIONAL: Falha ao acessar fornecedor: " + ex.ToString());
                
                if (ex.Message.Contains("timed out"))
                {
                    try
                    {
                        LogEstatico.setLogTitulo("TEMPO BIN NACIONAL: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                        LogEstatico.setLogTexto("CHAVE: " + this.logLancamento);
                        LogEstatico.setLogTexto("FORNECEDOR: CONSULTAUTO");
                        LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                        LogEstatico.setLogTexto("TIMEOUT");
                        LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                        LogEstatico.setLogTexto("PLACA: " + this.carro.Placa);
                    }
                    catch (Exception e)
                    {
                        LogEstatico.setLogTitulo("ERRO LOG CRONOMETRO: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                        LogEstatico.setLogTexto("MENSAGEM: " + e.Message + e.StackTrace);
                    }

                }

                LogEstatico.setLogTitulo("ERRO CONSULTAUTO BINNACIONAL: " + System.DateTime.Now);
                LogEstatico.setLogTexto(ex.Message + ex.StackTrace);
                LogEstatico.setLogTexto("PARAMETROS: LOGON : " + this.dadosUsuario.Logon + "; PLACA: " + this.carro.Placa);
            }

            return this.binNacional;
        }

        // BIN ROUBO FURTO RESPOSTA
        public BinRouboFurtoModel getBinRouboFurto()
        {
            try
            {
                setUrlRequisicao("BNRF");

                this.binRouboFurto = new BinRouboFurtoModel();
                // FAZ A CONSULTA NO FORNECEDOR
                var request = new CustomTimeOut();
                var relogio = Stopwatch.StartNew();
                this.retornoWs = request.DownloadString(this.urlRequisicao);
                var decorrido = relogio.ElapsedMilliseconds;
                // RETORNO PADRÃO DO FORNECEDOR
                // this.retornoWs = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><RESPOSTA><BINXML><SITUACAOVEICULO> CIRCULACAO</SITUACAOVEICULO><OCORRENCIA>VEICULO COM OCORRENCIA DE ROUBO/FURTO</OCORRENCIA><PLACA>ARN1547</PLACA><MUNICIPIO>SAO JOSE DOS PINHAIS</MUNICIPIO><UF>PR</UF><RENAVAM>00153644958</RENAVAM><PROPRIETARIO></PROPRIETARIO><DATAULTIMAATUALIZACAO>2015-03-17</DATAULTIMAATUALIZACAO><CHASSI>9BD25504998855598</CHASSI><SITUACAOCHASSI>NORMAL</SITUACAOCHASSI><MARCAMODELO>FIAT/FIORINO FLEX</MARCAMODELO><ANOFABRICACAO>2009</ANOFABRICACAO><ANOMODELO>2009</ANOMODELO><TIPOVEICULO>CAMINHONETE</TIPOVEICULO><TIPOMONTAGEM>COMPLETA</TIPOMONTAGEM><TIPOCARROCERIA>FURGAO</TIPOCARROCERIA><COR>BRANCA</COR><ESPECIE>CARGA</ESPECIE><COMBUSTIVEL>ALCOOL/GASOLINA</COMBUSTIVEL><POTENCIA>71</POTENCIA><CILINDRADA>1300</CILINDRADA><CAPACIDADECARGA>0</CAPACIDADECARGA><PROCEDENCIA>NACIONAL</PROCEDENCIA><CAPACIDADEPASSAGEIRO>2</CAPACIDADEPASSAGEIRO><NUMEROMOTOR>178E9011*8689135*</NUMEROMOTOR><NUMEROCAIXACAMBIO></NUMEROCAIXACAMBIO><NUMEROCARROCERIA>76559046</NUMEROCARROCERIA><NUMEROEIXOTRASEIRO></NUMEROEIXOTRASEIRO><NUMEROTERCEIROEIXO></NUMEROTERCEIROEIXO><CMT>242</CMT><PBT>0</PBT><EIXOS>2</EIXOS><TIPODOCUMENTOFATURADO>JURIDICA</TIPODOCUMENTOFATURADO><CPFCNPJFATURADO>79.763.884/0002-77</CPFCNPJFATURADO><UFFATURADO>SC</UFFATURADO><TIPODOCUMENTOIMPORTADORA></TIPODOCUMENTOIMPORTADORA><RESTRICAO1>VEICULO COM OCORRENCIA DE FURTO/ROUBO</RESTRICAO1><RESTRICAO2>NADA CONSTA</RESTRICAO2><RESTRICAO3>ALIENACAO FIDUCIARIA</RESTRICAO3><RESTRICAO4>NADA CONSTA</RESTRICAO4><INDICADORCOMUNICACAODEVENDAS>NAO</INDICADORCOMUNICACAODEVENDAS><INDICADORRESTRICAORENAJUD>NAO</INDICADORRESTRICAORENAJUD><NUMERO_DI>0000000000</NUMERO_DI><CODIGORETORNOPESQUISA>16021941</CODIGORETORNOPESQUISA><DATALIMITERESTRICAOTRIBUTARIA></DATALIMITERESTRICAOTRIBUTARIA></BINXML><HISTORICOROUBOEFURTO><EXISTE_ERRO>0</EXISTE_ERRO><MSG_ERRO></MSG_ERRO></HISTORICOROUBOEFURTO><RF><CATEGORIA_OCORRENCIA>DECLARACAO  DE ROUBO</CATEGORIA_OCORRENCIA><ANO>2015</ANO><ORGAO_SEGURANCA>0001</ORGAO_SEGURANCA><BOLETIM>2008602</BOLETIM><NUMERO_OCORRENCIA></NUMERO_OCORRENCIA><TIPO_DECLARACAO></TIPO_DECLARACAO><DATA_OCORRENCIA>27/11/2015</DATA_OCORRENCIA><ALARME></ALARME></RF><INDICADOR><HOUVEDECLARACAODEROUBOEFURTO>SIM</HOUVEDECLARACAODEROUBOEFURTO><HOUVERECUPERACAODEROUBOEFURTO>NAO</HOUVERECUPERACAODEROUBOEFURTO><HOUVEDEVOLUCAODEROUBOEFURTO>NAO</HOUVEDEVOLUCAODEROUBOEFURTO></INDICADOR></RESPOSTA>";
                
                // GRAVA O TEMPO DECORRIDO EM ARQUIVO DE LOG
                try
                {
                    LogEstatico.setLogTitulo("TEMPO BIN ROUBO E FURTO: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                    LogEstatico.setLogTexto("CHAVE: " + this.logLancamento);
                    LogEstatico.setLogTexto("FORNECEDOR: CONSULTAUTO");
                    LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                    LogEstatico.setLogTexto("TEMPO: " + decorrido.ToString().PadRight(10, ' ') + "|" + "".ToString().PadRight(7, ' '));
                    LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                    LogEstatico.setLogTexto("PLACA: " + this.carro.Placa);
                }
                catch (Exception ex)
                {
                    LogEstatico.setLogTitulo("ERRO LOG CRONOMETRO: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                    LogEstatico.setLogTexto("MENSAGEM: " + ex.Message + ex.StackTrace);
                    this.binRouboFurto.ErroBinNacional = new Erros("0", "BIN ROUBO E FURTO: TIMEOUT: Falha ao acessar fornecedor: " + ex.ToString());
                    return this.binRouboFurto;
                }

                if (String.IsNullOrEmpty(this.retornoWs))
                {
                    this.binRouboFurto.ErroBinNacional = new Inteligencia.Erros("0", "CONSULTAUTO: CONSULTA INDISPONIVEL");
                    LogEstatico.setLogTitulo("ERRO BIN ROUBO E FURTO CONSULTAUTO" + System.DateTime.Now);
                    LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                    LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                    return this.binRouboFurto;
                }

                this.retornoWs = this.retornoWs.Trim();

                XmlDocument xmlResposta = new XmlDocument();
                xmlResposta.LoadXml(this.retornoWs);

                // REGISTROS NÃO ENCONTRADOS NA BASES CONSULTADAS
                if (xmlResposta.SelectNodes("/RESPOSTA/BINXML/OCORRENCIA").Item(0).InnerText == "NENHUM REGISTRO ENCONTRADO PARA O DOCUMENTO CONSULTADO.")
                {
                    this.binRouboFurto.MsgBinNacional = new Erros("0", "INFORMACAO NAO ENCONTRADA PARA O DOCUMENTO CONSULTADO");
                    return this.binRouboFurto;
                }
                // FALHA NA CONSULTA
                if (xmlResposta.SelectNodes("/RESPOSTA/HISTORICOROUBOEFURTO/EXISTE_ERRO").Item(0).InnerText.Contains("1"))
                {
                    this.binRouboFurto.ErroBinNacional = new Erros("0", xmlResposta.SelectNodes("/RESPOSTA/HISTORICOROUBOEFURTO/MSG_ERRO").Item(0).InnerText.Replace("ERROR:", ""));
                    return this.binRouboFurto;
                }
                // PARAMETRO INCORRETO
                if (this.retornoWs.Contains("PARAMETRO PLACA INFORMADO INCORRETAMENTE"))
                {
                    this.binRouboFurto.MsgBinNacional = new Erros("2", "PARAMETROS: PLACA INFORMADA INCORRETAMENTE");
                    return this.binRouboFurto;
                }
                // SISTEMA INDISPONIVEL
                if (this.retornoWs.Contains("INDISPONIVEL"))
                {
                    this.binRouboFurto.MsgBinNacional = new Erros("3", "SISTEMA INDISPONIVEL TEMPORARIAMENTE");
                    return this.binRouboFurto;
                }
                // SISTEMA INDISPONIVEL
                if (this.retornoWs.Contains("PROBLEMAS NA EXECUCAO"))
                {
                    this.binRouboFurto.MsgBinNacional = new Erros("3", "SISTEMA INDISPONIVEL TEMPORARIAMENTE");
                    return this.binRouboFurto;
                }
                
                // INFORMACAO ENCONTRADA
                this.binRouboFurto.MsgBinNacional = new Erros("1", "CONSTA REGISTRO FEDERAL");
                string tempData = string.Empty;
                string[] tempDigitos;

                this.binRouboFurto.Automovel = new Veiculo();

                this.binRouboFurto.SituacaoVeiculo = xmlResposta.SelectNodes("/RESPOSTA/BINXML/SITUACAOVEICULO").Item(0).InnerText;
                this.binRouboFurto.Obs = xmlResposta.SelectNodes("/RESPOSTA/BINXML/OCORRENCIA").Item(0).InnerText.Trim();
                this.binRouboFurto.DocFaturado = xmlResposta.SelectNodes("/RESPOSTA/BINXML/CPFCNPJFATURADO").Item(0).InnerText.Trim();
                this.binRouboFurto.TipoDocFaturado = xmlResposta.SelectNodes("/RESPOSTA/BINXML/TIPODOCUMENTOFATURADO").Item(0).InnerText;
                this.binRouboFurto.TipoDocImportadora = xmlResposta.SelectNodes("/RESPOSTA/BINXML/TIPODOCUMENTOIMPORTADORA").Item(0).InnerText;
                this.binRouboFurto.UFFaturado = xmlResposta.SelectNodes("/RESPOSTA/BINXML/UFFATURADO").Item(0).InnerText;
                this.binRouboFurto.IndicaRestricaoRenajud = xmlResposta.SelectNodes("/RESPOSTA/BINXML/INDICADORRESTRICAORENAJUD").Item(0).InnerText;

                tempData = xmlResposta.SelectNodes("/RESPOSTA/BINXML/DATALIMITERESTRICAOTRIBUTARIA").Item(0).InnerText;

                if (tempData.Contains("-"))
                {
                    tempDigitos = tempData.Split('-');
                    tempData = tempDigitos[2] + "/" + tempDigitos[1] + "/" + tempDigitos[0];
                }

                this.binRouboFurto.DtLimiteRestricaoTributaria = tempData;

                tempData = xmlResposta.SelectNodes("/RESPOSTA/BINXML/DATAULTIMAATUALIZACAO").Item(0).InnerText;

                if (tempData.Contains("-"))
                {
                    tempDigitos = tempData.Split('-');
                    tempData = tempDigitos[2] + "/" + tempDigitos[1] + "/" + tempDigitos[0];
                }

                this.binRouboFurto.DtUltimaAtualizacao = tempData;
                this.binRouboFurto.ListaRestricao = new List<string>();

                for (int x = 1; x < 5; x++)
                {
                    string aux = xmlResposta.SelectNodes("/RESPOSTA/BINXML/RESTRICAO" + x.ToString()).Item(0).InnerText;
                    if (aux.Trim() != string.Empty && aux != "NADA CONSTA")
                        this.binRouboFurto.ListaRestricao.Add(aux);
                }

                if (this.binRouboFurto.ListaRestricao.Count <= 0)
                    this.binRouboFurto.ListaRestricao = null;

                this.binRouboFurto.Automovel.Chassi = xmlResposta.SelectNodes("/RESPOSTA/BINXML/CHASSI").Item(0).InnerText.Trim();
                this.binRouboFurto.Automovel.Renavam = xmlResposta.SelectNodes("/RESPOSTA/BINXML/RENAVAM").Item(0).InnerText;
                this.binRouboFurto.Automovel.Placa = xmlResposta.SelectNodes("/RESPOSTA/BINXML/PLACA").Item(0).InnerText;
                this.binRouboFurto.Automovel.Uf = xmlResposta.SelectNodes("/RESPOSTA/BINXML/UF").Item(0).InnerText;
                this.binRouboFurto.Automovel.MunicipioEmplacamento = xmlResposta.SelectNodes("/RESPOSTA/BINXML/MUNICIPIO").Item(0).InnerText;
                this.binRouboFurto.Automovel.Modelo = xmlResposta.SelectNodes("/RESPOSTA/BINXML/MARCAMODELO").Item(0).InnerText;
                this.binRouboFurto.Automovel.Cor = xmlResposta.SelectNodes("/RESPOSTA/BINXML/COR").Item(0).InnerText;
                this.binRouboFurto.Automovel.Tipo = xmlResposta.SelectNodes("/RESPOSTA/BINXML/TIPOVEICULO").Item(0).InnerText;
                this.binRouboFurto.Automovel.Especie = xmlResposta.SelectNodes("/RESPOSTA/BINXML/ESPECIE").Item(0).InnerText;
                this.binRouboFurto.Automovel.Combustivel = xmlResposta.SelectNodes("/RESPOSTA/BINXML/COMBUSTIVEL").Item(0).InnerText;
                this.binRouboFurto.Automovel.CapacidadePassageiros = xmlResposta.SelectNodes("/RESPOSTA/BINXML/CAPACIDADEPASSAGEIRO").Item(0).InnerText;
                this.binRouboFurto.Automovel.NrMotor = xmlResposta.SelectNodes("/RESPOSTA/BINXML/NUMEROMOTOR").Item(0).InnerText;
                this.binRouboFurto.Automovel.AnoModelo = xmlResposta.SelectNodes("/RESPOSTA/BINXML/ANOMODELO").Item(0).InnerText;
                this.binRouboFurto.Automovel.AnoFabrica = xmlResposta.SelectNodes("/RESPOSTA/BINXML/ANOFABRICACAO").Item(0).InnerText;
                this.binRouboFurto.Automovel.TipoMontagem = xmlResposta.SelectNodes("/RESPOSTA/BINXML/TIPOMONTAGEM").Item(0).InnerText;
                this.binRouboFurto.Automovel.Potencia = xmlResposta.SelectNodes("/RESPOSTA/BINXML/POTENCIA").Item(0).InnerText;
                this.binRouboFurto.Automovel.Cilindradas = xmlResposta.SelectNodes("/RESPOSTA/BINXML/CILINDRADA").Item(0).InnerText;
                this.binRouboFurto.Automovel.NrCarroceria = xmlResposta.SelectNodes("/RESPOSTA/BINXML/NUMEROCARROCERIA").Item(0).InnerText;
                this.binRouboFurto.Automovel.Nacionalidade = xmlResposta.SelectNodes("/RESPOSTA/BINXML/PROCEDENCIA").Item(0).InnerText;
                this.binRouboFurto.Automovel.CapacidadeMaximaTracao = xmlResposta.SelectNodes("/RESPOSTA/BINXML/CMT").Item(0).InnerText;
                this.binRouboFurto.Automovel.CapacidadeCarga = xmlResposta.SelectNodes("/RESPOSTA/BINXML/CAPACIDADECARGA").Item(0).InnerText;
                this.binRouboFurto.Automovel.NrCambio = xmlResposta.SelectNodes("/RESPOSTA/BINXML/NUMEROCAIXACAMBIO").Item(0).InnerText;
                this.binRouboFurto.Automovel.NrEixoTraseiro = xmlResposta.SelectNodes("/RESPOSTA/BINXML/NUMEROEIXOTRASEIRO").Item(0).InnerText;
                this.binRouboFurto.Automovel.PesoBruto = xmlResposta.SelectNodes("/RESPOSTA/BINXML/PBT").Item(0).InnerText;
                this.binRouboFurto.Automovel.QtdEixos = xmlResposta.SelectNodes("/RESPOSTA/BINXML/EIXOS").Item(0).InnerText;
                this.binRouboFurto.Automovel.TipoCarroceria = xmlResposta.SelectNodes("/RESPOSTA/BINXML/TIPOCARROCERIA").Item(0).InnerText;
                this.binRouboFurto.Automovel.TipoChassi = xmlResposta.SelectNodes("/RESPOSTA/BINXML/SITUACAOCHASSI").Item(0).InnerText;

                // ANULA VALORES EM BRANCO
                this.binRouboFurto.Automovel = Util.unSetDadosVazios<Veiculo>(this.binRouboFurto.Automovel);

                int contRF = xmlResposta.SelectNodes("/RESPOSTA/RF").Count;

                if (contRF > 0)
                {
                    this.binRouboFurto.MsgBinNacional = new Erros("1", "CONSTA HISTORICO DE ROUBO E FURTO");
                    binRouboFurto.ListaRouboFurto = new List<AuxOcorrencisRF>();

                    for (int i = 0; i < contRF; i++)
                    {
                        AuxOcorrencisRF tmpOcorrencia = new AuxOcorrencisRF();
                        
                        tmpOcorrencia.NrOcorrencia = xmlResposta.SelectNodes("/RESPOSTA/RF/NUMERO_OCORRENCIA").Item(i).InnerText;
                        tmpOcorrencia.CategOcorrencia = xmlResposta.SelectNodes("/RESPOSTA/RF/CATEGORIA_OCORRENCIA").Item(i).InnerText;
                        tmpOcorrencia.OrgaoSeguranca = xmlResposta.SelectNodes("/RESPOSTA/RF/ORGAO_SEGURANCA").Item(i).InnerText;
                        tmpOcorrencia.NrBO = xmlResposta.SelectNodes("/RESPOSTA/RF/BOLETIM").Item(i).InnerText;
                        tmpOcorrencia.AnoBO = xmlResposta.SelectNodes("/RESPOSTA/RF/ANO").Item(i).InnerText;
                        tmpOcorrencia.DtOcorrencia = xmlResposta.SelectNodes("/RESPOSTA/RF/DATA_OCORRENCIA").Item(i).InnerText;
                        
                        binRouboFurto.ListaRouboFurto.Add(tmpOcorrencia);
                    }
                }

                this.binRouboFurto = Util.unSetDadosVazios<BinRouboFurtoModel>(this.binRouboFurto);
                return this.binRouboFurto;
            }
            catch (Exception ex)
            {
                EnvioEmail SendMail = new EnvioEmail();
                SendMail.EnviaEmail(SendMail.LayoutFornecedor("CONSULTAUTO", "BIN ROUBO E FURTO", this.carro.Placa + this.carro.Chassi), "CONSULTA INDISPONIVEL", "antonio.carlos@emepar.com.br");

                this.binRouboFurto.ErroBinNacional = new Erros("0", "BIN ROUBO E FURTO: Falha ao acessar fornecedor");

                if (ex.Message.Contains("timed out"))
                {
                    try
                    {
                        LogEstatico.setLogTitulo("TEMPO BIN ROUBO E FURTO: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                        LogEstatico.setLogTexto("CHAVE: " + this.logLancamento);
                        LogEstatico.setLogTexto("FORNECEDOR: CONSULTAUTO");
                        LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                        LogEstatico.setLogTexto("TIMEOUT");
                        LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                        LogEstatico.setLogTexto("PLACA: " + this.carro.Placa);
                    }
                    catch (Exception e)
                    {
                        LogEstatico.setLogTitulo("ERRO LOG CRONOMETRO: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                        LogEstatico.setLogTexto("MENSAGEM: " + e.Message + e.StackTrace);
                    }

                }

                LogEstatico.setLogTitulo("ERRO CONSULTAUTO BIN ROUBO E FURTO: " + System.DateTime.Now);
                LogEstatico.setLogTexto(ex.Message + ex.StackTrace);
                LogEstatico.setLogTexto("PARAMETROS: LOGON : " + this.dadosUsuario.Logon + "; PLACA: " + this.carro.Placa);
            }

            return this.binRouboFurto;
        }
        
        // MONTA A URL PARA OBTER O XML DO FORNECEDOR
        public void setUrlRequisicao(string tpConsulta = "BN")
        {
            switch (tpConsulta)
            {
                // PROPRIETARIO
                case "PP":
                    this.urlRequisicao = "http://200.98.202.64/outrosestados.php?txtusuario=" + this.acesLogin + "&txtsenha=" + this.acesPassword + "&txtplaca=" + this.carro.Placa + "&txtchassi=" + this.carro.Chassi + "";
                break;
                // BIN ESTADUAL
                case "BE":
                    this.urlRequisicao = "http://200.98.202.64/outrosestados.php?txtusuario=" + this.acesLogin + "&txtsenha=" + this.acesPassword + "&txtfornecedor=2&txtplaca=" + this.carro.Placa + "&txtchassi=" + this.carro.Chassi + "&txtuf=" + this.carro.Uf + "";
                break;
                // BIN NACIONAL
                case "BN":
                    this.urlRequisicao = "http://200.98.202.64/bin.php/?txtusuario=" + this.acesLogin + "&txtsenha=" + this.acesPassword + "&txtfornecedor=2&txtformatosaida=3" + "&txtchassi=" + this.carro.Chassi + "&txtplaca=" + this.carro.Placa + (this.carro.NrMotor != "" ? "&txtmotor=" + this.carro.NrMotor : "");
                break;
                // BIN NACIONAL COM RENAVAM
                case "BNR":
                    this.urlRequisicao = "http://pesquisaconsultaauto.com.br/bin.php?txtusuario=" + this.acesLogin + "&txtsenha=" + this.acesPassword + "&txtfornecedor=2&txtformatosaida=3&txtrenavam=" + this.carro.Renavam;
                break;
                // BIN NACIONAL ROUBO E FURTO
                case "BNRF":
                    this.urlRequisicao = "http://pesquisaconsultaauto.com.br/bin.php?txtusuario=" + this.acesLogin + "&txtsenha=" + this.acesPassword + "&txtfornecedor=2";
                    this.urlRequisicao += !String.IsNullOrEmpty(this.carro.Placa) ? "&txtplaca=" + this.carro.Placa : string.Empty;
                    this.urlRequisicao += !String.IsNullOrEmpty(this.carro.Chassi) ? "&txtchassi=" + this.carro.Chassi : string.Empty;
                    this.urlRequisicao += "&txtformatosaida=3&HISTORICOROUBOEFURTO";

                    if (this.dadosUsuario.Logon == "605641" || this.dadosUsuario.Logon == "601952")
                        this.urlRequisicao += "&ESTADUAL";

                break;
            }
        }
    }
}