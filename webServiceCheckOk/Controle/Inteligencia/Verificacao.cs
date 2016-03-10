using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using webServiceCheckOk.BaseDados;

namespace webServiceCheckOk.Controle.Inteligencia
{
    public class Verificacao
    {
        private static DataBases db;
        string retorno = string.Empty;
 
        #region VERIFICACAO
        // VERIFICA EM QUAL FORNECEDOR DEVE FAZER A CONSULTA
        public static bool verificaFornecedor(int idConsulta, int valorFornecedor)
        {
            Oracle.DataAccess.Client.OracleCommand comandoSQL = new Oracle.DataAccess.Client.OracleCommand();
            db = new DataBases();

            comandoSQL.Connection = db.Conn;

            System.Data.ConnectionState estatusConexao = comandoSQL.Connection.State;
            object returnValue;

            if (((comandoSQL.Connection.State & System.Data.ConnectionState.Open) != System.Data.ConnectionState.Open))
                comandoSQL.Connection.Open();

            comandoSQL.CommandText = "SELECT * FROM CHECKOK.WS_FORNECEDOR WHERE ID=" + idConsulta + " AND VALOR=" + valorFornecedor;

            try
            {
                returnValue = comandoSQL.ExecuteScalar();
            }
            finally
            {
                if ((estatusConexao == System.Data.ConnectionState.Closed))
                    db.Close();
            }

            if (((returnValue == null) || (returnValue.GetType() == typeof(System.DBNull))))
                return false;
            else
                return true;
        }

        // DOFARI - 28.07.2015 - VALIDAÇÃO DO INTEGRADOR
        public static bool verificaIntegrador(string logon)
        {
            OracleCommand comandoSQL = new OracleCommand();
            System.Data.ConnectionState estatusConexao = comandoSQL.Connection.State;
            HttpContext dadosCliente = HttpContext.Current;
            
            try
            {

                bool ip_integrador_valido = false;
                bool retorno = false;
                char tipoIp = 'E';
                string[] ips_interno = ConfigurationManager.AppSettings["LISTA_IP_INTERNO"].Split(';');
                string[] ips_integrador;
                string ipUsuario = dadosCliente.Request.ServerVariables["REMOTE_ADDR"];

                // CRIA O COMANDO A SER EXECUTADO NO BANCO DE DADOS: USA A CONEXÃO DA CLASSE DATABASES
                OracleDataReader leitorDados;
                db = new DataBases();

                comandoSQL.Connection = db.Conn;


                if (((comandoSQL.Connection.State & System.Data.ConnectionState.Open) != System.Data.ConnectionState.Open))
                    comandoSQL.Connection.Open();

                if (ConfigurationManager.AppSettings["HABILITA_IP_INTERNO"] == "S")
                {
                    //VERIFICA SE É IP INTERNO
                    foreach (string ip_interno in ips_interno)
                    {
                        if (ipUsuario.Contains(ip_interno))
                        {
                            tipoIp = 'I';
                            break;
                        }
                    }
                    //VALIDA SE O INTEGRADOR POSSUI ACESSO NO WebService
                    if (tipoIp == 'E')
                    {
                        comandoSQL.CommandText = "SELECT LOGON, IP FROM CHECKOK.INTEGRADOR WHERE LOGON = '" + logon + "'";
                        comandoSQL.CommandType = System.Data.CommandType.Text;
                        comandoSQL.Connection = db.Conn;

                        leitorDados = comandoSQL.ExecuteReader();

                        if (leitorDados.Read())
                        {
                            if (leitorDados["IP"].ToString() != string.Empty)
                            {
                                ips_integrador = leitorDados["IP"].ToString().Split(';');

                                foreach (string ip_integrador in ips_integrador)
                                {
                                    if (ipUsuario == ip_integrador)
                                    {
                                        ip_integrador_valido = true;
                                        break;
                                    }
                                }

                                retorno = (!ip_integrador_valido) ? false : retorno;
                            }
                        }
                        else
                        {
                            retorno = false;
                        }
                    }
                }
                return retorno;
            }
            catch
            {
                return false;
            }
            finally
            {
                if ((estatusConexao == System.Data.ConnectionState.Closed))
                    db.Close();
            }
        }

        // RETORNA QUAL O FORNECEDOR DA CONSULTA
        public static int getFornecedorConsulta(int idConsulta)
        {
            OracleCommand comandoSQL = new OracleCommand();
            OracleDataReader leitorDados;
            db = new DataBases();
            int retorno = 0;
            comandoSQL.Connection = db.Conn;

            System.Data.ConnectionState estatusConexao = comandoSQL.Connection.State;

            if (((comandoSQL.Connection.State & System.Data.ConnectionState.Open) != System.Data.ConnectionState.Open))
                comandoSQL.Connection.Open();

            comandoSQL.CommandText = "SELECT VALOR FROM CHECKOK.WS_FORNECEDOR WHERE ID=" + idConsulta ;

            try
            {
                leitorDados = comandoSQL.ExecuteReader();
                if (leitorDados.Read())
                {
                    retorno = int.Parse(leitorDados["VALOR"].ToString());
                }
                db.Conn.Dispose();
                db.Conn.Close();

                return retorno;
            }
            catch
            {
                return 0;
            }
            finally
            {
                if ((estatusConexao == System.Data.ConnectionState.Closed))
                    db.Close();
            }
        }
        
        // RETORNA QUAL A RECEITA DO LOGON ENVIADO
        public string getReceitaLogon(string logon)
        {
            Oracle.DataAccess.Client.OracleCommand comandoSQL = new Oracle.DataAccess.Client.OracleCommand();
            db = new DataBases();

            comandoSQL.Connection = db.Conn;

            System.Data.ConnectionState estatusConexao = comandoSQL.Connection.State;

            try
            {
                if (((comandoSQL.Connection.State & System.Data.ConnectionState.Open) != System.Data.ConnectionState.Open))
                    comandoSQL.Connection.Open();

                comandoSQL.CommandText = "SELECT B.RECEITA FROM CHECKOK.LOGONS A INNER JOIN CHECKOK.CLIENTE B ON A.COD_ENTIDADE = B.CODIGO WHERE SUBSTR(A.LOGON, 1, 6) = '" + logon + "'";
                
                retorno = comandoSQL.ExecuteReader().GetValue(0).ToString();
                
                db.Conn.Dispose();
                db.Conn.Close();

                return retorno;
            }
            catch (Exception ex)
            {
                LogEstatico.setLogTitulo("ERRO RECEITA LOGON -> " + System.DateTime.Now);
                LogEstatico.setLogTexto(ex.Message + ex.StackTrace);
                LogEstatico.setLogTexto("PARAMETROS: " + logon);

                return string.Empty;
            }
            finally
            {
                if ((estatusConexao == System.Data.ConnectionState.Open))
                    db.Close();
            }
        }
        
        
        #endregion

    }
}