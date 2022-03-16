using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardAuction.ViewModels
{
    public class CAuctionPostViewModel
    {
        //public string fItemId { get; set; }
        public string fPostUserId { get; set; }
        public HttpPostedFileBase Photo0 { get; set; }
        public HttpPostedFileBase Photo1 { get; set; }
        public HttpPostedFileBase Photo2 { get; set; }
        public HttpPostedFileBase Photo3 { get; set; }
        public string fItemName { get; set; }
        public string fItemDescription { get; set; }
        public string fSort { get; set; }
        public int fMoneyStart { get; set; }
        public bool isBuy { get; set; }
        public int fBuyPrice { get; set; }
        public int fMoneyStep { get; set; }
        public string fGrading { get; set; }
        public DateTime fEndTimeDate { get; set; }
        public DateTime fEndTimeTime { get; set; }
        public bool isPerson { get; set; }
        public int fTransPerson { get; set; }
        public bool isSeven { get; set; }
        public int fTransSeven { get; set; }
        public bool isFami { get; set; }
        public int fTransFami { get; set; }
        public bool isLogi { get; set; }
        public int fTransLogi { get; set; }
        public int fClick { get; set; }
        public int fReport { get; set; }
        public bool fDelete { get; set; }


    }
}