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

namespace webServiceCheckOk.Controle.Fornecedores
{
    public class FornBoaVista
    {
        public string codConsulta { get; set; }
        public string tipoConsulta { get; set; }
        public string urlRequisicao { get; set; }
        private string retornoWs = string.Empty;
        private string acesLogin = string.Empty;
        private string acesPassword = string.Empty;
        
        private GravameModel boaVistaGravame;
        private LeilaoModel boaVistaLeilao;
        private Veiculo carro;
        private List<Veiculo> dadosCarro { get; set; }
        private List<GravameModel> dadosGravame { get; set; }

        // CONSTRUTOR
        public FornBoaVista(Veiculo carro)
        {
            this.carro = carro;
            this.acesLogin = "00404925";
            this.acesPassword = "Y34K5I";
        }
        
        // GRAVAME RESPOSTA
        public GravameModel getGravame()
        {
            try
            {
                if (this.carro.Chassi.Trim().Length == 0)
                {
                    this.boaVistaGravame.ErroGravame = new Inteligencia.Erros("CHASSI NÃO INFORMADO");
                    return this.boaVistaGravame;
                }
                // MONTA A URL DE REQUISIÇÃO
                setUrlRequisicao();

                // FAZ A CONSULTA NO FORNECEDOR
                this.retornoWs = new CustomTimeOut(40000).DownloadString(urlRequisicao);
                
                if (String.IsNullOrEmpty(this.retornoWs))
                {
                    this.boaVistaGravame.ErroGravame = new Inteligencia.Erros("0", "BV: CONSULTA INDISPONIVEL");
                    
                    LogEstatico.setLogTitulo("ERRO GRAVAME BOAVISTA" + System.DateTime.Now);
                    LogEstatico.setLogTexto("requisicao : " + urlRequisicao);
                    LogEstatico.setLogTexto("chassi : " + this.carro.Chassi);
                  
                    return this.boaVistaGravame;
                }

                // XML PADRÃO SEAPE
                //this.retorno = "<?xml version=\"1.0\"?>\n<!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for SEAPE SERVIÇO DE APOIO AO EMPRESARIO LTDA-->\n<Consulta xmlns:xs=\"http://www.w3.org/2001/XMLSchema-instance\">\n\t<ObjHeader>\n\t\t<ID_CONSULTA>270120123095306</ID_CONSULTA>\n\t\t<LOGON_CONSULTA>108289</LOGON_CONSULTA>\n\t\t<DATA_CONSULTA>27/01/2012 14:16</DATA_CONSULTA>\n\t\t<EXISTE_ERRO>0</EXISTE_ERRO>\n\t\t<MSG_ERRO/>\n\t\t<PARAMETROS>\n\t\t\t<PLACA/>\n\t\t\t<CHASSI>9BFZZZ54ZRB561921</CHASSI>\n\t\t\t<MOTOR/>\n\t\t\t<CAMBIO/>\n\t\t\t<RENAVAM/>\n\t\t\t<UF/>\n\t\t\t<CPF_CNPJ/>\n\t\t</PARAMETROS>\n\t</ObjHeader>\n\t<ObjSNG>\n\t\t<EXISTE_ERRO>0</EXISTE_ERRO>\n\t\t<MSG_ERRO>TRANSACAO EFETUADA COM SUCESSO.</MSG_ERRO>\n\t\t<CODIGO_RETORNO_EXECUCAO_SNG>01</CODIGO_RETORNO_EXECUCAO_SNG>\n\t\t<CHASSI_SNG>9BFZZZ54ZRB561921</CHASSI_SNG>\n\t\t<FINANCIAMENTO>\n\t\t\t<Financiamento>\n\t\t\t\t<CHASSI/>\n\t\t\t\t<NUMERO_RESTRINCAO/>\n\t\t\t\t<CODIGO_AGENTE/>\n\t\t\t\t<ASSINATURA/>\n\t\t\t\t<CONTRATO/>\n\t\t\t\t<INFORMANTE/>\n\t\t\t\t<ANO_MODELO/>\n\t\t\t\t<TIPO_RESTRICAO/>\n\t\t\t\t<REMARCACAO_CHASSI>N</REMARCACAO_CHASSI>\n\t\t\t\t<RENAVAM_SNG>621673226</RENAVAM_SNG>\n\t\t\t\t<STATUS_VEICULO>ALIENACAO FIDUCIARIA BAIXADO PELA FINANCEIRA</STATUS_VEICULO>\n\t\t\t\t<UF_PLACA>SP</UF_PLACA>\n\t\t\t\t<PLACA>CBQ2456</PLACA>\n\t\t\t\t<UF_LICENCIAMENTO>SP</UF_LICENCIAMENTO>\n\t\t\t\t<CPF_CGC_FINANCIADO>88319822572</CPF_CGC_FINANCIADO>\n\t\t\t\t<NOME_FINANCIADO>JOSIVAN MIRANDA SILVA</NOME_FINANCIADO>\n\t\t\t\t<NOME_AGENTE>BANCO INTERCAP S A</NOME_AGENTE>\n\t\t\t\t<CGC_AGENTE>58497702000102</CGC_AGENTE>\n\t\t\t\t<DATA_INCLUSAO_GRAVAME>18/08/2006</DATA_INCLUSAO_GRAVAME>\n\t\t\t\t<DADOS_AGENTE>\n\t\t\t\t\t<EXISTE_DADOS>0</EXISTE_DADOS>\n\t\t\t\t\t<CPF_CNPJ/>\n\t\t\t\t\t<TELEFONE/>\n\t\t\t\t\t<NOME/>\n\t\t\t\t\t<TP_PESSOA/>\n\t\t\t\t\t<LOGRADOURO/>\n\t\t\t\t\t<BAIRRO/>\n\t\t\t\t\t<CIDADE/>\n\t\t\t\t\t<UF/>\n\t\t\t\t</DADOS_AGENTE>\n\t\t\t\t<DADOS_FINANCIADO>\n\t\t\t\t\t<EXISTE_DADOS>0</EXISTE_DADOS>\n\t\t\t\t\t<CPF_CNPJ/>\n\t\t\t\t\t<TELEFONE/>\n\t\t\t\t\t<NOME/>\n\t\t\t\t\t<TP_PESSOA/>\n\t\t\t\t\t<LOGRADOURO/>\n\t\t\t\t\t<BAIRRO/>\n\t\t\t\t\t<CIDADE/>\n\t\t\t\t\t<UF/>\n\t\t\t\t</DADOS_FINANCIADO>\n\t\t\t</Financiamento>\n\t\t\t<Financiamento>\n\t\t\t\t<CHASSI/>\n\t\t\t\t<NUMERO_RESTRINCAO/>\n\t\t\t\t<CODIGO_AGENTE/>\n\t\t\t\t<ASSINATURA/>\n\t\t\t\t<CONTRATO/>\n\t\t\t\t<INFORMANTE/>\n\t\t\t\t<ANO_MODELO/>\n\t\t\t\t<TIPO_RESTRICAO/>\n\t\t\t\t<REMARCACAO_CHASSI>N</REMARCACAO_CHASSI>\n\t\t\t\t<RENAVAM_SNG>621673226</RENAVAM_SNG>\n\t\t\t\t<STATUS_VEICULO>ALIENACAO FIDUCIARIA BAIXADO PELA FINANCEIRA</STATUS_VEICULO>\n\t\t\t\t<UF_PLACA>SP</UF_PLACA>\n\t\t\t\t<PLACA>CBQ2456</PLACA>\n\t\t\t\t<UF_LICENCIAMENTO>SP</UF_LICENCIAMENTO>\n\t\t\t\t<CPF_CGC_FINANCIADO>07662037860</CPF_CGC_FINANCIADO>\n\t\t\t\t<NOME_FINANCIADO>NEILOR ALVES DE OLIVEIRA</NOME_FINANCIADO>\n\t\t\t\t<NOME_AGENTE>HSBC BANK BRASIL S A   BCO MULTIPLO</NOME_AGENTE>\n\t\t\t\t<CGC_AGENTE>01701201000189</CGC_AGENTE>\n\t\t\t\t<DATA_INCLUSAO_GRAVAME>04/06/2009</DATA_INCLUSAO_GRAVAME>\n\t\t\t\t<DADOS_AGENTE>\n\t\t\t\t\t<EXISTE_DADOS>0</EXISTE_DADOS>\n\t\t\t\t\t<CPF_CNPJ/>\n\t\t\t\t\t<TELEFONE/>\n\t\t\t\t\t<NOME/>\n\t\t\t\t\t<TP_PESSOA/>\n\t\t\t\t\t<LOGRADOURO/>\n\t\t\t\t\t<BAIRRO/>\n\t\t\t\t\t<CIDADE/>\n\t\t\t\t\t<UF/>\n\t\t\t\t</DADOS_AGENTE>\n\t\t\t\t<DADOS_FINANCIADO>\n\t\t\t\t\t<EXISTE_DADOS>0</EXISTE_DADOS>\n\t\t\t\t\t<CPF_CNPJ/>\n\t\t\t\t\t<TELEFONE/>\n\t\t\t\t\t<NOME/>\n\t\t\t\t\t<TP_PESSOA/>\n\t\t\t\t\t<LOGRADOURO/>\n\t\t\t\t\t<BAIRRO/>\n\t\t\t\t\t<CIDADE/>\n\t\t\t\t\t<UF/>\n\t\t\t\t</DADOS_FINANCIADO>\n\t\t\t</Financiamento>\n\t\t\t<Financiamento>\n\t\t\t\t<CHASSI/>\n\t\t\t\t<NUMERO_RESTRINCAO/>\n\t\t\t\t<CODIGO_AGENTE/>\n\t\t\t\t<ASSINATURA/>\n\t\t\t\t<CONTRATO/>\n\t\t\t\t<INFORMANTE/>\n\t\t\t\t<ANO_MODELO/>\n\t\t\t\t<TIPO_RESTRICAO/>\n\t\t\t\t<REMARCACAO_CHASSI/>\n\t\t\t\t<RENAVAM_SNG/>\n\t\t\t\t<STATUS_VEICULO/>\n\t\t\t\t<UF_PLACA/>\n\t\t\t\t<PLACA/>\n\t\t\t\t<UF_LICENCIAMENTO/>\n\t\t\t\t<CPF_CGC_FINANCIADO/>\n\t\t\t\t<NOME_FINANCIADO/>\n\t\t\t\t<NOME_AGENTE/>\n\t\t\t\t<CGC_AGENTE/>\n\t\t\t\t<DATA_INCLUSAO_GRAVAME/>\n\t\t\t</Financiamento>\n\t\t</FINANCIAMENTO>\n\t</ObjSNG>\n</Consulta>\n";

                // SEPARA O RESULTADO
                this.retornoWs = this.retornoWs.Replace("&", " E ");

                XmlDocument arrayResposta = new XmlDocument();
                arrayResposta.LoadXml(this.retornoWs);
                try
                {
                    this.boaVistaGravame.CodFornecedor = "_" + arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(61, 8).PadLeft(10, '0');
                }
                catch
                {
                    this.boaVistaGravame.CodFornecedor = "_ERRO";
                }

                // INFOMACAO NAO ENCONTRADA NAS BASES CONSULTADAS
                if ((arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(72, 3) == "138") || (arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(72, 3) == "098"))
                {
                    this.boaVistaGravame.MsgGravame = new Erros("2", "INFORMACAO NAO ENCONTRADA NAS BASES CONSULTADAS");
                    return this.boaVistaGravame;
                }

                // INFORMACAO ENCONTRADA
                if ((arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(72, 3) == "655") || (arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(72, 3) == "661"))
                {
                    this.boaVistaGravame.MsgGravame = new Erros("1", "CONSTA REGISTRO DE GRAVAME PARA O VEICULO INFORMADO");
                    string documentoAux = string.Empty;
                    Veiculo dadosCarro;
                    GravameModel dadosGravame;
                    int x = 0;
                    // GRAVAMES 1 (103) 2 (314) 3 (525)
                    for(int i = 0; i <= 2; i++)
                    {
                        if (arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(103 + x, 1) == "S" || arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(103 + x, 1) == "N")
                        {
                            dadosCarro = new Veiculo();
                            dadosGravame = new GravameModel();

                            dadosCarro.Renavam = arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(104 + x, 11);
                            dadosCarro.Placa = arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(177 + x, 7);
                            dadosGravame.RemarcacaoChassi = arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(103 + x, 1);
                            dadosGravame.StatusVeiculo = arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(115 + x, 60).Trim();
                            dadosGravame.UFPlaca = arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(175 + x, 2);
                            dadosGravame.UFLicenciamento = arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(184 + x, 2);
                            dadosGravame.NomeFinanciado = arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(206 + x, 40).Trim();
                            dadosGravame.NomeAgente = arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(246 + x, 40).Trim();
                            dadosGravame.DataInclusaoGravame = arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(306 + x, 2) + "/" + arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(308 + x, 2) + "/" + arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(310 + x, 4);
                            // trata DocumentoFinanciado
                            documentoAux = arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(186 + x, 20).Trim().Replace("CPF", "").Replace("CNPJ", "");
                            dadosGravame.DocumentoFinanciado = (documentoAux.Length > 11 ? "CNPJ" : "CPF") + documentoAux;
                            // trata DocumentoAgente
                            documentoAux = arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(286 + x, 20).Trim().Replace("CPF", "").Replace("CNPJ", "");
                            dadosGravame.DocumentoAgente = (documentoAux.Length > 11 ? "CNPJ" : "CPF") + documentoAux;

                            this.boaVistaGravame.HistoricoGravames.Add(new AuxHistoricoGravames(dadosCarro, dadosGravame));
                        }
                        x += 211;
                    }
                    return this.boaVistaGravame;
                }
            }
            catch (Exception e)
            {
                this.boaVistaGravame.ErroGravame = new Erros("0", "Falha ao acessar fornecedor: " + e.ToString());
            }

            return this.boaVistaGravame;
        }
        
