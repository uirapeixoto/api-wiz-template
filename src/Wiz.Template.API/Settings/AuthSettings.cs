using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Wiz.Template.Infra.Context;

namespace Wiz.Template.API.Settings
{
    public class AuthSettings
    {
        public AuthSettings(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void RegisterAuthService(IServiceCollection services)
        {
            services.AddTransient<EntityContext>();

            var signingConfigurations = new SigningConfigurations();
            services.AddSingleton(signingConfigurations);

            var tokenConfigurations = new TokenConfigurations();
            new ConfigureFromConfigurationOptions<TokenConfigurations>(
                Configuration.GetSection("TokenConfigurations"))
                    .Configure(tokenConfigurations);
            services.AddSingleton(tokenConfigurations);


            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>
            {
                var paramsValidation = bearerOptions.TokenValidationParameters;
                paramsValidation.IssuerSigningKey = signingConfigurations.Key;
                paramsValidation.ValidAudience = tokenConfigurations.Audience;
                paramsValidation.ValidIssuer = tokenConfigurations.Issuer;

                // Valida a assinatura de um token recebido
                paramsValidation.ValidateIssuerSigningKey = true;

                // Verifica se um token recebido ainda é válido
                paramsValidation.ValidateLifetime = true;

                // Tempo de tolerância para a expiração de um token (utilizado
                // caso haja problemas de sincronismo de horário entre diferentes
                // computadores envolvidos no processo de comunicação)
                paramsValidation.ClockSkew = TimeSpan.Zero;
            });

            // Ativa o uso do token como forma de autorizar o acesso
            // a recursos deste projeto
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });
        }

        public void RegisterAuthRepository(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
        }

    }

    public class TokenConfigurations
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int Seconds { get; set; }
    }

    public class SigningConfigurations
    {
        public SecurityKey Key { get; }
        public SigningCredentials SigningCredentials { get; }

        public SigningConfigurations()
        {
            using (var provider = new RSACryptoServiceProvider(2048))
            {
                Key = new RsaSecurityKey(provider.ExportParameters(true));
            }

            SigningCredentials = new SigningCredentials(
                Key, SecurityAlgorithms.RsaSha256Signature);
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string AccessKey { get; set; }
    }

    public interface IUserRepository
    {
         IEnumerable<User> GetAll();
         User Get(int id);
         User GetByUserId(string userId);
    }

    public class UserRepository : IUserRepository
    {
        public IList<User> _userData;

        public UserRepository()
        {
            _userData = new List<User>
            {
                new User{ Id = 1, UserId = "usuario01", AccessKey = new Guid("531fd5b1-9d58-438d-a0fd-9afface43b3c").ToString()},
                new User{ Id = 2, UserId = "usuario02", AccessKey = new Guid("605cd102-c4ef-459b-b91b-9becca3fded1").ToString()},
                new User{ Id = 3, UserId = "usuario03", AccessKey = new Guid("79bd8b63-2cb6-4d86-bf7a-a979c370fb09").ToString()}
            };
        }

        public IEnumerable<User> GetAll() => _userData.ToList();
        public User Get(int id) => _userData.FirstOrDefault(x => x.Id == id);
        public User GetByUserId(string userId) => _userData.FirstOrDefault(x => x.UserId == userId);
    }
}
