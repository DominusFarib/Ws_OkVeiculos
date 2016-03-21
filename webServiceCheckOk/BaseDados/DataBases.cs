using System;
using System.Configuration;

using Oracle.DataAccess;
using Oracle.DataAccess.Client;

using webServiceCheckOk.Controle.Inteligencia;
using System.Data;

namespace webServiceCheckOk.BaseDados
{
    public class DataBases
    {
        private string ConnectionString { get; set; }

        public OracleConnection Conn { get; set; }

        public DataBases()
        {
            try
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["Oracle_ATNREP"].ConnectionString;
                Conn = new OracleConnection(this.ConnectionString);
                Conn.Open();
            }
            catch (Exception e)
            {
                Conn = null;
                throw e;
            }
        }

        public void Close()
        {
            if (Conn != null) Conn.Close();
        }

        // INSERE LOG DE CONSULTAS NO SISTEMA
        public static int InsertLog(decimal LAUNCHING, string LOGON, string TRANSACTION, string SUBTRANSACTION, string DOCTYPE, string DOCNO, System.DateTime OPERDATE, string SERVERID, decimal SESSIONNO, decimal SEQNO, decimal COMMANDRESULT, decimal HASRESTRICTION, decimal ISREQUEST, string BUFFER, bool FlagCobrar = true)
        {
            OracleConnection Conn;
            OracleTransaction bdTransacao;
            OracleCommand comandoSQL = new OracleCommand();
            
            Conn = new OracleConnection(ConfigurationManager.ConnectionStrings["Oracle_ATNREP"].ConnectionString);

            string sqlEstatico = string.Empty;
            int retorno;

            if (FlagCobrar)
            {
                try
                {
                    Conn.Open();
                    using (Conn)
                    {
                        sqlEstatico = "INSERT INTO TACCESS.T_LOGCONCENTRATORWB (LAUNCHING, LOGON, TRANSACTION, SUBTRANSACTION, DOCTYPE, DOCNO, OPERDATE, OPERTIME, SERVERID, SESSIONNO, SEQNO, COMMANDRESULT, HASRESTRICTION, ISREQUEST, BUFFER) VALUES (" + (decimal)LAUNCHING + ",'" + LOGON + "','" + TRANSACTION + "','" + SUBTRANSACTION + "','" + DOCTYPE + "','" + DOCNO + "',to_date('" + (System.DateTime)OPERDATE + "','dd/mm/yyyy hh24:mi:ss'),to_date('" + (System.DateTime)OPERDATE + "','dd/mm/yyyy hh24:mi:ss'),'" + SERVERID + "'," + (decimal)SESSIONNO + "," + (decimal)SEQNO + "," + (decimal)COMMANDRESULT + "," + (decimal)HASRESTRICTION + "," + (decimal)ISREQUEST + ",'" + BUFFER + "')";

                        comandoSQL.Connection = Conn;
                        
                        comandoSQL.CommandText = @"INSERT INTO TACCESS.T_LOGCONCENTRATORWB(
                                LAUNCHING,
                                LOGON,
                                TRANSACTION,
                                SUBTRANSACTION,
                                DOCTYPE,
                                DOCNO,
                                OPERDATE,
                                OPERTIME,
                                SERVERID,
                                SESSIONNO,
                                SEQNO,
                                COMMANDRESULT,
                                HASRESTRICTION,
                                ISREQUEST,
                                BUFFER)
                            VALUES (
                                :LAUNCHING, 
                                :LOGON, 
                                :TRANSACTION, 
                                :SUBTRANSACTION, 
                                :DOCTYPE, 
                                :DOCNO, 
                                :OPERDATE, 
                                :OPERTIME, 
                                :SERVERID, 
                                :SESSIONNO, 
                                :SEQNO, 
                                :COMMANDRESULT, 
                                :HASRESTRICTION, 
                                :ISREQUEST, 
                                :BUFFER)";
                        comandoSQL.CommandType = System.Data.CommandType.Text;

                        comandoSQL.Parameters.Add("LAUNCHING", OracleDbType.Varchar2, 6, "LAUNCHING", ParameterDirection.Input);
                        comandoSQL.Parameters.Add("LOGON", OracleDbType.Varchar2, 50, "LOGON", ParameterDirection.Input);
                        comandoSQL.Parameters.Add("TRANSACTION", OracleDbType.Varchar2, 6, "TRANSACTION", ParameterDirection.Input);
                        comandoSQL.Parameters.Add("SUBTRANSACTION", OracleDbType.Varchar2, 6, "SUBTRANSACTION", ParameterDirection.Input);
                        comandoSQL.Parameters.Add("DOCTYPE", OracleDbType.Varchar2, 2, "DOCTYPE", ParameterDirection.Input);
                        comandoSQL.Parameters.Add("DOCNO", OracleDbType.Varchar2, 16, "DOCNO", ParameterDirection.Input);
                        comandoSQL.Parameters.Add("OPERDATE", OracleDbType.Date, "OPERDATE", ParameterDirection.Input);
                        comandoSQL.Parameters.Add("OPERTIME", OracleDbType.Date, "OPERTIME", ParameterDirection.Input);
                        comandoSQL.Parameters.Add("SERVERID", OracleDbType.Varchar2, 500, "SERVERID", ParameterDirection.Input);
                        comandoSQL.Parameters.Add("SESSIONNO", OracleDbType.Decimal, 10, "SESSIONNO", ParameterDirection.Input);
                        comandoSQL.Parameters.Add("SEQNO", OracleDbType.Decimal, 10, "SEQNO", ParameterDirection.Input);
                        comandoSQL.Parameters.Add("COMMANDRESULT", OracleDbType.Decimal, 10, "COMMANDRESULT", ParameterDirection.Input);
                        comandoSQL.Parameters.Add("HASRESTRICTION", OracleDbType.Decimal, 2, "HASRESTRICTION", ParameterDirection.Input);
                        comandoSQL.Parameters.Add("ISREQUEST", OracleDbType.Decimal, 2, "ISREQUEST", ParameterDirection.Input);
                        comandoSQL.Parameters.Add("BUFFER", OracleDbType.Varchar2, 4000, "BUFFER", ParameterDirection.Input);

                        var nulo = System.DBNull.Value;

                        comandoSQL.Parameters[0].Value = (decimal)LAUNCHING;

                        if (String.IsNullOrEmpty(LOGON))
                            comandoSQL.Parameters[1].Value = nulo;
                        else
                            comandoSQL.Parameters[1].Value = (string)(LOGON);

                        if (String.IsNullOrEmpty(TRANSACTION))
                            comandoSQL.Parameters[2].Value = nulo;
                        else
                            comandoSQL.Parameters[2].Value = ((string)(TRANSACTION));

                        if (String.IsNullOrEmpty(SUBTRANSACTION))
                            comandoSQL.Parameters[3].Value = nulo;
                        else
                            comandoSQL.Parameters[3].Value = ((string)(SUBTRANSACTION));

                        if (String.IsNullOrEmpty(DOCTYPE))
                            comandoSQL.Parameters[4].Value = nulo;
                        else
                            comandoSQL.Parameters[4].Value = ((string)(DOCTYPE));

                        if (String.IsNullOrEmpty(DOCNO))
                            comandoSQL.Parameters[5].Value = nulo;
                        else
                            comandoSQL.Parameters[5].Value = ((string)(DOCNO));

                        comandoSQL.Parameters[6].Value = ((System.DateTime)(OPERDATE));
                        comandoSQL.Parameters[7].Value = ((System.DateTime)(OPERDATE));

                        if (String.IsNullOrEmpty(SERVERID))
                            throw new System.ArgumentNullException("SERVERID");
                        else
                            comandoSQL.Parameters[8].Value = ((string)(SERVERID));

                        comandoSQL.Parameters[9].Value = ((decimal)(SESSIONNO));
                        comandoSQL.Parameters[10].Value = ((decimal)(SEQNO));
                        comandoSQL.Parameters[11].Value = ((decimal)(COMMANDRESULT));
                        comandoSQL.Parameters[12].Value = ((decimal)(HASRESTRICTION));
                        comandoSQL.Parameters[13].Value = ((decimal)(ISREQUEST));

                        if ((BUFFER == null))
                            comandoSQL.Parameters[14].Value = nulo;
                        else
                            comandoSQL.Parameters[14].Value = ((string)(BUFFER));

                        if (((comandoSQL.Connection.State & System.Data.ConnectionState.Open) != System.Data.ConnectionState.Open))
                            comandoSQL.Connection.Open();

                        bdTransacao = Conn.BeginTransaction();
                        comandoSQL.Transaction = bdTransacao;

                        try
                        {
                            retorno = comandoSQL.ExecuteNonQuery();
                            bdTransacao.Commit();
                        }
                        catch (Exception e)
                        {
                            comandoSQL.Dispose();
                            bdTransacao.Rollback();
                            LogEstatico.setLog(e.StackTrace, "registroLogErro", true);
                            LogEstatico.setLog(sqlEstatico, "inserirOracle", false);
                            return 1;
                        }
                        return retorno;
                    }
                }
                catch (Exception e)
                {
                    LogEstatico.setLogTitulo("ERRO INSERT LOG -> " + System.DateTime.Now);
                    LogEstatico.setLogTexto(e.Message + e.StackTrace);
                    LogEstatico.setLogTexto("Comando que teve excecao -> " + sqlEstatico);
                    return 1;
                }
            }

