using Marten.Schema;

namespace CatalogAPI.Data
{
    public class CatalogInitialData : IInitialData
    {
        public async Task Populate(IDocumentStore store, CancellationToken cancellation)
        {
            using var session = store.LightweightSession();

            if (await session.Query<Product>().AnyAsync())
                return;

            // Marten UPSERT will cater for existing records
            session.Store<Product>(GetPreconfiguredProducts()); 
            await session.SaveChangesAsync();
        }

        private static IEnumerable<Product> GetPreconfiguredProducts() => new List<Product>()
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Laptop",
                Category = new List<string> { "Electronics", "Computers" },
                Description = "A high-performance laptop.",
                ImageFile = "laptop.jpg",
                Price = 999.99m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Smartphone",
                Category = new List<string> { "Electronics", "Mobile" },
                Description = "A latest generation smartphone.",
                ImageFile = "smartphone.jpg",
                Price = 799.99m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Headphones",
                Category = new List<string> { "Electronics", "Audio" },
                Description = "Noise-cancelling headphones.",
                ImageFile = "headphones.jpg",
                Price = 199.99m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Coffee Maker",
                Category = new List<string> { "Home Appliances", "Kitchen" },
                Description = "Automatic coffee maker.",
                ImageFile = "coffee_maker.jpg",
                Price = 49.99m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Gaming Console",
                Category = new List<string> { "Electronics", "Gaming" },
                Description = "Next-gen gaming console.",
                ImageFile = "gaming_console.jpg",
                Price = 499.99m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Smart Watch",
                Category = new List<string> { "Electronics", "Wearable" },
                Description = "Smart watch with health tracking.",
                ImageFile = "smart_watch.jpg",
                Price = 299.99m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Refrigerator",
                Category = new List<string> { "Home Appliances", "Kitchen" },
                Description = "Energy-efficient refrigerator.",
                ImageFile = "refrigerator.jpg",
                Price = 899.99m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Microwave Oven",
                Category = new List<string> { "Home Appliances", "Kitchen" },
                Description = "Compact microwave oven.",
                ImageFile = "microwave_oven.jpg",
                Price = 149.99m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Digital Camera",
                Category = new List<string> { "Electronics", "Photography" },
                Description = "High-resolution digital camera.",
                ImageFile = "digital_camera.jpg",
                Price = 699.99m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Bluetooth Speaker",
                Category = new List<string> { "Electronics", "Audio" },
                Description = "Portable Bluetooth speaker.",
                ImageFile = "bluetooth_speaker.jpg",
                Price = 99.99m
            }
        };
    }
}
