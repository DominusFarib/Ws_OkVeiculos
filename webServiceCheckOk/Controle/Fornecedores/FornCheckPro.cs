using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;
using webServiceCheckOk.Model;
using webServiceCheckOk.Model.ProdutosModel;
using webServiceCheckOk.Controle.Inteligencia;
using webServiceCheckOk.Controle.Inteligencia.Utils;

namespace webServiceCheckOk.Controle.Fornecedores
{
    public class FornCheckPro
    {
        public UsuarioModel dadosUsuario = new UsuarioModel();

        public string codConsulta { get; set; }
        public string tipoConsulta { get; set; }
        public string urlRequisicao { get; set; }
        public string logLancamento { get; set; }
        
        public bool flagDocProprietario { get; set; }
        private XmlDocument retornoWsXml;
        private string retornoWs = string.Empty;
        private string acesLogin = string.Empty;
        private string acesPassword = string.Empty;

        private WsCheckPro.Service wsCheckPro;
        private SinistroModel perdaTotal;
        private Veiculo carro;
        private Pessoa pessoa;

        // CONSTRUTOR
        public FornCheckPro(Veiculo carro, Pessoa pessoa = null)
        {
            this.carro = carro;
            this.pessoa = pessoa;

            this.acesLogin = "07680458863";
            this.acesPassword = "5bP&1a";
        }

        // SINISTRO - PERDA TOTAL
        public SinistroModel getPerdaTotal()
        {
            try
            {
                this.perdaTotal = new SinistroModel();
                // FAZ A CONSULTA NO FORNECEDOR
                this.wsCheckPro = new WsCheckPro.Service();

                if (!String.IsNullOrEmpty(this.carro.Chassi))
                    this.retornoWs = wsCheckPro.ConsultaBdsiiPorChassi(this.acesLogin, this.acesPassword, this.carro.Chassi);
                else if (!String.IsNullOrEmpty(this.carro.Placa))
                    this.retornoWs = wsCheckPro.ConsultaBdsiiPorPlaca(this.acesLogin, this.acesPassword, this.carro.Placa);
                
                // RETORNO PADRÃO DO FORNECEDOR
                // this.retornoWs = "<Retorno xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><Protocolo>670887</Protocolo><MensagemRetorno>Não Retornou Dados</MensagemRetorno><StatusRetorno>6</StatusRetorno><DataProtocolo>2016-03-08T10:32:54.76</DataProtocolo></Retorno>";

                if (String.IsNullOrEmpty(this.retornoWs))
                {
                    this.perdaTotal.ErroSinistro = new Inteligencia.Erros("0", "CHECKPRO: CONSULTA INDISPONIVEL");
                    LogEstatico.setLogTitulo("ERRO PERDA TOTAL CHECKPRO" + System.DateTime.Now);
                    LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                    LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                    return this.perdaTotal;
                }

                this.retornoWs = this.retornoWs.Trim();

                // INFORMACAO NÃO ENCONTRADA
                if (this.retornoWs.Contains("Não Retornou Dados"))
                {
                    this.perdaTotal.MsgSinistro = new Erros("0", "NAO CONSTA SINISTRO DE INDENIZACAO INTEGRAL");
                    return this.perdaTotal;
                }

                this.retornoWsXml = new XmlDocument();
                this.retornoWsXml.LoadXml(this.retornoWs);

                // INFORMACAO ENCONTRADA
                if (this.retornoWs.Contains("Retorno Ok"))
                {
                    string tempData = string.Empty;
                    string[] tempDigitos;

                    tempData = retornoWsXml.GetElementsByTagName("DataOcorrencia").Item(0).InnerText;

                    if (tempData.Contains("-"))
                    {
                        tempDigitos = tempData.Split('-');
                        tempData = tempDigitos[2] + "/" + tempDigitos[1] + "/" + tempDigitos[0];
                    }

                    this.perdaTotal.MsgSinistro = new Erros("1", "CONSTA SINISTRO DE INDENIZACAO INTEGRAL");
                    this.perdaTotal.DtRegistro = tempData;
                    this.perdaTotal.Descricao = "INDICIO DE SINISTRO ENCONTRADO";
                    this.perdaTotal.Classificacao = "PERDA TOTAL";
                }
                else 
                {
                    this.perdaTotal.ErroSinistro = new Erros("0", "SISTEMA INDISPONIVEL TEMPORARIAMENTE");
                    return this.perdaTotal;
                }

                // ANULA VALORES EM BRANCO
                this.perdaTotal = Util.unSetDadosVazios<SinistroModel>(this.perdaTotal);

                return this.perdaTotal;
            }
            catch (Exception ex)
            {
                EnvioEmail SendMail = new EnvioEmail();
                SendMail.EnviaEmail(SendMail.LayoutFornecedor("CHECKPRO", "PERDA TOTAL", this.carro.Placa + this.carro.Chassi), "CONSULTA INDISPONIVEL", "antonio.carlos@emepar.com.br");

                this.perdaTotal.ErroSinistro = new Erros("0", "PERDA TOTAL: Falha ao acessar fornecedor");

                if (ex.Message.Contains("timed out"))
                {
                    try
                    {
                        LogEstatico.setLogTitulo("TEMPO PERDA TOTAL: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                        LogEstatico.setLogTexto("CHAVE: " + this.logLancamento);
                        LogEstatico.setLogTexto("FORNECEDOR: CHECKPRO");
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

                LogEstatico.setLogTitulo("ERRO CHECKPRO PERDA TOTAL: " + System.DateTime.Now);
                LogEstatico.setLogTexto(ex.Message + ex.StackTrace);
                LogEstatico.setLogTexto("PARAMETROS: LOGON : " + this.dadosUsuario.Logon + "; PLACA: " + this.carro.Placa);
            }

            return this.perdaTotal;
        }
    }
}