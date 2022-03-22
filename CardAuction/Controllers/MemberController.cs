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
        dbCardAuctionEntities db = new dbCardAuctionEntities();

        // GET: Member
        public ActionResult Index()
        {
            if(Session[CDictionary.SK_UserAccount] == null)
            {
                return RedirectToAction("Login");
            }

            return View();          // Member / Index 要有 View 嗎? 取代 mypage?
        }

        [HttpGet]
        public ActionResult Login()
        {
            if(Session[CDictionary.SK_UserAccount] == null)        // 無登入，進登入頁
            {
                return View();
            }
            else                                                   // 有登入，送進 Home/Index
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Login(CLoginViewModel vModel)
        {
            if(string.IsNullOrEmpty(vModel.Account))
            {
                ViewBag.errorMessage = "帳號不得為空";
                return View();
            }
            if (string.IsNullOrEmpty(vModel.Password))
            {
                ViewBag.errorMessage = "密碼不得為空";
                return View();
            }
            string cypher = Service.getCypher(vModel.Password);

            tMember queryResult = db.tMember
                .Where(m => m.fAccount == vModel.Account && m.fPassword == cypher)
                .FirstOrDefault();

            if(queryResult != null)
            {
                Session[CDictionary.SK_UserAccount] = queryResult.fAccount;
                Session[CDictionary.SK_UserUserId] = queryResult.fUserId;

                if (queryResult.fManager)
                {
                    return RedirectToAction("Index", "Admin");
                }

                if(TempData[CDictionary.SK_RedirectToAction] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    string toAction = TempData[CDictionary.SK_RedirectToAction].ToString();
                    string toController = TempData[CDictionary.SK_RedirectToController].ToString();
                    if (TempData[CDictionary.SK_RedirectToId] == null)
                    {
                        return RedirectToAction(toAction, toController);
                    }
                    else
                    {
                        string toId = TempData[CDictionary.SK_RedirectToId].ToString();
                        return RedirectToAction(toAction, toController, new { id = toId });
                    }
                }
            }
            else
            {
                ViewBag.errorMessage = "帳號或密碼不正確！";
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
                ViewBag.errAcc = "帳號不得為空";
                return View();
            }
            if (vModel.Account.Length < 6)
            {
                ViewBag.errAcc = "帳號長度必須大於 5 個字元";
                return View();
            }
            
            if(!Regex.IsMatch(vModel.Account, @"^[0-9a-zA-Z@.]+$"))
            {
                ViewBag.errAcc = "帳號必須由英數字或 @ . 組成";
                return View();
            }

            if (CMemberFactory.QueryByAccount(vModel.Account) != 0)
            {
                ViewBag.errAcc = "帳號已存在";
                return View();
            }
            if (vModel.Password.Length < 8)
            {
                ViewBag.errPwd = "密碼長度必須大於 7 個字元";
                return View();
            }
            if (string.IsNullOrEmpty(vModel.Password))
            {
                ViewBag.errPwd = "密碼不得為空";
                return View();
            }
            if (string.IsNullOrEmpty(vModel.PasswordCheck))
            {
                ViewBag.errPwdCheck = "密碼不得為空";
                return View();
            }
            if (vModel.Password != vModel.PasswordCheck)
            {
                ViewBag.errPwd = "密碼必須相同";
                ViewBag.errPwdCheck = "密碼必須相同";
                return View();
            }
            if (string.IsNullOrEmpty(vModel.Name))
            {
                ViewBag.errName = "姓名不得為空";
                return View();
            }
            if (string.IsNullOrEmpty(vModel.Email))
            {
                ViewBag.errEmail = "Email 不得為空";
                return View();
            }
            if (string.IsNullOrEmpty(vModel.Address))
            {
                ViewBag.errAddr = "地址不得為空";
                return View();
            }
            if (string.IsNullOrEmpty(vModel.Phone))
            {
                ViewBag.errPhone = "電話不得為空";
                return View();
            }
            if (vModel.Birthday == null)
            {
                ViewBag.errBirth = "生日不得為空";
                return View();
            }
            if(vModel.Birthday.Year < 1950)
            {
                ViewBag.errBirth = "生日輸入有異";
                return View();
            }

            Random rnd = new Random();
            int rndNum = rnd.Next(10);
            string newId = Guid.NewGuid().GetHashCode().ToString().Replace("-",rndNum.ToString()).Substring(0,8) + rnd.Next(1000,10000).ToString();
            tMember newMember = new tMember
            {
                fUserId = newId,
                fAccount = vModel.Account,
                fPassword = Service.getCypher(vModel.Password),
                fName = vModel.Name,
                fEmail = vModel.Email,
                fAddress = vModel.AddressSelect + vModel.Address,
                fPhone = vModel.Phone,
                fBirthday = vModel.Birthday,
                fSubscribe = vModel.Subscribe,
                fManager = false,
                fActive = false,
                fDelete = false
            };

            db.tMember.Add(newMember);
            db.SaveChanges();

            Session[CDictionary.SK_ValidationAccount] = vModel.Account;
            Session[CDictionary.SK_ValidationEmail] = vModel.Email;

            return RedirectToAction("Index", "Home");
            //return RedirectToAction("EmailValidation");

        }

        public ActionResult AccountCount(string Account)            // 註冊時檢查Account是否已存在用
        {
            bool isExist = db.tMember.Any(m => m.fAccount == Account);

            return Content(isExist.ToString());
        }

        [HttpGet]
        public ActionResult EmailValidation()
        {
            int validateNum = Service.GetRandomNumber();
            string validateAccount = Session[CDictionary.SK_ValidationAccount].ToString();
            string accountMask = validateAccount.Substring(0, 3) + new string('*', validateAccount.Length - 3);
            Session[CDictionary.SK_ValidationNumber] = validateNum;
            string email = Session[CDictionary.SK_ValidationEmail].ToString();
            string emailSubject = "Card.卡牌競標網站註冊驗証碼";
            string emailContent = $"<h2>Card.卡牌競標網站，歡迎您的註冊</h2><h3>註冊帳號為：{accountMask}</h3><h3>這是您的驗證碼：{validateNum} </h3><br /><br /><h3 style=\"color: red\">若非您本人註冊，請不要理會本信件。</h3>";

            Service.SendEmail(email, emailSubject, emailContent);
            return View();
        }
        [HttpPost]
        public ActionResult EmailValidation(string validationNumber)
        {
            int sessionNumber = (int)Session[CDictionary.SK_ValidationNumber];
            if(sessionNumber == Convert.ToInt32(validationNumber))
            {
                Session[CDictionary.SK_ValidationSuccess] = true;
                CMemberFactory.Activate(Session[CDictionary.SK_ValidationAccount].ToString());
                return RedirectToAction("Activate");
            }
            else
            {
                ViewData["errorMsg"] = "驗証碼輸入有誤";
                return View();
            }
            
        }

        public ActionResult Activate()
        {
            bool isSuccess = (bool)Session[CDictionary.SK_ValidationSuccess];
            if (isSuccess)
            {
                return View();
            }
            return RedirectToAction("Login");
            
        }

        [HttpGet]
        public ActionResult PasswordForget()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PasswordForget(string account, string email)
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(email))
            {
                ViewData["errorMsg"] = "不得有空值";
                return View();
            }
            string sqlStatement = "select * from tMember where fAccount=@acc and fEmail=@email";
            List<SqlParameter> paras = new List<SqlParameter>();
            paras.Add(new SqlParameter("acc", account));
            paras.Add(new SqlParameter("email", email));
            List<CMember> qryResult = CMemberFactory.QueryBy(sqlStatement, paras);
            if(qryResult.Count == 0)
            {
                ViewData["errorMsg"] = "查無符合資料";
                return View();
            }
            int newPassword = Service.GetRandomNumber();

            sqlStatement = "update tMember set fPassword=@pwd where fAccount=@acc";
            paras.Clear();
            paras.Add(new SqlParameter("pwd", Service.getCypher(newPassword.ToString())));
            paras.Add(new SqlParameter("acc", account));
            Service.ExecuteSql(sqlStatement, paras);

            string emailSubject = "Card.卡牌競標網站重發密碼信件";
            string accMask = account.Substring(0, 3) + new string('*', account.Length - 3);
            string emailContent = $"<h2>Card.卡牌競標網站會員 {accMask} 您的新密碼為：</h2><h3>{newPassword}</h3><h3>請盡速至網站更新您的密碼</h3><br /><br /><h3 style=\"color: red\">若非您本人註冊，請不要理會本信件。</h3>";

            Service.SendEmail(email, emailSubject, emailContent);

            return RedirectToAction("PasswordChanged");
        }

        public ActionResult PasswordChanged()
        {
            return View();
        }

        public ActionResult IsFavorite(string ItemId)
        {
            string UserId = string.Empty;
            if (Session[CDictionary.SK_UserUserId] == null)
            {
                return Content("False");
            }
            else
            {
                UserId = Session[CDictionary.SK_UserUserId].ToString();
            }
            tAuctionFavorite result = db.tAuctionFavorite.Where(m => m.fFromUserId == UserId && m.fToItemId == ItemId).FirstOrDefault();
            if(result != null)
            {
                return Content("True");
            }
            else
            {
                return Content("False");
            }
        }
        public ActionResult FavoriteAuction(string ItemId)
        {
            string UserId = Session[CDictionary.SK_UserUserId].ToString();
            tAuctionFavorite result = db.tAuctionFavorite.Where(m => m.fFromUserId == UserId && m.fToItemId == ItemId).FirstOrDefault();
            if (result != null)
            {
                db.tAuctionFavorite.Remove(result);
                db.SaveChanges();
                return Content("False");
            }
            else
            {
                db.tAuctionFavorite.Add(new tAuctionFavorite
                {
                    fFromUserId = UserId,
                    fToItemId = ItemId
                });
                db.SaveChanges();
                return Content("True");
            }
        }

    }
}