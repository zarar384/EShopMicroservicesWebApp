using MediatR;

namespace BuildingBlocks.CQRS
{
    public interface IComand: ICommand<Unit> 
    { 
    }

    public interface ICommand<out TResponse>: IRequest<TResponse>
    {
    }
}
