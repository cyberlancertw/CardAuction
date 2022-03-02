﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardAuction.Models;

namespace CardAuction.Controllers
{
    public class AuctionController : Controller
    {
        // GET: Auction
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Post()
        {
            if(Session[CDictionary.SK_User] == null)
            {
                return RedirectToAction("Login", "Member");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Post(string name)       // use view model
        {
            return View();
        }
    }
}