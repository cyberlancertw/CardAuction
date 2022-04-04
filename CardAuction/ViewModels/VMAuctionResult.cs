using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CardAuction.Models;

namespace CardAuction.ViewModels
{
    public class VMAuctionResult
    {
        public string postUserAccount { get; set; }
        public string winUserAccount { get; set; }
        public tAuctionResult result { get; set; }
        public IEnumerable<BidDetail> history { get; set; }
    }
    public class BidDetail
    {
        public string bidAccount { get; set; }
        public int bidMoney { get; set; }
        public DateTime bidTime { get; set; }

    }
}