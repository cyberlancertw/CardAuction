using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardAuction.Models
{
    public class CAuctionItem
    {
        public string fItemName { get; set; }
        public string fItemDescription { get; set; }
        public string fSort { get; set; }
        public int fPosterUserId { get; set; }
        public int fTopBidUserId { get; set; }

    }
}