using SmplBank.Domain.Validation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SmplBank.Domain.Validation
{
    public class ValidatedObject<T> : IValidatedObject<T> where T : class
    {
        private readonly Dictionary<string, object> federatedObjects = new Dictionary<string, object>();

        internal ValidatedObject()
        {

        }

        internal void AddFederatedObject<TFederated>(TFederated obj, Expression<Func<T, object>> predicate)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            this.federatedObjects.Add(GenerateFederatedObjectKey(predicate), obj);
        }

        public TFederated GetFederatedObject<TFederated>(Expression<Func<T, object>> predicate)
        {
            var key = GenerateFederatedObjectKey(predicate);

            return GetFederatedObject<TFederated>(key);
        }

        private TFederated GetFederatedObject<TFederated>(string key)
        {
            if (this.federatedObjects.TryGetValue(key, out var obj) &&
                obj is TFederated federatedObject)
            {
                return federatedObject;
            }

            return default;
        }

        private string GenerateFederatedObjectKey(Expression<Func<T, object>> predicate = null)
        {
            if (predicate == null)
                return typeof(T).Name;

            if ((predicate.Body is MemberExpression) ||
                (predicate.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression))
            {
                var body = (predicate.Body as MemberExpression) ?? 
                    ((predicate.Body as UnaryExpression).Operand as MemberExpression);

                var memberExpression = GetMemberExpression(body);
                var propertyName = memberExpression.Member.Name;

                return $"{typeof(T).Name}.{propertyName}";
            }
                
            throw new ArgumentException($"Invalid expression", nameof(predicate));
        }
            

        private MemberExpression GetMemberExpression(MemberExpression memberExpression)
        {
            while (memberExpression.Expression is MemberExpression nestedExpression)
            {
                memberExpression = nestedExpression;
            }

            return memberExpression;
        }

    }
}
