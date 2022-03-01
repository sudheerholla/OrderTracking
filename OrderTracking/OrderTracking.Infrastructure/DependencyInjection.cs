using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderTracking.Application.Interfaces;
using OrderTracking.Infrastructure.Context;
using OrderTracking.Infrastructure.Persistance;
using OrderTracking.Infrastructure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderTracking.Infrastructure
{
    [ExcludeFromCodeCoverageAttribute]
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            IConfiguration configuration = serviceProvider.GetService<IConfiguration>();

            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("OrderDb"));
            }
            else
            {
                string cosmosDbEndpoint = configuration.GetValue<string>("CosmosDBEndpoint");
                string accountKey = configuration.GetValue<string>("CosmosDBAccountKey");

                services.AddDbContext<ApplicationDbContext>(options => {
                    options.UseCosmos(
                        cosmosDbEndpoint,
                        accountKey,
                        databaseName: "OrderDb");
                });

                using var client = new CosmosClient(cosmosDbEndpoint, accountKey);
                var db = client.CreateDatabaseIfNotExistsAsync("OrderDb").GetAwaiter().GetResult();
                var container = db.Database.CreateContainerIfNotExistsAsync("Order", "/RestaurantId").GetAwaiter().GetResult();
              
            }
            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

            // if(cosmosDbEndpoint.Contains("localhost"))
            // {
            //     serviceProvider = services.BuildServiceProvider();

            //     AdminDbContextSeed.SeedSampleDataAsync(
            //         serviceProvider.GetService<AdminDbContext>(),
            //         serviceProvider.GetService<IDateTime>()
            //     );
            // }


            // service bus factory is a singleton to ensure new connections are only
            // made once per queue/topic for the lifetime of the app
            services.AddSingleton<IServiceBusConfiguration, ServiceBusConfiguration>();
            services.AddSingleton<IBusEndpointFactory, AzureServiceBusEndpointFactory>();

            // note: the below dependencies use a scope context (per call scope)
            services.AddScoped<ICallContext, MutableCallContext>();
            services.AddScoped<IMessageEnricher, AzureServiceBusCausalityEnricher>();
            return services;
        }
     }
}
