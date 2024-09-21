using Ordering.Domain.Events;

namespace Ordering.Infrastructure.Extensions
{
    public class InitialData
    {
        public static IEnumerable<Customer> Customers =>
            new List<Customer>
            {
                Customer.Create(CustomerId.Of(new Guid("e6e99a13-d3a8-4d03-b391-df5d720b2813")), "Bob", "bob@lox.com"),
                Customer.Create(CustomerId.Of(new Guid("a236ec92-aba6-4d48-b11a-de0fdb5c7d26")), "Ivan", "ebln@mail.ru")
            };

        public static IEnumerable<Product> Products =>
           new List<Product>
           {
                Product.Create(ProductId.Of(new Guid("386cc98b-0469-44a9-b0ae-ff0961427d03")), "Google Pixel", 5000),
                Product.Create(ProductId.Of(new Guid("778f0398-ff17-4123-9dad-9697c90043ee")), "IPhone", 228),
                Product.Create(ProductId.Of(new Guid("2171404d-04be-419a-8ec9-efd33f4c926d")), "Huawei Plus", 1356),
                Product.Create(ProductId.Of(new Guid("f29f0752-82d3-4b1b-9448-f59ab315f312")), "YandexPhone", 1)
           };

        public static IEnumerable<Order> OrderWithItems
        {
            get
            {
                var address1 = Address.Of("Bob", "Dickins", "bob@lox.com", "BNo:1", "Canada", "Vancouver", "V5J");
                var address2 = Address.Of("Ivan", "Axe", "ebln@mail.ru", "A:1", "France", "Paris", "70123");

                var payment1 = Payment.Of("Bob", "2222222222224444", "12/28", "322", "1");
                var payment2 = Payment.Of("Ivan", "8882222222224444", "06/30", "222", "2");

                var order1 = Order.Create(
                    OrderId.Of(Guid.NewGuid()),
                    CustomerId.Of(new Guid("e6e99a13-d3a8-4d03-b391-df5d720b2813")),
                    OrderName.Of("O1"),
                    shippingAddress: address1,
                    billingAddress: address1,
                    payment1);
                order1.Add(ProductId.Of(new Guid("778f0398-ff17-4123-9dad-9697c90043ee")), 1, 228);
                order1.Add(ProductId.Of(new Guid("f29f0752-82d3-4b1b-9448-f59ab315f312")), 2, 1);

                var order2 = Order.Create(
                    OrderId.Of(Guid.NewGuid()),
                    CustomerId.Of(new Guid("a236ec92-aba6-4d48-b11a-de0fdb5c7d26")),
                    OrderName.Of("O2"),
                    shippingAddress: address2,
                    billingAddress: address2,
                    payment2);
                order2.Add(ProductId.Of(new Guid("2171404d-04be-419a-8ec9-efd33f4c926d")), 12, 1356);

                return new List<Order> { order1, order2 };
            }
        }
    }
}
