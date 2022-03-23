using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardAuction.Models;
using System.IO;


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
   

        public ActionResult AdvManage()
        {
            //var datas = from Ad in db.tAdminAd select Ad;
            IEnumerable<tAdminAd> datas = null;
            string keyword = Request.Form["txtKeyword"];
            if (string.IsNullOrEmpty(keyword))
                datas = from Ad in db.tAdminAd select Ad;
            else
                datas = db.tAdminAd.Where(A => A.fAdName.Contains(keyword));
            return View(datas);
        }

        public ActionResult AdvCreate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdvCreate(tAdminAd Ad)
        {

            db.tAdminAd.Add(Ad);
            db.SaveChanges();
            return RedirectToAction("AdvManage");
        }

        public ActionResult AdvDelete(int? id)
        {
            if (id != null)
            {
                tAdminAd Adv = db.tAdminAd.FirstOrDefault(A => A.fAdId == (int)id);
                if (Adv != null)
                {
                    db.tAdminAd.Remove(Adv);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("AdvManage");
        }

        public ActionResult AdvEdit(int? id)
        {
            if (id != null)
            {
                tAdminAd Adv = db.tAdminAd.FirstOrDefault(A => A.fAdId == (int)id);
                if (Adv != null)
                {
                    return View(Adv);
                }
            }
            return RedirectToAction("AdvManage");
        }
        [HttpPost]
        public ActionResult AdvEdit(tAdminAd editAd)
        {
            tAdminAd Adv = db.tAdminAd.FirstOrDefault(A => A.fAdId == editAd.fAdId);
            if (Adv != null)
            {
                if (editAd.photo != null)
                {
                    //string name = Guid.NewGuid().ToString() + ".jpg";
                    string name = editAd.fAdId.ToString() + ".jpg";
                    editAd.photo.SaveAs(Server.MapPath("~/Images/User/Index/SlideShowAD/") + name);
                    Adv.fAdFileName = name;
                }
                Adv.fAdName = editAd.fAdName;
                Adv.fAdNote = editAd.fAdNote;
                db.SaveChanges();
            }
            return RedirectToAction("AdvManage");
        }








        public ActionResult AdManage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdManage(HttpPostedFileBase[] photos)
        {
            string fileName = "";
            // 使用for 迴圈取得所有上傳的檔案
            for (int i = 0; i < photos.Length; i++)
            {
                // 取得目前檔案上傳的HttpPostedFileBase物件
                // 即虛引數的photos[i]可以取得第i 個所上傳的檔案
                HttpPostedFileBase f = (HttpPostedFileBase)photos[i];
                // 若目前檔案上傳的HttpPostedFileBase物件的檔案名稱為不為空白
                // 即表示第 i 個f物件有指定上傳檔案
                if (f != null)
                {
                    //取得圖檔名稱
                    fileName = Path.GetFileName(f.FileName);
                    //將檔案儲存到網站的Photos資料夾下
                    var path = Path.Combine
                      (Server.MapPath("~/Images/User/Index/SlideShowAD"), fileName);
                    f.SaveAs(path);
                }
            }
            return RedirectToAction("ShowPhotos");
        }

        // ShowPhotos方法使用ContentResult傳回HTML
        // 可顯示Photos資料夾下所有圖檔
        public ContentResult ShowPhotos()
        {
            string strHtml = "";
            // 建立可操作Photos資料夾的dir物件
            DirectoryInfo dir =
                new DirectoryInfo(Server.MapPath("~/Images/User/Index/SlideShowAD"));
            //取得dir物件下的所有檔案(即photos資料夾下)並放入finfo檔案資訊陣列
            FileInfo[] fInfo = dir.GetFiles();
            // 逐一將finfo檔案資訊陣列內的所有圖檔指定給strHtml變數
            foreach (FileInfo result in fInfo)
            {
                // 將顯示圖的HTML字串指定給strHtml
                strHtml += $"<a href='../Images/User/Index/SlideShowAD/{result.Name}' target='_blank'>" +
                    $"<img src='../Images/User/Index/SlideShowAD/{result.Name}' width='150' height='120' border='0'>" +
                    $"</a>　";
            }
            // strHtml變數再加上 '返回' Create動作方法的連結
            strHtml += "<p><a href='/Admin/AdManage'>返回</a></p>";
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }













        //DataAnalyze //DataAnalyze //數據分析
        public ActionResult DataAnalyze()
        {
            return View();

        }


    }
}
