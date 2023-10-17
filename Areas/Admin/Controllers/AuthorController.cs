using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Fahasa.Models;
using Newtonsoft.Json;
using static Fahasa.FilterConfig;

namespace Fahasa.Areas.Admin.Controllers
{
    [RedirectingAction]
    public class AuthorController : Controller
    {
        private BookManagementEntities db = new BookManagementEntities();

        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult GetAuthors()
        {
            try
            {
                return Json(db.Authors.Select(x => new { x.Id, x.Name }).ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        
        [HttpPost]
        public ActionResult Add(string values)
        {
            try
            {
                var author = new Author();
                JsonConvert.PopulateObject(values, author);

                if (!TryValidateModel(author))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                author = db.Authors.Add(author);
                db.SaveChanges();

                return Json(new
                {
                    Id = author.Id,
                    Name = author.Name
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPut]
        public ActionResult Update(int key, string values)
        {
            try
            {
                var author = db.Authors.FirstOrDefault(o => o.Id == key);
                if (author == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                JsonConvert.PopulateObject(values, author);

                if (!TryValidateModel(author))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                db.SaveChanges();
                return Json(new { code = 200, msg = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { code = 500, msg = "Cập nhật thất bại!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpDelete]
        public ActionResult Delete(int key)
        {
            try
            {
                var author = db.Authors.FirstOrDefault(model => model.Id == key);

                if (author == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                db.Authors.Remove(author);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Xóa thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { code = 500, msg = "Xóa thất bại! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
