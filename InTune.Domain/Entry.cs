using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InTune.Domain
{
    public class Entry
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AccountId { get; set; }
        public TxnType TxnType { get; set; }
        public string Notes { get; set; }
        public DateTime TxnDate { get; set; }
        public double Quantity { get; set; }
        public decimal Amount { get; set; }
        public int VoidId { get; set; }

        public Entry()
        {
        }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Notes))
                return false;

            if (Quantity == 0 && Amount == 0)
                return false;

            return true;
        }
    }
}
