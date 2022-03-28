﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardAuction.Models
{
    public class QueryResult
    {
        public string fItemId { get; set; }
        public string fItemName { get; set; }
        public string fPhoto { get; set; }
        public int CommentCount { get; set; }
        public int fMoneyNow { get; set; }
        public DateTime fEndTime { get; set; }
        public int fBidCount { get; set; }
        public int fChangeCount { get; set; }
        public QueryResult()
        {
            CommentCount = 0;
            fMoneyNow = 0;
        }
    }
}