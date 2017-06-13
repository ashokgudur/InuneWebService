using InTune.Data;
using System;

namespace InTune.Logic
{
    public class OtpDao
    {
        DbContext _dbc = null;

        public OtpDao(DbContext dbc)
        {
            _dbc = dbc;
        }

        public void SendEmailOtp(string emailAddress, string otp)
        {
            var sql = "insert into EmailOtp (Email, Otp, OtpTimestamp) " +
                      "values (@email, @otp, @otpTimestamp)";

            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@email", emailAddress);
            _dbc.AddParameterWithValue(cmd, "@otp", otp);
            _dbc.AddParameterWithValue(cmd, "@otpTimestamp", DateTime.Now);
            cmd.ExecuteNonQuery();
        }

        public bool IsEmailOtpValid(string emailAddress, string otp)
        {
            const int otpValidityInMins = 15;
            var sql = "select count * from EmailOtp where Email=@email and Otp=@otp " +
                      "and datediff(mi, OtpTimestamp, getdate()) <= @otpValidityInMins";

            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@email", emailAddress);
            _dbc.AddParameterWithValue(cmd, "@otp", otp);
            _dbc.AddParameterWithValue(cmd, "@otpValidityInMins", otpValidityInMins);
            var result = cmd.ExecuteScalar();
            var count = Convert.ToInt32(result);
            return count > 0;
        }

        public void SendMobileOtp(string mobileNumber, string otp)
        {
            var sql = "insert into MobileOtp  (Mobile, Otp, OtpTimestamp) " +
                      "values (@mobile, @otp, @otpTimestamp)";

            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@mobile", mobileNumber);
            _dbc.AddParameterWithValue(cmd, "@otp", otp);
            _dbc.AddParameterWithValue(cmd, "@otpTimestamp", DateTime.Now);
            cmd.ExecuteNonQuery();
        }

        public bool IsMobileOtpValid(string mobileNumber, string otp)
        {
            const int otpValidityInMins = 15;
            var sql = "select count * from MobileOtp where Mobile=@mobile and Otp=@otp " +
                      "and datediff(mi, OtpTimestamp, getdate()) <= @otpValidityInMins";

            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@mobile", mobileNumber);
            _dbc.AddParameterWithValue(cmd, "@otp", otp);
            _dbc.AddParameterWithValue(cmd, "@otpValidityInMins", otpValidityInMins);
            var result = cmd.ExecuteScalar();
            var count = Convert.ToInt32(result);
            return count > 0;
        }
    }
}
