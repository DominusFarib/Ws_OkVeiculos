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
    public class DecodChassiController
    {
        public string logServer { get; set; }
        public string requisicaoFornecedor { get; set; }
        public DecodChassiModel getDecodChassi(UsuarioModel usuario, Veiculo carro, bool isFeature = false)
        {
            // CODIGO CONSULTA = 4
            // FORNECEDORES 1=CHECKAUTO;2=AUTORISCO;3=TECNOBANK;4=CREDIFY
            DecodChassiModel decodChassiDados = new DecodChassiModel();
            // PEGA O CODIGO DO FORNECEDOR DA CONSULTA
            int codFornecedor = Verificacao.getFornecedorConsulta(4);

            try
            {
                switch (codFornecedor)
                {
                    case 3:
                        this.logServer += "|TECNOBANK_DECODCHASSI";
                        FornTecnoBank respDecodChassi = new FornTecnoBank(carro);
                        // PEGA RESPOSTA NO FORNECEDOR
                        decodChassiDados = respDecodChassi.getDecodChassi();
                        this.requisicaoFornecedor = respDecodChassi.strRequisicao;
                    break;
                    default:
                        this.logServer += "|TECNOBANK_DECODCHASSI";
                        FornTecnoBank respDecodChassiDefault = new FornTecnoBank(carro);
                        // PEGA RESPOSTA NO FORNECEDOR
                        decodChassiDados = respDecodChassiDefault.getDecodChassi();
                        this.requisicaoFornecedor = respDecodChassiDefault.strRequisicao;
                    break;
                }

                return decodChassiDados;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}