        // LEILAO RESPOSTA
        public LeilaoModel getLeilao()
        {
            try
            {
                if (this.carro.Chassi.Trim().Length == 0)
                {
                    this.boaVistaLeilao.ErroLeilao = new Inteligencia.Erros("CHASSI NÃO INFORMADO");
                    return this.boaVistaLeilao;
                }

                // FAZ A CONSULTA NO FORNECEDOR
                setUrlRequisicao();

                this.retornoWs = new CustomTimeOut(40000).DownloadString(this.urlRequisicao);

                if (String.IsNullOrEmpty(this.retornoWs))
                {
                    this.boaVistaLeilao.ErroLeilao = new Inteligencia.Erros("0", "BV: CONSULTA INDISPONIVEL");

                    LogEstatico.setLogTitulo("ERRO LEILAO BOAVISTA" + System.DateTime.Now);
                    LogEstatico.setLogTexto("requisicao : " + this.urlRequisicao);
                    LogEstatico.setLogTexto("chassi : " + this.carro.Chassi);

                    return this.boaVistaLeilao;
                }
                
                this.retornoWs = this.retornoWs.Trim();

                XmlDocument arrayResposta = new XmlDocument();
                arrayResposta.LoadXml(this.retornoWs);

                this.retornoWs = this.retornoWs.Replace("Cond. Veiculo: ", "").Replace("Sit. Chassi: ", "").Replace("Cond. Motor: ", "").Replace("Cond. Cambio: ", "").Replace("NÃƒO", "NAO").Replace("NÃO", "NAO").Replace("N¿¿?¿¿?O", "NAO").Replace("NÃƒÂƒO", "NAO");

                try
                {
                    this.boaVistaLeilao.CodFornecedor = "_" + arrayResposta.SelectNodes("/PRE").Item(0).InnerText.Substring(61, 8).PadLeft(10, '0');
                }
                catch
                {
                    this.boaVistaLeilao.CodFornecedor = "_ERRO";
                }

                this.retornoWs = this.retornoWs.Remove(0, 77);

                // INFOMACAO NAO ENCONTRADA NAS BASES CONSULTADAS
                if (this.retornoWs.Substring(6, 1).Trim() == "N")
                {
                    this.boaVistaLeilao.MsgLeilao = new Erros("2", "NENHUMA OCORRENCIA ENCONTRADA NAS BASES CONSULTADAS");
                    return this.boaVistaLeilao;
                }

                // INFORMACAO ENCONTRADA
                int qtdOcorrencias =  Int32.Parse(this.retornoWs.Substring(18, 1).Trim());
                this.boaVistaLeilao.MsgLeilao = new Erros("1", "CONSTA " + qtdOcorrencias + " REGISTRO(S) DE LEILAO PARA O VEICULO INFORMADO");
                
                string documentoAux = string.Empty;
                string registroLeilao = string.Empty;

                Veiculo carroLeilao;
                LeilaoModel dadosLeilao;

                // INSERINDO OCORRENCIAS DE LEILAO AO RETORNO
                for (int i = 0; i < qtdOcorrencias; i++)
                {
                    int valorCorte = this.retornoWs.IndexOf("741770", 5);
                    
                    if (valorCorte != -1)
                        registroLeilao = this.retornoWs.Substring(0, valorCorte);
                    else
                        registroLeilao = this.retornoWs;
                    
                    carroLeilao = new Veiculo();
                    dadosLeilao = new LeilaoModel();

                    // LEILOEIRO                  
                    if (registroLeilao.Substring(584, 50).Trim() != string.Empty)
                        dadosLeilao.Leiloeiro = registroLeilao.Substring(584, 50);
                    // ID LEILAO
                    if (registroLeilao.Substring(28, 9).Trim() != string.Empty)
                        dadosLeilao.IdLeilao = registroLeilao.Substring(28, 9);
                    // QT REGISTROS
                    if (registroLeilao.Substring(10, 9).Trim() != string.Empty)
                        dadosLeilao.QtdRegistros = registroLeilao.Substring(10, 9);
                    // ID VEICULO
                    if (registroLeilao.Substring(19, 9).Trim() != string.Empty)
                        dadosLeilao.IdVeiculo = registroLeilao.Substring(19, 9);
                    // CONDICAO GERAL
                    if (registroLeilao.Substring(355, 20).Trim() != string.Empty)
                        dadosLeilao.CondicaoGeral = registroLeilao.Substring(355, 20);
                    // CONDICAO CHASSI
                    if (registroLeilao.Substring(375, 20).Trim() != string.Empty)
                        dadosLeilao.CondicaoChassi = registroLeilao.Substring(375, 20);
                    // PATIO
                    if (registroLeilao.Substring(484, 100).Trim() != string.Empty)
                        dadosLeilao.Patio = registroLeilao.Substring(484, 100);
                    // DATA LEILAO
                    if (registroLeilao.Substring(634, 10).Trim() != string.Empty)
                        dadosLeilao.DataLeilao = registroLeilao.Substring(634, 10);
                    // COMITENTE
                    if (registroLeilao.Substring(644, 100).Trim() != string.Empty)
                        dadosLeilao.Comitente = registroLeilao.Substring(644, 100);
                    // CLASSIFICACAO
                    if (registroLeilao.Substring(335, 20).Trim() != string.Empty)
                        dadosLeilao.Classificacao = registroLeilao.Substring(335, 20);
                    // OBSERVACAO
                    if (registroLeilao.Substring(37, 20).Trim() != string.Empty)
                        dadosLeilao.Observacao = registroLeilao.Substring(37, 20);

                    // RENAVAM
                    if (registroLeilao.Substring(214, 11).Trim() != string.Empty)
                        carroLeilao.Renavam = registroLeilao.Substring(214, 11);
                    // PLACA
                    if (registroLeilao.Substring(190, 7).Trim() != string.Empty)
                        carroLeilao.Placa = registroLeilao.Substring(190, 7);
                    // MARCA
                    if (registroLeilao.Substring(57, 50).Trim() != string.Empty)
                        carroLeilao.Marca = registroLeilao.Substring(57, 50);
                    // MODELO
                    if (registroLeilao.Substring(107, 75).Trim() != string.Empty)
                        carroLeilao.Modelo = registroLeilao.Substring(107, 75);
                    // ANO MODELO
                    if (registroLeilao.Substring(182, 4).Trim() != string.Empty)
                        carroLeilao.AnoModelo = registroLeilao.Substring(182, 4);
                    // ANO FABRICACAO
                    if (registroLeilao.Substring(186, 4).Trim() != string.Empty)
                        carroLeilao.AnoFabrica = registroLeilao.Substring(186, 4);
                    // CHASSI
                    if (registroLeilao.Substring(197, 17).Trim() != string.Empty)
                        carroLeilao.Chassi = registroLeilao.Substring(197, 17);
                    // COR
                    if (registroLeilao.Substring(255, 50).Trim() != string.Empty)
                        carroLeilao.Cor = registroLeilao.Substring(225, 50);
                    // COMBUSTIVEL
                    if (registroLeilao.Substring(275, 30).Trim() != string.Empty)
                        carroLeilao.Combustivel = registroLeilao.Substring(275, 30);
                    // CATEGORIA
                    if (registroLeilao.Substring(305, 50).Trim() != string.Empty)
                        carroLeilao.Categoria = registroLeilao.Substring(305, 50);
                    // NR MOTOR
                    if (registroLeilao.Substring(395, 20).Trim() != string.Empty)
                        carroLeilao.NrMotor = registroLeilao.Substring(395, 20);
                    // NR CAMBIO
                    if (registroLeilao.Substring(751, 100).Trim() != string.Empty)
                        carroLeilao.NrCambio = registroLeilao.Substring(415, 20);
                    // NR CARROCERIA
                    if (registroLeilao.Substring(435, 20).Trim() != string.Empty)
                        carroLeilao.NrCarroceria = registroLeilao.Substring(435, 20);
                    // NR EIXO TRASEIRO
                    if (registroLeilao.Substring(455, 20).Trim() != string.Empty)
                        carroLeilao.NrEixoTraseiro = registroLeilao.Substring(455, 20);
                    // QT EIXOS
                    if (registroLeilao.Substring(475, 9).Trim() != string.Empty)
                        carroLeilao.QtdEixos = (registroLeilao.Substring(475, 9)).Trim();
                    // FOTOS DO VEICULO
                    if (registroLeilao.Substring(751, 100).Trim() != string.Empty)
                        dadosLeilao.Fotos.Add(registroLeilao.Substring(751, 100));

                    if (registroLeilao.Substring(851, 100).Trim() != string.Empty)
                        dadosLeilao.Fotos.Add(registroLeilao.Substring(851, 100));
                    
                    if (registroLeilao.Substring(951, 100).Trim() != string.Empty)
                        dadosLeilao.Fotos.Add(registroLeilao.Substring(951, 100));
                    
                    if (registroLeilao.Substring(1051, 100).Trim() != string.Empty)
                        dadosLeilao.Fotos.Add(registroLeilao.Substring(1051, 100));

                    this.boaVistaLeilao.HistoricoLeilao.Add(new AuxHistoricoLeiloes(carroLeilao, dadosLeilao));

                }
                return this.boaVistaLeilao;
            }
            catch (Exception e)
            {
                this.boaVistaLeilao.ErroLeilao = new Erros("0", "Leilao: Falha ao acessar fornecedor: " + e.ToString());
            }

            return this.boaVistaLeilao;
        }
        
