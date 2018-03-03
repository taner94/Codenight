using CodeNight.Entities;
using CodeNight.BusinessLayer;
using CodeNight.BusinessLayer.Result;
using CodeNight.Entities.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeNight.Models;
using System.Data.Entity;
using CodeNight.ViewModels;

namespace CodeNight.Controllers
{
  
    public class AccountController : Controller
    {
        UserManager userManager = new UserManager();
        ShareManager shareManager = new ShareManager();
        // GET: Account
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                BusinessLayerResult<User> res = userManager.UserRegister(model);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(model);
                }


               

                OkVM notifyObj = new OkVM()
                {
                    Title = "Kayıt Başarılı",
                    RedirectingUrl = "/Account/Login",
                };

                notifyObj.Items.Add("Lütfen e-posta adresinize gönderdiğimiz aktivasyon link'ine tıklayarak hesabınızı aktive ediniz. Hesabınızı aktive etmeden not ekleyemez ve beğenme yapamazsınız.");

                return View("Ok", notifyObj);
            }

            return View(model);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                BusinessLayerResult<User> res = userManager.LoginUser(model);

                if (res.Errors.Count > 0)
                {
                   
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));

                    return View(model);
                }

                CurrentSession.Set<User>("login", res.Result); // Session'a kullanıcı bilgi saklama..
                return RedirectToAction("Index","Home");   // yönlendirme..
            }

            return View(model);
        }
        public ActionResult EditProfile()
        {
            BusinessLayerResult<User> res = userManager.GetUserById(CurrentSession.User.Id);
            return View(res.Result);
        }
        [HttpPost]
        public ActionResult EditProfile(User model, HttpPostedFileBase ProfileImage)
        {
            if (ModelState.IsValid)
            {
                if (ProfileImage != null &&
                    (ProfileImage.ContentType == "image/jpeg" ||
                    ProfileImage.ContentType == "image/jpg" ||
                    ProfileImage.ContentType == "image/png"))
                {
                    string filename = $"user_{model.Id}.{ProfileImage.ContentType.Split('/')[1]}";

                    ProfileImage.SaveAs(Server.MapPath($"~/images/{filename}"));
                    model.ProfileImageFileName = filename;
                }

                BusinessLayerResult<User> res = userManager.UpdateUserProfile(model);

                //if (res.Errors.Count > 0)
                //{
                //    ErrorViewModel errorNotifyObj = new ErrorViewModel()
                //    {
                //        Items = res.Errors,
                //        Title = "Profil Güncellenemedi.",
                //        RedirectingUrl = "/Home/EditProfile"
                //    };

                //    return View("Error", errorNotifyObj);
                //}

                // Profil güncellendiği için session güncellendi.
                CurrentSession.Set<User>("login", res.Result);

                return RedirectToAction("Index","Home");
            }

            return View(model);
            
        }
        public ActionResult ShowProfile(int? id)
        {
            var shares = shareManager.ListQueryable().Include("Owner").Where(
            x => x.Owner.Id == id).OrderByDescending(
            x => x.CreatedDate);

            return View(shares.ToList());
            
        }
        public ActionResult MyProfile()
        {
            var shares = shareManager.ListQueryable().Include("Owner").Where(
            x => x.Owner.Id == CurrentSession.User.Id).OrderByDescending(
            x => x.CreatedDate);

            return View(shares.ToList());
        }
        public ActionResult FootballerActivate(Guid id)
        {
            BusinessLayerResult<User> res = userManager.ActivateUser(id);

            if (res.Errors.Count > 0)
            {
                ErrorVM errorNotifyObj = new ErrorVM()
                {
                    Title = "Geçersiz İşlem",
                    Items = res.Errors
                };

                return View("Error", errorNotifyObj);
            }

            OkVM okNotifyObj = new OkVM()
            {
                Title = "Hesap Aktifleştirildi",
                RedirectingUrl = "/Home/FootballerLogin"
            };

            okNotifyObj.Items.Add("Hesabınız aktifleştirildi. Artık paylaşım yapabilirsiniz.");

            return View("Ok", okNotifyObj);
        }
        public ActionResult DeleteFootballerProfile()
        {
            BusinessLayerResult<User> res = userManager.RemoveUserById(CurrentSession.User.Id);
            if (res.Errors.Count > 0)
            {
                ErrorVM errorNotifyObj = new ErrorVM()
                {
                    Items = res.Errors,
                    Title = "Profil Silinemedi",
                    RedirectingUrl = "/Home/FootballerProfileInformations"
                };
                return View("Error", errorNotifyObj);
            }
            Session.Clear();
            return RedirectToAction("Index");
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index","Home");
        }
    }
}