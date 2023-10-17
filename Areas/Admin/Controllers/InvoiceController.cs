using Fahasa.Models;
using Fahasa.Ultis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Fahasa.FilterConfig;
using System.Text.Json.Serialization;

namespace Fahasa.Areas.Admin.Controllers
{
    [RedirectingAction]
    public class InvoiceController : Controller
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
        public string GetOrders()
        {
            try
            {
                var orders = db.Orders.OrderByDescending(x => x.OrderDate).ToList();
                return JsonConvert.SerializeObject(orders.Select(x => new
                {
                    x.Id,
                    x.CustomerId,
                    x.OrderDate,
                    x.FeeShip,
                    x.Person.Name,
                    x.Person.Email,
                    x.Person.Phone,
                    x.ShipCode,
                    x.Status,
                    Address = new
                    {
                        x.Person.Street,
                        x.Person.Ward,
                        x.Person.District,
                        x.Person.Province,
                    },
                    Price = CartUltis.calcOrder(((long)x.OrderDetails.Sum(detail => detail.Price * detail.Amount)), x.Discounts.ToList(), x.FeeShip ?? 0),
                    Details = x.OrderDetails.Select(detail => new { detail.OrderId, detail.Product.Id, detail.Product.Name, detail.Product.ImageSrc, detail.Price, detail.Amount, detail.TotalPrice }).ToList(),
                    Discounts = x.Discounts.Select(discount => new { discount.Code, discount.Title, discount.Value, discount.Type, discount.DateExpired, discount.Amount, discount.DateStart, discount.ApplyField, discount.ConditionalOperator, discount.ConditionalPrice, discount.Content, discount.Used, discount.SubTitle, discount.LimitUsed }).ToList()
                }).ToList());
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return JsonConvert.SerializeObject(null);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int key, string Reason)
        {
            try
            {
                var order = db.Orders.First(x => x.Id == key);
                HttpClient client = new HttpClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                string data = JsonConvert.SerializeObject(new
                {
                    order_codes = new List<string> { order.ShipCode },
                });

                HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add("Token", Properties.Setting.Token_API_GHN.ToString());
                client.DefaultRequestHeaders.Add("ShopId", Properties.Setting.ShopID_API_GHN.ToString());
                HttpResponseMessage response = await client.PostAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/v2/switch-status/cancel", content);

                if (response.IsSuccessStatusCode)
                {
                    if (order.Person.Email != null && order.Person.Email != string.Empty)
                    {
                        var message = await MailUltis.SendMailGoogleSmtp(order.Person.Email, order.Person.Email, "Fahasa từ chối vận chuyển đơn hàng của bạn!", "Từ chối đơn hàng với lý do: " + Reason, Properties.Setting.Gmail, Properties.Setting.Password);
                    }

                    foreach (var item in order.OrderDetails)
                    {
                        var product = db.Products.First(x => x.Id == item.Product.Id);
                        product.Amount += item.Amount;
                    }

                    db.Orders.Remove(order);
                    db.SaveChanges();
                    return Json(new { key, Reason }, JsonRequestBehavior.AllowGet);
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
    }
}