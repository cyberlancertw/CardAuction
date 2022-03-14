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
        //public ActionResult Index()
        //{
        //    return View();
        //}

        [HttpGet]
        public ActionResult Item(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("List");
            }
            var result = db.tAuctionItem.Find(id);
            if(result == null)
            {
                return RedirectToAction("List");
            }
            return View(result);
        }
        [HttpGet]
        public ActionResult Post()
        {
            if(Session[CDictionary.SK_UserAccount] == null)             // 沒登入不給上架，送去登入頁
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
                item.fBuyPrice = -1;           // 以 -1 表示不提供直購價
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
            if (Photo0 != null)                 // 無視沒上傳圖片的位置，全部往前推
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
            if(photos.Count == 0)
            {
                ViewData["errorMessage"] = "請上傳圖片";
                return View();
            }
            int count = 0;
            string fileNameInitial = nowTime.ToString("yyyyMMddHHmmss") + Guid.NewGuid().GetHashCode().ToString().Replace("-","").Substring(0,6);
            foreach(HttpPostedFileBase photo in photos){
                string newFileName = fileNameInitial + count + Path.GetExtension(photo.FileName);       // 檔名組成：日期、時間、6數字組成字串、編號.副檔名
                switch (count)
                {
                    case 0: item.fPhoto0 = newFileName; break;
                    case 1: item.fPhoto1 = newFileName; break;
                    case 2: item.fPhoto2 = newFileName; break;
                    case 3: item.fPhoto3 = newFileName; break;
                    default: break;
                }
                photo.SaveAs(Server.MapPath("~/Images/AuctionItemImages/") + newFileName);              // 存入 ~/Images/AuctionItemImages 資料夾內
                count++;
            }
            item.fPosterUserId = (int)Session[CDictionary.SK_UserUserId];                               // 張貼者 id 即 Session 登入者 id
            db.tAuctionItem.Add(item);
            db.SaveChanges();
            return RedirectToAction("Index","Home");
        }
    }

}