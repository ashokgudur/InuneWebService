using InTune.Data;
using InTune.Domain;
using InTune.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InTune.Controllers
{
    public class ContactController : ApiController
    {
        [Route("api/contact/create")]
        [HttpPost]
        public HttpResponseMessage AddContact(Contact contact)
        {
            try
            {
                var cs = new ContactService();
                cs.AddContact(contact);
                return Request.CreateResponse(HttpStatusCode.OK, contact);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot add this contact", ex);
            }
        }

        [Route("api/contact/update")]
        [HttpPost]
        public HttpResponseMessage UpdateContact(Contact contact)
        {
            try
            {
                var cs = new ContactService();
                cs.UpdateContact(contact);
                return Request.CreateResponse(HttpStatusCode.OK, contact);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot update this contact", ex);
            }
        }

        [Route("api/contact/contact")]
        [HttpGet]
        public Contact GetContact(int contactId)
        {
            var cs = new ContactService();
            var result = cs.ReadContact(contactId);
            return result;
        }

        [Route("api/contact/allcontacts")]
        [HttpGet]
        public IEnumerable<Contact> GetAllContacts(int userId)
        {
            var cs = new ContactService();
            var result = cs.ReadAllContacts(userId);
            return result;
        }
    }
}
