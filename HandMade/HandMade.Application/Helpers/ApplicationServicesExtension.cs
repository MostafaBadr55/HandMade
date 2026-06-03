using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Helpers
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
        {
            //Inject MediatR package
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblies(typeof(ApplicationAssymblyMaker).Assembly));

            return services;
        }
    }
}
