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
    public class UpdateUserTokenController : ApiController
    {
        private Entities db = new Entities();
        private ApplicationDBContext AuthDB = new ApplicationDBContext();

        //api/UpdateUserToken/Update
        [HttpGet]
        public IHttpActionResult Update(string DeviceToken)
        {
            var UserId = User.Identity.GetUserId();
            var UserData = AuthDB.Users.Where(u => u.Id == UserId).FirstOrDefault();
            UserData.DeviceToken = DeviceToken;
            AuthDB.SaveChanges();
            return Ok();
        }
    }
}
