using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webServiceCheckOk.Controle.Inteligencia;
using webServiceCheckOk.Model;
using webServiceCheckOk.Model.ProdutosModel;

namespace webServiceCheckOk.Controle.Fornecedores
{
    public class FornTdi
    {
        private GravameModel tdiGravame;
        private Veiculo carro;
        private List<Veiculo> dadosCarro;
        private List<GravameModel> dadosGravame;
        private UsuarioModel usuario;

        private string acesToken = string.Empty;
        private string acesLogin = string.Empty;
        private string acesPassword = string.Empty;

        private string tipoPessoa = string.Empty;
        private string docPessoa = string.Empty;
        private string strRequisicao = string.Empty;
        private string retornoWs = string.Empty;
        public string idConsulta { get; set; }

        // CONSTRUTOR
        public FornTdi(Veiculo carro, UsuarioModel usuario)
        {
            this.carro = carro;
            this.usuario = usuario;
            this.acesToken = "LSCQRZ3TMXS";
            this.acesLogin = "checktudo";
            this.acesPassword = "47c78b7235969fb5ff899a0862906dea";
        }
        // GRAVAME RESPOSTA
        public GravameModel getGravame()
        {
            Veiculo carroTdi;
            GravameModel gravameTdi;

            try
            {
                WsTdi.Consulta wsTdi = new WsTdi.Consulta();
                WsTdi.Gravame retornoWs = wsTdi.consultaGravame(this.acesLogin, this.acesPassword, this.carro.Chassi);
                this.tdiGravame = new GravameModel();

                // XML PADRÃO TDI
                //var xml = "<?xml version='1.0' encoding='ISO-8859-1'?><RESPOSTA><RESPOSTA_GRAVAME><CHASSI>8BCLDRFJWBG533681</CHASSI><ANO_FABRICACAO>2010</ANO_FABRICACAO><ANO_MODELO>2011</ANO_MODELO><CNPJ_AGENTE>06043050000132</CNPJ_AGENTE><CODIGO_AGENTE>000000001112</CODIGO_AGENTE><DATA_STATUS>20150702</DATA_STATUS><DESCRICAO_STATUS>VEÍCULO COM ALIENAÇÃO FIDUCIÁRIA COM DOCUMENTO JÁ EMITIDO</DESCRICAO_STATUS><NOME_AGENTE>BB ADM DE CONSORCIO SA</NOME_AGENTE><NUMERO_GRAVAME>10483028</NUMERO_GRAVAME><PLACA>ATM7552</PLACA><STATUS_VEICULO>11</STATUS_VEICULO><UF_GRAVAME>PR</UF_GRAVAME><UF_PLACA>PR</UF_PLACA></RESPOSTA_GRAVAME></RESPOSTA>";

                // ERRO NO FORNECEDOR
                if (retornoWs == null)
                {
                    this.tdiGravame.ErroGravame = new Erros("0", "ERR TDI: RETORNO VAZIO");
                    return this.tdiGravame;
                }

                // INFOMACAO NAO ENCONTRADA NAS BASES CONSULTADAS
                if (String.IsNullOrEmpty(retornoWs.gravameanofabricacao))
                {
                    this.tdiGravame.MsgGravame = new Erros("2", "TDI: INFORMACAO NAO ENCONTRADA NAS BASES CONSULTADAS");
                    return this.tdiGravame;
                }
                // INFOMACAO ENCONTRADA NAS BASES CONSULTADAS
                if (!String.IsNullOrEmpty(retornoWs.gravameanofabricacao))
                {
                    this.tdiGravame.MsgGravame = new Erros("1", "CONSTA REGISTRO DE GRAVAME PARA O VEICULO INFORMADO");
                    this.tdiGravame.IdConsulta = retornoWs.gravamenumerogravame;
                    carroTdi = new Veiculo();
                    gravameTdi = new GravameModel();

                    carroTdi.Placa = retornoWs.gravameplaca;
                    carroTdi.Chassi = retornoWs.chassi;
                    carroTdi.AnoFabrica = retornoWs.gravameanofabricacao;
                    carroTdi.AnoModelo = retornoWs.gravameanomodelo;

                    gravameTdi.StatusVeiculo = retornoWs.gravamedescricaostatus;
                    gravameTdi.UFGravame = retornoWs.gravameufgravame;
                    gravameTdi.UFPlaca = retornoWs.gravameufplaca;
                    gravameTdi.NomeAgente = retornoWs.gravamenomeagente;
                    gravameTdi.DocumentoAgente = retornoWs.gravamecnpjagente;
                    gravameTdi.DataInclusaoGravame = retornoWs.gravamedatastatus.Substring(6, 2) + "/" + retornoWs.gravamedatastatus.Substring(4, 2) + "/" + retornoWs.gravamedatastatus.Substring(0, 4);

                    //this.tdiGravame.ocorrencias = new Object[4] { 1, "Ocorrencia", carroTdi, gravameTdi };

                    this.tdiGravame.HistoricoGravames = new List<AuxHistoricoGravames>();
                    this.tdiGravame.HistoricoGravames.Add(new AuxHistoricoGravames(carroTdi, gravameTdi));
                }
            }
            catch (Exception e)
            {
                this.tdiGravame.ErroGravame = new Erros("0", "Falha ao acessar fornecedor: " + e.ToString());
            }

            return this.tdiGravame;
        }
    }
}