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
            return View();              // 之後塞AuctionItem和ExchangeItem，也可以不用，搜尋用 Ajax 撈出來放不用 vModel
        }

        [HttpGet]
        public ActionResult Error(string ErrorMessage, string ToController, string ToAction)
        {
            TempData["ErrorMessage"] = ErrorMessage;
            TempData["ToController"] = ToController;
            TempData["ToAction"] = ToAction;
            return View();
        }
        [HttpGet]
        public ActionResult Search(string keyword = "pokemon")
        {
            // 搜尋 split
            // 比對 fItemName, fItemDescription, fSort, fGrading
            // intersect , union
            //return Json();

            string[] keywordList = keyword.Split(' ');
            int keywordCount = keywordList.Length;

            
            IQueryable<QueryItem> informations = db.tAuctionItem
                .Select(m => new QueryItem
                {
                    itemId = m.fItemId,
                    information = m.fItemName + m.fItemDescription + m.fSort + m.fGrading
                });
            List<IQueryable<QueryItem>> queryResult = new List<IQueryable<QueryItem>>();
            List<List<string>> queryResultItemId = new List<List<string>>();

            foreach (string queryKey in keywordList)
            {
                var resultStrings = informations.Where(m => m.information.Contains(queryKey)).Select(r => r.itemId).ToList();
                queryResultItemId.Add(resultStrings);
            }

            CHomeSearchViewModel vModel = new CHomeSearchViewModel();
            List<QueryResult> finalResult = new List<QueryResult>();
            List<string> AndResult = queryResultItemId[0];

            for(int i=1; i < queryResultItemId.Count; i++)
            {
                AndResult = AndResult.Intersect(queryResultItemId[i]).ToList();
            }


            foreach(string str in AndResult)
            {
                tAuctionItem item = db.tAuctionItem.Find(str);
                finalResult.Add(new QueryResult
                {
                    fItemId = item.fItemId,
                    fEndTime = item.fEndTime,
                    fItemName = item.fItemName,
                    fMoneyNow = item.fMoneyNow,
                    TotalMatch = true,
                    fPhoto = item.fPhoto0
                });
            }
            vModel.fullMatch = finalResult;

            if(queryResultItemId.Count == 1)
            {
                vModel.partialMatch = new List<QueryResult>();
                return View(vModel);
            }

            List<string> OrResult = new List<string>();
            foreach(List<string> result in queryResultItemId)
            {
                //
                //

            }
            return View(vModel);
        }

    }

    public class QueryItem
    {
        public string itemId { get; set; }
        public string information { get; set; }
    }
}