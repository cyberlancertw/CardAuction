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
        public List<QueryResult> fullMatch { get; set; }
        public List<QueryResult> partialMatch { get; set; }
    }
}