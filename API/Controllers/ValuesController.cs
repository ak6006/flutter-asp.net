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
                        DriverName = item.transVehcile_driver_name
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
                             select new
                             {
                                 OrderId = o.order_id,
                                 OrderDate = o.date,
                                 ProductId = p.product_product_id,
                                 ProductName = p.product.productName,
                                 WieghtId = p.weight_weight_id,
                                 WieghtName = p.weight.weight_net,
                                 MeasureId = p.measurement_measure_id,
                                 MeasureName = p.measurement.measre_name,
                                 quantity = p.Quantity
                             };
                foreach (var item in result)
                {
                    CustomerOrdersDto item2 = new CustomerOrdersDto()
                    {
                        MeasureId = item.MeasureId,
                        MeasureName = item.MeasureName,
                        OrderId = item.OrderId,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        WieghtId = item.WieghtId,
                        WieghtName = item.WieghtName,
                        OrderDate = item.OrderDate,
                        quantity = item.quantity
                    };
                    customerOrders.Add(item2);
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
                    productName = item.productName
                };
                ProdNames.Add(PNames);
            }
            ProductName PName = new ProductName()
            {
                productName = "كل المنتجات"
            };
            ProdNames.Add(PName);

            if (ProdNames != null)
                return Ok(new { customerOrders, CustName, ProdNames, storeNames, weightNames, measureNames, vehiclesData });
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

        // GET api/values/5
        //public string Get(int id)
        //{

        //    return "value";
        //}

        // POST api/values
        public void Post([FromBody] string value)
        {

        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
