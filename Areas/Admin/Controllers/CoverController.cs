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
    public class CoverController : Controller
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
        public ActionResult GetCovers()
        {
            try
            {
                return Json(db.Covers.Select(x => new { x.Id, x.Name }).ToList(), JsonRequestBehavior.AllowGet);
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
                var cover = new Cover();
                JsonConvert.PopulateObject(values, cover);

                if (!TryValidateModel(cover))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                cover = db.Covers.Add(cover);
                db.SaveChanges();

                return Json(new
                {
                    Id = cover.Id,
                    Name = cover.Name
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
                var cover = db.Covers.FirstOrDefault(o => o.Id == key);
                if (cover == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                JsonConvert.PopulateObject(values, cover);

                if (!TryValidateModel(cover))
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
                var cover = db.Covers.FirstOrDefault(model => model.Id == key);

                if (cover == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                db.Covers.Remove(cover);
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
