using System;
using System.Net;
using System.Net.Http;

namespace InTune.Logic
{
    public static class SmsService
    {
        const string _otpServerUri = @"https://control.msg91.com/api/";
        private static string otpServerUri
        {
            get
            {
                return _otpServerUri;
            }
        }

        const string _authenticationKey = @"155572AW9nGMav593a0eb9";
        private static string authenticationKey
        {
            get
            {
                return _authenticationKey;
            }
        }

        public static void SendOtp(string mobileNumber, string message, string senderId, string otp)
        {
            string sendOtpApiName = @"sendotp.php";
            string sendOtpApiParams = string.Format("?authkey={0}&mobile={1}&message={2}&sender={3}&otp={4}",
                                      authenticationKey, mobileNumber, message, senderId, otp);
            string sendOtpApiUriString = string.Format("{0}{1}", sendOtpApiName, sendOtpApiParams);
            var client = new HttpClient();
            client.BaseAddress = new Uri(otpServerUri);
            var response = client.GetAsync(sendOtpApiUriString).Result;
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(string.Format("Cannot send OTP. Error: {0}", response.ReasonPhrase));
        }
    }
}
