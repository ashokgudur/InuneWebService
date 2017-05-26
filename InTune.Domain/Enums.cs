using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InTune.Domain
{
    public enum TxnType
    {
        [Description("Paid")]
        Paid = 0,
        [Description("Issued")]
        Issu = 1,
        [Description("Received")]
        Rcvd = 2
    }

    public enum CommentStatus
    {
        Unread = 0,
        Read = 1,
    }

    public enum UserAccountRole
    {
        Owner = 0,
        Impersonator = 1,
        Collaborator = 2,
        Viewer = 3
    }
}
