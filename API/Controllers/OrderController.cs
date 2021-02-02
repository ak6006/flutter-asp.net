using API.Dto;
using API.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace API.Controllers
{
    [Authorize]
    public class OrderController : ApiController
    {
        private Entities db = new Entities();
        private ApplicationDBContext AuthDB = new ApplicationDBContext();

        [HttpPost]
        public IHttpActionResult Order(List<CustomerOrdersDto> Orders)
        {
            /// first Store Id in DB
            var StoreId = db.stores.FirstOrDefault().store_id;
            /// 
            /// Customer Id
            var UserId = User.Identity.GetUserId();
            var UserData = AuthDB.Users.Where(u => u.Id == UserId).FirstOrDefault();
            var UserPhone = UserData.PhoneNumber;
            var CustomerId = db.customers.Where(c => c.address.phone == UserPhone).FirstOrDefault().Customers_id;
            ////
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);//400
            }
            else
            {
                try
                {
                    foreach (var item in Orders)
                    {
                        if (item.OrderHasProductId == 0)
                        {
                            // Cars Ids to string
                            string CarIds = "";
                            foreach (var car in item.orderCars)
                            {
                                CarIds += car.VId + ",";
                            }
                            if (CarIds.Length > 1)
                            {
                                CarIds.TrimEnd(CarIds[CarIds.Length - 1]);
                            }
                            ///
                            // Add Order
                            db.SP_Flutter_Order_Add_New(CustomerId, StoreId, item.OrderDate, null, item.ProductId,
                            item.MeasureId, item.WieghtId, item.quantity, CarIds);
                        }
                        else
                        {
                            // Edit Order
                            string CarIds = "";
                            foreach (var car in item.orderCars)
                            {
                                CarIds += car.VId + ",";
                            }
                            if (CarIds.Length > 1)
                            {
                                CarIds.TrimEnd(CarIds[CarIds.Length - 1]);
                            }
                            db.SP_Flutter_Order_van_Update(item.OrderId, item.OrderHasProductId, item.OrderDate,
                                item.ProductId, item.WieghtId, item.MeasureId, item.quantity, CarIds);
                        }
                    }
                    return Ok("Done");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);//400
                }
            }
        }

        public IHttpActionResult Delete(int OrderId)
        {
            try
            {
                var OrderCars = db.transvehcile_has_order.Where(o=>o.order_order_id == OrderId).ToList();
                foreach (var item in OrderCars)
                {
                    db.transvehcile_has_order.Remove(item);
                }
                var OrderProducts = db.order_has_product.Where(p=>p.order_order_id == OrderId).ToList();
                foreach (var item in OrderProducts)
                {
                    db.order_has_product.Remove(item);
                }
                db.orders.Remove(db.orders.Find(OrderId));
                db.SaveChanges();
                return Ok("Deleted");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
