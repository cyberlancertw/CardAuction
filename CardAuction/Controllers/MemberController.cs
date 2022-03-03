using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
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
            if(Session[CDictionary.SK_User] == null)
            {
                return RedirectToAction("Login");
            }

            return View();          // Member / Index 要有 View 嗎? 取代 mypage?
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
                Session[CDictionary.SK_User] = queryResult[0];      // 比對得到的帳號放入 Session ，要放入完整 CMember 物件
                                                                    // 還是只要使用者帳號名待之後查詢？

                if(Session[CDictionary.SK_RedirectToAction] != null)
                {
                    return RedirectToAction(CDictionary.SK_RedirectToAction.ToString(), CDictionary.SK_RedirectToController.ToString());
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
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
            if (vModel.Account.Length < 6)
            {
                ViewData["errAcc"] = "帳號長度必須大於 5 個字元";
                return View();
            }
            
            if(!Regex.IsMatch(vModel.Account, @"^[0-9a-zA-Z@.]+$"))
            {
                ViewData["errAcc"] = "帳號必須由英數字或 @ . 組成";
                return View();
            }

            if (CMemberFactory.QueryByAccount(vModel.Account) != 0)
            {
                ViewData["errAcc"] = "帳號已存在";
                return View();
            }
            if (vModel.Password.Length < 8)
            {
                ViewData["errPwd"] = "密碼長度必須大於 7 個字元";
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
                Password = vModel.Password,
                Name = vModel.Name,
                Email = vModel.Email,
                Address = vModel.AddressSelect + vModel.Address,
                Phone = vModel.Phone,
                Birthday = vModel.Birthday,
                Subscribe = vModel.Subscribe
            });

            Session[CDictionary.SK_RedirectToAction] = "Index";
            Session[CDictionary.SK_RedirectToController] = "Home";
            return RedirectToAction("Login");
        }

        public ActionResult AccountCount(string Account)            // 註冊時檢查Account是否已存在用
        {
            return Content(CMemberFactory.QueryByAccount(Account).ToString());
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