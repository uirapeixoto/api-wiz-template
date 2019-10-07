using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wiz.Template.API.Services.Interfaces;
using Wiz.Template.API.ViewModels.Customer;

namespace Wiz.Template.API.Controllers
{
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/customers-redis")]
    public class CustomersRedisController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IDistributedCache _cache;

        public CustomersRedisController(ICustomerService customerService, IDistributedCache cache)
        {
            _customerService = customerService;
            _cache = cache;
        }


        /// <summary>
        /// Lista de clientes.
        /// </summary>
        /// <returns>Clientes.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerAddressViewModel>>> GetAll()
        {
            var  cachekey = _cache.GetString("CustomersRedis");
            IEnumerable<CustomerAddressViewModel> result = new List<CustomerAddressViewModel>();

            if (cachekey == null)
            {
                result = await _customerService.GetAllAsync();

                DistributedCacheEntryOptions opcoesCache = new DistributedCacheEntryOptions();
                opcoesCache.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));

                _cache.SetString("Customers", cachekey, opcoesCache);
            }

            return Ok(result);
        }

    }
}