using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardAuction.Models;

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


            foreach (string queryKey in keywordList)
            {
                IQueryable<QueryItem> result = informations
                    .Where(m => m.information.Contains(queryKey));

                queryResult.Add(result);
            }

            List<QueryItem> finalResult = new List<QueryItem>();

            foreach(var item in queryResult)
            {
                foreach(var itemmm in item)
                {

                    finalResult.Add(itemmm);
                }
            }
            return View(finalResult);
        }

    }

    public class QueryItem
    {
        public string itemId { get; set; }
        public string information { get; set; }
    }
}