﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rhisis.Database
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection databaseConfigSection = configuration.GetSection(nameof(DatabaseConfiguration));
            DatabaseConfiguration databaseConfiguration = databaseConfigSection.Get<DatabaseConfiguration>();

            services.AddSingleton(databaseConfiguration); // TODO: remove this and use IOptions<> instead
            services.Configure<DatabaseConfiguration>(databaseConfigSection);
            services.AddDbContext<IRhisisDatabase, RhisisDatabaseContext>(options =>
            {
                options.UseMySql(DatabaseFactory.BuildConnectionString(databaseConfiguration));
            });

            return services;
        }
    }
}