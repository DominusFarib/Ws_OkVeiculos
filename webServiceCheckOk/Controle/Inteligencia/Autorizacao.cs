using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using webServiceCheckOk.BaseDados;

namespace webServiceCheckOk.Controle.Inteligencia
{
    public class Autorizacao
    {
    #region VARIAVEIS
       
        private DataBases baseDados = new DataBases();
        private OracleCommand bdComando = new OracleCommand();
        private OracleDataReader bdResposta;
        private OracleTransaction bdTransacao;
        public Erros erro;
        public string valorConsulta = string.Empty;
        public string planoLogon = string.Empty;
        public string idAgenda { get; set; }
        public bool travaPrecificador;
        public bool travaLeilao;
        public bool travaPerdaTotal;
        private bool flagCobrar = true;
        private string logon = string.Empty;
        private string senha = string.Empty; 
        private string codSubTransacao = string.Empty;
        private string transacao = string.Empty;

    #endregion

        // CONSTRUTOR
        public Autorizacao(string logon, string senha, string codTransacao)
        {
            //autenticao = false;
            //validacpf = false;

            this.logon = logon;
            this.senha = senha.Insert(0,"S");
            this.codSubTransacao = codTransacao;
            this.transacao = getTransacao();
            //this.buffer = this.codTransacao + "|" + this.transaction() + "|LOGON" + this.logon + this.senha + "|CPF" + this.cpf + "|PESSOA" + this.doctype;

        }

