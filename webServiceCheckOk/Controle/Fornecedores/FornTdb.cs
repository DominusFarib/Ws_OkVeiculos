using System;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using webServiceCheckOk.Model;
using webServiceCheckOk.Model.ProdutosModel;
using webServiceCheckOk.Controle.Inteligencia;
using System.Diagnostics;
using webServiceCheckOk.Controle.Inteligencia.Utils;
using System.Reflection;

namespace webServiceCheckOk.Controle.Fornecedores
{
    public class FornTdb
    {
        private UsuarioModel dadosUsuario;

        public string codConsulta { get; set; }
        public string urlRequisicao { get; set; }
        public string logLancamento { get; set; }
        public string tipoBaseLeilao { get; set; } // 1=BASE ONLINE; 2=BASE COM HISTORICO

        private Veiculo tempCarro;
        private LeilaoModel tempLeilao;

        private string retornoWs = string.Empty;
        private string acesLogin = string.Empty;
        private string acesPassword = string.Empty;
        
        private LeilaoModel retornoLeilao;
        private Veiculo carro;
        private List<Veiculo> dadosCarro { get; set; }

        // CONSTRUTOR
        public FornTdb(Veiculo carro, string tipoBaseLeilao, UsuarioModel usuario)
        {
            this.dadosUsuario = usuario;
            this.carro = carro;
            this.acesLogin = "CHECKOK";
            this.acesPassword = "CHK10109798";
            this.tipoBaseLeilao = tipoBaseLeilao;
        }
        
