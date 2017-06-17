using InTune.Data;
using InTune.Domain;
using System;
using System.Collections.Generic;

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
            emailAddress = emailAddress.ToLower();
            var sql = "insert into EmailOtp (Email, Otp, OtpTimestamp) " +
                      "values (@email, @otp, getdate())";

            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@email", emailAddress);
            _dbc.AddParameterWithValue(cmd, "@otp", otp);
            cmd.ExecuteNonQuery();
        }

        public bool IsEmailOtpValid(string emailAddress, string otp)
        {
            emailAddress = emailAddress.ToLower();
            const int otpValidityInMins = 180;
            var sql = "select count(*) from EmailOtp where Email=@email and Otp=@otp " +
                      "and datediff(mi, OtpTimestamp, getdate()) <= @otpValidityInMins";

            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@email", emailAddress);
            _dbc.AddParameterWithValue(cmd, "@otp", otp);
            _dbc.AddParameterWithValue(cmd, "@otpValidityInMins", otpValidityInMins);
            var result = cmd.ExecuteScalar();
            var count = Convert.ToInt32(result);
            return count > 0;
        }

        public bool IsIsdCodeValid(string isdCode)
        {
            var sql = "select count(*) from CountryIsdCode where IsdCode=@isdCode";
            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@isdCode", isdCode);
            var result = cmd.ExecuteScalar();
            var count = Convert.ToInt32(result);
            return count > 0;
        }

        public List<Country> GetCountryIsdCodes()
        {
            var result = new List<Country>();
            var sql = "select * from CountryIsdCode";
            using (var rdr = _dbc.ExecuteReader(sql))
            {
                while (rdr.Read())
                {
                    result.Add(new Country
                    {
                        Name = rdr["Name"].ToString(),
                        Code = rdr["Code"].ToString(),
                        IsdCode = rdr["IsdCode"].ToString(),
                    });
                }
            }

            return result;
        }

        public void SendMobileOtp(string mobileNumber, string otp)
        {
            var sql = "insert into MobileOtp  (Mobile, Otp, OtpTimestamp) " +
                      "values (@mobile, @otp, getdate())";

            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@mobile", mobileNumber);
            _dbc.AddParameterWithValue(cmd, "@otp", otp);
            _dbc.AddParameterWithValue(cmd, "@otpTimestamp", DateTime.Now);
            cmd.ExecuteNonQuery();
        }

        public bool IsMobileOtpValid(string mobileNumber, string otp)
        {
            const int otpValidityInMins = 15;
            var sql = "select count(*) from MobileOtp where Mobile=@mobile and Otp=@otp " +
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
