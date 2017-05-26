using InTune.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InTune.Controllers
{
    public class PersonController : ApiController
    {
        public IEnumerable<Person> GetAllPersons()
        {
            return new Person[] 
            {
                new Person { Name="Ashok", Email="ashok.gudur@gmail.com" },
                new Person { Name="Siddu", Email="sidguduru@gmail.com" }
            };
        }
    }
}
