using BuildingBlocks.CQRS;
using FluentValidation;
using MediatR;

namespace BuildingBlocks.Behaviors
{
    public class ValidationBehavior<TReqest, TResponse>
        (IEnumerable<IValidator<TReqest>> validators)
        : IPipelineBehavior<TReqest, TResponse> 
        where TReqest : ICommand<TResponse>
    {
        public async Task<TResponse> Handle(TReqest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TReqest>(request);

            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if(failures.Any()) 
                throw new ValidationException(failures);

            return await next();
        }
    }
}
