using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wiz.Template.API.Services.Interfaces;
using Wiz.Template.API.ViewModels.Address;
using Wiz.Template.API.ViewModels.Customer;
using Wiz.Template.Domain.Interfaces.Notifications;
using Wiz.Template.Domain.Interfaces.Repository;
using Wiz.Template.Domain.Interfaces.Services;
using Wiz.Template.Domain.Interfaces.UoW;
using Wiz.Template.Domain.Models;
using Wiz.Template.Domain.Validation.CustomerValidation;

namespace Wiz.Template.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IViaCEPService _viaCEPService;
        private readonly IDomainNotification _domainNotification;
        private readonly IMapper _mapper;

        public IEnumerable<Customer> _customerData;
        public IEnumerable<Address> _addressData;

        public CustomerService(ICustomerRepository customerRepository, IViaCEPService viaCEPService, IDomainNotification domainNotification, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _viaCEPService = viaCEPService;
            _domainNotification = domainNotification;
            _mapper = mapper;

            _addressData = new List<Address>()
                {
                    new Address{ CEP = "17052520", Id = 1, Customers = new List<Customer>{ new Customer{Id = 1}}},
                    new Address{ CEP = "44573100", Id = 2, Customers = new List<Customer>{ new Customer{Id = 2}}},
                    new Address{ CEP = "50080490", Id = 3, Customers = new List<Customer>{ new Customer{Id = 3}}}
                };

            _customerData = new List<Customer>()
                {
                    new Customer{ Id = 1, AddressId = 1, Name = "Zier Zuveiku", Address = _addressData.FirstOrDefault(x => x.Id.Equals(1))},
                    new Customer{ Id = 2, AddressId = 2, Name = "Vikehel Pleamakh", Address = _addressData.FirstOrDefault(x => x.Id.Equals(2))},
                    new Customer{ Id = 3, AddressId = 3, Name = "Diuor PleaBolosmakh", Address = _addressData.FirstOrDefault(x => x.Id.Equals(3))}
                };
        }

        public async Task<IEnumerable<CustomerAddressViewModel>> GetAllAsync()
        {
            var customers = _customerData.Select(x => new CustomerAddressViewModel {
                Id = x.Id,
                AddressId = x.AddressId,
                Name = x.Name,
                DateCreated = x.DateCreated,
                CEP = x.Address?.CEP,
                Address = new AddressViewModel { Id = x.Address.Id, CEP = x.Address.CEP }

            }).ToList();

            foreach (var customer in customers)
            {
                var address = await _viaCEPService.GetByCEPAsync(customer.CEP);

                customer.Address.Id = customer.AddressId;
                customer.Address.Street = address?.Street;
                customer.Address.StreetFull = address?.StreetFull;
                customer.Address.UF = address?.UF;
            }

            return customers;
        }

        public async Task<CustomerViewModel> GetByIdAsync(CustomerIdViewModel customerVM)
        {
            return _mapper.Map<CustomerViewModel>(await Task.FromResult(_customerData.FirstOrDefault(x => x.Id == customerVM.Id)));
        }

        public async Task<CustomerAddressViewModel> GetAddressByIdAsync(CustomerIdViewModel customerVM)
        {
            var customer = _mapper.Map<CustomerAddressViewModel>(await Task.FromResult(_customerData.FirstOrDefault(x => x.Id == customerVM.Id).Address));

            if (customer != null)
            {
                var address = await _viaCEPService.GetByCEPAsync(customer.CEP);

                customer.Address.Id = customer.AddressId;
                customer.Address.Street = address?.Street;
                customer.Address.StreetFull = address?.StreetFull;
                customer.Address.UF = address?.UF;
            }

            return customer;
        }

        public async Task<CustomerAddressViewModel> GetAddressByNameAsync(CustomerNameViewModel customerVM)
        {
            var customer = _mapper.Map<CustomerAddressViewModel>(await Task.FromResult(_customerData.FirstOrDefault(x => x.Name == customerVM.Name)));

            if (customer != null)
            {
                var address = await _viaCEPService.GetByCEPAsync(customer.CEP);

                customer.Address.Id = customer.AddressId;
                customer.Address.Street = address?.Street;
                customer.Address.StreetFull = address?.StreetFull;
                customer.Address.UF = address?.UF;
            }

            return customer;
        }

        public CustomerViewModel Add(CustomerViewModel customerVM)
        {
            return customerVM;
        }

        public void Update(CustomerViewModel customerVM)
        {
            var model = _mapper.Map<Customer>(customerVM);

            var r = _customerData.FirstOrDefault(x => x.Id == model.Id);
            r.Name = customerVM.Name;
            r.Address = _mapper.Map<Address>(customerVM.Address);

        }

        public void Remove(CustomerViewModel customerVM)
        {
            
        }
    }
}
