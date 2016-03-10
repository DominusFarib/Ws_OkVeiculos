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
    public class FornMotorCheck
    {
        public UsuarioModel dadosUsuario = new UsuarioModel();

        public string codConsulta { get; set; }
        public string tipoConsulta { get; set; }
        public string urlRequisicao { get; set; }
        public string logLancamento { get; set; }
        
        public bool flagDocProprietario { get; set; }

        private string retornoWs = string.Empty;
        private string acesLogin = string.Empty;
        private string acesPassword = string.Empty;

        private AgregadosModel agregados;
        private BinModel binNacional;
        private BinRouboFurtoModel binRouboFurto;
        private Veiculo carro;
        private List<Veiculo> dadosCarro { get; set; }

        // CONSTRUTOR
        public FornMotorCheck(Veiculo carro)
        {
            this.carro = carro;
            this.acesLogin = "CHECKOK";
            this.acesPassword = "J86KF8K4329F";
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
                // this.retornoWs = "<?xml version='1.0' encoding='ISO-8859-1'?><RESPOSTA><binXML><situacao>CIRCULACAO</situacao><ocorrencia>VEICULO NAO INDICA OCORRENCIA DE ROUBO/FURTO</ocorrencia><placa>BTA1395</placa><municipio>GUARULHOS</municipio><uf>SP</uf><renavam>667886001</renavam><documento>27078567004809</documento><ultima_atualizacao>1997-01-24 00:00:00</ultima_atualizacao><chassi>9BM388054TB111112</chassi><remarcacao_do_chassi>N</remarcacao_do_chassi><montagem>COMPLETA</montagem><motor>47697910699962</motor><caixa_cambio>7183801282232</caixa_cambio><num_carroceria>3848131001151</num_carroceria><num_eixo>2</num_eixo><num_eixo_aux>7406551917271</num_eixo_aux><aux></aux><marca>M.BENZ/ LS 1935</marca><tipo_veiculo>Caminhao Trator</tipo_veiculo><ano_fabricacao>1996</ano_fabricacao><ano_modelo>1997</ano_modelo><procedencia>Nacional</procedencia><especie>Tracao</especie><combustivel>Diesel</combustivel><cilindrada>0</cilindrada><cor>BRANCA</cor><potencia>360</potencia><capacidade_passageiros>0</capacidade_passageiros><capacidade_carga>0</capacidade_carga><CMT>8000</CMT><PTB>1800</PTB><restricao></restricao></binXML><RF><categoria_ocorrencia>RECUPERACAO</categoria_ocorrencia><ano>2004</ano><orgao_seguranca></orgao_seguranca><boletim>94</boletim><numero_ocorrencia>SAO PAULO</numero_ocorrencia><tipo_declaracao></tipo_declaracao><data_ocorrencia>26/4/2004</data_ocorrencia><alarme></alarme></RF><RF><categoria_ocorrencia>DEVOLUCAO</categoria_ocorrencia><ano>2004</ano><orgao_seguranca></orgao_seguranca><boletim>94</boletim><numero_ocorrencia>SAO PAULO</numero_ocorrencia><tipo_declaracao></tipo_declaracao><data_ocorrencia>26/4/2004</data_ocorrencia><alarme></alarme></RF><RF><categoria_ocorrencia>DECLARACAO</categoria_ocorrencia><ano>1999</ano><orgao_seguranca>DELEGACIA DO ESTADO SP</orgao_seguranca><boletim>4634</boletim><numero_ocorrencia>SAO PAULO</numero_ocorrencia><tipo_declaracao></tipo_declaracao><data_ocorrencia>30/6/1999</data_ocorrencia><alarme></alarme></RF></RESPOSTA>";

                // GRAVA O TEMPO DECORRIDO EM ARQUIVO DE LOG
                try
                {
                    LogEstatico.setLogTitulo("TEMPO BIN ROUBO E FURTO: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                    LogEstatico.setLogTexto("CHAVE: " + this.logLancamento);
                    LogEstatico.setLogTexto("FORNECEDOR: CHECKAUTO");
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
                    this.binRouboFurto.ErroBinNacional = new Inteligencia.Erros("0", "CHECKAUTO: CONSULTA INDISPONIVEL");
                    LogEstatico.setLogTitulo("ERRO BIN ROUBO E FURTO CHECKAUTO" + System.DateTime.Now);
                    LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                    LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                    return this.binRouboFurto;
                }

                this.retornoWs = this.retornoWs.Trim();

                XmlDocument xmlResposta = new XmlDocument();
                xmlResposta.LoadXml(this.retornoWs);

                // FALHA NA CONSULTA
                if (this.retornoWs.Contains("ERROR"))
                {
                    this.binRouboFurto.MsgBinNacional = new Erros("2", xmlResposta.SelectNodes("/RETORNO/MENSAGEM").Item(0).InnerText.Replace("ERROR:", ""));
                    return this.binRouboFurto;
                }
                // SISTEMA INDISPONIVEL
                if (xmlResposta.SelectNodes("/RESPOSTA/BINXML/SITUACAO").Item(0).InnerText.Contains("SISTEMA"))
                {
                    this.binRouboFurto.MsgBinNacional = new Erros("0", xmlResposta.SelectNodes("/RESPOSTA/BINXML/situacao").Item(0).InnerText);
                    return this.binRouboFurto;
                }

                // INFORMACAO ENCONTRADA
                this.binRouboFurto.MsgBinNacional = new Erros("1", "CONSTA REGISTRO FEDERAL");
                string tempData = string.Empty;
                string[] tempDigitos;

                this.binRouboFurto.Automovel = new Veiculo();

                this.binRouboFurto.SituacaoVeiculo = xmlResposta.SelectNodes("/RESPOSTA/BINXML/SITUACAO").Item(0).InnerText;
                // this.binRouboFurto.Obs = xmlResposta.SelectNodes("/RESPOSTA/BINXML/OCORRENCIA").Item(0).InnerText.Trim();
                // this.binRouboFurto.DocFaturado = xmlResposta.SelectNodes("/RESPOSTA/BINXML/CPFCNPJFATURADO").Item(0).InnerText.Trim();
                // this.binRouboFurto.TipoDocFaturado = xmlResposta.SelectNodes("/RESPOSTA/BINXML/TIPODOCUMENTOFATURADO").Item(0).InnerText;
                // this.binRouboFurto.TipoDocImportadora = xmlResposta.SelectNodes("/RESPOSTA/BINXML/TIPODOCUMENTOIMPORTADORA").Item(0).InnerText;
                // this.binRouboFurto.UFFaturado = xmlResposta.SelectNodes("/RESPOSTA/BINXML/UFFATURADO").Item(0).InnerText;
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
                SendMail.EnviaEmail(SendMail.LayoutFornecedor("CHECKAUTO", "BIN ROUBO E FURTO", this.carro.Placa + this.carro.Chassi), "CONSULTA INDISPONIVEL", "antonio.carlos@emepar.com.br");

                this.binRouboFurto.ErroBinNacional = new Erros("0", "BIN ROUBO E FURTO: Falha ao acessar fornecedor");

                if (ex.Message.Contains("timed out"))
                {
                    try
                    {
                        LogEstatico.setLogTitulo("TEMPO BIN ROUBO E FURTO: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                        LogEstatico.setLogTexto("CHAVE: " + this.logLancamento);
                        LogEstatico.setLogTexto("FORNECEDOR: CHECKAUTO");
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

                LogEstatico.setLogTitulo("ERRO CHECKAUTO BIN ROUBO E FURTO: " + System.DateTime.Now);
                LogEstatico.setLogTexto(ex.Message + ex.StackTrace);
                LogEstatico.setLogTexto("PARAMETROS: LOGON : " + this.dadosUsuario.Logon + "; PLACA: " + this.carro.Placa);
            }

            return this.binRouboFurto;
        }
        
        // AGREGADOS RESPOSTA
        public AgregadosModel getAgregados()
        {
            try
            {
                // MONTA A URL DO FORNECEDOR
                setUrlRequisicao("AG");

                this.agregados = new AgregadosModel();
                // FAZ A CONSULTA NO FORNECEDOR
                var request = new CustomTimeOut();
                var relogio = Stopwatch.StartNew();
                this.retornoWs = request.DownloadString(this.urlRequisicao);
                var decorrido = relogio.ElapsedMilliseconds;
                // RETORNO PADRÃO DO FORNECEDOR
                // this.retornoWs = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><RESPOSTA><RETORNO><QTDEREG>1</QTDEREG></RETORNO><AGREGADOSXML><DATAULTIMAATUALIZACAO>2013-07-18</DATAULTIMAATUALIZACAO><PLACA>KNW3789</PLACA><CHASSI>8AFDR12A2AJ290586</CHASSI><NUMEROFATURADO>34029967000541</NUMEROFATURADO><ANOFABRICACAO>2009</ANOFABRICACAO><SEGMENTO>COMERCIAL LEVE</SEGMENTO><MARCA>FORD </MARCA><MODELO>FORD RANGER XLT 12A</MODELO><SUBSEGMENTO>CL - PICK-UP GRANDE</SUBSEGMENTO><MUNICIPIO>SAO GONCALO</MUNICIPIO><UF>RJ</UF><COMBUSTIVEL>GASOLINA</COMBUSTIVEL><POTENCIA>150</POTENCIA><CAPACIDADEDECARGA>80</CAPACIDADEDECARGA><PROCEDENCIA>IMPORTADO</PROCEDENCIA><NRCARROCERIA></NRCARROCERIA><NRCAIXACAMBIO></NRCAIXACAMBIO><EIXOTRASEIRODIFERENCIAL></EIXOTRASEIRODIFERENCIAL><TERCEIROEIXO></TERCEIROEIXO><NUMEROMOTOR>AJ290586</NUMEROMOTOR><ANOMODELO>2010</ANOMODELO><TIPOVEICULO>CAMINHONETE</TIPOVEICULO><ESPECIEVEICULO>ESPECIAL</ESPECIEVEICULO><TIPOCARROCERIA>ABERTA/CABINE DUPLA</TIPOCARROCERIA><COR>PRETA</COR><CAPACIDADEPASSAGEIRO>5</CAPACIDADEPASSAGEIRO><SITUACAOCHASSI>NORMAL</SITUACAOCHASSI><NUMEROEIXOS>2</NUMEROEIXOS><TIPOMONTAGEM>COMPLETA</TIPOMONTAGEM><TIPODOCFATURADO>JURIDICA</TIPODOCFATURADO><UFFATURADO>RJ</UFFATURADO><TIPODOCIMPORTADORA></TIPODOCIMPORTADORA><NUMEROIMPORTADORA></NUMEROIMPORTADORA><NUMERODI>0915625525</NUMERODI><DT_REGISTRODI></DT_REGISTRODI><DT_EMPLACAMENTO>2010-01-04</DT_EMPLACAMENTO><CD_RESTRICAO1>SEM RESTRICAO</CD_RESTRICAO1><CD_RESTRICAO2>SEM RESTRICAO</CD_RESTRICAO2><CD_RESTRICAO3>SEM RESTRICAO</CD_RESTRICAO3><CD_RESTRICAO4>SEM RESTRICAO</CD_RESTRICAO4><DT_LIMITERESTRICAOTRIBUTARIA></DT_LIMITERESTRICAOTRIBUTARIA><CILINDRADAS>2260</CILINDRADAS><CAPACMAXTRACAO>265</CAPACMAXTRACAO><PESOBRUTOTOTAL>248</PESOBRUTOTOTAL><RENAVAM></RENAVAM><SITUACAOVEICULO>EM CIRCULACAO</SITUACAOVEICULO><CODIGORETORNOPESQUISA>2939081</CODIGORETORNOPESQUISA></AGREGADOSXML></RESPOSTA>";

                // GRAVA O TEMPO DECORRIDO EM ARQUIVO DE LOG
                try
                {
                    LogEstatico.setLogTitulo("TEMPO AGREGADOS: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                    LogEstatico.setLogTexto("CHAVE: " + this.logLancamento);
                    LogEstatico.setLogTexto("FORNECEDOR: MOTORCHECK");
                    LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                    LogEstatico.setLogTexto("TEMPO: " + decorrido.ToString().PadRight(10, ' ') + "|" + "".ToString().PadRight(7, ' '));
                    LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                    LogEstatico.setLogTexto("PLACA: " + this.carro.Placa);
                }
                catch (Exception ex)
                {
                    LogEstatico.setLogTitulo("ERRO LOG CRONOMETRO: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                    LogEstatico.setLogTexto("MENSAGEM: " + ex.Message + ex.StackTrace);
                    this.agregados.ErroAgregados = new Erros("0", "AGREGADOS: TIMEOUT: Falha ao acessar fornecedor: " + ex.ToString());
                    return this.agregados;
                }

                if (String.IsNullOrEmpty(this.retornoWs))
                {
                    this.agregados.ErroAgregados = new Inteligencia.Erros("0", "MOTORCHECK: CONSULTA INDISPONIVEL");
                    LogEstatico.setLogTitulo("ERRO AGREGADOS MOTORCHECK" + System.DateTime.Now);
                    LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                    LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                    return this.agregados;
                }

                this.retornoWs = this.retornoWs.Trim();

                XmlDocument arrayResposta = new XmlDocument();
                arrayResposta.LoadXml(this.retornoWs);

                // ERRO AO ACESSAR FORNECEDOR
                if (this.retornoWs.Contains("SISTEMA INDISPONIVEL") || retornoWs.Contains("PROBLEMAS NA EXECUCAO"))
                {
                    this.agregados.ErroAgregados = new Erros("-1", "SISTEMA INDISPONIVEL TEMPORARIAMENTE");
                    return this.agregados;
                }
                // REGISTROS NÃO ENCONTRADOS NA BASES CONSULTADAS
                if (this.retornoWs.Contains("NENHUM REGISTRO ENCONTRADO"))
                {
                    this.agregados.MsgAgregados = new Erros("2", "INFORMACAO NAO ENCONTRADA NAS BASES CONSULTADAS");
                    return this.agregados;
                }
                // ERRO RETORNADO PELO FORNECEDOR
                if (this.retornoWs.Contains("ERROR"))
                {
                    this.agregados.ErroAgregados = new Erros("0", arrayResposta.SelectNodes("/RETORNO/MENSAGEM").Item(0).InnerText.Replace("ERROR:", ""));
                    return this.agregados;
                }

                // INFORMACAO ENCONTRADA
                this.agregados.CodRetorno = "009";
                this.agregados.MsgAgregados = new Erros("1", "CONSTA REGISTRO ESTADUAL");
                this.agregados.ListaVeiculos = new List<Veiculo>();
                Veiculo tmpCarro;
                for (int i = 0; i <= arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML").Count - 1; i++)
                {
                    tmpCarro = new Veiculo();

                    tmpCarro.Modelo = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/MARCA").Item(0).InnerText;
                    tmpCarro.Placa = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/PLACA").Item(0).InnerText;
                    tmpCarro.Chassi = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/CHASSI").Item(0).InnerText.Trim();
                    tmpCarro.MunicipioEmplacamento = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/MUNICIPIO").Item(0).InnerText;
                    tmpCarro.Uf = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/UF").Item(0).InnerText;
                    tmpCarro.AnoModelo = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/ANOMODELO").Item(0).InnerText;
                    tmpCarro.AnoFabrica = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/ANOFABRICACAO").Item(0).InnerText;
                    tmpCarro.Cor = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/COR").Item(0).InnerText;
                    tmpCarro.Combustivel = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/COMBUSTIVEL").Item(0).InnerText;
                    tmpCarro.Especie = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/ESPECIEVEICULO").Item(0).InnerText;
                    tmpCarro.CapacidadePassageiros = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/CAPACIDADEPASSAGEIRO").Item(0).InnerText;
                    tmpCarro.Tipo = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/TIPOVEICULO").Item(0).InnerText;
                    tmpCarro.TipoCarroceria = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/TIPOVEICULO").Item(0).InnerText;
                    tmpCarro.NrCarroceria = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/NRCARROCERIA").Item(0).InnerText;
                    tmpCarro.Potencia = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/POTENCIA").Item(0).InnerText;
                    tmpCarro.Cilindradas = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/CILINDRADAS").Item(0).InnerText;
                    tmpCarro.CapacidadeCarga = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/CAPACIDADEDECARGA").Item(0).InnerText;
                    tmpCarro.PesoBruto = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/PESOBRUTOTOTAL").Item(0).InnerText;
                    tmpCarro.CapacidadeMaximaTracao = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/CAPACMAXTRACAO").Item(0).InnerText;
                    tmpCarro.Nacionalidade = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/PROCEDENCIA").Item(0).InnerText;
                    tmpCarro.TipoChassi = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/SITUACAOCHASSI").Item(0).InnerText;
                    tmpCarro.NrCambio = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/NRCAIXACAMBIO").Item(0).InnerText;
                    tmpCarro.QtdEixos = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/NUMEROEIXOS").Item(0).InnerText;
                    tmpCarro.NrTerceiroEixo = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/TERCEIROEIXO").Item(0).InnerText;
                    tmpCarro.NrEixoTraseiro = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/EIXOTRASEIRODIFERENCIAL").Item(0).InnerText;
                    tmpCarro.TipoMontagem = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/TIPOMONTAGEM").Item(0).InnerText;
                    tmpCarro.NrMotor = arrayResposta.SelectNodes("/RESPOSTA/AGREGADOSXML/NUMEROMOTOR").Item(0).InnerText;

                    tmpCarro = Util.unSetDadosVazios<Veiculo>(tmpCarro);

                    this.agregados.ListaVeiculos.Add(tmpCarro);
                }
                // ANULA VALORES EM BRANCO
                this.agregados = Util.unSetDadosVazios<AgregadosModel>(this.agregados);

                return this.agregados;
            }
            catch (Exception ex)
            {
                EnvioEmail SendMail = new EnvioEmail();
                SendMail.EnviaEmail(SendMail.LayoutFornecedor("MOTORCHECK", "AGREGADOS", this.carro.Placa + this.carro.Chassi), "CONSULTA INDISPONIVEL", "antonio.carlos@emepar.com.br");

                this.agregados.ErroAgregados = new Erros("0", "AGREGADOS: Falha ao acessar fornecedor: " + ex.ToString());

                if (ex.Message.Contains("timed out"))
                {
                    try
                    {
                        LogEstatico.setLogTitulo("TEMPO AGREGADOS: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                        LogEstatico.setLogTexto("CHAVE: " + this.logLancamento);
                        LogEstatico.setLogTexto("FORNECEDOR: MOTORCHECK");
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

                LogEstatico.setLogTitulo("ERRO MOTORCHECK: " + System.DateTime.Now);
                LogEstatico.setLogTexto(ex.Message + ex.StackTrace);
                LogEstatico.setLogTexto("PARAMETROS: LOGON : " + this.dadosUsuario.Logon + "; PLACA: " + this.carro.Placa);
            }

            return this.agregados;
        }

        // MONTA A URL PARA OBTER O XML DO FORNECEDOR
        public void setUrlRequisicao(string tpConsulta = "AG")
        {
            switch (tpConsulta)
            {
                // PROPRIETARIO
                case "AG":
                    this.urlRequisicao = "http://201.20.2.154/MotorCheck/URL/PesquisaAgregados.aspx?txtusuario=" + this.acesLogin + "&txtsenha=" + this.acesPassword;
                    
                    if (!String.IsNullOrEmpty(this.carro.Placa))
                        this.urlRequisicao += "&txtplaca=" + this.carro.Placa;
                    else if (!String.IsNullOrEmpty(this.carro.Chassi))
                        this.urlRequisicao += "&txtchassi=" + this.carro.Chassi;
                    else if (!String.IsNullOrEmpty(this.carro.NrCarroceria))
                        this.urlRequisicao += "&txtnumerocarroceria=" + this.carro.NrCarroceria;
                    else if (!String.IsNullOrEmpty(this.carro.NrCambio))
                        this.urlRequisicao += "&txtnumerocambio=" + this.carro.NrCambio;
                    else if (!String.IsNullOrEmpty(this.carro.NrTerceiroEixo))
                        this.urlRequisicao += "&txtnumeroterceiroeixo=" + this.carro.NrTerceiroEixo;
                    else if (!String.IsNullOrEmpty(this.carro.NrEixoTraseiro))
                        this.urlRequisicao += "&txtnumeroeixotraseiro=" + this.carro.NrEixoTraseiro;
                    else if (!String.IsNullOrEmpty(this.carro.NrMotor))
                        this.urlRequisicao += "&txtnumeromotor=" + this.carro.NrMotor;
                break;

                case "BNRF":
                    this.urlRequisicao = "http://201.20.2.154/MotorCheck/URL/PesquisaBINRF.aspx?txtusuario=" + this.acesLogin + "&txtsenha=" + this.acesPassword;

                    if (!String.IsNullOrEmpty(this.carro.Placa))
                        this.urlRequisicao += "&txtplaca=" + this.carro.Placa;
                    if (!String.IsNullOrEmpty(this.carro.Chassi))
                        this.urlRequisicao += "&txtchassi=" + this.carro.Chassi;
                break;
            }
        }
    }
}