using System;
using Wiz.Template.API.ViewModels.Address;

namespace Wiz.Template.API.ViewModels.Customer
{
    public class CustomerAddressViewModel
    {
        public CustomerAddressViewModel()
        {
            Address = new AddressViewModel();
        }

        public int Id { get; set; }
        public int AddressId { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public string CEP { get; set; }
        public AddressViewModel Address { get; set; }
    }
}
