using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Wiz.Template.API.Services.Interfaces;
using Wiz.Template.API.ViewModels.Customer;
using Newtonsoft.Json;

namespace Wiz.Template.API.Controllers
{
    
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/redis-teste-key")]
    public class TestKeyController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IDistributedCache _cache;

        public TestKeyController(ICustomerService customerService, IDistributedCache cache)
        {
            _customerService = customerService;
            _cache = cache;
        }

        private void ArmazenaValorCache(string chave, string valor)
        {
            DistributedCacheEntryOptions opcoesCache =
                new DistributedCacheEntryOptions();
            opcoesCache.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));

            _cache.SetString(chave, valor);
        }

        [HttpGet]  
        public async Task<IActionResult> GetAsync()
        {
            string testeString = _cache.GetString("TesteString");
            IEnumerable<CustomerAddressViewModel> result = null;
            string valorJson = "";

            if(testeString == null)
            {
                //testeString = "Valor de exemplo";
                //ArmazenaValorCache("TesteString", testeString);

                result = await _customerService.GetAllAsync();
                valorJson = JsonConvert.SerializeObject(result);
                ArmazenaValorCache("TesteString", valorJson);
            }
            else
            {
                result = JsonConvert.DeserializeObject<IEnumerable<CustomerAddressViewModel>>(testeString);
            }

            return Ok(testeString);
        }


    }
}