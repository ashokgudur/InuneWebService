using InTune.Logic;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InTune.Controllers
{
    public class OtpController : ApiController
    {
        [Route("api/email/otp/send")]
        [HttpGet]
        public HttpResponseMessage SendEmailOtp(string emailAddress)
        {
            try
            {
                var os = new OtpService();
                os.SendEmailOtp(emailAddress);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot send OTP", ex);
            }
        }

        [Route("api/email/otp/verify")]
        [HttpGet]
        public HttpResponseMessage VerifyEmailOtp(string emailAddress, string otp)
        {
            try
            {
                var os = new OtpService();
                os.VerifyEmailOtp(emailAddress, otp);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot Verify OTP", ex);
            }
        }

        [Route("api/mobile/otp/send")]
        [HttpGet]
        public HttpResponseMessage SendMobileOtp(string isdCode, string mobileNumber)
        {
            try
            {
                var os = new OtpService();
                os.SendMobileOtp(isdCode, mobileNumber);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot send OTP", ex);
            }
        }

        [Route("api/mobile/otp/verify")]
        [HttpGet]
        public HttpResponseMessage VerifyMobileOtp(string isdCode, string mobileNumber, string otp)
        {
            try
            {
                var os = new OtpService();
                os.VerifyMobileOtp(isdCode, mobileNumber, otp);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot Verify OTP", ex);
            }
        }
    }
}
