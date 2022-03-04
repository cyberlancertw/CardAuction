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
            return View();              // 之後塞AuctionItem和ExchangeItem
        }

        public ActionResult SendTest()
        {
            Service.SendEmail("cyberlancer@gmail.com", "testQQ", "test agin sorry...");
            return Content("~~");
        }
    }
}