        // VERIFICA AS AUTORIZAÇÕES DO USUARIO
        public void verificaAutorizacao()
        {
            string planoLogon = String.Empty;

            this.baseDados = new DataBases();
            this.bdComando = this.baseDados.Conn.CreateCommand();
            System.Data.ConnectionState estatusConexao = this.bdComando.Connection.State;

            try
            {
                //LOGON

                this.bdComando.CommandText = "SELECT * FROM LOGON WHERE LOGON = '" + this.logon + "'";
                this.bdComando.CommandType = CommandType.Text;
                this.bdComando.Connection = this.baseDados.Conn;

                bdResposta = bdComando.ExecuteReader();

                if (bdResposta.Read())
                {
                    planoLogon = bdResposta["PLANO"].ToString();

                    switch (planoLogon)
                    {
                        //CONTROLE DE QUANTIDADE DE CONSULTAS POR LOGON
                        case "3":
                            if (Convert.ToInt32(bdResposta["QTD_CONS"].ToString()) > 0)
                            {
                                using (this.baseDados.Conn)
                                {
                                    OracleCommand command = new OracleCommand("UPDATE LOGON SET QTD_CONS = QTD_CONS-1 WHERE LOGON = '" + this.logon + "'", this.baseDados.Conn);
                                    this.bdTransacao = this.baseDados.Conn.BeginTransaction();
                                    command.Transaction = bdTransacao;
                                    try
                                    {
                                        command.ExecuteNonQuery();
                                        this.bdTransacao.Commit();
                                    }
                                    catch
                                    {
                                        command.Dispose();
                                        this.bdTransacao.Rollback();
                                        setErroAutorizacao(8);
                                    }
                                }
                            }
                            else
                            {
                                setErroAutorizacao(15);
                            }

                            bdResposta.Dispose();
                            bdResposta.Close();
                            this.baseDados.Conn.Dispose();
                            this.baseDados.Conn.Close();
                            bdComando.Dispose();
                            break;

                        //CONTROLE POR QUANTIDADE POR LOGON (SEM BLOQUEIO)
                        case "7":
                            using (this.baseDados.Conn)
                            {
                                OracleCommand command = new OracleCommand("UPDATE LOGON SET QTD_CONS = QTD_CONS-1 WHERE LOGON = '" + this.logon + "'", this.baseDados.Conn);
                                this.bdTransacao = this.baseDados.Conn.BeginTransaction();
                                command.Transaction = bdTransacao;
                                try
                                {
                                    command.ExecuteNonQuery();
                                    this.bdTransacao.Commit();
                                }
                                catch
                                {
                                    command.Dispose();
                                    this.bdTransacao.Rollback();
                                    setErroAutorizacao(8);
                                }
                            }

                            bdResposta.Dispose();
                            bdResposta.Close();
                            this.baseDados.Conn.Dispose();
                            this.baseDados.Conn.Close();
                            bdComando.Dispose();
                            break;

                        //CONTROLE POR VALOR POR LOGON
                        case "5":
                            using (this.baseDados.Conn)
                            {
                                OracleCommand command = new OracleCommand("SELECT C.VLR_UNITARIO FROM CHECKOK.RELAC_TRANSACOES A, CHECKOK.TARIFA B, CHECKOK.TABPRECO_D C, CHECKOK.LOGONS D WHERE A.SERVICO_BO = B.SERVICO AND NVL(A.SUBTRANS_BO, '0') = NVL(B.SUBTRANSACAO, '0') AND B.DEBITO = C.COD_TIPDEBITO AND D.COD_ENTIDADE = B.CLIENTE AND C.COD_TABPRECO = 5 AND A.MEIO_ACESSO = 9 AND A.SERVICO_BO <> 'IM00' AND A.TRANS_SAF = '" + this.transacao + "' AND A.SUBTRANS_SAF = '" + this.codSubTransacao + "' AND D.LOGON LIKE '" + this.logon + "%' AND ROWNUM = 1 ORDER BY C.COD_TABPRECO DESC", this.baseDados.Conn);
                                OracleDataReader respostaValor = command.ExecuteReader();

                                if (respostaValor.Read())
                                {
                                    if (Convert.ToDouble(bdResposta["VALOR_CONS"].ToString()) >= Convert.ToDouble(respostaValor["VLR_UNITARIO"].ToString()))
                                    {
                                        if (this.flagCobrar == true)
                                        {
                                            OracleCommand comandoAux = new OracleCommand("UPDATE LOGON SET VALOR_CONS = VALOR_CONS - " + respostaValor["VLR_UNITARIO"].ToString().Replace(",", ".") + " WHERE LOGON = '" + this.logon + "'", this.baseDados.Conn);
                                           
                                            this.bdTransacao = this.baseDados.Conn.BeginTransaction();
                                            comandoAux.Transaction = bdTransacao;
                                            
                                            try
                                            {
                                                comandoAux.ExecuteNonQuery();
                                                this.bdTransacao.Commit();
                                            }
                                            catch
                                            {
                                                this.bdTransacao.Rollback();
                                                setErroAutorizacao(8);
                                            }
                                        }
                                        else
                                        {
                                            this.valorConsulta = respostaValor["VLR_UNITARIO"].ToString().Replace(",", ".");
                                            this.planoLogon = planoLogon;
                                        }
                                    }
                                    else
                                    {
                                        setErroAutorizacao(15);
                                        bdResposta.Dispose();
                                        bdResposta.Close();
                                        this.baseDados.Conn.Dispose();
                                        this.baseDados.Conn.Close();
                                        bdComando.Dispose();
                                    }
                                }
                                else
                                {
                                    setErroAutorizacao(15);
                                    bdResposta.Dispose();
                                    bdResposta.Close();
                                    this.baseDados.Conn.Dispose();
                                    this.baseDados.Conn.Close();
                                    bdComando.Dispose();
                                }
                            }
                            break;
                    }
                }
                else
                {
                    setErroAutorizacao(2);
                    bdResposta.Dispose();
                    bdResposta.Close();
                    this.baseDados.Conn.Dispose();
                    this.baseDados.Conn.Close();
                    bdComando.Dispose();
                }

                //CERTIFICA
                OracleCommand comandoAux2 = new OracleCommand(@"SELECT C.IDLOGON, C.CODTRANSACAO, C.SUBTRANSACAO, C.STATUS, C.QTD_CONS, C.VALOR_CONS, C.ID_MENSAGEM, C.IND_FONTE, "
                             + "L.LOGON, L.SENHA, L.MAX_SIMULTANEO, L.CONEX_ATUAIS, L.MEIO_ACESSO, L.PLANO, L.AG_SEMANAL, L.DESCONECTAR, L.CHAVE, L.NUM_CNPJ "
                             + "FROM TACCESS.CERTIFICA C INNER JOIN TACCESS.LOGON L ON L.ID = C.IDLOGON WHERE (L.LOGON = '" + this.logon + "' ) AND (L.SENHA =  '" + this.senha + "')", this.baseDados.Conn);
                
                OracleDataReader respostaSql = comandoAux2.ExecuteReader();
                
                if (respostaSql.Read())
                {

                    DataTable tabelaPermissoes = respostaSql.GetSchemaTable();
                    while (respostaSql.Read())
                    {
                        if (respostaSql["CODTRANSACAO"].ToString() == this.transacao && respostaSql["SUBTRANSACAO"].ToString() == this.codSubTransacao)
                        {
                            if (Convert.ToInt16(respostaSql["STATUS"]) == 1)
                            {
                                //CONTROLE POR TRANSACAO
                                if (planoLogon == "2")
                                {
                                    if (respostaSql["QTD_CONS"].ToString() == "")
                                    {
                                        setErroAutorizacao(15);
                                    }
                                    if (Convert.ToInt32(respostaSql["QTD_CONS"].ToString()) > 0)
                                    {
                                        DataBases.executeQuery("UPDATE TACCESS.CERTIFICA SET QTD_CONS = QTD_CONS-1 WHERE IDLOGON = " + respostaSql["IDLOGON"] + " AND CODTRANSACAO = '" + respostaSql["CODTRANSACAO"].ToString() + "' AND SUBTRANSACAO = '" + respostaSql["SUBTRANSACAO"].ToString() + "'");
                                    }
                                    else
                                    {
                                        setErroAutorizacao(15);
                                    }
                                }
                                //CONTROLE POR TRANSACAO (SEM BLOQUEIO)
                                else if (planoLogon == "6")
                                {
                                    DataBases.executeQuery("UPDATE TACCESS.CERTIFICA SET QTD_CONS = QTD_CONS-1 WHERE IDLOGON = " + respostaSql["IDLOGON"] + " AND CODTRANSACAO = '" + respostaSql["CODTRANSACAO"].ToString() + "' AND SUBTRANSACAO = '" + respostaSql["SUBTRANSACAO"].ToString() + "'");
                                }

                                this.idAgenda = respostaSql["AG_SEMANAL"].ToString();
                                this.erro = null;
                                break;
                            }
                            else if (Convert.ToInt16(respostaSql["STATUS"]) == 3)
                            {
                                setErroAutorizacao(5);
                                break;
                            }
                            else if (Convert.ToInt16(respostaSql["STATUS"]) == 4)
                            {
                                setErroAutorizacao(6);
                                break;
                            }
                        }
                        else if (this.erro == null)
                        {
                            setErroAutorizacao(3);
                        }
                    }
                }
                else
                {
                    setErroAutorizacao(2);
                }

                this.baseDados.Conn.Dispose();
                this.baseDados.Conn.Close();
            }
            catch (Exception ex)
            {
                LogEstatico.setLogTitulo("ERRO AUTORIZAÇÃO USUARIOS -> " + System.DateTime.Now);
                LogEstatico.setLogTexto(ex.Message + ex.StackTrace);

                string teste = ex.ToString().Substring(42, 9);

                if ((teste == "ORA-12170") || (teste == "ORA-01034") || (teste == "ORA-27101"))
                    setErroAutorizacao(8);
            }
        }