            return 0;
        }
        
        // EXECUTA QUERY NA BASE DE DADOS [FUNÇÃO IDEAL PARA EXECUÇÃO DE UPADATE E DELETE]
        public static void executeQuery(string querySQL)
        {
            OracleConnection Conn;
            OracleTransaction bdTransacao;
            Oracle.DataAccess.Client.OracleCommand comandoSQL = new Oracle.DataAccess.Client.OracleCommand();
            System.Data.ConnectionState estatusConexao = comandoSQL.Connection.State;
            Conn = new OracleConnection(ConfigurationManager.ConnectionStrings["Oracle_ATNREP"].ConnectionString);

            try
            {
                using (Conn)
                {
                    comandoSQL.Connection = Conn;
                    comandoSQL.CommandText = querySQL;

                    if (((comandoSQL.Connection.State & System.Data.ConnectionState.Open) != System.Data.ConnectionState.Open))
                        comandoSQL.Connection.Open();

                    bdTransacao = Conn.BeginTransaction();
                    comandoSQL.Transaction = bdTransacao;

                    try
                    {
                        comandoSQL.ExecuteNonQuery();
                        bdTransacao.Commit();
                    }
                    catch (Exception e)
                    {
                        comandoSQL.Dispose();
                        bdTransacao.Rollback();
                        throw new Exception("Falha ao acessar Base de dados: " + e.Message);
                    }
                    finally
                    {
                        if ((estatusConexao == System.Data.ConnectionState.Closed))
                        {
                            Conn.Dispose();
                            Conn.Close();
                            comandoSQL.Dispose();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Falha ao acessar Base de dados" + e.Message);
            }
        }

        public static string getLaunching(){

            OracleCommand comandoSQL = new OracleCommand();
            OracleConnection Conn;
            OracleDataReader leitorDados;
            string retorno = string.Empty;

            try
            {
                Conn = new OracleConnection(ConfigurationManager.ConnectionStrings["Oracle_ATNREP"].ConnectionString);
                Conn.Open();

                comandoSQL.Connection = Conn;
                comandoSQL.CommandText = "SELECT TO_CHAR(SYSDATE, 'SSSSS') AS LAUNCHING FROM  SYS.DUAL";

                try
                {
                    leitorDados = comandoSQL.ExecuteReader();
                    if (leitorDados.Read())
                    {
                        retorno = leitorDados["LAUNCHING"].ToString();
                    }

                    Conn.Dispose();
                    Conn.Close();
                    return retorno;
                }
                catch
                {
                    return string.Empty;
                }
                finally
                {
                    if ((Conn.State == System.Data.ConnectionState.Closed))
                        Conn.Close();
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}