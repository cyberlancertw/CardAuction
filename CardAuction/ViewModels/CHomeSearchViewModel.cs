using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CardAuction.Models;

namespace CardAuction.ViewModels
{
    public class CHomeSearchViewModel
    {
        public string keyword { get; set; }
        public List<QueryResult> auctionFullMatch { get; set; }
        public List<QueryResult> auctionPartialMatch { get; set; }
        public List<QueryResult> exchangeFullMatch { get; set; }
        public List<QueryResult> exchangePartialMatch { get; set; }
    }
}