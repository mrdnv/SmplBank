using System;
using System.Linq.Expressions;

namespace SmplBank.Domain.Validation.Interfaces
{
    public interface IValidatedObject<T>
    {
        TFederated GetFederatedObject<TFederated>();
        TFederated GetFederatedObject<TFederated>(Expression<Func<T, object>> predicate);
    }
}
