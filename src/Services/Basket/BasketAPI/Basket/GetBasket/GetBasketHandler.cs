namespace BasketAPI.Basket.GetBasket
{
    public record GetBasketQuery(string UserName): IQuery<GetBasketResult>;
    public record GetBasketResult(ShoppingCart Cart);

    // mapping to app layer
    public class GetBasketQueryHandler : IQueryHandler<GetBasketQuery, GetBasketResult>
    {
        public async Task<GetBasketResult> Handle(GetBasketQuery request, CancellationToken cancellationToken)
        {
            //TODO: get from db

            return new GetBasketResult(new ShoppingCart("swn"));
        }
    }
}
