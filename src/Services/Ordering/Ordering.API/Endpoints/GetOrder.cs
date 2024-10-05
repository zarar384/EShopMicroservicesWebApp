using BuildingBlocks.Pagination;
using Ordering.Application.Orders.Queries.GetOrders;

namespace Ordering.API.Endpoints
{
    //- Accepts pagination parameters.
    //- Constructs a GetOrdersQuery with these parameters.
    //- Retrieves the data and returns it in a paginated format.

    public record GetOrderResponse(PaginationResult<OrderDto> Orders);

    public class GetOrder : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders", async ([AsParameters] PaginationRequest request, ISender sender) =>
            {
                var result = await sender.Send(new GetOrdersQuery(request));

                var response = result.Adapt<GetOrderResponse>();

                return Results.Ok(response);
            })
             .WithName("GetOrders")
            .Produces<GetOrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Orders")
            .WithDescription("Get Orders");
        }
    }
}
