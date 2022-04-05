using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardAuction.Models
{
    public class BidItem
    {        
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public int[] BidMoney { get; set; }
    }
}