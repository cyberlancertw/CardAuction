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
    
    public partial class tMember
    {
        public int fUserId { get; set; }
        public string fAccount { get; set; }
        public string fPassword { get; set; }
        public string fName { get; set; }
        public string fEmail { get; set; }
        public string fAddress { get; set; }
        public string fPhone { get; set; }
        public System.DateTime fBirthday { get; set; }
        public Nullable<bool> fSubscribe { get; set; }
        public Nullable<bool> fManager { get; set; }
        public Nullable<bool> fActive { get; set; }
        public Nullable<bool> fDelete { get; set; }
    }
}
