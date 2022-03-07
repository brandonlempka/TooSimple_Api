using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooSimple_Database.Entities
{
    public class Transaction
    {
        public string TransactionId { get; set; } = string.Empty;
        public string? AccountOwner { get; set; }
        public decimal Amount { get; set; }
        public DateTime? AuthorizedDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? CurrencyCode { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
