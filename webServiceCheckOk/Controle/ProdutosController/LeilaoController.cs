using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using webServiceCheckOk.BaseDados;
using webServiceCheckOk.Controle.Fornecedores;
using webServiceCheckOk.Controle.Inteligencia;
using webServiceCheckOk.Controle.Inteligencia.Utils;

namespace webServiceCheckOk.Model.ProdutosModel
{
    public class LeilaoController
    {
        public string logServer { get; set; }
        public string requisicaoFornecedor { get; set; }

        public LeilaoModel getLeilao(UsuarioModel usuario, Veiculo carro, int codProduto = 6, string logFeature = "")
        {
            /*
             PRODUTOS:
                3 = LEILÃO SINTÉTICO
                6 = VEICULO LEILAO SIMPLES
                9 = VEICULO LEILAO SINTETICO
                11 = VEICULOLEILAOCOMPLETO
                20 = LEILAO HIST TDB
                21 = LEILAO HIST CFY
                25 = LEILAO FORNECEDORES
            */
            LeilaoModel leilaoDados = new LeilaoModel();
            int codFornecedor = 0;

            // 3 = LEILÃO SINTÉTICO
            if (codProduto == 3)
            {
                this.logServer = string.Empty;

                // 1º CHECA A CREDIFY SE ESTA HABILITADA PARA HISTORICO DE LEILAO
                if (Verificacao.getFornecedorConsulta(21) == 1)
                {
                    this.logServer += "|CHECKOK_246";
                    FornCredify leilaoCredify = new FornCredify(carro, usuario);

                    leilaoDados = leilaoCredify.getLeilao();
                    
                    this.logServer += "_" + leilaoDados.CodFornecedor;
                    this.requisicaoFornecedor = leilaoCredify.xmlRequisicao;

                    // 2º SE A CREDIFY NÃO TROUXER RESULTADOS TENTA NA TBD
                    if (((leilaoDados.MsgLeilao.Codigo != "1") && (Verificacao.getFornecedorConsulta(20) == 1) ) || (leilaoDados.ErroLeilao != null))
                    {
                        this.logServer += "|TDB_HIST_LEILAO";
                        FornTdb leilaoTdb = new FornTdb(carro,"1", usuario);

                        leilaoDados = leilaoTdb.getLeilao();

                        this.logServer += "_" + leilaoDados.CodFornecedor;
                        this.requisicaoFornecedor = leilaoTdb.urlRequisicao;
                    }
                }
            }
            else if (codProduto == 6)
            {
                // CODIGO CONSULTA: 6
                // FORNECEDORES: 1=CHECKAUTO;2=AUTORISCO;3=BOAVISTA;6=MOTORCHECK;7=ABSOLUTA;8=SEAPE;9=TDB

                codFornecedor = Verificacao.getFornecedorConsulta(6);

                try
                {
                    switch (codFornecedor)
                    {
                        case 9:
                            this.logServer += "|TDB_LEILAO";
                            FornTdb leilaoTDB = new FornTdb(carro, "1", usuario);

                            leilaoDados = leilaoTDB.getLeilao();

                            this.logServer += "_" + leilaoDados.CodFornecedor;
                            this.requisicaoFornecedor = leilaoTDB.urlRequisicao;
                            break;

                        case 3:
                            this.logServer += "|BOAVISTA_LEILAO";
                            FornBoaVista leilaoBV = new FornBoaVista(carro);
                            leilaoBV.codConsulta = "780";

                            leilaoDados = leilaoBV.getLeilao();

                            this.logServer += "_" + leilaoDados.CodFornecedor;
                            this.requisicaoFornecedor = leilaoBV.urlRequisicao;
                            break;
                    }

                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return leilaoDados;
        }
    
    }
}