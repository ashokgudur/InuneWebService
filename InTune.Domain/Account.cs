﻿using System;

namespace InTune.Domain
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public UserAccountRole Role { get; set; }
        public DateTime AddedOn { get; set; }
        public int UserId { get; set; }
        public int ContactId { get; set; }
        public decimal Balance { get; set; }
        public bool HasEntries { get; set; }
        public bool HasComments { get; set; }
        public bool HasUnreadComments { get; set; }

        public Account()
        {
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }
    }
}
