using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardAuction.Models;
using CardAuction.ViewModels;
using System.Data.Entity.Validation;

namespace CardAuction.Controllers
{
    public class ExchangeController : Controller
    {
        dbCardAuctionEntities db = new dbCardAuctionEntities();

        // GET: Exchange
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }


        [HttpGet]
        public ActionResult Item(string id)
        {
            if (id == null)                             // 沒輸入 ItemId
            {
                return RedirectToAction("Error", "Home", new { ErrorMessage = "需要商品編號", ToController = "Exchange", ToAction = "List" });
            }
            var result = db.tExchangeItem.Find(id);
            if (result == null)                         // 有輸入 Id 但查不到
            {
                return RedirectToAction("Error", "Home", new { ErrorMessage = "商品不存在", ToController = "Exchange", ToAction = "List" });
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
                catch (Exception e)
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
                Session[CDictionary.SK_RedirectToController] = "Exchange";
                Session[CDictionary.SK_RedirectToAction] = "Post";
                Session[CDictionary.SK_RedirectToId] = string.Empty;
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
                Session[CDictionary.SK_RedirectToController] = "Exchange";
                Session[CDictionary.SK_RedirectToAction] = "Post";
                Session[CDictionary.SK_RedirectToId] = string.Empty;
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
            createItem.fSort = vModel.fSort;
            createItem.fPostUserId = vModel.fPostUserId;
            createItem.fItemDescription = vModel.fItemDescription;
            createItem.fItemLocation = vModel.fItemLocation;
            createItem.fItemLevel = vModel.fItemLevel;
            createItem.fHopeItemName = vModel.fHopeItemName;
            createItem.fHopeItemLocation = vModel.fHopeItemLocation;
            createItem.fUserInfo = vModel.fUserInfo;
            createItem.fCreateTime = nowTime; // 現在時間為建立時間
            createItem.fEndTime = vModel.fEndTimeDate.Date.Add(vModel.fEndTimeTime.TimeOfDay);      // 由選擇的日期和時間合併成結束時間
            createItem.fClick = 0;
            createItem.fDelete = false;
            createItem.fReport = 0;

            db.tExchangeItem.Add(createItem);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return RedirectToAction("Error", "Home", new { ErrorMessage = $"糟糕！發生某些狀況…… {e.ToString()}", ToController = "Exchange", ToAction = "List" });
            }
            return RedirectToAction("List");

        }
        [HttpGet]
        public ActionResult Couple(string id)
        {
            ViewBag.itemId = id;
            if (Session[CDictionary.SK_UserAccount] == null)             // 沒登入不給上架，送去登入頁
            {
                Session[CDictionary.SK_RedirectToController] = "Exchange";
                Session[CDictionary.SK_RedirectToAction] = "Couple";
                Session[CDictionary.SK_RedirectToId] = id;
                return RedirectToAction("Login", "Member");
            }
            else
            {
                //ViewBag.ItemTableIdB = id;
                return View();
            }
        }
        [HttpPost]
        public ActionResult Couple(CExchangePostViewModel vModel,tExchangeItem item)
        {
            
            if (Session[CDictionary.SK_UserAccount] == null)             // 沒登入不給上架，送去登入頁
            {
                Session[CDictionary.SK_RedirectToController] = "Exchange";
                Session[CDictionary.SK_RedirectToAction] = "Couple";
                Session[CDictionary.SK_RedirectToId] = string.Empty;
                return RedirectToAction("Login", "Member");
            }

            tExchangeItemTable createItem = new tExchangeItemTable();
            DateTime nowTime = DateTime.Now;
            Random rnd = new Random();
            string fileNameInitial = nowTime.ToString("yyyyMMddHHmmss") + Guid.NewGuid().GetHashCode().ToString().Replace("-", "").Substring(0, 6) + rnd.Next(100, 1000).ToString();
            createItem.fItemTableId = fileNameInitial;
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
            createItem.fItemId = vModel.itemId;
            createItem.fItemName = vModel.fItemName;
            createItem.fSort = vModel.fSort;
            createItem.fPostUserId = vModel.fPostUserId;
            createItem.fItemDescription = vModel.fItemDescription;
            createItem.fItemLocation = vModel.fItemLocation;
            createItem.fItemLevel = vModel.fItemLevel;
            createItem.fUserInfo = vModel.fUserInfo;
            createItem.fDelete = false;
            createItem.fReport = 0;

            db.tExchangeItemTable.Add(createItem);
            try
            {
                db.SaveChanges();
            }
            catch(DbEntityValidationException ex)
            {
                return RedirectToAction("Error", "Home", new { ErrorMessage = $"糟糕！發生某些狀況…… {ex.ToString()}", ToController = "Exchange", ToAction = "List" });
            }
            catch (Exception e)
            {
                return RedirectToAction("Error", "Home", new { ErrorMessage = $"糟糕！發生某些狀況…… {e.ToString()}", ToController = "Exchange", ToAction = "List" });
            }
            return RedirectToAction("Item", new { id =  vModel.itemId});
        }
        [HttpGet]
        public ActionResult List()
        {
            return View();
            
        }

