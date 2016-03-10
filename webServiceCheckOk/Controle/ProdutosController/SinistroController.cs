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
        // BASE ESTADUAL
        public SinistroModel getPerdaTotal(UsuarioModel usuario, Veiculo carro, bool isFeature = false)
        {
            // CODIGO CONSULTA: 7
            // FORNECEDORES:    '1=CHECKAUTO;2=MOTORCHECK;3=BOA VISTA;4=CHECKPRO;5=CREDIFY'
            SinistroModel perdaTotalDados = new SinistroModel();
            SinistroModel logBuffer = new SinistroModel();
            string logServer = string.Empty;
            string logLancamento = string.Empty;
            // PEGA O CODIGO DO FORNECEDOR DA CONSULTA
            int codFornecedor = Verificacao.getFornecedorConsulta(7);

            try
            {
                logServer = usuario.Ip;
                logLancamento = DataBases.getLaunching();
                string subtransacao = string.Empty;
                subtransacao = isFeature ? "FT21" : "PC7";
                // QUANDO FOR FEATURE USA FORNECEDOR
                if (isFeature)
                {
                    switch (codFornecedor)
                    {
                        case 4:
                            logServer += "|CHECKPRO_PERDATOTAL";
                            FornCheckPro binConsultAuto = new FornCheckPro(carro);
                            // INSERE LOG DE REQUISIÇÃO
                            DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("1"), carro.Chassi);
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
                // INSERE LOG DE RESPOSTA
                DataBases.InsertLog(Convert.ToDecimal(logLancamento), usuario.Logon, "CHAUT", subtransacao, "", "", DateTime.Now, logServer, Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), Convert.ToDecimal("0"), logBuffer.ToString());

                return perdaTotalDados;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}