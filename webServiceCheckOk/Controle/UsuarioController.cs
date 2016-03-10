using System;
using Oracle.DataAccess.Client;
using webServiceCheckOk.BaseDados;
using webServiceCheckOk.Model;

namespace webServiceCheckOk.Controle
{
    public class UsuarioController: UsuarioModel
    {
        private DataBases baseDados;

        // PEGA O CPF/CNPJ DO USUARIO LOGADO/CONSULTANTE
        public string getDocUsuarioByLogon(string logon = "")
        {
            OracleCommand comandoSQL = new  OracleCommand();
            System.Data.ConnectionState estatusConexao;
            string retorno = string.Empty;
            
            this.baseDados = new DataBases();
            comandoSQL.Connection = baseDados.Conn;
            estatusConexao = comandoSQL.Connection.State;

            try
            {
                if (((comandoSQL.Connection.State & System.Data.ConnectionState.Open) != System.Data.ConnectionState.Open))
                    comandoSQL.Connection.Open();

                comandoSQL.CommandText = @"SELECT C.NUM_CNPJ
                                            FROM CHECKOK.LOGONS A, CHECKOK.CLIENTE B, CHECKOK.REDESPC_SERCTR_CLI C 
                                            WHERE A.COD_ENTIDADE = B.CODIGO 
	                                        AND B.CGCMF = SUBSTR(C.NUM_CNPJ,2,14) 
	                                        AND SUBSTR(A.LOGON,1,6) = '" + logon + "'";

                if (comandoSQL.ExecuteReader().HasRows)
                {
                    OracleDataReader leitor = comandoSQL.ExecuteReader();
                    leitor.Read();
                    retorno = leitor.GetValue(0).ToString();
                }

                baseDados.Conn.Dispose();
                baseDados.Conn.Close();

                return retorno;
            }
            catch
            {
                throw new Exception("BD: Erro ao acessar Dados");
            }
            finally
            {
                if ((estatusConexao == System.Data.ConnectionState.Closed))
                    baseDados.Close();
            }
        }
    }
}