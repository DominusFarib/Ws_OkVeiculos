using System;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using webServiceCheckOk.Model;
using webServiceCheckOk.Model.ProdutosModel;
using webServiceCheckOk.Controle.Inteligencia;

namespace webServiceCheckOk.Controle.Fornecedores
{
    public class FornSeape 
    {
        private string consultaLeilao = string.Empty;
        public string requisicao = string.Empty;
        public string dataLeilao = string.Empty;
        public string codConsulta = string.Empty;
        public string idConsultaGravame = string.Empty;
        public string msgGravame = string.Empty;
        public string erroGravame = string.Empty;
        public string retornoWs = string.Empty;
        public string acesToken = string.Empty;
        public string acesLogin = string.Empty;
        public string acesPassword = string.Empty;
        public int codRetorno;

        private GravameModel seapGravame;
        private Veiculo carro;
        public List<Veiculo> dadosCarro { get; set; }
        public List<GravameModel> dadosGravame { get; set; }
        public int acesConsultaId;

        //CONSTRUTOR
        public FornSeape(Veiculo carro)
        {
            this.carro = carro;
            this.acesToken = "90AEC480-6024-4576-ADA7-D1A58E5BD7CA";
            this.acesLogin = "122114";
            this.acesPassword = "tefFG5rK";
        }
        /*
        #region HISTORICO DE PROPRIETARIOS

        public string moduloProprietarios()
        {
            try
            {
                this.acesConsultaId = 452;

                WebRequest request = WebRequest.Create("http://webservice.seape.com.br/Service-HttpPost/Veicular.aspx");
                request.Method = "POST";
                string postData = "CodigoProduto=" + this.acesConsultaId + "&ChaveAcesso=" + this.acesToken + "&Usuario=" + this.acesLogin + "&Senha=" + this.acesPassword + "&Placa=" + this.placa + "&Chassi=" + this.chassi + "";
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                request.ContentType = "application/x-www-form-urlencoded";

                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();

                dataStream.Write(byteArray, 0, byteArray.Length);

                dataStream.Close();

                WebResponse response = request.GetResponse();

                Console.WriteLine(((HttpWebResponse)response).StatusDescription);

                dataStream = response.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);

                this.retornoWs = reader.ReadToEnd();

                //Console.WriteLine(responseFromServer);
                //Console.ReadKey();

                reader.Close();
                dataStream.Close();
                response.Close();

                //ID_CONSULTA
                XmlDocument xmlAlterado = new XmlDocument();
                XmlDocument xmlOriginal = new XmlDocument();

                xmlOriginal.LoadXml(this.retornoWs);



                XmlElement raiz = xmlAlterado.CreateElement("proprietarios_anteriores");
                xmlAlterado.AppendChild(raiz);



                XmlNodeList listaRegistro = xmlOriginal.SelectNodes("//PROP_ANTERIORES_ITEM");
                XmlNodeList listaErro = xmlOriginal.SelectNodes("//MSG_ERRO");

                Int32 ocorrenciaCount = 0;


                if (listaRegistro.Count > 0)
                {


                    foreach (XmlNode node in listaRegistro)
                    {
                        XmlElement registro = xmlAlterado.CreateElement("registro");
                        XmlElement ocorrencia = xmlAlterado.CreateElement("ocorrencia");
                        XmlElement idPagamentoDut = xmlAlterado.CreateElement("idPagamentoDUT");
                        XmlElement ufDut = xmlAlterado.CreateElement("ufDUT");
                        XmlElement numeroDut = xmlAlterado.CreateElement("numero_dut");
                        XmlElement anoExercicio = xmlAlterado.CreateElement("anoexercicio");
                        XmlElement placa = xmlAlterado.CreateElement("placa");
                        XmlElement renavam = xmlAlterado.CreateElement("renavam");
                        XmlElement chassi = xmlAlterado.CreateElement("chassi");
                        XmlElement proprietario = xmlAlterado.CreateElement("proprietario");
                        XmlElement cpfcnpj = xmlAlterado.CreateElement("cpfcnpj");
                        XmlElement numero_banco = xmlAlterado.CreateElement("Numero_Banco");
                        XmlElement dtprocessamento = xmlAlterado.CreateElement("dtProcessamento");
                        XmlElement dtEmissaoGuia = xmlAlterado.CreateElement("dtEmissaoGuia");
                        XmlElement saldo = xmlAlterado.CreateElement("saldo");

                        registro.AppendChild(ocorrencia);
                        registro.AppendChild(idPagamentoDut);
                        registro.AppendChild(ufDut);
                        registro.AppendChild(numeroDut);
                        registro.AppendChild(anoExercicio);
                        registro.AppendChild(placa);
                        registro.AppendChild(renavam);
                        registro.AppendChild(chassi);
                        registro.AppendChild(proprietario);
                        registro.AppendChild(cpfcnpj);
                        registro.AppendChild(numero_banco);
                        registro.AppendChild(dtprocessamento);
                        registro.AppendChild(dtEmissaoGuia);
                        registro.AppendChild(saldo);

                        ocorrenciaCount++;
                        ocorrencia.InnerText = ocorrenciaCount.ToString();
                        idPagamentoDut.InnerText = node.SelectSingleNode("ID_PAG_DUT").InnerText;
                        ufDut.InnerText = node.SelectSingleNode("UF_DUT").InnerText;
                        numeroDut.InnerText = node.SelectSingleNode("NUMERO_DUT").InnerText;
                        anoExercicio.InnerText = node.SelectSingleNode("ANO_EXERCICIO").InnerText;
                        placa.InnerText = node.SelectSingleNode("PLACA").InnerText;
                        renavam.InnerText = node.SelectSingleNode("RENAVAM").InnerText;
                        chassi.InnerText = node.SelectSingleNode("CHASSI").InnerText;
                        proprietario.InnerText = node.SelectSingleNode("NOME_PROPRIETARIO").InnerText;
                        cpfcnpj.InnerText = node.SelectSingleNode("CGC_CPF").InnerText;
                        dtprocessamento.InnerText = node.SelectSingleNode("DATA_PROCESSAMENTO").InnerText;
                        dtEmissaoGuia.InnerText = node.SelectSingleNode("DATA_EMISSAO_GUIA").InnerText;
                        saldo.InnerText = node.SelectSingleNode("SALDO").InnerText;

                        raiz.AppendChild(registro);

                    }
                }

                else if (listaErro.Count > 0)
                {
                    XmlElement mensagem = xmlAlterado.CreateElement("mensagem");
                    mensagem.InnerText = "SISTEMA INDISPONIVEL";
                    raiz.AppendChild(mensagem);

                }
                else
                {
                    XmlElement mensagem = xmlAlterado.CreateElement("mensagem");
                    mensagem.InnerText = "SEM REGISTROS ENCONTRADOS";
                    raiz.AppendChild(mensagem);


                }






                return xmlAlterado.InnerXml;
            }
            catch(Exception ex)
            {
                XmlDocument xmlAlterado = new XmlDocument();
                XmlElement raiz = xmlAlterado.CreateElement("proprietarios_anteriores");
                xmlAlterado.AppendChild(raiz);
                XmlElement mensagem = xmlAlterado.CreateElement("mensagem");
                mensagem.InnerText = "SISTEMA INDISPONIVEL";
                raiz.AppendChild(mensagem);

                LogEstatico.titulo("ERRO SEAPE PROPRIETARIOS -> " + System.DateTime.Now);
                LogEstatico.escrever(ex.Message + ex.StackTrace);
                LogEstatico.escrever("PARAMETROS: " + placa + "/" + chassi);

                return xmlAlterado.InnerXml;

            }


        }

        public string histpropConsulta()
        {
            this.acesConsultaId = 452;
            WebRequest request = WebRequest.Create("http://webservice.seape.com.br/Service-HttpPost/Veicular.aspx");
            request.Method = "POST";
            string postData = "CodigoProduto=" + this.acesConsultaId + "&ChaveAcesso=" + this.acesToken + "&Usuario=" + this.acesLogin + "&Senha=" + this.acesPassword + "&Placa=" + this.placa + "&Chassi=" + this.chassi + "";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            request.ContentType = "application/x-www-form-urlencoded";

            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();

            dataStream.Write(byteArray, 0, byteArray.Length);

            dataStream.Close();

            WebResponse response = request.GetResponse();

            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            this.retornoWs = reader.ReadToEnd();

            //Console.WriteLine(responseFromServer);
            //Console.ReadKey();

            reader.Close();
            dataStream.Close();
            response.Close();

            //ID_CONSULTA
            XmlDocument xmlAlterado = new XmlDocument();
            XmlDocument xmlOriginal = new XmlDocument();

            xmlOriginal.LoadXml(this.retornoWs);



            XmlElement raiz = xmlAlterado.CreateElement("checkok");
            xmlAlterado.AppendChild(raiz);

            XmlDeclaration pi = xmlAlterado.CreateXmlDeclaration("1.0", "ISO-8859-1", "yes"); //PADRÃO DO XML
            xmlAlterado.InsertBefore(pi, raiz);

            XmlNodeList listaRegistro = xmlOriginal.SelectNodes("//PROP_ANTERIORES_ITEM");
            XmlNodeList listaErro = xmlOriginal.SelectNodes("//MSG_ERRO");
            this.idConsulta = xmlOriginal.SelectNodes("//ID_CONSULTA").Item(0).InnerText.Trim();

            Int32 ocorrenciaCount = 0;


            XmlElement dados_consulta = xmlAlterado.CreateElement("dados_consulta");
            XmlElement codigo_consulta = xmlAlterado.CreateElement("codigo_consulta");
            XmlElement data_consulta = xmlAlterado.CreateElement("data_consulta");
            XmlElement tipo_consulta = xmlAlterado.CreateElement("tipo_consulta");
            XmlElement hora_consulta = xmlAlterado.CreateElement("hora_consulta");
            XmlElement placaI = xmlAlterado.CreateElement("placa");
            XmlElement chassiI = xmlAlterado.CreateElement("chassi");
            XmlElement ufI = xmlAlterado.CreateElement("uf");
            XmlElement motor = xmlAlterado.CreateElement("motor");

            dados_consulta.AppendChild(codigo_consulta);
            dados_consulta.AppendChild(tipo_consulta);
            dados_consulta.AppendChild(data_consulta);
            dados_consulta.AppendChild(hora_consulta);
            dados_consulta.AppendChild(placaI);
            dados_consulta.AppendChild(chassiI);
            dados_consulta.AppendChild(ufI);
            dados_consulta.AppendChild(motor);

            codigo_consulta.InnerText = this.codigo_consulta;
            tipo_consulta.InnerText = "Veículo Proprietarios";
            data_consulta.InnerText = DateTime.Now.ToString("dd/MM/yyyy");
            hora_consulta.InnerText = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
            placaI.InnerText = this.placa;
            chassiI.InnerText = this.chassi;

            raiz.AppendChild(dados_consulta);

            XmlElement proprietarios_anteriores = xmlAlterado.CreateElement("proprietarios_anteriores");
            raiz.AppendChild(proprietarios_anteriores);


            if (listaRegistro.Count > 0)
            {



                foreach (XmlNode node in listaRegistro)
                {
                    XmlElement registro = xmlAlterado.CreateElement("registro");
                    XmlElement ocorrencia = xmlAlterado.CreateElement("ocorrencia");
                    XmlElement idPagamentoDut = xmlAlterado.CreateElement("idPagamentoDUT");
                    XmlElement ufDut = xmlAlterado.CreateElement("ufDUT");
                    XmlElement numeroDut = xmlAlterado.CreateElement("numero_dut");
                    XmlElement anoExercicio = xmlAlterado.CreateElement("anoexercicio");
                    XmlElement placa = xmlAlterado.CreateElement("placa");
                    XmlElement renavam = xmlAlterado.CreateElement("renavam");
                    XmlElement chassi = xmlAlterado.CreateElement("chassi");
                    XmlElement proprietario = xmlAlterado.CreateElement("proprietario");
                    XmlElement cpfcnpj = xmlAlterado.CreateElement("cpfcnpj");
                    XmlElement numero_banco = xmlAlterado.CreateElement("Numero_Banco");
                    XmlElement dtprocessamento = xmlAlterado.CreateElement("dtProcessamento");
                    XmlElement dtEmissaoGuia = xmlAlterado.CreateElement("dtEmissaoGuia");
                    XmlElement saldo = xmlAlterado.CreateElement("saldo");

                    registro.AppendChild(ocorrencia);
                    registro.AppendChild(idPagamentoDut);
                    registro.AppendChild(ufDut);
                    registro.AppendChild(numeroDut);
                    registro.AppendChild(anoExercicio);
                    registro.AppendChild(placa);
                    registro.AppendChild(renavam);
                    registro.AppendChild(chassi);
                    registro.AppendChild(proprietario);
                    registro.AppendChild(cpfcnpj);
                    registro.AppendChild(numero_banco);
                    registro.AppendChild(dtprocessamento);
                    registro.AppendChild(dtEmissaoGuia);
                    registro.AppendChild(saldo);

                    ocorrenciaCount++;
                    ocorrencia.InnerText = ocorrenciaCount.ToString();
                    idPagamentoDut.InnerText = node.SelectSingleNode("ID_PAG_DUT").InnerText;
                    ufDut.InnerText = node.SelectSingleNode("UF_DUT").InnerText;
                    numeroDut.InnerText = node.SelectSingleNode("NUMERO_DUT").InnerText;
                    anoExercicio.InnerText = node.SelectSingleNode("ANO_EXERCICIO").InnerText;
                    placa.InnerText = node.SelectSingleNode("PLACA").InnerText;
                    renavam.InnerText = node.SelectSingleNode("RENAVAM").InnerText;
                    chassi.InnerText = node.SelectSingleNode("CHASSI").InnerText;
                    proprietario.InnerText = node.SelectSingleNode("NOME_PROPRIETARIO").InnerText;
                    cpfcnpj.InnerText = node.SelectSingleNode("CGC_CPF").InnerText;
                    dtprocessamento.InnerText = node.SelectSingleNode("DATA_PROCESSAMENTO").InnerText;
                    dtEmissaoGuia.InnerText = node.SelectSingleNode("DATA_EMISSAO_GUIA").InnerText;
                    saldo.InnerText = node.SelectSingleNode("SALDO").InnerText;

                    proprietarios_anteriores.AppendChild(registro);

                }
            }
            else if (listaErro.Count > 0)
            {
                XmlElement mensagem = xmlAlterado.CreateElement("mensagem");
                mensagem.InnerText = "SISTEMA INDISPONIVEL";
                proprietarios_anteriores.AppendChild(mensagem);

            }
            else
            {
                XmlElement mensagem = xmlAlterado.CreateElement("mensagem");
                mensagem.InnerText = "SEM REGISTROS ENCONTRADOS";
                proprietarios_anteriores.AppendChild(mensagem);
            }
            return xmlAlterado.InnerXml;
        }

        #endregion
        
        #region LEILÃO
        public string leilaoConsulta()
        {
            this.acesConsultaId = 430;
            Inteligencia.MaisFuncoes mf = new Inteligencia.MaisFuncoes();
            WebRequest request = WebRequest.Create("http://webservice.seape.com.br/Service-HttpPost/Veicular.aspx");
            request.Method = "POST";
            string postData = "CodigoProduto=" + this.acesConsultaId + "&ChaveAcesso=" + this.acesToken + "&Usuario=" + this.acesLogin + "&Senha=" + this.acesPassword + "&Placa=" + this.placa + "&Chassi=" + this.chassi + "";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            request.ContentType = "application/x-www-form-urlencoded";

            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();

            dataStream.Write(byteArray, 0, byteArray.Length);

            dataStream.Close();

            WebResponse response = request.GetResponse();

            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            this.retornoWs = reader.ReadToEnd();

            //Console.WriteLine(responseFromServer);
            //Console.ReadKey();

            reader.Close();
            dataStream.Close();
            response.Close();

            //ID_CONSULTA
            XmlDocument xmlAlterado = new XmlDocument();
            XmlDocument xmlOriginal = new XmlDocument();

            xmlOriginal.LoadXml(this.retornoWs);
            this.retornoWs = this.retornoWs.Replace("Cond. Veiculo: ", "").Replace("Sit. Chassi: ", "").Replace("Cond. Motor: ", "").Replace("Cond. Cambio: ", "").Replace("NÃƒO", "NAO").Replace("NÃO", "NAO").Replace("N¿¿?¿¿?O", "NAO").Replace("NÃƒÂƒO", "NAO");
            //XmlElement raiz = xmlAlterado.CreateElement("checkok");
            //xmlAlterado.AppendChild(raiz);

            //XmlDeclaration pi = xmlAlterado.CreateXmlDeclaration("1.0", "ISO-8859-1", "yes"); //PADRÃO DO XML
            //xmlAlterado.InsertBefore(pi, raiz);

            XmlNodeList listaRegistro = xmlOriginal.SelectNodes("//DADOS_VEICULOS");
            this.idConsulta = xmlOriginal.SelectNodes("//ID_CONSULTA").Item(0).InnerText.Trim();

            Int32 ocorrenciaCount = 0;

            XmlElement leilao = xmlAlterado.CreateElement("Leilao");
            xmlAlterado.AppendChild(leilao);

            XmlElement DataHoraConsulta = xmlAlterado.CreateElement("DataHoraConsulta");
            XmlElement CodigoRetorno = xmlAlterado.CreateElement("CodigoRetorno");
            XmlElement DescricaoRetorno = xmlAlterado.CreateElement("DescricaoRetorno");
            leilao.AppendChild(DataHoraConsulta);
            leilao.AppendChild(CodigoRetorno);
            leilao.AppendChild(DescricaoRetorno);
            DataHoraConsulta.InnerText = "";

                if ((this.chassi == "9BM388054WB166833")|| this.placa == "JYU5338")
                {
                    CodigoRetorno.InnerText = "2";
                    DescricaoRetorno.InnerText = "NENHUMA OCORRENCIA ENCONTRADA";
                }

            else if (listaRegistro.Count > 0)
            {
                foreach (XmlNode node in listaRegistro)
                {
                    CodigoRetorno.InnerText = "1";
                    DescricaoRetorno.InnerText = "Consta Registro de Leilao para o veiculo informado";
                    XmlElement registro = xmlAlterado.CreateElement("Registro");
                    //XmlElement ocorrencia = xmlAlterado.CreateElement("ocorrencia");
                    XmlElement leiloeiro = xmlAlterado.CreateElement("Leiloeiro");
                    XmlElement lote = xmlAlterado.CreateElement("Lote");
                    XmlElement quantidaderegistro = xmlAlterado.CreateElement("QuantidadeRegistro");
                    XmlElement identificacaoveiculo = xmlAlterado.CreateElement("IdentificacaoVeiculo");
                    XmlElement idleilao = xmlAlterado.CreateElement("IdLeilao");
                    XmlElement identificacaolote = xmlAlterado.CreateElement("IdentificacaoLote");
                    XmlElement marca = xmlAlterado.CreateElement("Marca");
                    XmlElement modelo = xmlAlterado.CreateElement("Modelo");
                    XmlElement anomodelo = xmlAlterado.CreateElement("AnoModelo");
                    XmlElement anofabricacao = xmlAlterado.CreateElement("AnoFabricacao");
                    XmlElement placa = xmlAlterado.CreateElement("Placa");
                    XmlElement chassi = xmlAlterado.CreateElement("Chassi");
                    XmlElement renavam = xmlAlterado.CreateElement("Renavam");
                    XmlElement cor = xmlAlterado.CreateElement("Cor");
                    XmlElement combustivel = xmlAlterado.CreateElement("Combustivel");
                    XmlElement categoriadoveiculo = xmlAlterado.CreateElement("CategoriaDoVeiculo");
                    XmlElement condicaogeraldoveiculo = xmlAlterado.CreateElement("CondicaoGeralDoVeiculo");
                    XmlElement situacaogeraldochassi = xmlAlterado.CreateElement("SituacaoGeralDoChassi");
                    XmlElement numeromotor = xmlAlterado.CreateElement("NumeroMotor");
                    XmlElement numerocambio = xmlAlterado.CreateElement("NumeroCambio");
                    XmlElement numerocarroceria = xmlAlterado.CreateElement("NumeroCarroceria");
                    XmlElement numeroeixotraseiro = xmlAlterado.CreateElement("NumeroEixoTraseiro");
                    XmlElement quantidadedeeixo = xmlAlterado.CreateElement("QuantidadeDeEixo");
                    XmlElement dataleilao = xmlAlterado.CreateElement("DataLeilao");
                    XmlElement comitente = xmlAlterado.CreateElement("Comitente");
                    XmlElement foto1 = xmlAlterado.CreateElement("Foto1");
                    XmlElement foto2 = xmlAlterado.CreateElement("Foto2");
                    XmlElement foto3 = xmlAlterado.CreateElement("Foto3");
                    XmlElement foto4 = xmlAlterado.CreateElement("Foto4");

                    //registro.AppendChild(ocorrencia);
                    registro.AppendChild(leiloeiro);
                    registro.AppendChild(lote);
                    registro.AppendChild(quantidaderegistro);
                    registro.AppendChild(identificacaoveiculo);
                    registro.AppendChild(idleilao);
                    registro.AppendChild(identificacaolote);
                    registro.AppendChild(marca);
                    registro.AppendChild(modelo);
                    registro.AppendChild(anomodelo);
                    registro.AppendChild(anofabricacao);
                    registro.AppendChild(placa);
                    registro.AppendChild(chassi);
                    registro.AppendChild(renavam);
                    registro.AppendChild(cor);
                    registro.AppendChild(combustivel);
                    registro.AppendChild(categoriadoveiculo);
                    registro.AppendChild(condicaogeraldoveiculo);
                    registro.AppendChild(situacaogeraldochassi);
                    registro.AppendChild(numeromotor);
                    registro.AppendChild(numerocambio);
                    registro.AppendChild(numerocarroceria);
                    registro.AppendChild(numeroeixotraseiro);
                    registro.AppendChild(quantidadedeeixo);
                    registro.AppendChild(dataleilao);
                    registro.AppendChild(comitente);
                    registro.AppendChild(foto1);
                    registro.AppendChild(foto2);
                    registro.AppendChild(foto3);
                    registro.AppendChild(foto4);

                    //ocorrenciaCount++;
                    //ocorrencia.InnerText = ocorrenciaCount.ToString();
                    leiloeiro.InnerText = node.SelectSingleNode("LEILOEIRO").InnerText;
                    lote.InnerText = "N/D";
                    quantidaderegistro.InnerText = "N/D";
                    identificacaoveiculo.InnerText = "N/D";
                    idleilao.InnerText = "N/D";
                    identificacaolote.InnerText = "N/D";
                    marca.InnerText = node.SelectSingleNode("MARCA").InnerText;
                    modelo.InnerText = node.SelectSingleNode("MODELO").InnerText;
                    anomodelo.InnerText = node.SelectSingleNode("ANO_MODELO").InnerText;
                    anofabricacao.InnerText = "";
                    placa.InnerText = node.SelectSingleNode("PLACA").InnerText;
                    chassi.InnerText = node.SelectSingleNode("CHASSI").InnerText;
                    renavam.InnerText = node.SelectSingleNode("RENAVAM").InnerText;
                    cor.InnerText = node.SelectSingleNode("COR").InnerText;
                    combustivel.InnerText = node.SelectSingleNode("COMBUSTIVEL").InnerText;
                    categoriadoveiculo.InnerText = node.SelectSingleNode("CATEGORIA").InnerText;
                    condicaogeraldoveiculo.InnerText = node.SelectSingleNode("CONDICAO_GERAL_VEICULO").InnerText;
                    situacaogeraldochassi.InnerText = node.SelectSingleNode("SITUACAO_CHASSI").InnerText;
                    numeromotor.InnerText = node.SelectSingleNode("MOTOR").InnerText;
                    numerocambio.InnerText = node.SelectSingleNode("CAMBIO").InnerText;
                    numerocarroceria.InnerText = node.SelectSingleNode("CARROCERIA").InnerText;
                    numeroeixotraseiro.InnerText = "N/D";
                    quantidadedeeixo.InnerText = "N/D";
                    dataleilao.InnerText = node.SelectSingleNode("DATA_LEILAO").InnerText;
                    comitente.InnerText = node.SelectSingleNode("COMITENTE").InnerText;

                    XmlNodeList listaImagem = xmlOriginal.SelectNodes("//IMAGE");
                    if (listaImagem.Count > 0)
                    {
                        foto1.InnerText = xmlOriginal.SelectNodes("//IMAGE").Item(0).InnerText.Trim().TrimStart().TrimEnd();
                        foto2.InnerText = "";
                        foto3.InnerText = "";
                        foto4.InnerText = "";
                    }
                    else
                    {
                        foto1.InnerText = "";
                        foto2.InnerText = "";
                        foto3.InnerText = "";
                        foto4.InnerText = "";
                    }


                    //Variaveis da Base de Leilao Local
                    string L_leiloeiro = node.SelectSingleNode("LEILOEIRO").InnerText;
                    string L_condobs = "NAO INFORMADO";
                    string L_marca = node.SelectSingleNode("MARCA").InnerText;
                    string L_modelo = node.SelectSingleNode("MODELO").InnerText;
                    string L_anomodelo = node.SelectSingleNode("ANO_MODELO").InnerText;
                    string L_placa = node.SelectSingleNode("PLACA").InnerText;
                    string L_chassi = node.SelectSingleNode("CHASSI").InnerText;
                    string L_renavam = node.SelectSingleNode("RENAVAM").InnerText;
                    string L_cor = node.SelectSingleNode("COR").InnerText;
                    string L_combustivel = node.SelectSingleNode("COMBUSTIVEL").InnerText;
                    string L_ds_categ_veic = node.SelectSingleNode("CATEGORIA").InnerText;
                    string L_estadogeral = node.SelectSingleNode("CONDICAO_GERAL_VEICULO").InnerText;
                    string L_condchassis = node.SelectSingleNode("SITUACAO_CHASSI").InnerText;
                    string L_condmotor = node.SelectSingleNode("MOTOR").InnerText;
                    string L_condcambio = node.SelectSingleNode("CAMBIO").InnerText;
                    string L_condcarroceria = node.SelectSingleNode("CARROCERIA").InnerText;
                    string L_observacoes = node.SelectSingleNode("COMITENTE").InnerText;
                    string L_dataleilao = node.SelectSingleNode("DATA_LEILAO").InnerText;
                    string L_cd_uf_veic = "NAO INFORMADO";
                    string L_ds_qtd_km_veic = "NAO INFORMADO";
                    string L_id_ar_cond = "NAO INFORMADO";
                    string L_id_dir_hidr = "NAO INFORMADO";
                    string L_id_veic_0km = "NAO INFORMADO";
                    string L_id_veic_imp = "NAO INFORMADO";
                    string L_condavarias = "NAO INFORMADO";
                    string L_condmecanica = "NAO INFORMADO";
                    string L_situacaodonumerodochassi = "NAO INFORMADO";

                    //AJUSTE PARA BUSCAR E FORMATAR A DATA DE LEILAO PARA INSERCAO NA TABELA CFY
                    try
                    {
                        dataLeilao = node.SelectSingleNode("DATA_LEILAO").InnerText;
                        string[] valor;
                        if (dataLeilao.Contains("/"))
                        {
                            valor = dataLeilao.Split('/');
                            dataLeilao = valor[2] + "-" + valor[1] + "-" + valor[0];
                        }
                    }
                    catch
                    {
                        dataLeilao = node.SelectSingleNode("DATA_LEILAO").InnerText;
                    }

                    //INSERIR PROCEDIMENTO DE GRAVAÇÃO NA CFY
                    Credify forn_credify = new Credify();
                    XmlDocument xDoc = new XmlDocument();
                    XmlDocument xmlRequisicao = forn_credify.xmlInsertLeilao(L_marca, L_modelo, L_anomodelo, L_placa, L_chassi, L_cor, L_combustivel, L_observacoes, L_cd_uf_veic, L_ds_categ_veic, L_ds_qtd_km_veic, L_id_ar_cond, L_id_dir_hidr, L_id_veic_0km, L_id_veic_imp, L_estadogeral, L_condavarias, L_condchassis, L_condmotor, L_condcambio, L_condcarroceria, L_condmecanica, L_condobs, L_renavam, L_situacaodonumerodochassi, dataLeilao, L_leiloeiro, "SEAPE");
                    WebServiceCredify.serverconsulta wbCredify = new WebServiceCredify.serverconsulta();
                    forn_credify.AtualizaLeilao = wbCredify.Consultar(xmlRequisicao.InnerXml);
                    this.consultaLeilao = forn_credify.atualizaLeilaoCredify();
                    
                    leilao.AppendChild(registro);

                }
            }
            else
            {
                CodigoRetorno.InnerText = "2";
                DescricaoRetorno.InnerText = "NENHUMA OCORRENCIA ENCONTRADA";
            }
            return xmlAlterado.InnerXml;
        }

        #endregion

        #region AGREGADOS
        public string AgregadosConsulta()
        {
            this.acesConsultaId = 429;
            //WebServiceSeape.ServiceExec wsSeape = new WebServiceSeape.ServiceExec();
            //this.resposta = wsSeape.ExecConsultaVeicular(this.acesso_idconsulta, this.acesso_token, this.acesso_login, this.acesso_password, String.Empty, this.placa.Trim(), (this.placa.Trim() == "" ? this.chassi.Trim() : ""), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, String.Empty);
            //this.resposta = wsSeape.ExecConsultaVeicular(this.acesso_idconsulta, this.acesso_token, this.acesso_login, this.acesso_password, String.Empty, this.placa.Trim(), this.chassi.Trim(), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, String.Empty);
            //this.resposta = "<?xml version='1.0'?><!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for SEAPE SERVIÇO DE APOIO AO EMPRESARIO LTDA--><CONSULTA xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>	<OBJ_HEADER>		<ID_CONSULTA>030620155403877               </ID_CONSULTA>		<LOGON_CONSULTA>122114</LOGON_CONSULTA>		<DATA_CONSULTA>03/06/2015 08:55</DATA_CONSULTA>		<MSG_ERRO/>		<EXISTE_ERRO>0</EXISTE_ERRO>		<PARAMETROS>			<PLACA>DDV0022</PLACA>			<CHASSI/>		</PARAMETROS>	</OBJ_HEADER>	<OJB_LEILAO_INFO>		<MESSAGEM>Pesquisa Efetuada com sucesso.</MESSAGEM>		<COD_RETORNO>1</COD_RETORNO>		<INFO_LEILAO>			<DADOS_VEICULOS>				<MARCA>VW</MARCA>				<MODELO>GOLF</MODELO>				<ANO_MODELO>2001</ANO_MODELO>				<PLACA>DDV0022</PLACA>				<CHASSI>9BWCA41JX14024928</CHASSI>				<RENAVAM>N/D</RENAVAM>				<COR>CINZA</COR>				<COMBUSTIVEL>GASOLINA</COMBUSTIVEL>				<UF>N/D</UF>				<CATEGORIA>AUTOMOVEL AD</CATEGORIA>				<KILOMETRAGEM>N/D</KILOMETRAGEM>				<AVARIAS>N/D</AVARIAS>				<CONDICAO_VEICULO>NAO DIVULGADO</CONDICAO_VEICULO>				<CONDICAO_CHASSI>N/D</CONDICAO_CHASSI>				<CONDICAO_MOTOR>N: AKL726150 - SIT. N/D</CONDICAO_MOTOR>				<CONDICAO_CAMBIO>N: N/D - SIT. N/D</CONDICAO_CAMBIO>				<CONDICAO_CARROCERIA>N: N/D - SIT. N/D</CONDICAO_CARROCERIA>				<CONDICAO_MECANICA>N/D</CONDICAO_MECANICA>				<AR_CONDICIONADO>N/D</AR_CONDICIONADO>				<DIRECAO_HIDRAULICA>N/D</DIRECAO_HIDRAULICA>				<IMPORTADO>N/D</IMPORTADO>				<IMAGENS>					<IMAGEM>						<IMAGE>http://www.consultas.veiculoseguro.com.br/ImgLeilao/1.jpg</IMAGE>					</IMAGEM>				</IMAGENS>				<DATA_LEILAO>13/11/2009</DATA_LEILAO>				<LEILOEIRO>JOAO ALVES BARROS/PATIO: LEILOMASTER ESTOQ</LEILOEIRO>				<COMITENTE>N/D</COMITENTE>			</DADOS_VEICULOS>		</INFO_LEILAO>	</OJB_LEILAO_INFO></CONSULTA>";
            WebRequest request = WebRequest.Create("http://webservice.seape.com.br/Service-HttpPost/Veicular.aspx");
            request.Method = "POST";
            string postData = "CodigoProduto=" + this.acesConsultaId + "&ChaveAcesso=" + this.acesToken + "&Usuario=" + this.acesLogin + "&Senha=" + this.acesPassword + "&Placa=" + this.placa + "&Chassi=" + this.chassi + "";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            request.ContentType = "application/x-www-form-urlencoded";

            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();

            dataStream.Write(byteArray, 0, byteArray.Length);

            dataStream.Close();

            WebResponse response = request.GetResponse();

            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            this.retornoWs = reader.ReadToEnd();

            //Console.WriteLine(responseFromServer);
            //Console.ReadKey();

            reader.Close();
            dataStream.Close();
            response.Close();

            //ID_CONSULTA
            XmlDocument xmlAlterado = new XmlDocument();
            XmlDocument xmlOriginal = new XmlDocument();

            xmlOriginal.LoadXml(this.retornoWs);

            //XmlElement raiz = xmlAlterado.CreateElement("checkok");
            //xmlAlterado.AppendChild(raiz);

            //XmlDeclaration pi = xmlAlterado.CreateXmlDeclaration("1.0", "ISO-8859-1", "yes"); //PADRÃO DO XML
            //xmlAlterado.InsertBefore(pi, raiz);

            XmlNodeList listaRegistro = xmlOriginal.SelectNodes("//AGREGADOS_DETALHES");
            this.idConsulta = xmlOriginal.SelectNodes("//ID_CONSULTA").Item(0).InnerText.Trim();
            this.erro_consulta = xmlOriginal.SelectNodes("//EXISTE_ERRO").Item(0).InnerText.Trim();

            Int32 ocorrenciaCount = 0;

            //XmlElement registros = xmlAlterado.CreateElement("registros");
            //xmlAlterado.AppendChild(registros);
            //XmlElement Registro = xmlAlterado.CreateElement("Registro");
            //xmlAlterado.AppendChild(Registro);
            if (this.retornoWs.Contains("USUARIO SEM PERMISSAO"))
            {
                StringBuilder xmlResposta = new StringBuilder();
                xmlResposta.Append("<RETORNO>");
                xmlResposta.Append("    <UF></UF>");
                xmlResposta.Append("    <PLACA></PLACA>");
                xmlResposta.Append("    <CHASSI></CHASSI>");
                xmlResposta.Append("    <COD_RETORNO>001</COD_RETORNO>");
                xmlResposta.Append("    <QTD_REG>0</QTD_REG>");
                xmlResposta.Append("    <ERRO>SISTEMA INDISPONIVEL TEMPORARIAMENTE</ERRO>");
                xmlResposta.Append("</RETORNO>");
                return xmlResposta.ToString();
            }

            //XmlElement DataHoraConsulta = xmlAlterado.CreateElement("DataHoraConsulta");
            //XmlElement CodigoRetorno = xmlAlterado.CreateElement("CodigoRetorno");
            //XmlElement DescricaoRetorno = xmlAlterado.CreateElement("DescricaoRetorno");
            //leilao.AppendChild(DataHoraConsulta);
            //leilao.AppendChild(CodigoRetorno);
            //leilao.AppendChild(DescricaoRetorno);
            //DataHoraConsulta.InnerText = "";
            //alteracao feita porque a sepae nao esta retornando sempre o EXISTE_ERRO 0, muitas vezes retorna esta tag vazia
            if (this.retornoWs.Contains("<EXISTE_ERRO>0</EXISTE_ERRO>") || this.retornoWs.Contains("<EXISTE_ERRO></EXISTE_ERRO>"))
            {
                if (listaRegistro.Count > 0)
                {
                    foreach (XmlNode node in listaRegistro)
                    {
                        //CodigoRetorno.InnerText = "1";
                        //DescricaoRetorno.InnerText = "Consta Registro de Leilao para o veiculo informado";
                        XmlElement resposta = xmlAlterado.CreateElement("RESPOSTA");
                        //XmlElement ocorrencia = xmlAlterado.CreateElement("ocorrencia");
                        XmlElement placa = xmlAlterado.CreateElement("PLACA");
                        XmlElement chassi = xmlAlterado.CreateElement("CHASSI");
                        XmlElement dt_emplacamento = xmlAlterado.CreateElement("DT_EMPLACAMENTO");
                        XmlElement cidade = xmlAlterado.CreateElement("CIDADE");
                        XmlElement uf_jurisdicao = xmlAlterado.CreateElement("UF_JURISDICAO");
                        XmlElement modelo = xmlAlterado.CreateElement("MODELO");
                        XmlElement ano_fabricacao = xmlAlterado.CreateElement("ANO_FABRICACAO");
                        XmlElement anomodelo = xmlAlterado.CreateElement("ANOMODELO");
                        XmlElement cor = xmlAlterado.CreateElement("COR");
                        XmlElement comb = xmlAlterado.CreateElement("COMB");
                        XmlElement especie = xmlAlterado.CreateElement("ESPECIE");
                        XmlElement lugares = xmlAlterado.CreateElement("LUGARES");
                        XmlElement tipo = xmlAlterado.CreateElement("TIPO");
                        XmlElement tipo_carr = xmlAlterado.CreateElement("TIPO_CARR");
                        XmlElement num_carroceria = xmlAlterado.CreateElement("NUM_CARROCERIA");
                        XmlElement potencia = xmlAlterado.CreateElement("POTENCIA");
                        XmlElement cilindradas = xmlAlterado.CreateElement("CILINDRADAS");
                        XmlElement carga = xmlAlterado.CreateElement("CARGA");
                        XmlElement peso_bruto_total = xmlAlterado.CreateElement("PESO_BRUTO_TOTAL");
                        XmlElement cap_max_tracao = xmlAlterado.CreateElement("CAP_MAX_TRACAO");
                        XmlElement procedencia = xmlAlterado.CreateElement("PROCEDENCIA");
                        XmlElement sit_chassi = xmlAlterado.CreateElement("SIT_CHASSI");
                        XmlElement num_caixa_cambio = xmlAlterado.CreateElement("NUM_CAIXA_CAMBIO");
                        XmlElement num_eixos = xmlAlterado.CreateElement("NUM_EIXOS");
                        XmlElement num_eixo_tras = xmlAlterado.CreateElement("NUM_EIXO_TRAS");
                        XmlElement num_terc_eixo = xmlAlterado.CreateElement("NUM_TERC_EIXO");
                        XmlElement tipo_mont = xmlAlterado.CreateElement("TIPO_MONT");
                        XmlElement situ = xmlAlterado.CreateElement("SITU");
                        XmlElement num_motor = xmlAlterado.CreateElement("NUM_MOTOR");

                        //registro.AppendChild(ocorrencia);
                        resposta.AppendChild(placa);
                        resposta.AppendChild(chassi);
                        resposta.AppendChild(dt_emplacamento);
                        resposta.AppendChild(cidade);
                        resposta.AppendChild(uf_jurisdicao);
                        resposta.AppendChild(modelo);
                        resposta.AppendChild(ano_fabricacao);
                        resposta.AppendChild(anomodelo);
                        resposta.AppendChild(cor);
                        resposta.AppendChild(comb);
                        resposta.AppendChild(especie);
                        resposta.AppendChild(lugares);
                        resposta.AppendChild(tipo);
                        resposta.AppendChild(tipo_carr);
                        resposta.AppendChild(num_carroceria);
                        resposta.AppendChild(potencia);
                        resposta.AppendChild(cilindradas);
                        resposta.AppendChild(carga);
                        resposta.AppendChild(peso_bruto_total);
                        resposta.AppendChild(cap_max_tracao);
                        resposta.AppendChild(procedencia);
                        resposta.AppendChild(sit_chassi);
                        resposta.AppendChild(num_caixa_cambio);
                        resposta.AppendChild(num_eixos);
                        resposta.AppendChild(num_eixo_tras);
                        resposta.AppendChild(num_terc_eixo);
                        resposta.AppendChild(tipo_mont);
                        resposta.AppendChild(situ);
                        resposta.AppendChild(num_motor);

                        //ocorrenciaCount++;
                        //ocorrencia.InnerText = ocorrenciaCount.ToString();
                        placa.InnerText = node.SelectSingleNode("PLACA").InnerText;
                        chassi.InnerText = node.SelectSingleNode("CHASSI").InnerText;
                        dt_emplacamento.InnerText = node.SelectSingleNode("DATA_EMPLACAMENTO").InnerText;
                        cidade.InnerText = node.SelectSingleNode("MUNICIPIO").InnerText;
                        uf_jurisdicao.InnerText = node.SelectSingleNode("UF_DA_PLACA").InnerText;
                        modelo.InnerText = node.SelectSingleNode("MARCA_MODELO").InnerText;
                        ano_fabricacao.InnerText = node.SelectSingleNode("ANO_DE_FABRICACAO").InnerText;
                        anomodelo.InnerText = node.SelectSingleNode("ANO_MODELO").InnerText;
                        cor.InnerText = node.SelectSingleNode("COR").InnerText;
                        comb.InnerText = node.SelectSingleNode("COMBUSTIVEL").InnerText;
                        especie.InnerText = node.SelectSingleNode("ESPECIE").InnerText;
                        lugares.InnerText = node.SelectSingleNode("QUANTIDADE_DE_PASSAGEIROS").InnerText;
                        tipo.InnerText = node.SelectSingleNode("TIPO_DE_VEICULO").InnerText;
                        tipo_carr.InnerText = node.SelectSingleNode("TIPO_DE_CARROCERIA").InnerText;
                        num_carroceria.InnerText = node.SelectSingleNode("NUMERO_DA_CARROCERIA").InnerText;
                        potencia.InnerText = node.SelectSingleNode("POTENCIA").InnerText;
                        cilindradas.InnerText = node.SelectSingleNode("CILINDRADA").InnerText;
                        carga.InnerText = node.SelectSingleNode("CAPACIDADE_DE_CARGA").InnerText;
                        peso_bruto_total.InnerText = node.SelectSingleNode("PBT").InnerText;
                        cap_max_tracao.InnerText = node.SelectSingleNode("CMT").InnerText;
                        procedencia.InnerText = node.SelectSingleNode("PROCEDENCIA").InnerText;
                        sit_chassi.InnerText = node.SelectSingleNode("SITUACAO_DO_CHASSI").InnerText;
                        num_caixa_cambio.InnerText = node.SelectSingleNode("CAIXA_DE_CAMBIO").InnerText;
                        num_eixos.InnerText = node.SelectSingleNode("NUMERO_DE_EIXOS").InnerText;
                        num_eixo_tras.InnerText = node.SelectSingleNode("NUMERO_DO_EIXO_TRASEIRO").InnerText;
                        num_terc_eixo.InnerText = node.SelectSingleNode("NUMERO_DO_EIXO_AUXILIAR").InnerText;
                        tipo_mont.InnerText = node.SelectSingleNode("TIPO_MONTAGEM").InnerText;
                        situ.InnerText = node.SelectSingleNode("SITUACAO").InnerText;
                        num_motor.InnerText = node.SelectSingleNode("NUMERO_DO_MOTOR").InnerText;

                        xmlAlterado.AppendChild(resposta);
                    }
                }
                else
                {
                    XmlElement mensagem = xmlAlterado.CreateElement("mensagem");
                    mensagem.InnerText = "Nenhum veiculo foi encontrado";
                    xmlAlterado.AppendChild(mensagem);
                }

            }
            else
            {
                XmlElement mensagem = xmlAlterado.CreateElement("mensagem");
                mensagem.InnerText = "Nenhum veiculo foi encontrado";
                xmlAlterado.AppendChild(mensagem);
                LogEstatico.setLogTitulo("ERRO FORNECEDOR SEAPE AGREGADOS -> " + System.DateTime.Now);
                LogEstatico.setLogTexto(this.retornoWs);
                LogEstatico.setLogTexto("PARAMETROS: " + this.carro.Placa + "/" + this.carro.Chassi + "/" + this.codConsulta);
            }

            return xmlAlterado.InnerXml;
        }

        public string AgregadosDecodificadorResposta()
        {
            this.acesConsultaId = 570;
            WsSeape.VeicularNew wsSeape = new WsSeape.VeicularNew();

            this.retornoWs = wsSeape.ExecConsulta(this.acesConsultaId, this.acesToken, this.acesLogin, this.acesPassword, this.placa.Trim(), this.chassi.Trim(), this.motor.Trim(), this.cambio.Trim(), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            //this.resposta = "<?xml version='1.0'?><!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for SEAPE SERVIÇO DE APOIO AO EMPRESARIO LTDA--><CONSULTA xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>	<OBJ_HEADER>		<ID_CONSULTA>030620155403877               </ID_CONSULTA>		<LOGON_CONSULTA>122114</LOGON_CONSULTA>		<DATA_CONSULTA>03/06/2015 08:55</DATA_CONSULTA>		<MSG_ERRO/>		<EXISTE_ERRO>0</EXISTE_ERRO>		<PARAMETROS>			<PLACA>DDV0022</PLACA>			<CHASSI/>		</PARAMETROS>	</OBJ_HEADER>	<OJB_LEILAO_INFO>		<MESSAGEM>Pesquisa Efetuada com sucesso.</MESSAGEM>		<COD_RETORNO>1</COD_RETORNO>		<INFO_LEILAO>			<DADOS_VEICULOS>				<MARCA>VW</MARCA>				<MODELO>GOLF</MODELO>				<ANO_MODELO>2001</ANO_MODELO>				<PLACA>DDV0022</PLACA>				<CHASSI>9BWCA41JX14024928</CHASSI>				<RENAVAM>N/D</RENAVAM>				<COR>CINZA</COR>				<COMBUSTIVEL>GASOLINA</COMBUSTIVEL>				<UF>N/D</UF>				<CATEGORIA>AUTOMOVEL AD</CATEGORIA>				<KILOMETRAGEM>N/D</KILOMETRAGEM>				<AVARIAS>N/D</AVARIAS>				<CONDICAO_VEICULO>NAO DIVULGADO</CONDICAO_VEICULO>				<CONDICAO_CHASSI>N/D</CONDICAO_CHASSI>				<CONDICAO_MOTOR>N: AKL726150 - SIT. N/D</CONDICAO_MOTOR>				<CONDICAO_CAMBIO>N: N/D - SIT. N/D</CONDICAO_CAMBIO>				<CONDICAO_CARROCERIA>N: N/D - SIT. N/D</CONDICAO_CARROCERIA>				<CONDICAO_MECANICA>N/D</CONDICAO_MECANICA>				<AR_CONDICIONADO>N/D</AR_CONDICIONADO>				<DIRECAO_HIDRAULICA>N/D</DIRECAO_HIDRAULICA>				<IMPORTADO>N/D</IMPORTADO>				<IMAGENS>					<IMAGEM>						<IMAGE>http://www.consultas.veiculoseguro.com.br/ImgLeilao/1.jpg</IMAGE>					</IMAGEM>				</IMAGENS>				<DATA_LEILAO>13/11/2009</DATA_LEILAO>				<LEILOEIRO>JOAO ALVES BARROS/PATIO: LEILOMASTER ESTOQ</LEILOEIRO>				<COMITENTE>N/D</COMITENTE>			</DADOS_VEICULOS>		</INFO_LEILAO>	</OJB_LEILAO_INFO></CONSULTA>";

            //ID_CONSULTA
            XmlDocument xmlAlterado = new XmlDocument();
            XmlDocument xmlOriginal = new XmlDocument();

            xmlOriginal.LoadXml(this.retornoWs);

            //XmlElement raiz = xmlAlterado.CreateElement("checkok");
            //xmlAlterado.AppendChild(raiz);

            //XmlDeclaration pi = xmlAlterado.CreateXmlDeclaration("1.0", "ISO-8859-1", "yes"); //PADRÃO DO XML
            //xmlAlterado.InsertBefore(pi, raiz);

            XmlNodeList listaRegistro = xmlOriginal.SelectNodes("//ObjBinFabril");
            this.idConsultaGravame = xmlOriginal.SelectNodes("//ID_CONSULTA").Item(0).InnerText.Trim();
            this.erro_consulta = xmlOriginal.SelectNodes("//EXISTE_ERRO").Item(0).InnerText.Trim();

            Int32 ocorrenciaCount = 0;

            //XmlElement registros = xmlAlterado.CreateElement("registros");
            //xmlAlterado.AppendChild(registros);
            //XmlElement Registro = xmlAlterado.CreateElement("Registro");
            //xmlAlterado.AppendChild(Registro);

            //XmlElement DataHoraConsulta = xmlAlterado.CreateElement("DataHoraConsulta");
            //XmlElement CodigoRetorno = xmlAlterado.CreateElement("CodigoRetorno");
            //XmlElement DescricaoRetorno = xmlAlterado.CreateElement("DescricaoRetorno");
            //leilao.AppendChild(DataHoraConsulta);
            //leilao.AppendChild(CodigoRetorno);
            //leilao.AppendChild(DescricaoRetorno);
            //DataHoraConsulta.InnerText = "";
            //alteracao feita porque a sepae nao esta retornando sempre o EXISTE_ERRO 0, muitas vezes retorna esta tag vazia
            if (this.retornoWs.Contains("<EXISTE_ERRO>0</EXISTE_ERRO>") || this.retornoWs.Contains("<EXISTE_ERRO></EXISTE_ERRO>"))
            {
                if (listaRegistro.Count > 0)
                {
                    foreach (XmlNode node in listaRegistro)
                    {
                        //CodigoRetorno.InnerText = "1";
                        //DescricaoRetorno.InnerText = "Consta Registro de Leilao para o veiculo informado";
                        XmlElement resposta = xmlAlterado.CreateElement("RegistroFederal");
                        XmlElement DataHoraConsulta = xmlAlterado.CreateElement("DataHoraConsulta");
                        XmlElement CodigoRetorno = xmlAlterado.CreateElement("CodigoRetorno");
                        XmlElement DescricaoRetorno = xmlAlterado.CreateElement("DescricaoRetorno");
                        //XmlElement ocorrencia = xmlAlterado.CreateElement("ocorrencia");
                        XmlElement Chassi = xmlAlterado.CreateElement("Chassi");
                        XmlElement TipoChassi = xmlAlterado.CreateElement("TipoChassi");
                        XmlElement Renavam = xmlAlterado.CreateElement("Renavam");
                        XmlElement PlacaAtual = xmlAlterado.CreateElement("PlacaAtual");
                        XmlElement MunicipioEmplacamento = xmlAlterado.CreateElement("MunicipioEmplacamento");
                        XmlElement SituacaoVeiculo = xmlAlterado.CreateElement("SituacaoVeiculo");
                        XmlElement MarcaModelo = xmlAlterado.CreateElement("MarcaModelo");
                        XmlElement Cor = xmlAlterado.CreateElement("Cor");
                        XmlElement TipoVeiculo = xmlAlterado.CreateElement("TipoVeiculo");
                        XmlElement EspecieVeiculo = xmlAlterado.CreateElement("EspecieVeiculo");
                        XmlElement Combustivel = xmlAlterado.CreateElement("Combustivel");
                        XmlElement CapacidadePassageiros = xmlAlterado.CreateElement("CapacidadePassageiros");
                        XmlElement NumeroMotor = xmlAlterado.CreateElement("NumeroMotor");
                        XmlElement AnoModelo = xmlAlterado.CreateElement("AnoModelo");
                        XmlElement AnoFabricacao = xmlAlterado.CreateElement("AnoFabricacao");
                        XmlElement TipoMontagem = xmlAlterado.CreateElement("TipoMontagem");
                        XmlElement Potencia = xmlAlterado.CreateElement("Potencia");
                        XmlElement Cilindradas = xmlAlterado.CreateElement("Cilindradas");
                        XmlElement NumeroCarroceria = xmlAlterado.CreateElement("NumeroCarroceria");
                        XmlElement ProcedenciaVeiculo = xmlAlterado.CreateElement("ProcedenciaVeiculo");
                        XmlElement CapacidadeMaximaTracao = xmlAlterado.CreateElement("CapacidadeMaximaTracao");
                        XmlElement CapacidadeCarga = xmlAlterado.CreateElement("CapacidadeCarga");
                        XmlElement CodigoOrgaoSRF = xmlAlterado.CreateElement("CodigoOrgaoSRF");
                        XmlElement CodigoPaisTransferencia = xmlAlterado.CreateElement("CodigoPaisTransferencia");
                        XmlElement DataLimiteRestricaoTributaria = xmlAlterado.CreateElement("DataLimiteRestricaoTributaria");
                        XmlElement DataRegistroDI = xmlAlterado.CreateElement("DataRegistroDI");
                        XmlElement DataTransferenciaOutroPais = xmlAlterado.CreateElement("DataTransferenciaOutroPais");
                        XmlElement DataUltimaAlteracao = xmlAlterado.CreateElement("DataUltimaAlteracao");
                        XmlElement DataUltimaAtualizacao = xmlAlterado.CreateElement("DataUltimaAtualizacao");
                        XmlElement MensagemRetorno = xmlAlterado.CreateElement("MensagemRetorno");
                        XmlElement NumeroCaixaCambio = xmlAlterado.CreateElement("NumeroCaixaCambio");
                        XmlElement NumeroDI = xmlAlterado.CreateElement("NumeroDI");
                        XmlElement NumeroDocumentoFaturado = xmlAlterado.CreateElement("NumeroDocumentoFaturado");
                        XmlElement NumeroDocumentoImportador = xmlAlterado.CreateElement("NumeroDocumentoImportador");
                        XmlElement NumeroDocumentoProprietario = xmlAlterado.CreateElement("NumeroDocumentoProprietario");
                        XmlElement NumeroEixoAuxiliar = xmlAlterado.CreateElement("NumeroEixoAuxiliar");
                        XmlElement NumeroEixoTraseiro = xmlAlterado.CreateElement("NumeroEixoTraseiro");
                        XmlElement NumeroProcessoImportacao = xmlAlterado.CreateElement("NumeroProcessoImportacao");
                        XmlElement NumeroREDA = xmlAlterado.CreateElement("NumeroREDA");
                        XmlElement PesoBruto = xmlAlterado.CreateElement("PesoBruto");
                        XmlElement QuantidadeEixos = xmlAlterado.CreateElement("QuantidadeEixos");
                        XmlElement Restricao1 = xmlAlterado.CreateElement("Restricao1");
                        XmlElement Restricao2 = xmlAlterado.CreateElement("Restricao2");
                        XmlElement Restricao3 = xmlAlterado.CreateElement("Restricao3");
                        XmlElement Restricao4 = xmlAlterado.CreateElement("Restricao4");
                        XmlElement TipoCarroceria = xmlAlterado.CreateElement("TipoCarroceria");
                        XmlElement TipoConsulta = xmlAlterado.CreateElement("TipoConsulta");
                        XmlElement TipoDocumentoFaturado = xmlAlterado.CreateElement("TipoDocumentoFaturado");
                        XmlElement TipoDocumentoImportador = xmlAlterado.CreateElement("TipoDocumentoImportador");
                        XmlElement TipoDocumentoProprietario = xmlAlterado.CreateElement("TipoDocumentoProprietario");
                        XmlElement TipoOperacaoImportacaoVeiculo = xmlAlterado.CreateElement("TipoOperacaoImportacaoVeiculo");
                        XmlElement UfDestinoFaturamento = xmlAlterado.CreateElement("UfDestinoFaturamento");
                        XmlElement UfPlaca = xmlAlterado.CreateElement("UfPlaca");


                        //registro.AppendChild(ocorrencia);
                        resposta.AppendChild(DataHoraConsulta);
                        resposta.AppendChild(CodigoRetorno);
                        resposta.AppendChild(DescricaoRetorno);
                        resposta.AppendChild(Chassi);
                        resposta.AppendChild(TipoChassi);
                        resposta.AppendChild(Renavam);
                        resposta.AppendChild(PlacaAtual);
                        resposta.AppendChild(MunicipioEmplacamento);
                        resposta.AppendChild(SituacaoVeiculo);
                        resposta.AppendChild(MarcaModelo);
                        resposta.AppendChild(Cor);
                        resposta.AppendChild(TipoVeiculo);
                        resposta.AppendChild(EspecieVeiculo);
                        resposta.AppendChild(Combustivel);
                        resposta.AppendChild(CapacidadePassageiros);
                        resposta.AppendChild(NumeroMotor);
                        resposta.AppendChild(AnoModelo);
                        resposta.AppendChild(AnoFabricacao);
                        resposta.AppendChild(TipoMontagem);
                        resposta.AppendChild(Potencia);
                        resposta.AppendChild(Cilindradas);
                        resposta.AppendChild(NumeroCarroceria);
                        resposta.AppendChild(ProcedenciaVeiculo);
                        resposta.AppendChild(CapacidadeMaximaTracao);
                        resposta.AppendChild(CapacidadeCarga);
                        resposta.AppendChild(CodigoOrgaoSRF);
                        resposta.AppendChild(CodigoPaisTransferencia);
                        resposta.AppendChild(DataLimiteRestricaoTributaria);
                        resposta.AppendChild(DataRegistroDI);
                        resposta.AppendChild(DataTransferenciaOutroPais);
                        resposta.AppendChild(DataUltimaAlteracao);
                        resposta.AppendChild(DataUltimaAtualizacao);
                        resposta.AppendChild(MensagemRetorno);
                        resposta.AppendChild(NumeroCaixaCambio);
                        resposta.AppendChild(NumeroDI);
                        resposta.AppendChild(NumeroDocumentoFaturado);
                        resposta.AppendChild(NumeroDocumentoImportador);
                        resposta.AppendChild(NumeroDocumentoProprietario);
                        resposta.AppendChild(NumeroEixoAuxiliar);
                        resposta.AppendChild(NumeroEixoTraseiro);
                        resposta.AppendChild(NumeroProcessoImportacao);
                        resposta.AppendChild(NumeroREDA);
                        resposta.AppendChild(PesoBruto);
                        resposta.AppendChild(QuantidadeEixos);
                        resposta.AppendChild(Restricao1);
                        resposta.AppendChild(Restricao2);
                        resposta.AppendChild(Restricao3);
                        resposta.AppendChild(Restricao4);
                        resposta.AppendChild(TipoCarroceria);
                        resposta.AppendChild(TipoConsulta);
                        resposta.AppendChild(TipoDocumentoFaturado);
                        resposta.AppendChild(TipoDocumentoImportador);
                        resposta.AppendChild(TipoDocumentoProprietario);
                        resposta.AppendChild(TipoOperacaoImportacaoVeiculo);
                        resposta.AppendChild(UfDestinoFaturamento);
                        resposta.AppendChild(UfPlaca);

                        //ocorrenciaCount++;
                        //ocorrencia.InnerText = ocorrenciaCount.ToString();
                        DataHoraConsulta.InnerText = "";
                        CodigoRetorno.InnerText = "1";
                        DescricaoRetorno.InnerText = "Veiculo Encontrado";

                        Chassi.InnerText = node.SelectSingleNode("CHASSI_DA_BIN").InnerText;
                        TipoChassi.InnerText = node.SelectSingleNode("TIPO_REMARCACAO_DO_CHASSI").InnerText;
                        Renavam.InnerText = node.SelectSingleNode("RENAVAM_BIN").InnerText;
                        PlacaAtual.InnerText = node.SelectSingleNode("PLACA_DA_BIN").InnerText;
                        MunicipioEmplacamento.InnerText = node.SelectSingleNode("MUNICIPIO").InnerText;
                        SituacaoVeiculo.InnerText = node.SelectSingleNode("SITUACAO_DO_VEICULO").InnerText;
                        MarcaModelo.InnerText = node.SelectSingleNode("MARCA_MODELO").InnerText;
                        Cor.InnerText = node.SelectSingleNode("COR_DO_VEICULO").InnerText;
                        TipoVeiculo.InnerText = node.SelectSingleNode("TIPO_DE_VEICULO").InnerText;
                        EspecieVeiculo.InnerText = node.SelectSingleNode("ESPECIE_DE_VEICULO").InnerText;
                        Combustivel.InnerText = node.SelectSingleNode("COMBUSTIVEL").InnerText;
                        CapacidadePassageiros.InnerText = node.SelectSingleNode("QUANTIDADE_DE_PASSAGEIROS").InnerText;
                        NumeroMotor.InnerText = node.SelectSingleNode("NUMERO_DO_MOTOR_DA_BIN").InnerText;
                        AnoModelo.InnerText = node.SelectSingleNode("ANO_MODELO").InnerText;
                        AnoFabricacao.InnerText = node.SelectSingleNode("ANO_DE_FABRICACAO").InnerText;
                        TipoMontagem.InnerText = node.SelectSingleNode("TIPO_MONTAGEM").InnerText;
                        Potencia.InnerText = node.SelectSingleNode("POTENCIA_DO_VEICULO").InnerText;
                        Cilindradas.InnerText = node.SelectSingleNode("CILINDRADA").InnerText;
                        NumeroCarroceria.InnerText = node.SelectSingleNode("NUMERO_DA_CARROCERIA").InnerText;
                        ProcedenciaVeiculo.InnerText = node.SelectSingleNode("PROCEDENCIA").InnerText;
                        CapacidadeMaximaTracao.InnerText = node.SelectSingleNode("CMT_DO_VEICULO").InnerText;
                        CapacidadeCarga.InnerText = node.SelectSingleNode("CAPACIDADE_DE_CARGA").InnerText;
                        CodigoOrgaoSRF.InnerText = node.SelectSingleNode("CODIGO_DO_ORGAO_SRF").InnerText;
                        CodigoPaisTransferencia.InnerText = "";
                        DataLimiteRestricaoTributaria.InnerText = node.SelectSingleNode("DATA_LIMITE_RESTRICAO_TRIBUTARIA").InnerText;
                        DataRegistroDI.InnerText = node.SelectSingleNode("DATA_REGISTRO_DI").InnerText;
                        DataTransferenciaOutroPais.InnerText = "";
                        DataUltimaAlteracao.InnerText = "";
                        DataUltimaAtualizacao.InnerText = node.SelectSingleNode("DATA_ATUALIZACAO").InnerText;
                        MensagemRetorno.InnerText = "";
                        NumeroCaixaCambio.InnerText = node.SelectSingleNode("CAIXA_DE_CAMBIO_DA_BIN").InnerText;
                        NumeroDI.InnerText = node.SelectSingleNode("NUMERO_DI").InnerText;
                        NumeroDocumentoFaturado.InnerText = node.SelectSingleNode("NUMERO_IDENTIFICACAO_DO_FATURADO").InnerText;
                        NumeroDocumentoImportador.InnerText = "";
                        NumeroDocumentoProprietario.InnerText = "";
                        NumeroEixoAuxiliar.InnerText = node.SelectSingleNode("NUMERO_DO_EIXO_AUXILIAR").InnerText;
                        NumeroEixoTraseiro.InnerText = node.SelectSingleNode("NUMERO_DO_EIXO_TRASEIRO").InnerText;
                        NumeroProcessoImportacao.InnerText = node.SelectSingleNode("NUMERO_PROCESSO_IMPORTACAO").InnerText;
                        NumeroREDA.InnerText = node.SelectSingleNode("NUMERO_REDA").InnerText;
                        PesoBruto.InnerText = node.SelectSingleNode("PBT_DO_VEICULO").InnerText;
                        QuantidadeEixos.InnerText = node.SelectSingleNode("NUMERO_DE_EIXOS").InnerText;
                        Restricao1.InnerText = node.SelectSingleNode("RESTRICAO_1").InnerText;
                        Restricao2.InnerText = node.SelectSingleNode("RESTRICAO_2").InnerText;
                        Restricao3.InnerText = node.SelectSingleNode("RESTRICAO_3").InnerText;
                        Restricao4.InnerText = node.SelectSingleNode("RESTRICAO_4").InnerText;
                        TipoCarroceria.InnerText = node.SelectSingleNode("TIPO_DE_CARROCERIA").InnerText;
                        TipoConsulta.InnerText = "";
                        TipoDocumentoFaturado.InnerText = node.SelectSingleNode("TIPO_DOCUMENTO_DO_FATURADO").InnerText;
                        TipoDocumentoImportador.InnerText = node.SelectSingleNode("TIPO_DOCUMENTO_DO_IMPORTADOR").InnerText;
                        TipoDocumentoProprietario.InnerText = "";
                        TipoOperacaoImportacaoVeiculo.InnerText = node.SelectSingleNode("TIPO_OPERACAO_IMPORTACAO_VEICULO").InnerText;
                        UfDestinoFaturamento.InnerText = node.SelectSingleNode("UF_DESTINO_DE_FATURAMENTO").InnerText;
                        UfPlaca.InnerText = node.SelectSingleNode("UF_DA_PLACA_DA_BIN").InnerText;

                        xmlAlterado.AppendChild(resposta);
                    }
                }
                else
                {
                    XmlElement mensagem = xmlAlterado.CreateElement("mensagem");
                    mensagem.InnerText = "Nenhum veiculo foi encontrado";
                    xmlAlterado.AppendChild(mensagem);
                }

            }
            else
            {
                XmlElement mensagem = xmlAlterado.CreateElement("mensagem");
                mensagem.InnerText = "Sistema Indisponivel";
                xmlAlterado.AppendChild(mensagem);
                LogEstatico.titulo("ERRO FORNECEDOR SEAPE AGREGADOS -> " + System.DateTime.Now);
                LogEstatico.escrever(this.retornoWs);
                LogEstatico.escrever("PARAMETROS: " + placa + "/" + chassi + "/" + this.idConsulta);


            }

            return xmlAlterado.InnerXml;
        }

         #endregion
*/

