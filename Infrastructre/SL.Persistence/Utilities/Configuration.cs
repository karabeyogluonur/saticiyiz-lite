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
        public static string PostgresConnectionString
        {

            get
            {
                return ConfigurationManager.GetConnectionString("PostgresConnection");
            }

        }
        public static string TenantConnectionString(string tenantDatabaseName)
        {

            return ConfigurationManager.GetConnectionString("TenantConnection").Replace("{tenantDatabaseName}", tenantDatabaseName);

        }
    }
}

