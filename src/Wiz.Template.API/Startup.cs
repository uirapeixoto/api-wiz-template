﻿using AutoMapper;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using NSwag;
using NSwag.SwaggerGeneration.Processors.Security;
using Polly;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using Wiz.Template.API.Extensions;
using Wiz.Template.API.Filters;
using Wiz.Template.API.Middlewares;
using Wiz.Template.API.Services;
using Wiz.Template.API.Services.Interfaces;
using Wiz.Template.API.Settings;
using Wiz.Template.API.Swagger;
using Wiz.Template.Domain.Interfaces.Identity;
using Wiz.Template.Domain.Interfaces.Notifications;
using Wiz.Template.Domain.Interfaces.Repository;
using Wiz.Template.Domain.Interfaces.Services;
using Wiz.Template.Domain.Interfaces.UoW;
using Wiz.Template.Domain.Notifications;
using Wiz.Template.Infra.Context;
using Wiz.Template.Infra.Identity;
using Wiz.Template.Infra.Repository;
using Wiz.Template.Infra.Services;
using Wiz.Template.Infra.UoW;

[assembly: ApiConventionType(typeof(MyApiConventions))]
namespace Wiz.Template.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add<DomainNotificationFilter>();
                options.Filters.Add<ActionFilterAuthState>();
                options.EnableEndpointRouting = false;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddJsonOptions(options =>
             {
                 options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
             });

            #region configuração da autenticação por token

            var authSettings = new AuthSettings(Configuration);
            authSettings.RegisterAuthService(services);
            authSettings.RegisterAuthRepository(services);

            #endregion

            // Ativando o uso de cache em memória
            services.AddMemoryCache();

            services.AddApiVersioning(x => x.ApiVersionReader = new HeaderApiVersionReader("api-version"));
            services.Configure<GzipCompressionProviderOptions>(x => x.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(x =>
            {
                x.Providers.Add<GzipCompressionProvider>();
            });

            ResgisterRedis(services);

            services.AddHttpClient<IViaCEPService, ViaCEPService>((s, c) =>
            {
                c.BaseAddress = new Uri(Configuration["API:ViaCEP"]);
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.OrResult(response =>
                    !response.IsSuccessStatusCode)
              .WaitAndRetryAsync(3, retry =>
                   TimeSpan.FromSeconds(Math.Pow(2, retry)) +
                   TimeSpan.FromMilliseconds(new Random().Next(0, 100))))
              .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.CircuitBreakerAsync(
                   handledEventsAllowedBeforeBreaking: 3,
                   durationOfBreak: TimeSpan.FromSeconds(30)
            ));

            if (!HostingEnvironment.IsProduction())
            {
                services.AddOpenApiDocument(document =>
                {
                    document.DocumentName = "v1";
                    document.Version = "v1";
                    document.Title = "Template API";
                    document.Description = "API de Template";
                    document.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
                    document.AddSecurity("JWT", Enumerable.Empty<string>(), new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.ApiKey,
                        Name = HeaderNames.Authorization,
                        Description = "Token de autenticação via SSO",
                        In = SwaggerSecurityApiKeyLocation.Header
                    });
                });
            }

            services.AddAutoMapper(typeof(Startup));
            services.AddHttpContextAccessor();
            RegisterServices(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptions<ApplicationInsightsSettings> options)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseResponseCompression();

            if (!env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUi3();
            }

            app.UseAuthentication();
            //app.UseLogMiddleware();

            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = new ErrorHandlerMiddleware(options, env).Invoke
            });

            app.UseMvc();
        }

        private void ResgisterRedis(IServiceCollection services)
        {
            // Ativando o uso de cache via Redis
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = "localhost";
                options.InstanceName = "TesteRedisCache";
            });

        }

        private void RegisterServices(IServiceCollection services)
        {
            services.Configure<ApplicationInsightsSettings>(Configuration.GetSection("ApplicationInsights"));

            #region Service

            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IAuthService, AuthService>();

            #endregion

            #region Domain

            services.AddScoped<IDomainNotification, DomainNotification>();

            #endregion

            #region Infra

            services.AddDbContext<EntityContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ExemploJWT")));
            services.AddScoped<DapperContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IIdentityService, IdentityService>();

            #endregion
        }
    }
}
