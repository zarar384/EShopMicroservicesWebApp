using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Messaging.MassTransit
{
    public static class Extensions
    {
        // Extension method to add the message broker (RabbitMQ in this case) to the IServiceCollection
        public static IServiceCollection AddMessageBroker(
            this IServiceCollection services, 
            IConfiguration configuration, 
            Assembly? assembly = null,
            Action<MassTransitOptions>? configureOptions = null)
        {
            var options = new MassTransitOptions();
            configureOptions?.Invoke(options);

            services.AddMassTransit(config =>
            {
                // Use kebab-case for endpoint names (e.g., 'order-created' instead of 'OrderCreated')
                config.SetKebabCaseEndpointNameFormatter();

                if (assembly != null)
                    config.AddConsumers(assembly);

                // CALLBACK FOR ENDPOINT CONFIG
                if (options.ConfigureEndpoint != null)
                {
                    config.AddConfigureEndpointsCallback((ctx, name, endpointCfg) =>
                    {
                        options.ConfigureEndpoint(endpointCfg);
                    });
                }

                // Configure RabbitMQ as the transport mechanism
                config.UsingRabbitMq((context, configurator) =>
                {
                    // Set the RabbitMQ host URL, username, and password from the configuration
                    configurator.Host(new Uri(configuration["MessageBroker:Host"]!), host =>
                    {
                        host.Username(configuration["MessageBroker:UserName"]!);
                        host.Password(configuration["MessageBroker:Password"]!);
                    });

                    // custom bus config
                    options.ConfigureBus?.Invoke(context, configurator);

                    // REQUIRED — register endpoints
                    configurator.ConfigureEndpoints(context);

                });
            });

            return services;
        }
    }
}
