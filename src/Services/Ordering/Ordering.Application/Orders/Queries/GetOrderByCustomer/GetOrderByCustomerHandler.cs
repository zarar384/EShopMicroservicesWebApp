namespace Ordering.Application.Orders.Queries.GetOrderByCustomer;

public class GetOrderByCustomerHandler(IApplicationDbContext context) : IQueryHandler<GetOrderByCustomerQuery, GetOrderByCustomerResult>
{
    public async Task<GetOrderByCustomerResult> Handle(GetOrderByCustomerQuery query, CancellationToken cancellationToken)
    {
        var orders = await context.Orders
               .Include(o => o.OrderItems)
               .AsNoTracking()
               .Where(o => o.CustomerId == CustomerId.Of(query.CustomerId))
               .OrderBy(o => o.OrderName.Value)
               .ToListAsync(cancellationToken);

        return new GetOrderByCustomerResult(orders.ToOrderDtoList());
    }
}
