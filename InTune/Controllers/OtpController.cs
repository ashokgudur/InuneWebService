using InTune.Domain;
using InTune.Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InTune.Controllers
{
    public class OtpController : ApiController
    {
        [Route("api/country/isdcodes")]
        [HttpGet]
        public List<Country> GetCountryIsdCodes()
        {
            try
            {
                return JsonConvert.DeserializeObject<List<Country>>(
                            File.ReadAllText("CountryISDCodes.json"));
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot send ISD codes", ex);
            }
        }

        [Route("api/country/isdcode/validate")]
        [HttpGet]
        public HttpResponseMessage ValidateCountryIsdCode(string isdCode)
        {
            try
            {
                var countries = JsonConvert.
                            DeserializeObject<List<Country>>(
                                File.ReadAllText("CountryISDCodes.json"));

                if (!countries.Exists(c => c.IsdCode == isdCode))
                    throw new ArgumentException("Invalid country ISD code");

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot validate ISD code", ex);
            }
        }

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
