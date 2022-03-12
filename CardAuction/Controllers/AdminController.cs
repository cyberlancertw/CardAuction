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

        //Personnel Manage //PsnManage //人事管理
        public ActionResult PsnManage()
        {
            var Members = db.tMember.OrderBy(m => m.fUserId).ToList();
            return View(Members);
        }

        //PsnCreate //新增人員
        public ActionResult PsnCreate() {
            return View();
        }
        [HttpPost]
        public ActionResult PsnCreate(tMember member)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Error = false;
                var temp = db.tMember.Where(m => m.fAccount == member.fAccount)
                    .FirstOrDefault();
                if (temp != null)
                {
                    ViewBag.Error = true;
                    return View(member);
                }
                db.tMember.Add(member);
                db.SaveChanges();
                return RedirectToAction("PsnManage");
            }
            return View(member);
        }

        //PsnEdit //編輯人員
        public ActionResult PsnEdit(string fAccount)
        {
            var member = db.tMember
                .Where(m => m.fAccount == fAccount).FirstOrDefault();
            return View(member);
        }

        [HttpPost]
        public ActionResult PsnEdit(tMember member)
        {
            if (ModelState.IsValid)
            {                
                var temp = db.tMember
                    .Where(m => m.fAccount == member.fAccount)
                    .FirstOrDefault();
                temp.fPassword = member.fPassword;
                temp.fName = member.fName;
                temp.fEmail = member.fEmail;
                temp.fAddress = member.fAddress;
                temp.fPhone = member.fPhone;
                temp.fBirthday = member.fBirthday;
                temp.fSubscribe = member.fSubscribe;
                temp.fManager = member.fManager;
                temp.fActive = member.fActive;
                temp.fDelete = member.fDelete;
                db.SaveChanges();
                return RedirectToAction("PsnManage");
            }
            return View(member);
        }
        //PsnDelete //刪除人員
        public ActionResult PsnDelete(string fAccount)
        {
            var member = db.tMember
                .Where(m => m.fAccount == fAccount).FirstOrDefault();
            db.tMember.Remove(member);
            db.SaveChanges();
            return RedirectToAction("PsnManage");
        }



        //ProductsManage //ProdManage //商品管理
        public ActionResult ProdManage()
        {
            return View();
        }

        //AdvertiseManage //AdManage //廣告管理
        public ActionResult AdManage()
        {
            return View();
        }

        //DataAnalyze //DataAnalyze //數據分析
        public ActionResult DataAnalyze()
        {
            return View();

        }


    }
}
