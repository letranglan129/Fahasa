using Fahasa.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static Fahasa.FilterConfig;

namespace Fahasa.Areas.Admin.Controllers
{
    [RedirectingAction]
    public class SupplierController : Controller
    {
        // GET: Admin/Supplier
        public ActionResult Index()
        {
            return View();
        }

        private BookManagementEntities db = new BookManagementEntities();

        public ActionResult GetSuppliers()
        {
            return Json(db.Suppliers.Select(x => new { x.Id, x.Name, x.Phone, x.Address }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(string values)
        {
            try
            {
                var supplier = new Supplier();
                JsonConvert.PopulateObject(values, supplier);

                if (!TryValidateModel(supplier))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                supplier = db.Suppliers.Add(supplier);
                db.SaveChanges();

                return Json(new
                {
                    Id = supplier.Id,
                    Name = supplier.Name
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
                var supplier = db.Suppliers.FirstOrDefault(o => o.Id == key);
                if (supplier == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                JsonConvert.PopulateObject(values, supplier);

                if (!TryValidateModel(supplier))
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
                var supplier = db.Suppliers.FirstOrDefault(model => model.Id == key);

                if (supplier == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                db.Suppliers.Remove(supplier);
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