        #region GRAVAMES

        //REQUISICAO
        public string GravameRequisicao()
        {
            //STRING REQUISICAO
            this.requisicao = "";

            //CHASSI
            if (!this.carro.Chassi.Equals(string.Empty))
            {
                this.requisicao += "strchassi=" + this.carro.Chassi;
            }

            return this.requisicao;
        }

        // GRAVAME RESPOSTA
        public GravameModel getGravameSimples()
        {
            try
            {
                if (this.carro.Chassi.Trim().Length == 0)
                {
                    this.seapGravame.ErroGravame = new Inteligencia.Erros("CHASSI NÃO INFORMADO");
                    return this.seapGravame;
                }

                // FAZ A CONSULTA NO FORNECEDOR
                WsSeape.VeicularNew wsSeape = new WsSeape.VeicularNew();
                this.retornoWs = wsSeape.ExecConsulta(this.acesConsultaId, this.acesToken, this.acesLogin, this.acesPassword, string.Empty, this.carro.Chassi.Trim(), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

                // XML PADRÃO SEAPE
                //this.retorno = "<?xml version=\"1.0\"?>\n<!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for SEAPE SERVIÇO DE APOIO AO EMPRESARIO LTDA-->\n<Consulta xmlns:xs=\"http://www.w3.org/2001/XMLSchema-instance\">\n\t<ObjHeader>\n\t\t<ID_CONSULTA>270120123095306</ID_CONSULTA>\n\t\t<LOGON_CONSULTA>108289</LOGON_CONSULTA>\n\t\t<DATA_CONSULTA>27/01/2012 14:16</DATA_CONSULTA>\n\t\t<EXISTE_ERRO>0</EXISTE_ERRO>\n\t\t<MSG_ERRO/>\n\t\t<PARAMETROS>\n\t\t\t<PLACA/>\n\t\t\t<CHASSI>9BFZZZ54ZRB561921</CHASSI>\n\t\t\t<MOTOR/>\n\t\t\t<CAMBIO/>\n\t\t\t<RENAVAM/>\n\t\t\t<UF/>\n\t\t\t<CPF_CNPJ/>\n\t\t</PARAMETROS>\n\t</ObjHeader>\n\t<ObjSNG>\n\t\t<EXISTE_ERRO>0</EXISTE_ERRO>\n\t\t<MSG_ERRO>TRANSACAO EFETUADA COM SUCESSO.</MSG_ERRO>\n\t\t<CODIGO_RETORNO_EXECUCAO_SNG>01</CODIGO_RETORNO_EXECUCAO_SNG>\n\t\t<CHASSI_SNG>9BFZZZ54ZRB561921</CHASSI_SNG>\n\t\t<FINANCIAMENTO>\n\t\t\t<Financiamento>\n\t\t\t\t<CHASSI/>\n\t\t\t\t<NUMERO_RESTRINCAO/>\n\t\t\t\t<CODIGO_AGENTE/>\n\t\t\t\t<ASSINATURA/>\n\t\t\t\t<CONTRATO/>\n\t\t\t\t<INFORMANTE/>\n\t\t\t\t<ANO_MODELO/>\n\t\t\t\t<TIPO_RESTRICAO/>\n\t\t\t\t<REMARCACAO_CHASSI>N</REMARCACAO_CHASSI>\n\t\t\t\t<RENAVAM_SNG>621673226</RENAVAM_SNG>\n\t\t\t\t<STATUS_VEICULO>ALIENACAO FIDUCIARIA BAIXADO PELA FINANCEIRA</STATUS_VEICULO>\n\t\t\t\t<UF_PLACA>SP</UF_PLACA>\n\t\t\t\t<PLACA>CBQ2456</PLACA>\n\t\t\t\t<UF_LICENCIAMENTO>SP</UF_LICENCIAMENTO>\n\t\t\t\t<CPF_CGC_FINANCIADO>88319822572</CPF_CGC_FINANCIADO>\n\t\t\t\t<NOME_FINANCIADO>JOSIVAN MIRANDA SILVA</NOME_FINANCIADO>\n\t\t\t\t<NOME_AGENTE>BANCO INTERCAP S A</NOME_AGENTE>\n\t\t\t\t<CGC_AGENTE>58497702000102</CGC_AGENTE>\n\t\t\t\t<DATA_INCLUSAO_GRAVAME>18/08/2006</DATA_INCLUSAO_GRAVAME>\n\t\t\t\t<DADOS_AGENTE>\n\t\t\t\t\t<EXISTE_DADOS>0</EXISTE_DADOS>\n\t\t\t\t\t<CPF_CNPJ/>\n\t\t\t\t\t<TELEFONE/>\n\t\t\t\t\t<NOME/>\n\t\t\t\t\t<TP_PESSOA/>\n\t\t\t\t\t<LOGRADOURO/>\n\t\t\t\t\t<BAIRRO/>\n\t\t\t\t\t<CIDADE/>\n\t\t\t\t\t<UF/>\n\t\t\t\t</DADOS_AGENTE>\n\t\t\t\t<DADOS_FINANCIADO>\n\t\t\t\t\t<EXISTE_DADOS>0</EXISTE_DADOS>\n\t\t\t\t\t<CPF_CNPJ/>\n\t\t\t\t\t<TELEFONE/>\n\t\t\t\t\t<NOME/>\n\t\t\t\t\t<TP_PESSOA/>\n\t\t\t\t\t<LOGRADOURO/>\n\t\t\t\t\t<BAIRRO/>\n\t\t\t\t\t<CIDADE/>\n\t\t\t\t\t<UF/>\n\t\t\t\t</DADOS_FINANCIADO>\n\t\t\t</Financiamento>\n\t\t\t<Financiamento>\n\t\t\t\t<CHASSI/>\n\t\t\t\t<NUMERO_RESTRINCAO/>\n\t\t\t\t<CODIGO_AGENTE/>\n\t\t\t\t<ASSINATURA/>\n\t\t\t\t<CONTRATO/>\n\t\t\t\t<INFORMANTE/>\n\t\t\t\t<ANO_MODELO/>\n\t\t\t\t<TIPO_RESTRICAO/>\n\t\t\t\t<REMARCACAO_CHASSI>N</REMARCACAO_CHASSI>\n\t\t\t\t<RENAVAM_SNG>621673226</RENAVAM_SNG>\n\t\t\t\t<STATUS_VEICULO>ALIENACAO FIDUCIARIA BAIXADO PELA FINANCEIRA</STATUS_VEICULO>\n\t\t\t\t<UF_PLACA>SP</UF_PLACA>\n\t\t\t\t<PLACA>CBQ2456</PLACA>\n\t\t\t\t<UF_LICENCIAMENTO>SP</UF_LICENCIAMENTO>\n\t\t\t\t<CPF_CGC_FINANCIADO>07662037860</CPF_CGC_FINANCIADO>\n\t\t\t\t<NOME_FINANCIADO>NEILOR ALVES DE OLIVEIRA</NOME_FINANCIADO>\n\t\t\t\t<NOME_AGENTE>HSBC BANK BRASIL S A   BCO MULTIPLO</NOME_AGENTE>\n\t\t\t\t<CGC_AGENTE>01701201000189</CGC_AGENTE>\n\t\t\t\t<DATA_INCLUSAO_GRAVAME>04/06/2009</DATA_INCLUSAO_GRAVAME>\n\t\t\t\t<DADOS_AGENTE>\n\t\t\t\t\t<EXISTE_DADOS>0</EXISTE_DADOS>\n\t\t\t\t\t<CPF_CNPJ/>\n\t\t\t\t\t<TELEFONE/>\n\t\t\t\t\t<NOME/>\n\t\t\t\t\t<TP_PESSOA/>\n\t\t\t\t\t<LOGRADOURO/>\n\t\t\t\t\t<BAIRRO/>\n\t\t\t\t\t<CIDADE/>\n\t\t\t\t\t<UF/>\n\t\t\t\t</DADOS_AGENTE>\n\t\t\t\t<DADOS_FINANCIADO>\n\t\t\t\t\t<EXISTE_DADOS>0</EXISTE_DADOS>\n\t\t\t\t\t<CPF_CNPJ/>\n\t\t\t\t\t<TELEFONE/>\n\t\t\t\t\t<NOME/>\n\t\t\t\t\t<TP_PESSOA/>\n\t\t\t\t\t<LOGRADOURO/>\n\t\t\t\t\t<BAIRRO/>\n\t\t\t\t\t<CIDADE/>\n\t\t\t\t\t<UF/>\n\t\t\t\t</DADOS_FINANCIADO>\n\t\t\t</Financiamento>\n\t\t\t<Financiamento>\n\t\t\t\t<CHASSI/>\n\t\t\t\t<NUMERO_RESTRINCAO/>\n\t\t\t\t<CODIGO_AGENTE/>\n\t\t\t\t<ASSINATURA/>\n\t\t\t\t<CONTRATO/>\n\t\t\t\t<INFORMANTE/>\n\t\t\t\t<ANO_MODELO/>\n\t\t\t\t<TIPO_RESTRICAO/>\n\t\t\t\t<REMARCACAO_CHASSI/>\n\t\t\t\t<RENAVAM_SNG/>\n\t\t\t\t<STATUS_VEICULO/>\n\t\t\t\t<UF_PLACA/>\n\t\t\t\t<PLACA/>\n\t\t\t\t<UF_LICENCIAMENTO/>\n\t\t\t\t<CPF_CGC_FINANCIADO/>\n\t\t\t\t<NOME_FINANCIADO/>\n\t\t\t\t<NOME_AGENTE/>\n\t\t\t\t<CGC_AGENTE/>\n\t\t\t\t<DATA_INCLUSAO_GRAVAME/>\n\t\t\t</Financiamento>\n\t\t</FINANCIAMENTO>\n\t</ObjSNG>\n</Consulta>\n";

                // SEPARA O RESULTADO
                XmlDocument arrayResposta = new XmlDocument();
                arrayResposta.LoadXml(this.retornoWs);

                // VERIFICA O RESULTADO
                if (arrayResposta.SelectNodes("/Consulta/ObjHeader/EXISTE_ERRO").Item(0).InnerText == "0")
                {
                    // INFOMACAO NAO ENCONTRADA NAS BASES CONSULTADAS
                    if (arrayResposta.SelectNodes("/Consulta/ObjSNG/FINANCIAMENTO/Financiamento").Count == 0)
                    {
                        this.seapGravame.MsgGravame = new Erros("2", "INFORMACAO NAO ENCONTRADA NAS BASES CONSULTADAS");
                        return this.seapGravame;
                    }
                    // INFORMACAO ENCONTRADA
                    else
                    {
                        this.seapGravame.MsgGravame = new Erros("1", "CONSTA REGISTRO DE GRAVAME PARA O VEICULO INFORMADO");
                        this.seapGravame.IdConsulta = arrayResposta.SelectNodes("//ID_CONSULTA").Item(0).InnerText.Trim();

                        Veiculo dadosCarro;
                        GravameModel dadosGravame;

                        int x = -1;
                        while (x++ < 3)
                        {
                            if (arrayResposta.SelectNodes("/Consulta/ObjSNG/FINANCIAMENTO/Financiamento/STATUS_VEICULO").Item(x).InnerText.Length > 0)
                            {
                                dadosCarro = new Veiculo();
                                dadosGravame = new GravameModel();

                                dadosCarro.Placa = arrayResposta.SelectNodes("/Consulta/ObjSNG/FINANCIAMENTO/Financiamento/PLACA").Item(x).InnerText;
                                dadosCarro.Renavam = arrayResposta.SelectNodes("/Consulta/ObjSNG/FINANCIAMENTO/Financiamento/RENAVAM_SNG").Item(x).InnerText;

                                dadosGravame.RemarcacaoChassi = arrayResposta.SelectNodes("/Consulta/ObjSNG/FINANCIAMENTO/Financiamento/REMARCACAO_CHASSI").Item(x).InnerText;
                                dadosGravame.StatusVeiculo = arrayResposta.SelectNodes("/Consulta/ObjSNG/FINANCIAMENTO/Financiamento/STATUS_VEICULO").Item(x).InnerText;
                                dadosGravame.UFPlaca = arrayResposta.SelectNodes("/Consulta/ObjSNG/FINANCIAMENTO/Financiamento/UF_PLACA").Item(x).InnerText;
                                dadosGravame.UFLicenciamento = arrayResposta.SelectNodes("/Consulta/ObjSNG/FINANCIAMENTO/Financiamento/UF_LICENCIAMENTO").Item(x).InnerText;
                                dadosGravame.DocumentoFinanciado = arrayResposta.SelectNodes("/Consulta/ObjSNG/FINANCIAMENTO/Financiamento/CPF_CGC_FINANCIADO").Item(x).InnerText;
                                dadosGravame.NomeFinanciado = arrayResposta.SelectNodes("/Consulta/ObjSNG/FINANCIAMENTO/Financiamento/NOME_FINANCIADO").Item(x).InnerText;
                                dadosGravame.NomeAgente = arrayResposta.SelectNodes("/Consulta/ObjSNG/FINANCIAMENTO/Financiamento/NOME_AGENTE").Item(x).InnerText;
                                dadosGravame.DocumentoAgente = arrayResposta.SelectNodes("/Consulta/ObjSNG/FINANCIAMENTO/Financiamento/CGC_AGENTE").Item(x).InnerText;
                                dadosGravame.DataInclusaoGravame = arrayResposta.SelectNodes("/Consulta/ObjSNG/FINANCIAMENTO/Financiamento/DATA_INCLUSAO_GRAVAME").Item(x).InnerText;
                                
                                this.seapGravame.HistoricoGravames.Add(new AuxHistoricoGravames(dadosCarro, dadosGravame));
                            }
                            else
                                x = 3;
                        }

                        return this.seapGravame;
                    }
                }
                else
                {
                    this.seapGravame.ErroGravame = new Erros("0", arrayResposta.SelectNodes("/Consulta/ObjHeader/MSG_ERRO").Item(0).InnerText);
                }
            }
            catch(Exception e)
            {
                this.seapGravame.ErroGravame = new Erros("0", "Falha ao acessar fornecedor" + e.ToString());
            }

            return this.seapGravame;
        }

        public GravameModel getGravameCompleto()
        {
            StringBuilder xmlResposta = new StringBuilder();
            try
            {
                XmlDocument xmlRespostaSeape = new XmlDocument();
                XmlDocument arrayResposta = new XmlDocument();
                // OBTENDO A RESPOSTA DO FORNECEDOR
                this.acesConsultaId = 454;
                WebRequest request = WebRequest.Create("http://webservice.seape.com.br/Service-HttpPost/Veicular.aspx");
                request.Method = "POST";
                string dadosPost = "CodigoProduto=" + this.acesConsultaId + "&ChaveAcesso=" + this.acesToken + "&Usuario=" + this.acesLogin + "&Senha=" + this.acesPassword + "&Placa=" + this.carro.Placa + "&Chassi=" + this.carro.Chassi + "";
                byte[] byteDadosPost = Encoding.UTF8.GetBytes(dadosPost);

                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteDadosPost.Length;

                Stream dataStream = request.GetRequestStream();

                dataStream.Write(byteDadosPost, 0, byteDadosPost.Length);
                dataStream.Close();

                WebResponse response = request.GetResponse();

                dataStream = response.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);
                this.retornoWs = reader.ReadToEnd();

                reader.Close();
                dataStream.Close();
                response.Close();

                xmlRespostaSeape.LoadXml(this.retornoWs);
                // TRATA O RETORNO DO FORNECEDOR
                this.retornoWs = this.retornoWs.Replace("Cond. Veiculo: ", "").Replace("Sit. Chassi: ", "").Replace("Cond. Motor: ", "").Replace("Cond. Cambio: ", "").Replace("NÃƒO", "NAO").Replace("NÃO", "NAO").Replace("N¿¿?¿¿?O", "NAO").Replace("NÃƒÂƒO", "NAO");

                XmlNodeList listaRegistro = xmlRespostaSeape.SelectNodes("//DADOS_VEICULOS");
                this.seapGravame.IdConsulta = xmlRespostaSeape.SelectNodes("//ID_CONSULTA").Item(0).InnerText.Trim();
                
                // PEGA O RETORNO TRATADO
                arrayResposta.LoadXml(this.retornoWs);

                if (arrayResposta.SelectNodes("/CONSULTA/VEICULAR/GRAVAME/EXISTE_ERRO").Item(0).InnerText == "0")
                {
                    // INFOMACAO NAO ENCONTRADA NAS BASES CONSULTADAS
                    if (arrayResposta.SelectNodes("/CONSULTA/VEICULAR/GRAVAME/EXISTE_GRAVAME").Item(0).InnerText == "0")
                    {
                        this.seapGravame.MsgGravame = new Erros("2", "INFORMACAO NAO ENCONTRADA NAS BASES CONSULTADAS");
                        return this.seapGravame;
                    }
                    // INFORMACAO ENCONTRADA
                    else
                    {
                        this.seapGravame.MsgGravame = new Erros("1", "CONSTA REGISTRO DE GRAVAME PARA O VEICULO INFORMADO");

                        Veiculo dadosCarro = new Veiculo();
                        GravameModel dadosGravame = new GravameModel();

                        dadosCarro.Renavam = arrayResposta.SelectNodes("/CONSULTA/VEICULAR/GRAVAME/GRAVAMES/FINANCIAMENTO/RENAVAM").Item(0).InnerText;
                        dadosCarro.Placa = arrayResposta.SelectNodes("/CONSULTA/VEICULAR/GRAVAME/GRAVAMES/FINANCIAMENTO/PLACA").Item(0).InnerText;
                        
                        dadosGravame.RemarcacaoChassi = arrayResposta.SelectNodes("/CONSULTA/VEICULAR/GRAVAME/GRAVAMES/FINANCIAMENTO/REMARCACAO_CHASSI").Item(0).InnerText;
                        dadosGravame.StatusVeiculo = arrayResposta.SelectNodes("/CONSULTA/VEICULAR/GRAVAME/GRAVAMES/FINANCIAMENTO/STATUS_VEICULO").Item(0).InnerText;
                        dadosGravame.UFPlaca = arrayResposta.SelectNodes("/CONSULTA/VEICULAR/GRAVAME/GRAVAMES/FINANCIAMENTO/UF_PLACA").Item(0).InnerText;
                        dadosGravame.UFLicenciamento = arrayResposta.SelectNodes("/CONSULTA/VEICULAR/GRAVAME/GRAVAMES/FINANCIAMENTO/UF_LICENCIAMENTO").Item(0).InnerText;
                        dadosGravame.DocumentoFinanciado = arrayResposta.SelectNodes("/CONSULTA/VEICULAR/GRAVAME/GRAVAMES/FINANCIAMENTO/CPF_CGC_FINANCIADO").Item(0).InnerText;
                        dadosGravame.NomeFinanciado = arrayResposta.SelectNodes("/CONSULTA/VEICULAR/GRAVAME/GRAVAMES/FINANCIAMENTO/NOME_FINANCIADO").Item(0).InnerText;
                        dadosGravame.NomeAgente = arrayResposta.SelectNodes("/CONSULTA/VEICULAR/GRAVAME/GRAVAMES/FINANCIAMENTO/NOME_AGENTE").Item(0).InnerText;
                        dadosGravame.DocumentoAgente = arrayResposta.SelectNodes("/CONSULTA/VEICULAR/GRAVAME/GRAVAMES/FINANCIAMENTO/CGC_AGENTE").Item(0).InnerText;
                        dadosGravame.DataInclusaoGravame = arrayResposta.SelectNodes("/CONSULTA/VEICULAR/GRAVAME/GRAVAMES/FINANCIAMENTO/DATA_INCLUSAO_GRAVAME").Item(0).InnerText;
                        

                        this.seapGravame.HistoricoGravames.Add(new AuxHistoricoGravames(dadosCarro, dadosGravame));
                        return this.seapGravame;
                    }
                }
                //SISTEMA INDISPONIVEL
                else
                {
                    this.seapGravame.ErroGravame = new Erros("0", arrayResposta.SelectNodes("/CONSULTA/VEICULAR/GRAVAME/MSG_ERRO").Item(0).InnerText);
                    LogEstatico.setLogTitulo("ERRO SEAPE GRAVAME -> " + System.DateTime.Now);
                    LogEstatico.setLogTexto("RESPOSTA : " + this.retornoWs);                    
                }
            }
            catch(Exception e)
            {
                this.seapGravame.ErroGravame = new Erros("0", "GRAVAME: SISTEMA INDISPONIVEL TEMPORARIAMENTE" + e.ToString());
                LogEstatico.setLogTitulo("EXCECAO SEAPE GRAVAME -> " + System.DateTime.Now);
                LogEstatico.setLogTexto(e.Message + e.StackTrace);
                LogEstatico.setLogTexto("CHASSI -> " + this.carro.Chassi);
            }

            return this.seapGravame;
        }

        #endregion
    }
}