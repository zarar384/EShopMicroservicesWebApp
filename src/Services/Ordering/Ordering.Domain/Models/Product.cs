namespace Ordering.Domain.Models
{
    internal class Product: Entity<ProductId>
    {
        public string Name { get; private set; } = default!;
        public decimal Price { get; private set; } = default!;
    }
}
