using BuildingBlocks.Exceptions;

namespace Ordering.Application.Exceptions
{
    public class OrderNotFoundException : NotFountException
    {
        public OrderNotFoundException(Guid id) : base("Order", id)
        {
        }
    }
}
