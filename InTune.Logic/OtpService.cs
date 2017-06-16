using System;
using InTune.Data;
using InTune.Domain;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace InTune.Logic
{
    public enum OtpStyle
    {
        Alpha,
        Numeric,
        AlphaNumeric
    }

    public class OtpService
    {
        public void SendEmailOtp(string emailAddress)
        {
            var email = new EmailAddress(emailAddress);

            using (DbContext dbc = new DbContext())
            {
                var otpNumber = generateOtp(OtpStyle.Numeric);

                try
                {
                    Emailer.SendMail(createOtpEmailMessage(emailAddress, otpNumber));
                }
                catch (Exception ex)
                {
                    logError(dbc, ex.ToString());
                    throw ex;
                }

                var dao = new OtpDao(dbc);
                dao.SendEmailOtp(email.Address, otpNumber);
            }
        }

        public void ValidateCountryIsdCode(string isdCode)
        {
            var countries = JsonConvert.
                            DeserializeObject<List<Country>>(
                                File.ReadAllText("CountryISDCodes.json"));

            isdCode = isdCode.StartsWith("+") ? isdCode : $"+{isdCode}";

            if (!countries.Exists(c => c.IsdCode == isdCode))
                throw new ArgumentException("Invalid country ISD code");
        }

        public List<Country> GetCountryIsdCodes()
        {
            return JsonConvert.DeserializeObject<List<Country>>(
            File.ReadAllText("CountryISDCodes.json"));
        }

        private void logError(DbContext dbc, string message)
        {
            var logger = new ErrorLogDao(dbc);
            logger.LogError(message);
        }

        private EmailMessage createOtpEmailMessage(string email, string otpNumber)
        {
            return new EmailMessage
            {
                FromAddress = "intune.userservice@gmail.com",
                FromAddressDisplayName = "Intune user services",
                FromPassword = StringCipher.Decrypt("zNElhP2H2K9AVdqSolXa5g==", "SynergyUserFeedbackMailPassword"),
                ToAddress = email,
                ToAddressDisplayName = "Intune",
                Subject = "Your intune verification code",
                Body = string.Format("Your unique verification code for Intune is {0}. Thanks you.", otpNumber),
            };
        }

        public void VerifyEmailOtp(string emailAddress, string otp)
        {
            var email = new EmailAddress(emailAddress);

            using (DbContext dbc = new DbContext())
            {
                var dao = new OtpDao(dbc);
                if (!dao.IsEmailOtpValid(email.Address, otp))
                    throw new Exception("Invalid OTP. Please request new one");
            }
        }

        public void SendMobileOtp(string isdCode, string mobileNumber)
        {
            var mobile = new MobileNumber(isdCode, mobileNumber);

            using (DbContext dbc = new DbContext())
            {
                var otpNumber = generateOtp(OtpStyle.Numeric);
                var otpMessage = string.Format("Your unique verification code for Intune is {0}. Thanks you.", otpNumber);
                const string senderId = "INTUNE";

                try
                {
                    SmsService.SendOtp(mobile.FullNumberWithoutPlus, otpMessage, senderId, otpNumber);
                }
                catch (Exception ex)
                {
                    logError(dbc, ex.ToString());
                    throw ex;
                }

                var dao = new OtpDao(dbc);
                dao.SendMobileOtp(mobile.FullNumber, otpNumber);
            }
        }

        public void VerifyMobileOtp(string isdCode, string mobileNumber, string otp)
        {
            var mobile = new MobileNumber(isdCode, mobileNumber);

            using (DbContext dbc = new DbContext())
            {
                var dao = new OtpDao(dbc);
                if (!dao.IsMobileOtpValid(mobile.FullNumber, otp))
                    throw new Exception("Invalid OTP. Please request new one");
            }
        }

        private static string generateOtp(OtpStyle style)
        {
            const int maxSize = 6;

            var sequence = getOtpCodeSequence(style);
            var chars = sequence.ToCharArray();

            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();

            var data = new byte[1];
            crypto.GetNonZeroBytes(data);

            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);

            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
                result.Append(chars[b % (chars.Length - 1)]);

            return result.ToString();
        }

        private static string getOtpCodeSequence(OtpStyle style)
        {
            switch (style)
            {
                case OtpStyle.Alpha:
                    return "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                case OtpStyle.Numeric:
                    return "1234567890";
                case OtpStyle.AlphaNumeric:
                    return "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                default:
                    throw new ArgumentException("Invalid argument value");
            }
        }
    }
}
