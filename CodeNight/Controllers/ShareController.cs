using CodeNight.BusinessLayer;
using CodeNight.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeNight.Models;
using System.IO;

namespace CodeNight.Controllers
{
    public class ShareController : Controller
    {
        ShareManager shareManager = new ShareManager();
        LikedManager likedManager = new LikedManager();
        // GET: Share
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Paylas(Share share)
        {

            if (ModelState.IsValid)
            {
                share.Owner = CurrentSession.User;
                share.CreatedDate = DateTime.Now;
                shareManager.Insert(share);
                return RedirectToAction("Index", "Home");
            }

            return View(share);
        }
        public ActionResult ImageUpload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImageUpload(Share share, HttpPostedFileBase image)
        {

            if (ModelState.IsValid)
            {
                //image Upload
                string fileName = Path.GetFileNameWithoutExtension(image.FileName);
                string extension = Path.GetExtension(image.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmsfff") + extension;
                share.ShareImagFileName = fileName;
                fileName = Path.Combine(Server.MapPath("~/images/"), fileName);
                image.SaveAs(fileName);
                
                share.Owner = CurrentSession.User;
                share.CreatedDate = DateTime.Now;

                shareManager.Insert(share);
                return RedirectToAction("Index", "Home");
            }



            return View(share);
        }

        public ActionResult VideoUpload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VideoUpload(Share share, HttpPostedFileBase video)
        {
            if (ModelState.IsValid)
            {
                //image Upload
                string fileName = Path.GetFileNameWithoutExtension(video.FileName);
                string extension = Path.GetExtension(video.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmsfff") + extension;
                share.ShareVideoFileName = fileName;
                fileName = Path.Combine(Server.MapPath("~/videos"), fileName);
                video.SaveAs(fileName);


                share.Owner = CurrentSession.User;
                share.CreatedDate = DateTime.Now;

                shareManager.Insert(share);
                return RedirectToAction("Index", "Home");
            }

            return View(share);

        }
        [HttpPost]
        public ActionResult GetLiked(int[] ids)
        {
            List<int> likedShareeIds = likedManager.List
                (x => x.LikedUser.Id == CurrentSession.User.Id && ids.Contains
                (x.Share.Id)).Select(x => x.Share.Id)
                .ToList();

            return Json(new { result = likedShareeIds });
        }
        [HttpPost]
        public ActionResult SetLikeState(int shareid, bool liked)
        {
            int res = 0;

            Liked like = likedManager.Find(x => x.Share.Id == shareid && x.LikedUser.Id == CurrentSession.User.Id);

            Share share = shareManager.Find(x => x.Id == shareid);

            if (like != null && liked == false)
            {
                res = likedManager.Delete(like);
            }
            else if (like == null && liked == true)
            {
                res = likedManager.Insert(new Liked()
                {
                    LikedUser = CurrentSession.User,
                    Share = share
                });
            }
            if (res > 0)
            {
                if (liked)
                {
                    share.LikeCount++;
                }
                else
                {
                    share.LikeCount--;
                }
                shareManager.Update(share);
                return Json(new { hasError = false, errorMessage = string.Empty, result = share.LikeCount });
            }
            return Json(new { hasError = true, errorMessage = "Beğenme işlemi gerçekleştirilemedi", result = share.LikeCount });
        }
    }
}