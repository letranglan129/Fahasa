using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Fahasa.Models;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Drawing;
using static Fahasa.FilterConfig;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Fahasa.Ultis;
using System.Threading.Tasks;
//using CloudinaryDotNet;
//using CloudinaryDotNet.Actions;

namespace Fahasa.Areas.Admin.Controllers
{
    [RedirectingAction]
    public class DashBoardController : Controller
    {
        BookManagementEntities db = new BookManagementEntities();
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

        [HttpPost]
        public ActionResult GetSummaries(DateTime startDate, DateTime endDate)
        {
            try
            {
                var profit = db.Database.SqlQuery<decimal?>(@"SELECT SUM(CASE 
                       WHEN Discount.Type = 'VNĐ' THEN OrderDetail.TotalPrice - Discount.Value
                       WHEN Discount.Type = '%' THEN OrderDetail.TotalPrice - (OrderDetail.TotalPrice * Discount.Value / 100)
                       ELSE OrderDetail.TotalPrice
                        END)
                        FROM OrderDetail
                        JOIN [Order] ON OrderDetail.OrderId = [Order].Id
                        LEFT JOIN DiscountOrder ON [Order].Id = DiscountOrder.OrderId
                        LEFT JOIN Discount ON DiscountOrder.DiscountCode = Discount.Code
                        WHERE [Order].Status != N'Đã hủy' AND [Order].OrderDate BETWEEN '" + startDate.ToString("yyyy-MM-dd HH:mm:ss") +
                        @"' AND '" + endDate.ToString("yyyy-MM-dd HH:mm:ss") + @"'").Select(x => x).FirstOrDefault();
                var users = db.Orders
                                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status != "Đã hủy")
                                .Select(o => o.CustomerId)
                                .Distinct()
                                .Count();
                var orders = db.Orders
                                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status != "Đã hủy")
                .Count();
                profit = profit == null ? 0 : profit;
                return Json(new { profit, users, orders, stat = startDate.ToString("yyyy-MM-dd HH:mm:ss"), end = endDate.ToString("yyyy-MM-dd HH:mm:ss") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string CurPassword, string NewPassword, string ConfirmPassword)
        {
            try
            {
                var people = Session[name: "infoAdmin"] as Person;
                people = db.People.FirstOrDefault(x => x.Id == people.Id);
                if (NewPassword == ConfirmPassword && NewPassword.Length > 5 && Encryptor.MD5Hash(CurPassword) == people.Password)
                {
                    people.Password = Encryptor.MD5Hash(NewPassword);
                    db.SaveChanges();
                    return Json("Thay đổi mật khẩu thành công", JsonRequestBehavior.AllowGet);
                }

                return Json("Thay đổi mật khẩu thất bại", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public async Task<ActionResult> EditProfile()
        {
            try
            {
                string profileJson = "";
                var people = Session["infoAdmin"] as Person;
                if (people != null)
                {
                    people = db.People.FirstOrDefault(x => x.Id == people.Id);

                    var addressUltis = new AddressUltis();
                    var province = await addressUltis.GetProvinceByName(people.Province != string.Empty && people.Province != null ? people.Province : "");
                    var district = await addressUltis.GetDistrictByName(people.District != string.Empty && people.District != null ? people.District : "", province.ProvinceID);
                    var ward = await addressUltis.GetWardByName(people.Ward != string.Empty && people.Ward != null ? people.Ward : "", district.DistrictID);

                    profileJson = JsonConvert.SerializeObject(new
                    {
                        people.Avatar,
                        people.Name,
                        people.Username,
                        people.Gender,
                        people.Birth,
                        people.Email,
                        people.Phone,
                        people.Street,
                        people.Ward,
                        people.District,
                        people.Province,
                        ward,
                        district,
                        province,
                    });
                }
                ViewData["profileJson"] = profileJson;
                return View();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public string EditProfile(string values)
        {
            try
            {
                string profileJson = "";
                var people = Session[name: "infoAdmin"] as Person;
                if (people != null)
                {
                    people = db.People.FirstOrDefault(x => x.Id == people.Id);
                    JsonConvert.PopulateObject(values, people);
                    db.SaveChanges();

                    people = db.People.FirstOrDefault(x => x.Id == people.Id);
                    profileJson = JsonConvert.SerializeObject(new
                    {
                        people.Avatar,
                        people.Name,
                        people.Username,
                        people.Gender,
                        people.Birth,
                        people.Email,
                        people.Phone,
                        people.Street,
                        people.Ward,
                        people.District,
                        people.Province
                    });

                    return JsonConvert.SerializeObject(profileJson);
                }

                return JsonConvert.SerializeObject(null);
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                return JsonConvert.SerializeObject(null);
            }
        }
    }
}