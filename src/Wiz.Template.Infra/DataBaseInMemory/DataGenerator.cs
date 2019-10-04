using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wiz.Template.Domain.Models;
using Wiz.Template.Infra.Context;

namespace Wiz.Template.Infra.DataBaseInMemory
{
    public class DataGenerator
    {

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new EntityContext(
                serviceProvider.GetRequiredService<DbContextOptions<EntityContext>>()))
            {
                // Look for any customer.
                if (context.Customers.Any())
                {
                    return;   // Data was already seeded
                }

                context.Customers.AddRange(
                    new Customer(1, 1, "Jose da Silva"),
                    new Customer(2, 2, "Joao da Silva"),
                    new Customer(3, 3, "Maria da Silva")
                );

                context.Addresses.AddRange(
                    new Address("72870221") { Id = 1, Customers = context.Customers.Where((item, index) => index == 0).ToList() },
                    new Address("72870263") { Id = 1, Customers = context.Customers.Where((item, index) => index == 1).ToList() },
                    new Address("71727506") { Id = 1, Customers = context.Customers.Where((item, index) => index == 2).ToList() }
                );
            }
        }
    }
}
