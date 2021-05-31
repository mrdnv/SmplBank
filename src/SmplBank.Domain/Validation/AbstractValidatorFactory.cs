using SmplBank.Domain.Exception;
using SmplBank.Domain.Validation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmplBank.Domain.Validation
{
    public abstract class AbstractValidatorFactory<T> : IValidatorFactory<T> where T : Entity.Entity
    {
        private readonly Dictionary<Type, object> validatorCollection = new Dictionary<Type, object>();

        protected AbstractValidatorFactory(params object[] validators)
        {
            foreach (var validator in validators)
            {
                var validatorType = validator.GetType();
                var interfaces = validatorType.GetInterfaces();
                var interfaceType = interfaces.FirstOrDefault(type => GenericInterfaceComparer.Equals(type, typeof(IValidator<,>)));

                if (interfaceType == null)
                {
                    throw new ArgumentException("Invalid validator type.");
                }

                validatorCollection.Add(interfaceType, validator);
            }
        }

        public IValidator<T, TDto> Resolve<TDto>() where TDto : class
        {
            var validatorType = typeof(IValidator<T, TDto>);

            if (this.validatorCollection.TryGetValue(validatorType, out var value) &&
                value is IValidator<T, TDto> validator)
            {
                return validator;
            }

            throw new InternalErrorDomainException($"Validator for dto {typeof(TDto).Name} of entity {typeof(T).Name} was not registered.");
        }

        protected void RegisterValidator<TDto>(IValidator<T, TDto> validator) where TDto : class
        {
            var validatorType = validator.GetType();
            this.validatorCollection.Add(validatorType, validator);
        }

        private class GenericInterfaceComparer : IEqualityComparer<Type>
        {
            public static bool Equals(Type x, Type y) 
            {
                IEqualityComparer<Type> comparer = new GenericInterfaceComparer();

                return comparer.Equals(x, y);
            }

            bool IEqualityComparer<Type>.Equals(Type x, Type y)
            {
                var instance = (this as IEqualityComparer<Type>);

                return instance.GetHashCode(x) == instance.GetHashCode(y);
            }

            int IEqualityComparer<Type>.GetHashCode(Type obj)
            {
                unchecked
                {
                    return obj.Name.GetHashCode() ^ obj.Namespace.GetHashCode() ^ obj.Assembly.FullName.GetHashCode();
                }
            }
        }
    }
}
