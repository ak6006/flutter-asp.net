using API.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    [Authorize]
    public class PaymentController : ApiController
    {
        private Entities db = new Entities();
        private ApplicationDBContext AuthDB = new ApplicationDBContext();

        //api/Payment/get
        [HttpGet]
        public IHttpActionResult Get(DateTime beginDate , DateTime endDate , string ProductName)
        {
            var UserId = User.Identity.GetUserId();
            var UserData = AuthDB.Users.Where(u => u.Id == UserId).FirstOrDefault();
            var UserPhone = UserData.PhoneNumber;
            var result = db.SP_flutter_SumQuantity_Orders_Query(UserPhone,beginDate,endDate).ToList();
            if(ProductName == null)
            {
                if (result != null)
                    return Ok(result);
                else
                    return NotFound();
            }
            else
            {
                List<SP_flutter_SumQuantity_Orders_Query_Result> FilteredResult = new
                    List<SP_flutter_SumQuantity_Orders_Query_Result>();
                foreach (var item in result)
                {
                    if (item.productName == ProductName)
                    {
                        FilteredResult.Add(item);
                    }
                }
                if (FilteredResult != null)
                    return Ok(FilteredResult);
                else
                    return NotFound();
            }
        }
    }
}

