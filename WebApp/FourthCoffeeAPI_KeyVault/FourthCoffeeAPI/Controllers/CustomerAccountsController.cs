using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using FourthCoffeeAPI;
using System.Data.Entity.Core.EntityClient;

namespace FourthCoffeeAPI.Controllers
{
    public class CustomerAccountsController : ApiController
    {
        private static FourthCoffeeEntities db;

        static CustomerAccountsController()
        {
            const string metaData = "res://*/FourthCoffee.csdl|res://*/FourthCoffee.ssdl|res://*/FourthCoffee.msl";
            const string appName = "EntityFramework";
            const string providerName = "System.Data.SqlClient";

            EntityConnectionStringBuilder efBuilder = new EntityConnectionStringBuilder();
            efBuilder.Metadata = metaData;
            efBuilder.Provider = providerName;
            efBuilder.ProviderConnectionString = Util.EncryptSecret;

            db = new FourthCoffeeEntities(efBuilder.ConnectionString);
        }

        // GET: api/CustomerAccounts
        public IQueryable<CustomerAccount> GetCustomerAccounts()
        {            
            return db.CustomerAccounts;
        }

        // GET: api/CustomerAccounts/5
        [ResponseType(typeof(CustomerAccount))]
        public IHttpActionResult GetCustomerAccount(Guid id)
        {
            CustomerAccount customerAccount = db.CustomerAccounts.Find(id);
            if (customerAccount == null)
            {
                return NotFound();
            }

            return Ok(customerAccount);
        }

        // PUT: api/CustomerAccounts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCustomerAccount(Guid id, CustomerAccount customerAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customerAccount.CustomerAccountId)
            {
                return BadRequest();
            }

            db.Entry(customerAccount).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerAccountExists(id))
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

        // POST: api/CustomerAccounts
        [ResponseType(typeof(CustomerAccount))]
        public IHttpActionResult PostCustomerAccount(CustomerAccount customerAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CustomerAccounts.Add(customerAccount);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CustomerAccountExists(customerAccount.CustomerAccountId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = customerAccount.CustomerAccountId }, customerAccount);
        }

        // DELETE: api/CustomerAccounts/5
        [ResponseType(typeof(CustomerAccount))]
        public IHttpActionResult DeleteCustomerAccount(Guid id)
        {
            CustomerAccount customerAccount = db.CustomerAccounts.Find(id);
            if (customerAccount == null)
            {
                return NotFound();
            }

            db.CustomerAccounts.Remove(customerAccount);
            db.SaveChanges();

            return Ok(customerAccount);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerAccountExists(Guid id)
        {
            return db.CustomerAccounts.Count(e => e.CustomerAccountId == id) > 0;
        }
    }
}