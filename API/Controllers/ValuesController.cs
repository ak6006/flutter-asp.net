using API.Dto;
using API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class ValuesController : ApiController
    {

        private Entities db = new Entities();

        [HttpGet]
        public List<SP_Sales_BarCode_Check_Result> Data(string Key)
        {
            ObjectParameter RecFound = new ObjectParameter("rec_found", typeof(int));
            var result = db.SP_Sales_BarCode_Check(Key, RecFound).ToList();
            return result;
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
