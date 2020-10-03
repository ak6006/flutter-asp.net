using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using API.Dto;
using API.Models;
using Microsoft.AspNet.Identity;

namespace API.Controllers
{
    [Authorize]
    public class CarsController : ApiController
    {
        private Entities db = new Entities();
        private ApplicationDBContext AuthDB = new ApplicationDBContext();


        // GET: api/Cars
        public IQueryable<transvehcile> GetCars()
        {
            return db.transvehciles;
        }

        // GET: api/Cars/5
        [ResponseType(typeof(transvehcile))]
        public async Task<IHttpActionResult> GetCar(int id)
        {
            transvehcile transvehcile = await db.transvehciles.FindAsync(id);
            if (transvehcile == null)
            {
                return NotFound();
            }

            return Ok(transvehcile);
        }

        // PUT: api/Cars/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCar(VehiclesData transvehcile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var UserId = User.Identity.GetUserId();
            var UserData = AuthDB.Users.Where(u => u.Id == UserId).FirstOrDefault();
            var UserPhone = UserData.PhoneNumber;
            var CustomerId = db.customers.Where(c => c.address.phone == UserPhone).FirstOrDefault().Customers_id;
            try
            {
                ObjectParameter RecFound = new ObjectParameter("rec_found", typeof(int));
                db.SP_Trans_vin_Update(transvehcile.VehicleId, transvehcile.DriverName, transvehcile.Number,
                    transvehcile.Model, transvehcile.Phone, CustomerId, "",
                    transvehcile.Serial, RecFound);
                await db.SaveChangesAsync();
                return Ok("تم التعديل");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Cars
        [ResponseType(typeof(transvehcile))]
        public async Task<IHttpActionResult> PostCar (VehiclesData NewCar)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var UserId = User.Identity.GetUserId();
            var UserData = AuthDB.Users.Where(u => u.Id == UserId).FirstOrDefault();
            var UserPhone = UserData.PhoneNumber;
            var CustomerId = db.customers.Where(c => c.address.phone == UserPhone).FirstOrDefault().Customers_id;

            ObjectParameter RecFound = new ObjectParameter("rec_found", typeof(int));
            ObjectParameter NewIdentity = new ObjectParameter("new_identity", typeof(int));
            db.SP_Trans_Vin_Add_New(NewCar.DriverName, NewCar.Number, NewCar.Model, NewCar.Phone, CustomerId,
                "", NewCar.Serial, NewIdentity, RecFound).ToList();
            await db.SaveChangesAsync();
            if ((int)RecFound.Value == 0)
            {
                
                return Ok("تمت الاضافه بنجاح");
            }
            else
            {
                return BadRequest("رقم العربية موجود بالفعل");
            }
            
        }

        // DELETE: api/Cars/5
        [ResponseType(typeof(transvehcile))]
        public async Task<IHttpActionResult> DeleteCar(int id)
        {
            try
            {
                db.SP_Trans_Vin_DELETE(id);
                await db.SaveChangesAsync();
                return Ok("تم الحذف");
            }
            catch
            {
                return BadRequest("لم يتم الحذف");
            }
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool transvehcileExists(int id)
        {
            return db.transvehciles.Count(e => e.v_id == id) > 0;
        }
    }
}