using System;
using System.IO;
using System.Xml.Serialization;
using webServiceCheckOk.BaseDados;
using webServiceCheckOk.Controle.Fornecedores;
using webServiceCheckOk.Controle.Inteligencia;
using webServiceCheckOk.Controle.Inteligencia.Utils;
using webServiceCheckOk.Model;
using webServiceCheckOk.Model.ProdutosModel;

namespace webServiceCheckOk.Controle.ProdutosController
{
    public class SinistroController
    {
        public string logServer { get; set; }

        public SinistroModel getPerdaTotal(UsuarioModel usuario, Veiculo carro, bool isFeature = false)
        {
            // CODIGO CONSULTA: 7
            // FORNECEDORES:    '1=CHECKAUTO;2=MOTORCHECK;3=BOA VISTA;4=CHECKPRO;5=CREDIFY'
            SinistroModel perdaTotalDados = new SinistroModel();
            SinistroModel logBuffer = new SinistroModel();
            string logLancamento = string.Empty;
            // PEGA O CODIGO DO FORNECEDOR DA CONSULTA
            int codFornecedor = Verificacao.getFornecedorConsulta(7);

            try
            {
                logLancamento = DataBases.getLaunching();
                string subtransacao = string.Empty;
                subtransacao = isFeature ? "FT21" : "PC7";
                // QUANDO FOR FEATURE USA FORNECEDOR
                if (isFeature)
                {
                    switch (codFornecedor)
                    {
                        case 4:
                            this.logServer += "|CHECKPRO_PERDATOTAL";
                            FornCheckPro binConsultAuto = new FornCheckPro(carro);

                            binConsultAuto.dadosUsuario = usuario;
                            binConsultAuto.logLancamento = logLancamento;
                            // PEGA RESPOSTA NO FORNECEDOR
                            perdaTotalDados = binConsultAuto.getPerdaTotal();
                            logBuffer = perdaTotalDados;
                            break;
                    }
                }

                var serializer = new XmlSerializer(typeof(SinistroModel));

                using (StringWriter writer = new EncodingTextUTF8())
                {
                    serializer.Serialize(writer, logBuffer);
                }

                return perdaTotalDados;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}