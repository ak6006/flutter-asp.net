using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using API.Dto;
using API.Models;

namespace API.Controllers
{
    public class customersController : ApiController
    {
        private Entities db = new Entities();

        // GET: api/customers
        public List<SP_Customer_To_DataGrid_Result> Getcustomers()
        {
            List<SP_Customer_To_DataGrid_Result> customers = db.SP_Customer_To_DataGrid().ToList();
            return customers;
        }

        // GET: api/customers/5
        [ResponseType(typeof(customer))]
        public async Task<IHttpActionResult> Getcustomer(int id)
        {
            customer customer = await db.customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // PUT: api/customers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putcustomer(int id, customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.Customers_id)
            {
                return BadRequest();
            }

            db.Entry(customer).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!customerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/customers
        [ResponseType(typeof(customer))]
        public async Task<IHttpActionResult> Postcustomer(customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.customers.Add(customer);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = customer.Customers_id }, customer);
        }

        // DELETE: api/customers/5
        [ResponseType(typeof(customer))]
        public async Task<IHttpActionResult> Deletecustomer(int id)
        {
            customer customer = await db.customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            db.customers.Remove(customer);
            await db.SaveChangesAsync();

            return Ok(customer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool customerExists(int id)
        {
            return db.customers.Count(e => e.Customers_id == id) > 0;
        }
    }
}