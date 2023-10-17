using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using Fahasa.Models;
using Microsoft.Ajax.Utilities;
using WebGrease.Css.Extensions;
using System.Dynamic;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using static Fahasa.FilterConfig;
using System.Net;

namespace Fahasa.Controllers
{
    [RefreshAction]
    public class HomeController : Controller
    {
        BookManagementEntities db = new BookManagementEntities();
        public ActionResult Index()
        {
            try
            {
                dynamic model = new ExpandoObject();
                model.Literatures = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Văn học") != null).Take(10).ToList();
                model.Stationeries = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Văn phòng phẩm - Dụng Cụ Học Sinh") != null).Take(10).ToList();
                model.ScienceTechnology = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Khoa học kỹ thuật") != null).Take(10).ToList();
                model.ReferenceBooks = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Giáo khoa - Tham khảo") != null).Take(10).ToList();
                model.SchoolSupplies = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Dụng cụ học sinh") != null).Take(10).ToList();
                model.Economic = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Kinh tế") != null).Take(10).ToList();
                model.ForeignBooks = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Foreign books") != null).Take(10).ToList();
                model.OtherLanguages = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Other languages") != null).Take(10).ToList();
                model.Toys = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Đồ Chơi") != null).Take(10).ToList();
                return View(model);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
    }
}