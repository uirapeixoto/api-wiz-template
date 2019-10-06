using System;

namespace Wiz.Template.Domain.Models
{
    public class Customer
    {
        public Customer() {
            DateCreated = DateTime.Now;
        }

        public int Id { get;  set; }
        public int AddressId { get;  set; }
        public string Name { get;  set; }
        public DateTime DateCreated { get;  set; }

        public Address Address { get;  set; }
    }
}
