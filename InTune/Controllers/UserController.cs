﻿using InTune.Domain;
using InTune.Logic;
using System;
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

        [Route("api/user/update")]
        [HttpPost]
        public HttpResponseMessage UpdateUser(User user)
        {
            try
            {
                var us = new UserService();
                us.UpdateUser(user);
                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot update the user", ex);
            }
        }

        [Route("api/user/id")]
        [HttpGet]
        public User GetUserById(int userId)
        {
            var us = new UserService();
            var result = us.ReadUserById(userId);
            return result;
        }

        [Route("api/user/signinid")]
        [HttpGet]
        public User GetUserByEmail(string signinid)
        {
            var us = new UserService();
            var result = us.ReadUserByEmail(signinid);
            return result;
        }

        [Route("api/user/resetpassword")]
        [HttpPost]
        public HttpResponseMessage ResetPassword(User user)
        {
            try
            {
                var os = new OtpService();
                os.VerifyEmailOtp(user.Email, user.SessionToken);

                var us = new UserService();
                us.ResetPassword(user);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException(
                    string.Format("{0}. Cannot reset password.", ex.Message), ex);
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
