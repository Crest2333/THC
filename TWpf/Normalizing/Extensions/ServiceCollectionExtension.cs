using Microsoft.Extensions.DependencyInjection;
using Normalizing.Services;
using Normalizing.Siemens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizing.Extensions
{
    internal static class ServiceCollectionExtension
    {
        public static IServiceCollection UseMock(this IServiceCollection services)
        {
            services.AddSingleton<IDeviceManager, MockDeviceManager>();
            return services;    
        }

        public static IServiceCollection UseSiemens(this IServiceCollection services)
        {
            services.AddSingleton<IDeviceManager, SiemensManager>();
            return services;
        }
    }
}
