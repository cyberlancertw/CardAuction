using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardAuction.Models;

namespace CardAuction.Controllers
{
    public class AdminController : Controller
    {

        dbCardAuctionEntities db = new dbCardAuctionEntities();
        // GET: Admin
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult MemberManage()
        {
            var tMembers = db.tMember.OrderBy(m => m.fUserId).ToList();
            return View(tMembers);
        }
    }
}