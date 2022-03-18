using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardAuction.Models;

namespace CardAuction.Controllers
{
    public class ExchangeController : Controller
    {
        dbCardAuctionEntities db = new dbCardAuctionEntities();

        // GET: Exchange
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Post()
        {
            if (Session[CDictionary.SK_UserAccount] == null)             // 沒登入不給上架，送去登入頁
            {
                Session[CDictionary.SK_RedirectToAction] = "Post";
                Session[CDictionary.SK_RedirectToController] = "Auction";
                return RedirectToAction("Login", "Member");
            }
            else
            {
                return View();
            }
        }
    }
}