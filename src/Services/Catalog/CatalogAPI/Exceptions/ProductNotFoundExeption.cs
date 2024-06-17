using BuildingBlocks.Exceptions;

namespace CatalogAPI.Exceptions
{
    public class ProductNotFoundExeption: NotFountException
    {
        public ProductNotFoundExeption(Guid Id): base("Product", Id)
        {
            
        }
    }
}
