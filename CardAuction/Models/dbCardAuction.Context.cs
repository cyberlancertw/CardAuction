﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class dbCardAuctionEntities : DbContext
    {
        public dbCardAuctionEntities()
            : base("name=dbCardAuctionEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tAdminAd> tAdminAd { get; set; }
        public virtual DbSet<tAuctionBid> tAuctionBid { get; set; }
        public virtual DbSet<tAuctionFavorite> tAuctionFavorite { get; set; }
        public virtual DbSet<tAuctionItem> tAuctionItem { get; set; }
        public virtual DbSet<tCommentAuction> tCommentAuction { get; set; }
        public virtual DbSet<tExchangeFavorite> tExchangeFavorite { get; set; }
        public virtual DbSet<tExchangeItem> tExchangeItem { get; set; }
        public virtual DbSet<tMember> tMember { get; set; }
    }
}
