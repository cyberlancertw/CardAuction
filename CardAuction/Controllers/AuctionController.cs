using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
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

        [HttpGet]
        public ActionResult Item(string id)
        {
            if (id == null)                             // 沒輸入 ItemId
            {
                return RedirectToAction("Error", "Home", new { ErrorMessage = "需要商品編號", ToController = "Auction", ToAction = "List" });
            }
            var result = db.tAuctionItem.Find(id);
            if (result == null)                         // 有輸入 Id 但查不到
            {
                return RedirectToAction("Error", "Home", new { ErrorMessage = "商品不存在", ToController = "Auction", ToAction = "List" });
            }

            if (db.tAuctionResult.Find(id) != null)
            {
                return RedirectToAction("Result", new { id = id });
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

        public ActionResult Result(string id)
        {
            if(id == null)
            {
                return RedirectToAction("Error", "Home", new { ErrorMessage = "需要商品編號", ToController = "Auction", ToAction = "List" });
            }
            tAuctionItem item = db.tAuctionItem.Find(id);
            if(item == null)
            {
                return RedirectToAction("Error", "Home", new { ErrorMessage = "商品不存在", ToController = "Auction", ToAction = "List" });
            }
            tAuctionResult result = db.tAuctionResult.Find(id);
            if(result == null)
            {
                return RedirectToAction("Error", "Home", new { ErrorMessage = "商品還未結速競標", ToController = "Auction", ToAction = "Item", ToId = id });
            }
            string postUserId = result.fPostUserId;
            string winUserId = result.fWinUserId;
            
            if (Session[CDictionary.SK_UserUserId] == null)
            {
                TempData[CDictionary.SK_RedirectToController] = "Auction";
                TempData[CDictionary.SK_RedirectToAction] = "Result";
                TempData[CDictionary.SK_RedirectToId] = id;
                return RedirectToAction("Login", "Member");
            }

            if(Session[CDictionary.SK_UserUserId].ToString() != postUserId || Session[CDictionary.SK_UserUserId].ToString() != winUserId)
            {
                return RedirectToAction("Error", "Home", new { ErrorMessage = "您非得標者或商品主人", ToController = "Auction", ToAction = "List" });
            }

            return View(result);
        }
        [HttpGet]
        public ActionResult Post()
        {
            if (Session[CDictionary.SK_UserAccount] == null)             // 沒登入不給上架，送去登入頁
            {
                TempData[CDictionary.SK_RedirectToAction] = "Post";
                TempData[CDictionary.SK_RedirectToController] = "Auction";

                return RedirectToAction("Login", "Member");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Post(CAuctionPostViewModel vModel)
        {
            if (Session[CDictionary.SK_UserAccount] == null)             // 沒登入不給上架，送去登入頁
            {
                TempData[CDictionary.SK_RedirectToAction] = "Post";
                TempData[CDictionary.SK_RedirectToController] = "Auction";
                return RedirectToAction("Login", "Member");
            }

            tAuctionItem createItem = new tAuctionItem();
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

            createItem.fBuyPrice = vModel.isBuy ? vModel.fBuyPrice : -1;                            // 以負數表示不提供直購價

            createItem.fTransPerson = vModel.isPerson ? vModel.fTransPerson : -1;                   // 以負數表示不提供此運送選項
            createItem.fTransSeven = vModel.isSeven ? vModel.fTransSeven : -1;
            createItem.fTransFami = vModel.isFami ? vModel.fTransFami : -1;
            createItem.fTransLogi = vModel.isLogi ? vModel.fTransLogi : -1;
            createItem.fUserInfo = vModel.fUserInfo;

            createItem.fEndTime = vModel.fEndTimeDate.Date.Add(vModel.fEndTimeTime.TimeOfDay);      // 由選擇的日期和時間合併成結標時間
            createItem.fCreateTime = nowTime;                                                       // 現在時間為建立時間
            createItem.fMoneyStart = vModel.fMoneyStart;
            createItem.fMoneyStep = vModel.fMoneyStep;
            createItem.fMoneyNow = vModel.fMoneyStart;                                              // 目前價格即起標價格
            createItem.fClick = 0;
            createItem.fDelete = false;
            createItem.fReport = 0;

            db.tAuctionItem.Add(createItem);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return RedirectToAction("Error", "Home", new { ErrorMessage = $"糟糕！發生某些狀況…… {e.ToString()}", ToController = "Auction", ToAction = "Post" });
            }
            return RedirectToAction("List");
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
                        var queryResult = db.tAuctionItem
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
                                fMoneyNow = n.fMoneyNow,
                                fBidCount = n.fBidCount
                            });
                        return Json(queryResult, JsonRequestBehavior.AllowGet);
                    }
                case "HotClick":
                    {
                        var queryResult = db.tAuctionItem
                            .Where(m => m.fEndTime > DateTime.Now && m.fSort.Contains(sortName))
                            .OrderByDescending(p=>p.fClick).ThenBy(q=>q.fEndTime)
                            .Skip(page * 12)
                            .Take(12)
                            .Select(n => new QueryResult
                            {
                                fItemId = n.fItemId,
                                fEndTime = n.fEndTime,
                                fItemName = n.fItemName,
                                fPhoto = n.fPhoto0,
                                fMoneyNow = n.fMoneyNow,
                                fBidCount = n.fBidCount
                            });
                        return Json(queryResult, JsonRequestBehavior.AllowGet);
                    }
                case "JustPost":
                    {
                        var queryResult = db.tAuctionItem
                            .Where(m => m.fEndTime > DateTime.Now && m.fSort.Contains(sortName))
                            .OrderByDescending(p=>p.fCreateTime)
                            .Skip(page * 12)
                            .Take(12)
                            .Select(n => new QueryResult
                            {
                                fItemId = n.fItemId,
                                fEndTime = n.fEndTime,
                                fItemName = n.fItemName,
                                fPhoto = n.fPhoto0,
                                fMoneyNow = n.fMoneyNow,
                                fBidCount = n.fBidCount
                            });
                        return Json(queryResult, JsonRequestBehavior.AllowGet);
                    }
                
                default:
                    break;
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);

        }

        public int GetCount(string sortName)
        {
            return db.tAuctionItem.Where(m => m.fSort.Contains(sortName)).Count();
        }



        public void Bid(string itemId, int amount)
        {
            if (Session[CDictionary.SK_UserUserId] == null)
            {
                return;
            }
            string userId = Session[CDictionary.SK_UserUserId].ToString();
            tAuctionItem item = db.tAuctionItem.Find(itemId);

            if (userId.Equals(item.fPostUserId))      // 不給自己商品出價
            {
                return;
            }
            if (amount < item.fMoneyNow + item.fMoneyStep)         // 防另一人在另一端已出過價，不能只靠前端
            {
                return;
            }
            //  無直購                 有直購但出不到直購價
            if (item.fBuyPrice < 0 || amount < item.fBuyPrice)
            {
                UpdateBid(item, amount, userId, itemId);
                return;
            }

            //   直購的部份
            if (item.fBuyPrice > 0 && amount >= item.fMoneyNow + item.fMoneyStep)
            {
                UpdateBid(item, amount, userId, itemId);
                HandleWinBid(item, amount, userId, itemId);
            }

            return;
        }

        private void UpdateBid(tAuctionItem item, int amount, string userId, string itemId)
        {
            item.fMoneyNow = amount;
            item.fBidCount++;
            tAuctionBid newBid = new tAuctionBid
            {
                fItemId = itemId,
                fTime = DateTime.Now,
                fUserId = userId,
                fMoney = amount
            };

            db.tAuctionBid.Add(newBid);
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public ActionResult ReceiveBidRecords(string itemId)
        {
            bool isExist = db.tAuctionBid.Any(m => m.fItemId == itemId);
            if (isExist)
            {
                var queryResult = db.tAuctionBid
                    .Where(p => p.fItemId == itemId)
                    .Join(db.tMember,
                          b => b.fUserId,
                          m => m.fUserId,
                          (b, m) => new
                          {
                              bidAcc = m.fAccount,
                              bidTime = b.fTime,
                              bidMoney = b.fMoney
                          })
                    .OrderByDescending(r => r.bidTime);
                return Json(queryResult, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReceiveComments(string itemId)
        {
            bool isExist = db.tCommentAuction.Any(m => m.fItemId == itemId);
            if (isExist)
            {
                var queryResult = db.tCommentAuction
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

        public void WriteComment(string itemId, string message)
        {
            if (Session[CDictionary.SK_UserUserId] == null)
            {
                return;
            }
            string userId = Session[CDictionary.SK_UserUserId].ToString();
            bool isExist = db.tCommentAuction.Any(m => m.fItemId == itemId && m.fContent == message && m.fFromUserId == userId);
            if (isExist)
            {
                return;
            }
            tCommentAuction newComment = new tCommentAuction
            {
                fItemId = itemId,
                fFromUserId = userId,
                fPostTime = DateTime.Now,
                fContent = message
            };
            db.tCommentAuction.Add(newComment);
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

        public void HandleWinBid(tAuctionItem item, int amount, string userId, string itemId)
        {
            tAuctionResult query = db.tAuctionResult.Find(itemId);
            if(query != null)
            {
                return;
            }
            tAuctionResult newResult = new tAuctionResult
            {
                fResultId = itemId,
                fPostUserId = item.fPostUserId,
                fWinUserId = userId,
                fTotalMoney = amount,
                fBidCount = item.fBidCount,
                fWinTime = DateTime.Now,
                fBidMoney = item.fMoneyNow,
                fDeliveryInfo = string.Empty
            };

            db.tAuctionResult.Add(newResult);
            try
            {
                db.SaveChanges();
            }
            catch(DbEntityValidationException ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return;
        }
    }

}