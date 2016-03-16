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
using webServiceCheckOk.Model;
using webServiceCheckOk.Model.ProdutosModel;

namespace webServiceCheckOk.Controle.ProdutosController
{
    public class AgregadosController
    {
        public string logServer {get; set;}
        public string requisicaoFornecedor { get; set; }

        public AgregadosModel getAgregados(UsuarioModel usuario, Veiculo carro, bool isFeature = false)
        {
            // CODIGO CONSULTA = 15
            // FORNECEDORES 1=WLLM;2=MOTORCHECK;3=INFOCAR;4=CREDIFY;5=SEAPE;
            AgregadosModel agregadosDados = new AgregadosModel();

            int codFornecedor = Verificacao.getFornecedorConsulta(15);

            try
            {
                switch (codFornecedor)
                {
                    case 2:
                        this.logServer += "|MOTORCHECK_AGREGADOS";
                        FornMotorCheck respAgregados = new FornMotorCheck(carro);

                        agregadosDados = respAgregados.getAgregados();
                        
                        this.requisicaoFornecedor = respAgregados.urlRequisicao;
                        this.logServer += "_" + agregadosDados.IdConsulta;

                        break;
                    default:
                        this.logServer += "|MOTORCHECK_AGREGADOS";
                        FornMotorCheck respDefault = new FornMotorCheck(carro);

                        agregadosDados = respDefault.getAgregados();

                        this.requisicaoFornecedor = respDefault.urlRequisicao;
                        this.logServer += "_" + agregadosDados.IdConsulta;

                        break;
                }

                return agregadosDados;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}