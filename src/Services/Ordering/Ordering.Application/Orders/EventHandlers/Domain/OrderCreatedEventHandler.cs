﻿namespace Ordering.Application.Orders.EventHandlers.Domain
{
    public class OrderCreatedEventHandler(ILogger<OrderCreatedEvent> logger)
        : INotificationHandler<OrderCreatedEvent>
    {
        public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain Event handler: {DomainEvent}", notification.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
