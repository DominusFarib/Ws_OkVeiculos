using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;
using webServiceCheckOk.Controle.Inteligencia;
using webServiceCheckOk.Controle.Inteligencia.Utils;
using webServiceCheckOk.Model;
using webServiceCheckOk.Model.ProdutosModel;

namespace webServiceCheckOk.Controle.Fornecedores
{
    public class FornSinaliza
    {
        private GravameModel dadosGravame;
        private Veiculo carro;
        private UsuarioModel usuario;
        private string acesToken = string.Empty;
        private string acesLogin = string.Empty;
        private string acesPassword = string.Empty;
        private string codintuf = string.Empty;
        private string retornoWs = string.Empty;
        private string idConsulta { get; set; }
        private string codConsulta { get; set; }

        // CONSTRUTOR
        public FornSinaliza(Veiculo carro, UsuarioModel usuario, string codConsulta)
        {
            this.carro = carro;
            this.usuario = usuario;
            this.codConsulta = codConsulta;
            this.acesToken = "LSCQRZ3TMXS";
            this.acesLogin = "webcredi";
            this.acesPassword = "mudar123"; // "TE#08WEB";
        }
        // GRAVAME RESPOSTA
        public GravameModel getGravame()
        {
            GravameModel tempGravame;

            try
            {
                WsSinaliza.ConsultasVeiculos wsSinaliza = new WsSinaliza.ConsultasVeiculos();
                WsSinaliza.AuthHeader autSinaliza = new WsSinaliza.AuthHeader();

                autSinaliza.Usuario = this.acesLogin;
                autSinaliza.Senha = this.acesPassword;
                wsSinaliza.AuthHeaderValue = autSinaliza;

                // REALIZA A CONSULTA O FORNECEDOR
                /*
                var relogio = Stopwatch.StartNew();
                var retornoWs = wsSinaliza.ConsultaSinaliza(int.Parse(this.codConsulta), this.carro.Placa, this.carro.Chassi, this.carro.Uf, int.Parse(String.IsNullOrEmpty(this.carro.CodMunicipio) ? "0" : this.carro.CodMunicipio));
                relogio.Stop();
                var elapsedMs = relogio.ElapsedMilliseconds;
                */
                // RETORNO MODELO
                var retornoWs = @"<PRODUTO><HEADER><DESC_PRODUTO /> <COD_RETORNO>000</COD_RETORNO> <MSGERRO /> </HEADER><DETALHES><SERVICO><COD_SERVICO>WS062</COD_SERVICO> <CONSULTA><RETORNO><CHASSI>9BWCA41JX14024928</CHASSI> <PLACA /> <COD_RETORNO>000</COD_RETORNO> <MSG_RETORNO>000</MSG_RETORNO> <CHAVE_PESQUISA>0</CHAVE_PESQUISA> </RETORNO><RESPOSTA><GRAVAME><FINANCIADO><Nome>VERA LUCIA LINS DE ARAUJO</Nome> <Documento>8075764846</Documento> </FINANCIADO><VEICULO><Placa>DDV0022</Placa> <Chassi>9BWCA41JX14024928</Chassi> <Renavam>746606508</Renavam> <Ano_Fabricacao /> <Ano_Modelo>2001</Ano_Modelo> <UF>SP</UF> <Situacao>Veiculo teve gravame c</Situacao> <Data>28/03/2003</Data> <Hora>13:38:35</Hora> </VEICULO><CONTRATO><Num_Gravame>6633184</Num_Gravame> <Informante_Restr>FINANC</Informante_Restr> <UF_Gravame>SP</UF_Gravame> <Nome_Agente>PONTA ADMINISTRADORA DE C</Nome_Agente> <CNPJ_Agente>165510610001-87</CNPJ_Agente> <Cod_Agente>2995</Cod_Agente> <Num_Contrato>00477978</Num_Contrato> <Data_Contrato>17/01/2003</Data_Contrato> <Ass_Eletronica>NHPV562WG1UEXJN1K4JQ4</Ass_Eletronica> <Reg_Contr /> </CONTRATO></GRAVAME><GRAVAME><FINANCIADO><Nome>ANDRE MARIO MALAFAIA DE ANDRADE</Nome> <Documento>58870318591</Documento> </FINANCIADO><VEICULO><Placa>DDV0022</Placa> <Chassi>9BWCA41JX14024928</Chassi> <Renavam>746606508</Renavam> <Ano_Fabricacao /> <Ano_Modelo>2001</Ano_Modelo> <UF>DF</UF> <Situacao>Veiculo teve gravame b</Situacao> <Data>31/10/2008</Data> <Hora>13:06:06</Hora> </VEICULO><CONTRATO><Num_Gravame>992417</Num_Gravame> <Informante_Restr>FINANC</Informante_Restr> <UF_Gravame>DF</UF_Gravame> <Nome_Agente>BANCO FINASA SA</Nome_Agente> <CNPJ_Agente>575616150001-04</CNPJ_Agente> <Cod_Agente>2040</Cod_Agente> <Num_Contrato>3643731414</Num_Contrato> <Data_Contrato>08/06/2006</Data_Contrato> <Ass_Eletronica>JOGZ4ZQJ01ONXR3XRF58C</Ass_Eletronica> <Reg_Contr>1</Reg_Contr> </CONTRATO></GRAVAME><GRAVAME><FINANCIADO><Nome>ANDERSON SABOYA RAMOS DOS SANTOS</Nome> <Documento>87418592191</Documento> </FINANCIADO><VEICULO><Placa>DDV0022</Placa> <Chassi>9BWCA41JX14024928</Chassi> <Renavam>746606508</Renavam> <Ano_Fabricacao /> <Ano_Modelo>2001</Ano_Modelo> <UF>DF</UF> <Situacao>Veiculo teve gravame c</Situacao> <Data>29/09/2009</Data> <Hora>08:30:25</Hora> </VEICULO><CONTRATO><Num_Gravame>1618805</Num_Gravame> <Informante_Restr>FINANC</Informante_Restr> <UF_Gravame>DF</UF_Gravame> <Nome_Agente>BV FINANCEIRA S A C F I</Nome_Agente> <CNPJ_Agente>011499530001-89</CNPJ_Agente> <Cod_Agente>2266</Cod_Agente> <Num_Contrato>570212783</Num_Contrato> <Data_Contrato>01/08/2008</Data_Contrato> <Ass_Eletronica>B4OAML0791QJAJJF5IWMR</Ass_Eletronica> <Reg_Contr>1</Reg_Contr> </CONTRATO></GRAVAME><GRAVAME><FINANCIADO><Nome>ELMA PEREIRA DE ARAUJO PENA</Nome> <Documento>92557341134</Documento> </FINANCIADO><VEICULO><Placa>DDV0022</Placa> <Chassi>9BWCA41JX14024928</Chassi> <Renavam>746606508</Renavam> <Ano_Fabricacao /> <Ano_Modelo>2001</Ano_Modelo> <UF>GO</UF> <Situacao>Veiculo teve gravame b</Situacao> <Data>06/05/2011</Data> <Hora>08:03:02</Hora> </VEICULO><CONTRATO><Num_Gravame>2750392</Num_Gravame> <Informante_Restr>FINANC</Informante_Restr> <UF_Gravame>GO</UF_Gravame> <Nome_Agente>BANCO FINASA BMC S A</Nome_Agente> <CNPJ_Agente>072079960001-50</CNPJ_Agente> <Cod_Agente>81653</Cod_Agente> <Num_Contrato>558100314239282562</Num_Contrato> <Data_Contrato>08/01/2010</Data_Contrato> <Ass_Eletronica>128M482M7TR2CFCVDI7TB</Ass_Eletronica> <Reg_Contr>2</Reg_Contr> </CONTRATO></GRAVAME></RESPOSTA></CONSULTA></SERVICO></DETALHES></PRODUTO>";
                
                // ERRO NO FORNECEDOR
                if (String.IsNullOrEmpty(retornoWs))
                {
                    this.dadosGravame.ErroGravame = new Erros("0", "ERR SINALIZA: RETORNO VAZIO");
                    return this.dadosGravame;
                }

                XmlDocument respostaWs = new XmlDocument();
                respostaWs.LoadXml(retornoWs);
                this.dadosGravame = new GravameModel();

                // INFOMACAO NAO ENCONTRADA NAS BASES CONSULTADAS
                if (retornoWs.Contains("<COD_RETORNO>111</COD_RETORNO>"))
                {
                    this.dadosGravame.MsgGravame = new Erros("2", "SINALIZA: INFORMACAO NAO ENCONTRADA NAS BASES CONSULTADAS");
                    return this.dadosGravame;
                }

                if (retornoWs.Contains("<MSGERRO>"))
                {
                    this.dadosGravame.MsgGravame = new Erros("3", "FALHA AO ACESSAR FORNECEDOR");
                    return this.dadosGravame;
                }
                
                // INFOMACAO ENCONTRADA NAS BASES CONSULTADAS
                if (!String.IsNullOrEmpty(retornoWs))
                {
                    this.dadosGravame.MsgGravame = new Erros("1", "CONSTA REGISTRO DE GRAVAME PARA O VEICULO INFORMADO");
                    this.dadosGravame.IdConsulta = respostaWs.GetElementsByTagName("COD_SERVICO").Item(0).InnerText;
                    this.dadosGravame.OcorrenciasGravame = new List<GravameModel>();
                    this.dadosGravame.Carro = new Veiculo();
                    
                    foreach (XmlNode node in respostaWs.SelectNodes("//GRAVAME"))
                    {
                        tempGravame = new GravameModel();

                        this.dadosGravame.Carro.Placa = node.ChildNodes[1]["Placa"].InnerText;
                        this.dadosGravame.Carro.AnoFabrica = node.ChildNodes[1]["Ano_Fabricacao"].InnerText;
                        this.dadosGravame.Carro.AnoModelo = node.ChildNodes[1]["Ano_Modelo"].InnerText;
                        this.dadosGravame.Carro.Renavam = node.ChildNodes[1]["Renavam"].InnerText;
                        this.dadosGravame.Carro.Chassi = this.carro.Chassi;

                        tempGravame.NomeFinanciado = node.ChildNodes[0]["Nome"].InnerText;
                        tempGravame.DocumentoFinanciado = node.ChildNodes[0]["Documento"].InnerText;
                        tempGravame.StatusVeiculo = node.ChildNodes[1]["Situacao"].InnerText;
                        tempGravame.UFPlaca = node.ChildNodes[1]["UF"].InnerText;
                        tempGravame.DataInclusaoGravame = node.ChildNodes[1]["Data"].InnerText;
                        tempGravame.NomeAgente = node.ChildNodes[2]["Nome_Agente"].InnerText;
                        tempGravame.DocumentoAgente = node.ChildNodes[2]["CNPJ_Agente"].InnerText;
                        tempGravame.UFGravame = node.ChildNodes[2]["UF_Gravame"].InnerText;
                        tempGravame.DataContrato = node.ChildNodes[2]["Data_Contrato"].InnerText;
                        tempGravame.NrGravame = node.ChildNodes[2]["Num_Gravame"].InnerText;
                        tempGravame.NrContrato = node.ChildNodes[2]["Num_Contrato"].InnerText;
                        tempGravame.AssEletronica = node.ChildNodes[2]["Ass_Eletronica"].InnerText;

                        // ANULA VALORES EM BRANCO
                        this.dadosGravame.Carro = Util.unSetDadosVazios<Veiculo>(this.dadosGravame.Carro);
                        tempGravame = Util.unSetDadosVazios<GravameModel>(tempGravame);

                        this.dadosGravame.OcorrenciasGravame.Add(tempGravame);
                    }
                }
            }
            catch (Exception e)
            {
                this.dadosGravame.MsgGravame = null;
                this.dadosGravame.ErroGravame = new Erros("0", "Falha ao acessar fornecedor: " + e.ToString());
            }

            return this.dadosGravame;
        }
    }
}