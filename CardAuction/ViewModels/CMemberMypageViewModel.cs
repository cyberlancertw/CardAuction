using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CardAuction.Models;
using PagedList.Mvc;
using PagedList;

namespace CardAuction.ViewModels
{
    public class CMemberMypageViewModel
    {
        public tMember MyAccount { get; set; }

        public IPagedList<tAuctionItem> MyAuctionFavorite { get; set; }

        public IPagedList<tExchangeItem> MyExchangeFavorite { get; set; }

        public IPagedList<tAuctionItem> myAuctionItem { get; set; }

        public IPagedList<tExchangeItem> myExchangeItem { get; set; }
        //public List<tItem> myAuctionItem { get;set }
    }
}