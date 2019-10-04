using System;

namespace Wiz.Template.Domain.Models
{
    public class Customer
    {
        protected Customer() {
            DateCreated = DateTime.Now;
        }

        public Customer(int id, int addressId, string name)
        {
            Id = id;
            AddressId = addressId;
            Name = name;
        }

        public Customer(int addressId, string name)
        {
            AddressId = addressId;
            Name = name;
        }

        public int Id { get;  set; }
        public int AddressId { get;  set; }
        public string Name { get;  set; }
        public DateTime DateCreated { get;  set; }

        public Address Address { get;  set; }
    }
}
