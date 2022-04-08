using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardAuction.Models;
using CardAuction.ViewModels;

namespace CardAuction.Controllers
{
    public class HomeController : Controller
    {
        dbCardAuctionEntities db = new dbCardAuctionEntities();

        [HttpGet]
        // GET: Home
        public ActionResult Index()
        {
            Session[CDictionary.SK_BackTo] = new CLinkTo("Home", "Index");
            return View();
        }


        [HttpGet]
        public ActionResult Error(string ErrorMessage, string ToController, string ToAction, string ToId)
        {
            TempData["ErrorMessage"] = ErrorMessage;
            TempData["ToController"] = ToController;
            TempData["ToAction"] = ToAction;
            TempData["ToId"] = ToId;
            Session[CDictionary.SK_BackTo] = new CLinkTo(ToController, ToAction, ToId);
            return View();
        }
        [HttpGet]
        public ActionResult Search(string id)
        {
            Session[CDictionary.SK_BackTo] = new CLinkTo("Home", "Search", id);
            Session[CDictionary.SK_RedirectTo] = new CLinkTo("Home", "Search", id);
            CHomeSearchViewModel vModel = new CHomeSearchViewModel();
            vModel.keyword = id;
            if (string.IsNullOrEmpty(id))                               // 空的
            {
                vModel.auctionFullMatch = new List<QueryResult>();
                vModel.auctionPartialMatch = new List<QueryResult>();
                vModel.exchangeFullMatch = new List<QueryResult>();
                vModel.exchangePartialMatch = new List<QueryResult>();
                return View(vModel);
            }

            string[] keywordList = id.Split(' ');
            int keywordCount = keywordList.Length;

            IQueryable<QueryItem> auctionInformations = db.tAuctionItem        // 撈商品名、描述、分類、鑑定 成索引
                .Select(m => new QueryItem
                {
                    itemId = m.fItemId,
                    information = m.fItemName + m.fItemDescription + m.fSort + m.fGrading
                });
            IQueryable<QueryItem> exchangeInformations = db.tExchangeItem        // 撈商品名、描述、分類、鑑定 成索引
                .Select(m => new QueryItem
                {
                    itemId = m.fItemId,
                    information = m.fItemName + m.fItemDescription + m.fSort + m.fItemLevel + m.fItemLocation
                });
            List<List<string>> auctionQueryResultItemId = new List<List<string>>();
            List<List<string>> exchangeQueryResultItemId = new List<List<string>>();

            foreach (string queryKey in keywordList)
            {
                var auctionResultStrings = auctionInformations.Where(m => m.information.Contains(queryKey)).Select(r => r.itemId).ToList();
                var exchangeResultStrings = exchangeInformations.Where(m => m.information.Contains(queryKey)).Select(r => r.itemId).ToList();
                auctionQueryResultItemId.Add(auctionResultStrings);
                exchangeQueryResultItemId.Add(exchangeResultStrings);
            }

            List<QueryResult> auctionAndQueryResult = new List<QueryResult>();
            List<QueryResult> exchangeAndQueryResult = new List<QueryResult>();

            List<string> auctionAndResult = auctionQueryResultItemId[0];
            List<string> exchangeAndResult = exchangeQueryResultItemId[0];

            for (int i = 1; i < auctionQueryResultItemId.Count; i++)
            {
                auctionAndResult = auctionAndResult.Intersect(auctionQueryResultItemId[i]).ToList();
            }
            for (int i = 1; i < exchangeQueryResultItemId.Count; i++)
            {
                exchangeAndResult = exchangeAndResult.Intersect(exchangeQueryResultItemId[i]).ToList();
            }
            foreach (string str in auctionAndResult)
            {
                tAuctionItem item = db.tAuctionItem.Find(str);
                auctionAndQueryResult.Add(new QueryResult
                {
                    fItemId = item.fItemId,
                    fEndTime = item.fEndTime,
                    fItemName = item.fItemName,
                    fMoneyNow = item.fMoneyNow,
                    fPhoto = item.fPhoto0
                });
            }
            foreach (string str in exchangeAndResult)
            {
                tExchangeItem item = db.tExchangeItem.Find(str);
                exchangeAndQueryResult.Add(new QueryResult
                {
                    fItemId = item.fItemId,
                    fEndTime = item.fEndTime,
                    fItemName = item.fItemName,
                    fChangeCount = item.fChangeCount,
                    fPhoto = item.fPhoto0
                });
            }
            vModel.auctionFullMatch = auctionAndQueryResult;
            vModel.exchangeFullMatch = exchangeAndQueryResult;

            List<QueryResult> auctionOrQueryResult = new List<QueryResult>();
            List<string> auctionOrResult = new List<string>();
            List<QueryResult> exchangeOrQueryResult = new List<QueryResult>();
            List<string> exchangeOrResult = new List<string>();

            foreach (List<string> result in auctionQueryResultItemId)
            {
                List<string> temp = result.Except(auctionAndResult).Except(auctionOrResult).ToList();         // 差集掉交集結果和前面已蒐集的部分
                auctionOrResult.AddRange(temp);
            }
            foreach (List<string> result in exchangeQueryResultItemId)
            {
                List<string> temp = result.Except(exchangeAndResult).Except(exchangeOrResult).ToList();         // 差集掉交集結果和前面已蒐集的部分
                exchangeOrResult.AddRange(temp);
            }
            foreach (string str in auctionOrResult)
            {
                tAuctionItem item = db.tAuctionItem.Find(str);
                auctionOrQueryResult.Add(new QueryResult
                {
                    fItemId = item.fItemId,
                    fEndTime = item.fEndTime,
                    fItemName = item.fItemName,
                    fMoneyNow = item.fMoneyNow,
                    fPhoto = item.fPhoto0
                });
            }
            vModel.auctionPartialMatch = auctionOrQueryResult;

            foreach (string str in exchangeOrResult)
            {
                tExchangeItem item = db.tExchangeItem.Find(str);
                exchangeOrQueryResult.Add(new QueryResult
                {
                    fItemId = item.fItemId,
                    fEndTime = item.fEndTime,
                    fItemName = item.fItemName,
                    fChangeCount = item.fChangeCount,
                    fPhoto = item.fPhoto0
                });
            }
            vModel.exchangePartialMatch = exchangeOrQueryResult;
            return View(vModel);
        }
        [HttpGet]
        public ActionResult Post()
        {
            Session[CDictionary.SK_BackTo] = new CLinkTo("Home", "Post");
            Session[CDictionary.SK_RedirectTo] = new CLinkTo("Home", "Post");
            return View();
        }

