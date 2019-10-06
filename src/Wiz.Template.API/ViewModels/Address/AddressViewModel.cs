using System.Collections.Generic;
using Wiz.Template.API.ViewModels.Customer;

namespace Wiz.Template.API.ViewModels.Address
{
    public class AddressViewModel
    {
        public AddressViewModel()
        {
            Customers = new List<CustomerViewModel>();
            Street = "";
            StreetFull = "";
            UF = "";
        }

        public int Id { get; set; }
        public string CEP { get; set; }
        public string Street { get; set; }
        public string StreetFull { get; set; }
        public string UF { get; set; }

        public IEnumerable<CustomerViewModel> Customers{get; set;}
    }
}
