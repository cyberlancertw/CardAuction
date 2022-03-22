﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CardAuction.Models
{
    public class CMemberFactory
    {
        public static void Create(CMember member)
        {
            string sql = "insert into tMember values(@acc,@pwd,@name,@email,@addr,@phone,@birth,@subs,@manag,@active)";
            List<SqlParameter> paras = new List<SqlParameter>();
            paras.Add(new SqlParameter("acc", member.Account));
            paras.Add(new SqlParameter("pwd", Service.getCypher(member.Password)));         // 密碼做加密
            paras.Add(new SqlParameter("name", member.Name));
            paras.Add(new SqlParameter("email", member.Email));
            paras.Add(new SqlParameter("addr", member.Address));
            paras.Add(new SqlParameter("phone", member.Phone));
            paras.Add(new SqlParameter("birth", member.Birthday));
            paras.Add(new SqlParameter("subs", member.Subscribe));
            paras.Add(new SqlParameter("manag", false));        // 預設不是管理員，待後台修改
            paras.Add(new SqlParameter("active", false));       // 預設 false，經過 email 認證才改成 true

            Service.ExecuteSql(sql, paras);
        }

        public static void Activate(string account)
        {
            string sqlStatement = "update tMember set fActive=@act where fAccount=@acc";
            List<SqlParameter> paras = new List<SqlParameter>();
            paras.Add(new SqlParameter("act", true));
            paras.Add(new SqlParameter("acc", account));
            Service.ExecuteSql(sqlStatement, paras);
        }

        public static List<CMember> QueryBy(string sqlStatement, List<SqlParameter> parameterList)
        {
            List<CMember> queryResult = new List<CMember>();
            SqlConnection connection = new SqlConnection(Service.ConnStr());
            connection.Open();
            SqlCommand command = new SqlCommand(sqlStatement, connection);
            foreach(SqlParameter para in parameterList)
            {
                command.Parameters.Add(para);
            }
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                CMember member = new CMember();
                member.UserId = (int)reader["fUserId"];
                member.Account = (string)reader["fAccount"];
                member.Password = (string)reader["fPassword"];
                member.Name = (string)reader["fName"];
                member.Email = (string)reader["fEmail"];
                member.Address = (string)reader["fAddress"];
                member.Phone = (string)reader["fPhone"];
                member.Birthday = (DateTime)reader["fBirthday"];
                member.Subscribe = (bool)reader["fSubscribe"];
                member.Manager = (bool)reader["fManager"];
                member.Active = (bool)reader["fActive"];
                
                queryResult.Add(member);
            }
            reader.Close();
            connection.Close();

            return queryResult;
        }
        public static int QueryByAccount(string account)
        {
            string sql = "select * from tMember where fAccount=@acc";
            List<SqlParameter> paras = new List<SqlParameter>();
            paras.Add(new SqlParameter("acc", account));
            List<CMember> queryResult = QueryBy(sql, paras);

            return queryResult.Count;

        }
        public static CMember QueryById(int userId)
        {
            string sql = "select * from tMember where fUserId=@id";
            List<SqlParameter> paras = new List<SqlParameter>();
            paras.Add(new SqlParameter("id", userId));
            List<CMember> queryResult = QueryBy(sql, paras);
            if (queryResult.Count > 0)
            {
                return queryResult[0];
            }
            else
            {
                return null;
            }
        }

        // -------

        
        public void AddFavorite(int? id)
        {

        }

        public void RemoveFavorite(int? id)
        {

        }

        public void Update(CMember member)
        {
            string sql = "update tMember set fAccount=@acc, fPassword=@pwd, fName=@name, fEmail=@email, fAddress=@addr, fPhone=@phone, fBirthday=@birth, fSubscribe=@subs, fManager=@manag, fActive=@active where fUserId=@id)";
            List<SqlParameter> paras = new List<SqlParameter>();
            paras.Add(new SqlParameter("acc", member.Account));
            paras.Add(new SqlParameter("pwd", Service.getCypher(member.Password)));         // 密碼做加密
            paras.Add(new SqlParameter("name", member.Name));
            paras.Add(new SqlParameter("email", member.Email));
            paras.Add(new SqlParameter("addr", member.Address));
            paras.Add(new SqlParameter("phone", member.Phone));
            paras.Add(new SqlParameter("birth", member.Birthday));
            paras.Add(new SqlParameter("subs", member.Subscribe));
            paras.Add(new SqlParameter("manag", false));
            paras.Add(new SqlParameter("active", true));
            paras.Add(new SqlParameter("id", member.UserId));

            Service.ExecuteSql(sql, paras);
        }
    }
}