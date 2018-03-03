using CodeNight.BusinessLayer;
using CodeNight.DataAccessLayer;
using CodeNight.Entities.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Data.Entity;
using CodeNight.Filters;

namespace CodeNight.Controllers
{
    public class HomeController : Controller
    {
        ShareManager shareManager = new ShareManager();
        DatabaseContext db = new DatabaseContext();
        [Auth]
        public ActionResult Index()
        {
            //Test a = new Test();
            var shares = shareManager.ListQueryable().Include("Owner").OrderByDescending(x=>x.CreatedDate).ToList();

            return View(shares);
            
        }

        //public ActionResult Test()
        //{
        //    //Test a = new Test();
        //    //var shares = db.Shares.Include("Owner")
        //    //    .OrderByDescending(x => x.CreatedDate).ToList();

        //    //return View(shares);
        //    return View();
        //}
        //[HttpPost]
        //public async System.Threading.Tasks.Task<ActionResult> Test(RegisterViewModel cc)
        //{
        //    using (HttpClient x = new HttpClient())
        //    {
        //        x.BaseAddress = new Uri("http://localhost:61741/api/");
        //        var myContent = JsonConvert.SerializeObject(cc);
        //        HttpResponseMessage mymessage = await x.PostAsync("User",new StringContent(myContent,Encoding.UTF8,"application/json"));

        //    }
        //    //Test a = new Test();
        //    //var shares = db.Shares.Include("Owner")
        //    //    .OrderByDescending(x => x.CreatedDate).ToList();

        //    //return View(shares);
        //    return View();
        //}


    }
}