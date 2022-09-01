using System;
using Microsoft.Extensions.DependencyInjection;
using SkyCars.Data;
using SkyCars.Data.DataProviders;
using SkyCars.Services.Security;
using SkyCars.Services.Utilities;
using SkyCars.Services.Users;
using SkyCars.Service.Cities;
using SkyCars.Service.Media;

namespace SkyCarsWebAPI.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public static class DependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        public static void Register(IServiceCollection services)
        {
            /*
                - Transient operations are always different, a new instance is created with every retrieval of the service.
                - Scoped operations change only with a new scope, but are the same instance within a scope.
                - Singleton operations are always the same, a new instance is only created once.
                - The services are created by the service container and disposed automatically
             */
            /*
                - https://devblogs.microsoft.com/cesardelatorre/comparing-asp-net-core-ioc-service-life-times-and-autofac-ioc-instance-scopes/
                - InstancePerDependency = AddTransient
                - InstancePerLifetimeScope, InstancePerRequest = AddScoped
                - SingleInstance = AddSingleton
             */

            services.AddScoped<IDataProvider, MsSqlDataProvider>();
            services.AddScoped(typeof(IRepository<>), typeof(EntityRepository<>));
            //services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICityServices, CityService>();
            services.AddScoped<IMediaService, MediaService>();


        }
    }
}

