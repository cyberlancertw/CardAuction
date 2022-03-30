﻿using System;
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
            return View();
        }


        [HttpGet]
        public ActionResult Error(string ErrorMessage, string ToController, string ToAction, string ToId)
        {
            TempData["ErrorMessage"] = ErrorMessage;
            TempData["ToController"] = ToController;
            TempData["ToAction"] = ToAction;
            TempData["ToId"] = ToId;
            return View();
        }
        [HttpGet]
        public ActionResult Search(string id)
        {
            CHomeSearchViewModel vModel = new CHomeSearchViewModel();
            vModel.keyword = id;
            if (string.IsNullOrEmpty(id))                               // 空的
            {
                vModel.fullMatch = new List<QueryResult>();
                vModel.partialMatch = new List<QueryResult>();
                return View(vModel);
            }

            string[] keywordList = id.Split(' ');
            int keywordCount = keywordList.Length;

            IQueryable<QueryItem> informations = db.tAuctionItem        // 撈商品名、描述、分類、鑑定 成待查字串
                .Select(m => new QueryItem
                {
                    itemId = m.fItemId,
                    information = m.fItemName + m.fItemDescription + m.fSort + m.fGrading
                });
            List<List<string>> queryResultItemId = new List<List<string>>();

            foreach (string queryKey in keywordList)
            {
                var resultStrings = informations.Where(m => m.information.Contains(queryKey)).Select(r => r.itemId).ToList();
                queryResultItemId.Add(resultStrings);
            }

            
            List<QueryResult> andQueryResult = new List<QueryResult>();
            

            List<string> AndResult = queryResultItemId[0];
            for(int i=1; i < queryResultItemId.Count; i++)
            {
                AndResult = AndResult.Intersect(queryResultItemId[i]).ToList();
            }
            foreach(string str in AndResult)
            {
                tAuctionItem item = db.tAuctionItem.Find(str);
                andQueryResult.Add(new QueryResult
                {
                    fItemId = item.fItemId,
                    fEndTime = item.fEndTime,
                    fItemName = item.fItemName,
                    fMoneyNow = item.fMoneyNow,
                    fPhoto = item.fPhoto0
                });
            }
            vModel.fullMatch = andQueryResult;

            if(queryResultItemId.Count == 1)                        // 若只有一個關鍵字，就無部份符合的list要做，直接傳View
            {
                vModel.partialMatch = new List<QueryResult>();
                return View(vModel);
            }

            List<QueryResult> orQueryResult = new List<QueryResult>();
            List<string> OrResult = new List<string>();
            foreach(List<string> result in queryResultItemId)
            {
                List<string> temp = result.Except(AndResult).Except(OrResult).ToList();         // 差集掉交集結果和前面已蒐集的部分
                OrResult.AddRange(temp);
            }
            foreach (string str in OrResult)
            {
                tAuctionItem item = db.tAuctionItem.Find(str);
                orQueryResult.Add(new QueryResult
                {
                    fItemId = item.fItemId,
                    fEndTime = item.fEndTime,
                    fItemName = item.fItemName,
                    fMoneyNow = item.fMoneyNow,
                    fPhoto = item.fPhoto0
                });
            }
            vModel.partialMatch = orQueryResult;
            return View(vModel);
        }
        [HttpGet]
        public ActionResult Post()
        {
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