        // VERIFICA SE HÁ PERMISSÃO DE CONSULTA POR PERIODO/AGENDA
        public void verificaAgenda()
        {
            bdComando = new OracleCommand();
            baseDados = new DataBases();

            bdComando.Connection = baseDados.Conn;

            System.Data.ConnectionState estatusConexao = bdComando.Connection.State;

            DateTime inicio;
            DateTime final;
            DateTime atual = DateTime.Now;
            int diaAgenda = 0;
            
            try
            {
                if (((bdComando.Connection.State & System.Data.ConnectionState.Open) != System.Data.ConnectionState.Open))
                    bdComando.Connection.Open();

                // SE A AGENDA FOR PADRAO 24H NÃO FAZ VALIDAÇÃO
                if (this.idAgenda != "1")
                {
                    OracleDataReader respostaCmd;
                    bdComando.CommandText = "SELECT * FROM taccess.agenda WHERE  id = '" + this.idAgenda + "' AND ROWNUM < 2";
                    respostaCmd = bdComando.ExecuteReader();
                    diaAgenda = getDiaAtual();

                    if (respostaCmd.Read())
                    {

                        switch (diaAgenda)
                        {
                            case 0: // DOMINGO
                                if (Convert.ToInt16(respostaCmd["DOM_ACAO"]) == 1)
                                {
                                    inicio = Convert.ToDateTime(respostaCmd["DOM_INICIO"]);
                                    final = Convert.ToDateTime(respostaCmd["DOM_FIM"]);

                                    if (!(inicio.Hour == 1 && final.Hour == 1))
                                        setErroAutorizacao(4);
                                    else if (!(atual.Hour > inicio.Hour && atual.Hour < final.Hour))
                                        setErroAutorizacao(4);
                                }
                                break;
                            case 1: // SEGUNDA
                                if (Convert.ToInt16(respostaCmd["SEG_ACAO"]) == 1)
                                {
                                    inicio = Convert.ToDateTime(respostaCmd["SEG_INICIO"]);
                                    final = Convert.ToDateTime(respostaCmd["SEG_FIM"]);

                                    if (!(inicio.Hour == 1 && final.Hour == 1))
                                        setErroAutorizacao(4);
                                    else if (!(atual.Hour > inicio.Hour && atual.Hour < final.Hour))
                                        setErroAutorizacao(4);
                                }
                                break;
                            case 2: // TERÇA
                                if (Convert.ToInt16(respostaCmd["TER_ACAO"]) == 1)
                                {
                                    inicio = Convert.ToDateTime(respostaCmd["TER_INICIO"]);
                                    final = Convert.ToDateTime(respostaCmd["TER_FIM"]);

                                    if ((inicio.Hour != 1 || final.Hour != 1))
                                        setErroAutorizacao(4);
                                    else if (!(atual.Hour > inicio.Hour && atual.Hour < final.Hour))
                                        setErroAutorizacao(4);
                                }
                                break;
                            case 3: // QUARTA
                                if (Convert.ToInt16(respostaCmd["QUA_ACAO"]) == 1)
                                {
                                    inicio = Convert.ToDateTime(respostaCmd["QUA_INICIO"]);
                                    final = Convert.ToDateTime(respostaCmd["QUA_FIM"]);

                                    if (!(inicio.Hour == 1 && final.Hour == 1))
                                        setErroAutorizacao(4);
                                    else if (!(atual.Hour > inicio.Hour && atual.Hour < final.Hour))
                                        setErroAutorizacao(4);
                                }
                                break;
                            case 4: // QUINTA
                                if (Convert.ToInt16(respostaCmd["QUI_ACAO"]) == 1)
                                {
                                    inicio = Convert.ToDateTime(respostaCmd["QUI_INICIO"]);
                                    final = Convert.ToDateTime(respostaCmd["QUI_FIM"]);

                                    if (!(inicio.Hour == 1 && final.Hour == 1))
                                        setErroAutorizacao(4);
                                    else if (!(atual.Hour > inicio.Hour && atual.Hour < final.Hour))
                                        setErroAutorizacao(4);
                                }
                                break;
                            case 5: // SEXTA
                                if (Convert.ToInt16(respostaCmd["SEX_ACAO"]) == 1)
                                {
                                    inicio = Convert.ToDateTime(respostaCmd["SEX_INICIO"]);
                                    final = Convert.ToDateTime(respostaCmd["SEX_FIM"]);

                                    if (!(inicio.Hour == 1 && final.Hour == 1))
                                        setErroAutorizacao(4);
                                    else if (!(atual.Hour > inicio.Hour && atual.Hour < final.Hour))
                                        setErroAutorizacao(4);
                                }
                                break;
                            case 6: // SÁBADO
                                if (Convert.ToInt16(respostaCmd["SAB_ACAO"]) == 1)
                                {
                                    inicio = Convert.ToDateTime(respostaCmd["SAB_INICIO"]);
                                    final = Convert.ToDateTime(respostaCmd["SAB_FIM"]);

                                    if (!(inicio.Hour == 1 && final.Hour == 1))
                                        setErroAutorizacao(4);
                                    else if (!(atual.Hour > inicio.Hour && atual.Hour < final.Hour))
                                        setErroAutorizacao(4);
                                }
                                break;
                        }
                    }
                    respostaCmd.Dispose();
                    respostaCmd.Close();
                }
                baseDados.Conn.Dispose();
                baseDados.Conn.Close();
                bdComando.Dispose();
            }
            catch (Exception ex)
            {
                LogEstatico.setLogTitulo("ERRO RECEITA LOGON -> " + System.DateTime.Now);
                LogEstatico.setLogTexto(ex.Message + ex.StackTrace);
                LogEstatico.setLogTexto("PARAMETROS: " + logon);
                setErroAutorizacao(8);
            }
            finally
            {
                if ((estatusConexao == System.Data.ConnectionState.Open))
                    baseDados.Conn.Close();
            }
        }

