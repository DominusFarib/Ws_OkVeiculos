using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using webServiceCheckOk.BaseDados;
using webServiceCheckOk.Controle.Inteligencia;
using webServiceCheckOk.Model;
using webServiceCheckOk.Model.ProdutosModel;

namespace webServiceCheckOk.Controle.Fornecedores
{
    public class FornCredify
    {
        private GravameModel credifyGravame;
        private Veiculo carro;
        private List<Veiculo> dadosCarro;
        private List<GravameModel> dadosGravame;
        private UsuarioModel usuario;
        
        private string tipoPessoa = string.Empty;
        private string docPessoa = string.Empty;
        private string strRequisicao = string.Empty;
        private string retornoWs = string.Empty;
        public string idConsulta { get; set; }

        // CONSTRUTOR
        public FornCredify(Veiculo carro, UsuarioModel usuario)
        {
            this.carro = carro;
            this.usuario = usuario;
        }
        // GRAVAME RESPOSTA
        public GravameModel getGravame()
        {
            XmlDocument arrayResposta = new XmlDocument();
            Veiculo dadosCarro;
            GravameModel dadosGravame;

            try
            {
                this.idConsulta = "241";
                setStrRequisicao();
                WsCredify.serverconsulta wsCredify = new WsCredify.serverconsulta();
                this.retornoWs = wsCredify.Consultar(this.strRequisicao);
                // XML PADRÃO CREDIFY
                //this.retornoWs = "	<XML>	<CONSULTA>		<CODIGORESPOSTA>1195239</CODIGORESPOSTA>		<DATAHORA>04/06/2013 10:30:25</DATAHORA>		<LOGON>WS00000001</LOGON>		<IDCONSULTA>149</IDCONSULTA>	</CONSULTA>	<RESPOSTA><GRAVAME><FINANCIADO>ALEXANDRE PASCHOALOTTO PENHA</FINANCIADO><CPF_CNPJ>00026628162877</CPF_CNPJ><PLACA>BOX8846</PLACA><CHASSI>9BGLK19BRRB313127</CHASSI><RENAVAM>622196626</RENAVAM><SITUACAO>VEICULO TEVE GRAVAME BAIXADO PELO AGENTE FINANCEIRO</SITUACAO><DATA>28/06/2005</DATA><NOME_AGENTE>BC ABN AMRO REAL S A</NOME_AGENTE><CNPJ_AGENTE>33066408000115</CNPJ_AGENTE><UF_GRAVAME>SP</UF_GRAVAME><DATA_CONSULTA>04/06/2013 13:30:53</DATA_CONSULTA></GRAVAME></RESPOSTA></XML>";
                //this.retornoWs = "<XML><CONSULTA>	<CODIGORESPOSTA>1195182</CODIGORESPOSTA><DATAHORA>04/06/2013 10:24:09</DATAHORA>	<LOGON>WS00000001</LOGON>		<IDCONSULTA>149</IDCONSULTA>	</CONSULTA>	<RESPOSTA><GRAVAME></GRAVAME></RESPOSTA></XML>";            

                arrayResposta.LoadXml(this.retornoWs);

                try
                {
                    this.credifyGravame.CodFornecedor = "_" + arrayResposta.SelectNodes("/XML/CONSULTA/CODIGORESPOSTA").Item(0).InnerText.PadLeft(10, '0');
                }
                catch
                {
                    this.credifyGravame.CodFornecedor = "_ERRO";
                }
                // ERRO NO FORNECEDOR
                if (arrayResposta.SelectNodes("/XML/RESPOSTA/ERRO").Count > 0)
                {
                    this.credifyGravame.ErroGravame = new Erros("0", "ERR CRFY: CONSULTA INDISPONIVEL");
                    return this.credifyGravame;
                }
                // INFOMACAO NAO ENCONTRADA NAS BASES CONSULTADAS
                if ((arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME").Count == 0) || (arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/FAULTSTRING").Count > 0))
                {
                    this.credifyGravame.MsgGravame = new Erros("2", "CRFY: INFORMACAO NAO ENCONTRADA NAS BASES CONSULTADAS");
                    return this.credifyGravame;
                }

                if (arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME").Count > 0)
                {
                    this.credifyGravame.MsgGravame = new Erros("1", "CONSTA REGISTRO DE GRAVAME PARA O VEICULO INFORMADO");
                    int i = 0;
                    while (i != -1)
                    {
                        if (arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/GRAVAME/REGISTRO_" + i) != null)
                        {
                            dadosCarro = new Veiculo();
                            dadosGravame = new GravameModel();

                            dadosCarro.Placa = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/PLACA").Item(0).InnerText;
                            dadosCarro.Chassi = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/CHASSI").Item(0).InnerText;
                            dadosCarro.Renavam = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/RENAVAM").Item(0).InnerText;
                            dadosCarro.NrMotor = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/NUMERO_MOTOR").Item(0).InnerText;
                            dadosCarro.Marca = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/MARCA").Item(0).InnerText;
                            dadosCarro.Modelo = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/MODELO").Item(0).InnerText;
                            dadosCarro.Cor = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/COR").Item(0).InnerText;
                            dadosCarro.AnoFabrica = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/ANO_FABRICACAO").Item(0).InnerText;
                            dadosCarro.AnoModelo = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/ANO_MODELO").Item(0).InnerText;
                            dadosCarro.Combustivel = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/COMBUSTIVEL").Item(0).InnerText;
                            dadosCarro.Potencia = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/POTENCIA").Item(0).InnerText;
                            dadosCarro.Cilindradas = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/CILINDRADAS").Item(0).InnerText;
                            dadosCarro.CapacidadeCarga = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/CAPACIDADE_CARGA").Item(0).InnerText;
                            dadosCarro.Nacionalidade = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/NACIONALIDADE").Item(0).InnerText;
                            dadosCarro.QtdPassageiros = (arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/PASSAGEIROS").Item(0).InnerText).Trim();
                            dadosCarro.QtdEixos = (arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/EIXOS").Item(0).InnerText).Trim();
                            //dadosCarro.QtdPassageiros = Int16.Parse(arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/PASSAGEIROS").Item(0).InnerText);
                            //dadosCarro.QtdEixos = Int16.Parse(arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/EIXOS").Item(0).InnerText);

                            dadosGravame.RemarcacaoChassi = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/REMARCACAO_CHASSI").Item(0).InnerText;
                            dadosGravame.StatusVeiculo = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/GRAVAME/REGISTRO_" + i + "/STATUS").Item(0).InnerText;
                            dadosGravame.UFLicenciamento = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/UF").Item(0).InnerText;
                            dadosGravame.UFPlaca = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/UF").Item(0).InnerText;
                            dadosGravame.DocumentoFinanciado = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/DOCUMENTO").Item(0).InnerText;
                            dadosGravame.NomeFinanciado = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/PROPRIETARIO").Item(0).InnerText;
                            dadosGravame.NomeAgente = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/GRAVAME/REGISTRO_" + i + "/FINANCEIRA").Item(0).InnerText;
                            dadosGravame.DocumentoAgente = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/GRAVAME/REGISTRO_" + i + "/DOCUMENTO_FINANCEIRA").Item(0).InnerText;
                            dadosGravame.DataInclusaoGravame = arrayResposta.SelectNodes("/XML/RESPOSTA/GRAVAME/GRAVAME/REGISTRO_" + i + "/DATA_CONTRATO").Item(0).InnerText;

                            this.credifyGravame.HistoricoGravames.Add(new AuxHistoricoGravames(dadosCarro, dadosGravame));
                        }
                        else
                        {
                            i = -1;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                this.credifyGravame.ErroGravame = new Erros("0", "Falha ao acessar fornecedor: " + e.ToString());
            }

            return this.credifyGravame;
        }


        // MONTA A URL PARA OBTER O XML DO FORNECEDOR
        public void setStrRequisicao()
        {
            string[] arrayLogSenha = new string[2];

            XmlDocument xDoc = new XmlDocument();
            XmlElement raiz = xDoc.CreateElement("xml");
            xDoc.AppendChild(raiz);

            XmlDeclaration headerXml = xDoc.CreateXmlDeclaration("1.0", "ISO-8859-1", "yes");
            xDoc.InsertBefore(headerXml, raiz);

            XmlElement acesso = xDoc.CreateElement("ACESSO");
            XmlElement acessoUsuario = xDoc.CreateElement("LOGON");
            XmlElement acessoSenha = xDoc.CreateElement("SENHA");
            
            acesso.AppendChild(acessoUsuario);
            acesso.AppendChild(acessoSenha);
            //acesso_usuario.InnerText = "WS00000001";
            //acesso_senha.InnerText = "pRHFpB8xu3";

            arrayLogSenha = getLogonSenha(this.usuario.Logon);

            acessoUsuario.InnerText = arrayLogSenha[0];
            acessoSenha.InnerText = arrayLogSenha[1];

            raiz.AppendChild(acesso);

            XmlElement consulta = xDoc.CreateElement("CONSULTA");
            XmlElement IDConsulta = xDoc.CreateElement("IDCONSULTA");
            XmlElement TipoPessoa = xDoc.CreateElement("TIPOPESSOA");
            XmlElement CpfCnpj = xDoc.CreateElement("CPFCNPJ");
            XmlElement Chassi = xDoc.CreateElement("CHASSI");


            consulta.AppendChild(IDConsulta);
            consulta.AppendChild(TipoPessoa);
            consulta.AppendChild(CpfCnpj);
            consulta.AppendChild(IDConsulta);
            consulta.AppendChild(Chassi);

            IDConsulta.InnerText = this.idConsulta;
            Chassi.InnerText = this.carro.Chassi;
            TipoPessoa.InnerText = this.tipoPessoa;
            CpfCnpj.InnerText = this.docPessoa;
            IDConsulta.InnerText = this.idConsulta;

            raiz.AppendChild(consulta);

            this.strRequisicao = xDoc.InnerXml;
        }

        public string[] getLogonSenha(string logon)
        {
            OracleCommand comandoSQL = new OracleCommand();
            System.Data.ConnectionState estatusConexao = comandoSQL.Connection.State;
            string[] dadosAcesso = new string[2];
            DataBases baseDados = new DataBases();

            try
            {
                // CRIA O COMANDO A SER EXECUTADO NO BANCO DE DADOS: USA A CONEXÃO DA CLASSE DATABASES
                OracleDataReader leitorDados;

                if (((comandoSQL.Connection.State & System.Data.ConnectionState.Open) != System.Data.ConnectionState.Open))
                    comandoSQL.Connection.Open();
                
                comandoSQL.CommandText = @"SELECT LOGON, SENHA FROM taccess.fonte_acesso " +
                                        @"WHERE id_fonte = (SELECT DECODE(c.receita, 9,30, 56,31, 58,32, 59,33, 100,34, 52,35, 36) id_fonte " +
                                                        @"FROM checkok.logons L " +
                                                        @"INNER JOIN checkok.cliente C on (c.codigo = L.cod_entidade) "+
                                                        @"WHERE L.logon like '" + logon + "%')";
                comandoSQL.CommandType = System.Data.CommandType.Text;
                comandoSQL.Connection = baseDados.Conn;

                leitorDados = comandoSQL.ExecuteReader();

                if (leitorDados.Read())
                {
                    dadosAcesso[0] = leitorDados["LOGON"].ToString();
                    dadosAcesso[1] = leitorDados["SENHA"].ToString();
                }

                leitorDados.Dispose();
                leitorDados.Close();
                baseDados.Conn.Dispose();
                baseDados.Conn.Close();
                comandoSQL.Dispose();
            }
            catch (Exception ex) 
            { 
                throw ex; 
            }
            finally
            {
                if ((estatusConexao == System.Data.ConnectionState.Closed))
                    baseDados.Conn.Close();
            }
            ///*
            // * caso ocorra algum erro, o sistema busca o logon da maneira tradicional
            // */
            //if (dadosAcesso[0] == null || dadosAcesso[1] == null)
            //{
            //    dadosAcesso[0] = ConfigurationSettings.AppSettings["logonCredify"];
            //    dadosAcesso[1] = ConfigurationSettings.AppSettings["senhaCredify"];
            //}

            return dadosAcesso;
        }
    }
}