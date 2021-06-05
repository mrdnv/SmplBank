using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SmplBank.Application.Requests;
using SmplBank.Application.Requests.Commands;
using SmplBank.Application.Responses;

namespace SmplBank.Application.PipelineBehaviors
{
    public static class PipelineBehaviorRegistor
    {
        public static IServiceCollection RegisterValidationBehaviors(this IServiceCollection services)
        {
            return services
                .RegisterValidationBehaviors<InsertUserCommand>();
        }

        private static IServiceCollection RegisterValidationBehaviors<TRequest>(this IServiceCollection services) where TRequest : BaseRequest
        {
            return services
                .AddTransient<IPipelineBehavior<TRequest, Response>, ValidationBehavior<TRequest>>();
        }

        private static IServiceCollection RegisterValidationBehaviors<TRequest, TResponseValue>(this IServiceCollection services) where TRequest : BaseRequest<TResponseValue>
        {
            return services
                .AddTransient<IPipelineBehavior<TRequest, Response<TResponseValue>>, ValidationBehavior<TRequest, TResponseValue>>();
        }
    }
}
