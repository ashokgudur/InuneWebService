using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InTune.Domain
{
    public class AccountContactUser
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int ContactId { get; set; }
        public int UserId { get; set; }
        public UserAccountRole Role { get; set; }
        public int ContactUserId { get; set; }

        public AccountContactUser()
        {
        }
    }
}
