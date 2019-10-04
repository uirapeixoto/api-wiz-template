using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Wiz.Template.API.Services.Interfaces;
using Wiz.Template.API.ViewModels;
using Wiz.Template.API.ViewModels.Customer;

namespace Wiz.Template.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/cota")]
    public class CotacoesController : ControllerBase
    {

        private IConfiguration _configuration;
        private IMemoryCache _cache;
        private ICustomerService _service;
        private IAuthService _auth;

        private TokenAttributeViewModel _token;

        public CotacoesController(IConfiguration configuration, IMemoryCache cache, ICustomerService service, IAuthService auth)
        {
            _configuration = configuration;
            _cache = cache;
            _service = service;
            _auth = auth;

        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            _token = _auth.GetTokenAttributes(User.Claims);


            string valorJson = _cache.GetOrCreate(
                "Cotacoes",  ctx =>
                {
                    ctx.SetAbsoluteExpiration(TimeSpan.FromSeconds(3600));
                    ctx.SetPriority(CacheItemPriority.High);

                    return _token.AccessKey;
                });

           // var r = await _service.GetAllAsync();


            return Ok(new { data = valorJson });
        }
    }
}