        public ActionResult QueryBySort(string sortName, string filter = "EndTime", int page = 0)
        {
            switch (filter) 
            {
                case "EndTime":
                    {
                        var queryResult = db.tExchangeItem
                            .Where(m => m.fEndTime > DateTime.Now && m.fSort.Contains(sortName))
                            .OrderBy(p => p.fEndTime)
                            .Skip(page * 12)
                            .Take(12)
                            .Select(n => new QueryResult
                            {
                                fItemId = n.fItemId,
                                fEndTime = n.fEndTime,
                                fItemName = n.fItemName,
                                fPhoto = n.fPhoto0,
                                fChangeCount = n.fChangeCount,
                            });
                        return Json(queryResult, JsonRequestBehavior.AllowGet);
                    }

                case "HotClick":
                    {
                        var queryResult = db.tExchangeItem
                            .Where(m => m.fEndTime > DateTime.Now && m.fSort.Contains(sortName))
                            .OrderBy(p => p.fClick).ThenBy(q => q.fEndTime)
                            .Skip(page * 12)
                            .Take(12)
                            .Select(n => new QueryResult 
                            {
                                fItemId = n.fItemId,
                                fEndTime = n.fEndTime,
                                fItemName = n.fItemName,
                                fPhoto = n.fPhoto0,
                                fChangeCount = n.fChangeCount,
                            });
                        return Json(queryResult, JsonRequestBehavior.AllowGet);
                    }

                case "JustPost":
                    {
                        var queryResult = db.tExchangeItem
                            .Where(m => m.fEndTime > DateTime.Now && m.fSort.Contains(sortName))
                            .OrderByDescending(p => p.fCreateTime)
                            .Skip(page * 12)
                            .Take(12)
                            .Select(n => new QueryResult 
                            {
                                fItemId = n.fItemId,
                                fEndTime = n.fEndTime,
                                fItemName = n.fItemName,
                                fPhoto = n.fPhoto0,
                                fChangeCount = n.fChangeCount,
                            });
                        return Json(queryResult, JsonRequestBehavior.AllowGet);
                    }
                default:
                    break;
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReceiveComments(string itemId) //評論區
        {
            bool isExist = db.tCommentExchange.Any(m => m.fItemId == itemId);
            if (isExist)
            {
                var queryResult = db.tCommentExchange
                    .Where(p => p.fItemId == itemId)
                    .Join(db.tMember,
                          c => c.fFromUserId,
                          m => m.fUserId,
                          (c, m) => new
                          {
                              postAcc = m.fAccount,
                              content = c.fContent,
                              postTime = c.fPostTime
                          })
                    .OrderBy(n => n.postTime); ;
                return Json(queryResult, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public void WriteComment(string itemId, string message)//寫評論
        {
            if (Session[CDictionary.SK_UserUserId] == null)
            {
                return;
            }
            string userId = Session[CDictionary.SK_UserUserId].ToString();
            bool isExist = db.tCommentExchange.Any(m => m.fItemId == itemId && m.fContent == message && m.fFromUserId == userId);
            if (isExist)
            {
                return;
            }
            tCommentExchange newComment = new tCommentExchange
            {
                fItemId = itemId,
                fFromUserId = userId,
                fPostTime = DateTime.Now,
                fContent = message
            };
            db.tCommentExchange.Add(newComment);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return;
        }
        public int GetCount(string sortName)
        {
            return db.tExchangeItem.Where(m => m.fSort.Contains(sortName)).Count();
        }

        public ActionResult ExchangeItemUserB (string itemId)
        {
            bool isExist = db.tExchangeItemTable.Any(m => m.fItemId == itemId);
            if (isExist)
            {

                var queryResult = db.tExchangeItemTable
                    .Where(p => p.fItemId == itemId)
                    .Join(db.tMember,
                          c => c.fPostUserId,
                          m => m.fUserId,
                          (c, m) => new
                          {
                              fItemName = c.fItemName,
                              fItemTableId = c.fItemTableId,
                              postAcc = m.fAccount,   //帳戶
                              ItemDescription = c.fItemDescription,   //內容
                              fPhoto0 = c.fPhoto0,
                              fPhoto1 = c.fPhoto1,
                              fPhoto2 = c.fPhoto2,
                              fPhoto3 = c.fPhoto3,
                              Sort = c.fSort,
                              ItemLevel = c.fItemLevel,
                              ItemLocation = c.fItemLocation
                              //postTime = c.fPostTime  //發布時間
                          });
                    //.OrderBy(n => n.postTime); ;
                return Json(queryResult, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult btnSureResult(string itemIdA ,string itemIdB)
        {
            bool isExist = db.tExchangeItemTable.Any(m => m.fItemId == itemIdA && m.fItemTableId == itemIdB);
            if (isExist)
            {
                tExchangeItem itemA = db.tExchangeItem.Find(itemIdA);
                itemA.fDelete = true;
                tExchangeItemTable itemB = db.tExchangeItemTable.Find(itemIdB);
                string UserAId = itemA.fPostUserId;
                string UserBId = itemB.fPostUserId;
                tMember UserA = db.tMember.Find(UserAId);
                tMember UserB = db.tMember.Find(UserBId);
                tExchangeResult btnSureResult = new tExchangeResult
                {
                    fItemIdA = itemIdA,
                    fItemIdB = itemIdB,
                    fItemNameA = itemA.fItemName,
                    fItemNameB = itemB.fItemName,
                    fAPhoto0 = itemA.fPhoto0,
                    fBPhoto0 = itemB.fPhoto0,
                    fPostUserId = itemA.fPostUserId,
                    fPostAccount = UserA.fAccount,
                    fCoupleUserId = itemB.fPostUserId,
                    fCoupleAccount = UserB.fAccount,
                    fSubmitTime = DateTime.Now,
                    fEndTime = itemA.fEndTime,
                    fStatus = "交易成功"

                    
                };
                db.tExchangeResult.Add(btnSureResult);
                db.SaveChanges();
            }
            
            return View();
        }
    }
}