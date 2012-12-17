using System;
using System.Collections.Generic;
using System.Configuration;
using System.EnterpriseServices;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

[assembly: ApplicationName("dbIBANcomplus")]
[assembly: ApplicationActivation(ActivationOption.Library)]

namespace dbIBANcomplus
{
    [ Transaction(TransactionOption.RequiresNew) ]
    [ ObjectPooling(true, 5, 10) ]
    public class dbIBAN : ServicedComponent
    {
        [ AutoComplete(true) ]
        public string getIBAN(string AccNum)
        {
            string iban = "";
            string connStr = ConfigurationManager.AppSettings["connStr"];
            OleDbConnection cnn = new OleDbConnection(connStr);
            OleDbCommand cmd = new OleDbCommand("SELECT iban FROM Accounts WHERE AccNum = @accnum", cnn);
            OleDbDataReader reader = null;
            cmd.Parameters.AddWithValue("accnum", AccNum);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 300;
            //
            try
            {
                cnn.Open();
                reader = cmd.ExecuteReader();
                //
                if (reader.HasRows)
                {
                    reader.Read();
                    iban = reader["iban"].ToString();
                }
                cnn.Close();
                ContextUtil.SetComplete();
            }
            catch (Exception)
            {
                ContextUtil.SetAbort();
            }
            finally
            {
                if (cnn != null) cnn.Dispose();
                if (cmd != null) cmd.Dispose();
                if (reader != null) reader.Dispose();
            }
            return iban;
        }
    }
}
