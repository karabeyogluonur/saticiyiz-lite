using System;
using Microsoft.Extensions.Configuration;

namespace SL.Persistence.Utilities
{
    public static class Configuration
    {
        private static ConfigurationManager ConfigurationManager
        {
            get
            {
                ConfigurationManager configurationManager = new();
                configurationManager.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../Presentation/SL.Web"));
                configurationManager.AddJsonFile("appsettings.json");
                return configurationManager;
            }
        }
        public static string MasterConnectionString
        {

            get
            {
                return ConfigurationManager.GetConnectionString("MasterConnection");
            }

        }
    }
}

