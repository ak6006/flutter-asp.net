using API.Dto;
using API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using System.Reflection.Emit;
using System.Reflection;
using System.Data.Entity;

namespace API.Controllers
{
    [Authorize]
    public class ValuesController : ApiController
    {
        private Entities db = new Entities();
        private ApplicationDBContext AuthDB = new ApplicationDBContext();


        // route api/Values/Data
        [HttpGet]
        public IHttpActionResult Data(string Key)
        {
            var UserId = User.Identity.GetUserId();
            var UserData = AuthDB.Users.Where(u => u.Id == UserId).FirstOrDefault();
            var UserPhone = UserData.PhoneNumber;
            ObjectParameter RecFound = new ObjectParameter("rec_found", typeof(int));

            var result = db.SP_Sales_BarCode_Check(Key, RecFound).SingleOrDefault();
            if (result != null)
                return Ok(result);
            else
                return NotFound();
        }


        // route api/Values/products
        [HttpGet]
        public IHttpActionResult Products()
        {
            //// Customer name
            var UserId = User.Identity.GetUserId();
            var UserData = AuthDB.Users.Where(u => u.Id == UserId).FirstOrDefault();
            var UserPhone = UserData.PhoneNumber;
            CustomerName CustName = new CustomerName();
            try
            {
                var CName = db.addresses.Where(c => c.phone == UserPhone).FirstOrDefault().firstName;
                CustName.custName = CName;
            }
            catch
            {
            }
            //// customer cars
            List<VehiclesData> vehiclesData = new List<VehiclesData>();
            try
            {
                var CustomerAddressId = db.addresses.Where(c => c.phone == UserPhone).FirstOrDefault().add_id;
                var CustomerId = db.customers.Where(c => c.address_add_id == CustomerAddressId).FirstOrDefault().Customers_id;
                var CustomerCars = db.transvehciles.Where(c => c.customers_Customers_id == CustomerId).ToList();
                foreach (var item in CustomerCars)
                {
                    VehiclesData VData = new VehiclesData()
                    {
                        VehicleId = item.v_id,
                        DriverName = item.transVehcile_driver_name,
                        Model = item.transVehcile_model,
                        Number = item.transVehcile_num,
                        Phone = item.transVehcile_phone,
                        Serial = item.transVehcile_serial
                    };
                    vehiclesData.Add(VData);
                }
            }
            catch
            {

            }
            //// Customer Orders
            List<CustomerOrdersDto> customerOrders = new List<CustomerOrdersDto>();
            try
            {
                var CustomerAddressId = db.addresses.Where(c => c.phone == UserPhone).FirstOrDefault().add_id;
                var CustomerId = db.customers.Where(c => c.address_add_id == CustomerAddressId).FirstOrDefault().Customers_id;
                //var COrders = db.orders.Where(o => o.customers_Customers_id == CustomerId);
                var result = from o in db.orders
                             where (o.customers_Customers_id == CustomerId)
                             join p in db.order_has_product on o.order_id equals p.order_order_id
                             where o.order_state == 0
                             select new CustomerOrdersDto
                             {
                                 OrderId = o.order_id,
                                 OrderDate = o.date,
                                 ProductId = p.product_product_id,
                                 ProductName = p.product.productName,
                                 WieghtId = p.weight_weight_id,
                                 WieghtName = p.weight.weight_net,
                                 MeasureId = p.measurement_measure_id,
                                 MeasureName = p.measurement.measre_name,
                                 quantity = p.Quantity,
                                 OrderHasProductId = p.order_has_product_Id
                             };

                foreach (var item in result)
                {
                    try
                    {
                        var CarResult = from t in db.transvehcile_has_order
                                        where (t.order_order_id == item.OrderId)
                                        join v in db.transvehciles on t.transVehcile_v_id equals v.v_id
                                        select new OrderCars
                                        {
                                            VId = t.transVehcile_v_id,
                                            DriverName = v.transVehcile_driver_name
                                        };
                        foreach (var CarItem in CarResult)
                        {
                            item.orderCars.Add(CarItem);
                        }
                    }
                    catch
                    {

                    }

                    customerOrders.Add(item);
                }
            }
            catch
            {

            }
            //// measuremnts 
            var Measures = db.measurements.ToList();
            List<MeasureName> measureNames = new List<MeasureName>();
            foreach (var item in Measures)
            {
                MeasureName MNames = new MeasureName()
                {
                    MeasureId = item.measure_id,
                    measureName = item.measre_name
                };
                measureNames.Add(MNames);
            }
            //// Weights 
            var weights = db.weights.ToList();
            List<WeightName> weightNames = new List<WeightName>();
            foreach (var item in weights)
            {
                WeightName WNames = new WeightName()
                {
                    WeightId = item.weight_id,
                    weightName = item.weight_net
                };
                weightNames.Add(WNames);
            }
            //// Stores
            var stores = db.stores.ToList();
            List<StoreName> storeNames = new List<StoreName>();
            foreach (var item in stores)
            {
                StoreName SNames = new StoreName()
                {
                    StoreId = item.store_id,
                    storeName = item.storeName
                };
                storeNames.Add(SNames);
            }
            //// Products + كل المنتجات 
            var products = db.products.ToList();
            List<ProductName> ProdNames = new List<ProductName>();
            foreach (var item in products)
            {
                ProductName PNames = new ProductName()
                {
                    ProductId = item.product_id,
                    productName = item.productName,
                    Price = item.TodayPrice,
                    PriceUpdateTime = item.PriceUpdateTime
                };
                ProdNames.Add(PNames);
            }
            ProductName PName = new ProductName()
            {
                productName = "كل المنتجات"
            };
            ///////////////
            ///Gifts
            //// Products + كل المنتجات 
            var gifts = db.gifts.ToList();

            ///
            ProdNames.Add(PName);

            if (ProdNames != null)
                return Ok(new { gifts, customerOrders, CustName, ProdNames, storeNames, weightNames, measureNames, vehiclesData });
            else
                return NotFound();
        }

        // route api/Values/CustomerName
        [Route("api/Values/CustomerName")]
        [HttpGet]
        public IHttpActionResult CustomerName()
        {
            var UserId = User.Identity.GetUserId();
            var UserData = AuthDB.Users.Where(u => u.Id == UserId).FirstOrDefault();
            var UserPhone = UserData.PhoneNumber;
            try
            {
                var CustomerName = db.addresses.Where(c => c.phone == UserPhone).FirstOrDefault().firstName;

                if (CustomerName != null)
                    return Ok(new { CustomerName = CustomerName });
                else
                    return NotFound();
            }
            catch
            {
                return NotFound();
            }

        }

        // route api/Values/Query
        [Route("api/Values/Query")]
        [HttpGet]
        public string Query(string query)
        {
            string connectionString = "Server=SQL5080.site4now.net;Database=DB_A67616_makahighfeed;User Id=DB_A67616_makahighfeed_admin;Password=ak654321;";

            var result = db.Database.ExecuteSqlCommand(query).ToString();
            return result;
        }



    }
}
