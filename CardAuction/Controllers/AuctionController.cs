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
    public class AuctionController : Controller
    {

        dbCardAuctionEntities db = new dbCardAuctionEntities();

        // GET: Auction
        //public ActionResult Index()
        //{
        //    return View();
        //}
        public ActionResult test()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Item(string id)
        {
            if (id == null)
            {
                return RedirectToAction("List");
            }
            var result = db.tAuctionItem.Find(id);
            
            if (result == null)
            {
                return RedirectToAction("List");
            }

            if (Session[CDictionary.SK_UserUserId] == null || result.fPostUserId != (string)Session[CDictionary.SK_UserUserId])
            {
                result.fClick += 1;
                db.SaveChanges();
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

        public ActionResult Post(CAuctionPostViewModel vModel)
        {
            tAuctionItem createItem = new tAuctionItem();
            DateTime nowTime = DateTime.Now;
            Random rnd = new Random();
            string fileNameInitial = nowTime.ToString("yyyyMMddHHmmss") + Guid.NewGuid().GetHashCode().ToString().Replace("-", "").Substring(0, 6) + rnd.Next(100,1000).ToString();
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
                string newFileName = fileNameInitial + count + Path.GetExtension(photo.FileName);       // 檔名組成：日期、時間、6數字組成字串、編號.副檔名
                switch (count)
                {
                    case 0: createItem.fPhoto0 = newFileName; break;
                    case 1: createItem.fPhoto1 = newFileName; break;
                    case 2: createItem.fPhoto2 = newFileName; break;
                    case 3: createItem.fPhoto3 = newFileName; break;
                    default: break;
                }
                photo.SaveAs(Server.MapPath("~/Images/AuctionItemImages/") + newFileName);              // 存入 ~/Images/AuctionItemImages 資料夾內
                count++;
            }
            createItem.fPostUserId = vModel.fPostUserId;
            createItem.fItemName = vModel.fItemName;
            createItem.fItemDescription = vModel.fItemDescription;
            createItem.fSort = vModel.fSort;
            createItem.fGrading = vModel.fGrading;

            if (!vModel.isBuy || vModel.fBuyPrice < 0)
            {
                createItem.fBuyPrice = -1;           // 以 -1 表示不提供直購價
            }
            else
            {
                createItem.fBuyPrice = vModel.fBuyPrice;
            }
            if (!vModel.isPerson || vModel.fTransPerson < 0)
            {
                createItem.fTransPerson = -1;       // 以 -1 表示不提供此運送選項
            }
            else
            {
                createItem.fTransPerson = vModel.fTransPerson;
            }
            if (!vModel.isSeven || vModel.fTransSeven < 0)
            {
                createItem.fTransSeven = -1;
            }
            else
            {
                createItem.fTransSeven = vModel.fTransSeven;
            }
            if (!vModel.isFami || vModel.fTransFami < 0)
            {
                createItem.fTransFami = -1;
            }
            else
            {
                createItem.fTransFami = vModel.fTransFami;
            }
            if (!vModel.isLogi || vModel.fTransLogi < 0)
            {
                createItem.fTransLogi = -1;
            }
            else
            {
                createItem.fTransLogi = vModel.fTransLogi;
            }
            
            createItem.fEndTime = vModel.fEndTimeDate.Date.Add(vModel.fEndTimeTime.TimeOfDay);      // 由選擇的日期和時間合併成結標時間
            createItem.fCreateTime = nowTime;                                                       // 現在時間為建立時間
            createItem.fMoneyStart = vModel.fMoneyStart;
            createItem.fMoneyNow = vModel.fMoneyStart;                                              // 目前價格即起標價格
            createItem.fClick = 0;
            createItem.fDelete = false;
            createItem.fReport = 0;
            

            db.tAuctionItem.Add(createItem);
            db.SaveChanges();
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult List()
        {
            var result = db.tAuctionItem.Where(m => m.fEndTime > DateTime.Now)
                                        .OrderBy(m => m.fEndTime)
                                        .ToList();
            return View(result);
        }
    }

}