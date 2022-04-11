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

        //GET: Auction
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

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

            if (db.tAuctionResult.Find(id) != null)     // tAuctionResult 有東西表示此商品已結結標
            {
                return RedirectToAction("Result", new { id = id });
            }
            if (result.fDelete && result.fEndTime > DateTime.Now)            // 被檢舉下架或不明原因
            {
                return RedirectToAction("Error", "Home", new { ErrorMessage = "商品不存在", ToController = "Auction", ToAction = "List" });
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
            Session[CDictionary.SK_BackTo] = new CLinkTo("Auction", "Item", id);
            Session[CDictionary.SK_RedirectTo] = new CLinkTo("Auction", "Item", id);
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
                return RedirectToAction("Error", "Home", new { ErrorMessage = "商品還未結束競標", ToController = "Auction", ToAction = "Item", ToId = id });
            }
            string postUserId = result.fPostUserId;
            string winUserId = result.fWinUserId;
            
            if (Session[CDictionary.SK_UserUserId] == null)
            {
                Session[CDictionary.SK_BackTo] = new CLinkTo("Auction", "List");
                Session[CDictionary.SK_RedirectTo] = new CLinkTo("Auction", "Result", id);
                
                return RedirectToAction("Login", "Member");
            }
            string loginUserId = Session[CDictionary.SK_UserUserId].ToString();
            if (!(loginUserId == postUserId || loginUserId == winUserId))
            {
                return RedirectToAction("Error", "Home", new { ErrorMessage = "您非得標者或商品主人", ToController = "Auction", ToAction = "List" });
            }

            VMAuctionResult vModel = new VMAuctionResult();
            vModel.postUserAccount = db.tMember.Find(postUserId).fAccount;

            vModel.winUserAccount = string.IsNullOrEmpty(winUserId) ? "無人得標" : db.tMember.Find(winUserId).fAccount;
            vModel.result = result;
            vModel.history = db.tAuctionBid
                .Where(m=>m.fItemId == id)
                .Join(
                db.tMember,
                b => b.fUserId,
                m => m.fUserId,
                (b, m) => new BidDetail
                {
                    bidAccount = m.fAccount,
                    bidMoney = b.fMoney,
                    bidTime = b.fTime
                }).OrderByDescending(p => p.bidTime);

            return View(vModel);
        }
        [HttpGet]
        public ActionResult Post()
        {
            if (Session[CDictionary.SK_UserAccount] == null)             // 沒登入不給上架，送去登入頁
            {
                Session[CDictionary.SK_RedirectTo] = new CLinkTo("Auction", "Post");
                return RedirectToAction("Login", "Member");
            }
            else
            {
                Session[CDictionary.SK_BackTo] = new CLinkTo("Auction", "Post");
                return View();
            }
        }

        public ActionResult Post(CAuctionPostViewModel vModel)
        {
            if (Session[CDictionary.SK_UserAccount] == null)             // 沒登入不給上架，送去登入頁
            {
                Session[CDictionary.SK_RedirectTo] = new CLinkTo("Auction", "Post");
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
            createItem.fTransOk = vModel.isOk ? vModel.fTransOk : -1;
            createItem.fTransLife = vModel.isLife ? vModel.fTransLife : -1;
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
            Session[CDictionary.SK_BackTo] = new CLinkTo("Auction", "List");
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult List()
        {
            Session[CDictionary.SK_RedirectTo] = new CLinkTo("Auction", "List");
            Session[CDictionary.SK_BackTo] = new CLinkTo("Auction", "List");

            var itemFinish = db.tAuctionItem.Where(m => m.fEndTime < DateTime.Now && !m.fFinish);
            foreach (var item in itemFinish)
            {
                item.fFinish = true;

                
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                Service.ExceptionEmail(ex, "Auction/List");
            }
            catch(Exception e)
            {
                Service.ExceptionEmail(e, "Auction/List");
            }

            return View();
        }

        public ActionResult QueryBySort(string sortName, string filter = "EndTime", int page = 0)
        {
            switch (filter)
            {
                case "EndTime":
                    {
                        var queryResult = db.tAuctionItem
                            .Where(m => m.fEndTime > DateTime.Now && m.fSort.Contains(sortName) && !m.fFinish && !m.fDelete)
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
                            .Where(m => m.fEndTime > DateTime.Now && m.fSort.Contains(sortName) && !m.fFinish && !m.fDelete)
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
                            .Where(m => m.fEndTime > DateTime.Now && m.fSort.Contains(sortName) && !m.fFinish && !m.fDelete)
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

        public int GetCount(string sortName)                        // 做 ~/Auction/List 分頁按鈕用的
        {
            return db.tAuctionItem.Where(m => m.fSort.Contains(sortName) && !m.fFinish && !m.fDelete).Count();
        }



        public void Bid(string itemId, int amount)
        {
            if (Session[CDictionary.SK_UserUserId] == null)
            {
                return;
            }
            string userId = Session[CDictionary.SK_UserUserId].ToString();
            
            tMember user = db.tMember.Find(userId);
            if (!user.fActive || user.fDelete)                     // 停權、刪除
            {
                return;
            }
            
            tAuctionItem item = db.tAuctionItem.Find(itemId);

            if(item == null || item.fFinish || item.fDelete)       // 錯誤商品、已結束、已刪除
            {
                return;
            }
            if (userId.Equals(item.fPostUserId))                   // 不能給自己商品出價
            {
                return;
            }
            if(item.fEndTime < DateTime.Now)                       // 時間結束
            {
                return;
            }

            if(item.fBuyPrice < 0)      // 無直購
            {
                if(amount < item.fMoneyNow + item.fMoneyStep)      // 防另一人在另一端已出過價，靠後端判斷不予更新
                {
                    return;
                }

                UpdateBid(item, amount, userId);
                return;
            }
            else                        // 有直購價
            {
                if(amount >= item.fBuyPrice)
                {
                    UpdateBid(item, item.fBuyPrice, userId);        // 出價大於等於直購價，就以直購價結束
                    return;
                }
                else if(amount < item.fMoneyNow + item.fMoneyStep)  // 防另一人在另一端已出過價，靠後端判斷不予更新
                {
                    return;
                }
                UpdateBid(item, amount, userId);
                return;
            }

        }
        private void UpdateBid(tAuctionItem item, int amount, string userId)
        {
            item.fMoneyNow = amount;
            item.fBidCount++;
            item.fTopBidUserId = userId;
            
            
            tAuctionBid newBid = new tAuctionBid
            {
                fItemId = item.fItemId,
                fTime = DateTime.Now,
                fUserId = userId,
                fMoney = amount
            };
            db.tAuctionBid.Add(newBid);

            if (item.fBuyPrice > 0 && amount >= item.fBuyPrice)         // 有直購且出大於等於直購價
            {
                item.fFinish = true;
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                Service.ExceptionEmail(ex, "Auction/Item/UpdateBid");
            }
            catch (Exception e)
            {
                Service.ExceptionEmail(e, "Auction/Item/UpdateBid");
            }
            return;
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
            tMember user = db.tMember.Find(userId);
            if (!user.fActive || user.fDelete)                                                                                          // 停權
            {
                return;
            }
            bool isExist = db.tCommentAuction.Any(m => m.fItemId == itemId && m.fContent == message && m.fFromUserId == userId);        // 防灌水
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
            catch(DbEntityValidationException ex)
            {
                Service.ExceptionEmail(ex, "Auction/Item/WriteComment");
            }
            catch (Exception e)
            {
                Service.ExceptionEmail(e, "Auction/Item/WriteComment");
            }
            return;
        }
        
        public ActionResult GetEndInfo(string itemId)
        {
            if(itemId == null)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            tAuctionItem item = db.tAuctionItem.Find(itemId);
            if(item == null)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            if(Session[CDictionary.SK_UserUserId] == null)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            string userId = Session[CDictionary.SK_UserUserId].ToString();
            tMember queryMember = db.tMember.Find(userId);
            if(queryMember == null || (userId != item.fPostUserId && userId != item.fTopBidUserId))
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                topMoney = item.fMoneyNow,
                postUserInfo = item.fUserInfo
            }, JsonRequestBehavior.AllowGet);

        }

        

        public ActionResult FinishInfo(string itemId)
        {
            if(itemId == null)
            {
                return Json(new { statusMessage = "EmptyItemId" }, JsonRequestBehavior.AllowGet);       // 空argument
            }

            tAuctionItem item = db.tAuctionItem.Find(itemId);

            if(item == null)
            {
                return Json(new { statusMessage = "ItemNotExist" }, JsonRequestBehavior.AllowGet);      // 不存在競標商品
            }
            if (item.fDelete)
            {
                return Json(new { statusMessage = "Deleted" }, JsonRequestBehavior.AllowGet);           // 已刪除
            }

            if(item.fEndTime > DateTime.Now && item.fBuyPrice < 0)
            {
                return Json(new { statusMessage = "NotFinish" }, JsonRequestBehavior.AllowGet);         // 時間未到，無直購，未結束
            }

            if (item.fEndTime > DateTime.Now && item.fBuyPrice > 0 && item.fMoneyNow < item.fBuyPrice)
            {
                return Json(new { statusMessage = "NotFinish" }, JsonRequestBehavior.AllowGet);         // 時間未到，有直購，但還未出到，未結束
            }

            if (item.fEndTime < DateTime.Now && string.IsNullOrEmpty(item.fTopBidUserId))
            {
                return Json(new { statusMessage = "NoTopFinish"}, JsonRequestBehavior.AllowGet);        // 時間到，無人得標的結束
            }
            tMember topUser = db.tMember.Find(item.fTopBidUserId);
            if(topUser == null)
            {
                return Json(new { statusMessage = "NoTopNotFinish" }, JsonRequestBehavior.AllowGet);    // 時間到，無人得標的結束
            }
            string winUserAcc = topUser.fAccount;
            string statusMessage = "EndTimeFinish";                                                     // 時間到，有人得標的結束

            if (item.fBuyPrice > 0 && item.fMoneyNow >= item.fBuyPrice)
            {
                statusMessage = "BuyFinish";                                                            // 直購結束
            }

            return Json(new { 
                statusMessage = statusMessage,
                winUserAcc = winUserAcc,
                winMoney = item.fMoneyNow,
                transPerson = item.fTransPerson,
                transSeven = item.fTransSeven,
                transFami = item.fTransFami,
                transOk = item.fTransOk,
                transLife = item.fTransLife,
                transLogi = item.fTransLogi
            }, JsonRequestBehavior.AllowGet);

        }

        public void DeleteItem(string itemId)
        {
            if(itemId == null)                              // 無arguement
            {
                return;
            }
            
            tAuctionItem item = db.tAuctionItem.Find(itemId);
            
            if(item == null || item.fDelete)                // 查無競標商品 或 已結束
            {
                return;
            }

            item.fDelete = true;                            // 結束此競標商品

            tAuctionResult newResult = new tAuctionResult   // 新增 result 紀錄
            {
                fResultId = itemId,
                fPostUserId = item.fPostUserId,
                fWinUserId = item.fTopBidUserId,
                fPhoto0 = item.fPhoto0,
                fPhoto1 = item.fPhoto1,
                fPhoto2 = item.fPhoto2,
                fPhoto3 = item.fPhoto3,
                fTotalMoney = item.fMoneyNow,
                fBidCount = item.fBidCount,
                fWinTime = item.fEndTime,
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
                Service.ExceptionEmail(ex, "Auction/Item/DeleteItem");
            }
            catch(Exception e)
            {
                Service.ExceptionEmail(e, "Auction/Item/DeleteItem");
            }
        }

        public void GenerateResult(string itemId, int totalMoney, string deliveryInfo)
        {
            if(itemId == null || deliveryInfo == null)
            {
                return;
            }
            tAuctionItem item = db.tAuctionItem.Find(itemId);
            if(item == null)
            {
                return;
            }
            if (db.tAuctionResult.Any(m => m.fResultId == itemId))
            {
                return;
            }
            if (item.fDelete)
            {
                return;
            }
            if (item.fEndTime > DateTime.Now && item.fBuyPrice < 0)
            {
                return;
            }
            if(item.fEndTime > DateTime.Now && item.fBuyPrice > item.fMoneyNow)
            {
                return;
            }
            item.fDelete = true;
            tAuctionResult newResult = new tAuctionResult
            {
                fResultId = itemId,
                fPostUserId = item.fPostUserId,
                fBidCount = item.fBidCount,
                fBidMoney = item.fMoneyNow,
            };
            if (!string.IsNullOrEmpty(item.fPhoto0))
            {
                newResult.fPhoto0 = item.fPhoto0;
            }
            if (!string.IsNullOrEmpty(item.fPhoto1))
            {
                newResult.fPhoto2 = item.fPhoto2;
            }
            if (!string.IsNullOrEmpty(item.fPhoto2))
            {
                newResult.fPhoto2 = item.fPhoto2;
            }
            if (!string.IsNullOrEmpty(item.fPhoto3))
            {
                newResult.fPhoto3 = item.fPhoto3;
            }

            tMember postUser = db.tMember.Find(item.fPostUserId);
            string postUserAcc = postUser.fAccount;
            string postUserEmail = postUser.fEmail;

            if(item.fEndTime < DateTime.Now && item.fBidCount == 0)
            {
                newResult.fDeliveryInfo = "無人競標。流標。";
                string content = $"<h2>您好，您在 {item.fCreateTime.ToString()} 上架的商品「{item.fItemName}」在 {item.fEndTime.ToString()} 時間結束時無人參與競標，故已下架</h2><h3>系統信件請勿回信。by <a href=\"${CDictionary.WebHost}\">CARDs.卡市</a> 團隊</h3>";
                Service.SendEmail(postUserEmail, "CARDs.卡市 - 商品流標通知", content);
                newResult.fWinTime = item.fEndTime;
                db.tAuctionResult.Add(newResult);
                try
                {
                    db.SaveChanges();
                }
                catch(DbEntityValidationException ex)
                {
                    Service.ExceptionEmail(ex, "Auction/Item/GenerateResult/item.fEndTime<Date.Time.Now && item.fBidCount==0");
                }
                catch(Exception e)
                {
                    Service.ExceptionEmail(e, "Auction/Item/GenerateResult/item.fEndTime<Date.Time.Now && item.fBidCount==0");
                }
                return;
            }
            if(item.fBuyPrice > 0 && item.fBuyPrice <= item.fMoneyNow)
            {
                DateTime buyTime = db.tAuctionBid.Where(m => m.fItemId == itemId).Max(m => m.fTime);
                tMember buyUser = db.tMember.Find(item.fTopBidUserId);
                if(buyUser == null)
                {
                    return;
                }
                string buyUserAcc = buyUser.fAccount;
                string buyUserEmail = buyUser.fEmail;
                string content = $"<h2>您好，您在 {item.fCreateTime.ToString()} 上架的商品「{item.fItemName}」在 {buyTime.ToString()} 時由 {buyUserAcc} 以 {item.fMoneyNow} 元價格標下，恭喜您。以下是對方的聯絡運送資訊：</h2><h2>{deliveryInfo}</h2><h3>系統信件請勿回信。by <a href=\"${CDictionary.WebHost}\">CARDs.卡市</a> 團隊</h3>";
                Service.SendEmail(postUserEmail, "CARDs.卡市 - 商品標出通知", content);
                content = $"<h2>您好，您在 {buyTime.ToString()} 對 {postUserAcc} 上架的商品「{item.fItemName}」以 {item.fMoneyNow} 元價格標下，恭喜您。以下是對方的聯絡運送資訊：</h2><h2>{item.fUserInfo}</h2><h3>系統信件請勿回信。by <a href=\"${CDictionary.WebHost}\">CARDs.卡市</a> 團隊</h3>";
                Service.SendEmail(buyUserEmail, "CARDs.卡市 - 商品得標通知", content);
                newResult.fWinUserId = item.fTopBidUserId;
                newResult.fWinTime = buyTime;
                newResult.fDeliveryInfo = deliveryInfo;
                newResult.fTotalMoney = totalMoney;

                db.tAuctionResult.Add(newResult);
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    Service.ExceptionEmail(ex, "Auction/Item/GenerateResult/item.fBuyPrice > 0 && item.fBuyPrice <= item.fMoneyNow");
                }
                catch (Exception e)
                {
                    Service.ExceptionEmail(e, "Auction/Item/GenerateResult/item.fBuyPrice > 0 && item.fBuyPrice <= item.fMoneyNow");
                }
                return;
            }
            if (item.fEndTime < DateTime.Now && item.fBidCount > 0)
            {
                DateTime bidTime = db.tAuctionBid.Where(m => m.fItemId == itemId).Max(m => m.fTime);
                tMember bidUser = db.tMember.Find(item.fTopBidUserId);
                if (bidUser == null)
                {
                    return;
                }
                string bidUserAcc = bidUser.fAccount;
                string bidUserEmail = bidUser.fEmail;
                string content = $"<h2>您好，您在 {item.fCreateTime.ToString()} 上架的商品「{item.fItemName}」在 {bidTime.ToString()} 時由 {bidUserAcc} 以 {item.fMoneyNow} 元價格標下，恭喜您。以下是對方的聯絡運送資訊：</h2><h2>{deliveryInfo}</h2><h3>系統信件請勿回信。by <a href=\"${CDictionary.WebHost}\">CARDs.卡市</a> 團隊</h3>";
                Service.SendEmail(postUserEmail, "CARDs.卡市 - 商品標出通知", content);
                content = $"<h2>您好，您在 {bidTime.ToString()} 對 {postUserAcc} 上架的商品「{item.fItemName}」以 {item.fMoneyNow} 元價格標下，恭喜您。以下是對方的聯絡運送資訊：</h2><h2>{item.fUserInfo}</h2><h3>系統信件請勿回信。by <a href=\"${CDictionary.WebHost}\">CARDs.卡市</a> 團隊</h3>";
                Service.SendEmail(bidUserEmail, "CARDs.卡市 - 商品得標通知", content);
                newResult.fWinUserId = item.fTopBidUserId;
                newResult.fWinTime = item.fEndTime;
                newResult.fDeliveryInfo = deliveryInfo;
                newResult.fTotalMoney = totalMoney;

                db.tAuctionResult.Add(newResult);
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    Service.ExceptionEmail(ex, "Auction/Item/GenerateResult/item.fEndTime < DateTime.Now && item.fBidCount > 0");
                }
                catch (Exception e)
                {
                    Service.ExceptionEmail(e, "Auction/Item/GenerateResult/item.fEndTime < DateTime.Now && item.fBidCount > 0");
                }
                return;
            }

            return;
        }

        public void InfoBidWinner(string itemId)
        {
            if (string.IsNullOrEmpty(itemId) || Session[CDictionary.SK_UserUserId] == null)
            {
                return;
            }
            tAuctionItem item = db.tAuctionItem.Find(itemId);
            tMember topUser = db.tMember.Find(item.fTopBidUserId);
            if (item == null || topUser == null)
            {
                return;
            }
            string postUserId = item.fPostUserId;
            if(Session[CDictionary.SK_UserUserId] == null || Session[CDictionary.SK_UserUserId].ToString() != postUserId)
            {
                return;
            }
            string postAcc = db.tMember.Find(postUserId).fAccount;
            string userAcc = topUser.fAccount;
            string userEmail = topUser.fEmail;
            string subject = "CARDs.卡市 - 得標通知";
            string linkTo = CDictionary.WebHost + "Auction/Item/" + item.fItemId;
            string content = $"<h1>{userAcc}您好，您得標了{postAcc}的競標商品「{item.fItemName}」，請儘速登入個人頁面，或利用以下連結填寫運送資訊：</h1>"
                + $"<h2><a href=\"{linkTo}\">{linkTo}</a></h2><h3>系統信件請勿回信。by <a href=\"${CDictionary.WebHost}\">CARDs.卡市</a> 團隊</h3>";
            Service.SendEmail(userEmail, subject, content);



            List<string> infoItems = Session[CDictionary.SK_BellInfoItems] as List<string>;
            if (infoItems == null)
            {
                infoItems = new List<string>();
            }
            if (!infoItems.Contains(itemId))
            {
                infoItems.Add(itemId);
            }
            return;

        }

    }

}