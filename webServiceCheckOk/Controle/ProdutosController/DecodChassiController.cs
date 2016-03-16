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

        public DecodChassiModel getDecodChassi(UsuarioModel usuario, Veiculo carro, bool isFeature = false)
        {
            // CODIGO CONSULTA = 4
            // FORNECEDORES 1=CHECKAUTO;2=AUTORISCO;3=TECNOBANK;4=CREDIFY
            DecodChassiModel decodChassiDados = new DecodChassiModel();
            DecodChassiModel logBuffer = new DecodChassiModel();
            string logLancamento = string.Empty;
            int codFornecedor = Verificacao.getFornecedorConsulta(4);

            try
            {
                logLancamento = DataBases.getLaunching();
                string subtransacao = string.Empty;
                subtransacao = isFeature ? "PC27" : "FT27";

                switch (codFornecedor)
                {
                    case 3:
                        this.logServer += "|TECNOBANK_DECODCHASSI";
                        FornTecnoBank respDecodChassi = new FornTecnoBank(carro);

                        decodChassiDados = respDecodChassi.getDecodChassi();

                        //this.logServer += "_" + decodChassiDados.IdConsulta;
                        logBuffer = decodChassiDados;

                        break;
                    default:
                        this.logServer += "|TECNOBANK_DECODCHASSI";
                        FornTecnoBank respDecodChassiDefault = new FornTecnoBank(carro);

                        decodChassiDados = respDecodChassiDefault.getDecodChassi();

                        //this.logServer += "_" + decodChassiDados.IdConsulta;
                        logBuffer = decodChassiDados;
                        break;
                }

                var serializer = new XmlSerializer(typeof(DecodChassiModel));

                using (StringWriter writer = new EncodingTextUTF8())
                {
                    serializer.Serialize(writer, logBuffer);
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