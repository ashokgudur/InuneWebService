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
    public class UserController : ApiController
    {
        [Route("api/user/register")]
        [HttpPost]
        public HttpResponseMessage Register(User user)
        {
            try
            {
                var us = new UserService();
                us.Register(user);
                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot register this user", ex);
            }
        }

        [Route("api/user/userbyId")]
        [HttpGet]
        public User GetUserById(int userId)
        {
            var us = new UserService();
            var result = us.ReadUserById(userId);
            return result;
        }

        [Route("api/user/forgotpassword")]
        [HttpGet]
        public HttpResponseMessage Register(string email)
        {
            try
            {
                var us = new UserService();
                us.ForgotPassword(email);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException(
                    string.Format("{0}. Cannot send your password.", ex.Message), ex);
            }
        }

        [Route("api/user/signin")]
        [HttpPost]
        public HttpResponseMessage SignIn(User loginInfo)
        {
            try
            {
                var us = new UserService();
                var user = us.Login(loginInfo);
                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
        }
    }
}
