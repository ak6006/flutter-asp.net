using API.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace API.Controllers
{
    public class NotiController : ApiController
    {
        private Entities db = new Entities();
        private ApplicationDBContext AuthDB = new ApplicationDBContext();

        //api/noti/data
        [HttpPost]
        public IHttpActionResult Data(NotificationViewModel input)
        {
            var deviceId = AuthDB.Users.Where(u => u.PhoneNumber == input.CustomerPhone).FirstOrDefault().DeviceToken;
            try
            {
                PushNotification p = new PushNotification(input, deviceId);
                return Ok();
            }
            catch
            {
                return NotFound();
            }

        }
    }
}

