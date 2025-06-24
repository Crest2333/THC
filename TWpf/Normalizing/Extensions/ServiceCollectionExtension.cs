using Microsoft.Extensions.DependencyInjection;
using Normalizing.Services;
using Normalizing.Siemens;
using Normalizing.ViewModels.Pages;
using Normalizing.Views.Pages;
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

        public static IServiceCollection AddPage(this IServiceCollection services)
        {
            services.AddSingleton<DashboardPage>();
            services.AddSingleton<DataPage>();
            services.AddSingleton<SettingsPage>();
            services.AddSingleton<MachineToolPage>();
            services.AddSingleton<ManualControlPage>();
            services.AddSingleton<AutoControlPage>();
            return services;
        }

        public static IServiceCollection AddViewModel(this IServiceCollection services)
        {
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<DataViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<MachineToolViewModel>();
            services.AddSingleton<ManualControlViewModel>();
            return services;
        }
    }
}
