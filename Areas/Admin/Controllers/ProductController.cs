using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Drawing;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Diagnostics;
using Fahasa.Models;
using static Fahasa.FilterConfig;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Security.Policy;

namespace Fahasa.Areas.Admin.Controllers
{

    [RedirectingAction]
    public class ProductController : Controller
    {
        BookManagementEntities db = new BookManagementEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Load()
        {
            try
            {
                var products = db.Products.OrderByDescending(model => model.Id).Select(x => new
                {
                    Id = x.Id,
                    ImageSrc = x.ImageSrc,
                    Name = x.Name,
                    Price = x.Price,
                    SoonRelease = x.SoonRelease,
                    StockAvailable = x.StockAvailable,
                    SupplierId = x.SupplierId,
                    AuthorId = x.AuthorId,
                    CoverId = x.CoverId,
                    Desc = x.Desc,
                    Table = x.Table,
                    Amount = x.Amount,
                    Discount = x.Discount,
                    PublisherId = x.PublisherId,
                    Brand = x.Brand,
                    Origin = x.Origin,
                    Categories = x.Categories.Select(c => new { Id = c.Id, Name = c.Name }).ToList(),
                    Galleries = x.Galleries.Select(g => new { Source = g.Source }).ToList()
                });

                var result = Json(new { data = products }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet)
                ;
                result.MaxJsonLength = int.MaxValue;
                return result;
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
                var product = new Product();

                JsonConvert.PopulateObject(values, product);
                var categories = new List<Category>();
                product.StockAvailable = product.Amount > 0 ? "in_stock" : "out_of_stock";

                product.Categories.ToList().ForEach(c =>
                {
                    var category = db.Categories.FirstOrDefault(c2 => c2.Id == c.Id);
                    if (category != null) categories.Add(category);
                });

                product.Categories = categories;

                db.Products.Add(product);
                db.SaveChanges();

                return Json(new
                {
                    Id = product.Id,
                    Name = product.Name
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult SaveTable(int Id, string Table)
        {
            try
            {
                var product = db.Products.FirstOrDefault(x => x.Id == Id);
                product.Table = Table;
                db.SaveChanges();

                return Json(new
                {
                    Id = product.Id,
                    Name = product.Name
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult SaveDesc(int Id, string Desc)
        {
            try
            {
                var product = db.Products.FirstOrDefault(x => x.Id == Id);
                product.Desc = Desc;
                db.SaveChanges();

                return Json(new
                {
                    Id = product.Id,
                    Name = product.Name
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPut]
        public ActionResult Update(int key, string values)
        {
            try
            {
                var product = db.Products.First(p => p.Id == key);

                JsonConvert.PopulateObject(values, product);
                var categories = new List<Category>();

                product.Categories.ToList().ForEach(c =>
                {
                    var category = db.Categories.FirstOrDefault(c2 => c2.Id == c.Id);
                    if (category != null) categories.Add(category);
                });

                product.Categories = categories;

                db.SaveChanges();

                return Json(new
                {
                    Id = product.Id,
                    Name = product.Name
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int key)
        {
            try
            {
                var product = db.Products.First(p => p.Id == key);
                db.Products.Remove(product);
                db.SaveChanges();
                return Json(new
                {
                    Id = product.Id,
                    Name = product.Name
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult AddGallery(int Id, string Source)
        {
            try
            {
                Gallery gallery = new Gallery();
                gallery.ProductId = Id;
                gallery.Source = Source;

                db.Galleries.Add(gallery);
                db.SaveChanges();
                return Json(new
                {
                    Id = gallery.ProductId,
                    Name = gallery.Source
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult DeleteGallery(int Id, string Source)
        {
            try
            {
                CloudinaryBase.DeleteImage(Source);

                db.Galleries.Remove(db.Galleries.FirstOrDefault(g => g.ProductId == Id));
                db.SaveChanges();
                return Json(new
                {
                    Id = Id,
                    Name = Source
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
    }
}