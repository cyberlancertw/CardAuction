using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CardAuction.Models;

namespace CardAuction.ViewModels
{
    public class CMemberMypageViewModel
    {
        public tMember MyAccount { get; set; }

        public List<tAuctionItem> MyAuctionFavorite { get; set; }

        public List<tExchangeItem> MyExchangeFavorite { get; set; }

        public List<tAuctionItem> myAuctionItem { get; set; }

        public List<tExchangeItem> myExchangeItem { get; set; }
        //public List<tItem> myAuctionItem { get;set }
    }
}