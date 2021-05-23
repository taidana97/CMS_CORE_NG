using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WritableOptionsService;

namespace CMS_CORE_NG.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureWritable<T>(
            this IServiceCollection services,
            IConfigurationSection section,
            string filename) where T :class, new()
        {
            // Ex: section = var dataProtectionService = Configuration.GetSection("DataProtectionKeys");
            services.Configure<T>(section);
            //Ex: services.Configure<DataProtectionKeys>(dataProtectionService);

            services.AddTransient<IWritableSvc<T>>(provider =>
            {
                var env = provider.GetService<IWebHostEnvironment>();
                var options = provider.GetService<IOptionsMonitor<T>>();

                return new WritableSvc<T>(env, options, section.Key, filename);
            });
        }
    }
}