        // MONTA A URL PARA OBTER O XML DO FORNECEDOR
        public void setUrlRequisicao()
        {
            string urlAuxiliar = string.Empty;
            this.urlRequisicao = "https://www.bvsnet.com.br/cgi-bin/db2www/netpo028.mbr/string?consulta=CSR60   V1000000000000000000000000000000" + this.acesLogin + this.acesPassword + "  MODULAR 011C999";

            if (!String.IsNullOrEmpty(this.carro.Placa))
            {
                // PLACA
                this.urlRequisicao += this.carro.Placa + "              ";
                this.tipoConsulta = "2";
            }
            else if (!String.IsNullOrEmpty(this.carro.Chassi))
            {
                //CHASSI
                this.urlRequisicao += this.carro.Chassi + "    ";
                this.tipoConsulta = "1";
            }

            switch (this.codConsulta)
            {
                //SINISTRO
                case "944":
                    urlAuxiliar = @"                  944                     ";
                    break;
                //GRAVAME
                case "905":
                    urlAuxiliar = @"               905                        ";
                    break;
                //DPVAT
                case "904":
                    urlAuxiliar = @"            904                           ";
                    break;
                //BIN + RF PLACA 
                case "907":
                    urlAuxiliar = @"         907                              ";
                    break;
                //BIN + RF CHASSI 
                case "908":
                    urlAuxiliar = @"         908                              ";
                    break;
                // LEILAO CHASSI
                case "780":
                    urlAuxiliar = @"                           780            ";
                    break;
                // LEILAO PLACA
                case "770":
                    urlAuxiliar = @"                           770            ";
                    break;
                default:
                    urlAuxiliar = @"                                          ";
                    break;
            }
            urlAuxiliar += @"X""0D""";
            this.urlRequisicao += " 000000000000000  000000000000000000000" + this.tipoConsulta + urlAuxiliar;
        }
    }
}