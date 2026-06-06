using HandMade.Application.Interfaces;
using HandMade.Infrastructure.Data;
using HandMade.Infrastructure.Identity;
using HandMade.Infrastructure.Persistence;
using HandMade.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Infrastructure.Helpers
{
    public static class InfrastructureServicesExtension
    {
        public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration config)
        {
            //inject Dbcontext
            services.AddDbContext<ApplicationDbContext>(
                options =>
                {
                    options.UseSqlServer(config.GetConnectionString("HandMade"));
                });


            // IUnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // IAccountServices
            services.AddScoped<IAccountServices, AccountServices>();

            // IAuthTokenService
            services.AddScoped<IAuthTokenService, JwtTokenService>();
            services.AddScoped<IRoleServices, RolesServices>();

            //LocalStorge
            services.AddScoped<IStorageService, LocalStorageService>();
            return services;
        }
    }
}
