using System.Collections.Generic;

namespace Wiz.Template.Domain.Models
{
    public class Address
    {
        public Address()
        {
            Customers = new List<Customer>();
        }

        public Address(string cep)
        {
            CEP = cep;
        }

        public int Id { get;  set; }
        public string CEP { get; set; }

        public IEnumerable<Customer> Customers { get; set; }
    }
}
