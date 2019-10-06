using System.Collections.Generic;
using System.Linq;
using Wiz.Template.Domain.Models;

namespace Wiz.Template.Infra.Context
{
    public class EntityContextSeed
    {
        public void SeedInitial(EntityContext context)
        {
            if (!context.Addresses.Any())
            {
                var addresses = new List<Address>()
                {
                    new Address(cep: "17052520"),
                    new Address(cep: "44573100"),
                    new Address(cep: "50080490")
                };

                context.AddRange(addresses);
                context.SaveChanges();
            }

            if (!context.Customers.Any())
            {
                var addresses = context.Addresses.ToList();

                var customers = new List<Customer>()
                {
                    new Customer{ Id = 1, AddressId = addresses.First(x => x.CEP == "17052520").Id, Name = "Zier Zuveiku" },
                    new Customer{ Id = 2, AddressId  = addresses.First(x => x.CEP == "44573100").Id, Name = "Vikehel Pleamakh"},
                    new Customer{ Id = 3, AddressId   = addresses.First(x => x.CEP == "50080490").Id, Name = "Diuor PleaBolosmakh"}
                };

                context.AddRange(customers);
                context.SaveChanges();
            }
        }
    }
}
