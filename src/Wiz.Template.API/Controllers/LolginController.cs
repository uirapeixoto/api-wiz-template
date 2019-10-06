using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Wiz.Template.API.Settings;
using Wiz.Template.API.ViewModels;

namespace Wiz.Template.API.Controllers
{
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/login")]
    public class LolginController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private IMemoryCache _cache;
        private string _token;
        private DateTime _dataCriacao;
        private DateTime _dataExpiracao;

        private CacheResponseViewModel _cacheResponse;

        public LolginController(IUserRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
            _cacheResponse = new CacheResponseViewModel();
            _token = "";
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Post(
            [FromBody]Settings.User usuario,
            [FromServices]SigningConfigurations signingConfigurations,
            [FromServices]TokenConfigurations tokenConfigurations)
        {
            bool credenciaisValidas = false;
            if (usuario != null && !string.IsNullOrWhiteSpace(usuario.UserId))
            {
                var usuarioBase = _repository.GetByUserId(usuario.UserId);
                credenciaisValidas = (usuarioBase != null &&
                    usuario.UserId == usuarioBase.UserId &&
                    usuario.AccessKey == usuarioBase.AccessKey);
            }

            if (credenciaisValidas)
            {
               _token = _cache.GetOrCreate(
                "Authentication", ctx =>
                {
                    ctx.SetAbsoluteExpiration(TimeSpan.FromSeconds(3600));
                    ctx.SetPriority(CacheItemPriority.High);

                    _cacheResponse.Expiration = ctx.AbsoluteExpirationRelativeToNow.Value;

                    ClaimsIdentity identity = new ClaimsIdentity(
                    new GenericIdentity(usuario.UserId, "Login"),
                    new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, usuario.AccessKey),
                        new Claim(JwtRegisteredClaimNames.Sid, usuario.AccessKey),
                        new Claim(JwtRegisteredClaimNames.NameId, usuario.UserId)
                    }
                );

                    _dataCriacao = DateTime.Now;
                    _dataExpiracao = _dataCriacao +
                    TimeSpan.FromSeconds(tokenConfigurations.Seconds);

                    var handler = new JwtSecurityTokenHandler();
                    var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                    {
                        Issuer = tokenConfigurations.Issuer,
                        Audience = tokenConfigurations.Audience,
                        SigningCredentials = signingConfigurations.SigningCredentials,
                        Subject = identity,
                        NotBefore = _dataCriacao,
                        Expires = _dataExpiracao
                    });
                    return handler.WriteToken(securityToken);
                });

                

                return Ok( new 
                {
                    authenticated = true,
                    created = _dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                    expiration = _dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                    accessToken = _token,
                    message = "OK"
                });
            }
            else
            {
                return Ok(new 
                {
                    authenticated = false,
                    message = "Falha ao autenticar"
                });
            }
        }
    }
}