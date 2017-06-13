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
            string sendOtpApiUri = @"sendotp.php";
            string sendOtpApiUriString = string.Format("{0}?authkey={1}&mobile={2}&message={3}&sender={4}&otp={5}",
                    sendOtpApiUri, authenticationKey, mobileNumber, message, senderId, otp);
            var client = new HttpClient();
            client.BaseAddress = new Uri(otpServerUri);
            var response = client.GetAsync(sendOtpApiUriString).Result;
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(string.Format("Cannot send OTP. Error: {0}", response.ReasonPhrase));
        }
    }
}
