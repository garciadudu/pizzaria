using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Pizzaria.Util
{
    public static class Configuration
    {
        static IConfigurationRoot _configuration { get; set; }

        static Configuration()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                _configuration = builder.Build();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string GetStringProperty(string v)
        {
            return _configuration.GetConnectionString(v);
        }
    }
    
}
