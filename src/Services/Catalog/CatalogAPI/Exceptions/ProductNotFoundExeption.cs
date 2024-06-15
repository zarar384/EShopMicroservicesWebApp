namespace CatalogAPI.Exceptions
{
    public class ProductNotFoundExeption: Exception
    {
        public ProductNotFoundExeption(): base("Prodcut not found!")
        {
            
        }
    }
}
