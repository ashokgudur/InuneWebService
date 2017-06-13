using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace InTune.Domain
{
    public class Country
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string IsdCode { get; set; }
    }

    public class MobileNumber
    {
        string _isdCode;
        public string IsdCode
        {
            get { return _isdCode; }
        }

        string _number;
        public string Number
        {
            get { return _number; }
        }

        public string FullNumber
        {
            get { return string.Format("{0}{1}", IsdCode, Number); }
        }

        public string FullNumberWithoutPlus
        {
            get
            {
                if (IsdCode.Substring(0, 1) == "+")
                    return string.Format("{0}{1}", IsdCode.Substring(1), Number);
                else
                    return string.Format("{0}{1}", IsdCode, Number);
            }
        }

        public MobileNumber(string isdCode, string number)
        {
            _isdCode = (isdCode + "").Trim();
            _number = (number + "").Trim();

            validateMobileNumber();
        }

        private void validateMobileNumber()
        {
            if (string.IsNullOrWhiteSpace(_isdCode))
                throw new FormatException("Country ISD code cannot be empty");

            if (string.IsNullOrWhiteSpace(_number))
                throw new FormatException("Mobile number cannot be empty");

            if (_number.Length < 10)
                throw new FormatException("Mobile number length must be 10 digits");

            if (!Regex.IsMatch(_number, @"^[0-9]{10}$"))
                throw new FormatException("Mobile number must contain only numbers");
        }

        private void validateCountryCode()
        {
            var countries = JsonConvert.
                        DeserializeObject<List<Country>>(File.ReadAllText("CountryISDCodes.json"));

            if (!countries.Exists(c => c.IsdCode == _isdCode))
                throw new ArgumentException("Invalid country ISD code");
        }
    }
}
