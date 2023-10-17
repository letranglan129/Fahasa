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
    public class PublisherController : Controller
    {
        private BookManagementEntities db = new BookManagementEntities();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetPublishers()
        {
            return Json(db.Publishers.Select(x => new { x.Id, x.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(string values)
        {
            try
            {
                var publisher = new Publisher();
                JsonConvert.PopulateObject(values, publisher);

                if (!TryValidateModel(publisher))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                publisher = db.Publishers.Add(publisher);
                db.SaveChanges();

                return Json(new
                {
                    Id = publisher.Id,
                    Name = publisher.Name
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
                var publisher = db.Publishers.FirstOrDefault(o => o.Id == key);
                if (publisher == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                JsonConvert.PopulateObject(values, publisher);

                if (!TryValidateModel(publisher))
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
                var publisher = db.Publishers.FirstOrDefault(model => model.Id == key);

                if (publisher == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                db.Publishers.Remove(publisher);
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