        // BIN NACIONAL RESPOSTA
        public LeilaoModel getLeilao()
        {
            try
            {
                setUrlRequisicao();

                this.retornoLeilao = new LeilaoModel();
                // FAZ A CONSULTA NO FORNECEDOR
                var request = new CustomTimeOut();
                var relogio = Stopwatch.StartNew();
                //this.retornoWs = request.DownloadString(this.urlRequisicao);
                var decorrido = relogio.ElapsedMilliseconds;
                // RETORNO PADRÃO DO FORNECEDOR
                this.retornoWs = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><RESPOSTA><RETORNO><EXISTE_ERRO>0</EXISTE_ERRO><MSG_ERRO>pesquisa efetuada com sucesso.</MSG_ERRO><MENSAGEM></MENSAGEM><COD_RETORNO>I</COD_RETORNO><CHAVERETORNO>16020317273895000018</CHAVERETORNO><FORNECEDOR>1</FORNECEDOR><QTDTOTLEILAO>6</QTDTOTLEILAO></RETORNO><LEILAOXML><LEILAONUMERO>1</LEILAONUMERO><MARCA></MARCA><MODELO>CITROEN XSARA GLX 1.6</MODELO><ANO_MODELO>2001</ANO_MODELO><PLACA>IJY2323</PLACA><CHASSI>N/C</CHASSI><RENAVAM></RENAVAM><COR>PRATA</COR><COMBUSTIVEL></COMBUSTIVEL><UF></UF><CATEGORIA></CATEGORIA><KILOMETRAGEM></KILOMETRAGEM><CONDICAO_VEICULO>Cond. Veiculo: BATIDO</CONDICAO_VEICULO><CONDICAO_CHASSI>Sit. Chassi: N/C</CONDICAO_CHASSI><CONDICAO_MOTOR>Cond. Motor: N/C</CONDICAO_MOTOR><CONDICAO_CAMBIO>Cond. Cambio: N/C</CONDICAO_CAMBIO><CONDICAO_CARROCERIA></CONDICAO_CARROCERIA><CONDICAO_MECANICA>Sit. Chassi: N/C EIXO TRAZEIRO: LOTE:N/C VEICULO:</CONDICAO_MECANICA><AR_CONDICIONADO></AR_CONDICIONADO><DIRECAO_HIDRAULICA></DIRECAO_HIDRAULICA><IMPORTADO></IMPORTADO><IMAGE1>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14412-MLB3891251652_022013-F.jpg</IMAGE1><IMAGE2>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14592-MLB3891251080_022013-F.jpg</IMAGE2><IMAGE3>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14595-MLB3891252590_022013-F.jpg</IMAGE3><IMAGE4>http://img.olx.com.br/images/91/910522110043472.jpg</IMAGE4><DATA_LEILAO>12/04/2004</DATA_LEILAO><LEILOEIRO>LUIZ FERNANDO DE ABREU SODRE SANTORO</LEILOEIRO><COMITENTE>N/C</COMITENTE></LEILAOXML><LEILAOXML><LEILAONUMERO>2</LEILAONUMERO><MARCA></MARCA><MODELO>CITROEN XSARA</MODELO><ANO_MODELO>2001</ANO_MODELO><PLACA>IJY2323</PLACA><CHASSI>VF7N1N6AK1J402326</CHASSI><RENAVAM></RENAVAM><COR>PRATA</COR><COMBUSTIVEL></COMBUSTIVEL><UF></UF><CATEGORIA></CATEGORIA><KILOMETRAGEM></KILOMETRAGEM><CONDICAO_VEICULO>Cond. Veiculo: N/C</CONDICAO_VEICULO><CONDICAO_CHASSI>Sit. Chassi: N/C</CONDICAO_CHASSI><CONDICAO_MOTOR>Cond. Motor: N/C</CONDICAO_MOTOR><CONDICAO_CAMBIO>Cond. Cambio: N/C</CONDICAO_CAMBIO><CONDICAO_CARROCERIA></CONDICAO_CARROCERIA><CONDICAO_MECANICA>Sit. Chassi: N/C EIXO TRAZEIRO: LOTE:0135 VEICULO:</CONDICAO_MECANICA><AR_CONDICIONADO></AR_CONDICIONADO><DIRECAO_HIDRAULICA></DIRECAO_HIDRAULICA><IMPORTADO></IMPORTADO><IMAGE1>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14412-MLB3891251652_022013-F.jpg</IMAGE1><IMAGE2>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14592-MLB3891251080_022013-F.jpg</IMAGE2><IMAGE3>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14595-MLB3891252590_022013-F.jpg</IMAGE3><IMAGE4>http://img.olx.com.br/images/91/910522110043472.jpg</IMAGE4><DATA_LEILAO>12/04/2004 </DATA_LEILAO><LEILOEIRO>LUIZ FERNANDO DE ABREU SODRE SANTORO</LEILOEIRO><COMITENTE></COMITENTE></LEILAOXML><LEILAOXML><LEILAONUMERO>3</LEILAONUMERO><MARCA></MARCA><MODELO>CITROEN XSARA GLX 1.6</MODELO><ANO_MODELO>2001</ANO_MODELO><PLACA>IJY2323</PLACA><CHASSI>VF7N1N6AK1J402326</CHASSI><RENAVAM></RENAVAM><COR>PRATA</COR><COMBUSTIVEL></COMBUSTIVEL><UF></UF><CATEGORIA></CATEGORIA><KILOMETRAGEM></KILOMETRAGEM><CONDICAO_VEICULO>Cond. Veiculo: Analisar Avarias</CONDICAO_VEICULO><CONDICAO_CHASSI>Sit. Chassi: N/C</CONDICAO_CHASSI><CONDICAO_MOTOR>Cond. Motor: N/C</CONDICAO_MOTOR><CONDICAO_CAMBIO>Cond. Cambio: N/C</CONDICAO_CAMBIO><CONDICAO_CARROCERIA></CONDICAO_CARROCERIA><CONDICAO_MECANICA>Sit. Chassi: N/C EIXO TRAZEIRO: LOTE:N/C VEICULO:</CONDICAO_MECANICA><AR_CONDICIONADO></AR_CONDICIONADO><DIRECAO_HIDRAULICA></DIRECAO_HIDRAULICA><IMPORTADO></IMPORTADO><IMAGE1>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14412-MLB3891251652_022013-F.jpg</IMAGE1><IMAGE2>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14592-MLB3891251080_022013-F.jpg</IMAGE2><IMAGE3>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14595-MLB3891252590_022013-F.jpg</IMAGE3><IMAGE4>http://img.olx.com.br/images/91/910522110043472.jpg</IMAGE4><DATA_LEILAO>16/04/2004</DATA_LEILAO><LEILOEIRO>LUIZ FERNANDO DE ABREU SODRE SANTORO</LEILOEIRO><COMITENTE>N/C</COMITENTE></LEILAOXML><LEILAOXML><LEILAONUMERO>4</LEILAONUMERO><MARCA></MARCA><MODELO>CITROEN XSARA</MODELO><ANO_MODELO>2001</ANO_MODELO><PLACA>IJY2323</PLACA><CHASSI>VF7N1N6AK1J402326</CHASSI><RENAVAM></RENAVAM><COR>PRATA</COR><COMBUSTIVEL></COMBUSTIVEL><UF></UF><CATEGORIA></CATEGORIA><KILOMETRAGEM></KILOMETRAGEM><CONDICAO_VEICULO>Cond. Veiculo: N/C</CONDICAO_VEICULO><CONDICAO_CHASSI>Sit. Chassi: N/C</CONDICAO_CHASSI><CONDICAO_MOTOR>Cond. Motor: N/C</CONDICAO_MOTOR><CONDICAO_CAMBIO>Cond. Cambio: N/C</CONDICAO_CAMBIO><CONDICAO_CARROCERIA></CONDICAO_CARROCERIA><CONDICAO_MECANICA>Sit. Chassi: N/C EIXO TRAZEIRO: LOTE:0094 VEICULO:</CONDICAO_MECANICA><AR_CONDICIONADO></AR_CONDICIONADO><DIRECAO_HIDRAULICA></DIRECAO_HIDRAULICA><IMPORTADO></IMPORTADO><IMAGE1>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14412-MLB3891251652_022013-F.jpg</IMAGE1><IMAGE2>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14592-MLB3891251080_022013-F.jpg</IMAGE2><IMAGE3>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14595-MLB3891252590_022013-F.jpg</IMAGE3><IMAGE4>http://img.olx.com.br/images/91/910522110043472.jpg</IMAGE4><DATA_LEILAO>16/04/2004 </DATA_LEILAO><LEILOEIRO>LUIZ FERNANDO DE ABREU SODRE SANTORO</LEILOEIRO><COMITENTE></COMITENTE></LEILAOXML><LEILAOXML><LEILAONUMERO>5</LEILAONUMERO><MARCA></MARCA><MODELO>CITROEN XSARA GLX 1.6</MODELO><ANO_MODELO>2001</ANO_MODELO><PLACA>IJY2323</PLACA><CHASSI>VF7N1N6AK1J402326</CHASSI><RENAVAM></RENAVAM><COR>PRATA</COR><COMBUSTIVEL></COMBUSTIVEL><UF></UF><CATEGORIA></CATEGORIA><KILOMETRAGEM></KILOMETRAGEM><CONDICAO_VEICULO>Cond. Veiculo: Analisar Avarias</CONDICAO_VEICULO><CONDICAO_CHASSI>Sit. Chassi: N/C</CONDICAO_CHASSI><CONDICAO_MOTOR>Cond. Motor: N/C</CONDICAO_MOTOR><CONDICAO_CAMBIO>Cond. Cambio: N/C</CONDICAO_CAMBIO><CONDICAO_CARROCERIA></CONDICAO_CARROCERIA><CONDICAO_MECANICA>Sit. Chassi: N/C EIXO TRAZEIRO: LOTE:N/C VEICULO:</CONDICAO_MECANICA><AR_CONDICIONADO></AR_CONDICIONADO><DIRECAO_HIDRAULICA></DIRECAO_HIDRAULICA><IMPORTADO></IMPORTADO><IMAGE1>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14412-MLB3891251652_022013-F.jpg</IMAGE1><IMAGE2>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14592-MLB3891251080_022013-F.jpg</IMAGE2><IMAGE3>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14595-MLB3891252590_022013-F.jpg</IMAGE3><IMAGE4>http://img.olx.com.br/images/91/910522110043472.jpg</IMAGE4><DATA_LEILAO>23/04/2004</DATA_LEILAO><LEILOEIRO>LUIZ FERNANDO DE ABREU SODRE SANTORO</LEILOEIRO><COMITENTE>N/C</COMITENTE></LEILAOXML><LEILAOXML><LEILAONUMERO>6</LEILAONUMERO><MARCA></MARCA><MODELO>CITROEN XSARA</MODELO><ANO_MODELO>2001</ANO_MODELO><PLACA>IJY2323</PLACA><CHASSI>VF7N1N6AK1J402326</CHASSI><RENAVAM></RENAVAM><COR>PRATA</COR><COMBUSTIVEL></COMBUSTIVEL><UF></UF><CATEGORIA></CATEGORIA><KILOMETRAGEM></KILOMETRAGEM><CONDICAO_VEICULO>Cond. Veiculo: N/C</CONDICAO_VEICULO><CONDICAO_CHASSI>Sit. Chassi: N/C</CONDICAO_CHASSI><CONDICAO_MOTOR>Cond. Motor: N/C</CONDICAO_MOTOR><CONDICAO_CAMBIO>Cond. Cambio: N/C</CONDICAO_CAMBIO><CONDICAO_CARROCERIA></CONDICAO_CARROCERIA><CONDICAO_MECANICA>Sit. Chassi: N/C EIXO TRAZEIRO: LOTE:0122 VEICULO:</CONDICAO_MECANICA><AR_CONDICIONADO></AR_CONDICIONADO><DIRECAO_HIDRAULICA></DIRECAO_HIDRAULICA><IMPORTADO></IMPORTADO><IMAGE1>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14412-MLB3891251652_022013-F.jpg</IMAGE1><IMAGE2>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14592-MLB3891251080_022013-F.jpg</IMAGE2><IMAGE3>http://mlb-s1-p.mlstatic.com/citroen-xsara-glx-16-16-v-ano-2002-so-pecas-2-portas-14595-MLB3891252590_022013-F.jpg</IMAGE3><IMAGE4>http://img.olx.com.br/images/91/910522110043472.jpg</IMAGE4><DATA_LEILAO>23/04/2004 </DATA_LEILAO><LEILOEIRO>LUIZ FERNANDO DE ABREU SODRE SANTORO</LEILOEIRO><COMITENTE></COMITENTE></LEILAOXML></RESPOSTA>";
                
                // GRAVA O TEMPO DECORRIDO EM ARQUIVO DE LOG
                try
                {
                    LogEstatico.setLogTitulo("TEMPO LEILAO: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                    LogEstatico.setLogTexto("CHAVE: " + this.logLancamento);
                    LogEstatico.setLogTexto("FORNECEDOR: TDB");
                    LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                    LogEstatico.setLogTexto("TEMPO: " + decorrido.ToString().PadRight(10, ' ') + "|" + "".ToString().PadRight(7, ' '));
                    LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                    LogEstatico.setLogTexto("PLACA: " + this.carro.Placa);
                }
                catch (Exception ex)
                {
                    LogEstatico.setLogTitulo("ERRO LOG CRONOMETRO: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                    LogEstatico.setLogTexto("MENSAGEM: " + ex.Message + ex.StackTrace);
                    this.retornoLeilao.ErroLeilao = new Erros("0", "LEILAO: TIMEOUT: Falha ao acessar fornecedor: " + ex.ToString());
                    return this.retornoLeilao;
                }

                if (String.IsNullOrEmpty(this.retornoWs))
                {
                    this.retornoLeilao.ErroLeilao = new Inteligencia.Erros("0", "TDB: CONSULTA INDISPONIVEL");
                    LogEstatico.setLogTitulo("ERRO LEILAO TDB" + System.DateTime.Now);
                    LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                    LogEstatico.setLogTexto("CHASSI: " + this.carro.Chassi);
                    return this.retornoLeilao;
                }

                this.retornoWs = this.retornoWs.Replace("Cond. Veiculo: ", "").Replace("Sit. Chassi: ", "").Replace("Cond. Motor: ", "").Replace("Cond. Cambio: ", "").Replace("NÃƒO", "NAO").Replace("NÃO", "NAO");
                this.retornoWs = this.retornoWs.Trim();

                XmlDocument arrayResposta = new XmlDocument();
                arrayResposta.LoadXml(this.retornoWs);

                // FALHA NA CONSULTA
                if (this.retornoWs.Contains("ERROR"))
                {
                    this.retornoLeilao.ErroLeilao = new Erros("0", arrayResposta.SelectNodes("/RETORNO/MENSAGEM").Item(0).InnerText.Replace("ERROR:", ""));
                    return this.retornoLeilao;
                }
                // REGISTROS NÃO ENCONTRADOS NA BASES CONSULTADAS
                if (this.retornoWs.Contains("Nenhuma ocorrencia de leilao encontrada"))
                {
                    this.retornoLeilao.MsgLeilao = new Erros("0", "INFORMACAO NAO ENCONTRADA NAS BASES CONSULTADAS");
                    return this.retornoLeilao;
                }

                try
                {
                    this.retornoLeilao.CodFornecedor = "_" + arrayResposta.SelectNodes("/RESPOSTA/RETORNO/CHAVERETORNO").Item(0).InnerText;
                }
                catch
                {
                    this.retornoLeilao.CodFornecedor = "_ERRO";

                    if (!this.retornoWs.Contains("NENHUM REGISTRO ENCONTRADO PARA O DADO INFORMADO"))
                    {
                        LogEstatico.setLogTitulo("TDB LEILAO INDISPONIVEL: " + System.DateTime.Now);
                        LogEstatico.setLogTexto("REQUISICAO: " + this.urlRequisicao);
                        LogEstatico.setLogTexto("RETORNO: " + this.retornoWs);
                        LogEstatico.setLogTexto("PARAMETROS: LOGON: " + this.dadosUsuario.Logon + "; PLACA: " + this.carro.Placa);
                    }
                    this.retornoLeilao.ErroLeilao = new Erros("0", "TDB LEILAO INDISPONIVEL");
                    return this.retornoLeilao;
                }

                // INFORMACAO ENCONTRADA
                this.retornoLeilao.QtdRegistros = arrayResposta.SelectNodes("/RESPOSTA/RETORNO/QTDTOTLEILAO").Item(0).InnerText;
                this.retornoLeilao.MsgLeilao = new Erros("1", "CONSTA REGISTRO DE LEILAO");
                string tempData = string.Empty;

                this.retornoLeilao.HistoricoLeilao = new List<AuxHistoricoLeiloes>();

                for (int i = 0; i < int.Parse(this.retornoLeilao.QtdRegistros); i++)
                {
                    this.tempLeilao = new LeilaoModel();
                    this.tempCarro = new Veiculo();

                    this.tempLeilao.Leiloeiro = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/LEILOEIRO").Item(i).InnerText;
                    //AJUSTE PARA PEGAR O LOTE
                    int indice = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/CONDICAO_MECANICA").Item(i).InnerText.IndexOf("Lote:");
                    this.tempLeilao.Lote = (indice == -1) ? "NAO INFORMADO" : arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/CONDICAO_MECANICA").Item(i).InnerText.Substring(indice,5);
                    this.tempLeilao.IdLeilao = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/LEILAONUMERO").Item(i).InnerText;
                    this.tempCarro.Marca = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/MARCA").Item(i).InnerText;
				    this.tempCarro.Modelo = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/MODELO").Item(i).InnerText;
				    this.tempCarro.AnoModelo = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/ANO_MODELO").Item(i).InnerText;
                    this.tempCarro.Placa = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/PLACA").Item(i).InnerText;
				    this.tempCarro.Chassi = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/CHASSI").Item(i).InnerText;
				    this.tempCarro.Renavam = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/RENAVAM").Item(i).InnerText;
				    this.tempCarro.Cor = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/COR").Item(i).InnerText;
				    this.tempCarro.Combustivel = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/COMBUSTIVEL").Item(i).InnerText;
				    this.tempCarro.Categoria = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/CATEGORIA").Item(i).InnerText;

                    this.tempLeilao.CondicaoGeral = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/CONDICAO_VEICULO").Item(i).InnerText;
				    this.tempLeilao.CondicaoChassi = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/CONDICAO_CHASSI").Item(i).InnerText;
                    //AJUSTE PARA PEGAR O PATIO
                    this.tempLeilao.Patio = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/CONDICAO_MECANICA").Item(i).InnerText;
                    indice = this.tempLeilao.Patio.IndexOf("PATIO:");
                    this.tempLeilao.Patio = indice != -1 ? this.tempLeilao.Patio.Substring(indice) : null;
                    this.tempLeilao.DataLeilao = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/DATA_LEILAO").Item(i).InnerText;
                    this.tempLeilao.Comitente = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/COMITENTE").Item(i).InnerText;

                    switch (arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/IMAGE1").Item(i).InnerText)
                    {
                        case "0.jpg":
                            this.tempLeilao.Avarias = "VEICULO AINDA NAO CLASSIFICACADO";
                            break;

                        case "1.jpg":
                            this.tempLeilao.Avarias = "VEICULO COM PEQUENAS AVARIAS";
                            break;

                        case "2.jpg":
                            this.tempLeilao.Avarias = "VEICULO COM AVARIAS MEDIAS";
                            break;

                        case "3.jpg":
                            this.tempLeilao.Avarias = "VEICULO COM AVARIAS IMPORTANTES";
                            break;

                        default:
                            if (!String.IsNullOrEmpty(arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/CONDICAO_VEICULO").Item(0).InnerText))
                                this.tempLeilao.Avarias = arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/CONDICAO_VEICULO").Item(0).InnerText;
                            else
                                this.tempLeilao.Avarias = "NAO FOI POSSIVEL CLASSIFICAR";
                            break;
                    }

                    this.tempLeilao.Fotos = new List<string>();

                    for (int x = 1; x < 5; x++)
                    {
                        if(!String.IsNullOrEmpty(arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/IMAGE" + x.ToString()).Item(i).InnerText))
                            this.tempLeilao.Fotos.Add(arrayResposta.SelectNodes("/RESPOSTA/LEILAOXML/IMAGE" + x.ToString()).Item(i).InnerText);
                    }
                    // ANULA VALORES EM BRANCO
                    this.tempCarro = Util.unSetDadosVazios<Veiculo>(this.tempCarro);
                    this.tempLeilao = Util.unSetDadosVazios<LeilaoModel>(this.tempLeilao);

                    this.retornoLeilao.HistoricoLeilao.Add(new AuxHistoricoLeiloes(this.tempCarro, this.tempLeilao));
                }

                return this.retornoLeilao;
            }
            catch (Exception ex)
            {
                EnvioEmail SendMail = new EnvioEmail();
                SendMail.EnviaEmail(SendMail.LayoutFornecedor("TDB", "LEILAO", this.carro.Placa + this.carro.Chassi), "CONSULTA INDISPONIVEL", "antonio.carlos@emepar.com.br");

                this.retornoLeilao.ErroLeilao = new Erros("0", "LEILAO: Falha ao acessar fornecedor: " + ex.ToString());
                
                if (ex.Message.Contains("timed out"))
                {
                    try
                    {
                        LogEstatico.setLogTitulo("TEMPO LEILAO: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").PadRight(20, ' '));
                        LogEstatico.setLogTexto("CHAVE: " + this.logLancamento);
                        LogEstatico.setLogTexto("FORNECEDOR: TDB");
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

                LogEstatico.setLogTitulo("ERRO TDB LEILAO: " + System.DateTime.Now);
                LogEstatico.setLogTexto(ex.Message + ex.StackTrace);
                LogEstatico.setLogTexto("PARAMETROS: LOGON : " + this.dadosUsuario.Logon + "; PLACA: " + this.carro.Placa);
            }

            return this.retornoLeilao;
        }
        
        // MONTA A URL PARA OBTER O XML DO FORNECEDOR
        public void setUrlRequisicao()
        {
            this.urlRequisicao = "http://52.3.169.105/leilao.php?txtusuario=" + this.acesLogin + "&txtsenha=" + this.acesPassword + "&txtfornecedor=" + this.tipoBaseLeilao;
            //PLACA
            this.urlRequisicao += (String.IsNullOrEmpty(this.carro.Placa)) ? string.Empty : "&txtplaca=" + this.carro.Placa;
            //CHASSI
            this.urlRequisicao += (String.IsNullOrEmpty(this.carro.Chassi)) ? string.Empty : "&txtchassi=" + this.carro.Chassi;
        }
    }
}