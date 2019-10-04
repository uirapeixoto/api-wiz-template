using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wiz.Template.API.Services.Interfaces;
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
        private readonly ICustomerRepository _customerRepository;
        private readonly IViaCEPService _viaCEPService;
        private readonly IDomainNotification _domainNotification;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IViaCEPService viaCEPService, IDomainNotification domainNotification, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _viaCEPService = viaCEPService;
            _domainNotification = domainNotification;
            _unitOfWork = unitOfWork;
            _mapper = mapper;

            InitializeData();

        }

        public async Task<IEnumerable<CustomerAddressViewModel>> GetAllAsync()
        {
            var customers = _mapper.Map<IEnumerable<CustomerAddressViewModel>>(await Task.FromResult(_customerData));

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
            CustomerViewModel viewModel = null;
            var model = _mapper.Map<Customer>(customerVM);

            var validation = new CustomerInsertValidation(_customerRepository).Validate(model);

            if (!validation.IsValid)
            {
                _domainNotification.AddNotifications(validation);
                return viewModel;
            }

            /*
             * EXEMPLO COM TRANSAÇÃO: 
             * Adicione a função "BeginTransaction()": _unitOfWork.BeginTransaction();
             * Utilize transação somente se realizar mais de uma operação no banco de dados ou banco de dados distintos
            */

            _customerData.Add(model);
            viewModel = _mapper.Map<CustomerViewModel>(model);

            return viewModel;
        }

        public void Update(CustomerViewModel customerVM)
        {
            var model = _mapper.Map<Customer>(customerVM);

            var validation = new CustomerUpdateValidation(_customerRepository).Validate(model);

            if (!validation.IsValid)
            {
                _domainNotification.AddNotifications(validation);
                return;
            }

            var r = _customerData.FirstOrDefault(x => x.Id == model.Id);
            r.Name = customerVM.Name;
            r.Address = _mapper.Map<Address>(customerVM.Address);

        }

        public void Remove(CustomerViewModel customerVM)
        {
            var model = _mapper.Map<Customer>(customerVM);

            var validation = new CustomerDeleteValidation().Validate(model);

            if (!validation.IsValid)
            {
                _domainNotification.AddNotifications(validation);
                return;
            }

            _customerRepository.Remove(model);
            _unitOfWork.Commit();
        }

        public IList<Customer> _customerData;
        public IList<Address> _addressData;

        private void InitializeData()
        {

            _customerData = new List<Customer>()
                {
                    new Customer(addressId: _addressData.First(x => x.CEP == "17052520").Id, name: "Zier Zuveiku"),
                    new Customer(addressId: _addressData.First(x => x.CEP == "44573100").Id, name: "Vikehel Pleamakh"),
                    new Customer(addressId: _addressData.First(x => x.CEP == "50080490").Id, name: "Diuor PleaBolosmakh")
                };

            _addressData = new List<Address>()
                {
                    new Address(cep: "17052520"),
                    new Address(cep: "44573100"),
                    new Address(cep: "50080490")
                };
        }
    }
}
