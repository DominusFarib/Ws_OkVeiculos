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
    public class FornTecnoBank
    {
        public UsuarioModel dadosUsuario = new UsuarioModel();

        public int codRetorno { get; set; }
        public string tipoConsulta { get; set; }
        public string strRequisicao { get; set; }
        public string logLancamento { get; set; }
        
        public bool flagDocProprietario { get; set; }

        private string acesLogin = string.Empty;
        private string acesPassword = string.Empty;
        private string acesToken = string.Empty;

        private PrecificadorModel precificador;
        private DecodChassiModel decodChassi;

        private Veiculo carro;
        private List<Veiculo> dadosCarro { get; set; }

        // CONSTRUTOR
        public FornTecnoBank(Veiculo carro)
        {
            this.carro = carro;
            this.acesLogin = "yago";
            this.acesPassword = "172139";
            this.acesToken = "1006";
            //DESCOMENTAR TRECHO DE COD QUANDO FOR PARA AMBIENTE DE HOMOLOGAÇÃO
            /**/
            System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(
            delegate
            {
                return true;
            });
            /**/
        }

        // PRECIFICADOR RESPOSTA
        public PrecificadorModel getPrecificador()
        {
            try
            {
                // MONTA A URL DO FORNECEDOR
                setStrRequisicao("P1");
                this.precificador = new PrecificadorModel();
                // FAZ A CONSULTA NO FORNECEDOR
                var tecnobank = new WsTecnobank.IeDecodificacaoClient(this.strRequisicao);
                tecnobank.ClientCredentials.UserName.UserName = string.Format("{0}\\{1}", this.acesToken, this.acesLogin);
                tecnobank.ClientCredentials.UserName.Password = this.acesPassword;
                WsTecnobank.ENTModeloVeiculo retornoWs;

                var relogio = Stopwatch.StartNew();

                if (!string.IsNullOrEmpty(this.carro.Placa))
                    retornoWs = tecnobank.PrecificadorPlacaI(this.carro.Placa);
                else
                    retornoWs = tecnobank.PrecificadorChassiI(this.carro.Chassi);

                relogio.Stop();
                var tempoDecorrido = relogio.ElapsedMilliseconds;

                // RETORNO PADRÃO DO FORNECEDOR
                // retornoWs = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><RESPOSTA><RETORNO><QTDEREG>1</QTDEREG></RETORNO><AGREGADOSXML><DATAULTIMAATUALIZACAO>2013-07-18</DATAULTIMAATUALIZACAO><PLACA>KNW3789</PLACA><CHASSI>8AFDR12A2AJ290586</CHASSI><NUMEROFATURADO>34029967000541</NUMEROFATURADO><ANOFABRICACAO>2009</ANOFABRICACAO><SEGMENTO>COMERCIAL LEVE</SEGMENTO><MARCA>FORD </MARCA><MODELO>FORD RANGER XLT 12A</MODELO><SUBSEGMENTO>CL - PICK-UP GRANDE</SUBSEGMENTO><MUNICIPIO>SAO GONCALO</MUNICIPIO><UF>RJ</UF><COMBUSTIVEL>GASOLINA</COMBUSTIVEL><POTENCIA>150</POTENCIA><CAPACIDADEDECARGA>80</CAPACIDADEDECARGA><PROCEDENCIA>IMPORTADO</PROCEDENCIA><NRCARROCERIA></NRCARROCERIA><NRCAIXACAMBIO></NRCAIXACAMBIO><EIXOTRASEIRODIFERENCIAL></EIXOTRASEIRODIFERENCIAL><TERCEIROEIXO></TERCEIROEIXO><NUMEROMOTOR>AJ290586</NUMEROMOTOR><ANOMODELO>2010</ANOMODELO><TIPOVEICULO>CAMINHONETE</TIPOVEICULO><ESPECIEVEICULO>ESPECIAL</ESPECIEVEICULO><TIPOCARROCERIA>ABERTA/CABINE DUPLA</TIPOCARROCERIA><COR>PRETA</COR><CAPACIDADEPASSAGEIRO>5</CAPACIDADEPASSAGEIRO><SITUACAOCHASSI>NORMAL</SITUACAOCHASSI><NUMEROEIXOS>2</NUMEROEIXOS><TIPOMONTAGEM>COMPLETA</TIPOMONTAGEM><TIPODOCFATURADO>JURIDICA</TIPODOCFATURADO><UFFATURADO>RJ</UFFATURADO><TIPODOCIMPORTADORA></TIPODOCIMPORTADORA><NUMEROIMPORTADORA></NUMEROIMPORTADORA><NUMERODI>0915625525</NUMERODI><DT_REGISTRODI></DT_REGISTRODI><DT_EMPLACAMENTO>2010-01-04</DT_EMPLACAMENTO><CD_RESTRICAO1>SEM RESTRICAO</CD_RESTRICAO1><CD_RESTRICAO2>SEM RESTRICAO</CD_RESTRICAO2><CD_RESTRICAO3>SEM RESTRICAO</CD_RESTRICAO3><CD_RESTRICAO4>SEM RESTRICAO</CD_RESTRICAO4><DT_LIMITERESTRICAOTRIBUTARIA></DT_LIMITERESTRICAOTRIBUTARIA><CILINDRADAS>2260</CILINDRADAS><CAPACMAXTRACAO>265</CAPACMAXTRACAO><PESOBRUTOTOTAL>248</PESOBRUTOTOTAL><RENAVAM></RENAVAM><SITUACAOVEICULO>EM CIRCULACAO</SITUACAOVEICULO><CODIGORETORNOPESQUISA>2939081</CODIGORETORNOPESQUISA></AGREGADOSXML></RESPOSTA>";

                // GRAVA O TEMPO DECORRIDO EM ARQUIVO DE LOG
                try
                {
                    LogEstatico.setLogTitulo("TEMPO AGREGADOS: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                    LogEstatico.setLogTexto("CHAVE: " + this.logLancamento);
                    LogEstatico.setLogTexto("PRECIFICADOR: TECNOBANK");
                    LogEstatico.setLogTexto("REQUISICAO: " + this.strRequisicao);
                    LogEstatico.setLogTexto("TEMPO: " + tempoDecorrido.ToString().PadRight(10, ' ') + "|" + "".ToString().PadRight(7, ' '));
                    LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                    LogEstatico.setLogTexto("PLACA: " + this.carro.Placa);
                }
                catch (Exception ex)
                {
                    LogEstatico.setLogTitulo("ERRO LOG CRONOMETRO: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                    LogEstatico.setLogTexto("MENSAGEM: " + ex.Message + ex.StackTrace);
                    this.precificador.ErroPrecificador = new Erros("0", "PRECIFICADOR: TIMEOUT: Falha ao acessar fornecedor: " + ex.ToString());
                    return this.precificador;
                }

                // ERRO RETORNADO PELO FORNECEDOR
                if (retornoWs.CdRetorno.Equals(2039))
                {
                    this.precificador.ErroPrecificador = new Inteligencia.Erros("0", "TECNOBANK: CONSULTA INDISPONIVEL: " + retornoWs.DsRetorno);
                    LogEstatico.setLogTitulo("ERRO PRECIFICADOR TECNOBANK" + System.DateTime.Now);
                    LogEstatico.setLogTexto("REQUISICAO: " + this.strRequisicao);
                    LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                    return this.precificador;
                }

                this.codRetorno = (int)retornoWs.CdRetorno;

                // REGISTROS NÃO ENCONTRADOS NA BASES CONSULTADAS
                if (retornoWs.ModelosPrecificadorI.Length == 0 || this.codRetorno == 5001 || this.codRetorno == 5003 || this.codRetorno == 5004 || this.codRetorno == 5005 || this.codRetorno == 5999)
                {
                    this.precificador.MsgPrecificador = new Erros("2", "INFORMACAO NAO ENCONTRADA NAS BASES CONSULTADAS");
                    return this.precificador;
                }
                // INFORMACAO ENCONTRADA
                if (this.codRetorno == 2000 || (this.codRetorno > 2003 && this.codRetorno < 2010))
                {
                    this.precificador.CodRetorno = this.codRetorno.ToString();
                    this.precificador.MsgPrecificador = new Erros("1", "CONSTA REGISTROS DE PRECIFICACAO");
                    this.precificador.RegistrosPrecos = new List<PrecificadorModel>();
                    
                    var dadosPrecificador1 = retornoWs.ModelosPrecificadorI;
                    PrecificadorModel tmpPrecos;
                    string tmpCodFipe = string.Empty;
                    string tmpValor = string.Empty;

                    foreach (WsTecnobank.ENTModeloPrecificadorI dados in dadosPrecificador1) 
                    {
                        tmpPrecos = new PrecificadorModel();

                        tmpPrecos.CodFipe = dados.DsCodigo;
                        tmpPrecos.Valor = dados.ModeloPrecificadorIAnos[0].ModeloPrecificadorIValor[0].NuValor.Value.ToString();
                        tmpPrecos.Modelo = dados.DsModelo;
                        tmpPrecos.Marca = dados.DsMarca;
                        tmpPrecos.Combustivel = dados.DsCombustivel;
                        // ANULA VALORES EM BRANCO
                        tmpPrecos = Util.unSetDadosVazios<PrecificadorModel>(tmpPrecos);

                        this.precificador.RegistrosPrecos.Add(tmpPrecos);
                    }
                    // ANULA VALORES EM BRANCO
                    this.precificador = Util.unSetDadosVazios<PrecificadorModel>(this.precificador);

                    return this.precificador;
                }
            }
            catch (Exception ex)
            {
                EnvioEmail SendMail = new EnvioEmail();
                SendMail.EnviaEmail(SendMail.LayoutFornecedor("TECNOBANK", "PRECIFICADOR", this.carro.Placa + this.carro.Chassi), "CONSULTA INDISPONIVEL", "antonio.carlos@emepar.com.br");

                this.precificador.ErroPrecificador = new Erros("0", "PRECIFICADOR: Falha ao acessar fornecedor: " + ex.ToString());

                if (ex.Message.Contains("timed out"))
                {
                    try
                    {
                        LogEstatico.setLogTitulo("TEMPO PRECIFICADOR: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                        LogEstatico.setLogTexto("CHAVE: " + this.logLancamento);
                        LogEstatico.setLogTexto("FORNECEDOR: TECNOBANK");
                        LogEstatico.setLogTexto("REQUISICAO: " + this.strRequisicao);
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

                LogEstatico.setLogTitulo("ERRO TECNOBANK: " + System.DateTime.Now);
                LogEstatico.setLogTexto(ex.Message + ex.StackTrace);
                LogEstatico.setLogTexto("PARAMETROS: LOGON : " + this.dadosUsuario.Logon + "; PLACA: " + this.carro.Placa);
            }

            return this.precificador;
        }

        // DECODIFICADOR DE CHASSI RESPOSTA
        public DecodChassiModel getDecodChassi()
        {
            try
            {
                // MONTA A URL DO FORNECEDOR
                setStrRequisicao("DC");
                this.decodChassi = new DecodChassiModel();
                // FAZ A CONSULTA NO FORNECEDOR
                var tecnobank = new WsTecnobank.IeDecodificacaoClient(this.strRequisicao);
                tecnobank.ClientCredentials.UserName.UserName = string.Format("{0}\\{1}", this.acesToken, this.acesLogin);
                tecnobank.ClientCredentials.UserName.Password = this.acesPassword;
                WsTecnobank.ENTModeloVeiculo retornoWs;

                var relogio = Stopwatch.StartNew();

                if (!string.IsNullOrEmpty(this.carro.Placa))
                    retornoWs = tecnobank.DecodificarPlaca(this.carro.Placa);
                else
                    retornoWs = tecnobank.DecodificarChassi(this.carro.Chassi);

                relogio.Stop();
                var tempoDecorrido = relogio.ElapsedMilliseconds;

                // GRAVA O TEMPO DECORRIDO EM ARQUIVO DE LOG
                try
                {
                    LogEstatico.setLogTitulo("TEMPO DECODIFICADOR: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                    LogEstatico.setLogTexto("CHAVE: " + this.logLancamento);
                    LogEstatico.setLogTexto("DECODIFICADOR: TECNOBANK");
                    LogEstatico.setLogTexto("REQUISICAO: " + this.strRequisicao);
                    LogEstatico.setLogTexto("TEMPO: " + tempoDecorrido.ToString().PadRight(10, ' ') + "|" + "".ToString().PadRight(7, ' '));
                    LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                    LogEstatico.setLogTexto("PLACA: " + this.carro.Placa);
                }
                catch (Exception ex)
                {
                    LogEstatico.setLogTitulo("ERRO LOG CRONOMETRO: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                    LogEstatico.setLogTexto("MENSAGEM: " + ex.Message + ex.StackTrace);
                    this.decodChassi.ErroDecodChassi = new Erros("0", "DECODIFICADOR: TIMEOUT: Falha ao acessar fornecedor: " + ex.ToString());
                    return this.decodChassi;
                }

                // ERRO RETORNADO PELO FORNECEDOR
                if (retornoWs.CdRetorno.Equals(2039))
                {
                    this.decodChassi.ErroDecodChassi = new Inteligencia.Erros("0", "TECNOBANK: CONSULTA INDISPONIVEL: " + retornoWs.DsRetorno);
                    LogEstatico.setLogTitulo("ERRO DECODIFICADOR TECNOBANK" + System.DateTime.Now);
                    LogEstatico.setLogTexto("REQUISICAO: " + this.strRequisicao);
                    LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                    return this.decodChassi;
                }

                this.codRetorno = (int)retornoWs.CdRetorno;

                // REGISTROS NÃO ENCONTRADOS NA BASES CONSULTADAS
                if (this.codRetorno == 5001 || this.codRetorno == 5003 || this.codRetorno == 5004 || this.codRetorno == 5005 || this.codRetorno == 5999)
                {
                    this.decodChassi.MsgDecodChassi = new Erros("2", "INFORMACAO NAO ENCONTRADA NAS BASES CONSULTADAS");
                    return this.decodChassi;
                }
                // INFORMACAO ENCONTRADA
                if (this.codRetorno == 2000 || (this.codRetorno > 2003 && this.codRetorno < 2010))
                {
                    this.decodChassi.MsgDecodChassi = new Erros("1", retornoWs.DsRetorno.ToUpper());
                    this.decodChassi.Regiao = retornoWs.DsRegiao;
                    this.decodChassi.Pais = retornoWs.DsPais;
                    this.decodChassi.Marca = retornoWs.DsMarca;
                    this.decodChassi.Potencia = retornoWs.DsMotor;
                    this.decodChassi.Combustivel = retornoWs.DsCombustivel;
                    this.decodChassi.Versao = retornoWs.DsVersao;
                    this.decodChassi.TipoCarroceria = retornoWs.DsCarroceria;
                    this.decodChassi.Modelo = retornoWs.DsModelo;
                    this.decodChassi.AnoModelo = retornoWs.NuAno.ToString();
                    this.decodChassi.LocalFabricacao = retornoWs.DsLocalFabricacao;
                    
                    // ANULA VALORES EM BRANCO
                    this.decodChassi = Util.unSetDadosVazios<DecodChassiModel>(this.decodChassi);

                    return this.decodChassi;
                }
            }
            catch (Exception ex)
            {
                EnvioEmail SendMail = new EnvioEmail();
                SendMail.EnviaEmail(SendMail.LayoutFornecedor("TECNOBANK", "DECODIFICADOR", this.carro.Placa + this.carro.Chassi), "CONSULTA INDISPONIVEL", "antonio.carlos@emepar.com.br");

                this.precificador.ErroPrecificador = new Erros("0", "DECODIFICADOR: Falha ao acessar fornecedor: " + ex.ToString());

                if (ex.Message.Contains("timed out"))
                {
                    try
                    {
                        LogEstatico.setLogTitulo("TEMPO DECODIFICADOR: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                        LogEstatico.setLogTexto("CHAVE: " + this.logLancamento);
                        LogEstatico.setLogTexto("FORNECEDOR: TECNOBANK");
                        LogEstatico.setLogTexto("REQUISICAO: " + this.strRequisicao);
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

                LogEstatico.setLogTitulo("ERRO TECNOBANK: " + System.DateTime.Now);
                LogEstatico.setLogTexto(ex.Message + ex.StackTrace);
                LogEstatico.setLogTexto("PARAMETROS: LOGON : " + this.dadosUsuario.Logon + "; PLACA: " + this.carro.Placa);
            }

            return this.decodChassi;
        }

        // MONTA A URL PARA OBTER O XML DO FORNECEDOR
        public void setStrRequisicao(string tpConsulta = "P1")
        {
            switch (tpConsulta)
            {
                // PRECIFICADOR 1
                case "P1":
                    this.strRequisicao = "BasicHttpBinding_IeDecodificacao";
                break;
                default:
                this.strRequisicao = "BasicHttpBinding_IeDecodificacao";
                break;
            }
        }
    }
}