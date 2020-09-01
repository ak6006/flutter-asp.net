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
    public class TransController : ApiController
    {
        private Entities db = new Entities();
        private ApplicationDBContext AuthDB = new ApplicationDBContext();


        // route => api/trans/transdata 
        [HttpGet]
        public IHttpActionResult TransData(string Key)
        {
            var UserId = User.Identity.GetUserId();
            var UserData = AuthDB.Users.Where(u => u.Id == UserId).FirstOrDefault();
            var UserPhone = UserData.PhoneNumber;
            var result = db.SP_Sales_Order_Trans_Vin_Load(Key).ToList();

            if (result != null)
            {
                var Today = DateTime.Today;
                var TodayResult = result.Where(r => r.date.ToString() == Today.ToString());
                if(TodayResult!=null)
                    return Ok(TodayResult);
                else
                    return NotFound();
            }    
            else
                return NotFound();
        }
    }
}
