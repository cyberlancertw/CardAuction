using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardAuction.Models;

namespace CardAuction.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();              // 之後塞AuctionItem和ExchangeItem，也可以不用，搜尋用 Ajax 撈出來放不用 vModel
        }

        public ActionResult Error(string ErrorMessage, string ToController, string ToAction)
        {
            TempData["ErrorMessage"] = ErrorMessage;
            TempData["ToController"] = ToController;
            TempData["ToAction"] = ToAction;
            return View();
        }
        public ActionResult Search(string keyword)
        {
            // 搜尋 split
            // 比對 fItemName, fItemDescription, fSort
            // intersect , union
            //return Json();
            return View();
        }
    }
}