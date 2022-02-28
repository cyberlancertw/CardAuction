using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CardAuction.Models
{
    public class Service
    {

        public static string ConnStr()
        {
            SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder();
            scsb.DataSource = ".";
            scsb.InitialCatalog = "dbCardAuction";
            scsb.IntegratedSecurity = true;
            return scsb.ToString();
        }
        public static string getCypher(string plainText)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            byte[] source = Encoding.Default.GetBytes(plainText);
            byte[] crypto = sha256.ComputeHash(source);
            return Convert.ToBase64String(crypto);
        }
        public static void ExecuteSql(string SqlStatement, List<SqlParameter> parameterList)
        {
            SqlConnection connection = new SqlConnection(Service.ConnStr());
            connection.Open();
            SqlCommand command = new SqlCommand(SqlStatement, connection);
            foreach(SqlParameter para in parameterList)
            {
                command.Parameters.Add(para);
            }
            command.ExecuteNonQuery();
            connection.Close();
        }

    }
}