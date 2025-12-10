using MassTransit;

namespace BuildingBlocks.Messaging.MassTransit
{
    public class MassTransitOptions
    {
        public Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? ConfigureBus { get; set; }
        public Action<IReceiveEndpointConfigurator>? ConfigureEndpoint { get; set; }
    }
}
