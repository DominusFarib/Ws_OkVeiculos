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
    public class PrecificadorController
    {
        public string logServer { get; set; }

        public PrecificadorModel getPredificador(UsuarioModel usuario, Veiculo carro, bool isFeature = true)
        {
            // FEATURE 74
            // FORNECEDORES 3=TECNOBANK;

            PrecificadorModel precificadorDados = new PrecificadorModel();
            int codFornecedor = 3;

            try
            {
                switch (codFornecedor)
                {
                    case 3:
                        this.logServer += "|TECNOBANK_PRECIFICADOR";
                        FornTecnoBank respPrecificador = new FornTecnoBank(carro);
                        precificadorDados = respPrecificador.getPrecificador();
                    break;
                    default:
                        this.logServer += "|TECNOBANK_PRECIFICADOR";
                        FornTecnoBank respPrecificadorDefault = new FornTecnoBank(carro);
                        precificadorDados = respPrecificadorDefault.getPrecificador();
                    break;
                }

                return precificadorDados;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}