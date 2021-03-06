﻿using System;
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
            get { return $"+{IsdCode}{Number}"; }
        }

        public string FullNumberWithoutPlus
        {
            get
            {
                return $"{IsdCode}{Number}";
            }
        }

        public MobileNumber(string isdCode, string number)
        {
            _isdCode = (isdCode + "").Trim().Replace("+", "");
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
    }
}