        // VERIFICA IMPOSSIBILIDADES FINACEIRAS
        public void verificaTravaFinanceira()
        {
            StringBuilder querySQL = new StringBuilder();
            Boolean resposta = false;
            string valorConsulta = string.Empty;

            querySQL.Append("SELECT SUM(VALOR) AS VALOR FROM CHECKOK.TRAVA_PRODUTO WHERE TRIM(TRANSACAO)||TRIM(SUBTRANS) in ('" + this.transacao + this.codSubTransacao + "'");

            if (this.travaLeilao)
                querySQL.Append(",'CHAUTFT22'");

            if (this.travaPerdaTotal)
                querySQL.Append(",'CHAUTFT21'");

            if (this.travaPrecificador)
                querySQL.Append(",'CHAUTFT74'");

            querySQL.Append(")");
            
            try
            {
                bdComando = new Oracle.DataAccess.Client.OracleCommand();
                baseDados = new DataBases();

                bdComando.Connection = baseDados.Conn;

                System.Data.ConnectionState estatusConexao = bdComando.Connection.State;

                if (((bdComando.Connection.State & System.Data.ConnectionState.Open) != System.Data.ConnectionState.Open))
                    bdComando.Connection.Open();

                bdComando.CommandText = querySQL.ToString();

               

                OracleDataReader respostaSql = bdComando.ExecuteReader();

                if (respostaSql.Read())
                {
                    valorConsulta = respostaSql["VALOR"].ToString();

                    //EXECUTA PROCEDURE TRAVA FINANCEIRA E PEGA A CHAVE DE RESPOSTA
                    if (this.logon.Substring(0, 1) != "7")
                    {
                        OracleCommand comandoAux = new OracleCommand();

                        comandoAux.Connection = baseDados.Conn;
                        comandoAux.InitialLONGFetchSize = 1000;
                        comandoAux.CommandText = "PROC_TRAVA_FINANCEIRA_FEATURES";
                        comandoAux.CommandType = CommandType.StoredProcedure;

                        comandoAux.Parameters.Add("V_LOGON", this.logon);
                        comandoAux.Parameters.Add("V_VALORCONSULTA", Double.Parse(valorConsulta));
                        comandoAux.Parameters.Add("V_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        bdResposta = comandoAux.ExecuteReader();
                        bdResposta.Read();

                        resposta = Convert.ToBoolean(bdResposta["RESPOSTA"].ToString());
                        bdResposta.Close();
                        bdResposta.Dispose();
                    }
                    else
                    {
                        //SE O LOGON FOR DE INICIO 7 , ELE NAO VAI PASSAR PELA TRAVA,POR SE TRATAR DE UM LOGON OLHO NO CREDITO.'
                        resposta = true;
                    }
                }
                else
                {
                    setErroAutorizacao(5);
                    resposta = true;
                }

                bdComando.Dispose();
                baseDados.Conn.Dispose();
                baseDados.Conn.Close();
            }
            catch (Exception e)
            {
                //LOG DE ERRO
                resposta = true;
                LogEstatico.setLogTitulo("ERRO TRAVA FINANCEIRA -> " + System.DateTime.Now);
                LogEstatico.setLogTexto(e.Message + e.StackTrace);
                //LogEstatico.setLogTexto("PARAMETROS: " + cpf + "/" + logon + "/" + senha);
                LogEstatico.setLogTexto("PARAMETROS: " + logon + "/" + senha);
            }

            if (!resposta) setErroAutorizacao(8);
        }

        // PEGA A TRANSAÇÃO PELA SUBTRANSAÇÃO ATUAL
        private string getTransacao()
        {
            Oracle.DataAccess.Client.OracleCommand comandoSQL = new Oracle.DataAccess.Client.OracleCommand();
            this.baseDados = new DataBases();
            string retorno = string.Empty;

            comandoSQL.Connection = baseDados.Conn;

            System.Data.ConnectionState estatusConexao = comandoSQL.Connection.State;

            try
            {
                if (((comandoSQL.Connection.State & System.Data.ConnectionState.Open) != System.Data.ConnectionState.Open))
                    comandoSQL.Connection.Open();

                comandoSQL.CommandText = "SELECT * FROM relac_transacoes t WHERE t.subtrans_saf='" + this.codSubTransacao + "' AND ROWNUM < 2";
                if (comandoSQL.ExecuteReader().HasRows)
                {
                    OracleDataReader leitor = comandoSQL.ExecuteReader();
                    leitor.Read();
                    retorno = leitor.GetValue(0).ToString();
                }
                else
                    throw new Exception("Sub Transação inválida.");

                baseDados.Conn.Dispose();
                baseDados.Conn.Close();

                return retorno;
            }
            catch
            {
                throw new Exception("Sub Transação inválida.");
            }
            finally
            {
                if ((estatusConexao == System.Data.ConnectionState.Closed))
                    baseDados.Close();
            }
        }

        // PEGA O DIA DA AGENDA CONFORME O DIA DA SEMANA ATUAL
        private int getDiaAtual()
        {
            switch (DateTime.Now.DayOfWeek.ToString())
            {
                case "Sunday":
                    return 0;
                case "Monday":
                    return 1;
                case "Tuesday":
                    return 2;
                case "Wednesday":
                    return 3;
                case "Thursday":
                    return 4;
                case "Friday":
                    return 5;
                case "Saturday":
                    return 6;
                default:
                    return 0;
            }
        }

        // DETERMINA ERROS DE AUTORIZAÇÃO
        public void setErroAutorizacao(int codigo)
        {
            switch (codigo)
            {
                case 1:
                    this.erro = new Erros("001", "CPF Formatacao Invalida");
                    break;
                case 2:
                    this.erro = new Erros("002", "Acesso negado - Logon ou Senha Invalido");
                    break;
                case 3:
                    this.erro = new Erros("003", "Nao contem autorizacao para realizar esta consulta");
                    break;
                case 4:
                    this.erro = new Erros("004", "Horario e data nao sao permitidos para consulta");
                    break;
                case 5:
                    this.erro = new Erros("005", "Acesso  Negado Impedimento Administrativo");
                    break;
                case 6:
                    this.erro = new Erros("006", " Acesso  Negado Logon  Cancelado");
                    break;
                case 7:
                    this.erro = new Erros("007", "Usuario nao esta autenticado");
                    break;
                case 8:
                    this.erro = new Erros("008", "Problemas Internos! A Equipe de suporte está sendo acionada");
                    break;
                case 9:
                    this.erro = new Erros("009", "Problemas no Retorno XML");
                    break;
                case 10:
                    this.erro = new Erros("010", "Formato da Placa Inexistente");
                    break;
                case 11:
                    this.erro = new Erros("011", "Formato do Chassi Inexistente");
                    break;
                case 12:
                    this.erro = new Erros("012", "Formato do Motor Inexistente");
                    break;
                case 13:
                    this.erro = new Erros("013", "Formato do Cambio Inexistente");
                    break;
                case 14:
                    this.erro = new Erros("014", "Formato do Renavam Inexistente");
                    break;
                case 15:
                    this.erro = new Erros("015", "Falha ao acessar dados");
                    break;
                case 16:
                    this.erro = new Erros("016", "Trava financeira ativada");
                    break;
            }
        }
    
    }
}