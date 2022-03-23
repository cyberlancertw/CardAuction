using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardAuction.ViewModels
{
    public class CExchangePostViewModel
    {
        public string fPostUserId { get; set; }
        public HttpPostedFileBase Photo0 { get; set; }
        public HttpPostedFileBase Photo1 { get; set; }
        public HttpPostedFileBase Photo2 { get; set; }
        public HttpPostedFileBase Photo3 { get; set; }
        public string fSort { get; set; }
        public string fItemName { get; set; }
        public string fItemDescription { get; set; }
        public string fItemLocation { get; set; }
        public string fItemLevel { get; set; }
        public string fHopeItemName { get; set; }
        public string fHopeItemLocation { get; set; }
        public DateTime fEndTimeDate { get; set; }
        public DateTime fEndTimeTime { get; set; }
        public int fClick { get; set; }
        public int fReport { get; set; }
        public bool fDelete { get; set; }

    }
}