using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InTune.Domain
{
    public class EmailMessage
    {
        public string ToAddress { get; set; }
        public string ToAddressDisplayName { get; set; }
        public string FromAddress { get; set; }
        public string FromPassword { get; set; }
        public string FromAddressDisplayName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
