using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using webServiceCheckOk.Controle.Inteligencia;
using webServiceCheckOk.Controle.Inteligencia.Utils;
using webServiceCheckOk.Model;
using webServiceCheckOk.Model.ProdutosModel;

namespace webServiceCheckOk.Controle.Fornecedores
{
    public class FornCheckAuto
    {
        private GravameModel checkAutoGravame;
        private BinModel checkAutoBinNacional;
        private Veiculo carro;
        private Pessoa pessoa;
        private string acesLogin = string.Empty;
        private string acesPassword = string.Empty;
        private string acesToken = string.Empty;
        private string codFornecedor = string.Empty;
        public string tipoConsulta = string.Empty;
        private string xmlRequisicao = string.Empty;
        private string urlRequisicao = string.Empty;
        private string retornoWs = string.Empty;

        // CONSTRUTOR
        public FornCheckAuto(Veiculo carro)
        {
            this.carro = carro;
            this.acesLogin = "CHECKOK";
            this.acesPassword = "J86KF8K4329F";
        }

        public BinModel getBinNacional()
        {
            // PREPARA A URL DE REQUISIÇÃO
            try
            {
                this.urlRequisicao = "http://201.20.2.154/MotorCheck/URL/PesquisaBaseNacional.aspx?txtusuario=" + this.acesLogin + "&txtsenha=" + this.acesPassword;
                this.urlRequisicao += "&txtplaca=" + this.carro.Placa;
                this.urlRequisicao += "&txtchassi=" + this.carro.Chassi;
                this.urlRequisicao += this.carro.NrMotor != string.Empty ? "&txtnumeromotor=" + this.carro.NrMotor : string.Empty;
                this.urlRequisicao += this.carro.Renavam != string.Empty ? "&txtrenavam=" + this.carro.Renavam : string.Empty;

                // REALIZA A CONSULTA NO FORNECEDOR
                StringBuilder xmlResposta = new StringBuilder();
                
                var relogio = Stopwatch.StartNew();
                this.retornoWs = new CustomTimeOut(60000).DownloadString(urlRequisicao);
                relogio.Stop();

                var elapsedMs = relogio.ElapsedMilliseconds;

                //this.retornoWs = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?><RESPOSTA><binXML><situacao>CIRCULACAO</situacao><ocorrencia>VEICULO INDICA OCORRENCIA DE ROUBO/FURTO</ocorrencia><placa>DAF0906</placa><municipio>SAO PAULO</municipio><uf>SP</uf><renavam>758284470</renavam><documento>28835457823</documento><ultima_atualizacao>06/06/2005</ultima_atualizacao><chassi>9C2JC30201R051281</chassi><remarcacao_do_chassi>Normal</remarcacao_do_chassi><montagem>COMPLETA</montagem><motor>JC30E21051281</motor><caixa_cambio /><num_carroceria /><num_eixo /><num_eixo_aux /><aux /><marca>HONDA/CG 125 TITAN ES</marca><tipo_veiculo>MOTOCICLE</tipo_veiculo><ano_fabricacao>2001</ano_fabricacao><ano_modelo>2001</ano_modelo><procedencia>NACIONAL</procedencia><especie>PAS</especie><combustivel>GASOLINA</combustivel><cilindrada>124</cilindrada><cor>VERMELHA</cor><potencia /><capacidade_passageiros>2</capacidade_passageiros><capacidade_carga /><CMT /><PTB /><restricao /></binXML></RESPOSTA>";

                try
                {
                    if (String.IsNullOrEmpty(this.retornoWs))
                    {
                        this.checkAutoBinNacional.ErroBinNacional = new Inteligencia.Erros("0", "CHAUT: CONSULTA INDISPONIVEL");

                        LogEstatico.setLogTitulo("ERRO BIN NACIONAL CEHCKAUTO: " + System.DateTime.Now);
                        LogEstatico.setLogTexto("requisicao : " + this.urlRequisicao);
                        LogEstatico.setLogTexto("chassi : " + this.carro.Chassi);

                        return this.checkAutoBinNacional;
                    }

                    //Inteligencia.LogEstatico.tempoMotorcheck("BinNacional", Inteligencia.LogEstatico.chave_consulta.ToString().PadRight(6, ' ') + "|" + (this.placa == null ? "" : this.placa).PadRight(7, ' ') + "|" + (this.chassi == null ? "" : this.chassi).PadRight(20, ' ') + "|  |" + System.DateTime.Now.ToString().PadRight(20, ' ') + "|" + elapsedMs.ToString().PadRight(10, ' ') + "|" + "".ToString().PadRight(7, ' ') + "|" + requisicao);
                }
                catch (Exception ex)
                {
                    LogEstatico.setLogTitulo("ERRO LOG CRONOMETRO: " + System.DateTime.Now);
                    LogEstatico.setLogTexto(ex.Message + ex.StackTrace);
                }

                //SEPARA O RESULTADO
                XmlDocument arrayResposta = new XmlDocument();
                arrayResposta.LoadXml(this.retornoWs);
                try
                {
                    this.checkAutoBinNacional.CodFornecedor = "_" + arrayResposta.SelectNodes("/RESPOSTA/BINXML/CODIGORETORNOPESQUISA").Item(0).InnerText.PadLeft(10, '0');
                }
                catch
                {
                    this.checkAutoBinNacional.CodFornecedor = "ERRO";
                }

                // VERIFICA O RESULTADO OBTIDO NO FORNECEDOR E TRATA OS ERROS             
                if (this.retornoWs.Contains("ERROR"))
                {
                    this.checkAutoBinNacional.ErroBinNacional = new Erros("0", "FALHA AO REALIZAR CONSULTA");
                    return this.checkAutoBinNacional; 
                }
                else if (this.retornoWs.Contains("PARAMETRO PLACA INFORMADO INCORRETAMENTE"))
                {
                    this.checkAutoBinNacional.ErroBinNacional = new Erros("11", "PLACA DO VEÍCULO INFORMADA INCORRETAMENTE");
                    return this.checkAutoBinNacional;
                }
                else if (this.retornoWs.Contains("SISTEMA INDISPONIVEL"))
                {
                    this.checkAutoBinNacional.ErroBinNacional = new Erros("1", "SISTEMA INDISPONIVEL");
                    return this.checkAutoBinNacional;
                }
                else if (this.retornoWs.Contains("PROBLEMAS NA EXECUCAO"))
                {
                    this.checkAutoBinNacional.ErroBinNacional = new Erros("2", "SISTEMA INDISPONIVEL TEMPORARIAMENTE");
                    return this.checkAutoBinNacional;
                }
                else if (this.retornoWs.Contains("NENHUM REGISTRO ENCONTRADO"))
                {
                    this.checkAutoBinNacional.ErroBinNacional = new Erros("10", "INFORMACAO NAO ENCONTRADA NAS BASES CONSULTADAS");
                    return this.checkAutoBinNacional;
                }
                else if (arrayResposta.SelectNodes("/RETORNO/MENSAGEM").Count > 0)
                {
                    this.checkAutoBinNacional.ErroBinNacional = new Erros("3", "CONSULTA POR 'RENAVAM OU MOTOR' DESATIVADA TEMPORARIAMENTE POR NOSSOS FORNECEDORES");
                    return this.checkAutoBinNacional;
                }
                else
                {
                    //CODIGO FORNECEDOR

                    this.checkAutoBinNacional.DtUltimaAtualizacao = arrayResposta.SelectNodes("/RESPOSTA/BINXML/DATAULTIMAATUALIZACAO").Item(0).InnerText;
                    
                    if ( this.checkAutoBinNacional.DtUltimaAtualizacao.Contains("-"))
                    {
                        var valor = this.checkAutoBinNacional.DtUltimaAtualizacao.Split('-');
                        this.checkAutoBinNacional.DtUltimaAtualizacao = valor[2] + "/" + valor[1] + "/" + valor[0];
                    }
                    
                    this.checkAutoBinNacional.Automovel.Chassi = arrayResposta.SelectNodes("/RESPOSTA/BINXML/CHASSI").Item(0).InnerText.Trim();
                    this.checkAutoBinNacional.Automovel.Renavam = arrayResposta.SelectNodes("/RESPOSTA/BINXML/RENAVAM").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.Placa = arrayResposta.SelectNodes("/RESPOSTA/BINXML/PLACA").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.MunicipioEmplacamento = arrayResposta.SelectNodes("/RESPOSTA/BINXML/MUNICIPIO").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.Modelo = arrayResposta.SelectNodes("/RESPOSTA/BINXML/MARCAMODELO").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.Cor = arrayResposta.SelectNodes("/RESPOSTA/BINXML/COR").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.Tipo = arrayResposta.SelectNodes("/RESPOSTA/BINXML/TIPOVEICULO").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.Especie = arrayResposta.SelectNodes("/RESPOSTA/BINXML/ESPECIE").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.Combustivel = arrayResposta.SelectNodes("/RESPOSTA/BINXML/COMBUSTIVEL").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.CapacidadePassageiros = arrayResposta.SelectNodes("/RESPOSTA/BINXML/CAPACIDADEPASSAGEIRO").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.NrMotor = arrayResposta.SelectNodes("/RESPOSTA/BINXML/NUMEROMOTOR").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.AnoModelo = arrayResposta.SelectNodes("/RESPOSTA/BINXML/ANOMODELO").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.AnoFabrica = arrayResposta.SelectNodes("/RESPOSTA/BINXML/ANOFABRICACAO").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.TipoMontagem = arrayResposta.SelectNodes("/RESPOSTA/BINXML/TIPOMONTAGEM").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.Potencia = arrayResposta.SelectNodes("/RESPOSTA/BINXML/POTENCIA").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.Cilindradas = arrayResposta.SelectNodes("/RESPOSTA/BINXML/CILINDRADA").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.NrCarroceria = arrayResposta.SelectNodes("/RESPOSTA/BINXML/NUMEROCARROCERIA").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.Nacionalidade = arrayResposta.SelectNodes("/RESPOSTA/BINXML/PROCEDENCIA").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.CapacidadeMaximaTracao = arrayResposta.SelectNodes("/RESPOSTA/BINXML/CMT").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.CapacidadeCarga = arrayResposta.SelectNodes("/RESPOSTA/BINXML/CAPACIDADECARGA").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.TipoCarroceria = arrayResposta.SelectNodes("/RESPOSTA/BINXML/TIPOCARROCERIA").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.Uf = arrayResposta.SelectNodes("/RESPOSTA/BINXML/UF").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.NrCambio = arrayResposta.SelectNodes("/RESPOSTA/BINXML/NUMEROCAIXACAMBIO").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.NrEixoTraseiro = arrayResposta.SelectNodes("/RESPOSTA/BINXML/NUMEROEIXOTRASEIRO").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.PesoBruto = arrayResposta.SelectNodes("/RESPOSTA/BINXML/PBT").Item(0).InnerText;
                    this.checkAutoBinNacional.Automovel.QtdEixos = (arrayResposta.SelectNodes("/RESPOSTA/BINXML/EIXOS").Item(0).InnerText).Trim();
                    
                    this.checkAutoBinNacional.Ocorrencia = arrayResposta.SelectNodes("/RESPOSTA/BINXML/OCORRENCIA").Item(0).InnerText.Trim();
                    this.checkAutoBinNacional.SituacaoChassi = arrayResposta.SelectNodes("/RESPOSTA/BINXML/SITUACAOCHASSI").Item(0).InnerText;
                    this.checkAutoBinNacional.SituacaoVeiculo = arrayResposta.SelectNodes("/RESPOSTA/BINXML/SITUACAOVEICULO").Item(0).InnerText;
                    this.checkAutoBinNacional.DtLimiteRestricaoTributaria = arrayResposta.SelectNodes("/RESPOSTA/BINXML/DATALIMITERESTRICAOTRIBUTARIA").Item(0).InnerText;
                    this.checkAutoBinNacional.DocFaturado = arrayResposta.SelectNodes("/RESPOSTA/BINXML/CPFCNPJFATURADO").Item(0).InnerText;
                    this.checkAutoBinNacional.TipoDocFaturado = arrayResposta.SelectNodes("/RESPOSTA/BINXML/TIPODOCUMENTOFATURADO").Item(0).InnerText;
                    this.checkAutoBinNacional.TipoDocImportadora = arrayResposta.SelectNodes("/RESPOSTA/BINXML/TIPODOCUMENTOIMPORTADORA").Item(0).InnerText;
                    this.checkAutoBinNacional.UFFaturado = arrayResposta.SelectNodes("/RESPOSTA/BINXML/UFFATURADO").Item(0).InnerText;
                    this.checkAutoBinNacional.IndicaRestricaoRenajud = arrayResposta.SelectNodes("/RESPOSTA/BINXML/INDICADORRESTRICAORENAJUD").Item(0).InnerText;
                    
                    if( arrayResposta.SelectNodes("/RESPOSTA/BINXML/RESTRICAO1").Item(0).InnerText != string.Empty)
                        this.checkAutoBinNacional.Restricoes.Add(new AuxRestricoes("Restricao1", arrayResposta.SelectNodes("/RESPOSTA/BINXML/RESTRICAO1").Item(0).InnerText));
                    
                    if (arrayResposta.SelectNodes("/RESPOSTA/BINXML/RESTRICAO2").Item(0).InnerText != string.Empty)
                        this.checkAutoBinNacional.Restricoes.Add(new AuxRestricoes("Restricao2", arrayResposta.SelectNodes("/RESPOSTA/BINXML/RESTRICAO2").Item(0).InnerText));
                    
                    if (arrayResposta.SelectNodes("/RESPOSTA/BINXML/RESTRICAO3").Item(0).InnerText != string.Empty)
                        this.checkAutoBinNacional.Restricoes.Add(new AuxRestricoes("Restricao3", arrayResposta.SelectNodes("/RESPOSTA/BINXML/RESTRICAO3").Item(0).InnerText));
                    
                    if (arrayResposta.SelectNodes("/RESPOSTA/BINXML/RESTRICAO4").Item(0).InnerText != string.Empty)
                        this.checkAutoBinNacional.Restricoes.Add(new AuxRestricoes("Restricao4", arrayResposta.SelectNodes("/RESPOSTA/BINXML/RESTRICAO4").Item(0).InnerText));
                    
                    if( arrayResposta.SelectNodes("/RESPOSTA/BINXML/PROPRIETARIO").Item(0).InnerText != string.Empty)
                        this.checkAutoBinNacional.Proprietario.Documento = arrayResposta.SelectNodes("/RESPOSTA/BINXML/PROPRIETARIO").Item(0).InnerText;
                }
            }
            catch (Exception ex)
            {
                LogEstatico.setLogTitulo("ERRO MOTORCHECK BINNACIONAL -> " + System.DateTime.Now);
                LogEstatico.setLogTexto(ex.Message + ex.StackTrace);
                LogEstatico.setLogTexto("Parametros: PLACA -> " + this.carro.Placa);

                if (ex.Message.Contains("O tempo limite da operação foi atingido"))
                {
                    //Inteligencia.LogEstatico.tempoConsultaAuto("BinNacional", Inteligencia.LogEstatico.chave_consulta.ToString().PadRight(6, ' ') + "|" + (this.placa == null ? "" : this.placa).PadRight(7, ' ') + "|" + (this.chassi == null ? "" : this.chassi).PadRight(20, ' ') + "|  |" + System.DateTime.Now.ToString().PadRight(20, ' ') + "|" + "".ToString().PadRight(10, ' ') + "|" + "TIMEOUT" + "|" + requisicao);
                }
            }

            return this.checkAutoBinNacional;
        }


