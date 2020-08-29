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
