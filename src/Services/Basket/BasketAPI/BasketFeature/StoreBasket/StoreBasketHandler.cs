using BasketAPI.Data;
using Discount.Grps;

namespace BasketAPI.BasketFeature.StoreBasket
{
    public record StoreBasketCommand(ShoppingCart Cart): ICommand<StoreBasketResult>;
    public record StoreBasketResult(string UserName);

    public class StoreBasketCommandValidatior: AbstractValidator<StoreBasketCommand>
    {
        public StoreBasketCommandValidatior()
        {
            RuleFor(x => x.Cart).NotNull().WithMessage("Cart can not be null");
            RuleFor(x => x.Cart.UserName).NotEmpty().WithMessage("UserName is required");
        }
    }

    public class StoreBasketCommandHandler
        (IBasketRepository repository, DiscountProtoService.DiscountProtoServiceClient discountProto) 
        : ICommandHandler<StoreBasketCommand, StoreBasketResult>
    {
        public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
        {
            //comunicate with Discount.Grps and calculate prices of product
            await DeductDiscount(command.Cart, cancellationToken);

            // store basket in db (marten upsert - if exist = update, if not = insert)
            await repository.StoreBasket(command.Cart, cancellationToken);

            return new StoreBasketResult(command.Cart.UserName);
        }

        private async Task DeductDiscount(ShoppingCart cart, CancellationToken cancellationToken)
        {
            //comunicate with Discount.Grps and calculate prices of product
            foreach (var item in cart.Items)
            {
                var coupon = await discountProto.GetDiscountAsync(new GetDiscountRequest { ProductName = item.ProductName }, cancellationToken: cancellationToken);
                item.Price -= coupon.Amount;
            }
        }
    }
}
