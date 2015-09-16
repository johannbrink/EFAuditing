using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EFAuditing.TestHarness.Shared
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Timestamp]
        [DoNotAudit]
        public byte[] ConcurrencyStamp { get; set; }
    }
}