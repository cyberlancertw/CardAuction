﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardAuction.Models;
using CardAuction.ViewModels;

namespace CardAuction.Controllers
{
    public class MemberController : Controller
    {
        // GET: Member
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            if(Session[CDictionary.SK_User] == null)        // 無登入，進登入頁
            {
                return View();
            }
            else                                            // 有登入，送進 Home/Index
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Login(CLoginViewModel vModel)
        {
            //
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(CRegisterViewModel vModel)
        {
            /* 查非空
             * 檢查account是否存在
             * 比對兩個password
             * 查accuount是否alphabetnumber 
             * 查password是否含英數
             */
            if(CMemberFactory.QueryByAccount(vModel.Account) != null)
            {
                ViewBag.error = "帳號已存在";
                return View();
            }

            string sql = "insert into tMember values(@acc,@pwd,@name,@email,@addr,@phone,@birth,@subs,@manag,@active)";
            List<SqlParameter> paras = new List<SqlParameter>();
            paras.Add(new SqlParameter("acc", vModel.Account));
            paras.Add(new SqlParameter("pwd", Service.getCypher(vModel.Password)));         // 密碼做加密
            paras.Add(new SqlParameter("name", vModel.Name));
            paras.Add(new SqlParameter("email", vModel.Email));
            paras.Add(new SqlParameter("addr", vModel.AddressSelect + vModel.Address));    // 地址選擇的和輸入的兩個組合
            paras.Add(new SqlParameter("phone", vModel.Phone));
            paras.Add(new SqlParameter("birth", vModel.Birthday));
            paras.Add(new SqlParameter("subs", vModel.Subscribe));
            paras.Add(new SqlParameter("manag", false));
            paras.Add(new SqlParameter("active", true));        // 若加 Email 認證 feature，這裡就改 false，待validate
            Service.ExecuteSql(sql, paras);

            return RedirectToAction("Login");
        }
    }
}