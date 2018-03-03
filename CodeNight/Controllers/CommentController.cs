using CodeNight.BusinessLayer;
using CodeNight.Entities;
using CodeNight.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CodeNight.Controllers
{
    public class CommentController : Controller
    {
        // GET: Comment
        private ShareManager shareManager = new ShareManager();
        private CommentManager commentManager = new CommentManager();

        public ActionResult ShowShareComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Share share = shareManager.ListQueryable().Include
                ("Comments").FirstOrDefault(x => x.Id == id);
            if (share == null)
            {
                HttpNotFound();
            }


            return PartialView("_PartialComments", share.Comments);
        }


        [HttpPost]
        public ActionResult Edit(int? id, string text)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comments comment = commentManager.Find(x => x.Id == id);
            if (comment == null)
            {
                return new HttpNotFoundResult();
            }
            comment.CreatedDate = DateTime.Now;
            comment.CommentText = text;

            if (commentManager.Update(comment) > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comments comment = commentManager.Find(x => x.Id == id);
            if (comment == null)
            {
                return new HttpNotFoundResult();
            }


            if (commentManager.Delete(comment) > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(Comments comment, int? shareid)
        {
            if (ModelState.IsValid)
            {
                ModelState.Remove("CreatedDate");
                


                if (shareid == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Share share = shareManager.Find(x => x.Id == shareid);


                if (share == null)
                {
                    return new HttpNotFoundResult();
                }
                comment.Share = share;
                comment.Owner = CurrentSession.User;
                comment.CreatedDate = DateTime.Now;
                if (commentManager.Insert(comment) > 0)
                {
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

    }
}
