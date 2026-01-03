using FluentValidation;
using MediatR;

namespace NeoClinic.Application.Common.Behaviours;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
           _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
             .Where(r => !r.IsValid)
             .SelectMany(r => r.Errors)
             .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        var response = await next(cancellationToken);

        return response;
    }
}
