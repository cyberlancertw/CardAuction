using System;
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
            if(string.IsNullOrEmpty(vModel.Account))
            {
                ViewData["errorMessage"] = "帳號不得為空";
                return View();
            }
            if (string.IsNullOrEmpty(vModel.Password))
            {
                ViewData["errorMessage"] = "密碼不得為空";
                return View();
            }
            string sql = "select * from tMember where fAccount=@acc and fPassword=@pwd";
            List<SqlParameter> paras = new List<SqlParameter>();
            paras.Add(new SqlParameter("acc", vModel.Account));
            paras.Add(new SqlParameter("pwd", Service.getCypher(vModel.Password)));
            List<CMember> queryResult = CMemberFactory.QueryBy(sql, paras);
            if(queryResult.Count > 0)
            {
                Session[CDictionary.SK_User] = queryResult[0];
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["errorMessage"] = "帳號或密碼不正確！";
                return View();
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(CRegisterViewModel vModel)
        {
            if (string.IsNullOrEmpty(vModel.Account))
            {
                ViewData["errAcc"] = "帳號不得為空";
                return View();
            }
            if (vModel.Account.Length < 7)
            {
                ViewData["errAcc"] = "帳號長度必須大於 6 個字元";
                return View();
            }
            
            // 檢查英數字 ^.[A-Za-z0-9]+$

            if (CMemberFactory.QueryByAccount(vModel.Account) != null)
            {
                ViewData["errAcc"] = "帳號已存在";
                return View();
            }
            if (vModel.Password.Length < 9)
            {
                ViewData["errPwd"] = "密碼長度必須大於 8 個字元";
                return View();
            }
            if (string.IsNullOrEmpty(vModel.Password))
            {
                ViewData["errPwd"] = "密碼不得為空";
                return View();
            }
            if (string.IsNullOrEmpty(vModel.PasswordCheck))
            {
                ViewData["errPwdCheck"] = "密碼不得為空";
                return View();
            }
            if (vModel.Password != vModel.PasswordCheck)
            {
                ViewData["errPwd"] = "密碼必須相同";
                ViewData["errPwdCheck"] = "密碼必須相同";
                return View();
            }
            if (string.IsNullOrEmpty(vModel.Name))
            {
                ViewData["errName"] = "姓名不得為空";
                return View();
            }
            if (string.IsNullOrEmpty(vModel.Email))
            {
                ViewData["errEmail"] = "Email 不得為空";
                return View();
            }
            if (string.IsNullOrEmpty(vModel.Address))
            {
                ViewData["errAddr"] = "地址不得為空";
                return View();
            }
            if (string.IsNullOrEmpty(vModel.Phone))
            {
                ViewData["errPhone"] = "電話不得為空";
                return View();
            }
            if (vModel.Birthday == null)
            {
                ViewData["errBirth"] = "生日不得為空";
                return View();
            }
            if(vModel.Birthday.Year < 1950)
            {
                ViewData["errBirth"] = "生日輸入有異";
                return View();
            }
            CMemberFactory.Create(new CMember()
            {
                Account = vModel.Account,
                //Account = vModel.Subscribe.ToString(),
                Password = vModel.Password,
                Name = vModel.Name,
                Email = vModel.Email,
                Address = vModel.AddressSelect + vModel.Address,
                Phone = vModel.Phone,
                Birthday = vModel.Birthday,
                Subscribe = vModel.Subscribe            // 不知為何永遠 false
            });

            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult PasswordForget()
        {
            // 輸入帳號

            return View();
        }

        [HttpPost]
        public ActionResult PasswordForget(string Account)
        {
            if (string.IsNullOrEmpty(Account))
            {
                return RedirectToAction("Index", "Home");
            }
            // 查 Email

            // 送 Email

            return View();
        }

        [HttpGet]
        public ActionResult PasswordForget(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // 輸入驗證碼

            return View();
        }
    }
}