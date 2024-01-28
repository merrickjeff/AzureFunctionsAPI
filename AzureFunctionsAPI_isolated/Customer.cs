using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionsAPI_isolated
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public string? FullName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? ProfileImage { get; set; }
    }
}