        /*
        // MONTA O XML PARA OBTER A RESPOSTA DO FORNECEDOR
        protected void setXmlRequisicao()
        {
            XmlDocument xDoc = new XmlDocument();

            XmlElement raiz = xDoc.CreateElement("xml");
            xDoc.AppendChild(raiz);
            XmlDeclaration headerConfig = xDoc.CreateXmlDeclaration("1.0", "ISO-8859-1", "yes");

            xDoc.InsertBefore(headerConfig, raiz);
            XmlElement consulta = xDoc.CreateElement("CONSULTA");

            XmlElement acesso = xDoc.CreateElement("ACESSO");
            // DADOS DE ACESSO
            XmlElement acesso_usuario = xDoc.CreateElement("USUARIO");
            XmlElement acesso_senha = xDoc.CreateElement("SENHA");
            acesso.AppendChild(acesso_usuario);
            acesso.AppendChild(acesso_senha);
            acesso_usuario.InnerText = this.acesLogin;
            acesso_senha.InnerText = this.acesPassword;

            consulta.AppendChild(acesso);

            XmlElement veiculo = xDoc.CreateElement("VEICULO");
            // DADOS DO VEICULO
            XmlElement veiculo_chassi = xDoc.CreateElement("CHASSI");
            XmlElement veiculo_uf = xDoc.CreateElement("UF");
            XmlElement veiculo_placa = xDoc.CreateElement("PLACA");
            XmlElement veiculo_renavam = xDoc.CreateElement("RENAVAM");
            XmlElement veiculo_motor = xDoc.CreateElement("MOTOR");
            XmlElement veiculo_crlv = xDoc.CreateElement("CRLV");
            XmlElement veiculo_uf_crlv = xDoc.CreateElement("UF_CRLV");
            veiculo.AppendChild(veiculo_chassi);
            veiculo.AppendChild(veiculo_uf);
            veiculo.AppendChild(veiculo_placa);
            veiculo.AppendChild(veiculo_renavam);
            veiculo.AppendChild(veiculo_motor);
            veiculo.AppendChild(veiculo_crlv);
            veiculo.AppendChild(veiculo_uf_crlv);
            veiculo_chassi.InnerText = this.carro.Chassi;
            veiculo_uf.InnerText = this.carro.Uf;
            veiculo_placa.InnerText = this.carro.Placa;
            veiculo_renavam.InnerText = this.carro.Renavam;
            veiculo_motor.InnerText = this.carro.NrMotor;
            veiculo_crlv.InnerText = this.carro.Crlv;
            veiculo_uf_crlv.InnerText = this.carro.UfCrlv;

            consulta.AppendChild(veiculo);

            // DADOS DA PESSOA
            XmlElement dadospessoais = xDoc.CreateElement("DADOSPESSOAIS");
            XmlElement dadospessoais_tipopessoa = xDoc.CreateElement("TIPOPESSOARESTRICOES");
            XmlElement dadospessoais_cpfcnpj = xDoc.CreateElement("CPFCNPJRESTRICOES");
            XmlElement dadospessoais_ddd1 = xDoc.CreateElement("DDD1RESTRICOES");
            XmlElement dadospessoais_tel1 = xDoc.CreateElement("TEL1RESTRICOES");
            XmlElement dadospessoais_ddd2 = xDoc.CreateElement("DDD2RESTRICOES");
            XmlElement dadospessoais_tel2 = xDoc.CreateElement("TEL2RESTRICOES");
            dadospessoais.AppendChild(dadospessoais_tipopessoa);
            dadospessoais.AppendChild(dadospessoais_cpfcnpj);
            dadospessoais.AppendChild(dadospessoais_ddd1);
            dadospessoais.AppendChild(dadospessoais_tel1);
            dadospessoais.AppendChild(dadospessoais_ddd2);
            dadospessoais.AppendChild(dadospessoais_tel2);
            dadospessoais_tipopessoa.InnerText = this.pessoa.Tipo;
            dadospessoais_cpfcnpj.InnerText = this.pessoa.Documento;

            if (this.pessoa.Telefone.Count() > 0)
            {
                dadospessoais_ddd1.InnerText = this.pessoa.Telefone[0].DDD;
                dadospessoais_tel1.InnerText = this.pessoa.Telefone[0].Numero;
            }

            if (this.pessoa.Telefone.Count() > 1)
            {
                dadospessoais_ddd2.InnerText = this.pessoa.Telefone[1].DDD;
                dadospessoais_tel2.InnerText = this.pessoa.Telefone[1].Numero;
            }

            consulta.AppendChild(dadospessoais);

            // DADOS DE PERMISSÃO
            XmlElement permissoes = xDoc.CreateElement("PERMISSOES");
            XmlElement permissoes_contratoid = xDoc.CreateElement("CONTRATOID");
            XmlElement permissoes_pacoteid = xDoc.CreateElement("PACOTEID");
            XmlElement permissoes_opcionais = xDoc.CreateElement("OPCIONAIS");
            XmlElement permissoes_item = xDoc.CreateElement("ITEM");

            permissoes.AppendChild(permissoes_contratoid);
            permissoes.AppendChild(permissoes_pacoteid);
            permissoes.AppendChild(permissoes_opcionais);

            permissoes_opcionais.AppendChild(permissoes_item);
            permissoes_contratoid.InnerText = this.acesToken;
            permissoes_pacoteid.InnerText = this.tipoConsulta;
            permissoes_item.InnerText = string.Empty;

            consulta.AppendChild(permissoes);

            XmlElement consultaid = xDoc.CreateElement("CONSULTAID");
            consultaid.InnerText = string.Empty;

            consulta.AppendChild(consultaid);

            XmlElement nrcontrole = xDoc.CreateElement("NRCONTROLECLIENTE");
            nrcontrole.InnerText = string.Empty;

            consulta.AppendChild(nrcontrole);

            raiz.AppendChild(consulta);

            this.xmlRequisicao = xDoc.InnerXml;
        }

        public void consultaCheckAuto()
        {
            #region WEBSERVICE CHECKAUTO

            WsCheckAuto.WebService1 wsCheckAuto = new WsCheckAuto.WebService1();
            wsCheckAuto.Timeout = 600000;

            this.consultaRes = wsCheckAuto.Consultar(this.xmlRequisicao);//.Replace("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>", "").Replace("&", "&amp;").Replace("'", "&apos;").ToString();

            XmlDocument arrayResposta = new XmlDocument();

            arrayResposta.LoadXml(this.consultaRes);
            try
            {
                this.codigo_fornecedor = "_" + arrayResposta.SelectNodes("/Resposta/ConsultaID").Item(0).InnerText.PadLeft(10, '0');
            }
            catch (Exception e)
            {
                this.codigo_fornecedor = "ERRO";
            }

            #endregion
        }
         */

    }
}