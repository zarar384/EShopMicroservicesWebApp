
namespace CatalogAPI.ProductsFeature.GetProductByCategory
{
    public record GetProductByCategoryQuery(string Category): IQuery<GetProductByCategoryResult>;
    public record GetProductByCategoryResult(IEnumerable<Product> Products);

    internal class GetProductByCategoryHandler(IDocumentSession session, ILogger<GetProductByCategoryHandler> logger) :
        IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
    {
        public async Task<GetProductByCategoryResult> Handle(GetProductByCategoryQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation("GetProductByCategoryHandler.Handle with {@Guery}", query);

            var product = await session.Query<Product>()
                .Where(p=>p.Category.Contains(query.Category))
                .ToListAsync();

            return new GetProductByCategoryResult(product);
        }
    }
}
