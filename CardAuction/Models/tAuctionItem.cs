//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace CardAuction.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tAuctionItem
    {
        public int fItemId { get; set; }
        public string fItemName { get; set; }
        public string fItemDescription { get; set; }
        public string fSort { get; set; }
        public int fPosterUserId { get; set; }
        public Nullable<int> fTopBidUserId { get; set; }
        public int fMoneyNow { get; set; }
        public int fMoneyStep { get; set; }
        public int fMoneyStart { get; set; }
        public int fMoneyUpperBound { get; set; }
        public string fGrading { get; set; }
        public System.DateTime fCreateTime { get; set; }
        public System.DateTime fEndTime { get; set; }
        public string fPhoto0 { get; set; }
        public string fPhoto1 { get; set; }
        public string fPhoto2 { get; set; }
        public string fPhoto3 { get; set; }
        public int fTransPerson { get; set; }
        public int fTransSeven { get; set; }
        public int fTransFami { get; set; }
        public int fTransLogi { get; set; }
    }
}
