using System;
using System.Collections.Generic;
using System.IO;
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
            if(Session[CDictionary.SK_UserAccount] == null)
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
        public ActionResult Post(tAuctionItem item,
                                 HttpPostedFileBase Photo0, HttpPostedFileBase Photo1, 
                                 HttpPostedFileBase Photo2, HttpPostedFileBase Photo3,
                                 bool isBuy, DateTime fEndTimeDate, DateTime fEndTimeTime,
                                 bool isPerson, bool isSeven, bool isFami, bool isLogi)
        {
            if (!isBuy)
            {
                item.fBuyPrice = -1;           // 以 -1 表示不提供此運送選項
            }
            if (!isPerson)
            {
                item.fTransPerson = -1;       // 以 -1 表示不提供此運送選項
            }
            if (!isSeven)
            {
                item.fTransSeven = -1;        // 以 -1 表示不提供此運送選項
            }
            if (!isFami)
            {
                item.fTransFami = -1;        // 以 -1 表示不提供此運送選項
            }
            if (!isLogi)
            {
                item.fTransLogi = -1;         // 以 -1 表示不提供此運送選項
            }
            DateTime nowTime = DateTime.Now;
            item.fEndTime = fEndTimeDate.Date.Add(fEndTimeTime.TimeOfDay);      // 由選擇的日期和時間合併成結標時間
            item.fCreateTime = nowTime;                                         // 現在時間為建立時間
            item.fMoneyNow = item.fMoneyStart;                                  // 目前價格即起標價格

            List<HttpPostedFileBase> photos = new List<HttpPostedFileBase>();
            if (Photo0 != null)
            {
                photos.Add(Photo0);
            }
            if (Photo1 != null)
            {
                photos.Add(Photo1);
            }
            if (Photo2 != null)
            {
                photos.Add(Photo2);
            }
            if (Photo3 != null)
            {
                photos.Add(Photo3);
            }
            int count = 0;
            string guid = Guid.NewGuid().GetHashCode().ToString();
            foreach(HttpPostedFileBase photo in photos){
                string newFileName = nowTime.ToString("yyyyMMddHHmmss") + guid + count;
                switch (count)
                {
                    case 0: item.fPhoto0 = newFileName; break;
                    case 1: item.fPhoto1 = newFileName; break;
                    case 2: item.fPhoto2 = newFileName; break;
                    case 3: item.fPhoto3 = newFileName; break;
                    default: break;
                }
                photo.SaveAs(Server.MapPath("~/Images/AuctionItemImages/")  + newFileName + Path.GetExtension(photo.FileName));
                count++;
            }
            item.fPosterUserId = (int)Session[CDictionary.SK_UserUserId];
            db.tAuctionItem.Add(item);
            db.SaveChanges();
            return RedirectToAction("Index","Home");
        }
    }
}