        public ActionResult QueryNewest()
        {
            var queryAuctionResult = db.tAuctionItem
                .Where(m => m.fEndTime > DateTime.Now)
                .OrderByDescending(p => p.fCreateTime)
                .Take(4)
                .Select(n => new QueryResult
                {
                    fItemId = n.fItemId,
                    fEndTime = n.fEndTime,
                    fItemName = n.fItemName,
                    fPhoto = n.fPhoto0,
                    fMoneyNow = n.fMoneyNow,
                    fBidCount = n.fBidCount
                });
            var queryExchangeResult = db.tExchangeItem
                .Where(m => m.fEndTime > DateTime.Now)
                .OrderByDescending(p => p.fCreateTime)
                .Take(4)
                .Select(n => new QueryResult
                {
                    fItemId = n.fItemId,
                    fItemName = n.fItemName,
                    fEndTime = n.fEndTime,
                    fPhoto = n.fPhoto0,
                    fChangeCount = n.fChangeCount
                });
            QueryNewestList queryResult = new QueryNewestList();
            queryResult.newestAuctionItem = queryAuctionResult.ToArray();
            queryResult.newestExchangeItem = queryExchangeResult.ToArray();

            return Json(queryResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            Session[CDictionary.SK_RedirectTo] = new CLinkTo("Home", "About");
            Session[CDictionary.SK_BackTo] = new CLinkTo("Home", "About");
            return View();
        }

        public string FindFinishAuctionItem()
        {
            if(Session[CDictionary.SK_UserUserId] == null)
            {
                return false.ToString();
            }
            string userId = Session[CDictionary.SK_UserUserId].ToString();

            bool itemFinish = db.tAuctionItem.Any(m => (m.fPostUserId == userId || m.fTopBidUserId == userId)
                                                    && (m.fEndTime < DateTime.Now)
                                                     && m.fFinish && !m.fDelete);
            return itemFinish.ToString();
        }
    }



    public class QueryNewestList
    {
        public QueryResult[] newestAuctionItem { get; set; }
        public QueryResult[] newestExchangeItem { get; set; }
    }

    public class QueryItem
    {
        public string itemId { get; set; }
        public string information { get; set; }
    }


}