﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
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

        public static void SendEmail(string ToEmail, string Subject, string Message)
        {
            SmtpSection secObj = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            using(MailMessage mailMsg = new MailMessage())
            {
                mailMsg.From = new MailAddress(secObj.From);
                mailMsg.To.Add(ToEmail);
                mailMsg.Subject = Subject;
                mailMsg.Body = Message;
                mailMsg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(secObj.Network.Host, secObj.Network.Port);
                //smtp.Host = secObj.Network.Host;
                //smtp.Port = secObj.Network.Port;
                smtp.EnableSsl = true;  //secObj.Network.EnableSsl;
                NetworkCredential networkCred = new NetworkCredential(secObj.Network.UserName, secObj.Network.Password);
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = networkCred;
                try
                {
                    smtp.Send(mailMsg);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public static int GetRandomNumber()
        {
            Random rnd = new Random();
            int result = rnd.Next(9) + 1;
            for (int i = 0; i < 5; i++)
            {
                result = result * 10 + rnd.Next(10);
            }

            return result;
        }

    }
}