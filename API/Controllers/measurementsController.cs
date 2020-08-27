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
using API.Models;

namespace API.Controllers
{
    public class measurementsController : ApiController
    {
        private Entities db = new Entities();

        // GET: api/measurements
        public List<SP_Measurement_To_DataGrid_Result> Getmeasurements()
        {
            var result = db.SP_Measurement_To_DataGrid().ToList();
            return result;
        }

        // GET: api/measurements/5
        [ResponseType(typeof(measurement))]
        public async Task<IHttpActionResult> Getmeasurement(int id)
        {
            measurement measurement = await db.measurements.FindAsync(id);
            if (measurement == null)
            {
                return NotFound();
            }

            return Ok(measurement);
        }

        // PUT: api/measurements/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putmeasurement(int id, measurement measurement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != measurement.measure_id)
            {
                return BadRequest();
            }

            db.Entry(measurement).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!measurementExists(id))
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

        // POST: api/measurements
        [ResponseType(typeof(measurement))]
        public async Task<IHttpActionResult> Postmeasurement(measurement measurement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.measurements.Add(measurement);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = measurement.measure_id }, measurement);
        }

        // DELETE: api/measurements/5
        [ResponseType(typeof(measurement))]
        public async Task<IHttpActionResult> Deletemeasurement(int id)
        {
            measurement measurement = await db.measurements.FindAsync(id);
            if (measurement == null)
            {
                return NotFound();
            }

            db.measurements.Remove(measurement);
            await db.SaveChangesAsync();

            return Ok(measurement);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool measurementExists(int id)
        {
            return db.measurements.Count(e => e.measure_id == id) > 0;
        }
    }
}