using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardAuction.Models;
using CardAuction.ViewModels;

namespace CardAuction.Controllers
{
    public class ExchangeController : Controller
    {
        dbCardAuctionEntities db = new dbCardAuctionEntities();

        // GET: Exchange
        //public ActionResult Index()
        //{
        //    return View();
        //}
       [HttpGet]
        public ActionResult Item(string id)
        {
            if (id == null)                             // 沒輸入 ItemId
            {
                return RedirectToAction("Error","Home", new { ErrorMessage = "需要商品編號" , ToController = "Auction" , ToAction = "List" });
            }
            var result = db.tAuctionItem.Find(id);
            if (result == null)                         // 有輸入 Id 但查不到
            {
                return RedirectToAction("Error", "Home", new { ErrorMessage = "商品不存在", ToController = "Auction" , ToAction = "List" });
            }
            string postUserId = result.fPostUserId;
            ViewBag.PostUserAccount = db.tMember.Find(postUserId).fAccount;

            if (Session[CDictionary.SK_UserUserId] == null || result.fPostUserId != (string)Session[CDictionary.SK_UserUserId])
            {
                result.fClick += 1;                     // 無登入 或 有登入但非本人，則商品點擊數 + 1
                try 
                {
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    return RedirectToAction("Error", "Home", new { ErrorMessage = $"糟糕！發生某些狀況…… {e.ToString()}", ToController = "Auction", ToAction = "List" });
                }
            }
            return View(result);
        }

        [HttpGet]
        public ActionResult Post()
        {
            if (Session[CDictionary.SK_UserAccount] == null)             // 沒登入不給上架，送去登入頁
            {
                TempData[CDictionary.SK_RedirectToAction] = "Post";
                TempData[CDictionary.SK_RedirectToController] = "Exchange";
                return RedirectToAction("Login", "Member");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Post(CExchangePostViewModel vModel)
        {
            if (Session[CDictionary.SK_UserAccount] == null)             // 沒登入不給上架，送去登入頁
            {
                TempData[CDictionary.SK_RedirectToAction] = "Post";
                TempData[CDictionary.SK_RedirectToController] = "Exchange";
                return RedirectToAction("Login", "Member");
            }

            tExchangeItem createItem = new tExchangeItem();
            DateTime nowTime = DateTime.Now;
            Random rnd = new Random();
            string fileNameInitial = nowTime.ToString("yyyyMMddHHmmss") + Guid.NewGuid().GetHashCode().ToString().Replace("-", "").Substring(0, 6) + rnd.Next(100, 1000).ToString();
            createItem.fItemId = fileNameInitial;
            List<HttpPostedFileBase> photos = new List<HttpPostedFileBase>();

            if (vModel.Photo0 != null)                 // 無視沒上傳圖片的位置，全部往前推
            {
                photos.Add(vModel.Photo0);
            }
            if (vModel.Photo1 != null)
            {
                photos.Add(vModel.Photo1);
            }
            if (vModel.Photo2 != null)
            {
                photos.Add(vModel.Photo2);
            }
            if (vModel.Photo3 != null)
            {
                photos.Add(vModel.Photo3);
            }
            if (photos.Count == 0)
            {
                ViewData["errorMessage"] = "請上傳圖片";
                return View();
            }
            int count = 0;
            foreach (HttpPostedFileBase photo in photos)
            {
                // 檔名組成：日期、時間、6數字組成字串、編號.副檔名
                string newFileName = fileNameInitial + count + Path.GetExtension(photo.FileName);

                switch (count)
                {
                    case 0: createItem.fPhoto0 = newFileName; break;
                    case 1: createItem.fPhoto1 = newFileName; break;
                    case 2: createItem.fPhoto2 = newFileName; break;
                    case 3: createItem.fPhoto3 = newFileName; break;
                    default: break;
                }
                // 存入 ~/Images/ExchangeItemImages 資料夾內
                photo.SaveAs(Server.MapPath("~/Images/ExchangeItemImages/") + newFileName);
                count++;
            }
            createItem.fItemName = vModel.fItemName;
            createItem.fItemDescription = vModel.fItemDescription;
            createItem.fItemLocation = vModel.fItemLocation;
            createItem.fItemLevel = vModel.fItemLeavel;
            createItem.fHopeItemName = vModel.fHopeItemName;
            createItem.fHopeItemLocation = vModel.fHopeItemLocation;
            createItem.fCreateTime = nowTime; // 現在時間為建立時間
            createItem.fEndTime = vModel.fEndTimeDate.Date.Add(vModel.fEndTimeTime.TimeOfDay);      // 由選擇的日期和時間合併成結標時間

            db.tExchangeItem.Add(createItem);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return RedirectToAction("Error", "Home", new { ErrorMessage = $"糟糕！發生某些狀況…… {e.ToString()}", ToController = "Auction", ToAction = "Post" });
            }
            return RedirectToAction("Index");

        }

        [HttpGet]
        public ActionResult List()
        {
            return View();
        }




    }
}