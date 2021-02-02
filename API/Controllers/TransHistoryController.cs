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
    public class TransHistoryController : ApiController
    {
        private Entities db = new Entities();
        private ApplicationDBContext AuthDB = new ApplicationDBContext();

        //api/TransHistory/data
        [HttpGet]
        public IHttpActionResult Data(DateTime Key)
        {
            //return Ok("ay test");
            var UserId = User.Identity.GetUserId();
            var UserData = AuthDB.Users.Where(u => u.Id == UserId).FirstOrDefault();
            var UserPhone = UserData.PhoneNumber;
            var result = db.SP_flutter_Customer_Orders_Query(UserPhone).ToList();

            if (result != null)
            {
                //var Today = DateTime.Today;
                
                var TodayResult = result.Where(r => r.date.Value.Date == Key.Date);
                if (TodayResult != null)
                    return Ok(TodayResult);
                else
                    return NotFound();
            }
            else
                return NotFound();
        }
    }
}
