using FluentValidation;
using MediatR;
using SmplBank.Application.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Application.PipelineBehaviors
{
    public class ValidationBehavior<TRequest, TResponseValue> : IPipelineBehavior<TRequest, Response<TResponseValue>> 
        where TRequest : IRequest<Response<TResponseValue>>
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            this.validators = validators;
        }

        public async Task<Response<TResponseValue>> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Response<TResponseValue>> next)
        {
            var validationContext = new ValidationContext<TRequest>(request);
            var failures = (await Task.WhenAll(this.validators
                .Select(v => v.ValidateAsync(validationContext, cancellationToken))))
                .Where(v => v != null && !v.IsValid)
                .SelectMany(v => v.Errors);

            if (failures.Any())
            {
                var failureMessages = failures
                    .Select(_ => _.ErrorMessage)
                    .Distinct()
                    .ToArray();

                return Response.Fail<TResponseValue>(failureMessages);
            }

            return await next();
        }
    }

    public class ValidationBehavior<TRequest> : IPipelineBehavior<TRequest, Response>
        where TRequest : IRequest<Response>
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            this.validators = validators;
        }

        public async Task<Response> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Response> next)
        {
            var validationContext = new ValidationContext<TRequest>(request);
            var failures = (await Task.WhenAll(this.validators
                .Select(v => v.ValidateAsync(validationContext, cancellationToken))))
                .Where(v => v != null && !v.IsValid)
                .SelectMany(v => v.Errors);

            if (failures.Any())
            {
                var failureMessages = failures
                    .Select(_ => _.ErrorMessage)
                    .Distinct()
                    .ToArray();

                return Response.Fail(failureMessages);
            }

            return await next();
        }
    }
}
