using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardAuction.Models;

namespace CardAuction.Controllers
{
    public class AuctionController : Controller
    {

        dbCardAuctionEntities db = new dbCardAuctionEntities();

        // GET: Auction
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Post()
        {
            if(Session[CDictionary.SK_User] == null)
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

        [HttpPost]
        public ActionResult Post(tAuctionItem item)       // use view model
        {
            db.tAuctionItem.Add(item);
            item.fCreateTime = DateTime.Now;
            item.fMoneyNow = item.fMoneyStart;
            db.tAuctionItem.Add(item);
            db.SaveChanges();
            return View();
        }
    }
}